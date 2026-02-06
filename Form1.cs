using System.Text.Json;
using System.Drawing;
using 五通道自动测试.Calibration;
using 五通道自动测试.Config;
using 五通道自动测试.Instruments;
using 五通道自动测试.Services;
using 五通道自动测试.Test;
using TemperatureChamber;
using TemperatureChamber.Models;

namespace 五通道自动测试
{
    public partial class Form1 : Form
    {
        private InstrumentManager? _instrumentManager;
        private TestManager? _testManager;
        private bool _isChamberMode = false;
        private ChamberControlForm? _chamberControlForm;

        private int _currentChannel = 0;
        private ToolingBoardCommunicator? _toolingBoard;
        private FormCalibration? _calibrationForm;
        private bool _isCalibrationMode = false;
        private bool _isPowerOn = false;
        private InstrumentConfig? _instrumentConfig;

        public Form1(InstrumentManager instrumentManager)
        {
            _instrumentManager = instrumentManager;
            InitializeComponent();
            LoadInstrumentConfig();
            InitializeTestSystem();
        }

        /// <summary>
        /// 初始化测试系统
        /// </summary>
        private void InitializeTestSystem()
        {
            // 初始化测试管理器
            _testManager = new TestManager(_instrumentManager);

            // 订阅事件
            _testManager.TestCompleted += OnTestCompleted;
            _testManager.LogGenerated += OnLogGenerated;
            _testManager.ChannelChanging += (channel) => SwitchToChannel(channel);

            // 初始化测试结果表格
            InitializeTestResultsGrid();

            // 更新仪表状态
            UpdateInstrumentStatus();

            // 初始化工装板串口选择
            RefreshSerialPorts();

            // 自动连接仪表
            AutoConnectInstruments();

            // 自动连接工装板（COM3）
            AutoConnectToolingBoard();
            
            // 默认选择通道1
            SwitchToChannel(1);

            // 初始化电源开关按钮
            InitializePowerSwitchButton();

            // 订阅电源状态变化事件
            _instrumentManager.PowerStateChanged += OnPowerStateChanged;
        }

