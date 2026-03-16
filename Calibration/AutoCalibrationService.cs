// AutoCalibrationService.cs 自动校准核心算法
/// <summary>
/// 五通道自动测试系统的校准模块
/// 包含自动校准服务及相关数据模型
/// </summary>
using System;
using System.Threading;
using System.Threading.Tasks;
using 五通道自动测试.Instruments;

namespace 五通道自动测试.Calibration
{
    /// <summary>
    /// 校准请求数据类
    /// 封装一次校准操作所需的参数
    /// </summary>
    public class CalibrationRequest
    {
        /// <summary>通道号 (0-4)</summary>
        public int Channel { get; set; }
        /// <summary>频点索引</summary>
        public int FrequencyIndex { get; set; }
        /// <summary>温度索引</summary>
        public int TemperatureIndex { get; set; }
        /// <summary>校准模式: normal-正常模式, antenna-天线模式</summary>
        public string Mode { get; set; } = "normal";
        /// <summary>目标幅度值 (dB)</summary>
        public double TargetAmplitude { get; set; }
        /// <summary>目标相位值 (度)</summary>
        public double TargetPhase { get; set; }
        /// <summary>初始DAC高位值</summary>
        public byte InitialDacHigh { get; set; }
        /// <summary>初始DAC低位值</summary>
        public byte InitialDacLow { get; set; }
        /// <summary>初始XND高位值</summary>
        public byte InitialXndHigh { get; set; }
        /// <summary>初始XND低位值</summary>
        public byte InitialXndLow { get; set; }
    }

    /// <summary>
    /// 校准结果数据类
    /// 封装校准操作的结果信息
    /// </summary>
    public class CalibrationResult
    {
        /// <summary>校准是否成功</summary>
        public bool Success { get; set; }
        /// <summary>结果消息或错误信息</summary>
        public string Message { get; set; } = "";
        /// <summary>迭代次数</summary>
        public int Iterations { get; set; }
        /// <summary>最终幅度值 (dB)</summary>
        public double FinalAmplitude { get; set; }
        /// <summary>最终相位值 (度)</summary>
        public double FinalPhase { get; set; }
        /// <summary>最终DAC高位值</summary>
        public byte DacHigh { get; set; }
        /// <summary>最终DAC低位值</summary>
        public byte DacLow { get; set; }
        /// <summary>最终XND高位值</summary>
        public byte XndHigh { get; set; }
        /// <summary>最终XND低位值</summary>
        public byte XndLow { get; set; }
    }

    /// <summary>
    /// 自动校准服务
    /// 负责根据用户输入的校准参数，自动进行校准操作
    /// 封装所有与自动校准相关的逻辑
    /// </summary>
    public class AutoCalibrationService
    {
        /// <summary>矢量网络分析仪 (ZNB8)</summary>
        private readonly ZNB8 _znb8;
        /// <summary>温度串口通信端口</summary>
        private readonly TemperatureSerialPort _tempSerialPort;
        /// <summary>校准逻辑处理类</summary>
        private readonly CalibrationLogic _calibrationLogic;
        /// <summary>CH8通道地址计算器</summary>
        private readonly CalibrationAddressCalculator8 _addressCalculator8;
        /// <summary>当前通道模式 (如 "CH8" 或其他)</summary>
        private readonly string _channelMode;

        /// <summary>最小校准值</summary>
        private const byte MIN_CALIBRATION_VALUE = 0x01;
        /// <summary>正常模式DAC高位最大值</summary>
        private const byte MAX_DAC_HIGH_NORMAL = 0x0F;
        /// <summary>天线模式DAC高位最大值</summary>
        private const byte MAX_DAC_HIGH_ANTENNA = 0xFF;
        /// <summary>XND低位最大值</summary>
        private const byte MAX_XND_LOW = 0xFF;

