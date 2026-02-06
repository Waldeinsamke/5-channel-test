using 五通道自动测试.Instruments;
using System.ComponentModel;
using System.Threading;

namespace 五通道自动测试.Test
{
    /// <summary>
    /// 测试管理类，负责管理测试流程和结果
    /// </summary>
    public class TestManager
    {
        private readonly InstrumentManager _instrumentManager;
        private readonly List<TestResult> _testResults;
        private readonly Dictionary<string, TestItem> _testItems;
        private readonly SynchronizationContext? _syncContext;
        private CancellationTokenSource? _cts;
        
        /// <summary>
        /// 测试完成事件
        /// </summary>
        public event EventHandler<TestResult>? TestCompleted;
        
        /// <summary>
        /// 日志生成事件
        /// </summary>
        public event EventHandler<string>? LogGenerated;
        
        /// <summary>
        /// 显示确认对话框事件（用于跨线程弹窗）
        /// </summary>
        public event EventHandler<string>? ShowConfirmDialog;
        
        /// <summary>
        /// 通道切换事件
        /// </summary>
        public event Action<int>? ChannelChanging;
        
        /// <summary>
        /// 线缆损耗 L1（dB）
        /// </summary>
        public double CableLossL1 { get; set; } = 0.0;
        
        /// <summary>
        /// 线缆损耗 L2（dB）
        /// </summary>
        public double CableLossL2 { get; set; } = 0.0;
        
        /// <summary>
        /// 参考电平偏移（dBm）
        /// </summary>
        public double RefLevelOffset { get; set; } = 0.0;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="instrumentManager">仪表管理器实例</param>
        public TestManager(InstrumentManager instrumentManager)
        {
            _instrumentManager = instrumentManager;
            _testResults = new List<TestResult>();
            _testItems = InitializeTestItems();
            _syncContext = SynchronizationContext.Current;
        }
        
        /// <summary>
        /// 显示确认对话框（跨线程安全）
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <returns>对话框结果</returns>
        public bool? ShowConfirm(string message)
        {
            if (_syncContext != null)
            {
                bool? result = null;
                _syncContext.Send(_ =>
                {
                    result = MessageBox.Show(message, "端口驻波测试", MessageBoxButtons.OKCancel) == DialogResult.OK;
                }, null);
                return result;
            }
            return MessageBox.Show(message, "端口驻波测试", MessageBoxButtons.OKCancel) == DialogResult.OK;
        }
        
        /// <summary>
        /// 初始化测试项
        /// </summary>
        /// <returns>测试项字典</returns>
        private Dictionary<string, TestItem> InitializeTestItems()
        {
            return new Dictionary<string, TestItem>
            {
                { "动态范围及中频输出信号测试", new TestItem
                    {
                        Name = "动态范围及中频输出信号测试",
                        Description = "验证接收通道在指定输入功率范围内的输出响应及中频输出信号幅度",
                        StandardValue = 0.0,
                        Unit = "dBm",
                        ComparisonType = "",
                        TestFrequency = 3330.0,
                        TestBandwidth = 10.0
                    }
                },
                { "通道增益测试", new TestItem
                    {
                        Name = "通道增益测试",
                        Description = "测量接收通道的固定增益及衰减步进精度",
                        StandardValue = 0.0,
                        Unit = "dB",
                        ComparisonType = "",
                        TestFrequency = 3330.0,
                        TestBandwidth = 10.0
                    }
                },
                { "带外抑制测试", new TestItem
                    {
                        Name = "带外抑制测试",
                        Description = "测量通道对工作频率带外信号的抑制能力",
                        StandardValue = 0.0,
                        Unit = "dB",
                        ComparisonType = "",
                        TestFrequency = 3330.0,
                        TestBandwidth = 70.0
                    }
                },
                { "带内增益波动测试", new TestItem
                    {
                        Name = "带内增益波动测试",
                        Description = "测量通道在5MHz带宽内的增益平坦度",
                        StandardValue = 0.0,
                        Unit = "dB",
                        ComparisonType = "",
                        TestFrequency = 3330.0,
                        TestBandwidth = 5.0
                    }
                },
                { "镜像抑制测试", new TestItem
                    {
                        Name = "镜像抑制测试",
                        Description = "测量通道对镜像频率信号的抑制能力",
                        StandardValue = 0.0,
                        Unit = "dB",
                        ComparisonType = "",
                        TestFrequency = 3330.0,
                        TestBandwidth = 10.0
                    }
                },
                { "谐波抑制测试", new TestItem
                    {
                        Name = "谐波抑制测试",
                        Description = "测量中频输出信号的谐波抑制水平",
                        StandardValue = 0.0,
                        Unit = "dBc",
                        ComparisonType = "",
                        TestFrequency = 3330.0,
                        TestBandwidth = 50.0
                    }
                },
                { "邻道抑制测试", new TestItem
                    {
                        Name = "邻道抑制测试",
                        Description = "测量通道对相邻信道干扰信号的抑制能力",
                        StandardValue = 0.0,
                        Unit = "dB",
                        ComparisonType = "",
                        TestFrequency = 3330.0,
                        TestBandwidth = 50.0
                    }
                },
                { "端口驻波测试（输入）", new TestItem
                    {
                        Name = "端口驻波测试（输入）",
                        Description = "测量接收机输入驻波",
                        StandardValue = 0.0,
                        Unit = "",
                        ComparisonType = "",
                        TestFrequency = 3330.0,
                        TestBandwidth = 10.0
                    }
                },
                { "校准开关隔离度测试", new TestItem
                    {
                        Name = "校准开关隔离度测试",
                        Description = "测量校准开关的隔离度指标",
                        StandardValue = 0.0,
                        Unit = "dB",
                        ComparisonType = "",
                        TestFrequency = 3330.0,
                        TestBandwidth = 50.0
                    }
                }
            };
        }
        
