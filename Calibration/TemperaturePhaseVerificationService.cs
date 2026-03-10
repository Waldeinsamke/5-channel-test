using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TemperatureChamber;
using 五通道自动测试.Instruments;

namespace 五通道自动测试.Calibration
{
    public class TemperaturePhaseVerificationService
    {
        private readonly InstrumentManager _instrumentManager;
        private readonly ChamberController _chamberController;
        private readonly Form? _parentForm;
        private readonly Func<double>? _getModuleTemperature;
        private readonly Action<string> _logCallback;
        private readonly Action<int, int, double, double> _progressCallback;
        private readonly Action<bool> _powerStateCallback;
        private readonly Action<string, string>? _addResultCallback;
        public string ChannelMode { get; set; } = "CH5";
        public Func<Task>? OnTemperatureStable { get; set; }
        private CancellationTokenSource? _cancellationTokenSource;

        private static readonly Dictionary<double, double> TemperatureMapping = new()
        {
            { -52, -57 },
            { -45, -56 },
            { -35, -46 },
            { -25, -36 },
            { -15, -26 },
            { -5, -16 },
            { 5, -6 },
            { 15, 4 },
            { 25, 14 },
            { 35, 24 },
            { 45, 34 },
            { 55, 44 },
            { 65, 54 },
            { 75, 64 },
            { 85, 74 },
            { 92, 85 }
        };

        private static readonly double[] DefaultSequence1 = { 35, -25, -52, -25, 35, 65, 92, 65, 35 };
        private static readonly double[] DefaultSequence2 = { -52, -45, -35, -25, -15, -5, 5, 15, 25, 35, 45, 55, 65, 75, 85, 92 };
        private static readonly double[] DefaultSequence3 = { 35, -25, -52, -25, 35, 65, 92, 65, 35, -25, -52, -25, 35, 65, 92, 65, 35 };

        private const double TemperatureStabilityThreshold = 0.2;
        private const int StabilityCheckIntervalMs = 2000;
        private const int StabilityCheckCount = 10;
        private const int CooldownWaitMinutes = 20;
        private const double MinTempWaitHours = 1.0;
        private const double MinTempStartThreshold = -53;
        private const double MaxTempStartThreshold = 92;

        private double _currentModuleTemperature = 0;
        private bool _isCoolingDown = false;
        private bool _wasCoolingDown = false;
        private bool _skipRequested = false;
        private bool _stopRequested = false;

        public bool IsRunning { get; private set; } = false;

        public TemperaturePhaseVerificationService(
            InstrumentManager instrumentManager,
            ChamberController chamberController,
            Form? parentForm,
            Func<double>? getModuleTemperature,
            Action<string> logCallback,
            Action<int, int, double, double> progressCallback,
            Action<bool> powerStateCallback,
            Action<string, string>? addResultCallback = null,
            string channelMode = "CH5")
        {
            _instrumentManager = instrumentManager;
            _chamberController = chamberController;
            _parentForm = parentForm;
            _getModuleTemperature = getModuleTemperature;
            _logCallback = logCallback;
            _progressCallback = progressCallback;
            _powerStateCallback = powerStateCallback;
            _addResultCallback = addResultCallback;
            ChannelMode = channelMode;
        }

        public static double[] GetSequence1() => DefaultSequence1.ToArray();
        public static double[] GetSequence2() => DefaultSequence2.ToArray();
        public static double[] GetSequence3() => DefaultSequence3.ToArray();

        public static double CalculateChamberTemperature(double moduleTemperature)
        {
            if (TemperatureMapping.TryGetValue(moduleTemperature, out double chamberTemp))
            {
                return chamberTemp;
            }
            return moduleTemperature - 11;
        }