        /// <summary>最大迭代次数</summary>
        public int MaxIterations { get; set; } = 50;
        /// <summary>幅度收敛容差 (dB)</summary>
        public double AmplitudeTolerance { get; set; } = 0.2;
        /// <summary>相位收敛容差 (度)</summary>
        public double PhaseTolerance { get; set; } = 0.4;
        /// <summary>参数写入后的延迟时间 (毫秒)</summary>
        public int PostWriteDelayMs { get; set; } = 1000;

        /// <summary>日志消息事件</summary>
        public event Action<string>? LogMessage;
        /// <summary>进度变化事件 (当前步骤, 总步骤数)</summary>
        public event Action<int, int>? ProgressChanged;
        /// <summary>参数写入事件 (参数名, 参数值)</summary>
        public event Action<string, byte>? ParameterWritten;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="znb8">矢量网络分析仪实例</param>
        /// <param name="tempSerialPort">温度串口通信实例</param>
        /// <param name="calibrationLogic">校准逻辑处理类实例</param>
        /// <param name="addressCalculator8">CH8通道地址计算器实例</param>
        /// <param name="channelMode">通道模式标识</param>
        public AutoCalibrationService(
            ZNB8 znb8,
            TemperatureSerialPort tempSerialPort,
            CalibrationLogic calibrationLogic,
            CalibrationAddressCalculator8 addressCalculator8,
            string channelMode)
        {
            _znb8 = znb8;
            _tempSerialPort = tempSerialPort;
            _calibrationLogic = calibrationLogic;
            _addressCalculator8 = addressCalculator8;
            _channelMode = channelMode;
        }

        /// <summary>
        /// 获取指定模式下的最大DAC高位值
        /// </summary>
        /// <param name="mode">校准模式 (normal/antenna)</param>
        /// <returns>最大DAC高位值</returns>
        private byte GetMaxDacHigh(string mode)
        {
            return mode == "antenna" ? MAX_DAC_HIGH_ANTENNA : MAX_DAC_HIGH_NORMAL;
        }

        /// <summary>
        /// 执行自动校准的异步入口方法
        /// 先进行幅度校准，再进行相位校准
        /// </summary>
        /// <param name="request">校准请求参数</param>
        /// <returns>校准结果</returns>
        public async Task<CalibrationResult> CalibrateAsync(CalibrationRequest request)
        {
            var result = new CalibrationResult();

            try
            {
                LogMessage?.Invoke($"开始校准: 通道={request.Channel}, 频点={request.FrequencyIndex}, 模式={request.Mode}");

                CalculateAddresses(request);

                ushort addrDacHigh, addrDacLow, addrXndHigh, addrXndLow;
                GetAddresses(request, out addrDacHigh, out addrDacLow, out addrXndHigh, out addrXndLow);

                byte currentDacHigh = request.InitialDacHigh;
                byte currentDacLow = request.InitialDacLow;
                byte currentXndHigh = request.InitialXndHigh;
                byte currentXndLow = request.InitialXndLow;

                ParameterWritten?.Invoke("DacHigh", currentDacHigh);
                ParameterWritten?.Invoke("DacLow", currentDacLow);
                ParameterWritten?.Invoke("XndHigh", currentXndHigh);
                ParameterWritten?.Invoke("XndLow", currentXndLow);

                LogMessage?.Invoke($"初始参数: DAC High={currentDacHigh}, DAC Low={currentDacLow}, XND High={currentXndHigh}, XND Low={currentXndLow}");

                ProgressChanged?.Invoke(1, 2);
                var amplitudeResult = await CalibrateAmplitudeAsync(
                    request, addrDacHigh, addrDacLow, currentDacHigh, currentDacLow);

                if (!amplitudeResult.Success)
                {
                    result.Success = false;
                    result.Message = amplitudeResult.Message;
                    return result;
                }

                currentDacHigh = amplitudeResult.FinalDacHigh;
                currentDacLow = amplitudeResult.FinalDacLow;

                ProgressChanged?.Invoke(2, 2);
                var phaseResult = await CalibratePhaseAsync(
                    request, addrXndHigh, addrXndLow, currentXndHigh, currentXndLow);

                if (!phaseResult.Success)
                {
                    result.Success = false;
                    result.Message = phaseResult.Message;
                    return result;
                }

                result.Success = true;
                result.Message = "校准完成";
                result.Iterations = amplitudeResult.Iterations + phaseResult.Iterations;
                result.FinalAmplitude = amplitudeResult.FinalValue;
                result.FinalPhase = phaseResult.FinalValue;
                result.DacHigh = currentDacHigh;
                result.DacLow = currentDacLow;
                result.XndHigh = phaseResult.FinalXndHigh;
                result.XndLow = phaseResult.FinalXndLow;

                LogMessage?.Invoke($"校准完成: 幅度={result.FinalAmplitude}, 相位={result.FinalPhase}, 迭代次数={result.Iterations}");
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"校准异常: {ex.Message}";
                LogMessage?.Invoke($"校准异常: {ex.Message}");
            }

            return result;
        }

