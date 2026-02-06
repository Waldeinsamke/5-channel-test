namespace 五通道自动测试.Test
{
    /// <summary>
    /// 测试结果实体类
    /// </summary>
    public class TestResult
    {
        /// <summary>
        /// 通道号
        /// </summary>
        public int Channel { get; set; }
        
        /// <summary>
        /// 测试项名称
        /// </summary>
        public string TestItem { get; set; } = string.Empty;
        
        /// <summary>
        /// 测试值
        /// </summary>
        public double Value { get; set; }
        
        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; } = string.Empty;
        
        /// <summary>
        /// 是否合格
        /// </summary>
        public bool IsPass { get; set; }
        
        /// <summary>
        /// 测试时间
        /// </summary>
        public DateTime TestTime { get; set; }
        
        /// <summary>
        /// 标准值
        /// </summary>
        public double StandardValue { get; set; }
        
        /// <summary>
        /// 比较类型
        /// </summary>
        public string ComparisonType { get; set; } = string.Empty;
    }
}