        public async Task RunVerificationAsync(double[] sequence, CancellationToken cancellationToken = default)
        {
            if (IsRunning)
            {
                _logCallback("验证服务正在运行中...");
                return;
            }

            if (_chamberController == null)
            {
                _logCallback("温箱控制器未初始化，请检查温箱控制界面");
                throw new InvalidOperationException("温箱控制器未初始化");
            }

            if (!_chamberController.IsConnected)
            {
                _logCallback("温箱未连接，请先在温箱控制界面连接温箱设备");
                throw new InvalidOperationException("温箱未连接");
            }

            IsRunning = true;
            _chamberController.PausePolling();
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var linkedCts = _cancellationTokenSource;

            try
            {
                _logCallback("===== 开始温度相位一致性验证 =====");

                for (int i = 0; i < sequence.Length; i++)
                {
                    try
                    {
                        linkedCts.Token.ThrowIfCancellationRequested();

                        double targetModuleTemp = sequence[i];
                        double targetChamberTemp = CalculateChamberTemperature(targetModuleTemp);

                        _progressCallback(i + 1, sequence.Length, targetModuleTemp, targetChamberTemp);
                        _logCallback($"[{i + 1}/{sequence.Length}] 开始测试：模块温度 {targetModuleTemp}℃ → 温箱设定 {targetChamberTemp}℃");

                        await WaitForTemperatureStableAsync(targetModuleTemp, targetChamberTemp, i == 0, linkedCts.Token);

                        _logCallback($"温度已稳定，开始执行相位读取...");
                        await ExecutePhaseReadingAsync(linkedCts.Token);

                        _logCallback($"步骤 {i + 1} 完成");
                    }
                    catch (OperationCanceledException)
                    {
                        if (_stopRequested)
                        {
                            _logCallback("[停止] 验证已停止");
                            return;
                        }
                        if (_skipRequested)
                        {
                            _logCallback("[跳过] 跳过当前温度点，进入下一个温度点");
                            _skipRequested = false;

                            _cancellationTokenSource?.Cancel();
                            _cancellationTokenSource?.Dispose();
                            _cancellationTokenSource = new CancellationTokenSource();
                            linkedCts = _cancellationTokenSource;

                            continue;
                        }
                        throw;
                    }
                }

                _logCallback("===== 验证测试完成 =====");
            }
            catch (Exception ex)
            {
                _logCallback($"[错误] 测试过程发生错误: {ex.Message}");
                throw;
            }
            finally
            {
                IsRunning = false;
                _skipRequested = false;
                _stopRequested = false;
                _chamberController.ResumePolling();
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;

                try
                {
                    _chamberController.StopDevice();
                    _logCallback("验证完成，温箱已停止运行");

                    _instrumentManager.ODP3063.EnableChannel2Output(false);
                    _instrumentManager.SetPowerState(false);
                    _powerStateCallback?.Invoke(false);
                    _logCallback("验证完成，产品电源已关闭");
                }
                catch (Exception ex)
                {
                    _logCallback($"停止温箱失败: {ex.Message}");
                }
            }
        }

        public void Stop()
        {
            _stopRequested = true;
            _cancellationTokenSource?.Cancel();
            _logCallback("正在停止验证服务...");
        }

        public void SkipCurrentStep()
        {
            _skipRequested = true;
            _cancellationTokenSource?.Cancel();
            _logCallback("已发送跳过当前温度点命令");
        }

        private async Task WaitForTemperatureStableAsync(double targetModuleTemp, double targetChamberTemp, bool isFirstStep, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logCallback($"设置温箱温度：{targetChamberTemp}℃");

            for (int retry = 0; retry < 2; retry++)
            {
                try
                {
                    _chamberController.SetTemperature(targetChamberTemp);
                    break;
                }
                catch (Exception ex)
                {
                    if (retry == 1)
                    {
                        _logCallback($"设置温箱温度失败: {ex.Message}");
                        throw;
                    }
                    _logCallback($"设置温箱温度失败，1秒后重试: {ex.Message}");
                    await Task.Delay(1000, cancellationToken);
                }
            }

            try
            {
                bool isRunning = _chamberController.ReadDeviceStatus();
                if (!isRunning)
                {
                    _chamberController.StartDevice();
                }
                else
                {
                    _logCallback("温箱已在运行中");
                }
            }
            catch (Exception ex)
            {
                _logCallback($"检查温箱状态失败: {ex.Message}，尝试启动温箱...");
                try
                {
                    _chamberController.StartDevice();
                }
                catch
                {
                }
            }

            await Task.Delay(2000, cancellationToken);

            if (!isFirstStep)
            {
                double currentModuleTemp = _getModuleTemperature != null ? _getModuleTemperature() : _chamberController.ReadTemperature() + 11;
                bool isCooling = targetModuleTemp < currentModuleTemp;
                _isCoolingDown = isCooling;
                _wasCoolingDown = isCooling;

                if (isCooling)
                {
                    _logCallback("检测到降温模式，关闭产品供电...");
                    _instrumentManager.ODP3063.EnableChannel2Output(false);
                    _instrumentManager.SetPowerState(false);
                    _powerStateCallback?.Invoke(false);
                }

                if (targetModuleTemp == -52)
                {
                    await HandleMinTemperatureAsync(targetChamberTemp, cancellationToken);
                    return;
                }

                if (targetModuleTemp == 92)
                {
                    await HandleMaxTemperatureAsync(cancellationToken);
                    return;
                }

                if (isCooling)
                {
                    await WaitForChamberReachedAsync(targetChamberTemp, cancellationToken);
                    _logCallback($"温箱已达到 {targetChamberTemp}℃，等待 {CooldownWaitMinutes} 分钟...");
                    await Task.Delay(CooldownWaitMinutes * 60 * 1000, cancellationToken);

                    _logCallback("等待完成，恢复产品供电...");
                    _instrumentManager.ODP3063.EnableChannel2Output(true);
                    _instrumentManager.SetPowerState(true);
                    _powerStateCallback?.Invoke(true);
                }
            }

            await WaitForModuleTemperatureStableAsync(targetModuleTemp, cancellationToken);
        }