        /// <summary>
        /// 计算校准地址
        /// 根据通道模式和校准模式，计算EEPROM中存储校准参数的地址
        /// </summary>
        /// <param name="request">校准请求参数</param>
        private void CalculateAddresses(CalibrationRequest request)
        {
            if (_channelMode == "CH8")
            {
                if (request.Mode == "antenna")
                {
                    _addressCalculator8.CalculateAntennaAddresses(request.FrequencyIndex, request.TemperatureIndex);
                }
                else
                {
                    _addressCalculator8.CalculateNormalAddresses(request.FrequencyIndex, request.TemperatureIndex);
                }
            }
            else
            {
                if (request.Mode == "antenna")
                {
                    _calibrationLogic.CalculateAntennaAddresses(request.FrequencyIndex, request.TemperatureIndex);
                }
                else
                {
                    _calibrationLogic.CalculateNormalAddresses(request.FrequencyIndex, request.TemperatureIndex);
                }
            }
        }

        /// <summary>
        /// 获取EEPROM地址
        /// 根据请求参数获取DAC和XND的EEPROM存储地址
        /// </summary>
        /// <param name="request">校准请求参数</param>
        /// <param name="addrDacHigh">输出: DAC高位地址</param>
        /// <param name="addrDacLow">输出: DAC低位地址</param>
        /// <param name="addrXndHigh">输出: XND高位地址</param>
        /// <param name="addrXndLow">输出: XND低位地址</param>
        private void GetAddresses(CalibrationRequest request, out ushort addrDacHigh, out ushort addrDacLow, out ushort addrXndHigh, out ushort addrXndLow)
        {
            addrDacHigh = 0;
            addrDacLow = 0;
            addrXndHigh = 0;
            addrXndLow = 0;

            if (_channelMode == "CH8")
            {
                int indexDacHigh = _addressCalculator8.GetNormalAddressIndex(request.Channel, 1);
                int indexDacLow = _addressCalculator8.GetNormalAddressIndex(request.Channel, 2);
                int indexXndHigh = _addressCalculator8.GetNormalAddressIndex(request.Channel, 3);
                int indexXndLow = _addressCalculator8.GetNormalAddressIndex(request.Channel, 4);

                addrDacHigh = (ushort)_addressCalculator8.GetNormalAddressDecimal(indexDacHigh);
                addrDacLow = (ushort)_addressCalculator8.GetNormalAddressDecimal(indexDacLow);
                addrXndHigh = (ushort)_addressCalculator8.GetNormalAddressDecimal(indexXndHigh);
                addrXndLow = (ushort)_addressCalculator8.GetNormalAddressDecimal(indexXndLow);

                if (request.Mode == "antenna")
                {
                    int antIndexDacHigh = _addressCalculator8.GetAntennaAddressIndex(request.Channel, 1);
                    int antIndexXndHigh = _addressCalculator8.GetAntennaAddressIndex(request.Channel, 3);
                    int antIndexXndLow = _addressCalculator8.GetAntennaAddressIndex(request.Channel, 4);

                    addrDacHigh = (ushort)_addressCalculator8.GetAntennaAddressDecimal(antIndexDacHigh);
                    addrXndHigh = (ushort)_addressCalculator8.GetAntennaAddressDecimal(antIndexXndHigh);
                    addrXndLow = (ushort)_addressCalculator8.GetAntennaAddressDecimal(antIndexXndLow);
                    addrDacLow = 0;
                }
            }
            else
            {
                int startIndex = _calibrationLogic.GetStartIndexByChannel(request.Channel);
                addrDacHigh = _calibrationLogic.ParseAddressString(_calibrationLogic.GetNormalAddress(startIndex));
                addrDacLow = _calibrationLogic.ParseAddressString(_calibrationLogic.GetNormalAddress(startIndex + 1));
                addrXndHigh = _calibrationLogic.ParseAddressString(_calibrationLogic.GetNormalAddress(startIndex + 2));
                addrXndLow = _calibrationLogic.ParseAddressString(_calibrationLogic.GetNormalAddress(startIndex + 3));

                if (request.Mode == "antenna")
                {
                    int antStartIndex = _calibrationLogic.GetStartIndexByChannel(request.Channel);
                    addrDacHigh = _calibrationLogic.ParseAddressString(_calibrationLogic.GetAntennaAddress(antStartIndex));
                    addrXndHigh = _calibrationLogic.ParseAddressString(_calibrationLogic.GetAntennaAddress(antStartIndex + 2));
                    addrXndLow = _calibrationLogic.ParseAddressString(_calibrationLogic.GetAntennaAddress(antStartIndex + 3));
                    addrDacLow = 0;
                }
            }
        }

