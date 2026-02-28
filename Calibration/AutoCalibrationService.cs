using System;
using System.Threading;
using System.Threading.Tasks;
using 五通道自动测试.Instruments;

namespace 五通道自动测试.Calibration
{
    public class CalibrationRequest
    {
        public int Channel { get; set; }
        public int FrequencyIndex { get; set; }
        public int TemperatureIndex { get; set; }
        public string Mode { get; set; } = "normal";
        public double TargetAmplitude { get; set; }
        public double TargetPhase { get; set; }
        public byte InitialDacHigh { get; set; }
        public byte InitialDacLow { get; set; }
        public byte InitialXndHigh { get; set; }
        public byte InitialXndLow { get; set; }
    }

    public class CalibrationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public int Iterations { get; set; }
        public double FinalAmplitude { get; set; }
        public double FinalPhase { get; set; }
        public byte DacHigh { get; set; }
        public byte DacLow { get; set; }
        public byte XndHigh { get; set; }
        public byte XndLow { get; set; }
    }

    public class AutoCalibrationService
    {
        private readonly ZNB8 _znb8;
        private readonly TemperatureSerialPort _tempSerialPort;
        private readonly CalibrationLogic _calibrationLogic;
        private readonly CalibrationAddressCalculator8 _addressCalculator8;
        private readonly string _channelMode;

        private const byte MIN_CALIBRATION_VALUE = 0x01;
        private const byte MAX_DAC_HIGH_NORMAL = 0x0F;
        private const byte MAX_DAC_HIGH_ANTENNA = 0xFF;
        private const byte MAX_XND_LOW = 0xFF;

        public int MaxIterations { get; set; } = 50;
        public double AmplitudeTolerance { get; set; } = 0.2;
        public double PhaseTolerance { get; set; } = 0.4;
        public int PostWriteDelayMs { get; set; } = 1000;

        public event Action<string>? LogMessage;
        public event Action<int, int>? ProgressChanged;
        public event Action<string, byte>? ParameterWritten;

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

        private byte GetMaxDacHigh(string mode)
        {
            return mode == "antenna" ? MAX_DAC_HIGH_ANTENNA : MAX_DAC_HIGH_NORMAL;
        }

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

        private byte ReadCurrentValue(ushort address)
        {
            if (address == 0) return 0;
            _tempSerialPort.ReadEEPROM(address);
            Thread.Sleep(50);
            return 0;
        }

        private class AmplitudeResult
        {
            public bool Success { get; set; }
            public string Message { get; set; } = "";
            public int Iterations { get; set; }
            public double FinalValue { get; set; }
            public byte FinalDacHigh { get; set; }
            public byte FinalDacLow { get; set; }
        }

        private class PhaseResult
        {
            public bool Success { get; set; }
            public string Message { get; set; } = "";
            public int Iterations { get; set; }
            public double FinalValue { get; set; }
            public byte FinalXndHigh { get; set; }
            public byte FinalXndLow { get; set; }
        }

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