        private async Task HandleMinTemperatureAsync(double targetChamberTemp, CancellationToken cancellationToken)
        {
            _logCallback($"最低温特殊处理：温箱设定 {targetChamberTemp}℃，等待 {MinTempWaitHours} 小时后供电...");

            await Task.Delay((int)(MinTempWaitHours * 60 * 60 * 1000), cancellationToken);

            _logCallback("恢复产品供电...");
            _instrumentManager.ODP3063.EnableChannel2Output(true);
            _instrumentManager.SetPowerState(true);
            _powerStateCallback?.Invoke(true);

            _logCallback("等待模块温度到达 -53℃...");
            while (!cancellationToken.IsCancellationRequested)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    double currentTemp = _chamberController.ReadTemperature();
                    if (currentTemp <= MinTempStartThreshold)
                    {
                        _logCallback($"模块温度已到达 {currentTemp}℃，开始测试");
                        break;
                    }
                }
                catch (Exception ex)
                {
                    _logCallback($"读取温箱温度失败: {ex.Message}，1秒后重试...");
                }
                await Task.Delay(5000, cancellationToken);
            }
        }

        private async Task HandleMaxTemperatureAsync(CancellationToken cancellationToken)
        {
            _logCallback("最高温特殊处理：温箱设定 85℃，等待模块温度到达 92℃...");

            while (!cancellationToken.IsCancellationRequested)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    double currentTemp = _getModuleTemperature != null ? _getModuleTemperature() : _chamberController.ReadTemperature() + 11;
                    if (currentTemp >= MaxTempStartThreshold)
                    {
                        _logCallback($"模块温度已到达 {currentTemp}℃，开始测试");
                        break;
                    }
                }
                catch (Exception ex)
                {
                    _logCallback($"读取模块温度失败: {ex.Message}，1秒后重试...");
                }
                await Task.Delay(5000, cancellationToken);
            }
        }

        private async Task WaitForChamberReachedAsync(double targetChamberTemp, CancellationToken cancellationToken)
        {
            _logCallback($"等待温箱温度到达 {targetChamberTemp}℃...");

            while (!cancellationToken.IsCancellationRequested)
            {
                cancellationToken.ThrowIfCancellationRequested();

                double currentTemp = 0;
                bool readSuccess = false;

                for (int retry = 0; retry < 2; retry++)
                {
                    try
                    {
                        currentTemp = _chamberController.ReadTemperature();
                        readSuccess = true;
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (retry == 1)
                        {
                            _logCallback($"读取温箱温度失败: {ex.Message}");
                        }
                        await Task.Delay(1000, cancellationToken);
                    }
                }

                if (!readSuccess)
                {
                    continue;
                }

                if (Math.Abs(currentTemp - targetChamberTemp) <= 1)
                {
                    _logCallback($"温箱已达到 {currentTemp}℃");
                    return;
                }
                await Task.Delay(5000, cancellationToken);
            }
        }

        private async Task WaitForModuleTemperatureStableAsync(double targetModuleTemp, CancellationToken cancellationToken)
        {
            _logCallback($"等待模块温度稳定在 {targetModuleTemp}℃ 附近...");

            if (_getModuleTemperature != null)
            {
                _logCallback("（使用温度传感器实际温度）");
            }
            else
            {
                _logCallback("（使用温箱温度+11℃计算）");
            }

            List<double> temperatureHistory = new List<double>();
            bool lastWasStable = false;

            while (!cancellationToken.IsCancellationRequested)
            {
                cancellationToken.ThrowIfCancellationRequested();

                double moduleTemp;

                if (_getModuleTemperature != null)
                {
                    moduleTemp = _getModuleTemperature();
                }
                else
                {
                    moduleTemp = _chamberController.ReadTemperature() + 11;
                }
                _currentModuleTemperature = moduleTemp;

                temperatureHistory.Add(moduleTemp);
                if (temperatureHistory.Count > StabilityCheckCount)
                {
                    temperatureHistory.RemoveAt(0);
                }

                if (temperatureHistory.Count >= StabilityCheckCount)
                {
                    double maxTemp = temperatureHistory.Max();
                    double minTemp = temperatureHistory.Min();
                    double variation = maxTemp - minTemp;

                    double avgModuleTemp = temperatureHistory.Average();
                    double diffFromTarget = targetModuleTemp - avgModuleTemp;

                    bool isStable = variation < TemperatureStabilityThreshold && diffFromTarget <= 1;

                    if (isStable)
                    {
                        _logCallback($"模块温度稳定：当前 {avgModuleTemp:F1}℃，变化 {variation:F2}℃，与目标差 {diffFromTarget:F1}℃");
                        return;
                    }

                    if (!lastWasStable || variation > 1)
                    {
                        _logCallback($"模块温度变化中：{minTemp:F1}℃ ~ {maxTemp:F1}℃，变化 {variation:F2}℃，与目标差 {diffFromTarget:F1}℃");
                    }
                    lastWasStable = false;
                }

                await Task.Delay(StabilityCheckIntervalMs, cancellationToken);
            }
        }

        private async Task ExecutePhaseReadingAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logCallback("温度已稳定，触发验证回调...");
                if (OnTemperatureStable != null)
                {
                    await OnTemperatureStable();
                }
                _logCallback("验证回调已执行完成");
            }
            catch (OperationCanceledException)
            {
                _logCallback("相位读取已取消");
            }
            catch (Exception ex)
            {
                _logCallback($"相位读取失败: {ex.Message}");
            }
        }
    }
}
