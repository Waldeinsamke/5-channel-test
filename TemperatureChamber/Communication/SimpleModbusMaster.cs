using System;
using System.IO.Ports;
using TemperatureChamber.Models;

namespace TemperatureChamber.Communication
{
    /// <summary>
    /// Modbus通讯核心类
    /// 手动实现Modbus RTU协议通信
    /// </summary>
    public class SimpleModbusMaster : IDisposable
    {
        private SerialPort _serialPort;
        private ChamberConfig _config;
        private bool _isDisposed = false;

        /// <summary>
        /// 设备是否连接
        /// </summary>
        public bool IsConnected => _serialPort?.IsOpen ?? false;

        /// <summary>
        /// 调试信息事件
        /// </summary>
        public event EventHandler<string> Debug;

        /// <summary>
        /// 初始化Modbus通讯类
        /// </summary>
        /// <param name="config">温箱配置信息</param>
        public SimpleModbusMaster(ChamberConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// 打开串口连接
        /// </summary>
        public void Open()
        {
            try
            {
                _serialPort = new SerialPort(
                    _config.PortName,
                    _config.BaudRate,
                    _config.Parity,
                    _config.DataBits,
                    _config.StopBits
                );
                _serialPort.ReadTimeout = _config.Timeout;
                _serialPort.WriteTimeout = _config.Timeout;
                _serialPort.Open();
            }
            catch (Exception ex)
            {
                throw new Exception($"打开串口失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 关闭串口连接
        /// </summary>
        public void Close()
        {
            try
            {
                if (_serialPort?.IsOpen == true)
                {
                    _serialPort.Close();
                }
                _serialPort?.Dispose();
                _serialPort = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"关闭串口失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 读取当前温度
        /// 地址: 0x07D5, 值需除以100得到实际温度
        /// </summary>
        /// <returns>温度值（℃）</returns>
        public double ReadTemperature()
        {
            try
            {
                OnDebug($"开始读取温度，寄存器地址: 0x07D5, 数量: 1");
                ushort[] registers = ReadHoldingRegisters(0x07D5, 1);
                OnDebug($"读取到原始值: {registers[0]} (0x{registers[0]:X4})");
                double temperature = registers[0] / 100.0;
                OnDebug($"转换后温度: {temperature:F2}℃");
                return temperature;
            }
            catch (Exception ex)
            {
                OnDebug($"读取温度异常: {ex.Message}");
                throw new Exception($"读取温度失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 读取设备运行状态
        /// 地址: 0x01F5, 推荐用于测试连通性
        /// </summary>
        /// <returns>运行状态值</returns>
        public ushort ReadDeviceStatus()
        {
            try
            {
                OnDebug($"开始读取运行状态，寄存器地址: 0x01F5, 数量: 1");
                ushort[] registers = ReadHoldingRegisters(0x01F5, 1);
                OnDebug($"读取到运行状态值: {registers[0]} (0x{registers[0]:X4})");
                return registers[0];
            }
            catch (Exception ex)
            {
                OnDebug($"读取运行状态异常: {ex.Message}");
                throw new Exception($"读取运行状态失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 设置目标温度
        /// 地址: 0x0038, 值需乘以100
        /// </summary>
        /// <param name="temperature">目标温度（℃）</param>
        public void SetTemperature(double temperature)
        {
            try
            {
                ushort value = (ushort)(temperature * 100);
                WriteSingleRegister(0x0038, value);
            }
            catch (Exception ex)
            {
                throw new Exception($"设置温度失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 启动设备
        /// 地址: 寄存器0x01F4, 值1
        /// </summary>
        public void StartDevice()
        {
            try
            {
                WriteSingleRegister(0x01F4, 1);
            }
            catch (Exception ex)
            {
                OnDebug($"启动设备异常: {ex.Message}");
                throw new Exception($"启动设备失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 停止设备
        /// 地址: 寄存器0x01F4, 值0
        /// </summary>
        public void StopDevice()
        {
            try
            {
                WriteSingleRegister(0x01F4, 0);
            }
            catch (Exception ex)
            {
                OnDebug($"停止设备异常: {ex.Message}");
                throw new Exception($"停止设备失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 读取保持寄存器
        /// 功能码: 0x03
        /// </summary>
        private ushort[] ReadHoldingRegisters(ushort startAddress, ushort numberOfPoints)
        {
            byte[] request = BuildReadHoldingRegistersRequest(startAddress, numberOfPoints);
            SendRequest(request);
            byte[] response = ReadResponse();
            return ParseReadHoldingRegistersResponse(response, numberOfPoints);
        }

        /// <summary>
        /// 写入单个寄存器
        /// 功能码: 0x06
        /// </summary>
        private void WriteSingleRegister(ushort address, ushort value)
        {
            byte[] request = BuildWriteSingleRegisterRequest(address, value);
            SendRequest(request);
            byte[] response = ReadResponse();
            VerifyWriteResponse(response, 0x06);
        }

        /// <summary>
        /// 写入单个线圈
        /// 功能码: 0x05
        /// </summary>
        private void WriteSingleCoil(ushort address, bool value)
        {
            byte[] request = BuildWriteSingleCoilRequest(address, value);
            SendRequest(request);
            byte[] response = ReadResponse();
            VerifyWriteResponse(response, 0x05);
        }

        /// <summary>
        /// 构建读取保持寄存器请求
        /// </summary>
        private byte[] BuildReadHoldingRegistersRequest(ushort startAddress, ushort numberOfPoints)
        {
            byte[] request = new byte[8];
            request[0] = _config.SlaveId;
            request[1] = 0x03;
            request[2] = (byte)(startAddress >> 8);
            request[3] = (byte)(startAddress & 0xFF);
            request[4] = (byte)(numberOfPoints >> 8);
            request[5] = (byte)(numberOfPoints & 0xFF);

            ushort crc = CalculateCRC(request, 0, 6);
            request[6] = (byte)(crc & 0xFF);
            request[7] = (byte)(crc >> 8);

            return request;
        }

        /// <summary>
        /// 构建写入单个寄存器请求
        /// </summary>
        private byte[] BuildWriteSingleRegisterRequest(ushort address, ushort value)
        {
            byte[] request = new byte[8];
            request[0] = _config.SlaveId;
            request[1] = 0x06;
            request[2] = (byte)(address >> 8);
            request[3] = (byte)(address & 0xFF);
            request[4] = (byte)(value >> 8);
            request[5] = (byte)(value & 0xFF);

            ushort crc = CalculateCRC(request, 0, 6);
            request[6] = (byte)(crc & 0xFF);
            request[7] = (byte)(crc >> 8);

            return request;
        }

        /// <summary>
        /// 构建写入单个线圈请求
        /// </summary>
        private byte[] BuildWriteSingleCoilRequest(ushort address, bool value)
        {
            byte[] request = new byte[8];
            request[0] = _config.SlaveId;
            request[1] = 0x05;
            request[2] = (byte)(address >> 8);
            request[3] = (byte)(address & 0xFF);
            request[4] = value ? (byte)0xFF : (byte)0x00;
            request[5] = 0x00;

            ushort crc = CalculateCRC(request, 0, 6);
            request[6] = (byte)(crc & 0xFF);
            request[7] = (byte)(crc >> 8);

            return request;
        }

        /// <summary>
        /// 发送请求
        /// </summary>
        private void SendRequest(byte[] request)
        {
            if (!_serialPort.IsOpen)
            {
                throw new InvalidOperationException("串口未连接");
            }

            _serialPort.DiscardInBuffer();
            _serialPort.DiscardOutBuffer();

            string hexString = BitConverter.ToString(request).Replace("-", " ");
            OnDebug($"发送请求帧: {hexString}");

            _serialPort.Write(request, 0, request.Length);
        }

        /// <summary>
        /// 读取响应
        /// </summary>
        private byte[] ReadResponse()
        {
            if (!_serialPort.IsOpen)
            {
                throw new InvalidOperationException("串口未连接");
            }

            int startTime = Environment.TickCount;
            int timeout = _config.Timeout;

            OnDebug($"等待响应数据... (超时: {timeout}ms)");

            while (_serialPort.BytesToRead < 5)
            {
                if (Environment.TickCount - startTime > timeout)
                {
                    OnDebug($"读取响应超时! 当前可读字节数: {_serialPort.BytesToRead}");
                    throw new TimeoutException("读取响应超时");
                }
                System.Threading.Thread.Sleep(10);
            }

            byte[] header = new byte[5];
            _serialPort.Read(header, 0, 5);

            byte byteCount = header[2];
            int expectedLength = 5 + byteCount;

            OnDebug($"读取到响应头: {BitConverter.ToString(header).Replace("-", " ")}, 字节计数: {byteCount}, 期望总长度: {expectedLength}");

            while (_serialPort.BytesToRead < (expectedLength - 5))
            {
                if (Environment.TickCount - startTime > timeout)
                {
                    OnDebug($"读取响应数据超时! 已读取: 5, 还需要: {expectedLength - 5}, 当前可读: {_serialPort.BytesToRead}");
                    throw new TimeoutException("读取响应超时");
                }
                System.Threading.Thread.Sleep(10);
            }

            byte[] data = new byte[byteCount];
            _serialPort.Read(data, 0, byteCount);

            byte[] crc = new byte[2];
            _serialPort.Read(crc, 0, 2);

            byte[] response = new byte[expectedLength];
            Array.Copy(header, 0, response, 0, 5);
            Array.Copy(data, 0, response, 5, byteCount);
            Array.Copy(crc, 0, response, 5 + byteCount, 2);

            OnDebug($"接收响应帧: {BitConverter.ToString(response).Replace("-", " ")}");

            VerifyCRC(response);

            return response;
        }

        /// <summary>
        /// 解析读取保持寄存器响应
        /// </summary>
        private ushort[] ParseReadHoldingRegistersResponse(byte[] response, ushort numberOfPoints)
        {
            int byteCount = response[2];
            if (byteCount != numberOfPoints * 2)
            {
                throw new Exception($"字节数不匹配: 期望{numberOfPoints * 2}, 实际{byteCount}");
            }

            ushort[] values = new ushort[numberOfPoints];
            int dataStartIndex = 3;

            for (int i = 0; i < numberOfPoints; i++)
            {
                values[i] = (ushort)((response[dataStartIndex + i * 2] << 8) | response[dataStartIndex + i * 2 + 1]);
            }

            return values;
        }

        /// <summary>
        /// 验证写入响应
        /// </summary>
        private void VerifyWriteResponse(byte[] response, byte expectedFunctionCode)
        {
            if (response[0] != _config.SlaveId)
            {
                throw new Exception($"从站地址不匹配: 期望{_config.SlaveId}, 实际{response[0]}");
            }

            if (response[1] != expectedFunctionCode)
            {
                if (response[1] == (expectedFunctionCode | 0x80))
                {
                    byte errorCode = response[2];
                    throw new Exception($"Modbus错误: 错误码 {errorCode:X2}");
                }
                else
                {
                    throw new Exception($"功能码不匹配: 期望0x{expectedFunctionCode:X2}, 实际0x{response[1]:X2}");
                }
            }
        }

        /// <summary>
        /// 验证CRC校验
        /// </summary>
        private void VerifyCRC(byte[] response)
        {
            int length = response.Length;
            if (length < 2)
                return;

            ushort receivedCRC = (ushort)((response[length - 2] << 8) | response[length - 1]);
            ushort calculatedCRC = CalculateCRC(response, 0, length - 2);

            if (receivedCRC != calculatedCRC)
            {
                throw new Exception("CRC校验失败");
            }
        }

        /// <summary>
        /// 计算CRC校验
        /// </summary>
        private ushort CalculateCRC(byte[] buffer, int offset, int length)
        {
            ushort crc = 0xFFFF;

            for (int i = offset; i < offset + length; i++)
            {
                crc ^= buffer[i];

                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x0001) != 0)
                    {
                        crc >>= 1;
                        crc ^= 0xA001;
                    }
                    else
                    {
                        crc >>= 1;
                    }
                }
            }

            return crc;
        }

        /// <summary>
        /// 触发调试事件
        /// </summary>
        private void OnDebug(string message)
        {
            Debug?.Invoke(this, message);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;
            Close();
        }
    }
}
