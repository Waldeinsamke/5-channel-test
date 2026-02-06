using System.IO.Ports;

namespace TemperatureChamber.Models
{
    /// <summary>
    /// 温箱配置类
    /// 包含温箱设备的所有配置参数
    /// 包括串口参数、Modbus参数、温度/湿度范围等
    /// </summary>
    public class ChamberConfig
    {
        /// <summary>
        /// 串口名称
        /// 默认值: COM19
        /// </summary>
        public string PortName { get; set; } = "COM19";
        
        /// <summary>
        /// Modbus从站地址
        /// 默认值: 1 (根据实际设备配置)
        /// </summary>
        public byte SlaveId { get; set; } = 1;
        
        /// <summary>
        /// 波特率
        /// 默认值: 38400
        /// </summary>
        public int BaudRate { get; set; } = 38400;
        
        /// <summary>
        /// 校验位
        /// 默认值: Even (偶校验)
        /// </summary>
        public Parity Parity { get; set; } = Parity.Even;
        
        /// <summary>
        /// 数据位
        /// 默认值: 8
        /// </summary>
        public int DataBits { get; set; } = 8;
        
        /// <summary>
        /// 停止位
        /// 默认值: One (1位)
        /// </summary>
        public StopBits StopBits { get; set; } = StopBits.One;
        
        /// <summary>
        /// 超时时间(毫秒)
        /// 默认值: 3000
        /// </summary>
        public int Timeout { get; set; } = 3000;
        
        /// <summary>
        /// 最低温度(℃)
        /// 默认值: -40.0
        /// </summary>
        public double MinTemperature { get; set; } = -58.0;
        
        /// <summary>
        /// 最高温度(℃)
        /// 默认值: 150.0
        /// </summary>
        public double MaxTemperature { get; set; } = 150.0;
        
        /// <summary>
        /// 最低湿度(%)
        /// 默认值: 20.0
        /// </summary>
        public double MinHumidity { get; set; } = 20.0;
        
        /// <summary>
        /// 最高湿度(%)
        /// 默认值: 98.0
        /// </summary>
        public double MaxHumidity { get; set; } = 98.0;
    }
}