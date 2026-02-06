using System;

namespace TemperatureChamber.Models
{
    /// <summary>
    /// 设备状态信息类
    /// 包含温箱设备的当前状态信息
    /// 包括温度、湿度、运行状态和时间戳
    /// </summary>
    public class DeviceStatus
    {
        /// <summary>
        /// 当前温度（℃）
        /// </summary>
        public double Temperature { get; set; }
        
        /// <summary>
        /// 当前湿度（%）
        /// </summary>
        public double Humidity { get; set; }
        
        /// <summary>
        /// 运行状态
        /// true: 运行中
        /// false: 停止
        /// </summary>
        public bool IsRunning { get; set; }
        
        /// <summary>
        /// 状态时间戳
        /// </summary>
        public DateTime Timestamp { get; set; }
        
        /// <summary>
        /// 初始化设备状态信息
        /// </summary>
        public DeviceStatus()
        {
            Timestamp = DateTime.Now;
        }
        
        /// <summary>
        /// 初始化设备状态信息
        /// </summary>
        /// <param name="temperature">温度值（℃）</param>
        /// <param name="humidity">湿度值（%）</param>
        /// <param name="isRunning">运行状态</param>
        public DeviceStatus(double temperature, double humidity, bool isRunning)
        {
            Temperature = temperature;
            Humidity = humidity;
            IsRunning = isRunning;
            Timestamp = DateTime.Now;
        }
    }
}