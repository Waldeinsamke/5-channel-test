using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 五通道自动测试
{
    public class ToolingBoardCommunicator : IDisposable
    {
        private SerialPort _serialPort;
        private string _portName;
        private int _baudRate;
        private Queue<byte> _receiveQueue;
        private readonly object _queueLock = new object();
        private bool _disposed = false;

        public event Action<string> LogMessage;
        public event Action<byte, byte[]> DataReceived;
        public event Action<bool> CommunicationStatusChanged; // 通信状态事件

        public string PortName => _portName;

        public ToolingBoardCommunicator(string portName, int baudRate = 115200)
        {
            _portName = portName;
            _baudRate = baudRate;
            _receiveQueue = new Queue<byte>();
            InitializeSerialPort();
        }

        private void InitializeSerialPort()
        {
            _serialPort = new SerialPort(_portName, _baudRate)
            {
                DataBits = 8,
                StopBits = StopBits.One,
                Parity = Parity.None,
                Handshake = Handshake.None,
                ReadTimeout = 1000,
                WriteTimeout = 1000
            };

            _serialPort.DataReceived += SerialPort_DataReceived;
            _serialPort.ErrorReceived += SerialPort_ErrorReceived;
        }

        // 打开串口
        public bool Open()
        {
            try
            {
                if (!_serialPort.IsOpen)
                {
                    _serialPort.Open();
                    ClearReceiveQueue(); // 清空队列
                    LogMessage?.Invoke($"工装板串口 {_portName} 已打开");
                    CommunicationStatusChanged?.Invoke(true);
                    return true;
                }
                return true;
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke($"打开串口失败: {ex.Message}");
                CommunicationStatusChanged?.Invoke(false);
                return false;
            }
        }

        // 关闭串口
        public void Close()
        {
            try
            {
                if (_serialPort.IsOpen)
                {
                    _serialPort.Close();
                    ClearReceiveQueue(); // 清空队列
                    LogMessage?.Invoke("工装板串口已关闭");
                    CommunicationStatusChanged?.Invoke(false);
                }
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke($"关闭串口失败: {ex.Message}");
            }
        }

        // 清空接收队列
        public void ClearReceiveQueue()
        {
            lock (_queueLock)
            {
                _receiveQueue.Clear();
            }
        }

        // 获取队列长度（用于调试）
        public int GetQueueLength()
        {
            lock (_queueLock)
            {
                return _receiveQueue.Count;
            }
        }

        // 计算校验和
        private byte CalculateChecksum(byte[] data, int startIndex, int length)
        {
            byte sum = 0;
            for (int i = startIndex; i < startIndex + length; i++)
            {
                sum += data[i];
            }
            return sum;
        }

        // 构建协议帧
        private byte[] BuildFrame(byte cmd, byte[] value = null)
        {
            byte len = (byte)(value?.Length ?? 0);
            int frameLength = 5 + len;

            byte[] frame = new byte[frameLength];

            frame[0] = 0xEB;
            frame[1] = 0x97;
            frame[2] = cmd;
            frame[3] = len;

            if (len > 0 && value != null)
            {
                Array.Copy(value, 0, frame, 4, len);
            }

            frame[frameLength - 1] = CalculateChecksum(frame, 0, frameLength - 1);

            return frame;
        }

        // 发送命令并等待响应（带超时）
        public async Task<bool> SendCommandWithAck(byte cmd, byte[] value = null, int timeoutMs = 1000)
        {
            byte[] sentFrame = BuildFrame(cmd, value);
            string sentFrameHex = BitConverter.ToString(sentFrame).Replace("-", " ");

            // 记录发送的帧用于比较
            byte expectedCmd = cmd;

            try
            {
                if (!_serialPort.IsOpen)
                {
                    LogMessage?.Invoke("串口未打开，无法发送数据");
                    return false;
                }

                // 清空队列，确保我们只接收新的响应
                ClearReceiveQueue();

                // 发送数据
                _serialPort.Write(sentFrame, 0, sentFrame.Length);
                LogMessage?.Invoke($"发送: {sentFrameHex}");

                // 等待响应
                var responseTask = WaitForResponse(expectedCmd, timeoutMs);
                var completedTask = await Task.WhenAny(responseTask, Task.Delay(timeoutMs));

                if (completedTask == responseTask && responseTask.Result)
                {
                    LogMessage?.Invoke("命令执行成功");
                    return true;
                }
                else
                {
                    LogMessage?.Invoke($"命令响应超时 ({timeoutMs}ms)");
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke($"发送命令失败: {ex.Message}");
                return false;
            }
        }

        // 等待特定命令的响应
        private async Task<bool> WaitForResponse(byte expectedCmd, int timeoutMs)
        {
            DateTime startTime = DateTime.Now;

            while ((DateTime.Now - startTime).TotalMilliseconds < timeoutMs)
            {
                // 处理队列中的数据
                if (ProcessQueueData(expectedCmd))
                {
                    return true;
                }

                // 等待一段时间再检查
                await Task.Delay(10);
            }

            return false;
        }

        // 发送频率控制命令（带响应确认）
        public async Task<bool> SetFrequencyWithAck(byte frequencyCode)
        {
            byte[] value = new byte[] { frequencyCode };
            return await SendCommandWithAck(0x13, value);
        }

        // 发送衰减控制命令（带响应确认）
        public async Task<bool> SetAttenuationWithAck(byte attenuationCode)
        {
            byte[] value = new byte[] { attenuationCode };
            return await SendCommandWithAck(0x14, value);
        }
        
        // 发送FXJZ拉高命令（开启校准开关）
        public bool SetCalibrationSwitchHigh()
        {
            byte[] command = new byte[] { 0xEB, 0x97, 0x12, 0x01, 0x01, 0x96 };
            return SendFrame(command);
        }
        
        // 发送FXJZ拉低命令（关闭校准开关）
        public bool SetCalibrationSwitchLow()
        {
            byte[] command = new byte[] { 0xEB, 0x97, 0x12, 0x01, 0x00, 0x95 };
            return SendFrame(command);
        }

        // 发送校准源上电命令
        public bool SetCalibrationSourcePowerOn()
        {
            byte[] command = new byte[] { 0xEB, 0x97, 0x11, 0x05, 0xFE, 0x03, 0x05, 0x00, 0xF8, 0x96 };
            return SendFrame(command);
        }

        // 发送校准源下电命令
        public bool SetCalibrationSourcePowerOff()
        {
            byte[] command = new byte[] { 0xEB, 0x97, 0x11, 0x05, 0xFE, 0x03, 0x05, 0x01, 0xF9, 0x98 };
            return SendFrame(command);
        }

        // 发送天线矢能命令
        public bool SetAntennaEnable()
        {
            byte[] command = new byte[] { 0xEB, 0x97, 0x11, 0x05, 0xFE, 0x04, 0x05, 0x01, 0xFE, 0x9E };
            return SendFrame(command);
        }

        // 发送天线去能命令
        public bool SetAntennaDisable()
        {
            byte[] command = new byte[] { 0xEB, 0x97, 0x11, 0x05, 0xFE, 0x04, 0x05, 0x00, 0xFF, 0x9E };
            return SendFrame(command);
        }

        // 发送通道锁定命令
        // 通道代码: 1=A, 2=B, 3=C, 4=E, 5=F, 6=G, 7=H
        public bool SetChannelLock(byte channelCode)
        {
            byte[] command = channelCode switch
            {
                1 => new byte[] { 0xEB, 0x97, 0x11, 0x05, 0xFE, 0x02, 0x05, 0x01, 0xF8, 0x96 },
                2 => new byte[] { 0xEB, 0x97, 0x11, 0x05, 0xFE, 0x02, 0x05, 0x02, 0xFB, 0x9A },
                3 => new byte[] { 0xEB, 0x97, 0x11, 0x05, 0xFE, 0x02, 0x05, 0x03, 0xFA, 0x9A },
                4 => new byte[] { 0xEB, 0x97, 0x11, 0x05, 0xFE, 0x02, 0x05, 0x04, 0xFD, 0x9E },
                5 => new byte[] { 0xEB, 0x97, 0x11, 0x05, 0xFE, 0x02, 0x05, 0x05, 0xFC, 0x9E },
                6 => new byte[] { 0xEB, 0x97, 0x11, 0x05, 0xFE, 0x02, 0x05, 0x06, 0xFF, 0xA2 },
                7 => new byte[] { 0xEB, 0x97, 0x11, 0x05, 0xFE, 0x02, 0x05, 0x07, 0xFE, 0xA2 },
                _ => null
            };

            if (command == null)
            {
                Console.WriteLine($"[ERROR] 无效的通道锁定代码: {channelCode}");
                return false;
            }

            return SendFrame(command);
        }

        // 发送Antenna模式完整命令序列（FXJZ拉高、校准源上电、天线矢能）
        public bool SendAntennaModeCommands()
        {
            bool allSuccess = true;
            allSuccess &= SetCalibrationSwitchHigh();
            System.Threading.Thread.Sleep(50);
            allSuccess &= SetCalibrationSourcePowerOn();
            System.Threading.Thread.Sleep(50);
            allSuccess &= SetAntennaEnable();
            return allSuccess;
        }

        // 发送Antenna模式完整命令序列（异步版本）
        public async Task<bool> SendAntennaModeCommandsAsync()
        {
            bool allSuccess = true;
            allSuccess &= SetCalibrationSwitchHigh();
            await Task.Delay(50);
            allSuccess &= SetCalibrationSourcePowerOn();
            await Task.Delay(50);
            allSuccess &= SetAntennaEnable();
            return allSuccess;
        }

        // 发送Normal模式完整命令序列（FXJZ拉低、校准源下电、天线去能）
        public bool SendNormalModeCommands()
        {
            bool allSuccess = true;
            allSuccess &= SetCalibrationSwitchLow();
            System.Threading.Thread.Sleep(50);
            allSuccess &= SetCalibrationSourcePowerOff();
            System.Threading.Thread.Sleep(50);
            allSuccess &= SetAntennaDisable();
            return allSuccess;
        }

        // 发送Normal模式完整命令序列（异步版本）
        public async Task<bool> SendNormalModeCommandsAsync()
        {
            bool allSuccess = true;
            allSuccess &= SetCalibrationSwitchLow();
            await Task.Delay(50);
            allSuccess &= SetCalibrationSourcePowerOff();
            await Task.Delay(50);
            allSuccess &= SetAntennaDisable();
            return allSuccess;
        }

        // 发送原始帧（不带响应确认）
        public bool SendFrame(byte[] frame)
        {
            try
            {
                if (!_serialPort.IsOpen)
                {
                    LogMessage?.Invoke("串口未打开，无法发送数据");
                    return false;
                }

                _serialPort.Write(frame, 0, frame.Length);
                string hexString = BitConverter.ToString(frame).Replace("-", " ");
                LogMessage?.Invoke($"发送: {hexString}");

                return true;
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke($"发送数据失败: {ex.Message}");
                return false;
            }
        }

        // 串口数据接收事件
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                int bytesToRead = _serialPort.BytesToRead;
                if (bytesToRead > 0)
                {
                    byte[] buffer = new byte[bytesToRead];
                    int bytesRead = _serialPort.Read(buffer, 0, bytesToRead);

                    // 将数据加入队列
                    lock (_queueLock)
                    {
                        foreach (byte b in buffer.Take(bytesRead))
                        {
                            _receiveQueue.Enqueue(b);
                        }
                    }

                    LogMessage?.Invoke($"收到 {bytesRead} 字节数据，队列长度: {GetQueueLength()}");

                    // 处理队列中的数据
                    ProcessQueueData();
                }
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke($"接收数据错误: {ex.Message}");
                
                lock (_queueLock)
                {
                    _receiveQueue.Clear();
                }
                LogMessage?.Invoke("接收队列已清空，准备恢复接收");
            }
        }

        // 处理队列中的数据（无特定命令过滤）
        private void ProcessQueueData()
        {
            ProcessQueueData(null);
        }

        // 处理队列中的数据（可指定期望的命令）
        private bool ProcessQueueData(byte? expectedCmd)
        {
            bool foundValidFrame = false;

            lock (_queueLock)
            {
                while (_receiveQueue.Count >= 5) // 最小帧长度
                {
                    // 查找帧头
                    byte[] queueArray = _receiveQueue.ToArray();
                    int frameStartIndex = -1;

                    for (int i = 0; i <= queueArray.Length - 5; i++)
                    {
                        if (queueArray[i] == 0xEB &&
                            (i + 1 < queueArray.Length) &&
                            queueArray[i + 1] == 0x97)
                        {
                            frameStartIndex = i;
                            break;
                        }
                    }

                    if (frameStartIndex == -1)
                    {
                        // 没有找到有效帧头，清空队列
                        _receiveQueue.Clear();
                        break;
                    }

                    // 移除帧头之前的所有数据
                    for (int i = 0; i < frameStartIndex; i++)
                    {
                        _receiveQueue.Dequeue();
                    }

                    // 现在队列的第一个字节应该是帧头1
                    if (_receiveQueue.Count < 5)
                        break;

                    // 查看长度字段
                    byte[] header = _receiveQueue.Take(4).ToArray(); // 取前4个字节
                    byte len = header[3];
                    int totalFrameLength = 5 + len;

                    if (_receiveQueue.Count < totalFrameLength)
                    {
                        // 数据不完整，等待更多数据
                        break;
                    }

                    // 提取完整帧
                    byte[] frame = _receiveQueue.Take(totalFrameLength).ToArray();

                    // 验证校验和
                    byte calculatedSum = CalculateChecksum(frame, 0, totalFrameLength - 1);
                    byte receivedSum = frame[totalFrameLength - 1];

                    if (calculatedSum == receivedSum)
                    {
                        // 校验成功
                        byte cmd = frame[2];
                        byte[] value = len > 0 ?
                            frame.Skip(4).Take(len).ToArray() :
                            new byte[0];

                        // 如果指定了期望的命令，检查是否匹配
                        if (expectedCmd == null || cmd == expectedCmd)
                        {
                            // 触发数据接收事件
                            DataReceived?.Invoke(cmd, value);

                            // 从队列中移除这个帧
                            for (int i = 0; i < totalFrameLength; i++)
                            {
                                _receiveQueue.Dequeue();
                            }

                            foundValidFrame = true;

                            // 记录成功接收
                            string hexString = BitConverter.ToString(frame).Replace("-", " ");
                            LogMessage?.Invoke($"成功解析帧: {hexString}");

                            // 如果是期望的命令响应，返回true
                            if (expectedCmd.HasValue && cmd == expectedCmd.Value)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            // 命令不匹配，跳过这个帧
                            for (int i = 0; i < totalFrameLength; i++)
                            {
                                _receiveQueue.Dequeue();
                            }
                        }
                    }
                    else
                    {
                        // 校验和错误，丢弃第一个字节继续查找
                        LogMessage?.Invoke("校验和错误，丢弃一字节");
                        _receiveQueue.Dequeue();
                    }
                }
            }

            return foundValidFrame;
        }

        // 串口错误处理
        private void SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            LogMessage?.Invoke($"串口错误: {e.EventType}");
            CommunicationStatusChanged?.Invoke(false);
        }

        public static string[] GetAvailablePorts()
        {
            return SerialPort.GetPortNames();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // 释放托管资源
                    Close();
                    _serialPort?.Dispose();
                    lock (_queueLock)
                    {
                        _receiveQueue.Clear();
                    }
                }

                // 释放非托管资源（如果有）

                _disposed = true;
            }
        }

        ~ToolingBoardCommunicator()
        {
            Dispose(false);
        }
    }
}