        /// <summary>
        /// 电源状态变化事件处理，同步更新按钮状态
        /// </summary>
        private void OnPowerStateChanged(bool isOn)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<bool>(OnPowerStateChanged), isOn);
                return;
            }

            _isPowerOn = isOn;
            if (PowerSwitch != null)
            {
                if (_isPowerOn)
                {
                    PowerSwitch.BackColor = Color.LightCoral;
                    PowerSwitch.Text = "关闭供电";
                }
                else
                {
                    PowerSwitch.BackColor = Color.LightGreen;
                    PowerSwitch.Text = "开启供电";
                }
            }
        }

        /// <summary>
        /// 初始化电源开关按钮样式
        /// </summary>
        private void InitializePowerSwitchButton()
        {
            if (PowerSwitch != null)
            {
                PowerSwitch.BackColor = Color.LightGreen;
                PowerSwitch.Text = "开启供电";
                PowerSwitch.FlatStyle = FlatStyle.Flat;
                PowerSwitch.FlatAppearance.BorderSize = 0;
            }
        }

        /// <summary>
        /// 更新电源开关按钮状态
        /// </summary>
        private void UpdatePowerSwitchButton()
        {
            if (PowerSwitch == null) return;

            if (_instrumentManager != null)
            {
                _isPowerOn = _instrumentManager.IsPowerOn;
            }

            if (_isPowerOn)
            {
                PowerSwitch.BackColor = Color.LightCoral;
                PowerSwitch.Text = "关闭供电";
            }
            else
            {
                PowerSwitch.BackColor = Color.LightGreen;
                PowerSwitch.Text = "开启供电";
            }
        }

        /// <summary>
        /// 从配置文件加载仪表地址
        /// </summary>
        private void LoadInstrumentConfig()
        {
            try
            {
                string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "config.json");
                if (File.Exists(configPath))
                {
                    string json = File.ReadAllText(configPath);
                    _instrumentConfig = JsonSerializer.Deserialize<InstrumentConfig>(json);
                }
                else
                {
                    _instrumentConfig = new InstrumentConfig();
                }
            }
            catch (Exception ex)
            {
                _instrumentConfig = new InstrumentConfig();
            }
        }

        /// <summary>
        /// 自动连接仪表
        /// </summary>
        private void AutoConnectInstruments()
        {
            if (_instrumentManager == null)
                return;

            // 连接频谱仪
            string? spectrumAddr = _instrumentConfig?.Instruments?.SpectrumAnalyzer?.Address ?? "USB0::0x2A8D::0x0A0B::MY51282426::INSTR";
            if (!string.IsNullOrEmpty(spectrumAddr))
            {
                bool success = _instrumentManager.ConnectSpectrumAnalyzer(spectrumAddr);
                if (txtTestLog != null)
                {
                    txtTestLog.AppendText($"[{DateTime.Now:HH:mm:ss}] 频谱仪{(success ? "自动连接成功" : "自动连接失败")}：{spectrumAddr}\r\n");
                    txtTestLog.ScrollToCaret();
                }
            }

            // 连接信号源
            string? signalAddr = _instrumentConfig?.Instruments?.SignalGenerator?.Address ?? "USB0::0x0957::0x1F01::MY61252673::INSTR";
            if (!string.IsNullOrEmpty(signalAddr))
            {
                bool success = _instrumentManager.ConnectSignalGenerator(signalAddr);
                if (txtTestLog != null)
                {
                    txtTestLog.AppendText($"[{DateTime.Now:HH:mm:ss}] 信号源{(success ? "自动连接成功" : "自动连接失败")}：{signalAddr}\r\n");
                }
            }

            // 连接ZNB8矢量网络分析仪
            string? znb8Addr = _instrumentConfig?.Instruments?.ZNB8?.Address ?? "TCPIP0::192.168.0.252::inst0::INSTR";
            if (!string.IsNullOrEmpty(znb8Addr))
            {
                bool success = _instrumentManager.ConnectZNB8(znb8Addr);
                if (txtTestLog != null)
                {
                    txtTestLog.AppendText($"[{DateTime.Now:HH:mm:ss}] ZNB8矢量网络分析仪{(success ? "自动连接成功" : "自动连接失败")}：{znb8Addr}\r\n");
                }
            }

            // 连接ODP3063电源
            string? odp3063Addr = _instrumentConfig?.Instruments?.ODP3063?.Address ?? "USB0::0x5345::0x1234::2243130::INSTR";
            bool odpSuccess = _instrumentManager.ConnectODP3063(odp3063Addr);
            if (txtTestLog != null)
            {
                txtTestLog.AppendText($"[{DateTime.Now:HH:mm:ss}] ODP3063电源{(odpSuccess ? "自动连接成功" : "自动连接失败")}：{odp3063Addr}\r\n");
            }

            UpdateInstrumentStatus();
        }

        /// <summary>
        /// 自动连接工装板（COM3）
        /// </summary>
        private void AutoConnectToolingBoard()
        {
            const string defaultPort = "COM3";

            if (_toolingBoard == null)
            {
                _toolingBoard = new ToolingBoardCommunicator(defaultPort);
                _toolingBoard.LogMessage += OnBoardLogMessage;
                _toolingBoard.CommunicationStatusChanged += OnBoardStatusChanged;
            }

            if (_toolingBoard.Open())
            {
                if (lblBoardStatus != null)
                {
                    lblBoardStatus.Text = "已连接";
                    lblBoardStatus.ForeColor = Color.Green;
                }
                if (txtTestLog != null)
                {
                    txtTestLog.AppendText($"[{DateTime.Now:HH:mm:ss}] 工装板自动连接成功：{defaultPort}\r\n");
                    txtTestLog.ScrollToCaret();
                }

                // 将工装板传递给InstrumentManager
                _instrumentManager?.SetToolingBoard(_toolingBoard);
                
                // 发送Normal模式初始化命令（FXJZ拉低、校准源下电、天线去能）
                _instrumentManager?.SendNormalModeCommands();
            }
            else
            {
                if (lblBoardStatus != null)
                {
                    lblBoardStatus.Text = "连接失败";
                    lblBoardStatus.ForeColor = Color.Red;
                }
                if (txtTestLog != null)
                {
                    txtTestLog.AppendText($"[{DateTime.Now:HH:mm:ss}] 工装板自动连接失败：{defaultPort}\r\n");
                    txtTestLog.ScrollToCaret();
                }
            }
        }

        /// <summary>
        /// 刷新可用串口列表
        /// </summary>
        private void RefreshSerialPorts()
        {
            if (cbSerialPort == null) return;

            cbSerialPort.Items.Clear();
            var ports = ToolingBoardCommunicator.GetAvailablePorts();
            cbSerialPort.Items.AddRange(ports);
            
            // 设置COM5为默认选中项（如果可用）
            int com5Index = cbSerialPort.Items.IndexOf("COM5");
            if (com5Index >= 0)
                cbSerialPort.SelectedIndex = com5Index;
            else if (cbSerialPort.Items.Count > 0)
                cbSerialPort.SelectedIndex = 0;
        }

        /// <summary>
        /// 连接工装板按钮点击事件
        /// </summary>
        private void btnConnectBoard_Click(object sender, EventArgs e)
        {
            if (cbSerialPort == null || lblBoardStatus == null)
                return;

            string? portName = cbSerialPort.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(portName))
            {
                MessageBox.Show("请选择串口");
                return;
            }

            // 检查是否需要重新创建实例（端口不同或未初始化）
            if (_toolingBoard == null || _toolingBoard.PortName != portName)
            {
                // 先关闭现有连接
                if (_toolingBoard != null)
                {
                    _toolingBoard.Close();
                    _toolingBoard.LogMessage -= OnBoardLogMessage;
                    _toolingBoard.CommunicationStatusChanged -= OnBoardStatusChanged;
                    _toolingBoard.Dispose();
                }

                // 创建新实例
                _toolingBoard = new ToolingBoardCommunicator(portName);
                _toolingBoard.LogMessage += OnBoardLogMessage;
                _toolingBoard.CommunicationStatusChanged += OnBoardStatusChanged;
            }

            if (_toolingBoard.Open())
            {
                lblBoardStatus.Text = "已连接";
                lblBoardStatus.ForeColor = Color.Green;
                if (txtTestLog != null)
                {
                    txtTestLog.AppendText($"[{DateTime.Now:HH:mm:ss}] 工装板串口 {portName} 已连接\r\n");
                    txtTestLog.ScrollToCaret();
                }

                // 将工装板传递给InstrumentManager
                _instrumentManager?.SetToolingBoard(_toolingBoard);
            }
            else
            {
                lblBoardStatus.Text = "连接失败";
                lblBoardStatus.ForeColor = Color.Red;
            }
        }

        /// <summary>
        /// 工装板日志事件处理
        /// </summary>
        private void OnBoardLogMessage(string message)
        {
            if (txtTestLog == null) return;

            if (txtTestLog.InvokeRequired)
            {
                txtTestLog.Invoke(new Action(() =>
                {
                    txtTestLog.AppendText($"[{DateTime.Now:HH:mm:ss}] 工装板: {message}\r\n");
                    txtTestLog.ScrollToCaret();
                }));
            }
            else
            {
                txtTestLog.AppendText($"[{DateTime.Now:HH:mm:ss}] 工装板: {message}\r\n");
                txtTestLog.ScrollToCaret();
            }
        }

        /// <summary>
        /// 工装板状态变化事件处理
        /// </summary>
        private void OnBoardStatusChanged(bool connected)
        {
            if (lblBoardStatus == null) return;

            if (lblBoardStatus.InvokeRequired)
            {
                lblBoardStatus.Invoke(new Action(() =>
                {
                    lblBoardStatus.Text = connected ? "已连接" : "未连接";
                    lblBoardStatus.ForeColor = connected ? Color.Green : Color.Red;
                }));
            }
            else
            {
                lblBoardStatus.Text = connected ? "已连接" : "未连接";
                lblBoardStatus.ForeColor = connected ? Color.Green : Color.Red;
            }
        }

        /// <summary>
        /// 初始化测试结果表格
        /// </summary>
        private void InitializeTestResultsGrid()
        {
            if (dgvTestResults == null) return;

            dgvTestResults.Columns.Add("Channel", "通道");
            dgvTestResults.Columns.Add("TestItem", "测试项");
            dgvTestResults.Columns.Add("Value", "测试值");
            dgvTestResults.Columns.Add("Unit", "单位");
            dgvTestResults.Columns.Add("IsPass", "结果");
            dgvTestResults.Columns.Add("TestTime", "测试时间");

            // 设置列比例（FillWeight）
            dgvTestResults.Columns["Channel"].FillWeight = 25;
            dgvTestResults.Columns["TestItem"].FillWeight = 250;
            dgvTestResults.Columns["Value"].FillWeight = 30;
            dgvTestResults.Columns["Unit"].FillWeight = 25;
            dgvTestResults.Columns["IsPass"].FillWeight = 25;
            dgvTestResults.Columns["TestTime"].FillWeight = 100;

            // 设置AutoSizeColumnsMode为Fill
            dgvTestResults.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // 设置整列居中对齐
            foreach (DataGridViewColumn col in dgvTestResults.Columns)
            {
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            // 设置列头居中对齐（使用ColumnHeadersDefaultCellStyle更可靠）
            dgvTestResults.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvTestResults.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.NotSet;

            // 设置只读
            dgvTestResults.ReadOnly = true;
            // 设置行高
            dgvTestResults.RowTemplate.Height = 25;
        }

        /// <summary>
        /// 更新仪表状态显示
        /// </summary>
        private void UpdateInstrumentStatus()
        {
            if (_instrumentManager == null || pbSpectrumStatus == null || lblSpectrumStatus == null || pbSignalStatus == null || lblSignalStatus == null || pbZNB8Status == null || lblZNB8Status == null)
                return;

            // 更新频谱仪状态
            if (_instrumentManager.SpectrumAnalyzer.IsConnected)
            {
                pbSpectrumStatus.BackColor = Color.Green;
                lblSpectrumStatus.Text = "已连接";
            }
            else
            {
                pbSpectrumStatus.BackColor = Color.Red;
                lblSpectrumStatus.Text = "未连接";
            }

            // 更新信号源状态
            if (_instrumentManager.SignalGenerator.IsConnected)
            {
                pbSignalStatus.BackColor = Color.Green;
                lblSignalStatus.Text = "已连接";
            }
            else
            {
                pbSignalStatus.BackColor = Color.Red;
                lblSignalStatus.Text = "未连接";
            }

            // 更新ZNB8状态
            if (_instrumentManager.ZNB8.IsConnected)
            {
                pbZNB8Status.BackColor = Color.Green;
                lblZNB8Status.Text = "已连接";
            }
            else
            {
                pbZNB8Status.BackColor = Color.Red;
                lblZNB8Status.Text = "未连接";
            }
        }

        /// <summary>
        /// 获取选中的通道列表
        /// </summary>
        /// <returns>选中的通道列表</returns>
        private List<int> GetSelectedChannels()
        {
            var channels = new List<int>();
            if (cbChannel1?.Checked == true) channels.Add(1);
            else if (cbChannel2?.Checked == true) channels.Add(2);
            else if (cbChannel3?.Checked == true) channels.Add(3);
            else if (cbChannel4?.Checked == true) channels.Add(4);
            else if (cbChannel5?.Checked == true) channels.Add(5);
            else if (cbChannel6?.Checked == true) channels.Add(6);
            else if (cbChannel7?.Checked == true) channels.Add(7);
            else if (cbChannel8?.Checked == true) channels.Add(8);
            return channels;
        }

        /// <summary>
        /// 通道切换处理
        /// </summary>
        private void SwitchToChannel(int channel)
        {
            if (_currentChannel == channel) return;
            _currentChannel = channel;

            // 同步更新 radio button 状态（添加 null 检查，确保控件已初始化）
            if (cbChannel1 != null) cbChannel1.Checked = channel == 1;
            if (cbChannel2 != null) cbChannel2.Checked = channel == 2;
            if (cbChannel3 != null) cbChannel3.Checked = channel == 3;
            if (cbChannel4 != null) cbChannel4.Checked = channel == 4;
            if (cbChannel5 != null) cbChannel5.Checked = channel == 5;
            if (cbChannel6 != null) cbChannel6.Checked = channel == 6;
            if (cbChannel7 != null) cbChannel7.Checked = channel == 7;
            if (cbChannel8 != null) cbChannel8.Checked = channel == 8;

            _instrumentManager?.SwitchChannel(channel);
            txtTestLog?.AppendText($"[{DateTime.Now:HH:mm:ss}] 已切换到通道{channel}\r\n");
        }

        private void cbChannel1_CheckedChanged(object sender, EventArgs e)
        {
            if (cbChannel1?.Checked == true)
                SwitchToChannel(1);
        }

        private void cbChannel2_CheckedChanged(object sender, EventArgs e)
        {
            if (cbChannel2?.Checked == true)
                SwitchToChannel(2);
        }

        private void cbChannel3_CheckedChanged(object sender, EventArgs e)
        {
            if (cbChannel3?.Checked == true)
                SwitchToChannel(3);
        }

        private void cbChannel4_CheckedChanged(object sender, EventArgs e)
        {
            if (cbChannel4?.Checked == true)
                SwitchToChannel(4);
        }

        private void cbChannel5_CheckedChanged(object sender, EventArgs e)
        {
            if (cbChannel5?.Checked == true)
                SwitchToChannel(5);
        }
        private void cbChannel6_CheckedChanged(object sender, EventArgs e)
        {
            if (cbChannel6?.Checked == true)
                SwitchToChannel(6);
        }
        private void cbChannel7_CheckedChanged(object sender, EventArgs e)
        {
            if (cbChannel7?.Checked == true)
                SwitchToChannel(7);
        }
        private void cbChannel8_CheckedChanged(object sender, EventArgs e)
        {
            if (cbChannel8?.Checked == true)
                SwitchToChannel(8);
        }

        /// <summary>
        /// 获取线缆损耗值
        /// </summary>
        /// <returns>线缆损耗值（dB）</returns>
        public double GetCableLoss()
        {
            if (txtCableLoss != null && double.TryParse(txtCableLoss.Text, out double loss))
            {
                return loss;
            }
            return 0.0;
        }

        /// <summary>
        /// 频谱仪连接按钮点击事件
        /// </summary>
        private void btnConnectSpectrum_Click(object? sender, EventArgs e)
        {
            if (_instrumentManager == null || txtTestLog == null)
                return;

            string? address = _instrumentConfig?.Instruments?.SpectrumAnalyzer?.Address ?? "USB0::0x2A8D::0x0A0B::MY51282426::INSTR";
            bool success = _instrumentManager.ConnectSpectrumAnalyzer(address);
            UpdateInstrumentStatus();

            if (success)
            {
                txtTestLog.AppendText($"[{DateTime.Now:HH:mm:ss}] 频谱仪连接成功：{address}\r\n");
                txtTestLog.ScrollToCaret();
            }
            else
            {
                txtTestLog.AppendText($"[{DateTime.Now:HH:mm:ss}] 频谱仪连接失败：{address}\r\n");
                txtTestLog.ScrollToCaret();
            }
        }

        /// <summary>
        /// 信号源连接按钮点击事件
        /// </summary>
        private void btnConnectSignal_Click(object? sender, EventArgs e)
        {
            if (_instrumentManager == null || txtTestLog == null)
                return;

            string? address = _instrumentConfig?.Instruments?.SignalGenerator?.Address ?? "USB0::0x0957::0x1F01::MY61252673::INSTR";
            bool success = _instrumentManager.ConnectSignalGenerator(address);
            UpdateInstrumentStatus();

            if (success)
            {
                txtTestLog.AppendText($"[{DateTime.Now:HH:mm:ss}] 信号源连接成功：{address}\r\n");
                txtTestLog.ScrollToCaret();
            }
            else
            {
                txtTestLog.AppendText($"[{DateTime.Now:HH:mm:ss}] 信号源连接失败：{address}\r\n");
                txtTestLog.ScrollToCaret();
            }
        }

        /// <summary>
        /// 断开所有连接按钮点击事件
        /// </summary>
        private void btnDisconnectAll_Click(object sender, EventArgs e)
        {
            if (_instrumentManager == null || txtTestLog == null)
                return;

            _instrumentManager.DisconnectAll();
            UpdateInstrumentStatus();
            txtTestLog.AppendText($"[{DateTime.Now:HH:mm:ss}] 已断开所有仪表连接\r\n");
            txtTestLog.ScrollToCaret();
        }

        /// <summary>
        /// ZNB8连接按钮点击事件
        /// </summary>
        private void btnConnectZNB8_Click(object? sender, EventArgs e)
        {
            if (_instrumentManager == null || txtTestLog == null)
                return;

            string? address = _instrumentConfig?.Instruments?.ZNB8?.Address ?? "TCPIP0::192.168.0.252::inst0::INSTR";
            bool success = _instrumentManager.ConnectZNB8(address);
            UpdateInstrumentStatus();

            if (success)
            {
                txtTestLog.AppendText($"[{DateTime.Now:HH:mm:ss}] ZNB8连接成功：{address}\r\n");
                txtTestLog.ScrollToCaret();
            }
            else
            {
                txtTestLog.AppendText($"[{DateTime.Now:HH:mm:ss}] ZNB8连接失败：{address}\r\n");
                txtTestLog.ScrollToCaret();
            }
        }

        /// <summary>
        /// 动态范围及中频输出信号测试按钮点击事件
        /// </summary>
        private async void btnDynamicRangeTest_Click(object sender, EventArgs e)
        {
            await RunTestAsync("动态范围及中频输出信号测试");
        }

        /// <summary>
        /// 通道增益测试按钮点击事件
        /// </summary>
        private async void btnChannelGainTest_Click(object sender, EventArgs e)
        {
            await RunTestAsync("通道增益测试");
        }

        /// <summary>
        /// 带外抑制测试按钮点击事件
        /// </summary>
        private async void btnOutOfBandRejectionTest_Click(object sender, EventArgs e)
        {
            await RunTestAsync("带外抑制测试");
        }

        /// <summary>
        /// 带内增益波动测试按钮点击事件
        /// </summary>
        private async void btnGainFlatnessTest_Click(object sender, EventArgs e)
        {
            await RunTestAsync("带内增益波动测试");
        }

        /// <summary>
        /// 镜像抑制测试按钮点击事件
        /// </summary>
        private async void btnImageRejectionTest_Click(object sender, EventArgs e)
        {
            await RunTestAsync("镜像抑制测试");
        }

        /// <summary>
        /// 谐波抑制测试按钮点击事件
        /// </summary>
        private async void btnHarmonicSuppressionTest_Click(object sender, EventArgs e)
        {
            await RunTestAsync("谐波抑制测试");
        }

        /// <summary>
        /// 邻道抑制测试按钮点击事件
        /// </summary>
        private async void btnAdjacentChannelSuppressionTest_Click(object sender, EventArgs e)
        {
            await RunTestAsync("邻道抑制测试");
        }

        /// <summary>
        /// 端口驻波测试（输入）按钮点击事件
        /// </summary>
        private async void btnPortVSWRInputTest_Click(object sender, EventArgs e)
        {
            MessageBox.Show("请先完成网络分析仪校准，然后连接好测试线缆。\n\n校准完成后点击确定开始测试。",
                "端口驻波测试", MessageBoxButtons.OKCancel);
            await RunTestAsync("端口驻波测试（输入）");
        }

        /// <summary>
        /// 校准开关隔离度测试按钮点击事件
        /// </summary>
        private async void btnCalibrationSwitchIsolationTest_Click(object sender, EventArgs e)
        {
            await RunTestAsync("校准开关隔离度测试");
        }

        /// <summary>
        /// 批量测试按钮点击事件
        /// </summary>
        private async void btnBatchTest_Click(object sender, EventArgs e)
        {
            if (_testManager == null)
                return;

            _testManager.CableLossL2 = GetCableLoss();
            
            // 从textBox2获取参考电平偏移值并传递给测试管理器
            double refLevelOffset = 0.0;
            if (textBox2 != null && double.TryParse(textBox2.Text, out refLevelOffset))
            {
                _testManager.RefLevelOffset = refLevelOffset;
            }
            else
            {
                _testManager.RefLevelOffset = 0.0;
            }

            var selectedChannels = GetSelectedChannels();

            if (selectedChannels.Count == 0)
            {
                MessageBox.Show("请选择一个起始通道");
                return;
            }

            var startChannel = selectedChannels[0];

            var result = MessageBox.Show(
                "是否开始执行五通道批量测试？\n选择【是】执行五通道，选择【否】执行八通道",
                "批量测试确认",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            int maxChannel = result == DialogResult.Yes ? 5 : 8;
            var channelsToTest = Enumerable.Range(1, maxChannel).ToList();

            var testItems = _testManager.GetAllTestItemNames()
                .Where(item => item != "端口驻波测试（输入）")
                .ToList();

            await _testManager.RunBatchTestAsync(channelsToTest, testItems);

            MessageBox.Show("批量测试已完成！", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 导出报告按钮点击事件
        /// </summary>
        private void btnExportReport_Click(object sender, EventArgs e)
        {
            if (_testManager == null)
                return;

            var results = _testManager.GetAllTestResults();
            if (results.Count == 0)
            {
                MessageBox.Show("没有测试数据可导出", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var dialog = new SaveFileDialog();
            dialog.Filter = "Excel文件|*.xlsx";
            dialog.Title = "导出测试报告";
            dialog.FileName = $"测试报告_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "测试报告模板.xlsx");

            var service = new ExcelExportService();
            if (service.ExportToExcel(templatePath, dialog.FileName, results))
            {
                MessageBox.Show("导出成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// 清空结果按钮点击事件
        /// </summary>
        private void btnClearResults_Click(object sender, EventArgs e)
        {
            if (_testManager == null || dgvTestResults == null || txtTestLog == null)
                return;

            dgvTestResults.Rows.Clear();
            _testManager.ClearTestResults();
            txtTestLog.AppendText($"[{DateTime.Now:HH:mm:ss}] 已清空测试结果\r\n");
            txtTestLog.ScrollToCaret();
        }

        /// <summary>
        /// 停止测试按钮点击事件
        /// </summary>
        private void btnStopTest_Click(object sender, EventArgs e)
        {
            if (_testManager == null || txtTestLog == null)
                return;

            txtTestLog.AppendText($"[{DateTime.Now:HH:mm:ss}] 用户请求停止测试\r\n");
            txtTestLog.ScrollToCaret();
            _testManager.StopTest();
        }

        /// <summary>
        /// 执行测试
        /// </summary>
        /// <param name="testItemName">测试项名称</param>
        private async Task RunTestAsync(string testItemName)
        {
            if (_testManager == null)
                return;

            var selectedChannels = GetSelectedChannels();

            if (selectedChannels.Count == 0)
            {
                MessageBox.Show("请选择至少一个测试通道");
                return;
            }

            // 传递线缆损耗值给测试管理器
            _testManager.CableLossL2 = GetCableLoss();
            
            // 从textBox2获取参考电平偏移值并传递给测试管理器
            double refLevelOffset = 0.0;
            if (textBox2 != null && double.TryParse(textBox2.Text, out refLevelOffset))
            {
                _testManager.RefLevelOffset = refLevelOffset;
            }
            else
            {
                _testManager.RefLevelOffset = 0.0;
            }

            try
            {
                foreach (var channel in selectedChannels)
                {
                    await _testManager.RunTestAsync(channel, testItemName);
                }
            }
            catch (TaskCanceledException)
            {
                if (txtTestLog != null)
                {
                    txtTestLog.AppendText($"[{DateTime.Now:HH:mm:ss}] 测试已取消\r\n");
                    txtTestLog.ScrollToCaret();
                }
            }
            catch (OperationCanceledException)
            {
                if (txtTestLog != null)
                {
                    txtTestLog.AppendText($"[{DateTime.Now:HH:mm:ss}] 测试已取消\r\n");
                    txtTestLog.ScrollToCaret();
                }
            }
            catch (Exception ex)
            {
                if (txtTestLog != null)
                {
                    txtTestLog.AppendText($"[{DateTime.Now:HH:mm:ss}] 测试错误：{ex.Message}\r\n");
                    txtTestLog.ScrollToCaret();
                }
            }
        }

        /// <summary>
        /// 测试完成事件处理
        /// </summary>
        private void OnTestCompleted(object? sender, TestResult result)
        {
            // 更新测试结果表格
            Invoke(new Action(() =>
            {
                dgvTestResults?.Rows.Add(
                    result.Channel,
                    result.TestItem,
                    result.Value.ToString("F2"),
                    result.Unit,
                    result.IsPass ? "合格" : "不合格",
                    result.TestTime.ToString("yyyy-MM-dd HH:mm:ss")
                );

                // 滚动到最新行
                if (dgvTestResults?.Rows.Count > 0)
                {
                    dgvTestResults.FirstDisplayedScrollingRowIndex = dgvTestResults.Rows.Count - 1;
                }
            }));
        }

        /// <summary>
        /// 日志生成事件处理
        /// </summary>
        private void OnLogGenerated(object? sender, string log)
        {
            // 更新测试日志
            Invoke(new Action(() =>
            {
                txtTestLog?.AppendText($"[{DateTime.Now:HH:mm:ss}] {log}\r\n");
                txtTestLog?.ScrollToCaret();
            }));
        }
        
        /// <summary>
        /// 电源开关按钮点击事件
        /// </summary>
        private void PowerOn_Click(object sender, EventArgs e)
        {
            if (_instrumentManager == null || txtTestLog == null)
                return;
            
            try
            {
                _isPowerOn = !_isPowerOn;
                
                _instrumentManager.ODP3063.EnableChannel2Output(_isPowerOn);
                _instrumentManager.SetPowerState(_isPowerOn);
                
                if (_isPowerOn)
                {
                    if (PowerSwitch != null)
                    {
                        PowerSwitch.BackColor = Color.LightCoral;
                        PowerSwitch.Text = "关闭供电";
                    }
                    txtTestLog.AppendText($"[{DateTime.Now:HH:mm:ss}] 模块供电已开启\r\n");
                }
                else
                {
                    if (PowerSwitch != null)
                    {
                        PowerSwitch.BackColor = Color.LightGreen;
                        PowerSwitch.Text = "开启供电";
                    }
                    txtTestLog.AppendText($"[{DateTime.Now:HH:mm:ss}] 模块供电已关闭\r\n");
                }
                txtTestLog.ScrollToCaret();
            }
            catch (Exception ex)
            {
                _isPowerOn = !_isPowerOn;
                txtTestLog.AppendText($"[{DateTime.Now:HH:mm:ss}] 模块供电控制失败：{ex.Message}\r\n");
                txtTestLog.ScrollToCaret();
            }
        }
        
        /// <summary>
        /// 发送命令按钮点击事件处理程序
        /// </summary>
        private void Sendmessage_Click(object sender, EventArgs e)
        {
            if (_toolingBoard == null || textBox1 == null || txtTestLog == null)
                return;
            
            try
            {
                string commandText = textBox1.Text.Trim();
                if (string.IsNullOrEmpty(commandText))
                {
                    MessageBox.Show("请输入要发送的命令");
                    return;
                }
                
                // 将十六进制命令字符串转换为byte数组
                byte[] commandBytes = HexStringToByteArray(commandText);
                
                // 发送命令到串口
                bool success = _toolingBoard.SendFrame(commandBytes);
                
                // 记录发送结果
                string logMessage = success 
                    ? $"[{DateTime.Now:HH:mm:ss}] 命令发送成功：{commandText}\r\n"
                    : $"[{DateTime.Now:HH:mm:ss}] 命令发送失败：{commandText}\r\n";
                
                txtTestLog.AppendText(logMessage);
                txtTestLog.ScrollToCaret();
            }
            catch (Exception ex)
            {
                txtTestLog.AppendText($"[{DateTime.Now:HH:mm:ss}] 命令发送错误：{ex.Message}\r\n");
                txtTestLog.ScrollToCaret();
            }
        }
        
        /// <summary>
        /// 将十六进制字符串转换为byte数组
        /// </summary>
        /// <param name="hexString">空格分隔的十六进制字符串</param>
        /// <returns>转换后的byte数组</returns>
        private byte[] HexStringToByteArray(string hexString)
        {
            // 移除所有空格
            hexString = hexString.Replace(" ", "");
            
            // 检查字符串长度是否为偶数
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException("十六进制字符串长度必须为偶数");
            }
            
            byte[] result = new byte[hexString.Length / 2];
            
            for (int i = 0; i < hexString.Length; i += 2)
            {
                result[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            }
            
            return result;
        }

        /// <summary>
        /// 校准菜单点击事件
        /// </summary>
        private void 校准ToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (_instrumentManager == null)
                return;

            if (_isCalibrationMode)
                return;

            _isCalibrationMode = true;
            
            _calibrationForm = new FormCalibration(_instrumentManager, this);
            _calibrationForm.FormClosed += CalibrationForm_FormClosed;
            _calibrationForm.Show();
            
            this.Hide();
        }

        /// <summary>
        /// 常温测试菜单点击事件
        /// </summary>
        private void 常温测试ToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (_isCalibrationMode)
            {
                if (_calibrationForm != null && !_calibrationForm.IsDisposed)
                {
                    _calibrationForm.Close();
                    _calibrationForm = null;
                }
                _isCalibrationMode = false;
            }
            else if (_isChamberMode)
            {
                _isChamberMode = false;
                // 清理温箱控制表单
                if (_chamberControlForm != null && !_chamberControlForm.IsDisposed)
                {
                    _chamberControlForm.Close();
                    _chamberControlForm = null;
                }
            }

            // 显示正常测试界面控件
            gbInstrumentStatus?.Show();
            gbTestItems?.Show();
            gbChannels?.Show();
            dgvTestResults?.Show();
            btnClearResults?.Show();
            btnExportReport?.Show();
            PowerSwitch?.Show();
            textBox1?.Show();
            Sendmessage?.Show();

            this.Show();
        }

        /// <summary>
        /// 校准界面关闭事件
        /// </summary>
        private void CalibrationForm_FormClosed(object? sender, FormClosedEventArgs e)
        {
            if (_isCalibrationMode)
            {
                _isCalibrationMode = false;
                this.Show();
            }
        }

        /// <summary>
        /// 温箱控制菜单点击事件
        /// </summary>
        private void 温箱控制ToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (_isCalibrationMode)
                return;

            if (!_isChamberMode)
            {
                _isChamberMode = true;
                ShowChamberControlForm();
            }
        }

        /// <summary>
        /// 显示温箱控制表单
        /// </summary>
        private void ShowChamberControlForm()
        {
            // 隐藏当前表单
            this.Hide();

            // 创建并显示温箱控制表单
            _chamberControlForm = new ChamberControlForm();
            _chamberControlForm.FormClosed += ChamberControlForm_FormClosed;
            _chamberControlForm.Show();
        }

        /// <summary>
        /// 温箱控制表单关闭事件
        /// </summary>
        private void ChamberControlForm_FormClosed(object? sender, FormClosedEventArgs e)
        {
            _isChamberMode = false;
            _chamberControlForm = null;
            this.Show();
        }
    }
}