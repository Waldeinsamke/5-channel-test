namespace TemperatureChamber
{
    partial class ChamberControlForm
    {
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// contents of this method with code editor.
        /// </summary>
        private void InitializeComponent()
        {
            menuStrip1 = new MenuStrip();
            常温测试ToolStripMenuItem = new ToolStripMenuItem();
            校准ToolStripMenuItem = new ToolStripMenuItem();
            gbChamberConnection = new GroupBox();
            btnChamberSettings = new Button();
            btnTestChamberConnection = new Button();
            btnDisconnectChamber = new Button();
            btnConnectChamber = new Button();
            lblChamberStatus = new Label();
            pbChamberStatus = new Panel();
            gbTemperatureControl = new GroupBox();
            btnSetTemperature = new Button();
            nudTargetTemperature = new NumericUpDown();
            label2 = new Label();
            gbChamberOperation = new GroupBox();
            btnStopChamber = new Button();
            btnStartChamber = new Button();
            btnRefreshStatus = new Button();
            gbChamberStatus = new GroupBox();
            lblFaultValue = new Label();
            lblFaultInfo = new Label();
            lblRunningStatus = new Label();
            pbRunningStatus = new Panel();
            label6 = new Label();
            lblTemperatureValue = new Label();
            label3 = new Label();
            txtTestLog = new TextBox();
            menuStrip1.SuspendLayout();
            gbChamberConnection.SuspendLayout();
            gbTemperatureControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudTargetTemperature).BeginInit();
            gbChamberOperation.SuspendLayout();
            gbChamberStatus.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { 常温测试ToolStripMenuItem, 校准ToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1028, 28);
            menuStrip1.TabIndex = 6;
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
            // gbChamberConnection
            // 
            gbChamberConnection.Controls.Add(btnChamberSettings);
            gbChamberConnection.Controls.Add(btnTestChamberConnection);
            gbChamberConnection.Controls.Add(btnDisconnectChamber);
            gbChamberConnection.Controls.Add(btnConnectChamber);
            gbChamberConnection.Controls.Add(lblChamberStatus);
            gbChamberConnection.Controls.Add(pbChamberStatus);
            gbChamberConnection.Location = new Point(18, 33);
            gbChamberConnection.Margin = new Padding(4, 5, 4, 5);
            gbChamberConnection.Name = "gbChamberConnection";
            gbChamberConnection.Padding = new Padding(4, 5, 4, 5);
            gbChamberConnection.Size = new Size(545, 173);
            gbChamberConnection.TabIndex = 0;
            gbChamberConnection.TabStop = false;
            gbChamberConnection.Text = "温箱连接";
            // 
            // btnChamberSettings
            // 
            btnChamberSettings.Location = new Point(350, 100);
            btnChamberSettings.Margin = new Padding(4, 5, 4, 5);
            btnChamberSettings.Name = "btnChamberSettings";
            btnChamberSettings.Size = new Size(165, 50);
            btnChamberSettings.TabIndex = 5;
            btnChamberSettings.Text = "参数配置";
            btnChamberSettings.UseVisualStyleBackColor = true;
            btnChamberSettings.Click += btnChamberSettings_Click;
            // 
            // btnTestChamberConnection
            // 
            btnTestChamberConnection.Location = new Point(162, 100);
            btnTestChamberConnection.Margin = new Padding(4, 5, 4, 5);
            btnTestChamberConnection.Name = "btnTestChamberConnection";
            btnTestChamberConnection.Size = new Size(165, 50);
            btnTestChamberConnection.TabIndex = 4;
            btnTestChamberConnection.Text = "测试连接";
            btnTestChamberConnection.UseVisualStyleBackColor = true;
            btnTestChamberConnection.Click += btnTestChamberConnection_Click;
            // 
            // btnDisconnectChamber
            // 
            btnDisconnectChamber.Location = new Point(350, 30);
            btnDisconnectChamber.Margin = new Padding(4, 5, 4, 5);
            btnDisconnectChamber.Name = "btnDisconnectChamber";
            btnDisconnectChamber.Size = new Size(165, 50);
            btnDisconnectChamber.TabIndex = 3;
            btnDisconnectChamber.Text = "断开设备";
            btnDisconnectChamber.UseVisualStyleBackColor = true;
            btnDisconnectChamber.Click += btnDisconnectChamber_Click;
            // 
            // btnConnectChamber
            // 
            btnConnectChamber.Location = new Point(162, 30);
            btnConnectChamber.Margin = new Padding(4, 5, 4, 5);
            btnConnectChamber.Name = "btnConnectChamber";
            btnConnectChamber.Size = new Size(165, 50);
            btnConnectChamber.TabIndex = 2;
            btnConnectChamber.Text = "连接设备";
            btnConnectChamber.UseVisualStyleBackColor = true;
            btnConnectChamber.Click += btnConnectChamber_Click;
            // 
            // lblChamberStatus
            // 
            lblChamberStatus.AutoSize = true;
            lblChamberStatus.Location = new Point(50, 100);
            lblChamberStatus.Margin = new Padding(4, 0, 4, 0);
            lblChamberStatus.Name = "lblChamberStatus";
            lblChamberStatus.Size = new Size(54, 20);
            lblChamberStatus.TabIndex = 1;
            lblChamberStatus.Text = "未连接";
            // 
            // pbChamberStatus
            // 
            pbChamberStatus.Location = new Point(63, 62);
            pbChamberStatus.Margin = new Padding(4, 5, 4, 5);
            pbChamberStatus.Name = "pbChamberStatus";
            pbChamberStatus.Size = new Size(30, 33);
            pbChamberStatus.TabIndex = 0;
            // 
            // gbTemperatureControl
            // 
            gbTemperatureControl.Controls.Add(btnSetTemperature);
            gbTemperatureControl.Controls.Add(nudTargetTemperature);
            gbTemperatureControl.Controls.Add(label2);
            gbTemperatureControl.Location = new Point(18, 362);
            gbTemperatureControl.Margin = new Padding(4, 5, 4, 5);
            gbTemperatureControl.Name = "gbTemperatureControl";
            gbTemperatureControl.Padding = new Padding(4, 5, 4, 5);
            gbTemperatureControl.Size = new Size(239, 150);
            gbTemperatureControl.TabIndex = 1;
            gbTemperatureControl.TabStop = false;
            gbTemperatureControl.Text = "温度控制";
            // 
            // btnSetTemperature
            // 
            btnSetTemperature.Location = new Point(15, 93);
            btnSetTemperature.Margin = new Padding(4, 5, 4, 5);
            btnSetTemperature.Name = "btnSetTemperature";
            btnSetTemperature.Size = new Size(211, 38);
            btnSetTemperature.TabIndex = 2;
            btnSetTemperature.Text = "设置温度";
            btnSetTemperature.UseVisualStyleBackColor = true;
            btnSetTemperature.Click += btnSetTemperature_Click;
            // 
            // nudTargetTemperature
            // 
            nudTargetTemperature.DecimalPlaces = 1;
            nudTargetTemperature.Location = new Point(105, 42);
            nudTargetTemperature.Margin = new Padding(4, 5, 4, 5);
            nudTargetTemperature.Maximum = new decimal(new int[] { 150, 0, 0, 0 });
            nudTargetTemperature.Minimum = new decimal(new int[] { 58, 0, 0, int.MinValue });
            nudTargetTemperature.Name = "nudTargetTemperature";
            nudTargetTemperature.Size = new Size(121, 27);
            nudTargetTemperature.TabIndex = 1;
            nudTargetTemperature.Value = new decimal(new int[] { 25, 0, 0, 0 });
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(15, 44);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(69, 20);
            label2.TabIndex = 0;
            label2.Text = "目标温度";
            // 
            // gbChamberOperation
            // 
            gbChamberOperation.Controls.Add(btnStopChamber);
            gbChamberOperation.Controls.Add(btnStartChamber);
            gbChamberOperation.Controls.Add(btnRefreshStatus);
            gbChamberOperation.Location = new Point(18, 522);
            gbChamberOperation.Margin = new Padding(4, 5, 4, 5);
            gbChamberOperation.Name = "gbChamberOperation";
            gbChamberOperation.Padding = new Padding(4, 5, 4, 5);
            gbChamberOperation.Size = new Size(545, 105);
            gbChamberOperation.TabIndex = 3;
            gbChamberOperation.TabStop = false;
            gbChamberOperation.Text = "温箱操作";
            // 
            // btnStopChamber
            // 
            btnStopChamber.Location = new Point(365, 34);
            btnStopChamber.Margin = new Padding(4, 5, 4, 5);
            btnStopChamber.Name = "btnStopChamber";
            btnStopChamber.Size = new Size(165, 45);
            btnStopChamber.TabIndex = 2;
            btnStopChamber.Text = "停止设备";
            btnStopChamber.UseVisualStyleBackColor = true;
            btnStopChamber.Click += btnStopChamber_Click;
            // 
            // btnStartChamber
            // 
            btnStartChamber.Location = new Point(188, 34);
            btnStartChamber.Margin = new Padding(4, 5, 4, 5);
            btnStartChamber.Name = "btnStartChamber";
            btnStartChamber.Size = new Size(165, 45);
            btnStartChamber.TabIndex = 1;
            btnStartChamber.Text = "启动设备";
            btnStartChamber.UseVisualStyleBackColor = true;
            btnStartChamber.Click += btnStartChamber_Click;
            // 
            // btnRefreshStatus
            // 
            btnRefreshStatus.Location = new Point(15, 34);
            btnRefreshStatus.Margin = new Padding(4, 5, 4, 5);
            btnRefreshStatus.Name = "btnRefreshStatus";
            btnRefreshStatus.Size = new Size(165, 45);
            btnRefreshStatus.TabIndex = 0;
            btnRefreshStatus.Text = "刷新状态";
            btnRefreshStatus.UseVisualStyleBackColor = true;
            btnRefreshStatus.Click += btnRefreshStatus_Click;
            // 
            // gbChamberStatus
            // 
            gbChamberStatus.Controls.Add(lblFaultValue);
            gbChamberStatus.Controls.Add(lblFaultInfo);
            gbChamberStatus.Controls.Add(lblRunningStatus);
            gbChamberStatus.Controls.Add(pbRunningStatus);
            gbChamberStatus.Controls.Add(label6);
            gbChamberStatus.Controls.Add(lblTemperatureValue);
            gbChamberStatus.Controls.Add(label3);
            gbChamberStatus.Location = new Point(18, 216);
            gbChamberStatus.Margin = new Padding(4, 5, 4, 5);
            gbChamberStatus.Name = "gbChamberStatus";
            gbChamberStatus.Padding = new Padding(4, 5, 4, 5);
            gbChamberStatus.Size = new Size(545, 136);
            gbChamberStatus.TabIndex = 4;
            gbChamberStatus.TabStop = false;
            gbChamberStatus.Text = "温箱状态";
            // 
            // lblFaultValue
            // 
            lblFaultValue.AutoSize = true;
            lblFaultValue.Location = new Point(365, 90);
            lblFaultValue.Margin = new Padding(4, 0, 4, 0);
            lblFaultValue.Name = "lblFaultValue";
            lblFaultValue.Size = new Size(54, 20);
            lblFaultValue.TabIndex = 9;
            lblFaultValue.Text = "无故障";
            // 
            // lblFaultInfo
            // 
            lblFaultInfo.AutoSize = true;
            lblFaultInfo.Location = new Point(230, 90);
            lblFaultInfo.Margin = new Padding(4, 0, 4, 0);
            lblFaultInfo.Name = "lblFaultInfo";
            lblFaultInfo.Size = new Size(69, 20);
            lblFaultInfo.TabIndex = 8;
            lblFaultInfo.Text = "故障信息";
            // 
            // lblRunningStatus
            // 
            lblRunningStatus.AutoSize = true;
            lblRunningStatus.Location = new Point(365, 48);
            lblRunningStatus.Margin = new Padding(4, 0, 4, 0);
            lblRunningStatus.Name = "lblRunningStatus";
            lblRunningStatus.Size = new Size(54, 20);
            lblRunningStatus.TabIndex = 6;
            lblRunningStatus.Text = "未运行";
            // 
            // pbRunningStatus
            // 
            pbRunningStatus.Location = new Point(320, 39);
            pbRunningStatus.Margin = new Padding(4, 5, 4, 5);
            pbRunningStatus.Name = "pbRunningStatus";
            pbRunningStatus.Size = new Size(30, 33);
            pbRunningStatus.TabIndex = 5;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(230, 48);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(69, 20);
            label6.TabIndex = 4;
            label6.Text = "运行状态";
            // 
            // lblTemperatureValue
            // 
            lblTemperatureValue.AutoSize = true;
            lblTemperatureValue.Location = new Point(140, 48);
            lblTemperatureValue.Margin = new Padding(4, 0, 4, 0);
            lblTemperatureValue.Name = "lblTemperatureValue";
            lblTemperatureValue.Size = new Size(36, 20);
            lblTemperatureValue.TabIndex = 1;
            lblTemperatureValue.Text = "--℃";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(50, 48);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(69, 20);
            label3.TabIndex = 0;
            label3.Text = "当前温度";
            // 
            // txtTestLog
            // 
            txtTestLog.Location = new Point(571, 33);
            txtTestLog.Margin = new Padding(4, 5, 4, 5);
            txtTestLog.Multiline = true;
            txtTestLog.Name = "txtTestLog";
            txtTestLog.ScrollBars = ScrollBars.Vertical;
            txtTestLog.Size = new Size(448, 594);
            txtTestLog.TabIndex = 5;
            // 
            // ChamberControlForm
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1028, 634);
            Controls.Add(menuStrip1);
            Controls.Add(txtTestLog);
            Controls.Add(gbChamberStatus);
            Controls.Add(gbTemperatureControl);
            Controls.Add(gbChamberOperation);
            Controls.Add(gbChamberConnection);
            Margin = new Padding(4, 5, 4, 5);
            Name = "ChamberControlForm";
            Text = "温箱控制";
            FormClosed += ChamberControlForm_FormClosed;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            gbChamberConnection.ResumeLayout(false);
            gbChamberConnection.PerformLayout();
            gbTemperatureControl.ResumeLayout(false);
            gbTemperatureControl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudTargetTemperature).EndInit();
            gbChamberOperation.ResumeLayout(false);
            gbChamberStatus.ResumeLayout(false);
            gbChamberStatus.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbChamberConnection;
        private System.Windows.Forms.Button btnChamberSettings;
        private System.Windows.Forms.Button btnTestChamberConnection;
        private System.Windows.Forms.Button btnDisconnectChamber;
        private System.Windows.Forms.Button btnConnectChamber;
        private System.Windows.Forms.Label lblChamberStatus;
        private System.Windows.Forms.Panel pbChamberStatus;
        private System.Windows.Forms.GroupBox gbTemperatureControl;
        private System.Windows.Forms.Button btnSetTemperature;
        private System.Windows.Forms.NumericUpDown nudTargetTemperature;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox gbChamberOperation;
        private System.Windows.Forms.Button btnStopChamber;
        private System.Windows.Forms.Button btnStartChamber;
        private System.Windows.Forms.Button btnRefreshStatus;
        private System.Windows.Forms.GroupBox gbChamberStatus;
        private System.Windows.Forms.Label lblFaultValue;
        private System.Windows.Forms.Label lblFaultInfo;
        private System.Windows.Forms.Label lblRunningStatus;
        private System.Windows.Forms.Panel pbRunningStatus;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblTemperatureValue;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtTestLog;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 常温测试ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 校准ToolStripMenuItem;
    }
}
