using System;
using System.Threading;
using System.Threading.Tasks;
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
        private CancellationTokenSource? _pollingCts;
        private Task? _pollingTask;
        private DeviceStatus? _lastStatus;
        private readonly object _statusLock = new object();
        private readonly object _operationLock = new object();
        private bool _isPollingPaused = false;

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
                StartPolling();
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
                StopPolling();
                Thread.Sleep(100);
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

            lock (_operationLock)
            {
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
        /// <returns>运行状态 true=运行中, false=停止</returns>
        public bool ReadDeviceStatus()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("设备未连接");
            }

            lock (_operationLock)
            {
                try
                {
                    bool isRunning = _modbus.ReadDeviceStatus();
                    Console.WriteLine($"设备运行状态: {(isRunning ? "运行中" : "停止")}");
                    return isRunning;
                }
                catch (Exception ex)
                {
                    OnErrorOccurred($"读取运行状态失败: {ex.Message}");
                    throw;
                }
            }
        }

        /// <summary>
        /// 内部方法：读取温箱状态
        /// </summary>
        private DeviceStatus ReadStatusInternal()
        {
            double temperature = 0;
            bool isRunning = false;

            lock (_operationLock)
            {
                try
                {
                    temperature = _modbus.ReadTemperature();
                }
                catch
                {
                }

                try
                {
                    isRunning = _modbus.ReadDeviceStatus();
                }
                catch
                {
                }
            }

            return new DeviceStatus
            {
                Temperature = temperature,
                IsRunning = isRunning
            };
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
        /// 读取故障信息
        /// </summary>
        /// <returns>故障代码，0表示无故障</returns>
        public ushort ReadFaultInfo()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("设备未连接");
            }

            try
            {
                ushort faultInfo = _modbus.ReadFaultInfo();
                Console.WriteLine($"故障信息: {faultInfo}");
                return faultInfo;
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"读取故障信息失败: {ex.Message}");
                throw;
            }
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
        /// 启动状态轮询
        /// </summary>
        private void StartPolling()
        {
            if (_pollingTask != null && !_pollingTask.IsCompleted)
                return;

            _pollingCts = new CancellationTokenSource();
            _pollingTask = Task.Run(async () =>
            {
                while (!_pollingCts.Token.IsCancellationRequested)
                {
                    try
                    {
                        if (_modbus?.IsConnected == true && !_isPollingPaused)
                        {
                            var status = ReadStatusInternal();
                            bool shouldNotify = false;

                            lock (_statusLock)
                            {
                                if (_lastStatus == null ||
                                    Math.Abs(_lastStatus.Temperature - status.Temperature) > 0.1 ||
                                    _lastStatus.IsRunning != status.IsRunning)
                                {
                                    _lastStatus = status;
                                    shouldNotify = true;
                                }
                            }

                            if (shouldNotify)
                            {
                                OnStatusUpdated(status);
                            }
                        }
                    }
                    catch
                    {
                    }

                    await Task.Delay(1000, _pollingCts.Token);
                }
            }, _pollingCts.Token);
        }

        /// <summary>
        /// 暂停状态轮询
        /// </summary>
        public void PausePolling()
        {
            _isPollingPaused = true;
        }

        /// <summary>
        /// 恢复状态轮询
        /// </summary>
        public void ResumePolling()
        {
            _isPollingPaused = false;
        }

        /// <summary>
        /// 停止状态轮询
        /// </summary>
        private void StopPolling()
        {
            _pollingCts?.Cancel();
            try
            {
                _pollingTask?.Wait(1000);
            }
            catch
            {
            }
            _pollingCts?.Dispose();
            _pollingCts = null;
            _pollingTask = null;
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
