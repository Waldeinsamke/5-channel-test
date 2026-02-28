namespace 五通道自动测试.Instruments
{
    public class ZNB8 : InstrumentBase
    {
        /// <summary>
        /// 设置中心频率
        /// </summary>
        /// <param name="frequency">频率（MHz）</param>
        public void SetFrequency(double frequency)
        {
            Write($":FREQ:CENT {frequency} MHz");
        }
        
        /// <summary>
        /// 设置频率跨度
        /// </summary>
        /// <param name="span">跨度（MHz）</param>
        public void SetSpan(double span)
        {
            Write($":FREQ:SPAN {span} MHz");
        }
        
        /// <summary>
        /// 设置起始频率
        /// </summary>
        /// <param name="frequency">频率（MHz）</param>
        public void SetStartFrequency(double frequency)
        {
            Write($":FREQ:STAR {frequency} MHz");
        }
        
        /// <summary>
        /// 设置终止频率
        /// </summary>
        /// <param name="frequency">频率（MHz）</param>
        public void SetStopFrequency(double frequency)
        {
            Write($":FREQ:STOP {frequency} MHz");
        }
        
        /// <summary>
        /// 设置参考电平
        /// </summary>
        /// <param name="level">电平（dBm）</param>
        public void SetReferenceLevel(double level)
        {
            Write($":DISP:WIND:TRAC:Y:RLEV {level} dBm");
        }
        
        /// <summary>
        /// 设置扫描点数
        /// </summary>
        /// <param name="points">点数</param>
        public void SetSweepPoints(int points)
        {
            Write($":SWE:POIN {points}");
        }
        
        /// <summary>
        /// 设置扫描时间
        /// </summary>
        /// <param name="time">时间（秒）</param>
        public void SetSweepTime(double time)
        {
            Write($":SWE:TIME {time} s");
        }
        
        /// <summary>
        /// 启用连续扫描
        /// </summary>
        /// <param name="enable">是否启用</param>
        public void SetContinuousScan(bool enable)
        {
            Write($":INIT:CONT {(enable ? 1 : 0)}");
        }
        
        /// <summary>
        /// 触发单次扫描
        /// </summary>
        public void TriggerSingleScan()
        {
            Write(":INIT:CONT 0");
            Write(":INIT");
        }
        
        /// <summary>
        /// 读取S11参数（dB）
        /// </summary>
        /// <returns>S11值（dB）</returns>
        public double ReadS11()
        {
            Write(":CALC1:PAR:SEL 'Trc1'");
            return double.Parse(Query(":CALC1:DATA? FDATA"));
        }
        
        /// <summary>
        /// 读取S21参数（dB）
        /// </summary>
        /// <returns>S21值（dB）</returns>
        public double ReadS21()
        {
            Write(":CALC2:PAR:SEL 'Trc2'");
            return double.Parse(Query(":CALC2:DATA? FDATA"));
        }
        
        /// <summary>
        /// 执行峰值搜索
        /// </summary>
        /// <returns>峰值（dB）</returns>
        public double MeasurePeak()
        {
            Write(":CALC:MARK1:STAT ON");
            Write(":CALC:MARK1:MAX");
            Write(":CALC:MARK1:MAX:PEAK:SEARCH");
            return double.Parse(Query(":CALC:MARK1:Y?"));
        }
        
        /// <summary>
        /// 设置迹线平均
        /// </summary>
        /// <param name="enable">是否启用</param>
        /// <param name="count">平均次数</param>
        public void SetAveraging(bool enable, int count)
        {
            Write($":AVER:STAT {(enable ? 1 : 0)}");
            if (enable)
            {
                Write($":AVER:COUN {count}");
            }
        }
        
        /// <summary>
        /// 开启新平均
        /// </summary>
        public void NewAveraging()
        {
            Write(":AVER:CLE");
        }
        
        /// <summary>
        /// 设置输出功率
        /// </summary>
        /// <param name="power">功率（dBm）</param>
        public void SetOutputPower(double power)
        {
            Write($":SOUR:POW {power} dBm");
        }
        
        /// <summary>
        /// 启用输出
        /// </summary>
        /// <param name="enable">是否启用</param>
        public void EnableOutput(bool enable)
        {
            Write($":OUTP {(enable ? 1 : 0)}");
        }

        /// <summary>
        /// 选择轨迹（Trc1或Trc2）
        /// </summary>
        /// <param name="traceName">轨迹名称，如"Trc1"或"Trc2"</param>
        public void SelectTrace(string traceName)
        {
            Write($":CALCulate1:PARameter:SELect '{traceName}'");
        }

        /// <summary>
        /// 选择轨迹1（相位）
        /// </summary>
        public void SelectTrace1()
        {
            SelectTrace("Trc1");
        }

        /// <summary>
        /// 选择轨迹2（幅度）
        /// </summary>
        public void SelectTrace2()
        {
            SelectTrace("Trc2");
        }

        /// <summary>
        /// 读取轨迹1的mark1值（相位）
        /// </summary>
        /// <returns>相位mark值</returns>
        public double ReadTrace1Mark()
        {
            SelectTrace1();
            Write(":CALCulate1:MARKer1:STAT ON");
            return double.Parse(Query(":CALCulate1:MARKer1:Y?"));
        }

        /// <summary>
        /// 读取轨迹2的mark1值（幅度）
        /// </summary>
        /// <returns>幅度mark值</returns>
        public double ReadTrace2Mark()
        {
            SelectTrace2();
            Write(":CALCulate1:MARKer1:STAT ON");
            return double.Parse(Query(":CALCulate1:MARKer1:Y?"));
        }
    }
}