        /// <summary>
        /// 异步执行测试
        /// </summary>
        /// <param name="channel">通道号</param>
        /// <param name="testItemName">测试项名称</param>
        /// <param name="cancellationToken">取消令牌（可选）</param>
        /// <returns>测试结果</returns>
        public async Task RunTestAsync(int channel, string testItemName, CancellationToken? cancellationToken = null)
        {
            if (!cancellationToken.HasValue)
            {
                // 单测试模式：创建新的取消令牌源
                _cts = new CancellationTokenSource();
                cancellationToken = _cts.Token;
            }
            
            try
            {
                // 记录日志
                LogGenerated?.Invoke(this, $"开始测试：通道{channel} - {testItemName}");
                
                // 检查仪表连接
                if (!_instrumentManager.AreAllInstrumentsConnected())
                {
                    LogGenerated?.Invoke(this, "错误：频谱仪或信号源未连接");
                    return;
                }
                
                // 获取测试项
                if (!_testItems.TryGetValue(testItemName, out var testItem))
                {
                    LogGenerated?.Invoke(this, $"错误：测试项 {testItemName} 不存在");
                    return;
                }
                
                // 设置频谱仪参考电平偏移
                _instrumentManager.SpectrumAnalyzer.SetReferenceLevelOffset(RefLevelOffset);
                LogGenerated?.Invoke(this, $"已设置频谱仪参考电平偏移：{RefLevelOffset} dBm");
                
                // 根据测试项执行不同的测试流程
                if (testItemName == "动态范围及中频输出信号测试")
                {
                    await RunDynamicRangeTestAsync(channel, testItem, cancellationToken.Value);
                }
                else if (testItemName == "通道增益测试")
                {
                    await RunChannelGainTestAsync(channel, testItem, cancellationToken.Value);
                }
                else if (testItemName == "带外抑制测试")
                {
                    await RunOutOfBandRejectionTestAsync(channel, testItem, cancellationToken.Value);
                }
                else if (testItemName == "带内增益波动测试")
                {
                    await RunGainFlatnessTestAsync(channel, testItem, cancellationToken.Value);
                }
                else if (testItemName == "镜像抑制测试")
                {
                    await RunImageRejectionTestAsync(channel, testItem, cancellationToken.Value);
                }
                else if (testItemName == "谐波抑制测试")
                {
                    await RunHarmonicSuppressionTestAsync(channel, testItem, cancellationToken.Value);
                }
                else if (testItemName == "邻道抑制测试")
                {
                    await RunAdjacentChannelSuppressionTestAsync(channel, testItem, cancellationToken.Value);
                }
                else if (testItemName == "端口驻波测试（输入）")
                {
                    await RunPortVSWRInputTestAsync(channel, testItem, cancellationToken.Value);
                }
                else if (testItemName == "校准开关隔离度测试")
                {
                    await RunCalibrationSwitchIsolationTestAsync(channel, testItem, cancellationToken.Value);
                }
                else
                {
                    LogGenerated?.Invoke(this, $"错误：不支持的测试项 {testItemName}");
                    return;
                }
            }
            catch (OperationCanceledException)
            {
                LogGenerated?.Invoke(this, $"测试已取消：通道{channel} - {testItemName}");
                // 清理资源
                _instrumentManager.SignalGenerator.EnableOutput(false);
                
                // 在批量测试模式下，重新抛出异常以中断批量测试
                if (cancellationToken.HasValue)
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                LogGenerated?.Invoke(this, $"测试错误：通道{channel} - {testItemName}，错误信息：{ex.Message}");
            }
            finally
            {
                // 只在单测试模式下释放取消令牌源（批量测试模式下由 RunBatchTestAsync 管理）
                if (!cancellationToken.HasValue && _cts != null)
                {
                    _cts.Dispose();
                    _cts = null;
                }
            }
        }
        
