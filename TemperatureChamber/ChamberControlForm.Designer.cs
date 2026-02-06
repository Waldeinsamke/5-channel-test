namespace TemperatureChamber
{
    partial class ChamberControlForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// contents of this method with code editor.
        /// </summary>
        private void InitializeComponent()
        {
            gbChamberConnection = new GroupBox();
            btnSerialPortTest = new Button();
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
            gbHumidityControl = new GroupBox();
            btnSetHumidity = new Button();
            nudTargetHumidity = new NumericUpDown();
            label4 = new Label();
            gbChamberOperation = new GroupBox();
            btnStopChamber = new Button();
            btnStartChamber = new Button();
            btnRefreshStatus = new Button();
            gbChamberStatus = new GroupBox();
            txtChamberStatus = new TextBox();
            lblRunningStatus = new Label();
            pbRunningStatus = new Panel();
            label6 = new Label();
            lblHumidityValue = new Label();
            label5 = new Label();
            lblTemperatureValue = new Label();
            label3 = new Label();
            txtTestLog = new TextBox();
            gbChamberConnection.SuspendLayout();
            gbTemperatureControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudTargetTemperature).BeginInit();
            gbHumidityControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudTargetHumidity).BeginInit();
            gbChamberOperation.SuspendLayout();
            gbChamberStatus.SuspendLayout();
            SuspendLayout();
            // 
            // gbChamberConnection
            // 
            gbChamberConnection.Controls.Add(btnSerialPortTest);
            gbChamberConnection.Controls.Add(btnChamberSettings);
            gbChamberConnection.Controls.Add(btnTestChamberConnection);
            gbChamberConnection.Controls.Add(btnDisconnectChamber);
            gbChamberConnection.Controls.Add(btnConnectChamber);
            gbChamberConnection.Controls.Add(lblChamberStatus);
            gbChamberConnection.Controls.Add(pbChamberStatus);
            gbChamberConnection.Location = new Point(18, 20);
            gbChamberConnection.Margin = new Padding(4, 5, 4, 5);
            gbChamberConnection.Name = "gbChamberConnection";
            gbChamberConnection.Padding = new Padding(4, 5, 4, 5);
            gbChamberConnection.Size = new Size(600, 173);
            gbChamberConnection.TabIndex = 0;
            gbChamberConnection.TabStop = false;
            gbChamberConnection.Text = "温箱连接";
            // 
            // btnSerialPortTest
            // 
            btnSerialPortTest.Location = new Point(392, 105);
            btnSerialPortTest.Margin = new Padding(4, 5, 4, 5);
            btnSerialPortTest.Name = "btnSerialPortTest";
            btnSerialPortTest.Size = new Size(180, 50);
            btnSerialPortTest.TabIndex = 6;
            btnSerialPortTest.Text = "串口测试";
            btnSerialPortTest.UseVisualStyleBackColor = true;
            // 
            // btnChamberSettings
            // 
            btnChamberSettings.Location = new Point(204, 105);
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
            btnTestChamberConnection.Location = new Point(15, 105);
            btnTestChamberConnection.Margin = new Padding(4, 5, 4, 5);
            btnTestChamberConnection.Name = "btnTestChamberConnection";
            btnTestChamberConnection.Size = new Size(178, 50);
            btnTestChamberConnection.TabIndex = 4;
            btnTestChamberConnection.Text = "测试连接";
            btnTestChamberConnection.UseVisualStyleBackColor = true;
            btnTestChamberConnection.Click += btnTestChamberConnection_Click;
            // 
            // btnDisconnectChamber
            // 
            btnDisconnectChamber.Location = new Point(392, 33);
            btnDisconnectChamber.Margin = new Padding(4, 5, 4, 5);
            btnDisconnectChamber.Name = "btnDisconnectChamber";
            btnDisconnectChamber.Size = new Size(180, 50);
            btnDisconnectChamber.TabIndex = 3;
            btnDisconnectChamber.Text = "断开连接";
            btnDisconnectChamber.UseVisualStyleBackColor = true;
            btnDisconnectChamber.Click += btnDisconnectChamber_Click;
            // 
            // btnConnectChamber
            // 
            btnConnectChamber.Location = new Point(204, 33);
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
            lblChamberStatus.Location = new Point(101, 48);
            lblChamberStatus.Margin = new Padding(4, 0, 4, 0);
            lblChamberStatus.Name = "lblChamberStatus";
            lblChamberStatus.Size = new Size(54, 20);
            lblChamberStatus.TabIndex = 1;
            lblChamberStatus.Text = "未连接";
            // 
            // pbChamberStatus
            // 
            pbChamberStatus.Location = new Point(50, 40);
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
            gbTemperatureControl.Location = new Point(18, 203);
            gbTemperatureControl.Margin = new Padding(4, 5, 4, 5);
            gbTemperatureControl.Name = "gbTemperatureControl";
            gbTemperatureControl.Padding = new Padding(4, 5, 4, 5);
            gbTemperatureControl.Size = new Size(292, 150);
            gbTemperatureControl.TabIndex = 1;
            gbTemperatureControl.TabStop = false;
            gbTemperatureControl.Text = "温度控制";
            // 
            // btnSetTemperature
            // 
            btnSetTemperature.Location = new Point(15, 100);
            btnSetTemperature.Margin = new Padding(4, 5, 4, 5);
            btnSetTemperature.Name = "btnSetTemperature";
            btnSetTemperature.Size = new Size(255, 38);
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
            nudTargetTemperature.Minimum = new decimal(new int[] { 40, 0, 0, int.MinValue });
            nudTargetTemperature.Name = "nudTargetTemperature";
            nudTargetTemperature.Size = new Size(165, 27);
            nudTargetTemperature.TabIndex = 1;
            nudTargetTemperature.Value = new decimal(new int[] { 25, 0, 0, 0 });
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(15, 47);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(69, 20);
            label2.TabIndex = 0;
            label2.Text = "目标温度";
            // 
            // gbHumidityControl
            // 
            gbHumidityControl.Controls.Add(btnSetHumidity);
            gbHumidityControl.Controls.Add(nudTargetHumidity);
            gbHumidityControl.Controls.Add(label4);
            gbHumidityControl.Location = new Point(318, 203);
            gbHumidityControl.Margin = new Padding(4, 5, 4, 5);
            gbHumidityControl.Name = "gbHumidityControl";
            gbHumidityControl.Padding = new Padding(4, 5, 4, 5);
            gbHumidityControl.Size = new Size(300, 150);
            gbHumidityControl.TabIndex = 2;
            gbHumidityControl.TabStop = false;
            gbHumidityControl.Text = "湿度控制";
            // 
            // btnSetHumidity
            // 
            btnSetHumidity.Location = new Point(15, 100);
            btnSetHumidity.Margin = new Padding(4, 5, 4, 5);
            btnSetHumidity.Name = "btnSetHumidity";
            btnSetHumidity.Size = new Size(255, 38);
            btnSetHumidity.TabIndex = 2;
            btnSetHumidity.Text = "设置湿度";
            btnSetHumidity.UseVisualStyleBackColor = true;
            // 
            // nudTargetHumidity
            // 
            nudTargetHumidity.DecimalPlaces = 1;
            nudTargetHumidity.Location = new Point(105, 42);
            nudTargetHumidity.Margin = new Padding(4, 5, 4, 5);
            nudTargetHumidity.Maximum = new decimal(new int[] { 95, 0, 0, 0 });
            nudTargetHumidity.Minimum = new decimal(new int[] { 20, 0, 0, 0 });
            nudTargetHumidity.Name = "nudTargetHumidity";
            nudTargetHumidity.Size = new Size(165, 27);
            nudTargetHumidity.TabIndex = 1;
            nudTargetHumidity.Value = new decimal(new int[] { 50, 0, 0, 0 });
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(15, 47);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(69, 20);
            label4.TabIndex = 0;
            label4.Text = "目标湿度";
            // 
            // gbChamberOperation
            // 
            gbChamberOperation.Controls.Add(btnStopChamber);
            gbChamberOperation.Controls.Add(btnStartChamber);
            gbChamberOperation.Controls.Add(btnRefreshStatus);
            gbChamberOperation.Location = new Point(18, 363);
            gbChamberOperation.Margin = new Padding(4, 5, 4, 5);
            gbChamberOperation.Name = "gbChamberOperation";
            gbChamberOperation.Padding = new Padding(4, 5, 4, 5);
            gbChamberOperation.Size = new Size(600, 121);
            gbChamberOperation.TabIndex = 3;
            gbChamberOperation.TabStop = false;
            gbChamberOperation.Text = "温箱操作";
            // 
            // btnStopChamber
            // 
            btnStopChamber.Location = new Point(397, 34);
            btnStopChamber.Margin = new Padding(4, 5, 4, 5);
            btnStopChamber.Name = "btnStopChamber";
            btnStopChamber.Size = new Size(180, 67);
            btnStopChamber.TabIndex = 2;
            btnStopChamber.Text = "停止设备";
            btnStopChamber.UseVisualStyleBackColor = true;
            btnStopChamber.Click += btnStopChamber_Click;
            // 
            // btnStartChamber
            // 
            btnStartChamber.Location = new Point(202, 34);
            btnStartChamber.Margin = new Padding(4, 5, 4, 5);
            btnStartChamber.Name = "btnStartChamber";
            btnStartChamber.Size = new Size(180, 67);
            btnStartChamber.TabIndex = 1;
            btnStartChamber.Text = "启动设备";
            btnStartChamber.UseVisualStyleBackColor = true;
            btnStartChamber.Click += btnStartChamber_Click;
            // 
            // btnRefreshStatus
            // 
            btnRefreshStatus.Location = new Point(22, 34);
            btnRefreshStatus.Margin = new Padding(4, 5, 4, 5);
            btnRefreshStatus.Name = "btnRefreshStatus";
            btnRefreshStatus.Size = new Size(165, 67);
            btnRefreshStatus.TabIndex = 0;
            btnRefreshStatus.Text = "刷新状态";
            btnRefreshStatus.UseVisualStyleBackColor = true;
            btnRefreshStatus.Click += btnRefreshStatus_Click;
            // 
            // gbChamberStatus
            // 
            gbChamberStatus.Controls.Add(txtChamberStatus);
            gbChamberStatus.Controls.Add(lblRunningStatus);
            gbChamberStatus.Controls.Add(pbRunningStatus);
            gbChamberStatus.Controls.Add(label6);
            gbChamberStatus.Controls.Add(lblHumidityValue);
            gbChamberStatus.Controls.Add(label5);
            gbChamberStatus.Controls.Add(lblTemperatureValue);
            gbChamberStatus.Controls.Add(label3);
            gbChamberStatus.Location = new Point(18, 494);
            gbChamberStatus.Margin = new Padding(4, 5, 4, 5);
            gbChamberStatus.Name = "gbChamberStatus";
            gbChamberStatus.Padding = new Padding(4, 5, 4, 5);
            gbChamberStatus.Size = new Size(600, 272);
            gbChamberStatus.TabIndex = 4;
            gbChamberStatus.TabStop = false;
            gbChamberStatus.Text = "温箱状态";
            // 
            // txtChamberStatus
            // 
            txtChamberStatus.Location = new Point(19, 99);
            txtChamberStatus.Margin = new Padding(4, 5, 4, 5);
            txtChamberStatus.Multiline = true;
            txtChamberStatus.Name = "txtChamberStatus";
            txtChamberStatus.ScrollBars = ScrollBars.Vertical;
            txtChamberStatus.Size = new Size(553, 164);
            txtChamberStatus.TabIndex = 7;
            // 
            // lblRunningStatus
            // 
            lblRunningStatus.AutoSize = true;
            lblRunningStatus.Location = new Point(403, 33);
            lblRunningStatus.Margin = new Padding(4, 0, 4, 0);
            lblRunningStatus.Name = "lblRunningStatus";
            lblRunningStatus.Size = new Size(54, 20);
            lblRunningStatus.TabIndex = 6;
            lblRunningStatus.Text = "未运行";
            // 
            // pbRunningStatus
            // 
            pbRunningStatus.Location = new Point(358, 24);
            pbRunningStatus.Margin = new Padding(4, 5, 4, 5);
            pbRunningStatus.Name = "pbRunningStatus";
            pbRunningStatus.Size = new Size(30, 33);
            pbRunningStatus.TabIndex = 5;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(268, 33);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(69, 20);
            label6.TabIndex = 4;
            label6.Text = "运行状态";
            // 
            // lblHumidityValue
            // 
            lblHumidityValue.AutoSize = true;
            lblHumidityValue.Location = new Point(178, 74);
            lblHumidityValue.Margin = new Padding(4, 0, 4, 0);
            lblHumidityValue.Name = "lblHumidityValue";
            lblHumidityValue.Size = new Size(56, 20);
            lblHumidityValue.TabIndex = 3;
            lblHumidityValue.Text = "--%RH";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(88, 74);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(69, 20);
            label5.TabIndex = 2;
            label5.Text = "当前湿度";
            // 
            // lblTemperatureValue
            // 
            lblTemperatureValue.AutoSize = true;
            lblTemperatureValue.Location = new Point(178, 33);
            lblTemperatureValue.Margin = new Padding(4, 0, 4, 0);
            lblTemperatureValue.Name = "lblTemperatureValue";
            lblTemperatureValue.Size = new Size(36, 20);
            lblTemperatureValue.TabIndex = 1;
            lblTemperatureValue.Text = "--℃";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(88, 33);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(69, 20);
            label3.TabIndex = 0;
            label3.Text = "当前温度";
            // 
            // txtTestLog
            // 
            txtTestLog.Location = new Point(630, 20);
            txtTestLog.Margin = new Padding(4, 5, 4, 5);
            txtTestLog.Multiline = true;
            txtTestLog.Name = "txtTestLog";
            txtTestLog.ScrollBars = ScrollBars.Vertical;
            txtTestLog.Size = new Size(598, 746);
            txtTestLog.TabIndex = 5;
            // 
            // ChamberControlForm
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1248, 773);
            Controls.Add(txtTestLog);
            Controls.Add(gbChamberStatus);
            Controls.Add(gbChamberOperation);
            Controls.Add(gbHumidityControl);
            Controls.Add(gbTemperatureControl);
            Controls.Add(gbChamberConnection);
            Margin = new Padding(4, 5, 4, 5);
            Name = "ChamberControlForm";
            Text = "温箱控制";
            FormClosed += ChamberControlForm_FormClosed;
            gbChamberConnection.ResumeLayout(false);
            gbChamberConnection.PerformLayout();
            gbTemperatureControl.ResumeLayout(false);
            gbTemperatureControl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudTargetTemperature).EndInit();
            gbHumidityControl.ResumeLayout(false);
            gbHumidityControl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudTargetHumidity).EndInit();
            gbChamberOperation.ResumeLayout(false);
            gbChamberStatus.ResumeLayout(false);
            gbChamberStatus.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbChamberConnection;
        private System.Windows.Forms.Button btnSerialPortTest;
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
        private System.Windows.Forms.GroupBox gbHumidityControl;
        private System.Windows.Forms.Button btnSetHumidity;
        private System.Windows.Forms.NumericUpDown nudTargetHumidity;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox gbChamberOperation;
        private System.Windows.Forms.Button btnStopChamber;
        private System.Windows.Forms.Button btnStartChamber;
        private System.Windows.Forms.Button btnRefreshStatus;
        private System.Windows.Forms.GroupBox gbChamberStatus;
        private System.Windows.Forms.TextBox txtChamberStatus;
        private System.Windows.Forms.Label lblRunningStatus;
        private System.Windows.Forms.Panel pbRunningStatus;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblHumidityValue;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblTemperatureValue;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtTestLog;
    }
}
