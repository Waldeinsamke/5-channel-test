using System;
using TemperatureChamber.Communication;
using TemperatureChamber.Models;

namespace TemperatureChamber
{
    /// <summary>
    /// 温箱控制核心类
    /// 实现IChamberController接口，提供温箱设备的基本控制功能
    /// </summary>
    public class ChamberController : IChamberController
    {
        private SimpleModbusMaster _modbus;
        private ChamberConfig _config;

        /// <summary>
        /// 设备是否连接
        /// </summary>
        public bool IsConnected => _modbus?.IsConnected ?? false;

        /// <summary>
        /// 串口名称
        /// </summary>
        public string PortName => _config?.PortName;

        /// <summary>
        /// 从站地址
        /// </summary>
        public byte SlaveId => _config?.SlaveId ?? 0;

        /// <summary>
        /// 连接状态变化事件
        /// </summary>
        public event EventHandler<bool> ConnectionChanged;

        /// <summary>
        /// 错误发生事件
        /// </summary>
        public event EventHandler<string> ErrorOccurred;

        /// <summary>
        /// 状态更新事件
        /// </summary>
        public event EventHandler<DeviceStatus> StatusUpdated;

        /// <summary>
        /// 调试信息事件
        /// </summary>
        public event EventHandler<string> Debug;

        /// <summary>
        /// 初始化温箱控制器
        /// </summary>
        /// <param name="config">温箱配置信息</param>
        public ChamberController(ChamberConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// 连接到设备
        /// </summary>
        /// <returns>连接是否成功</returns>
        public bool Connect()
        {
            try
            {
                _modbus = new SimpleModbusMaster(_config);
                _modbus.Debug += OnModbusDebug;
                _modbus.Open();
                OnConnectionChanged(true);
                Console.WriteLine($"成功连接到温箱设备，串口: {_config.PortName}, 从站地址: {_config.SlaveId}");
                return true;
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"连接失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            try
            {
                _modbus?.Close();
                _modbus?.Dispose();
                _modbus = null;
                OnConnectionChanged(false);
                Console.WriteLine("已断开温箱设备连接");
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"断开连接失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 读取当前温度
        /// </summary>
        /// <returns>温度值（℃）</returns>
        public double ReadTemperature()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("设备未连接");
            }

            try
            {
                double temperature = _modbus.ReadTemperature();
                Console.WriteLine($"当前温度: {temperature:F2} ℃");
                return temperature;
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"读取温度失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 设置目标温度
        /// </summary>
        /// <param name="temperature">目标温度（℃）</param>
        public void SetTemperature(double temperature)
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("设备未连接");
            }

            if (temperature < _config.MinTemperature || temperature > _config.MaxTemperature)
            {
                throw new ArgumentOutOfRangeException(nameof(temperature), 
                    $"温度值应在{_config.MinTemperature}℃到{_config.MaxTemperature}℃之间");
            }

            try
            {
                _modbus.SetTemperature(temperature);
                Console.WriteLine($"成功设置温度: {temperature:F1} ℃");
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"设置温度失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 启动设备
        /// </summary>
        public void StartDevice()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("设备未连接");
            }

            try
            {
                _modbus.StartDevice();
                Console.WriteLine("设备启动成功");
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"启动设备失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 停止设备
        /// </summary>
        public void StopDevice()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("设备未连接");
            }

            try
            {
                _modbus.StopDevice();
                Console.WriteLine("设备停止成功");
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"停止设备失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 读取设备运行状态
        /// 推荐用于测试连通性
        /// </summary>
        /// <returns>运行状态值</returns>
        public ushort ReadDeviceStatus()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("设备未连接");
            }

            try
            {
                ushort status = _modbus.ReadDeviceStatus();
                Console.WriteLine($"设备运行状态: {status}");
                return status;
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"读取运行状态失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 触发连接状态变化事件
        /// </summary>
        protected virtual void OnConnectionChanged(bool isConnected)
        {
            ConnectionChanged?.Invoke(this, isConnected);
        }

        /// <summary>
        /// 触发错误发生事件
        /// </summary>
        protected virtual void OnErrorOccurred(string errorMessage)
        {
            Console.WriteLine($"[错误] {errorMessage}");
            ErrorOccurred?.Invoke(this, errorMessage);
        }

        /// <summary>
        /// 触发状态更新事件
        /// </summary>
        protected virtual void OnStatusUpdated(DeviceStatus status)
        {
            StatusUpdated?.Invoke(this, status);
        }

        /// <summary>
        /// 处理 Modbus 调试信息
        /// </summary>
        private void OnModbusDebug(object? sender, string message)
        {
            Debug?.Invoke(this, message);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Disconnect();
        }
    }
}
