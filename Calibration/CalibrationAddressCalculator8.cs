using System;

namespace 五通道自动测试.Calibration
{
    /// <summary>
    /// 八通道校准地址计算器
    /// 负责根据频率、温度和通道计算八通道模式的校准参数地址
    /// 封装所有与八通道校准地址计算相关的逻辑
    /// </summary>
    public class CalibrationAddressCalculator8
    {
        private int[] _normalAddrDec = new int[33];
        private string[] _normalAddr = new string[33];
        private int[] _antennaAddrDec = new int[25];
        private string[] _antennaAddr = new string[25];

        /// <summary>
        /// 根据当前选择的频率和温度索引计算八通道正常模式地址
        /// 正常起始地址 = (64 * 31) * (fre - 1) + 64 * (tem - 1) + 1
        /// </summary>
        /// <param name="frequencyIndex">频率索引 (1-4)</param>
        /// <param name="temperatureIndex">温度索引 (1-31)</param>
        public void CalculateNormalAddresses(int frequencyIndex, int temperatureIndex)
        {
            int fre = frequencyIndex;
            int tem = temperatureIndex;

            int normalStartAddress = (64 * 31) * (fre - 1) + 64 * (tem - 1) + 1;

            for (int i = 1; i <= 32; i++)
            {
                _normalAddrDec[i] = normalStartAddress + (i - 1);
                _normalAddr[i] = "0x" + _normalAddrDec[i].ToString("X4");
            }
        }

        /// <summary>
        /// 获取当前频率和温度下，特定地址索引对应的八通道正常模式地址
        /// </summary>
        /// <param name="index">地址索引</param>
        /// <returns>十六进制格式的地址字符串</returns>
        public string GetNormalAddress(int index)
        {
            if (index >= 1 && index <= 32)
            {
                return _normalAddr[index];
            }
            return "0x0000";
        }

        /// <summary>
        /// 获取当前频率和温度下，特定地址索引对应的八通道正常模式十进制地址值
        /// </summary>
        /// <param name="index">地址索引</param>
        /// <returns>十进制地址值</returns>
        public int GetNormalAddressDecimal(int index)
        {
            if (index >= 1 && index <= 32)
            {
                return _normalAddrDec[index];
            }
            return 0;
        }

        /// <summary>
        /// 根据当前选择的频率和温度索引计算八通道天线模式地址
        /// 天线起始地址 = 15872 + (64 * 31) * (fre - 1) + 64 * (tem - 1) + 1
        /// </summary>
        /// <param name="frequencyIndex">频率索引 (1-4)</param>
        /// <param name="temperatureIndex">温度索引 (1-31)</param>
        public void CalculateAntennaAddresses(int frequencyIndex, int temperatureIndex)
        {
            int fre = frequencyIndex;
            int tem = temperatureIndex;

            int antennaStartAddress = 15872 + (64 * 31) * (fre - 1) + 64 * (tem - 1) + 1;

            for (int i = 1; i <= 24; i++)
            {
                _antennaAddrDec[i] = antennaStartAddress + (i - 1);
                _antennaAddr[i] = "0x" + _antennaAddrDec[i].ToString("X4");
            }
        }

        /// <summary>
        /// 获取当前频率和温度下，特定地址索引对应的八通道天线模式地址
        /// </summary>
        /// <param name="index">地址索引</param>
        /// <returns>十六进制格式的地址字符串</returns>
        public string GetAntennaAddress(int index)
        {
            if (index >= 1 && index <= 24)
            {
                return _antennaAddr[index];
            }
            return "0x0000";
        }

        /// <summary>
        /// 获取当前频率和温度下，特定地址索引对应的八通道天线模式十进制地址值
        /// </summary>
        /// <param name="index">地址索引</param>
        /// <returns>十进制地址值</returns>
        public int GetAntennaAddressDecimal(int index)
        {
            if (index >= 1 && index <= 24)
            {
                return _antennaAddrDec[index];
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
        /// 获取八通道正常模式下，指定通道和参数类型对应的地址索引
        /// </summary>
        /// <param name="channel">通道号 (1-8)</param>
        /// <param name="paramType">参数类型：DAChigh=1, DAClow=2, XNDhigh=3, XNDlow=4</param>
        /// <returns>地址索引</returns>
        public int GetNormalAddressIndex(int channel, int paramType)
        {
            if (channel < 1 || channel > 8)
            {
                return 0;
            }

            int dacHighIndex = (channel - 1) * 2 + 1;
            int dacLowIndex = dacHighIndex + 1;
            int xndHighIndex = (channel - 1) * 2 + 17;
            int xndLowIndex = xndHighIndex + 1;

            switch (paramType)
            {
                case 1:
                    return dacHighIndex;
                case 2:
                    return dacLowIndex;
                case 3:
                    return xndHighIndex;
                case 4:
                    return xndLowIndex;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// 获取八通道天线模式下，指定通道和参数类型对应的地址索引
        /// </summary>
        /// <param name="channel">通道号 (1-8)</param>
        /// <param name="paramType">参数类型：DAChigh=1, XNDhigh=3, XNDlow=4</param>
        /// <returns>地址索引</returns>
        public int GetAntennaAddressIndex(int channel, int paramType)
        {
            if (channel < 1 || channel > 8)
            {
                return 0;
            }

            // 重新映射通道号到正确的顺序：4, 1, 2, 3, 5, 6, 7, 8
            int mappedChannel = channel;
            switch (channel)
            {
                case 1: mappedChannel = 2; break;
                case 2: mappedChannel = 3; break;
                case 3: mappedChannel = 4; break;
                case 4: mappedChannel = 1; break;
                // 通道5-8保持不变
            }

            int dacHighIndex = mappedChannel;
            int xndHighIndex = mappedChannel * 2 + 7;
            int xndLowIndex = xndHighIndex + 1;

            switch (paramType)
            {
                case 1:
                    return dacHighIndex;
                case 3:
                    return xndHighIndex;
                case 4:
                    return xndLowIndex;
                default:
                    return 0;
            }
        }
    }
}
