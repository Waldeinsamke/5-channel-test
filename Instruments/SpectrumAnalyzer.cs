namespace 五通道自动测试.Instruments
{
    public class SpectrumAnalyzer : InstrumentBase
    {
        protected override bool EnableDelay => true;
        
        // 保存参考电平偏移值，用于重置后恢复
        private double _referenceLevelOffset = 0.0;
        
        /// <summary>
        /// 测量指定频率和带宽的功率
        /// </summary>
        /// <param name="frequency">频率（MHz）</param>
        /// <param name="bandwidth">带宽（kHz）</param>
        /// <returns>功率值（dBm）</returns>
        public double MeasurePower(double frequency, double bandwidth)
        {
            // 设置中心频率固定为70MHz（中频输出频率）
            Write(":FREQ:CENT 70 MHz");
                        
            // 设置扫描时间为自动
            Write(":SWE:TIME:AUTO ON");
            
            // 等待参数设置完成
            Thread.Sleep(100);
            
            // 打开标记1
            Write(":CALC:MARK1:STAT ON");
            // 设置标记1为峰值搜索模式
            Write(":CALC:MARK1:MAX:PEAK");
            // 执行峰值搜索
            Write(":CALC:MARK1:MAX:PEAK:SEARCH");
            
            // 等待峰值搜索完成
            Thread.Sleep(200);
            
            // 读取标记1的功率值
            return double.Parse(Query(":CALC:MARK1:Y?"));
        }
                
        /// <summary>
        /// 设置频谱仪为平均模式
        /// </summary>
        /// <param name="enable">是否启用平均模式</param>
        public void SetAverageMode(bool enable)
        {
            if (enable)
            {
                Write(":TRAC:MODE 1,AVG");
            }
            else
            {
                Write(":TRAC:MODE 1,WRIT");
            }
        }
        
        /// <summary>
        /// 设置频谱仪为最大保持模式
        /// </summary>
        /// <param name="enable">是否启用最大保持</param>
        public void SetMaxHold(bool enable)
        {
            if (enable)
            {
                Write(":TRACE1:MODE MAXH");
            }
            else
            {
                Write(":TRACE1:MODE WRIT");
            }
        }
        
        /// <summary>
        /// 设置频谱仪中心频率和跨度
        /// </summary>
        /// <param name="centerFrequency">中心频率（MHz）</param>
        /// <param name="span">扫描跨度（MHz）</param>
        public void SetFrequencySpan(double centerFrequency, double span)
        {
            Write($":FREQ:CENT {centerFrequency} MHz");
            Write($":FREQ:SPAN {span} MHz");
        }
        
        /// <summary>
        /// 设置分辨率带宽
        /// </summary>
        /// <param name="bandwidth">带宽（kHz）</param>
        public void SetResolutionBandwidth(double bandwidth)
        {
            Write($":BWID:RES {bandwidth} kHz");
        }
        
        /// <summary>
        /// 设置分辨率带宽为Auto
        /// </summary>
        public void SetResolutionBandwidthAuto()
        {
            Write(":BWID:RES:AUTO ON");
        }
        
        /// <summary>
        /// 设置视频带宽
        /// </summary>
        /// <param name="bandwidth">带宽（kHz）</param>
        public void SetVideoBandwidth(double bandwidth)
        {
            Write($":BWID:VID {bandwidth} kHz");
        }
        
        /// <summary>
        /// 设置视频带宽为Auto
        /// </summary>
        public void SetVideoBandwidthAuto()
        {
            Write(":BWID:VID:AUTO ON");
        }
        
        /// <summary>
        /// 清除频谱仪显示
        /// </summary>
        public void ClearDisplay()
        {
            Write(":DISP:WIND:TRAC:CLE");
        }
        
        /// <summary>
        /// 打开标记1并执行峰值搜索
        /// </summary>
        /// <returns>标记1的功率值（dBm）</returns>
        public double MeasureMarkerPeak()
        {
            Write(":CALC:MARK1:STAT ON");
            Write(":CALC:MARK1:MAX:PEAK");
            Thread.Sleep(200);
            return double.Parse(Query(":CALC:MARK1:Y?"));
        }
        
        /// <summary>
        /// 读取当前标记点的功率值（不执行峰值搜索）
        /// </summary>
        /// <returns>标记点的功率值（dBm）</returns>
        public double ReadCurrentMarkerValue()
        {
            Write(":CALC:MARK1:STAT ON");
            Thread.Sleep(100);
            return double.Parse(Query(":CALC:MARK1:Y?"));
        }
        
        /// <summary>
        /// 设置DELTA标记并读取差值
        /// </summary>
        /// <param name="markerFreq">标记频率（MHz）</param>
        /// <returns>与参考点的差值（dB）</returns>
        public double ReadDeltaValue(double markerFreq)
        {
            // 设置delta标记相对于标记1的位置
            Write(":CALC:MARK1:MODE DELTA");
            Write($":CALC:MARK1:X {markerFreq} MHz");
                        
            // 等待标记切换完成
            Thread.Sleep(500);
            
            // 再次确认标记位置
            Write($":CALC:MARK1:X {markerFreq} MHz");
            
            return double.Parse(Query(":CALC:MARK1:Y?"));
        }

        /// <summary>
        /// 设置DELTA标记
        /// </summary>
        /// <param name="markerFreq">标记频率（MHz）</param>
        /// <returns>与参考点的差值（dB）</returns>
        public double SetDeltaValue()
        {
            // 设置delta标记相对于标记1的位置
            Write(":CALC:MARK1:MODE DELTA");
                        
            // 等待标记切换完成
            Thread.Sleep(500);
            
            return double.Parse(Query(":CALC:MARK1:Y?"));
        }
        
        /// <summary>
        /// 设置扫描时间
        /// </summary>
        /// <param name="seconds">扫描时间（秒）</param>
        public void SetSweepTime(double seconds)
        {
            Write($":SWE:TIME {seconds} s");
        }
        
        /// <summary>
        /// 打开视频触发
        /// </summary>
        public void EnableVideoTrigger()
        {
            Write(":TRIG:SOUR VID");
            Write(":TRIG:LEV:AUTO ON");
        }
        
        /// <summary>
        /// 开启峰值自动搜索
        /// </summary>
        public void EnablePeakAutoSearch()
        {
            Write(":CALC:MARK:MAX:AUTO ON");
        }
        
        /// <summary>
        /// 设置参考电平
        /// </summary>
        /// <param name="leveldBm">参考电平（dBm）</param>
        public void SetReferenceLevel(double leveldBm)
        {
            Write($":DISP:WIND:TRAC:Y:RLEV {leveldBm} dBm");
        }
        
        /// <summary>
        /// 设置参考电平偏移
        /// </summary>
        /// <param name="offsetdBm">参考电平偏移（dBm）</param>
        public void SetReferenceLevelOffset(double offsetdBm)
        {
            Write($":DISP:WIND:TRAC:Y:RLEV:OFFS {offsetdBm}");
            _referenceLevelOffset = offsetdBm;
        }
        
        /// <summary>
        /// 设置扫描模式
        /// </summary>
        /// <param name="continuous">true=连续扫描CONT，false=单次扫描SING</param>
        public void SetContinuousScan(bool continuous)
        {
            Write(continuous ? ":INIT:CONT ON" : ":INIT:CONT OFF");
        }
        
        /// <summary>
        /// 峰峰值搜索（Peak-to-Peak Search）
        /// 用于在标记的频谱范围内自动找到最高和最低峰值，并可能将两个标记分别置于其上。
        /// </summary>
        /// <returns>峰峰值（dB）</returns>
        public double ReadPeakToPeak()
        {
            Write(":CALC:MARK:PTP");
            Thread.Sleep(500);
            return double.Parse(Query(":CALC:MARK:Y?"));
        }
        
        /// <summary>
        /// 进入谐波测试模式
        /// </summary>
        public void InitializeHarmonicsTest()
        {
            Write(":INITiate:HARMonics");
            Thread.Sleep(2000);
        }
        
        /// <summary>
        /// 读取谐波幅度
        /// </summary>
        /// <returns>谐波值数组</returns>
        public double[] FetchHarmonicsAmplitude()
        {
            string response = Query(":FETCh:HARMonics:AMPLitude:ALL?");
            string[] parts = response.Split(',');
            return parts.Select(p => double.TryParse(p, out double val) ? val : 0).ToArray();
        }
        
        /// <summary>
        /// 获取二次谐波值
        /// </summary>
        /// <returns>二次谐波值（dBc）</returns>
        public double GetSecondHarmonicValue()
        {
            double[] harmonics = FetchHarmonicsAmplitude();
            if (harmonics.Length >= 2)
            {
                return Math.Round(harmonics[1], 1);
            }
            return 0;
        }

        /// <summary>
        /// 读取标记值
        /// </summary>
        /// <returns>delta值（dB）</returns>
        public double ReadMarkerValue()
        {
            return double.Parse(Query(":CALC:MARK1:Y?"));
        }

        /// <summary>
        /// 读取标记值
        /// </summary>
        /// <returns>关闭所有标记</returns>
        public void SetAllMarkOFF()
        {
            Write(":CALC:MARK1:AOFF");
        }
        
        /// <summary>
        /// 重写Reset方法，在重置后恢复参考电平偏移设置
        /// </summary>
        public override void Reset()
        {
            // 调用基类Reset方法执行仪表重置
            base.Reset();
            
            // 重置完成后，使用保存的值重新设置参考电平偏移
            SetReferenceLevelOffset(_referenceLevelOffset);
        }
    }
}