        /// <summary>
        /// 幅度校准内部结果类
        /// </summary>
        private class AmplitudeResult
        {
            /// <summary>校准是否成功</summary>
            public bool Success { get; set; }
            /// <summary>结果消息或错误信息</summary>
            public string Message { get; set; } = "";
            /// <summary>迭代次数</summary>
            public int Iterations { get; set; }
            /// <summary>最终幅度值</summary>
            public double FinalValue { get; set; }
            /// <summary>最终DAC高位值</summary>
            public byte FinalDacHigh { get; set; }
            /// <summary>最终DAC低位值</summary>
            public byte FinalDacLow { get; set; }
        }

        /// <summary>
        /// 相位校准内部结果类
        /// </summary>
        private class PhaseResult
        {
            /// <summary>校准是否成功</summary>
            public bool Success { get; set; }
            /// <summary>结果消息或错误信息</summary>
            public string Message { get; set; } = "";
            /// <summary>迭代次数</summary>
            public int Iterations { get; set; }
            /// <summary>最终相位值</summary>
            public double FinalValue { get; set; }
            /// <summary>最终XND高位值</summary>
            public byte FinalXndHigh { get; set; }
            /// <summary>最终XND低位值</summary>
            public byte FinalXndLow { get; set; }
        }

        /// <summary>
        /// 幅度校准异步方法
        /// 通过迭代调整DAC参数，使幅度值收敛到目标值
        /// </summary>
        /// <param name="request">校准请求参数</param>
        /// <param name="addrDacHigh">DAC高位EEPROM地址</param>
        /// <param name="addrDacLow">DAC低位EEPROM地址</param>
        /// <param name="initialDacHigh">初始DAC高位值</param>
        /// <param name="initialDacLow">初始DAC低位值</param>
        /// <returns>幅度校准结果</returns>
        private async Task<AmplitudeResult> CalibrateAmplitudeAsync(
            CalibrationRequest request,
            ushort addrDacHigh,
            ushort addrDacLow,
            byte initialDacHigh,
            byte initialDacLow)
        {
            var result = new AmplitudeResult();

            LogMessage?.Invoke("开始幅度校准...");

            _znb8.SelectTrace2();
            await Task.Delay(100);
            double initialValue = _znb8.ReadTrace2Mark();

            byte dacHigh = initialDacHigh;
            byte dacLow = initialDacLow;

            byte maxDacHigh = GetMaxDacHigh(request.Mode);
            int increaseDirection = 0;

            LogMessage?.Invoke($"初始幅度值: {initialValue:F2}");

            if (Math.Abs(initialValue - request.TargetAmplitude) <= AmplitudeTolerance)
            {
                result.Success = true;
                result.Message = "幅度已收敛";
                result.Iterations = 0;
                result.FinalValue = initialValue;
                result.FinalDacHigh = dacHigh;
                result.FinalDacLow = dacLow;
                return result;
            }

            LogMessage?.Invoke("确定校准方向...");
            byte testDacHigh = (byte)Math.Min(maxDacHigh, Math.Max(MIN_CALIBRATION_VALUE, (int)dacHigh + 1));
            if (testDacHigh != dacHigh && addrDacHigh != 0)
            {
                _tempSerialPort.WriteEEPROM(addrDacHigh, testDacHigh);
                ParameterWritten?.Invoke("DacHigh", testDacHigh);
                await Task.Delay(PostWriteDelayMs);
                _znb8.SelectTrace2();
                await Task.Delay(100);
                double testValue = _znb8.ReadTrace2Mark();
                increaseDirection = testValue > initialValue ? 1 : -1;
                _tempSerialPort.WriteEEPROM(addrDacHigh, dacHigh);
                ParameterWritten?.Invoke("DacHigh", dacHigh);
                await Task.Delay(PostWriteDelayMs);
                LogMessage?.Invoke($"方向确定: 增大参数 -> {(increaseDirection > 0 ? "幅度增大" : "幅度减小")}");
            }

            double currentValue = initialValue;
            int step = 5;
            int currentDirection = 0;
            double lastError = double.MaxValue;
            int noImprovementCount = 0;

            for (int i = 0; i < MaxIterations; i++)
            {
                double error = request.TargetAmplitude - currentValue;

                if (Math.Abs(error) <= AmplitudeTolerance)
                {
                    result.Success = true;
                    result.Message = "幅度收敛";
                    result.Iterations = i + 1;
                    result.FinalValue = currentValue;
                    result.FinalDacHigh = dacHigh;
                    result.FinalDacLow = dacLow;
                    LogMessage?.Invoke($"幅度校准完成: 目标={request.TargetAmplitude}, 实际={currentValue}, 迭代={i + 1}");
                    return result;
                }

                step = CalculateStepSize(step, lastError, error, ref noImprovementCount, currentDirection);
                currentDirection = (error * increaseDirection > 0) ? 1 : -1;
                step = step * currentDirection;

                LogMessage?.Invoke($"幅度迭代 {i + 1}: 步长={step}, 上次误差={lastError:F2}, 当前误差={error:F2}");

                if (request.Mode == "antenna")
                {
                    dacHigh = (byte)Math.Min(maxDacHigh, Math.Max(MIN_CALIBRATION_VALUE, dacHigh + step));
                }
                else
                {
                    if (increaseDirection > 0)
                    {
                        dacHigh = (byte)Math.Min(maxDacHigh, Math.Max(MIN_CALIBRATION_VALUE, dacHigh + step));
                        dacLow = (byte)Math.Min(255, dacLow + step);
                    }
                    else
                    {
                        dacHigh = (byte)Math.Min(maxDacHigh, Math.Max(MIN_CALIBRATION_VALUE, dacHigh + step));
                        dacLow = (byte)Math.Max(0, dacLow + step);
                    }
                }

                if (addrDacHigh != 0)
                {
                    _tempSerialPort.WriteEEPROM(addrDacHigh, dacHigh);
                    ParameterWritten?.Invoke("DacHigh", dacHigh);
                }
                if (addrDacLow != 0)
                {
                    _tempSerialPort.WriteEEPROM(addrDacLow, dacLow);
                    ParameterWritten?.Invoke("DacLow", dacLow);
                }

                LogMessage?.Invoke($"幅度迭代 {i + 1}: DAC High={dacHigh}, DAC Low={dacLow}, 误差={error:F2}");

                await Task.Delay(PostWriteDelayMs);

                _znb8.SelectTrace2();
                await Task.Delay(100);
                currentValue = _znb8.ReadTrace2Mark();
                lastError = error;
            }

            result.Success = false;
            result.Message = "幅度迭代超限";
            result.Iterations = MaxIterations;
            result.FinalValue = currentValue;
            result.FinalDacHigh = dacHigh;
            result.FinalDacLow = dacLow;

            return result;
        }

