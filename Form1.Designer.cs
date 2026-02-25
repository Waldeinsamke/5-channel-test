namespace 五通道自动测试
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // 常规测试界面控件
        private GroupBox gbInstrumentStatus;
        private Label label1;
        private TextBox textBox2;
        private Label ZNB8;
        private Label lblZNB8Status;
        private Button btnDisconnectAll;
        private PictureBox pbZNB8Status;
        private Label lblSignalStatus;
        private Button btnConnectZNB8;
        private PictureBox pbSignalStatus;
        private Button btnConnectSignal;
        private Label lblSignalGenerator;
        private Label lblSpectrumStatus;
        private PictureBox pbSpectrumStatus;
        private Button btnConnectSpectrum;
        private Label lblSpectrumAnalyzer;
        private Label lblCableLoss;
        private TextBox txtCableLoss;
        private Label lblBoardSerialPort;
        private ComboBox cbSerialPort;
        private Button btnConnectBoard;
        private Label lblBoardStatus;
        private Button btnBatchTest;
        private Button btnStopTest;
        private GroupBox gbTestItems;
        private Button btnDynamicRangeTest;
        private Button btnChannelGainTest;
        private Button btnOutOfBandRejectionTest;
        private Button btnGainFlatnessTest;
        private Button btnImageRejectionTest;
        private Button btnHarmonicSuppressionTest;
        private Button btnAdjacentChannelSuppressionTest;
        private Button btnPortVSWRInputTest;
        private Button btnCalibrationSwitchIsolationTest;
        private GroupBox gbChannels;
        private RadioButton cbChannel5;
        private RadioButton cbChannel4;
        private RadioButton cbChannel3;
        private RadioButton cbChannel2;
        private RadioButton cbChannel1;
        private Button btnClearResults;
        private Button btnExportReport;
        private DataGridView dgvTestResults;
        private RichTextBox txtTestLog;
        private Button PowerSwitch;
        private TextBox textBox1;
        private Button Sendmessage;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem 常温测试ToolStripMenuItem;
        private ToolStripMenuItem 校准ToolStripMenuItem;
        private ToolStripMenuItem 温箱控制ToolStripMenuItem;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            gbInstrumentStatus = new GroupBox();
            label1 = new Label();
            textBox2 = new TextBox();
            ZNB8 = new Label();
            lblZNB8Status = new Label();
            btnDisconnectAll = new Button();
            pbZNB8Status = new PictureBox();
            lblSignalStatus = new Label();
            btnConnectZNB8 = new Button();
            pbSignalStatus = new PictureBox();
            btnConnectSignal = new Button();
            lblSignalGenerator = new Label();
            PowerSwitch = new Button();
            lblSpectrumStatus = new Label();
            pbSpectrumStatus = new PictureBox();
            btnConnectSpectrum = new Button();
            lblSpectrumAnalyzer = new Label();
            lblCableLoss = new Label();
            txtCableLoss = new TextBox();
            lblBoardSerialPort = new Label();
            cbSerialPort = new ComboBox();
            btnConnectBoard = new Button();
            lblBoardStatus = new Label();
            btnBatchTest = new Button();
            btnStopTest = new Button();
            gbTestItems = new GroupBox();
            btnDynamicRangeTest = new Button();
            btnChannelGainTest = new Button();
            btnOutOfBandRejectionTest = new Button();
            btnGainFlatnessTest = new Button();
            btnImageRejectionTest = new Button();
            btnHarmonicSuppressionTest = new Button();
            btnAdjacentChannelSuppressionTest = new Button();
            btnPortVSWRInputTest = new Button();
            btnCalibrationSwitchIsolationTest = new Button();
            gbChannels = new GroupBox();
            cbChannel8 = new RadioButton();
            cbChannel7 = new RadioButton();
            cbChannel6 = new RadioButton();
            cbChannel5 = new RadioButton();
            cbChannel4 = new RadioButton();
            cbChannel3 = new RadioButton();
            cbChannel2 = new RadioButton();
            cbChannel1 = new RadioButton();
            btnClearResults = new Button();
            btnExportReport = new Button();
            dgvTestResults = new DataGridView();
            txtTestLog = new RichTextBox();
            textBox1 = new TextBox();
            Sendmessage = new Button();
            menuStrip1 = new MenuStrip();
            常温测试ToolStripMenuItem = new ToolStripMenuItem();
            校准ToolStripMenuItem = new ToolStripMenuItem();
            温箱控制ToolStripMenuItem = new ToolStripMenuItem();
            gbInstrumentStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbZNB8Status).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbSignalStatus).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbSpectrumStatus).BeginInit();
            gbTestItems.SuspendLayout();
            gbChannels.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvTestResults).BeginInit();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // gbInstrumentStatus
            // 
            gbInstrumentStatus.Controls.Add(label1);
            gbInstrumentStatus.Controls.Add(textBox2);
            gbInstrumentStatus.Controls.Add(ZNB8);
            gbInstrumentStatus.Controls.Add(lblZNB8Status);
            gbInstrumentStatus.Controls.Add(btnDisconnectAll);
            gbInstrumentStatus.Controls.Add(pbZNB8Status);
            gbInstrumentStatus.Controls.Add(lblSignalStatus);
            gbInstrumentStatus.Controls.Add(btnConnectZNB8);
            gbInstrumentStatus.Controls.Add(pbSignalStatus);
            gbInstrumentStatus.Controls.Add(btnConnectSignal);
            gbInstrumentStatus.Controls.Add(lblSignalGenerator);
            gbInstrumentStatus.Controls.Add(PowerSwitch);
            gbInstrumentStatus.Controls.Add(lblSpectrumStatus);
            gbInstrumentStatus.Controls.Add(pbSpectrumStatus);
            gbInstrumentStatus.Controls.Add(btnConnectSpectrum);
            gbInstrumentStatus.Controls.Add(lblSpectrumAnalyzer);
            gbInstrumentStatus.Controls.Add(lblCableLoss);
            gbInstrumentStatus.Controls.Add(txtCableLoss);
            gbInstrumentStatus.Controls.Add(lblBoardSerialPort);
            gbInstrumentStatus.Controls.Add(cbSerialPort);
            gbInstrumentStatus.Controls.Add(btnConnectBoard);
            gbInstrumentStatus.Controls.Add(lblBoardStatus);
            gbInstrumentStatus.Location = new Point(15, 39);
            gbInstrumentStatus.Margin = new Padding(4);
            gbInstrumentStatus.Name = "gbInstrumentStatus";
            gbInstrumentStatus.Padding = new Padding(4);
            gbInstrumentStatus.Size = new Size(1089, 142);
            gbInstrumentStatus.TabIndex = 0;
            gbInstrumentStatus.TabStop = false;
            gbInstrumentStatus.Text = "仪表状态";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(779, 95);
            label1.Name = "label1";
            label1.Size = new Size(69, 20);
            label1.TabIndex = 23;
            label1.Text = "输出损耗";
            // 
            // textBox2
            // 
            textBox2.Location = new Point(853, 92);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(99, 27);
            textBox2.TabIndex = 22;
            textBox2.Text = "4.2";
            textBox2.TextAlign = HorizontalAlignment.Right;
            // 
            // ZNB8
            // 
            ZNB8.AutoSize = true;
            ZNB8.Location = new Point(19, 110);
            ZNB8.Margin = new Padding(4, 0, 4, 0);
            ZNB8.Name = "ZNB8";
            ZNB8.Size = new Size(39, 20);
            ZNB8.TabIndex = 20;
            ZNB8.Text = "矢网";
            // 
            // lblZNB8Status
            // 
            lblZNB8Status.AutoSize = true;
            lblZNB8Status.ForeColor = Color.Black;
            lblZNB8Status.Location = new Point(110, 112);
            lblZNB8Status.Margin = new Padding(4, 0, 4, 0);
            lblZNB8Status.Name = "lblZNB8Status";
            lblZNB8Status.Size = new Size(54, 20);
            lblZNB8Status.TabIndex = 19;
            lblZNB8Status.Text = "未连接";
            // 
            // btnDisconnectAll
            // 
            btnDisconnectAll.Location = new Point(284, 28);
            btnDisconnectAll.Margin = new Padding(4);
            btnDisconnectAll.Name = "btnDisconnectAll";
            btnDisconnectAll.Size = new Size(116, 105);
            btnDisconnectAll.TabIndex = 9;
            btnDisconnectAll.Text = "断开所有连接";
            btnDisconnectAll.UseVisualStyleBackColor = true;
            btnDisconnectAll.Click += btnDisconnectAll_Click;
            // 
            // pbZNB8Status
            // 
            pbZNB8Status.BackColor = Color.Red;
            pbZNB8Status.Location = new Point(71, 108);
            pbZNB8Status.Margin = new Padding(4);
            pbZNB8Status.Name = "pbZNB8Status";
            pbZNB8Status.Size = new Size(26, 24);
            pbZNB8Status.TabIndex = 21;
            pbZNB8Status.TabStop = false;
            // 
            // lblSignalStatus
            // 
            lblSignalStatus.AutoSize = true;
            lblSignalStatus.Location = new Point(110, 72);
            lblSignalStatus.Margin = new Padding(4, 0, 4, 0);
            lblSignalStatus.Name = "lblSignalStatus";
            lblSignalStatus.Size = new Size(54, 20);
            lblSignalStatus.TabIndex = 8;
            lblSignalStatus.Text = "未连接";
            // 
            // btnConnectZNB8
            // 
            btnConnectZNB8.Location = new Point(180, 104);
            btnConnectZNB8.Margin = new Padding(4);
            btnConnectZNB8.Name = "btnConnectZNB8";
            btnConnectZNB8.Size = new Size(96, 29);
            btnConnectZNB8.TabIndex = 17;
            btnConnectZNB8.Text = "连接";
            btnConnectZNB8.UseVisualStyleBackColor = true;
            btnConnectZNB8.Click += btnConnectZNB8_Click;
            // 
            // pbSignalStatus
            // 
            pbSignalStatus.BackColor = Color.Red;
            pbSignalStatus.Location = new Point(71, 68);
            pbSignalStatus.Margin = new Padding(4);
            pbSignalStatus.Name = "pbSignalStatus";
            pbSignalStatus.Size = new Size(26, 24);
            pbSignalStatus.TabIndex = 7;
            pbSignalStatus.TabStop = false;
            // 
            // btnConnectSignal
            // 
            btnConnectSignal.Location = new Point(180, 67);
            btnConnectSignal.Margin = new Padding(4);
            btnConnectSignal.Name = "btnConnectSignal";
            btnConnectSignal.Size = new Size(96, 29);
            btnConnectSignal.TabIndex = 6;
            btnConnectSignal.Text = "连接";
            btnConnectSignal.UseVisualStyleBackColor = true;
            btnConnectSignal.Click += btnConnectSignal_Click;
            // 
            // lblSignalGenerator
            // 
            lblSignalGenerator.AutoSize = true;
            lblSignalGenerator.Location = new Point(12, 71);
            lblSignalGenerator.Margin = new Padding(4, 0, 4, 0);
            lblSignalGenerator.Name = "lblSignalGenerator";
            lblSignalGenerator.Size = new Size(54, 20);
            lblSignalGenerator.TabIndex = 4;
            lblSignalGenerator.Text = "信号源";
            // 
            // PowerSwitch
            // 
            PowerSwitch.Font = new Font("Microsoft YaHei UI Light", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            PowerSwitch.Location = new Point(972, 25);
            PowerSwitch.Name = "PowerSwitch";
            PowerSwitch.Size = new Size(110, 105);
            PowerSwitch.TabIndex = 22;
            PowerSwitch.Text = "模块供电";
            PowerSwitch.UseVisualStyleBackColor = true;
            PowerSwitch.Click += PowerOn_Click;
            // 
            // lblSpectrumStatus
            // 
            lblSpectrumStatus.AutoSize = true;
            lblSpectrumStatus.Location = new Point(110, 33);
            lblSpectrumStatus.Margin = new Padding(4, 0, 4, 0);
            lblSpectrumStatus.Name = "lblSpectrumStatus";
            lblSpectrumStatus.Size = new Size(54, 20);
            lblSpectrumStatus.TabIndex = 3;
            lblSpectrumStatus.Text = "未连接";
            // 
            // pbSpectrumStatus
            // 
            pbSpectrumStatus.BackColor = Color.Red;
            pbSpectrumStatus.Location = new Point(71, 31);
            pbSpectrumStatus.Margin = new Padding(4);
            pbSpectrumStatus.Name = "pbSpectrumStatus";
            pbSpectrumStatus.Size = new Size(26, 24);
            pbSpectrumStatus.TabIndex = 2;
            pbSpectrumStatus.TabStop = false;
            // 
            // btnConnectSpectrum
            // 
            btnConnectSpectrum.Location = new Point(180, 28);
            btnConnectSpectrum.Margin = new Padding(4);
            btnConnectSpectrum.Name = "btnConnectSpectrum";
            btnConnectSpectrum.Size = new Size(96, 29);
            btnConnectSpectrum.TabIndex = 1;
            btnConnectSpectrum.Text = "连接";
            btnConnectSpectrum.UseVisualStyleBackColor = true;
            btnConnectSpectrum.Click += btnConnectSpectrum_Click;
            // 
            // lblSpectrumAnalyzer
            // 
            lblSpectrumAnalyzer.AutoSize = true;
            lblSpectrumAnalyzer.Location = new Point(12, 33);
            lblSpectrumAnalyzer.Margin = new Padding(4, 0, 4, 0);
            lblSpectrumAnalyzer.Name = "lblSpectrumAnalyzer";
            lblSpectrumAnalyzer.Size = new Size(54, 20);
            lblSpectrumAnalyzer.TabIndex = 0;
            lblSpectrumAnalyzer.Text = "频谱仪";
            // 
            // lblCableLoss
            // 
            lblCableLoss.AutoSize = true;
            lblCableLoss.Location = new Point(621, 95);
            lblCableLoss.Margin = new Padding(4, 0, 4, 0);
            lblCableLoss.Name = "lblCableLoss";
            lblCableLoss.Size = new Size(39, 20);
            lblCableLoss.TabIndex = 10;
            lblCableLoss.Text = "线损";
            // 
            // txtCableLoss
            // 
            txtCableLoss.Location = new Point(671, 92);
            txtCableLoss.Margin = new Padding(4);
            txtCableLoss.Name = "txtCableLoss";
            txtCableLoss.Size = new Size(99, 27);
            txtCableLoss.TabIndex = 11;
            txtCableLoss.Text = "12.5";
            txtCableLoss.TextAlign = HorizontalAlignment.Right;
            // 
            // lblBoardSerialPort
            // 
            lblBoardSerialPort.AutoSize = true;
            lblBoardSerialPort.Location = new Point(621, 50);
            lblBoardSerialPort.Margin = new Padding(4, 0, 4, 0);
            lblBoardSerialPort.Name = "lblBoardSerialPort";
            lblBoardSerialPort.Size = new Size(39, 20);
            lblBoardSerialPort.TabIndex = 12;
            lblBoardSerialPort.Text = "串口";
            // 
            // cbSerialPort
            // 
            cbSerialPort.DropDownStyle = ComboBoxStyle.DropDownList;
            cbSerialPort.FormattingEnabled = true;
            cbSerialPort.Location = new Point(670, 46);
            cbSerialPort.Margin = new Padding(4);
            cbSerialPort.Name = "cbSerialPort";
            cbSerialPort.Size = new Size(100, 28);
            cbSerialPort.TabIndex = 13;
            // 
            // btnConnectBoard
            // 
            btnConnectBoard.Location = new Point(853, 47);
            btnConnectBoard.Margin = new Padding(4);
            btnConnectBoard.Name = "btnConnectBoard";
            btnConnectBoard.Size = new Size(100, 29);
            btnConnectBoard.TabIndex = 14;
            btnConnectBoard.Text = "打开串口";
            btnConnectBoard.UseVisualStyleBackColor = true;
            btnConnectBoard.Click += btnConnectBoard_Click;
            // 
            // lblBoardStatus
            // 
            lblBoardStatus.AutoSize = true;
            lblBoardStatus.Location = new Point(787, 51);
            lblBoardStatus.Margin = new Padding(4, 0, 4, 0);
            lblBoardStatus.Name = "lblBoardStatus";
            lblBoardStatus.Size = new Size(54, 20);
            lblBoardStatus.TabIndex = 15;
            lblBoardStatus.Text = "未连接";
            // 
            // btnBatchTest
            // 
            btnBatchTest.Location = new Point(975, 196);
            btnBatchTest.Margin = new Padding(4);
            btnBatchTest.Name = "btnBatchTest";
            btnBatchTest.Size = new Size(129, 59);
            btnBatchTest.TabIndex = 3;
            btnBatchTest.Text = "批量测试";
            btnBatchTest.UseVisualStyleBackColor = true;
            btnBatchTest.Click += btnBatchTest_Click;
            // 
            // btnStopTest
            // 
            btnStopTest.Location = new Point(975, 275);
            btnStopTest.Margin = new Padding(4);
            btnStopTest.Name = "btnStopTest";
            btnStopTest.Size = new Size(129, 59);
            btnStopTest.TabIndex = 2;
            btnStopTest.Text = "停止测试";
            btnStopTest.UseVisualStyleBackColor = true;
            btnStopTest.Click += btnStopTest_Click;
            // 
            // gbTestItems
            // 
            gbTestItems.Controls.Add(btnDynamicRangeTest);
            gbTestItems.Controls.Add(btnChannelGainTest);
            gbTestItems.Controls.Add(btnOutOfBandRejectionTest);
            gbTestItems.Controls.Add(btnGainFlatnessTest);
            gbTestItems.Controls.Add(btnImageRejectionTest);
            gbTestItems.Controls.Add(btnHarmonicSuppressionTest);
            gbTestItems.Controls.Add(btnAdjacentChannelSuppressionTest);
            gbTestItems.Controls.Add(btnPortVSWRInputTest);
            gbTestItems.Controls.Add(btnCalibrationSwitchIsolationTest);
            gbTestItems.Location = new Point(225, 189);
            gbTestItems.Margin = new Padding(4);
            gbTestItems.Name = "gbTestItems";
            gbTestItems.Padding = new Padding(4);
            gbTestItems.Size = new Size(733, 147);
            gbTestItems.TabIndex = 1;
            gbTestItems.TabStop = false;
            gbTestItems.Text = "测试项";
            // 
            // btnDynamicRangeTest
            // 
            btnDynamicRangeTest.Location = new Point(18, 28);
            btnDynamicRangeTest.Margin = new Padding(4);
            btnDynamicRangeTest.Name = "btnDynamicRangeTest";
            btnDynamicRangeTest.Size = new Size(132, 38);
            btnDynamicRangeTest.TabIndex = 0;
            btnDynamicRangeTest.Text = "动态范围";
            btnDynamicRangeTest.UseVisualStyleBackColor = true;
            btnDynamicRangeTest.Click += btnDynamicRangeTest_Click;
            // 
            // btnChannelGainTest
            // 
            btnChannelGainTest.Location = new Point(18, 89);
            btnChannelGainTest.Margin = new Padding(4);
            btnChannelGainTest.Name = "btnChannelGainTest";
            btnChannelGainTest.Size = new Size(132, 38);
            btnChannelGainTest.TabIndex = 1;
            btnChannelGainTest.Text = "通道增益";
            btnChannelGainTest.UseVisualStyleBackColor = true;
            btnChannelGainTest.Click += btnChannelGainTest_Click;
            // 
            // btnOutOfBandRejectionTest
            // 
            btnOutOfBandRejectionTest.Location = new Point(158, 28);
            btnOutOfBandRejectionTest.Margin = new Padding(4);
            btnOutOfBandRejectionTest.Name = "btnOutOfBandRejectionTest";
            btnOutOfBandRejectionTest.Size = new Size(132, 38);
            btnOutOfBandRejectionTest.TabIndex = 2;
            btnOutOfBandRejectionTest.Text = "带外抑制";
            btnOutOfBandRejectionTest.UseVisualStyleBackColor = true;
            btnOutOfBandRejectionTest.Click += btnOutOfBandRejectionTest_Click;
            // 
            // btnGainFlatnessTest
            // 
            btnGainFlatnessTest.Location = new Point(298, 28);
            btnGainFlatnessTest.Margin = new Padding(4);
            btnGainFlatnessTest.Name = "btnGainFlatnessTest";
            btnGainFlatnessTest.Size = new Size(132, 38);
            btnGainFlatnessTest.TabIndex = 3;
            btnGainFlatnessTest.Text = "带内增益波动";
            btnGainFlatnessTest.UseVisualStyleBackColor = true;
            btnGainFlatnessTest.Click += btnGainFlatnessTest_Click;
            // 
            // btnImageRejectionTest
            // 
            btnImageRejectionTest.Location = new Point(438, 28);
            btnImageRejectionTest.Margin = new Padding(4);
            btnImageRejectionTest.Name = "btnImageRejectionTest";
            btnImageRejectionTest.Size = new Size(132, 38);
            btnImageRejectionTest.TabIndex = 4;
            btnImageRejectionTest.Text = "镜像抑制";
            btnImageRejectionTest.UseVisualStyleBackColor = true;
            btnImageRejectionTest.Click += btnImageRejectionTest_Click;
            // 
            // btnHarmonicSuppressionTest
            // 
            btnHarmonicSuppressionTest.Location = new Point(578, 28);
            btnHarmonicSuppressionTest.Margin = new Padding(4);
            btnHarmonicSuppressionTest.Name = "btnHarmonicSuppressionTest";
            btnHarmonicSuppressionTest.Size = new Size(132, 38);
            btnHarmonicSuppressionTest.TabIndex = 5;
            btnHarmonicSuppressionTest.Text = "谐波抑制";
            btnHarmonicSuppressionTest.UseVisualStyleBackColor = true;
            btnHarmonicSuppressionTest.Click += btnHarmonicSuppressionTest_Click;
            // 
            // btnAdjacentChannelSuppressionTest
            // 
            btnAdjacentChannelSuppressionTest.Location = new Point(158, 92);
            btnAdjacentChannelSuppressionTest.Margin = new Padding(4);
            btnAdjacentChannelSuppressionTest.Name = "btnAdjacentChannelSuppressionTest";
            btnAdjacentChannelSuppressionTest.Size = new Size(132, 38);
            btnAdjacentChannelSuppressionTest.TabIndex = 6;
            btnAdjacentChannelSuppressionTest.Text = "邻道抑制";
            btnAdjacentChannelSuppressionTest.UseVisualStyleBackColor = true;
            btnAdjacentChannelSuppressionTest.Click += btnAdjacentChannelSuppressionTest_Click;
            // 
            // btnPortVSWRInputTest
            // 
            btnPortVSWRInputTest.Location = new Point(438, 92);
            btnPortVSWRInputTest.Margin = new Padding(4);
            btnPortVSWRInputTest.Name = "btnPortVSWRInputTest";
            btnPortVSWRInputTest.Size = new Size(132, 38);
            btnPortVSWRInputTest.TabIndex = 7;
            btnPortVSWRInputTest.Text = "端口驻波";
            btnPortVSWRInputTest.UseVisualStyleBackColor = true;
            btnPortVSWRInputTest.Click += btnPortVSWRInputTest_Click;
            // 
            // btnCalibrationSwitchIsolationTest
            // 
            btnCalibrationSwitchIsolationTest.Location = new Point(298, 92);
            btnCalibrationSwitchIsolationTest.Margin = new Padding(4);
            btnCalibrationSwitchIsolationTest.Name = "btnCalibrationSwitchIsolationTest";
            btnCalibrationSwitchIsolationTest.Size = new Size(132, 38);
            btnCalibrationSwitchIsolationTest.TabIndex = 8;
            btnCalibrationSwitchIsolationTest.Text = "校准开关隔离度";
            btnCalibrationSwitchIsolationTest.UseVisualStyleBackColor = true;
            btnCalibrationSwitchIsolationTest.Click += btnCalibrationSwitchIsolationTest_Click;
            // 
            // gbChannels
            // 
            gbChannels.Controls.Add(cbChannel8);
            gbChannels.Controls.Add(cbChannel7);
            gbChannels.Controls.Add(cbChannel6);
            gbChannels.Controls.Add(cbChannel5);
            gbChannels.Controls.Add(cbChannel4);
            gbChannels.Controls.Add(cbChannel3);
            gbChannels.Controls.Add(cbChannel2);
            gbChannels.Controls.Add(cbChannel1);
            gbChannels.Location = new Point(15, 189);
            gbChannels.Margin = new Padding(4);
            gbChannels.Name = "gbChannels";
            gbChannels.Padding = new Padding(4);
            gbChannels.Size = new Size(193, 147);
            gbChannels.TabIndex = 0;
            gbChannels.TabStop = false;
            gbChannels.Text = "通道选择";
            // 
            // cbChannel8
            // 
            cbChannel8.AutoSize = true;
            cbChannel8.Location = new Point(99, 103);
            cbChannel8.Name = "cbChannel8";
            cbChannel8.Size = new Size(72, 24);
            cbChannel8.TabIndex = 7;
            cbChannel8.TabStop = true;
            cbChannel8.Text = "通道H";
            cbChannel8.UseVisualStyleBackColor = true;
            cbChannel8.CheckedChanged += cbChannel8_CheckedChanged;
            // 
            // cbChannel7
            // 
            cbChannel7.AutoSize = true;
            cbChannel7.Location = new Point(99, 80);
            cbChannel7.Name = "cbChannel7";
            cbChannel7.Size = new Size(71, 24);
            cbChannel7.TabIndex = 6;
            cbChannel7.TabStop = true;
            cbChannel7.Text = "通道G";
            cbChannel7.UseVisualStyleBackColor = true;
            cbChannel7.CheckedChanged += cbChannel7_CheckedChanged;
            // 
            // cbChannel6
            // 
            cbChannel6.AutoSize = true;
            cbChannel6.Location = new Point(99, 56);
            cbChannel6.Name = "cbChannel6";
            cbChannel6.Size = new Size(68, 24);
            cbChannel6.TabIndex = 5;
            cbChannel6.TabStop = true;
            cbChannel6.Text = "通道F";
            cbChannel6.UseVisualStyleBackColor = true;
            cbChannel6.CheckedChanged += cbChannel6_CheckedChanged;
            // 
            // cbChannel5
            // 
            cbChannel5.AutoSize = true;
            cbChannel5.Location = new Point(99, 33);
            cbChannel5.Margin = new Padding(4);
            cbChannel5.Name = "cbChannel5";
            cbChannel5.Size = new Size(68, 24);
            cbChannel5.TabIndex = 4;
            cbChannel5.Text = "通道E";
            cbChannel5.UseVisualStyleBackColor = true;
            cbChannel5.CheckedChanged += cbChannel5_CheckedChanged;
            // 
            // cbChannel4
            // 
            cbChannel4.AutoSize = true;
            cbChannel4.Location = new Point(23, 103);
            cbChannel4.Margin = new Padding(4);
            cbChannel4.Name = "cbChannel4";
            cbChannel4.Size = new Size(71, 24);
            cbChannel4.TabIndex = 3;
            cbChannel4.Text = "通道D";
            cbChannel4.UseVisualStyleBackColor = true;
            cbChannel4.CheckedChanged += cbChannel4_CheckedChanged;
            // 
            // cbChannel3
            // 
            cbChannel3.AutoSize = true;
            cbChannel3.Location = new Point(23, 80);
            cbChannel3.Margin = new Padding(4);
            cbChannel3.Name = "cbChannel3";
            cbChannel3.Size = new Size(70, 24);
            cbChannel3.TabIndex = 2;
            cbChannel3.Text = "通道C";
            cbChannel3.UseVisualStyleBackColor = true;
            cbChannel3.CheckedChanged += cbChannel3_CheckedChanged;
            // 
            // cbChannel2
            // 
            cbChannel2.AutoSize = true;
            cbChannel2.Location = new Point(23, 56);
            cbChannel2.Margin = new Padding(4);
            cbChannel2.Name = "cbChannel2";
            cbChannel2.Size = new Size(69, 24);
            cbChannel2.TabIndex = 1;
            cbChannel2.Text = "通道B";
            cbChannel2.UseVisualStyleBackColor = true;
            cbChannel2.CheckedChanged += cbChannel2_CheckedChanged;
            // 
            // cbChannel1
            // 
            cbChannel1.AutoSize = true;
            cbChannel1.Location = new Point(23, 33);
            cbChannel1.Margin = new Padding(4);
            cbChannel1.Name = "cbChannel1";
            cbChannel1.Size = new Size(71, 24);
            cbChannel1.TabIndex = 0;
            cbChannel1.Text = "通道A";
            cbChannel1.UseVisualStyleBackColor = true;
            cbChannel1.CheckedChanged += cbChannel1_CheckedChanged;
            // 
            // btnClearResults
            // 
            btnClearResults.Location = new Point(152, 667);
            btnClearResults.Margin = new Padding(4);
            btnClearResults.Name = "btnClearResults";
            btnClearResults.Size = new Size(129, 35);
            btnClearResults.TabIndex = 2;
            btnClearResults.Text = "清空结果";
            btnClearResults.UseVisualStyleBackColor = true;
            btnClearResults.Click += btnClearResults_Click;
            // 
            // btnExportReport
            // 
            btnExportReport.Location = new Point(15, 667);
            btnExportReport.Margin = new Padding(4);
            btnExportReport.Name = "btnExportReport";
            btnExportReport.Size = new Size(129, 35);
            btnExportReport.TabIndex = 1;
            btnExportReport.Text = "导出报告";
            btnExportReport.UseVisualStyleBackColor = true;
            btnExportReport.Click += btnExportReport_Click;
            // 
            // dgvTestResults
            // 
            dgvTestResults.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvTestResults.Location = new Point(13, 357);
            dgvTestResults.Margin = new Padding(4);
            dgvTestResults.Name = "dgvTestResults";
            dgvTestResults.RowHeadersWidth = 51;
            dgvTestResults.Size = new Size(1091, 302);
            dgvTestResults.TabIndex = 0;
            // 
            // txtTestLog
            // 
            txtTestLog.BackColor = SystemColors.Window;
            txtTestLog.BorderStyle = BorderStyle.None;
            txtTestLog.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtTestLog.ForeColor = SystemColors.WindowText;
            txtTestLog.Location = new Point(1111, 39);
            txtTestLog.Margin = new Padding(0);
            txtTestLog.Name = "txtTestLog";
            txtTestLog.ReadOnly = true;
            txtTestLog.ScrollBars = RichTextBoxScrollBars.Vertical;
            txtTestLog.Size = new Size(327, 745);
            txtTestLog.TabIndex = 0;
            txtTestLog.Text = "";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(15, 709);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(975, 75);
            textBox1.TabIndex = 24;
            // 
            // Sendmessage
            // 
            Sendmessage.Location = new Point(996, 709);
            Sendmessage.Name = "Sendmessage";
            Sendmessage.Size = new Size(108, 75);
            Sendmessage.TabIndex = 25;
            Sendmessage.Text = "发送";
            Sendmessage.UseVisualStyleBackColor = true;
            Sendmessage.Click += Sendmessage_Click;
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { 常温测试ToolStripMenuItem, 校准ToolStripMenuItem, 温箱控制ToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1443, 28);
            menuStrip1.TabIndex = 26;
            menuStrip1.Text = "menuStrip1";
            // 
            // 常温测试ToolStripMenuItem
            // 
            常温测试ToolStripMenuItem.Name = "常温测试ToolStripMenuItem";
            常温测试ToolStripMenuItem.Size = new Size(83, 24);
            常温测试ToolStripMenuItem.Text = "常温测试";
            常温测试ToolStripMenuItem.Click += 常温测试ToolStripMenuItem_Click;
            // 
            // 校准ToolStripMenuItem
            // 
            校准ToolStripMenuItem.Name = "校准ToolStripMenuItem";
            校准ToolStripMenuItem.Size = new Size(53, 24);
            校准ToolStripMenuItem.Text = "校准";
            校准ToolStripMenuItem.Click += 校准ToolStripMenuItem_Click;
            // 
            // 温箱控制ToolStripMenuItem
            // 
            温箱控制ToolStripMenuItem.Name = "温箱控制ToolStripMenuItem";
            温箱控制ToolStripMenuItem.Size = new Size(83, 24);
            温箱控制ToolStripMenuItem.Text = "温箱控制";
            温箱控制ToolStripMenuItem.Click += 温箱控制ToolStripMenuItem_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1443, 796);
            Controls.Add(gbChannels);
            Controls.Add(gbTestItems);
            Controls.Add(btnBatchTest);
            Controls.Add(menuStrip1);
            Controls.Add(btnStopTest);
            Controls.Add(btnClearResults);
            Controls.Add(btnExportReport);
            Controls.Add(Sendmessage);
            Controls.Add(textBox1);
            Controls.Add(dgvTestResults);
            Controls.Add(txtTestLog);
            Controls.Add(gbInstrumentStatus);
            Name = "Form1";
            Text = "常温测试";
            gbInstrumentStatus.ResumeLayout(false);
            gbInstrumentStatus.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbZNB8Status).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbSignalStatus).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbSpectrumStatus).EndInit();
            gbTestItems.ResumeLayout(false);
            gbChannels.ResumeLayout(false);
            gbChannels.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvTestResults).EndInit();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RadioButton cbChannel8;
        private RadioButton cbChannel7;
        private RadioButton cbChannel6;
    }
}
