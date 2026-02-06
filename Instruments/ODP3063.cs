namespace 五通道自动测试.Instruments
{
    public class ODP3063 : InstrumentBase
    {
        /// <summary>
        /// 设置第二路电源的电压
        /// </summary>
        /// <param name="voltage">电压值（V）</param>
        public void SetChannel2Voltage(double voltage)
        {
            Write($":SOUR2:VOLT {voltage}");
        }
        
        /// <summary>
        /// 设置第二路电源的电流
        /// </summary>
        /// <param name="current">电流值（A）</param>
        public void SetChannel2Current(double current)
        {
            Write($":SOUR2:CURR {current}");
        }
        
        /// <summary>
        /// 启用或禁用第二路电源输出
        /// </summary>
        /// <param name="enable">是否启用输出</param>
        public void EnableChannel2Output(bool enable)
        {
            Write($":OUTP2:STAT {(enable ? 1 : 0)}");
        }
        
        /// <summary>
        /// 查询第二路电源输出状态
        /// </summary>
        /// <returns>输出状态（true=开启，false=关闭）</returns>
        public bool QueryChannel2OutputStatus()
        {
            string result = Query(":OUTP2:STAT?");
            return result.Trim() == "1";
        }
        
        /// <summary>
        /// 设置第二路电源的过电压保护
        /// </summary>
        /// <param name="voltage">过电压保护值（V）</param>
        public void SetChannel2OverVoltageProtection(double voltage)
        {
            Write($":SOUR2:VOLT:PROT {voltage}");
        }
        
        /// <summary>
        /// 设置第二路电源的过电流保护
        /// </summary>
        /// <param name="current">过电流保护值（A）</param>
        public void SetChannel2OverCurrentProtection(double current)
        {
            Write($":SOUR2:CURR:PROT {current}");
        }
    }
}