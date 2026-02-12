using System;

namespace 五通道自动测试.Calibration
{
    /// <summary>
    /// 校准地址计算器
    /// 负责根据频率、温度和通道计算校准参数地址
    /// 封装所有与校准地址计算相关的逻辑
    /// </summary>
    public class CalibrationAddressCalculator
    {
        private int[] _normalAddrDec = new int[21];
        private string[] _normalAddr = new string[21];
        private int[] _antennaAddrDec = new int[16]; // 天线模式地址数组，索引1-15
        private string[] _antennaAddr = new string[16]; // 天线模式地址字符串数组
        private int[] _db36AddrDec = new int[21]; // 36db模式地址数组
        private string[] _db36Addr = new string[21]; // 36db模式地址字符串数组
        
        /// <summary>
        /// 根据通道号计算校准参数起始索引
        /// 每个通道占用2个校准地址，起始索引从1开始
        /// </summary>
        /// <param name="channel">通道号 (1-5)</param>
        /// <returns>起始索引</returns>
        public int GetStartIndexByChannel(int channel)
        {
            return (channel - 1) * 2 + 1;
        }
        
        /// <summary>
        /// 根据当前选择的频率和温度索引计算校准地址
        /// 校准地址 = 基础地址 + 频率偏移 + 温度偏移
        /// </summary>
        /// <param name="frequencyIndex">频率索引 (1-4)</param>
        /// <param name="temperatureIndex">温度索引 (1-31)</param>
        public void CalculateNormalAddresses(int frequencyIndex, int temperatureIndex)
        {
            int fre = frequencyIndex;
            int tem = temperatureIndex;
            
            int normalStartAddress = (20 * 31) * (fre - 1) + 20 * (tem - 1) + 1;
            
            for (int i = 1; i <= 20; i++)
            {
                _normalAddrDec[i] = normalStartAddress + (i - 1);
                _normalAddr[i] = "0x" + _normalAddrDec[i].ToString("X4");
            }
        }
        
        /// <summary>
        /// 获取当前频率和温度下，特定地址索引对应的地址
        /// </summary>
        /// <param name="index">地址索引</param>
        /// <returns>十六进制格式的地址字符串</returns>
        public string GetNormalAddress(int index)
        {
            if (index >= 1 && index <= 20)
            {
                return _normalAddr[index];
            }
            return "0x0000";
        }
        
        /// <summary>
        /// 获取当前频率和温度下，特定地址索引对应的十进制地址值
        /// </summary>
        /// <param name="index">地址索引</param>
        /// <returns>十进制地址值</returns>
        public int GetNormalAddressDecimal(int index)
        {
            if (index >= 1 && index <= 20)
            {
                return _normalAddrDec[index];
            }
            return 0;
        }
        
        /// <summary>
        /// 将十六进制地址字符串转换为ushort类型
        /// </summary>
        /// <param name="addressStr">十六进制地址字符串（如"0x1234"）</param>
        /// <returns>ushort类型的地址值</returns>
        public ushort ParseAddressString(string addressStr)
        {
            try
            {
                if (addressStr.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                {
                    return ushort.Parse(addressStr.Substring(2), System.Globalization.NumberStyles.HexNumber);
                }
                return ushort.Parse(addressStr, System.Globalization.NumberStyles.HexNumber);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        
        /// <summary>
        /// 根据当前选择的频率和温度索引计算天线模式地址
        /// 天线起始地址 = 4340 + (15 × 31) × (fre - 1) + 15 × (tem - 1) + 1
        /// </summary>
        /// <param name="frequencyIndex">频率索引 (1-4)</param>
        /// <param name="temperatureIndex">温度索引 (1-31)</param>
        public void CalculateAntennaAddresses(int frequencyIndex, int temperatureIndex)
        {
            int fre = frequencyIndex;
            int tem = temperatureIndex;
            
            // 根据用户提供的示例，起始地址需要调整
            int antennaStartAddress = 4340 + (15 * 31) * (fre - 1) + 15 * (tem - 1) + 1;
            
            for (int i = 1; i <= 15; i++)
            {
                _antennaAddrDec[i] = antennaStartAddress + (i - 1);
                _antennaAddr[i] = "0x" + _antennaAddrDec[i].ToString("X4");
            }
        }
        
        /// <summary>
        /// 获取当前频率和温度下，特定地址索引对应的天线模式地址
        /// </summary>
        /// <param name="index">地址索引</param>
        /// <returns>十六进制格式的地址字符串</returns>
        public string GetAntennaAddress(int index)
        {
            if (index >= 1 && index <= 15)
            {
                return _antennaAddr[index];
            }
            return "0x0000";
        }
        
        /// <summary>
        /// 获取当前频率和温度下，特定地址索引对应的天线模式十进制地址值
        /// </summary>
        /// <param name="index">地址索引</param>
        /// <returns>十进制地址值</returns>
        public int GetAntennaAddressDecimal(int index)
        {
            if (index >= 1 && index <= 15)
            {
                return _antennaAddrDec[index];
            }
            return 0;
        }
        
        /// <summary>
        /// 根据当前选择的频率和温度索引计算36db模式地址
        /// 36db起始地址 = 33480 + (20 * 31) * (fre - 1) + 20 * (tem - 1) + 1
        /// </summary>
        /// <param name="frequencyIndex">频率索引 (1-4)</param>
        /// <param name="temperatureIndex">温度索引 (1-31)</param>
        public void CalculateDb36Addresses(int frequencyIndex, int temperatureIndex)
        {
            int fre = frequencyIndex;
            int tem = temperatureIndex;
            
            int db36StartAddress = 33480 + (20 * 31) * (fre - 1) + 20 * (tem - 1) + 1;
            
            for (int i = 1; i <= 20; i++)
            {
                _db36AddrDec[i] = db36StartAddress + (i - 1);
                _db36Addr[i] = "0x" + _db36AddrDec[i].ToString("X4");
            }
        }
        
        /// <summary>
        /// 获取当前频率和温度下，特定地址索引对应的36db模式地址
        /// </summary>
        /// <param name="index">地址索引</param>
        /// <returns>十六进制格式的地址字符串</returns>
        public string GetDb36Address(int index)
        {
            if (index >= 1 && index <= 20)
            {
                return _db36Addr[index];
            }
            return "0x0000";
        }
        
        /// <summary>
        /// 获取当前频率和温度下，特定地址索引对应的36db模式十进制地址值
        /// </summary>
        /// <param name="index">地址索引</param>
        /// <returns>十进制地址值</returns>
        public int GetDb36AddressDecimal(int index)
        {
            if (index >= 1 && index <= 20)
            {
                return _db36AddrDec[index];
            }
            return 0;
        }
    }
}