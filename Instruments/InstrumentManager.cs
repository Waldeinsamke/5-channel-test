using System;
using System.Threading.Tasks;
using 五通道自动测试;

namespace 五通道自动测试.Instruments
{
    public class InstrumentManager : IDisposable
    {
        /// <summary>
        /// 电源状态变化事件
        /// </summary>
        public event Action<bool>? PowerStateChanged;
        public SpectrumAnalyzer SpectrumAnalyzer { get; private set; }
        
        /// <summary>
        /// 信号源实例
        /// </summary>
        public SignalGenerator SignalGenerator { get; private set; }
        
        /// <summary>
        /// ZNB8矢量网络分析仪实例
        /// </summary>
        public ZNB8 ZNB8 { get; private set; }
        
        /// <summary>
        /// ODP3063电源实例
        /// </summary>
        public ODP3063 ODP3063 { get; private set; }
        
        /// <summary>
        /// 工装板通信实例
        /// </summary>
        public ToolingBoardCommunicator? ToolingBoard { get; private set; }
        
        /// <summary>
        /// 电源开关状态
        /// </summary>
        private bool _isPowerOn = false;
        
        /// <summary>
        /// 资源是否已释放的标志
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// 串口通信重试次数
        /// </summary>
        private int _serialRetryCount = 3;

        /// <summary>
        /// 串口通信重试间隔（毫秒）
        /// </summary>
        private int _serialRetryDelay = 100;
        
        /// <summary>
        /// 频率到HEX命令的映射字典
        /// </summary>
        private readonly Dictionary<double, byte[]> _frequencyCommands = new()
        {
            { 3330.0, new byte[] { 0xEB, 0x97, 0x13, 0x01, 0x00, 0x96 } },
            { 3350.0, new byte[] { 0xEB, 0x97, 0x13, 0x01, 0x01, 0x97 } },
            { 3370.0, new byte[] { 0xEB, 0x97, 0x13, 0x01, 0x02, 0x98 } },
            { 3390.0, new byte[] { 0xEB, 0x97, 0x13, 0x01, 0x03, 0x99 } }
        };
        
        /// <summary>
        /// 衰减到HEX命令的映射字典
        /// </summary>
        private readonly Dictionary<int, byte[]> _attenuationCommands = new()
        {
            { 0,  new byte[] { 0xEB, 0x97, 0x14, 0x01, 0x0F, 0xA6 } },  // 衰减0
            { 10, new byte[] { 0xEB, 0x97, 0x14, 0x01, 0x0C, 0xA3 } },  // 衰减10
            { 36, new byte[] { 0xEB, 0x97, 0x14, 0x01, 0x00, 0x97 } }   // 衰减36
        };
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public InstrumentManager()
        {
            SpectrumAnalyzer = new SpectrumAnalyzer();
            SignalGenerator = new SignalGenerator();
            ZNB8 = new ZNB8();
            ODP3063 = new ODP3063();
        }
        
        /// <summary>
        /// 连接频谱仪
        /// </summary>
        /// <param name="address">频谱仪地址</param>
        /// <returns>是否连接成功</returns>
        public bool ConnectSpectrumAnalyzer(string address)
        {
            SpectrumAnalyzer.Address = address;
            return SpectrumAnalyzer.Connect();
        }
        
        /// <summary>
        /// 连接信号源
        /// </summary>
        /// <param name="address">信号源地址</param>
        /// <returns>是否连接成功</returns>
        public bool ConnectSignalGenerator(string address)
        {
            SignalGenerator.Address = address;
            return SignalGenerator.Connect();
        }
        
        /// <summary>
        /// 连接工装板
        /// </summary>
        /// <param name="portName">串口名称</param>
        /// <returns>是否连接成功</returns>
        public bool ConnectToolingBoard(string portName)
        {
            ToolingBoard = new ToolingBoardCommunicator(portName);
            bool success = ToolingBoard.Open();
            if (success)
            {
                // 打开串口成功后，自动设置衰减为0dbm
                SendAttenuationCommand(0);
            }
            return success;
        }
        
        /// <summary>
        /// 设置工装板通信实例（已存在的实例）
        /// </summary>
        /// <param name="toolingBoard">工装板通信实例</param>
        public void SetToolingBoard(ToolingBoardCommunicator toolingBoard)
        {
            ToolingBoard = toolingBoard;
            // 设置工装板实例后，自动设置衰减为0dbm
            SendAttenuationCommand(0);
        }
        
