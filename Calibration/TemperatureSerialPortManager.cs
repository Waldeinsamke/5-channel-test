using System;
using System.Windows.Forms;

namespace 五通道自动测试.Calibration
{
    /// <summary>
    /// 温度串口管理器，负责管理温度串口通信
    /// </summary>
    public class TemperatureSerialPortManager
    {
        private readonly TemperatureSerialPort _temperatureSerialPort;
        private readonly Action<float> _temperatureUpdatedCallback;
        private readonly Action<string> _logMessageCallback;
        private readonly Action<ushort, byte> _eepromDataReceivedCallback;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="temperatureUpdatedCallback">温度更新回调</param>
        /// <param name="logMessageCallback">日志回调</param>
        /// <param name="eepromDataReceivedCallback">EEPROM数据接收回调</param>
        public TemperatureSerialPortManager(
            Action<float> temperatureUpdatedCallback,
            Action<string> logMessageCallback,
            Action<ushort, byte> eepromDataReceivedCallback)
        {
            _temperatureUpdatedCallback = temperatureUpdatedCallback;
            _logMessageCallback = logMessageCallback;
            _eepromDataReceivedCallback = eepromDataReceivedCallback;

            _temperatureSerialPort = new TemperatureSerialPort();
            _temperatureSerialPort.TemperatureUpdated += OnTemperatureUpdated;
            _temperatureSerialPort.LogMessage += OnLogMessage;
            _temperatureSerialPort.EepromDataReceived += OnEepromDataReceived;
        }

        /// <summary>
        /// 打开温度串口
        /// </summary>
        /// <param name="portName">串口名称</param>
        /// <returns>是否成功打开</returns>
        public bool OpenPort(string portName)
        {
            try
            {
                return _temperatureSerialPort.OpenPort(portName);
            }
            catch (Exception ex)
            {
                _logMessageCallback?.Invoke($"打开温度串口失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 关闭温度串口
        /// </summary>
        public void ClosePort()
        {
            try
            {
                _temperatureSerialPort.ClosePort();
            }
            catch (Exception ex)
            {
                _logMessageCallback?.Invoke($"关闭温度串口失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 读取EEPROM值
        /// </summary>
        /// <param name="address">EEPROM地址</param>
        public void ReadEEPROM(ushort address)
        {
            try
            {
                _temperatureSerialPort.ReadEEPROM(address);
            }
            catch (Exception ex)
            {
                _logMessageCallback?.Invoke($"读取EEPROM失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 写入EEPROM值
        /// </summary>
        /// <param name="address">EEPROM地址</param>
        /// <param name="value">要写入的值</param>
        public void WriteEEPROM(ushort address, byte value)
        {
            try
            {
                _temperatureSerialPort.WriteEEPROM(address, value);
            }
            catch (Exception ex)
            {
                _logMessageCallback?.Invoke($"写入EEPROM失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 停止温度上报
        /// </summary>
        /// <returns>操作是否成功</returns>
        public bool StopTemperatureReport()
        {
            try
            {
                return _temperatureSerialPort.StopTemperatureReport();
            }
            catch (Exception ex)
            {
                _logMessageCallback?.Invoke($"停止温度上报失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 进入ROM模式
        /// </summary>
        /// <returns>操作是否成功</returns>
        public bool EnterROMMode()
        {
            try
            {
                return _temperatureSerialPort.EnterROMMode();
            }
            catch (Exception ex)
            {
                _logMessageCallback?.Invoke($"进入ROM模式失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 退出ROM模式
        /// </summary>
        /// <returns>操作是否成功</returns>
        public bool ExitROMMode()
        {
            try
            {
                return _temperatureSerialPort.ExitROMMode();
            }
            catch (Exception ex)
            {
                _logMessageCallback?.Invoke($"退出ROM模式失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 恢复温度上报
        /// </summary>
        /// <returns>操作是否成功</returns>
        public bool ResumeTemperatureReport()
        {
            try
            {
                return _temperatureSerialPort.ResumeTemperatureReport();
            }
            catch (Exception ex)
            {
                _logMessageCallback?.Invoke($"恢复温度上报失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 温度更新事件处理
        /// </summary>
        /// <param name="temperature">温度值</param>
        private void OnTemperatureUpdated(float temperature)
        {
            _temperatureUpdatedCallback?.Invoke(temperature);
        }

        /// <summary>
        /// 日志事件处理
        /// </summary>
        /// <param name="message">日志消息</param>
        private void OnLogMessage(string message)
        {
            _logMessageCallback?.Invoke(message);
        }

        /// <summary>
        /// EEPROM数据接收事件处理
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        private void OnEepromDataReceived(ushort address, byte value)
        {
            _eepromDataReceivedCallback?.Invoke(address, value);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            try
            {
                _temperatureSerialPort?.Dispose();
            }
            catch (Exception ex)
            {
                _logMessageCallback?.Invoke($"释放温度串口资源失败: {ex.Message}");
            }
        }
    }
}
