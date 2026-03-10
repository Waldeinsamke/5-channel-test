using System;
using System.IO.Ports;
using System.Threading;
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
        private readonly object _lock = new object();

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
        /// 地址: 0x0000, 值需除以100得到实际温度
        /// </summary>
        /// <returns>温度值（℃）</returns>
        public double ReadTemperature()
        {
            try
            {
                ushort[] registers = ReadHoldingRegisters(0x0000, 1);
                ushort rawValue = registers[0];
                double temperature;

                if (rawValue > 32767)
                {
                    short signedValue = (short)rawValue;
                    temperature = signedValue / 100.0;
                }
                else
                {
                    temperature = rawValue / 100.0;
                }

                return temperature;
            }
            catch (Exception ex)
            {
                OnDebug($"读取温度失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 读取设备运行状态
        /// 地址: 0x0018
        /// 返回值第0位=1运行，=0停止
        /// </summary>
        /// <returns>运行状态 true=运行中, false=停止</returns>
        public bool ReadDeviceStatus()
        {
            try
            {
                ushort[] registers = ReadHoldingRegisters(0x0018, 1);
                ushort rawValue = registers[0];
                bool isRunning = (rawValue & 0x0001) == 1;
                return isRunning;
            }
            catch (Exception ex)
            {
                OnDebug($"读取运行状态失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 设置目标温度
        /// 地址: 0x0026
        /// 正温度: 值 = 目标温度 × 10
        /// 负温度: 值 = 65536 - (|目标温度| × 10)
        /// </summary>
        /// <param name="temperature">目标温度（℃）</param>
        public void SetTemperature(double temperature)
        {
            try
            {
                ushort value;
                if (temperature >= 0)
                {
                    value = (ushort)(temperature * 10);
                }
                else
                {
                    value = (ushort)(65536 - (Math.Abs(temperature) * 10));
                }
                WriteSingleRegister(0x0026, value);
            }
            catch (Exception ex)
            {
                OnDebug($"设置温度失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 启动设备
        /// 地址: 线圈0x0000, 值ON (FF 00)
        /// </summary>
        public void StartDevice()
        {
            try
            {
                WriteSingleCoil(0x0000, true);
            }
            catch (Exception ex)
            {
                OnDebug($"启动设备失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 停止设备
        /// 地址: 线圈0x0001, 值ON (FF 00)
        /// </summary>
        public void StopDevice()
        {
            try
            {
                WriteSingleCoil(0x0001, true);
            }
            catch (Exception ex)
            {
                OnDebug($"停止设备失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 读取故障信息
        /// 地址: 0x001B, 返回0表示无故障，1-27对应具体故障
        /// </summary>
        /// <returns>故障代码</returns>
        public ushort ReadFaultInfo()
        {
            try
            {
                ushort[] registers = ReadHoldingRegisters(0x001B, 1);
                return registers[0];
            }
            catch (Exception ex)
            {
                OnDebug($"读取故障信息失败: {ex.Message}");
                throw;
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
            lock (_lock)
            {
                if (!_serialPort.IsOpen)
                {
                    throw new InvalidOperationException("串口未连接");
                }

                _serialPort.DiscardInBuffer();
                _serialPort.DiscardOutBuffer();
                _serialPort.Write(request, 0, request.Length);
            }
        }

        /// <summary>
        /// 读取响应
        /// </summary>
        private byte[] ReadResponse()
        {
            lock (_lock)
            {
                if (!_serialPort.IsOpen)
                {
                    throw new InvalidOperationException("串口未连接");
                }

                int startTime = Environment.TickCount;
                int timeout = _config.Timeout;

                while (_serialPort.BytesToRead < 5)
                {
                    if (Environment.TickCount - startTime > timeout)
                    {
                        throw new TimeoutException("读取响应超时");
                    }
                    System.Threading.Thread.Sleep(10);
                }

                byte[] header = new byte[5];
                _serialPort.Read(header, 0, 5);

                byte byteCount = header[2];
                int dataLength = byteCount;
                int remainingBytes = dataLength;

                while (_serialPort.BytesToRead < remainingBytes)
                {
                    if (Environment.TickCount - startTime > timeout)
                    {
                        throw new TimeoutException("读取响应超时");
                    }
                    System.Threading.Thread.Sleep(10);
                }

                byte[] data = new byte[byteCount];
                _serialPort.Read(data, 0, byteCount);

                int actualResponseLength = 5 + byteCount;
                byte[] response = new byte[actualResponseLength];
                Array.Copy(header, 0, response, 0, 5);
                Array.Copy(data, 0, response, 5, byteCount);

                return response;
            }
        }

        /// <summary>
        /// 解析读取保持寄存器响应
        /// </summary>
        private ushort[] ParseReadHoldingRegistersResponse(byte[] response, ushort numberOfPoints)
        {
            int byteCount = response[2];
            if (byteCount == 0)
            {
                throw new Exception($"温箱响应数据为空，请检查温箱设备连接状态");
            }
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