        /// <summary>
        /// 断开频谱仪连接
        /// </summary>
        public void DisconnectSpectrumAnalyzer()
        {
            SpectrumAnalyzer.Disconnect();
        }
        
        /// <summary>
        /// 断开信号源连接
        /// </summary>
        public void DisconnectSignalGenerator()
        {
            SignalGenerator.Disconnect();
        }
        
        /// <summary>
        /// 断开工装板连接
        /// </summary>
        public void DisconnectToolingBoard()
        {
            ToolingBoard?.Dispose();
            ToolingBoard = null;
        }
        
        /// <summary>
        /// 重置所有仪表
        /// </summary>
        public void ResetAll()
        {
            SpectrumAnalyzer.Reset();
            SignalGenerator.Reset();
            SpectrumAnalyzer.SetContinuousScan(true);
            Console.WriteLine("[INFO] 所有仪表已复位");
        }
        
        /// <summary>
        /// 断开所有仪表连接
        /// </summary>
        public void DisconnectAll()
        {
            SpectrumAnalyzer.Disconnect();
            SignalGenerator.Disconnect();
            ZNB8.Disconnect();
            ODP3063.Disconnect();
            ToolingBoard?.Dispose();
            ToolingBoard = null;
        }
        
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        /// <summary>
        /// 受保护的Dispose方法，实现资源释放逻辑
        /// </summary>
        /// <param name="disposing">是否释放托管资源</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // 释放托管资源
                    DisconnectAll();
                }
                
                // 释放非托管资源（如果有）
                
