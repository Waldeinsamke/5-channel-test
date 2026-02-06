using System;
using System.Collections.Generic;

namespace 五通道自动测试.Calibration
{
    /// <summary>
    /// 校准逻辑类，处理校准逻辑和地址计算
    /// </summary>
    public class CalibrationLogic
    {
        private readonly CalibrationAddressCalculator _addressCalculator;
        private readonly Dictionary<ushort, byte> _calibrationDataCache;
        private readonly object _cacheLock;

        // 温度范围描述数组，用于UI显示
        public static readonly string[] TemperatureRanges = new string[]
        {
            "T<=-50", "-50<T<=-40", "-40<T<=-30", "-30<T<=-20", "-20<T<=-10", "-10<T<=0",
            "0<T<=10", "10<T<=20", "20<T<=30", "30<T<=40", "40<T<=50", "50<T<=60",
            "60<T<=70", "70<T<=80", "80<T<=90", "90<T"
        };

        // 温度范围边界值数组，用于将实际温度映射到对应的温度范围索引
        public static readonly (float min, float max)[] RangeBounds = new[]
        {
            (float.MinValue, -50f), (-50f, -40f), (-40f, -30f), (-30f, -20f),
            (-20f, -10f), (-10f, 0f), (0f, 10f), (10f, 20f), (20f, 30f),
            (30f, 40f), (40f, 50f), (50f, 60f), (60f, 70f), (70f, 80f),
            (80f, 90f), (90f, float.MaxValue)
        };

        /// <summary>
        /// 构造函数
        /// </summary>
        public CalibrationLogic()
        {
            _addressCalculator = new CalibrationAddressCalculator();
            _calibrationDataCache = new Dictionary<ushort, byte>();
            _cacheLock = new object();
        }

        /// <summary>
        /// 获取校准数据缓存
        /// </summary>
        public Dictionary<ushort, byte> CalibrationDataCache => _calibrationDataCache;

        /// <summary>
        /// 获取缓存锁对象
        /// </summary>
        public object CacheLock => _cacheLock;

        /// <summary>
        /// 根据频率索引获取对应的频率值
        /// </summary>
        /// <param name="frequencyIndex">频率索引 (1-4)</param>
        /// <returns>对应的频率值（MHz）</returns>
        public double GetFrequencyValue(int frequencyIndex)
        {
            return frequencyIndex switch
            {
                1 => 3330.0,
                2 => 3350.0,
                3 => 3370.0,
                4 => 3390.0,
                _ => 3330.0
            };
        }

        /// <summary>
        /// 根据实际温度值获取对应的温度范围描述
        /// </summary>
        /// <param name="temperature">实际温度值</param>
        /// <returns>温度范围描述字符串</returns>
        public string GetTemperatureRange(float temperature)
        {
            for (int i = 0; i < RangeBounds.Length; i++)
            {
                if (temperature >= RangeBounds[i].min && temperature <= RangeBounds[i].max)
                {
                    return TemperatureRanges[i];
                }
            }
            return "90<T";
        }

        /// <summary>
        /// 根据温度范围索引获取温度索引
        /// </summary>
        /// <param name="comboBoxIndex">下拉框选中索引</param>
        /// <returns>温度索引</returns>
        public int GetTemperatureIndex(int comboBoxIndex)
        {
            if (comboBoxIndex == 0)
                return 1;
            else if (comboBoxIndex == 1)
                return 2;
            else
                return 2 * comboBoxIndex;
        }

        /// <summary>
        /// 根据当前频率和温度索引计算校准地址
        /// </summary>
        /// <param name="frequencyIndex">频率索引</param>
        /// <param name="temperatureIndex">温度索引</param>
        public void CalculateNormalAddresses(int frequencyIndex, int temperatureIndex)
        {
            _addressCalculator.CalculateNormalAddresses(frequencyIndex, temperatureIndex);
        }

        /// <summary>
        /// 根据通道号获取起始索引
        /// </summary>
        /// <param name="channel">通道号 (1-5)</param>
        /// <returns>起始索引</returns>
        public int GetStartIndexByChannel(int channel)
        {
            return _addressCalculator.GetStartIndexByChannel(channel);
        }

        /// <summary>
        /// 获取指定索引的校准地址
        /// </summary>
        /// <param name="index">地址索引</param>
        /// <returns>地址字符串</returns>
        public string GetNormalAddress(int index)
        {
            return _addressCalculator.GetNormalAddress(index);
        }

        /// <summary>
        /// 解析地址字符串为ushort
        /// </summary>
        /// <param name="addressStr">地址字符串</param>
        /// <returns>解析后的地址</returns>
        public ushort ParseAddressString(string addressStr)
        {
            return _addressCalculator.ParseAddressString(addressStr);
        }

        /// <summary>
        /// 更新校准数据缓存
        /// </summary>
        /// <param name="address">EEPROM地址</param>
        /// <param name="value">值</param>
        public void UpdateCalibrationCache(ushort address, byte value)
        {
            lock (_cacheLock)
            {
                if (_calibrationDataCache.ContainsKey(address))
                {
                    _calibrationDataCache[address] = value;
                }
                else
                {
                    _calibrationDataCache.Add(address, value);
                }
            }
        }

        /// <summary>
        /// 从缓存中获取校准值
        /// </summary>
        /// <param name="address">EEPROM地址</param>
        /// <param name="value">输出值</param>
        /// <returns>是否找到</returns>
        public bool TryGetCalibrationValue(ushort address, out byte value)
        {
            lock (_cacheLock)
            {
                return _calibrationDataCache.TryGetValue(address, out value);
            }
        }
        
        /// <summary>
        /// 根据当前频率和温度索引计算天线模式地址
        /// </summary>
        /// <param name="frequencyIndex">频率索引</param>
        /// <param name="temperatureIndex">温度索引</param>
        public void CalculateAntennaAddresses(int frequencyIndex, int temperatureIndex)
        {
            _addressCalculator.CalculateAntennaAddresses(frequencyIndex, temperatureIndex);
        }
        
        /// <summary>
        /// 获取指定索引的天线模式校准地址
        /// </summary>
        /// <param name="index">地址索引</param>
        /// <returns>地址字符串</returns>
        public string GetAntennaAddress(int index)
        {
            return _addressCalculator.GetAntennaAddress(index);
        }
    }
}
