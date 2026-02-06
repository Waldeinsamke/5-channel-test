namespace 五通道自动测试.Test
{
    /// <summary>
    /// 测试项实体类
    /// </summary>
    public class TestItem
    {
        /// <summary>
        /// 测试项名称
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// 测试项描述
        /// </summary>
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// 标准值
        /// </summary>
        public double StandardValue { get; set; }
        
        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; } = string.Empty;
        
        /// <summary>
        /// 比较类型（如 "<", ">", "=="）
        /// </summary>
        public string ComparisonType { get; set; } = string.Empty;
        
        /// <summary>
        /// 测试频率（MHz）
        /// </summary>
        public double TestFrequency { get; set; }
        
        /// <summary>
        /// 测试带宽（kHz）
        /// </summary>
        public double TestBandwidth { get; set; }
        
        /// <summary>
        /// 判定测试结果是否合格
        /// </summary>
        /// <param name="actualValue">实际测试值</param>
        /// <returns>是否合格</returns>
        public bool IsPass(double actualValue)
        {
            switch (ComparisonType)
            {
                case "<":
                    return actualValue < StandardValue;
                case ">":
                    return actualValue > StandardValue;
                case "<=":
                    return actualValue <= StandardValue;
                case ">=":
                    return actualValue >= StandardValue;
                case "==":
                    return Math.Abs(actualValue - StandardValue) < 0.01; // 允许±0.01的误差
                default:
                    return false;
            }
        }
    }
}