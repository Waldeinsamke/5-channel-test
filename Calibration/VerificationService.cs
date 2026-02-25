using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using 五通道自动测试.Instruments;

namespace 五通道自动测试.Calibration
{
    /// <summary>
    /// 验证测试服务
    /// 负责执行通道验证测试，包括通道切换、频率设置和数据采集
    /// 封装所有与验证测试相关的业务逻辑
    /// </summary>
    public class VerificationService
    {
        private readonly InstrumentManager _instrumentManager;
        private readonly Form _parentForm;
        private readonly Action<string, string> _addResultCallback;
        private readonly Action<string> _logCallback;
        private readonly string _channelMode;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly int _overallTimeoutSeconds = 300;

        private static string GetChannelName(int channelNumber)
        {
            if (channelNumber >= 1 && channelNumber <= 8)
            {
                return ((char)('A' + channelNumber - 1)).ToString();
            }
            return channelNumber.ToString();
        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="instrumentManager">仪器管理器实例</param>
        /// <param name="parentForm">父窗体引用，用于UI线程调用</param>
        /// <param name="addResultCallback">添加测试结果到表格的回调函数</param>
        /// <param name="logCallback">日志记录回调函数</param>
        /// <param name="channelMode">通道模式："CH5"或"CH8"</param>
        public VerificationService(InstrumentManager instrumentManager, Form parentForm, Action<string, string> addResultCallback, Action<string> logCallback, string channelMode = "CH5")
        {
            _instrumentManager = instrumentManager;
            _parentForm = parentForm;
            _addResultCallback = addResultCallback;
            _logCallback = logCallback;
            _channelMode = channelMode;
        }
        
        /// <summary>
        /// 执行完整的验证测试流程
        /// </summary>
        public async Task RunVerificationTest(CancellationToken cancellationToken = default)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var linkedCts = _cancellationTokenSource;
            
            try
            {
                _logCallback("===== 开始验证测试流程 =====");
                
                _logCallback($"[超时保护] 测试总超时时间: {_overallTimeoutSeconds} 秒");
                linkedCts.CancelAfter(_overallTimeoutSeconds * 1000);
                
                await Task.Run(() =>
                {
                    RunVerificationTestSync(linkedCts.Token);
                }, linkedCts.Token);
            }
            catch (OperationCanceledException)
            {
                if (linkedCts.IsCancellationRequested)
                {
                    _logCallback("[超时保护] 测试流程已超时，自动取消");
                    throw new TimeoutException($"测试流程超过 {_overallTimeoutSeconds} 秒，已自动取消");
                }
                _logCallback("[取消] 用户取消了测试流程");
                throw new OperationCanceledException("用户取消了测试流程");
            }
            catch (TimeoutException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logCallback($"测试过程中发生错误: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 同步执行验证测试（内部方法）
        /// </summary>
        private void RunVerificationTestSync(CancellationToken cancellationToken)
        {
            _instrumentManager.ZNB8.Write(":CALCulate1:PARameter:SELect 'Trc1'");
            _logCallback("选择轨迹1: :CALCulate1:PARameter:SELect 'Trc1'");

            int[] channels;
            if (_channelMode == "CH5")
            {
                channels = new int[] { 1, 2, 4, 5 };
                _logCallback($"使用五通道测试模式：测试通道 {GetChannelName(1)}, {GetChannelName(2)}, {GetChannelName(4)}, {GetChannelName(5)}");
            }
            else
            {
                channels = new int[] { 1, 2, 3, 5, 6, 7, 8 };
                _logCallback($"使用八通道测试模式：测试通道 {GetChannelName(1)}, {GetChannelName(2)}, {GetChannelName(3)}, {GetChannelName(5)}, {GetChannelName(6)}, {GetChannelName(7)}, {GetChannelName(8)}");
            }

            double[] frequencies = { 3330.0, 3350.0, 3370.0, 3390.0 };

            foreach (int channel in channels)
            {
                cancellationToken.ThrowIfCancellationRequested();
                TestChannel(channel, frequencies, cancellationToken);
            }

            _logCallback("===== 测试完成 =====");
        }
        
        /// <summary>
        /// 测试单个通道
        /// </summary>
        private void TestChannel(int channel, double[] frequencies, CancellationToken cancellationToken)
        {
            _logCallback($"===== 开始测试通道 {GetChannelName(channel)} =====");

            _instrumentManager.SwitchChannel(channel);
            _logCallback($"切换到通道 {GetChannelName(channel)}");
            Thread.Sleep(800);

            foreach (double frequency in frequencies)
            {
                cancellationToken.ThrowIfCancellationRequested();
                TestFrequency(channel, frequency, cancellationToken);
            }
        }
        
        /// <summary>
        /// 测试单个频率点
        /// </summary>
        private void TestFrequency(int channel, double frequency, CancellationToken cancellationToken)
        {
            _logCallback($"-- 测试频率 {frequency} MHz --");

            _instrumentManager.SendFrequencyCommand(frequency);
            _instrumentManager.SignalGenerator.SetFrequency(frequency);
            _logCallback($"频率切换至 {frequency} MHz");

            Thread.Sleep(2000);

            try
            {
                string yValueResponse = _instrumentManager.ZNB8.Query(":CALCulate1:MARKer1:Y?");
                _logCallback($"读取轨迹1 y值: {yValueResponse}");

                Thread.Sleep(500);

                double yValue;
                if (double.TryParse(yValueResponse, out yValue))
                {
                    string formattedValue = yValue.ToString("F1");
                    SafeAddResult($"通道{GetChannelName(channel)} {frequency}MHz", formattedValue);
                    _logCallback($"解析结果: 通道{GetChannelName(channel)} {frequency}MHz -> {formattedValue}");
                }
                else
                {
                    _logCallback($"注意：返回值为非数值格式，直接显示原始数据: {yValueResponse}");
                    SafeAddResult($"通道{GetChannelName(channel)} {frequency}MHz", yValueResponse);
                }
            }
            catch (TimeoutException ex)
            {
                _logCallback($"[超时] 读取数据超时: {ex.Message}");
                SafeAddResult($"通道{GetChannelName(channel)} {frequency}MHz", "超时");
            }
            catch (Exception ex)
            {
                _logCallback($"读取数据失败: {ex.Message}");
                SafeAddResult($"通道{GetChannelName(channel)} {frequency}MHz", "失败");
            }
        }

        /// <summary>
        /// 线程安全地添加结果到 UI
        /// </summary>
        private void SafeAddResult(string testItem, string value)
        {
            if (_parentForm.InvokeRequired)
            {
                _parentForm.Invoke(new Action(() => _addResultCallback(testItem, value)));
            }
            else
            {
                _addResultCallback(testItem, value);
            }
        }
    }
}