        /// <summary>
        /// 执行动态范围及中频输出信号测试
        /// </summary>
        /// <param name="channel">通道号</param>
        /// <param name="testItem">测试项</param>
        /// <param name="cancellationToken">取消令牌</param>
        private async Task RunDynamicRangeTestAsync(int channel, TestItem testItem, CancellationToken cancellationToken)
        {
            // 重置所有仪表
            _instrumentManager.ResetAll();
            
            // 测试频率列表：3330MHz、3350MHz、3370MHz、3390MHz
            double[] testFrequencies = { 3330.0, 3350.0, 3370.0, 3390.0 };
            
            // 设置频谱仪参数：Center 70MHz，Span 10MHz，RBW/VBW Auto，reflevel 13dBm
            _instrumentManager.SpectrumAnalyzer.SetFrequencySpan(70, 10);
            _instrumentManager.SpectrumAnalyzer.SetVideoBandwidthAuto();
            _instrumentManager.SpectrumAnalyzer.SetResolutionBandwidth(10);
            _instrumentManager.SpectrumAnalyzer.EnablePeakAutoSearch();
            _instrumentManager.SpectrumAnalyzer.SetReferenceLevel(13);
            LogGenerated?.Invoke(this, "频谱仪已设置：中心频率70MHz，Span 50MHz，RBW/VBW Auto，峰值自动搜索");
            
            // 信号源初始化设置
            _instrumentManager.SignalGenerator.EnableModulation(false);
            _instrumentManager.SignalGenerator.SetPulseModulation(false);
            _instrumentManager.SignalGenerator.EnableOutput(true);
            LogGenerated?.Invoke(this, "信号源初始化完成，RF输出已开启");
            
            // 遍历所有测试频率
            foreach (double frequency in testFrequencies)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                LogGenerated?.Invoke(this, $"开始测试频率：{frequency} MHz，通道：{channel}");
                
                // 1. 设置接收机通道
                LogGenerated?.Invoke(this, $"切换到通道{channel}");
                
                // 2. 设置接收机衰减为0dB，工作频率为测试频率
                LogGenerated?.Invoke(this, $"设置接收机衰减为0dB，工作频率为{frequency} MHz");
                
                // 3. 信号源设置
                double signalPower = -90.0 + CableLossL2;
                LogGenerated?.Invoke(this, $"信号源输出{frequency} MHz，功率为{signalPower} dBm");
                
                _instrumentManager.SignalGenerator.SetFrequency(frequency);
                _instrumentManager.SendFrequencyCommand(frequency);
                _instrumentManager.SignalGenerator.SetPower(signalPower);
                
                // 等待信号稳定
                await Task.Delay(600, cancellationToken);
                
                
                double A = _instrumentManager.SpectrumAnalyzer.MeasurePower(frequency, 0);
                double actualPower = A + CableLossL1;
                LogGenerated?.Invoke(this, $"频谱仪读数A：{A} dBm，实际输出功率：{actualPower} dBm");
                
                // 记录测试结果
                RecordTestResult(channel, testItem.Name, actualPower, $"频率{frequency}MHz，衰减0dB，输入功率-90dBm");
                
                // 4. 改变信号源功率为(-50 + L2) dBm，记录读数A1，计算输出功率(A1+L1)
                signalPower = -50.0 + CableLossL2;
                LogGenerated?.Invoke(this, $"信号源输出功率调整为{signalPower} dBm");
                
                _instrumentManager.SignalGenerator.SetPower(signalPower);
                await Task.Delay(500, cancellationToken);
                
                double A1 = _instrumentManager.SpectrumAnalyzer.MeasurePower(frequency, 0);
                double actualPower1 = A1 + CableLossL1;
                LogGenerated?.Invoke(this, $"频谱仪读数A1：{A1} dBm，实际输出功率：{actualPower1} dBm");
                
                // 记录测试结果
                RecordTestResult(channel, testItem.Name, actualPower1, $"频率{frequency}MHz，衰减0dB，输入功率-50dBm");
                
                // 5. 设置产品衰减为36dB，信号源功率为(-10 + L2) dBm，记录读数A2
                _instrumentManager.SendAttenuationCommand(36);
                LogGenerated?.Invoke(this, "设置产品衰减为36dB");
                await Task.Delay(200, cancellationToken);

                signalPower = -10.0 + CableLossL2;
                LogGenerated?.Invoke(this, $"信号源输出功率调整为{signalPower} dBm");
                
                _instrumentManager.SignalGenerator.SetPower(signalPower);
                await Task.Delay(1000, cancellationToken);
                
                double A2 = _instrumentManager.SpectrumAnalyzer.MeasurePower(frequency, 0);
                double actualPower2 = A2 + CableLossL1;
                LogGenerated?.Invoke(this, $"频谱仪读数A2：{A2} dBm，实际输出功率：{actualPower2} dBm");
                
                // 记录测试结果
                RecordTestResult(channel, testItem.Name, actualPower2, $"频率{frequency}MHz，衰减36dB，输入功率-10dBm");
                
                // 6. 设置产品衰减为0dB（验证中频输出），记录读数A3
                _instrumentManager.SendAttenuationCommand(0);
                LogGenerated?.Invoke(this, "设置产品衰减为0dB（验证中频输出）");
                signalPower = -15.0 + CableLossL2;
                LogGenerated?.Invoke(this, $"信号源输出功率调整为{signalPower} dBm");
                await Task.Delay(500, cancellationToken);
                
                double A3 = _instrumentManager.SpectrumAnalyzer.MeasurePower(frequency, 0);
                double actualPower3 = A3 + CableLossL1;
                LogGenerated?.Invoke(this, $"频谱仪读数A3：{A3} dBm，实际输出功率：{actualPower3} dBm");
                
                // 记录测试结果
                RecordTestResult(channel, testItem.Name, actualPower3, $"频率{frequency}MHz，衰减0dB，验证中频输出");
                
                LogGenerated?.Invoke(this, $"频率{frequency} MHz测试完成");

                // 关闭所有标记
                _instrumentManager.SpectrumAnalyzer.SetAllMarkOFF();
            }
            
            // 测试完成，关闭信号源输出
            _instrumentManager.SignalGenerator.EnableOutput(false);
            
            // 测试完成
            LogGenerated?.Invoke(this, $"测试完成：通道{channel} - {testItem.Name}");
        }
        
        /// <summary>
        /// 执行镜像抑制测试
        /// </summary>
        /// <param name="channel">通道号</param>
        /// <param name="testItem">测试项</param>
        /// <param name="cancellationToken">取消令牌</param>
        private async Task RunImageRejectionTestAsync(int channel, TestItem testItem, CancellationToken cancellationToken)
        {
            // 1. 重置所有仪表
            _instrumentManager.ResetAll();
            
            // 2. 频谱仪设置：Center 70MHz，Span 10MHz，RBW 10kHz，VBW Auto
            _instrumentManager.SpectrumAnalyzer.SetFrequencySpan(70, 10);
            _instrumentManager.SpectrumAnalyzer.SetResolutionBandwidth(10);
            _instrumentManager.SpectrumAnalyzer.SetVideoBandwidthAuto();
            LogGenerated?.Invoke(this, "频谱仪已设置：Center 70MHz，Span 10MHz，RBW 10kHz，VBW Auto");
            
            // 3. 信号源初始化（Reset后自动为FIX模式）
            _instrumentManager.SignalGenerator.EnableModulation(false);
            _instrumentManager.SignalGenerator.EnableOutput(true);
            LogGenerated?.Invoke(this, "信号源初始化完成，RF输出已开启");
            
            // 频率对应关系：工作频率 → 镜像频率
            Dictionary<double, double> frequencyMap = new Dictionary<double, double>
            {
                { 3330.0, 2390.0 },
                { 3350.0, 2410.0 },
                { 3370.0, 2430.0 },
                { 3390.0, 2450.0 }
            };
            
            foreach (var freqPair in frequencyMap)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                double workFreq = freqPair.Key;
                double mirrorFreq = freqPair.Value;
                
                // 设置工作频率，信号源输出
                _instrumentManager.SignalGenerator.SetFrequency(workFreq);
                double signalPower = -50.0 + CableLossL2;
                _instrumentManager.SignalGenerator.SetPower(signalPower);
                
                // 发送工装板频率切换命令
                _instrumentManager.SendFrequencyCommand(workFreq);
                await Task.Delay(500, cancellationToken);
                
                // 峰值搜索，读取A值
                double A = _instrumentManager.SpectrumAnalyzer.MeasureMarkerPeak();
                LogGenerated?.Invoke(this, $"工作频率{workFreq}MHz，峰值搜索读数A：{A:F2} dBm");
                
                // 切换到镜像频率
                _instrumentManager.SignalGenerator.SetFrequency(mirrorFreq);
                await Task.Delay(500, cancellationToken);
                
                // 读取当前mark值，数值B
                double B = _instrumentManager.SpectrumAnalyzer.ReadCurrentMarkerValue();
                LogGenerated?.Invoke(this, $"镜像频率{mirrorFreq}MHz，mark读数B：{B:F2} dBm");
                
                // 计算镜像抑制
                double rejection = A - B;
                LogGenerated?.Invoke(this, $"镜像抑制：A - B = {rejection:F2} dB");
                
                // 记录测试结果
                RecordTestResult(channel, testItem.Name, rejection, "dB", $"工作频率{workFreq}MHz");

                // 关闭所有标记
                _instrumentManager.SpectrumAnalyzer.SetAllMarkOFF();
            }
            
