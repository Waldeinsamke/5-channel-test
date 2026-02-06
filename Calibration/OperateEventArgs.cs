using System;

namespace 五通道自动测试.Calibration
{
    /// <summary>
    /// 操作事件参数类，用于在读取和写入操作中传递地址和值
    /// </summary>
    public class OperateEventArgs : EventArgs
    {
        /// <summary>
        /// 操作地址
        /// </summary>
        public ushort Address { get; }
        
        /// <summary>
        /// 操作值（可为空）
        /// </summary>
        public byte? Value { get; set; }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="address">操作地址</param>
        public OperateEventArgs(ushort address)
        {
            Address = address;
            Value = null;
        }
    }
}