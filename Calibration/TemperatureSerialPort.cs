using System;
using System.IO.Ports;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace 五通道自动测试.Calibration
{
    /// <summary>
    /// 温度串口管理类，专门用于处理温度传感器的串口通信
    /// 独立管理温度串口，与工装板串口互不干扰
    /// </summary>
    public class TemperatureSerialPort : IDisposable
    {
        private SerialPort? _serialPort;
        private float _latestTemperature;
        private readonly object _temperatureLock = new object();
        private readonly object _lockObject = new object(); // 线程安全锁
        private byte[]? _lastEepromResponse;
        private readonly AutoResetEvent _eepromResponseEvent = new AutoResetEvent(false);
        private bool _disposed = false; // 资源是否已释放的标志

        /// <summary>
        /// 温度更新事件，当收到新的温度数据时触发
        /// </summary>
        public event Action<float>? TemperatureUpdated;
        
        /// <summary>
        /// 日志事件，当有串口相关信息需要记录时触发
        /// </summary>
        public event Action<string>? LogMessage;
        
        /// <summary>
        /// EEPROM数据接收事件，当收到EEPROM响应时触发
        /// </summary>
        public event Action<ushort, byte>? EepromDataReceived;

        /// <summary>
        /// 初始化温度串口管理器
        /// </summary>
        public TemperatureSerialPort()
        {
            _latestTemperature = 0;
        }

        /// <summary>
        /// 打开温度串口
        /// </summary>
        /// <param name="portName">串口名称</param>
        /// <returns>是否成功打开串口</returns>
        public bool OpenPort(string portName)
        {
            try
            {
                if (_serialPort?.IsOpen == true)
                {
                    ClosePort();
                }

                _serialPort = new SerialPort(portName, 115200)
                {
                    DataBits = 8,
                    StopBits = StopBits.One,
                    Parity = Parity.None,
                    Handshake = Handshake.None,
                    ReadTimeout = 1000,
                    WriteTimeout = 1000,
                    ReceivedBytesThreshold = 2
                };

                _serialPort.DataReceived += SerialPort_DataReceived;
                _serialPort.Open();

                // 触发日志事件，记录串口打开成功
                LogMessage?.Invoke($"[{DateTime.Now:HH:mm:ss}] 温度串口 {portName} 已打开");
                
                return true;
            }
            catch (Exception ex)
            {
                // 触发日志事件，记录串口打开失败
                LogMessage?.Invoke($"[{DateTime.Now:HH:mm:ss}] 打开温度串口失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 关闭温度串口
        /// </summary>
        public void ClosePort()
        {
            if (_serialPort?.IsOpen == true)
            {
                string portName = _serialPort.PortName;
                _serialPort.DataReceived -= SerialPort_DataReceived;
                _serialPort.Close();
                _serialPort.Dispose();
                
                // 触发日志事件，记录串口关闭
                LogMessage?.Invoke($"[{DateTime.Now:HH:mm:ss}] 温度串口 {portName} 已关闭");
            }
            
            _serialPort = null;
        }

        /// <summary>
        /// 串口数据接收事件
        /// 处理接收到的温度数据
        /// </summary>
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (_serialPort == null || !_serialPort.IsOpen)
                    return;

                int bytesToRead = _serialPort.BytesToRead;
                while (bytesToRead >= 2)
                {
                    // 检查是否有完整的EEPROM响应帧（8字节）
                    if (bytesToRead >= 8)
                    {
                        byte[] buffer = new byte[8];
                        int bytesRead = _serialPort.Read(buffer, 0, 8);
                        
                        if (bytesRead == 8)
                        {
                            // 检查是否是EEPROM响应帧（帧头AA 55，长度8字节）
                            if (buffer[0] == 0xAA && buffer[1] == 0x55 && buffer[2] == 0x08)
                            {
                                ProcessEepromResponse(buffer);
                                continue;
                            }
                        }
                    }
                    
                    // 处理温度数据
                    byte[] bufferTemp = new byte[2];
                    int bytesReadTemp = _serialPort.Read(bufferTemp, 0, 2);
                    
                    if (bytesReadTemp == 2)
                    {
                        ProcessTemperatureData(bufferTemp);
                    }
                    
                    bytesToRead = _serialPort.BytesToRead;
                }
            }
            catch (Exception)
            {
                // 忽略接收错误
            }
        }

        /// <summary>
        /// 处理EEPROM响应
        /// </summary>
        /// <param name="data">响应数据</param>
        private void ProcessEepromResponse(byte[] data)
        {
            if (data.Length != 8)
                return;
            
            // 解析响应数据
            ushort address = (ushort)((data[4] << 8) | data[5]);
            byte value = data[6];
            
            lock (_lockObject)
            {
                _lastEepromResponse = data;
                _eepromResponseEvent.Set(); // 通知等待中的ReadEEPROM方法
            }
            
            LogMessage?.Invoke($"[{DateTime.Now:HH:mm:ss}] 收到EEPROM响应: {BitConverter.ToString(data).Replace("-", " ")}");
            
            // 触发EEPROM数据接收事件
            EepromDataReceived?.Invoke(address, value);
        }

        /// <summary>
        /// 处理温度传感器原始数据
        /// 原始数据为16位有符号整数，低4位为小数位，实际温度 = 原始值 / 16
        /// </summary>
        /// <param name="data">原始字节数组（2字节）</param>
        private void ProcessTemperatureData(byte[] data)
        {
            if (data.Length != 2)
                return;

            try
            {
                // 过滤无效数据
                if (data[0] == 0xFE && data[1] == 0xFE)
                {
                    return;
                }

                // 转换为16位有符号整数
                short rawValue = (short)((data[0] << 8) | data[1]);

                // 验证数据有效性（温度范围：-60℃ 到 100℃）
                if (rawValue >= -960 && rawValue <= 1600)
                {
                    // 计算实际温度：原始值 / 16.0
                    float temperature = rawValue / 16.0f;
                    
                    lock (_temperatureLock)
                    {
                        _latestTemperature = temperature;
                    }
                    
                    // 触发日志事件，记录温度数据
                    LogMessage?.Invoke($"[{DateTime.Now:HH:mm:ss}] 原始数据: 0x{data[0]:X2} 0x{data[1]:X2}");
                    
                    // 触发温度更新事件
                    TemperatureUpdated?.Invoke(temperature);
                }
                else
                {
                    // 触发日志事件，记录无效温度数据
                    LogMessage?.Invoke($"[{DateTime.Now:HH:mm:ss}] 原始数据: 0x{data[0]:X2} 0x{data[1]:X2}, 无效数据范围");
                }
            }
            catch (Exception ex)
            {
                // 触发日志事件，记录温度数据解析错误
                LogMessage?.Invoke($"[{DateTime.Now:HH:mm:ss}] 温度数据解析错误: {ex.Message}");
            }
        }

        /// <summary>
        /// 读取EEPROM数据
        /// </summary>
        /// <param name="address">16位地址</param>
        /// <returns>读取到的字节值，失败返回null</returns>
        /// <summary>
        /// 读取EEPROM数据
        /// </summary>
        /// <param name="address">16位地址</param>
        /// <returns>读取到的字节值，失败返回null</returns>
        public void ReadEEPROM(ushort address)
        {
            lock (_lockObject)
            {
                try
                {
                    if (_serialPort == null || !_serialPort.IsOpen)
                    {
                        LogMessage?.Invoke($"[{DateTime.Now:HH:mm:ss}] 串口未打开，无法读取EEPROM数据");
                        return;
                    }
                    
                    // 解析地址为高低字节
                    byte addrHigh = (byte)((address >> 8) & 0xFF);
                    byte addrLow = (byte)(address & 0xFF);
                    
                    // 构建读命令帧：AA 55 08 01 [addr_high] [addr_low] 00 ED
                    byte[] readCommand = new byte[]
                    {
                        0xAA, 0x55, 0x08, 0x01,  // 帧头和命令类型
                        addrHigh, addrLow,       // 地址字节
                        0x00, 0xED               // 数据和帧尾
                    };
                    
                    // 清空之前的响应
                    _lastEepromResponse = null;
                    _serialPort.DiscardInBuffer();
                    
                    // 发送命令到串口
                    _serialPort.Write(readCommand, 0, readCommand.Length);
                    LogMessage?.Invoke($"[{DateTime.Now:HH:mm:ss}] 发送EEPROM读命令: {BitConverter.ToString(readCommand).Replace("-", " ")}");
                }
                catch (Exception ex)
                {
                    LogMessage?.Invoke($"[{DateTime.Now:HH:mm:ss}] EEPROM读取失败: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 写入EEPROM数据
        /// </summary>
        /// <param name="address">16位地址</param>
        /// <param name="value">要写入的字节值</param>
        public void WriteEEPROM(ushort address, byte value)
        {
            lock (_lockObject)
            {
                try
                {
                    if (_serialPort == null || !_serialPort.IsOpen)
                    {
                        LogMessage?.Invoke($"[{DateTime.Now:HH:mm:ss}] 串口未打开，无法写入EEPROM数据");
                        return;
                    }
                    
                    // 解析地址为高低字节
                    byte addrHigh = (byte)((address >> 8) & 0xFF);
                    byte addrLow = (byte)(address & 0xFF);
                    
                    // 构建写命令帧：AA 55 08 00 [addr_high] [addr_low] [value] ED
                    byte[] writeCommand = new byte[]
                    {
                        0xAA, 0x55, 0x08, 0x00,  // 帧头和命令类型
                        addrHigh, addrLow,       // 地址字节
                        value,                   // 要写入的数据
                        0xED                     // 帧尾
                    };
                    
                    // 清空接收缓冲区
                    _serialPort.DiscardInBuffer();
                    
                    // 发送命令到串口
                    _serialPort.Write(writeCommand, 0, writeCommand.Length);
                    LogMessage?.Invoke($"[{DateTime.Now:HH:mm:ss}] 发送EEPROM写命令: {BitConverter.ToString(writeCommand).Replace("-", " ")}");
                    
                    // 等待10ms，确保EEPROM有足够时间完成写入操作
                    Thread.Sleep(10);
                    
                    LogMessage?.Invoke($"[{DateTime.Now:HH:mm:ss}] EEPROM写入成功，地址0x{address:X4}，值0x{value:X2}");
                }
                catch (Exception ex)
                {
                    LogMessage?.Invoke($"[{DateTime.Now:HH:mm:ss}] EEPROM写入失败: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 进入ROM模式
        /// 批量操作前调用，提高操作效率和稳定性
        /// </summary>
        /// <returns>操作是否成功</returns>
        public bool EnterROMMode()
        {
            return SendModeCommand(new byte[] { 0xAA, 0x55, 0x09, 0x01, 0xED });
        }

        /// <summary>
        /// 退出ROM模式
        /// 批量操作完成后调用，恢复设备正常工作模式
        /// </summary>
        /// <returns>操作是否成功</returns>
        public bool ExitROMMode()
        {
            return SendModeCommand(new byte[] { 0xAA, 0x55, 0x09, 0x02, 0xED });
        }

        /// <summary>
        /// 停止温度上报
        /// 批量操作前调用，避免温度数据干扰批量读写
        /// </summary>
        /// <returns>操作是否成功</returns>
        public bool StopTemperatureReport()
        {
            return SendModeCommand(new byte[] { 0xAA, 0x55, 0x09, 0xFF, 0xED });
        }

        /// <summary>
        /// 恢复温度上报
        /// 批量操作完成后调用，恢复正常的温度监控
        /// </summary>
        /// <returns>操作是否成功</returns>
        public bool ResumeTemperatureReport()
        {
            return SendModeCommand(new byte[] { 0xAA, 0x55, 0x09, 0x00, 0xED });
        }

        /// <summary>
        /// 发送模式控制命令
        /// 用于发送ROM模式切换、温度上报控制等命令
        /// </summary>
        /// <param name="command">命令字节数组</param>
        /// <returns>操作是否成功</returns>
        private bool SendModeCommand(byte[] command)
        {
            lock (_lockObject)
            {
                try
                {
                    if (_serialPort == null || !_serialPort.IsOpen)
                    {
                        LogMessage?.Invoke($"[{DateTime.Now:HH:mm:ss}] 串口未打开，无法发送模式命令");
                        return false;
                    }

                    // 清空接收缓冲区，避免干扰
                    _serialPort.DiscardInBuffer();

                    // 发送模式控制命令
                    _serialPort.Write(command, 0, command.Length);
                    LogMessage?.Invoke($"[{DateTime.Now:HH:mm:ss}] 发送模式命令: {BitConverter.ToString(command).Replace("-", " ")}");

                    // 等待命令执行完成
                    Thread.Sleep(50);

                    return true;
                }
                catch (Exception ex)
                {
                    LogMessage?.Invoke($"[{DateTime.Now:HH:mm:ss}] 发送模式命令失败: {ex.Message}");
                    return false;
                }
            }
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
                    ClosePort();
                    _eepromResponseEvent.Dispose();
                }

                // 释放非托管资源（如果有）
                // 此处没有需要手动释放的非托管资源

                _disposed = true;
            }
        }

        /// <summary>
        /// 析构函数，确保资源被释放
        /// </summary>
        ~TemperatureSerialPort()
        {
            Dispose(false);
        }
    }
}