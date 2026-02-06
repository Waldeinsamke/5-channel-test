namespace TemperatureChamber.Communication
{
    public static class ModbusConstants
    {
        // 功能码
        public const byte READ_HOLDING_REGISTERS = 0x03;
        public const byte WRITE_SINGLE_REGISTER = 0x06;
        public const byte WRITE_SINGLE_COIL = 0x05;
        
        // 默认通讯参数
        public const int DEFAULT_BAUD_RATE = 38400;
        public const int DEFAULT_DATA_BITS = 8;
        public const int DEFAULT_STOP_BITS = 1;
        public const int DEFAULT_TIMEOUT = 1000;
        
        // 寄存器地址
        public const ushort REGISTER_TEMPERATURE = 0x0000;
        public const ushort REGISTER_HUMIDITY = 0x0003;
        public const ushort REGISTER_RUNNING_STATUS = 0x0024;
        public const ushort REGISTER_SET_TEMPERATURE = 0x0038;
        public const ushort REGISTER_SET_HUMIDITY = 0x0039;
        
        // 线圈地址
        public const ushort COIL_START = 0x0000;
        public const ushort COIL_STOP = 0x0001;
        
        // 错误码
        public static string GetErrorMessage(byte errorCode)
        {
            return errorCode switch
            {
                0x01 => "非法功能码",
                0x02 => "非法数据地址",
                0x03 => "非法数据值",
                0x04 => "从站设备故障",
                0x05 => "确认",
                0x06 => "从站设备忙",
                _ => "未知错误"
            };
        }
    }
}