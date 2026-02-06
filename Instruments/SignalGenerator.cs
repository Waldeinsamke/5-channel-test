namespace 五通道自动测试.Instruments
{
    public class SignalGenerator : InstrumentBase
    {
        /// <summary>
        /// 设置信号源频率
        /// </summary>
        /// <param name="frequency">频率（MHz）</param>
        public void SetFrequency(double frequency)
        {
            if (!EnsureConnection())
                throw new InvalidOperationException("Signal generator connection failed");
            Write($":FREQ {frequency} MHz");
        }
        
        /// <summary>
        /// 设置信号源功率
        /// </summary>
        /// <param name="power">功率（dBm）</param>
        public void SetPower(double power)
        {
            if (!EnsureConnection())
                throw new InvalidOperationException("Signal generator connection failed");
            Write($":POW {power} dBm");
        }
        
        /// <summary>
        /// 启用或禁用信号源输出
        /// </summary>
        /// <param name="enable">是否启用</param>
        public void EnableOutput(bool enable)
        {
            if (!EnsureConnection())
                throw new InvalidOperationException("Signal generator connection failed");
            Write($":OUTP {(enable ? 1 : 0)}");
        }
        
        /// <summary>
        /// 设置信号源调制类型
        /// </summary>
        /// <param name="modulationType">调制类型（AM, FM, PM等）</param>
        public void SetModulationType(string modulationType)
        {
            if (!EnsureConnection())
                throw new InvalidOperationException("Signal generator connection failed");
            Write($":MOD:TYPE {modulationType}");
        }
        
        /// <summary>
        /// 启用或禁用调制
        /// </summary>
        /// <param name="enable">是否启用</param>
        public void EnableModulation(bool enable)
        {
            if (!EnsureConnection())
                throw new InvalidOperationException("Signal generator connection failed");
            Write($":MOD:STAT {(enable ? 1 : 0)}");
        }
        
        /// <summary>
        /// 设置信号源的脉冲调制
        /// </summary>
        /// <param name="enable">是否启用</param>
        public void SetPulseModulation(bool enable)
        {
            if (!EnsureConnection())
                throw new InvalidOperationException("Signal generator connection failed");
            Write($":PULM:STAT {(enable ? 1 : 0)}");
        }
        
        /// <summary>
        /// 设置频率模式为扫描模式（SWE）
        /// </summary>
        public void SetFrequencyModeSweep()
        {
            if (!EnsureConnection())
                throw new InvalidOperationException("Signal generator connection failed");
            Write(":FREQ:MODE SWE");
        }
        
        /// <summary>
        /// 设置扫描起始频率
        /// </summary>
        /// <param name="frequencyHz">起始频率（Hz）</param>
        public void SetSweepStartFrequency(double frequencyHz)
        {
            if (!EnsureConnection())
                throw new InvalidOperationException("Signal generator connection failed");
            Write($":FREQ:STAR {frequencyHz}");
        }
        
        /// <summary>
        /// 设置扫描终止频率
        /// </summary>
        /// <param name="frequencyHz">终止频率（Hz）</param>
        public void SetSweepStopFrequency(double frequencyHz)
        {
            if (!EnsureConnection())
                throw new InvalidOperationException("Signal generator connection failed");
            Write($":FREQ:STOP {frequencyHz}");
        }
        
        /// <summary>
        /// 设置扫描点数
        /// </summary>
        /// <param name="points">扫描点数</param>
        public void SetSweepPoints(int points)
        {
            if (!EnsureConnection())
                throw new InvalidOperationException("Signal generator connection failed");
            Write($":SWE:POIN {points}");
        }
        
        /// <summary>
        /// 设置连续扫描模式
        /// </summary>
        /// <param name="enable">是否启用连续扫描</param>
        public void SetContinuousSweep(bool enable)
        {
            if (!EnsureConnection())
                throw new InvalidOperationException("Signal generator connection failed");
            Write($":INIT:CONT {(enable ? 1 : 0)}");
        }
    }
}