        /// <summary>
        /// 相位校准异步方法
        /// 通过迭代调整XND参数，使相位值收敛到目标值
        /// 当XND Low饱和时，自动切换到XND High调整
        /// </summary>
        /// <param name="request">校准请求参数</param>
        /// <param name="addrXndHigh">XND高位EEPROM地址</param>
        /// <param name="addrXndLow">XND低位EEPROM地址</param>
        /// <param name="initialXndHigh">初始XND高位值</param>
        /// <param name="initialXndLow">初始XND低位值</param>
        /// <returns>相位校准结果</returns>
        private async Task<PhaseResult> CalibratePhaseAsync(
            CalibrationRequest request,
            ushort addrXndHigh,
            ushort addrXndLow,
            byte initialXndHigh,
            byte initialXndLow)
        {
            var result = new PhaseResult();

            LogMessage?.Invoke("开始相位校准...");

            _znb8.SelectTrace1();
            await Task.Delay(100);
            double initialValue = _znb8.ReadTrace1Mark();

            byte xndHigh = initialXndHigh;
            byte xndLow = initialXndLow;

            int increaseDirection = 0;

            LogMessage?.Invoke($"初始相位值: {initialValue:F2}");

            if (Math.Abs(initialValue - request.TargetPhase) <= PhaseTolerance)
            {
                result.Success = true;
                result.Message = "相位已收敛";
                result.Iterations = 0;
                result.FinalValue = initialValue;
                result.FinalXndHigh = xndHigh;
                result.FinalXndLow = xndLow;
                return result;
            }

            LogMessage?.Invoke("确定校准方向...");
            byte testXndLow = (byte)Math.Min(MAX_XND_LOW, Math.Max((byte)0, (int)xndLow + 1));
            if (testXndLow != xndLow && addrXndLow != 0)
            {
                _tempSerialPort.WriteEEPROM(addrXndLow, testXndLow);
                ParameterWritten?.Invoke("XndLow", testXndLow);
                await Task.Delay(PostWriteDelayMs);
                _znb8.SelectTrace1();
                await Task.Delay(100);
                double testValue = _znb8.ReadTrace1Mark();
                increaseDirection = testValue > initialValue ? 1 : -1;
                _tempSerialPort.WriteEEPROM(addrXndLow, xndLow);
                ParameterWritten?.Invoke("XndLow", xndLow);
                await Task.Delay(PostWriteDelayMs);
                LogMessage?.Invoke($"方向确定: 增大参数 -> {(increaseDirection > 0 ? "相位增大" : "相位减小")}");
            }

            double currentValue = initialValue;
            bool adjustHigh = false;
            int step = 5;
            int currentDirection = 0;
            double lastError = double.MaxValue;
            int noImprovementCount = 0;

            for (int i = 0; i < MaxIterations; i++)
            {
                double error = request.TargetPhase - currentValue;

                if (Math.Abs(error) <= PhaseTolerance)
                {
                    result.Success = true;
                    result.Message = "相位收敛";
                    result.Iterations = i + 1;
                    result.FinalValue = currentValue;
                    result.FinalXndHigh = xndHigh;
                    result.FinalXndLow = xndLow;
                    LogMessage?.Invoke($"相位校准完成: 目标={request.TargetPhase}, 实际={currentValue}, 迭代={i + 1}");
                    return result;
                }

                step = CalculateStepSize(step, lastError, error, ref noImprovementCount, currentDirection);
                currentDirection = (error * increaseDirection > 0) ? 1 : -1;
                step = step * currentDirection;

                LogMessage?.Invoke($"相位迭代 {i + 1}: 步长={step}, 上次误差={lastError:F2}, 当前误差={error:F2}");

                if (!adjustHigh)
                {
                    int newXndLow = xndLow + step;

                    if (increaseDirection > 0 && newXndLow > MAX_XND_LOW)
                    {
                        adjustHigh = true;
                        xndHigh = (byte)Math.Min(MAX_DAC_HIGH_ANTENNA, Math.Max(MIN_CALIBRATION_VALUE, xndHigh + 1));
                        xndLow = 0x00;
                        LogMessage?.Invoke($"XND Low饱和，切换到High调整: High={xndHigh}, Low={xndLow}");
                    }
                    else if (increaseDirection < 0 && newXndLow < 0)
                    {
                        adjustHigh = true;
                        xndHigh = (byte)Math.Min(MAX_DAC_HIGH_ANTENNA, Math.Max(MIN_CALIBRATION_VALUE, (int)xndHigh - 1));
                        xndLow = MAX_XND_LOW;
                        LogMessage?.Invoke($"XND Low饱和，切换到High调整: High={xndHigh}, Low={xndLow}");
                    }
                    else
                    {
                        xndLow = (byte)Math.Max(0, Math.Min(MAX_XND_LOW, newXndLow));
                    }
                }
                else
                {
                    xndHigh = (byte)Math.Min(MAX_DAC_HIGH_ANTENNA, Math.Max(MIN_CALIBRATION_VALUE, xndHigh + step));

                    if (increaseDirection > 0 && xndHigh >= MAX_DAC_HIGH_ANTENNA && xndLow >= MAX_XND_LOW)
                    {
                        LogMessage?.Invoke($"相位校准无法继续: 参数已到上限");
                        break;
                    }
                    if (increaseDirection < 0 && xndHigh <= MIN_CALIBRATION_VALUE && xndLow <= 0)
                    {
                        LogMessage?.Invoke($"相位校准无法继续: 参数已到下限");
                        break;
                    }
                }

                if (addrXndHigh != 0)
                {
                    _tempSerialPort.WriteEEPROM(addrXndHigh, xndHigh);
                    ParameterWritten?.Invoke("XndHigh", xndHigh);
                }
                if (addrXndLow != 0)
                {
                    _tempSerialPort.WriteEEPROM(addrXndLow, xndLow);
                    ParameterWritten?.Invoke("XndLow", xndLow);
                }

                LogMessage?.Invoke($"相位迭代 {i + 1}: XND High={xndHigh}, XND Low={xndLow}, 误差={error:F2}");

                await Task.Delay(PostWriteDelayMs);

                _znb8.SelectTrace1();
                await Task.Delay(100);
                currentValue = _znb8.ReadTrace1Mark();
                lastError = error;
            }

            result.Success = false;
            result.Message = "相位迭代超限";
            result.Iterations = MaxIterations;
            result.FinalValue = currentValue;
            result.FinalXndHigh = xndHigh;
            result.FinalXndLow = xndLow;

            return result;
        }

        /// <summary>
        /// 计算步长
        /// 根据当前误差与上次误差的比较，动态调整迭代步长
        /// 如果连续两次没有改善，则减小步长
        /// </summary>
        /// <param name="step">当前步长</param>
        /// <param name="lastError">上次误差</param>
        /// <param name="currentError">当前误差</param>
        /// <param name="noImprovementCount">连续未改善次数 (引用)</param>
        /// <param name="currentDirection">当前调整方向</param>
        /// <returns>调整后的步长</returns>
        private int CalculateStepSize(int step, double lastError, double currentError, ref int noImprovementCount, int currentDirection)
        {
            if (lastError != double.MaxValue && Math.Abs(currentError) >= Math.Abs(lastError))
            {
                noImprovementCount++;
            }
            else
            {
                noImprovementCount = 0;
            }

            if (noImprovementCount >= 2)
            {
                noImprovementCount = 0;
                step = Math.Max(1, step - 1);
            }

            return step;
        }
    }
}
