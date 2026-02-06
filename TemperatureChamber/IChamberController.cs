using System;
using TemperatureChamber.Models;

namespace TemperatureChamber
{
    /// <summary>
    /// 温箱控制接口
    /// 定义温箱设备控制的核心功能
    /// </summary>
    public interface IChamberController : IDisposable
    {
        /// <summary>
        /// 设备是否连接
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// 串口名称
        /// </summary>
        string PortName { get; }

        /// <summary>
        /// 从站地址
        /// </summary>
        byte SlaveId { get; }

        /// <summary>
        /// 连接状态变化事件
        /// </summary>
        event EventHandler<bool> ConnectionChanged;

        /// <summary>
        /// 错误发生事件
        /// </summary>
        event EventHandler<string> ErrorOccurred;

        /// <summary>
        /// 状态更新事件
        /// </summary>
        event EventHandler<DeviceStatus> StatusUpdated;

        /// <summary>
        /// 连接到设备
        /// </summary>
        /// <returns>连接是否成功</returns>
        bool Connect();

        /// <summary>
        /// 断开连接
        /// </summary>
        void Disconnect();

        /// <summary>
        /// 读取当前温度
        /// </summary>
        /// <returns>温度值（℃）</returns>
        double ReadTemperature();

        /// <summary>
        /// 设置目标温度
        /// </summary>
        /// <param name="temperature">目标温度（℃）</param>
        void SetTemperature(double temperature);

        /// <summary>
        /// 启动设备
        /// </summary>
        void StartDevice();

        /// <summary>
        /// 停止设备
        /// </summary>
        void StopDevice();

        /// <summary>
        /// 读取设备运行状态
        /// 推荐用于测试连通性
        /// </summary>
        /// <returns>运行状态值</returns>
        ushort ReadDeviceStatus();
    }
}