                _disposed = true;
            }
        }

        /// <summary>
        /// 获取当前电源开关状态
        /// </summary>
        public bool IsPowerOn
        {
            get
            {
                if (ODP3063.IsConnected)
                {
                    try
                    {
                        return ODP3063.QueryChannel2OutputStatus();
                    }
                    catch
                    {
                        return _isPowerOn;
                    }
                }
                return _isPowerOn;
            }
        }

        /// <summary>
        /// 设置电源开关状态
        /// </summary>
        public void SetPowerState(bool isOn)
        {
            _isPowerOn = isOn;
            PowerStateChanged?.Invoke(_isPowerOn);
        }
        
        /// <summary>
        /// 析构函数，确保资源被释放
        /// </summary>
        ~InstrumentManager()
        {
            Dispose(false);
        }
        
        /// <summary>
        /// 连接ZNB8矢量网络分析仪
        /// </summary>
        /// <param name="address">ZNB8地址</param>
        /// <returns>是否连接成功</returns>
        public bool ConnectZNB8(string address)
        {
            ZNB8.Address = address;
            return ZNB8.Connect();
        }
        
        /// <summary>
        /// 连接ODP3063电源
        /// </summary>
        /// <param name="address">ODP3063地址</param>
        /// <returns>是否连接成功</returns>
        public bool ConnectODP3063(string address)
        {
            ODP3063.Address = address;
            return ODP3063.Connect();
        }
        
        /// <summary>
        /// 断开ZNB8连接
        /// </summary>
        public void DisconnectZNB8()
        {
            ZNB8.Disconnect();
        }
        
        /// <summary>
        /// 断开ODP3063连接
        /// </summary>
        public void DisconnectODP3063()
        {
            ODP3063.Disconnect();
        }
        
        /// <summary>
        /// 检查所有仪表是否连接
        /// </summary>
        /// <returns>是否所有仪表都已连接</returns>
        public bool AreAllInstrumentsConnected()
        {
            return SpectrumAnalyzer.IsConnected && SignalGenerator.IsConnected && ZNB8.IsConnected;
        }
        
        /// <summary>
        /// 切换通道（带重试机制）
        /// </summary>
        /// <param name="channel">通道号(1-5)</param>
        /// <returns>是否发送成功</returns>

        public bool SwitchChannel(int channel)
        {
            if (ToolingBoard == null)
            {
                Console.WriteLine($"[ERROR] 工装板未连接，无法切换通道");
                return false;
            }
            
            Console.WriteLine($"[INFO] 切换到通道 {channel}");
            
            byte dataValue = (byte)(channel - 1);
            byte checksum = (byte)(0x98 + dataValue);
            
            byte[] command = new byte[] 
            {
                0xEB, 0x97, 0x15, 0x01, dataValue, checksum
            };
            
            return SendFrameWithRetry(command, "通道切换");
        }
        
        /// <summary>
        /// 发送工装板频点切换命令（带重试机制）
        /// </summary>
        /// <param name="frequencyMHz">频率（MHz）</param>
        /// <returns>是否发送成功</returns>
        public bool SendFrequencyCommand(double frequencyMHz)
        {
            if (ToolingBoard == null)
            {
                Console.WriteLine($"[ERROR] 工装板未连接，无法发送频率 {frequencyMHz} MHz 的切换命令");
                return false;
            }
            
            if (_frequencyCommands.TryGetValue(frequencyMHz, out byte[]? command))
            {
                string hexString = BitConverter.ToString(command).Replace("-", " ");
                Console.WriteLine($"[INFO] 发送工装板频点切换命令: {frequencyMHz} MHz -> {hexString}");
                bool result = SendFrameWithRetry(command, $"频率切换 {frequencyMHz} MHz");
                Console.WriteLine($"[INFO] 发送结果: {(result ? "成功" : "失败")}");
                return result;
            }
            
            Console.WriteLine($"[ERROR] 未找到频率 {frequencyMHz} MHz 对应的命令");
            return false;
        }

        /// <summary>
        /// 发送帧数据（带重试机制）
        /// </summary>
        /// <param name="frame">要发送的帧数据</param>
        /// <param name="operationName">操作名称（用于日志）</param>
        /// <returns>是否发送成功</returns>
        private bool SendFrameWithRetry(byte[] frame, string operationName)
        {
            int attempt = 0;
            while (attempt < _serialRetryCount)
            {
                attempt++;
                try
                {
                    if (ToolingBoard == null)
                    {
                        Console.WriteLine($"[ERROR] 工装板未连接，{operationName}失败");
                        return false;
                    }

                    if (!ToolingBoard.SendFrame(frame))
                    {
                        if (attempt < _serialRetryCount)
                        {
                            Console.WriteLine($"[WARNING] {operationName}第{attempt}次尝试失败，{_serialRetryDelay}ms后重试");
                            System.Threading.Thread.Sleep(_serialRetryDelay);
                            continue;
                        }
                        return false;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    if (attempt < _serialRetryCount)
                    {
                        Console.WriteLine($"[WARNING] {operationName}第{attempt}次尝试发生异常: {ex.Message}，{_serialRetryDelay}ms后重试");
                        System.Threading.Thread.Sleep(_serialRetryDelay);
                        continue;
                    }
                    Console.WriteLine($"[ERROR] {operationName}最终失败: {ex.Message}");
                    return false;
                }
            }
            return false;
        }
        
        /// <summary>
        /// 发送衰减控制命令
        /// </summary>
        /// <param name="attenuationDb">衰减量（dB）</param>
        /// <returns>是否发送成功</returns>
        public bool SendAttenuationCommand(int attenuationDb)
        {
            if (ToolingBoard == null)
            {
                Console.WriteLine($"[ERROR] 工装板未连接，无法设置衰减 {attenuationDb} dB");
                return false;
            }
            
            if (_attenuationCommands.TryGetValue(attenuationDb, out byte[]? command))
            {
                string hexString = BitConverter.ToString(command).Replace("-", " ");
                Console.WriteLine($"[INFO] 发送衰减控制命令: {attenuationDb} dB -> {hexString}");
                bool result = ToolingBoard.SendFrame(command);
                Console.WriteLine($"[INFO] 发送结果: {(result ? "成功" : "失败")}");
                return result;
            }
            
            Console.WriteLine($"[ERROR] 未找到衰减 {attenuationDb} dB 对应的命令");
            return false;
        }
        
        /// <summary>
        /// 发送校准开关开启命令（FXJZ拉高）
        /// </summary>
        /// <returns>是否发送成功</returns>
        public bool SendCalibrationSwitchHigh()
        {
            if (ToolingBoard == null)
            {
                Console.WriteLine("[ERROR] 工装板未连接，无法开启校准开关");
                return false;
            }
            
            Console.WriteLine("[INFO] 发送校准开关开启命令 (FXJZ拉高)");
            bool result = ToolingBoard.SetCalibrationSwitchHigh();
            Console.WriteLine($"[INFO] 发送结果: {(result ? "成功" : "失败")}");
            return result;
        }
        
        /// <summary>
        /// 发送校准开关关闭命令（FXJZ拉低）
        /// </summary>
        /// <returns>是否发送成功</returns>
        public bool SendCalibrationSwitchLow()
        {
            if (ToolingBoard == null)
            {
                Console.WriteLine("[ERROR] 工装板未连接，无法关闭校准开关");
                return false;
            }
            
            Console.WriteLine("[INFO] 发送校准开关关闭命令 (FXJZ拉低)");
            bool result = ToolingBoard.SetCalibrationSwitchLow();
            Console.WriteLine($"[INFO] 发送结果: {(result ? "成功" : "失败")}");
            return result;
        }
        
        /// <summary>
        /// 发送校准源上电命令
        /// </summary>
        /// <returns>是否发送成功</returns>
        public bool SendCalibrationSourcePowerOn()
        {
            if (ToolingBoard == null)
            {
                Console.WriteLine("[ERROR] 工装板未连接，无法发送校准源上电命令");
                return false;
            }
            
            Console.WriteLine("[INFO] 发送校准源上电命令");
            bool result = ToolingBoard.SetCalibrationSourcePowerOn();
            Console.WriteLine($"[INFO] 发送结果: {(result ? "成功" : "失败")}");
            return result;
        }
        
        /// <summary>
        /// 发送校准源下电命令
        /// </summary>
        /// <returns>是否发送成功</returns>
        public bool SendCalibrationSourcePowerOff()
        {
            if (ToolingBoard == null)
            {
                Console.WriteLine("[ERROR] 工装板未连接，无法发送校准源下电命令");
                return false;
            }
            
            Console.WriteLine("[INFO] 发送校准源下电命令");
            bool result = ToolingBoard.SetCalibrationSourcePowerOff();
            Console.WriteLine($"[INFO] 发送结果: {(result ? "成功" : "失败")}");
            return result;
        }
        
        /// <summary>
        /// 发送天线矢能命令
        /// </summary>
        /// <returns>是否发送成功</returns>
        public bool SendAntennaEnable()
        {
            if (ToolingBoard == null)
            {
                Console.WriteLine("[ERROR] 工装板未连接，无法发送天线矢能命令");
                return false;
            }
            
            Console.WriteLine("[INFO] 发送天线矢能命令");
            bool result = ToolingBoard.SetAntennaEnable();
            Console.WriteLine($"[INFO] 发送结果: {(result ? "成功" : "失败")}");
            return result;
        }
        
        /// <summary>
        /// 发送天线去能命令
        /// </summary>
        /// <returns>是否发送成功</returns>
        public bool SendAntennaDisable()
        {
            if (ToolingBoard == null)
            {
                Console.WriteLine("[ERROR] 工装板未连接，无法发送天线去能命令");
                return false;
            }
            
            Console.WriteLine("[INFO] 发送天线去能命令");
            bool result = ToolingBoard.SetAntennaDisable();
            Console.WriteLine($"[INFO] 发送结果: {(result ? "成功" : "失败")}");
            return result;
        }
        
        /// <summary>
        /// 发送Antenna模式完整命令序列（FXJZ拉高、校准源上电、天线矢能）
        /// </summary>
        /// <returns>是否发送成功</returns>
        public bool SendAntennaModeCommands()
        {
            if (ToolingBoard == null)
            {
                Console.WriteLine("[ERROR] 工装板未连接，无法发送Antenna模式命令");
                return false;
            }
            
            Console.WriteLine("[INFO] 发送Antenna模式完整命令序列");
            bool result = ToolingBoard.SendAntennaModeCommands();
            Console.WriteLine($"[INFO] 发送结果: {(result ? "成功" : "失败")}");
            return result;
        }
        
        /// <summary>
        /// 发送Normal模式完整命令序列（FXJZ拉低、校准源下电、天线去能）
        /// </summary>
        /// <returns>是否发送成功</returns>
        public bool SendNormalModeCommands()
        {
            if (ToolingBoard == null)
            {
                Console.WriteLine("[ERROR] 工装板未连接，无法发送Normal模式命令");
                return false;
            }
            
            Console.WriteLine("[INFO] 发送Normal模式完整命令序列");
            bool result = ToolingBoard.SendNormalModeCommands();
            Console.WriteLine($"[INFO] 发送结果: {(result ? "成功" : "失败")}");
            return result;
        }
    }
}