            // 关闭信号源
            _instrumentManager.SignalGenerator.EnableOutput(false);
            LogGenerated?.Invoke(this, $"测试完成：通道{channel} - {testItem.Name}");
        }
        
        /// <summary>
        /// 执行谐波抑制测试
        /// </summary>
        /// <param name="channel">通道号</param>
        /// <param name="testItem">测试项</param>
        /// <param name="cancellationToken">取消令牌</param>
        private async Task RunHarmonicSuppressionTestAsync(int channel, TestItem testItem, CancellationToken cancellationToken)
        {
            // 1. 重置所有仪表
            _instrumentManager.ResetAll();
            
            // 2. 频谱仪设置：Center 70MHz，Span 50MHz，RBW/VBW Auto
            _instrumentManager.SpectrumAnalyzer.SetFrequencySpan(70, 50);
            _instrumentManager.SpectrumAnalyzer.SetReferenceLevel(10);
            _instrumentManager.SpectrumAnalyzer.SetResolutionBandwidthAuto();
            _instrumentManager.SpectrumAnalyzer.SetVideoBandwidthAuto();
            LogGenerated?.Invoke(this, "频谱仪已设置：Center 70MHz，Span 50MHz，RBW/VBW Auto");
            
            // 3. 信号源初始化
            _instrumentManager.SignalGenerator.EnableModulation(false);
            _instrumentManager.SignalGenerator.EnableOutput(true);
            LogGenerated?.Invoke(this, "信号源初始化完成，RF输出已开启");
            
            // 测试频率列表：3330MHz、3350MHz、3370MHz、3390MHz
            double[] testFrequencies = { 3330.0, 3350.0, 3370.0, 3390.0 };
            
            foreach (double frequency in testFrequencies)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                LogGenerated?.Invoke(this, $"开始测试频率：{frequency} MHz，通道：{channel}");
                
                // 4. 设置信号源频率和功率，接收机设置衰减0dBm
                _instrumentManager.SendAttenuationCommand(0);
                _instrumentManager.SignalGenerator.SetFrequency(frequency);
                double signalPower = -50.0 + CableLossL2;
                _instrumentManager.SignalGenerator.SetPower(signalPower);
                LogGenerated?.Invoke(this, $"信号源已设置：{frequency} MHz，功率{signalPower} dBm");
                
                // 5. 发送工装板频率切换命令
                _instrumentManager.SendFrequencyCommand(frequency);
                await Task.Delay(500, cancellationToken);
                
                // 6. 读取二次谐波值
                _instrumentManager.SpectrumAnalyzer.InitializeHarmonicsTest();
                double harmonic = _instrumentManager.SpectrumAnalyzer.GetSecondHarmonicValue();
                LogGenerated?.Invoke(this, $"二次谐波值：{harmonic:F1} dBc");
                
                // 记录测试结果
                RecordTestResult(channel, testItem.Name, harmonic, "dBc", $"工作频率{frequency}MHz");
                
                LogGenerated?.Invoke(this, $"频率{frequency} MHz测试完成");
            }
            
            // 关闭信号源RF输出
            _instrumentManager.SignalGenerator.EnableOutput(false);
            LogGenerated?.Invoke(this, $"测试完成：通道{channel} - {testItem.Name}");
        }
        
        /// <summary>
        /// 执行邻道抑制测试
        /// </summary>
        /// <param name="channel">通道号</param>
        /// <param name="testItem">测试项</param>
        /// <param name="cancellationToken">取消令牌</param>
        private async Task RunAdjacentChannelSuppressionTestAsync(int channel, TestItem testItem, CancellationToken cancellationToken)
        {
            // 1. 重置所有仪表
            _instrumentManager.ResetAll();
            
            // 2. 频谱仪设置：Center 70MHz，Span 50MHz，Ref Level -10dBm，RBW 30kHz
            _instrumentManager.SpectrumAnalyzer.SetFrequencySpan(70, 50);
            _instrumentManager.SpectrumAnalyzer.SetReferenceLevel(0);
            _instrumentManager.SpectrumAnalyzer.SetResolutionBandwidth(30);
            _instrumentManager.SpectrumAnalyzer.SetVideoBandwidthAuto();
            LogGenerated?.Invoke(this, "频谱仪已设置：Center 70MHz，Span 50MHz，Ref -10dBm，RBW 30kHz，VBW Auto");
            
            // 3. 信号源初始化
            _instrumentManager.SignalGenerator.EnableModulation(false);
            _instrumentManager.SignalGenerator.EnableOutput(true);
            LogGenerated?.Invoke(this, "信号源初始化完成，RF输出已开启");
            
            // 测试频率列表：3330MHz、3350MHz、3370MHz、3390MHz
            double[] testFrequencies = { 3330.0, 3350.0, 3370.0, 3390.0 };
            
            foreach (double frequency in testFrequencies)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                LogGenerated?.Invoke(this, $"开始测试频率：{frequency} MHz，通道：{channel}");
                
                // 4. 设置信号源到工作频率
                _instrumentManager.SignalGenerator.SetFrequency(frequency);
                double signalPower = -50.0 + CableLossL2;
                _instrumentManager.SignalGenerator.SetPower(signalPower);
                LogGenerated?.Invoke(this, $"信号源已设置：{frequency} MHz，功率{signalPower} dBm");
                
                // 5. 发送工装板频率切换命令
                _instrumentManager.SendFrequencyCommand(frequency);
                await Task.Delay(500, cancellationToken);

                // 6. 频谱仪执行峰值搜索                
                _instrumentManager.SpectrumAnalyzer.MeasureMarkerPeak();
                await Task.Delay(500, cancellationToken);

                // 7. 读取-15MHz偏移的mark值
                _instrumentManager.SignalGenerator.SetFrequency(frequency - 15);
                _instrumentManager.SignalGenerator.SetPower(-30+ CableLossL2);
                _instrumentManager.SpectrumAnalyzer.ReadDeltaValue(-15);
                await Task.Delay(500, cancellationToken);
                
                double deltaleft = _instrumentManager.SpectrumAnalyzer.ReadMarkerValue();
                LogGenerated?.Invoke(this, $"delta(-15MHz)值：{deltaleft:F2} dB");
                
                // 记录测试结果（加20dB）
                RecordTestResult(channel, testItem.Name, deltaleft - 20, "dB", $"工作频率{frequency}MHz");
                
                LogGenerated?.Invoke(this, $"频率{frequency} MHz测试完成");
                
                // 8. 关闭所有标记
                _instrumentManager.SpectrumAnalyzer.SetAllMarkOFF();
            }
            
            // 关闭信号源RF输出
            _instrumentManager.SignalGenerator.EnableOutput(false);
            LogGenerated?.Invoke(this, $"测试完成：通道{channel} - {testItem.Name}");
        }
        
        /// <summary>
        /// 执行带内增益波动测试（5MHz带宽内增益平坦度）
        /// </summary>
        /// <param name="channel">通道号</param>
        /// <param name="testItem">测试项</param>
        /// <param name="cancellationToken">取消令牌</param>
        private async Task RunGainFlatnessTestAsync(int channel, TestItem testItem, CancellationToken cancellationToken)
        {
            // 1. 重置所有仪表
            _instrumentManager.ResetAll();
            
            // 2. 频谱仪设置：Center 70MHz，Span 5MHz，MAX HOLD模式
            _instrumentManager.SpectrumAnalyzer.SetFrequencySpan(70, 5);
            _instrumentManager.SpectrumAnalyzer.SetReferenceLevel(20);
            _instrumentManager.SpectrumAnalyzer.SetMaxHold(true);
            _instrumentManager.SpectrumAnalyzer.SetResolutionBandwidth(100);
            LogGenerated?.Invoke(this, "频谱仪已设置：Center 70MHz，Span 5MHz，MAX HOLD");
            
            // 3. 信号源初始化设置
            _instrumentManager.SignalGenerator.EnableModulation(false);
            _instrumentManager.SignalGenerator.EnableOutput(true);
            LogGenerated?.Invoke(this, "信号源初始化完成，RF输出已开启");
            
            // 测试频率列表：3330MHz、3350MHz、3370MHz、3390MHz
            double[] testFrequencies = { 3330.0, 3350.0, 3370.0, 3390.0 };
            
            foreach (double frequency in testFrequencies)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                LogGenerated?.Invoke(this, $"开始测试频率：{frequency} MHz，通道：{channel}");
                
                // 4. 打开连续扫描
                _instrumentManager.SignalGenerator.SetContinuousSweep(true);
                
                // 5. 设置信号源扫描模式
                _instrumentManager.SignalGenerator.SetFrequencyModeSweep();
                _instrumentManager.SignalGenerator.SetSweepStartFrequency((frequency - 3) * 1e6);
                _instrumentManager.SignalGenerator.SetSweepStopFrequency((frequency + 3) * 1e6);
                _instrumentManager.SignalGenerator.SetSweepPoints(600);
                LogGenerated?.Invoke(this, $"信号源已设置：扫描{frequency-3}~{frequency+3}MHz，200点");
                
                // 6. 发送工装板频率切换命令
                _instrumentManager.SendFrequencyCommand(frequency);
                LogGenerated?.Invoke(this, $"工装板已切换到 {frequency} MHz");
                
                // 7. 设置信号源功率
                double signalPower = -50.0 + CableLossL2;
                _instrumentManager.SignalGenerator.SetPower(signalPower);
                LogGenerated?.Invoke(this, $"信号源功率：{signalPower} dBm");
                
                // 8. 等待扫描完成
                _instrumentManager.SpectrumAnalyzer.SetMaxHold(false);
                _instrumentManager.SpectrumAnalyzer.SetMaxHold(true);
                await Task.Delay(10000, cancellationToken);
                
                // 9. 使用pk-pk search读取数值
                double pkpk = _instrumentManager.SpectrumAnalyzer.ReadPeakToPeak();
                LogGenerated?.Invoke(this, $"带内增益波动（pk-pk）：{pkpk:F2} dB");
                
                // 10. 关闭连续扫描
                _instrumentManager.SignalGenerator.SetContinuousSweep(false);
                
                // 记录测试结果
                RecordTestResult(channel, testItem.Name, pkpk, "dB", $"频率{frequency}MHz，带内波动");
                
                LogGenerated?.Invoke(this, $"频率{frequency} MHz测试完成");
            }
            
            // 关闭MAX HOLD模式
            _instrumentManager.SpectrumAnalyzer.SetMaxHold(false);
            
            // 关闭信号源RF输出
            _instrumentManager.SignalGenerator.EnableOutput(false);
            
            LogGenerated?.Invoke(this, $"测试完成：通道{channel} - {testItem.Name}");
        }
        
        /// <summary>
        /// 执行带外抑制测试
        /// </summary>
        /// <param name="channel">通道号</param>
        /// <param name="testItem">测试项</param>
        /// <param name="cancellationToken">取消令牌</param>
        private async Task RunOutOfBandRejectionTestAsync(int channel, TestItem testItem, CancellationToken cancellationToken)
        {
            // 1. 重置所有仪表
            _instrumentManager.ResetAll();
            
            // 2. 频谱仪设置：Center 70MHz，Span 100MHz，RBW/VBW Auto，Ref Level -10dBm，
            _instrumentManager.SpectrumAnalyzer.SetFrequencySpan(70, 100);
            _instrumentManager.SpectrumAnalyzer.SetResolutionBandwidth(150);
            _instrumentManager.SpectrumAnalyzer.SetVideoBandwidthAuto();
            _instrumentManager.SpectrumAnalyzer.SetReferenceLevel(5);
            LogGenerated?.Invoke(this, "频谱仪已设置：Center 70MHz，Span 100MHz，RBW/VBW Auto，Ref -10dBm，轨迹平均（扫描CONT已全局设置）");
            
            // 3. 信号源初始化设置
            _instrumentManager.SignalGenerator.EnableModulation(false);
            _instrumentManager.SignalGenerator.SetPulseModulation(false);
            _instrumentManager.SignalGenerator.EnableOutput(true);
            LogGenerated?.Invoke(this, "信号源初始化完成，RF输出已开启");
            
            // 测试频率列表：3330MHz、3350MHz、3370MHz、3390MHz
            double[] testFrequencies = { 3330.0, 3350.0, 3370.0, 3390.0 };
            
            foreach (double frequency in testFrequencies)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                LogGenerated?.Invoke(this, $"开始测试频率：{frequency} MHz，通道：{channel}");
                
                // 4. 信号源设置：输出工作频率，功率-50dBm，发送工装板频率切换命令，接收机设置衰减0dBm
                _instrumentManager.SendAttenuationCommand(0);
                double signalPower = -50.0 + CableLossL2;
                _instrumentManager.SignalGenerator.SetFrequency(frequency);
                _instrumentManager.SendFrequencyCommand(frequency);
                _instrumentManager.SignalGenerator.SetPower(signalPower);
                LogGenerated?.Invoke(this, $"信号源输出{frequency} MHz，功率{signalPower} dBm");
                
                await Task.Delay(1000, cancellationToken);
                
                // 5. 重置轨迹最大保持，读取峰值，建立参考点
                _instrumentManager.SpectrumAnalyzer.SetMaxHold(false);
                _instrumentManager.SpectrumAnalyzer.SetMaxHold(true);
                LogGenerated?.Invoke(this, "读取峰值，建立参考点");
                _instrumentManager.SpectrumAnalyzer.MeasureMarkerPeak();
                await Task.Delay(1000, cancellationToken);
                // 6. 开启delta功能，读取+14MHz差值
                double delta = _instrumentManager.SpectrumAnalyzer.ReadDeltaValue(14);
                LogGenerated?.Invoke(this, $"delta+14MHz：{delta:F2} dB");
                
                RecordTestResult(channel, testItem.Name, delta, "dB", $"频率{frequency}MHz，delta+40MHz"); 
                
                LogGenerated?.Invoke(this, $"频率{frequency} MHz测试完成");
                
                //关闭所有标记
                _instrumentManager.SpectrumAnalyzer.SetAllMarkOFF();
            }
            
            // 关闭平均模式
            _instrumentManager.SpectrumAnalyzer.SetAverageMode(false);
            
            _instrumentManager.SignalGenerator.EnableOutput(false);
            LogGenerated?.Invoke(this, $"测试完成：通道{channel} - {testItem.Name}");
        }
        
        /// <summary>
        /// 执行通道增益测试
        /// </summary>
        /// <param name="channel">通道号</param>
        /// <param name="testItem">测试项</param>
        /// <param name="cancellationToken">取消令牌</param>
        private async Task RunChannelGainTestAsync(int channel, TestItem testItem, CancellationToken cancellationToken)
        {
            // 重置所有仪表
            _instrumentManager.ResetAll();
            
            // 测试频率列表：3330MHz、3350MHz、3370MHz、3390MHz
            double[] testFrequencies = { 3330.0, 3350.0, 3370.0, 3390.0 };
            
            // 信号源初始化设置
            _instrumentManager.SignalGenerator.EnableModulation(false);
            _instrumentManager.SignalGenerator.SetPulseModulation(false);
            _instrumentManager.SignalGenerator.EnableOutput(true);
            LogGenerated?.Invoke(this, "信号源初始化完成，RF输出已开启");
            
            // 频谱仪设置：Center 70MHz，Span 50MHz，RBW/VBW Auto
            _instrumentManager.SpectrumAnalyzer.SetFrequencySpan(70, 50);
            _instrumentManager.SpectrumAnalyzer.SetResolutionBandwidthAuto();
            _instrumentManager.SpectrumAnalyzer.SetVideoBandwidthAuto();
            _instrumentManager.SpectrumAnalyzer.SetReferenceLevel(10);
            LogGenerated?.Invoke(this, "频谱仪已设置：Center 70MHz，Span 50MHz，RBW/VBW Auto，Ref 10dBm");
            
            foreach (double frequency in testFrequencies)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                LogGenerated?.Invoke(this, $"开始测试频率：{frequency} MHz，通道：{channel}");
                
                // 1. 设置接收机工作频率和衰减0dBm
                _instrumentManager.SendAttenuationCommand(0);
                LogGenerated?.Invoke(this, $"设置接收机频率：{frequency} MHz，衰减：0dB");
                
                // 2. 设置信号源
                double signalPower = -50.0 + CableLossL2;
                LogGenerated?.Invoke(this, $"信号源输出{frequency} MHz，功率为{signalPower} dBm");
                
                _instrumentManager.SignalGenerator.SetFrequency(frequency);
                _instrumentManager.SendFrequencyCommand(frequency);
                _instrumentManager.SignalGenerator.SetPower(signalPower);
                
                await Task.Delay(500, cancellationToken);
                
                // 3. 记录频谱仪读数（频谱仪中心频率固定为70MHz，RBW/VBW自动）
                double A = _instrumentManager.SpectrumAnalyzer.MeasurePower(frequency, 0);
                LogGenerated?.Invoke(this, $"频谱仪读数A：{A} dBm");
                
                // 4. 计算通道增益：A + 50 + L1
                double channelGain = A + 50.0 + CableLossL1;
                LogGenerated?.Invoke(this, $"通道增益：{channelGain} dB");
                
                // 记录测试结果
                RecordTestResult(channel, testItem.Name, channelGain, "dB", $"频率{frequency}MHz，衰减0dB");
                
                LogGenerated?.Invoke(this, $"频率{frequency} MHz测试完成");

                // 关闭所有标记
                _instrumentManager.SpectrumAnalyzer.SetAllMarkOFF();
            }
            
            // 测试完成，关闭信号源输出
            _instrumentManager.SignalGenerator.EnableOutput(false);
            
            // 测试完成
            LogGenerated?.Invoke(this, $"测试完成：通道{channel} - {testItem.Name}");
        }
        
        /// <summary>
        /// 记录测试结果
        /// </summary>
        /// <param name="channel">通道号</param>
        /// <param name="testItemName">测试项名称</param>
        /// <param name="value">测试值</param>
        /// <param name="description">测试条件描述</param>
        private void RecordTestResult(int channel, string testItemName, double value, string description)
        {
            RecordTestResult(channel, testItemName, value, "dBm", description);
        }
        
        /// <summary>
        /// 记录测试结果（重载方法）
        /// </summary>
        /// <param name="channel">通道号</param>
        /// <param name="testItemName">测试项名称</param>
        /// <param name="value">测试值</param>
        /// <param name="unit">单位</param>
        /// <param name="description">测试条件描述</param>
        private void RecordTestResult(int channel, string testItemName, double value, string unit, string description)
        {
            // 创建测试结果
            var testResult = new TestResult
            {
                Channel = channel,
                TestItem = $"{testItemName} - {description}",
                Value = value,
                Unit = unit,
                IsPass = true,
                TestTime = DateTime.Now,
                StandardValue = 0.0,
                ComparisonType = ""
            };
            
            // 添加到结果列表
            _testResults.Add(testResult);
            
            // 触发测试完成事件
            TestCompleted?.Invoke(this, testResult);
        }
        
        /// <summary>
        /// 批量执行测试
        /// </summary>
        /// <param name="channels">通道列表</param>
        /// <param name="testItemNames">测试项名称列表</param>
        /// <returns>测试结果列表</returns>
        public async Task<List<TestResult>> RunBatchTestAsync(List<int> channels, List<string> testItemNames)
        {
            // 创建新的取消令牌源
            _cts = new CancellationTokenSource();
            var cancellationToken = _cts.Token;
            
            try
            {
                foreach (var channel in channels)
                {
                    ChannelChanging?.Invoke(channel);
                    
                    foreach (var testItemName in testItemNames)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await RunTestAsync(channel, testItemName, cancellationToken);
                        
                        // 检查是否已取消（如果当前测试被取消）
                        if (cancellationToken.IsCancellationRequested)
                        {
                            throw new OperationCanceledException(cancellationToken);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                LogGenerated?.Invoke(this, "批量测试已取消");
                // 清理资源
                _instrumentManager.SignalGenerator.EnableOutput(false);
            }
            finally
            {
                // 释放取消令牌源
                _cts.Dispose();
                _cts = null;
            }
            
            return _testResults;
        }
        
        /// <summary>
        /// 获取所有测试结果
        /// </summary>
        /// <returns>测试结果列表</returns>
        public List<TestResult> GetAllTestResults()
        {
            return new List<TestResult>(_testResults);
        }
        
        /// <summary>
        /// 清空测试结果
        /// </summary>
        public void ClearTestResults()
        {
            _testResults.Clear();
        }
        
        /// <summary>
        /// 获取测试项信息
        /// </summary>
        /// <param name="testItemName">测试项名称</param>
        /// <returns>测试项信息</returns>
        public TestItem? GetTestItem(string testItemName)
        {
            _testItems.TryGetValue(testItemName, out var testItem);
            return testItem;
        }
        
        /// <summary>
        /// 获取所有测试项名称
        /// </summary>
        /// <returns>测试项名称列表</returns>
        public List<string> GetAllTestItemNames()
        {
            return _testItems.Keys.ToList();
        }
        
        /// <summary>
        /// 执行端口驻波测试（输入）
        /// </summary>
        /// <param name="channel">通道号</param>
        /// <param name="testItem">测试项</param>
        /// <param name="cancellationToken">取消令牌</param>
        private async Task RunPortVSWRInputTestAsync(int channel, TestItem testItem, CancellationToken cancellationToken)
        {
            if (!_instrumentManager.ZNB8.IsConnected)
            {
                LogGenerated?.Invoke(this, "错误：ZNB8矢量网络分析仪未连接");
                return;
            }
            
            var confirmResult = ShowConfirm("请先完成网络分析仪校准，然后连接好测试线缆。\n\n校准完成后点击确定开始测试。");
            if (confirmResult != true)
            {
                LogGenerated?.Invoke(this, "用户取消端口驻波测试");
                return;
            }
            
            double[] testFrequencies = { 3330.0, 3350.0, 3370.0, 3390.0 };
            int[] markers = { 1, 2, 3, 4 };
            
            _instrumentManager.ZNB8.SetStartFrequency(3300);
            _instrumentManager.ZNB8.SetStopFrequency(3400);
            _instrumentManager.ZNB8.SetSweepPoints(1001);
            _instrumentManager.ZNB8.SetReferenceLevel(10);
            _instrumentManager.ZNB8.SetAveraging(true, 16);
            _instrumentManager.ZNB8.NewAveraging();
            _instrumentManager.ZNB8.EnableOutput(true);
            LogGenerated?.Invoke(this, "ZNB8已设置：频率范围3300-3400MHz，扫描点数1001，参考电平10dBm，平均16次，输出已开启");
            
            for (int i = 0; i < testFrequencies.Length; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                double frequency = testFrequencies[i];
                int marker = markers[i];
                
                _instrumentManager.SendFrequencyCommand(frequency);
                await Task.Delay(500, cancellationToken);
                
                string command = $":CALC:MARK{marker}:Y?";
                string response = _instrumentManager.ZNB8.Query(command);
                double vswr = double.Parse(response);
                
                LogGenerated?.Invoke(this, $"频率{frequency}MHz，MARK{marker}读数：{vswr}");
                RecordTestResult(channel, testItem.Name, vswr, $"频率{frequency}MHz");
            }
            
            // 关闭网络分析仪输出
            _instrumentManager.ZNB8.EnableOutput(false);
            LogGenerated?.Invoke(this, $"测试完成：通道{channel} - {testItem.Name}");
        }
        
        /// <summary>
        /// 执行校准开关隔离度测试
        /// </summary>
        /// <param name="channel">通道号</param>
        /// <param name="testItem">测试项</param>
        /// <param name="cancellationToken">取消令牌</param>
        private async Task RunCalibrationSwitchIsolationTestAsync(int channel, TestItem testItem, CancellationToken cancellationToken)
        {
            double[] testFrequencies = { 3330.0, 3350.0, 3370.0, 3390.0 };

            //重置所有仪表
            _instrumentManager.ResetAll();

            // 1. 设置频谱仪参数：70MHz中心频率，50MHz带宽，5dB参考电平
            _instrumentManager.SpectrumAnalyzer.SetFrequencySpan(70, 50);
            _instrumentManager.SpectrumAnalyzer.SetReferenceLevel(5);
            _instrumentManager.SpectrumAnalyzer.SetResolutionBandwidth(50);
            _instrumentManager.SpectrumAnalyzer.SetVideoBandwidth(50);
            _instrumentManager.SpectrumAnalyzer.SetResolutionBandwidthAuto();
            _instrumentManager.SpectrumAnalyzer.SetVideoBandwidthAuto();
            LogGenerated?.Invoke(this, $"频谱仪设置：中心频率70MHz，带宽50MHz，参考电平5dB，RBW/VBW自动");
            
            await Task.Delay(500, cancellationToken);

            foreach (double frequency in testFrequencies)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                LogGenerated?.Invoke(this, $"开始测试频率：{frequency} MHz，通道：{channel}");                                         
                
                // 2. 设置信号源：frequency MHz，信号功率-50dBm，开启输出，关闭调制
                _instrumentManager.SignalGenerator.EnableModulation(false);
                _instrumentManager.SignalGenerator.EnableOutput(true);
                LogGenerated?.Invoke(this, "信号源初始化完成，RF输出已开启");
                _instrumentManager.SignalGenerator.SetFrequency(frequency);
                _instrumentManager.SendFrequencyCommand(frequency);
                _instrumentManager.SignalGenerator.SetPower(-50.0 + CableLossL2);
                LogGenerated?.Invoke(this, $"信号源设置：频率{frequency}MHz，功率-50dBm");
                
                await Task.Delay(500, cancellationToken);
                
                // 3. 频谱仪读取当前峰值
                double peakValue = _instrumentManager.SpectrumAnalyzer.MeasureMarkerPeak();
                LogGenerated?.Invoke(this, $"初始峰值读数：{peakValue} dBm");
                
                await Task.Delay(500, cancellationToken);
                
                // 4. 开启delta标记功能
                _instrumentManager.SpectrumAnalyzer.SetDeltaValue();
                LogGenerated?.Invoke(this, "已开启delta标记功能");
                
                await Task.Delay(500, cancellationToken);
                
                // 5. 开启校准开关（FXJZ拉高）
                _instrumentManager.SendCalibrationSwitchHigh();
                LogGenerated?.Invoke(this, "校准开关已开启");
                
                await Task.Delay(300, cancellationToken);
                
                // 6. 设置信号源功率为0dBm
                _instrumentManager.SignalGenerator.SetPower(0.0 + CableLossL2);
                LogGenerated?.Invoke(this, "信号源功率设置为0dBm");
                
                await Task.Delay(500, cancellationToken);
                
                // 7. 读取delta值
                double deltaValue = _instrumentManager.SpectrumAnalyzer.ReadMarkerValue();
                LogGenerated?.Invoke(this, $"Delta标记读数：{deltaValue} dB");
                
                // 8. 关闭校准开关（FXJZ拉低）
                _instrumentManager.SendCalibrationSwitchLow();
                LogGenerated?.Invoke(this, "校准开关已关闭");
                
                await Task.Delay(300, cancellationToken);
                
                // 9. 计算最终结果：delta - 50
                double isolation = deltaValue - 50.0;
                LogGenerated?.Invoke(this, $"隔离度计算：{deltaValue} - 50 = {isolation} dB");
                
                // 记录测试结果
                RecordTestResult(channel, testItem.Name, isolation, "dB", $"频率{frequency}MHz");
                
                LogGenerated?.Invoke(this, $"频率{frequency} MHz测试完成");

                // 关闭所有标记
                _instrumentManager.SpectrumAnalyzer.SetAllMarkOFF();
            }
            
            // 确保校准开关最终关闭
            _instrumentManager.SendCalibrationSwitchLow();
            LogGenerated?.Invoke(this, "最终确认：校准开关已关闭");
            
            // 关闭信号源输出
            _instrumentManager.SignalGenerator.EnableOutput(false);
            
            // 测试完成
            LogGenerated?.Invoke(this, $"测试完成：通道{channel} - {testItem.Name}");
        }
        
        /// <summary>
        /// 停止测试
        /// </summary>
        public void StopTest()
        {
            if (_cts != null)
            {
                LogGenerated?.Invoke(this, "正在停止测试...");
                _cts.Cancel();
            }
        }
    }
}