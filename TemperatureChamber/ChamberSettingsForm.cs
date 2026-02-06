using System;
using System.IO.Ports;
using System.Windows.Forms;
using TemperatureChamber.Models;

namespace TemperatureChamber
{
    public partial class ChamberSettingsForm : Form
    {
        public ChamberConfig ChamberConfig { get; private set; }

        public ChamberSettingsForm(ChamberConfig config)
        {
            InitializeComponent();
            ChamberConfig = config;
            LoadConfig();
        }

        private void LoadConfig()
        {
            txtPortName.Text = ChamberConfig.PortName;
            numSlaveId.Value = ChamberConfig.SlaveId;
            numBaudRate.Value = ChamberConfig.BaudRate;
            cmbParity.SelectedItem = ChamberConfig.Parity.ToString();
            numDataBits.Value = ChamberConfig.DataBits;
            cmbStopBits.SelectedItem = ChamberConfig.StopBits.ToString();
            numTimeout.Value = ChamberConfig.Timeout;
        }

        private void SaveConfig()
        {
            ChamberConfig.PortName = txtPortName.Text;
            ChamberConfig.SlaveId = (byte)numSlaveId.Value;
            ChamberConfig.BaudRate = (int)numBaudRate.Value;
            ChamberConfig.Parity = Enum.Parse<Parity>(cmbParity.SelectedItem.ToString());
            ChamberConfig.DataBits = (int)numDataBits.Value;
            ChamberConfig.StopBits = Enum.Parse<StopBits>(cmbStopBits.SelectedItem.ToString());
            ChamberConfig.Timeout = (int)numTimeout.Value;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SaveConfig();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void InitializeComponent()
        {
            txtPortName = new TextBox();
            numSlaveId = new NumericUpDown();
            numBaudRate = new NumericUpDown();
            cmbParity = new ComboBox();
            numDataBits = new NumericUpDown();
            cmbStopBits = new ComboBox();
            numTimeout = new NumericUpDown();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            btnOK = new Button();
            btnCancel = new Button();
            ((System.ComponentModel.ISupportInitialize)numSlaveId).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numBaudRate).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numDataBits).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numTimeout).BeginInit();
            SuspendLayout();
            // 
            // txtPortName
            // 
            txtPortName.Location = new Point(134, 20);
            txtPortName.Name = "txtPortName";
            txtPortName.Size = new Size(150, 27);
            txtPortName.TabIndex = 0;
            // 
            // numSlaveId
            // 
            numSlaveId.Location = new Point(134, 50);
            numSlaveId.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            numSlaveId.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numSlaveId.Name = "numSlaveId";
            numSlaveId.Size = new Size(150, 27);
            numSlaveId.TabIndex = 1;
            numSlaveId.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // numBaudRate
            // 
            numBaudRate.Location = new Point(134, 80);
            numBaudRate.Maximum = new decimal(new int[] { 115200, 0, 0, 0 });
            numBaudRate.Name = "numBaudRate";
            numBaudRate.Size = new Size(150, 27);
            numBaudRate.TabIndex = 2;
            // 
            // cmbParity
            // 
            cmbParity.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbParity.FormattingEnabled = true;
            cmbParity.Items.AddRange(new object[] { "None", "Odd", "Even", "Mark", "Space" });
            cmbParity.Location = new Point(134, 110);
            cmbParity.Name = "cmbParity";
            cmbParity.Size = new Size(150, 28);
            cmbParity.TabIndex = 3;
            // 
            // numDataBits
            // 
            numDataBits.Location = new Point(134, 140);
            numDataBits.Maximum = new decimal(new int[] { 8, 0, 0, 0 });
            numDataBits.Minimum = new decimal(new int[] { 5, 0, 0, 0 });
            numDataBits.Name = "numDataBits";
            numDataBits.Size = new Size(150, 27);
            numDataBits.TabIndex = 4;
            numDataBits.Value = new decimal(new int[] { 8, 0, 0, 0 });
            // 
            // cmbStopBits
            // 
            cmbStopBits.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStopBits.FormattingEnabled = true;
            cmbStopBits.Items.AddRange(new object[] { "None", "One", "Two", "OnePointFive" });
            cmbStopBits.Location = new Point(134, 170);
            cmbStopBits.Name = "cmbStopBits";
            cmbStopBits.Size = new Size(150, 28);
            cmbStopBits.TabIndex = 5;
            // 
            // numTimeout
            // 
            numTimeout.Location = new Point(134, 200);
            numTimeout.Maximum = new decimal(new int[] { 30000, 0, 0, 0 });
            numTimeout.Name = "numTimeout";
            numTimeout.Size = new Size(150, 27);
            numTimeout.TabIndex = 6;
            numTimeout.Value = new decimal(new int[] { 3000, 0, 0, 0 });
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(20, 23);
            label1.Name = "label1";
            label1.Size = new Size(73, 20);
            label1.TabIndex = 7;
            label1.Text = "串口名称:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(20, 53);
            label2.Name = "label2";
            label2.Size = new Size(73, 20);
            label2.TabIndex = 8;
            label2.Text = "从站地址:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(20, 83);
            label3.Name = "label3";
            label3.Size = new Size(58, 20);
            label3.TabIndex = 9;
            label3.Text = "波特率:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(20, 113);
            label4.Name = "label4";
            label4.Size = new Size(58, 20);
            label4.TabIndex = 10;
            label4.Text = "校验位:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(20, 143);
            label5.Name = "label5";
            label5.Size = new Size(58, 20);
            label5.TabIndex = 11;
            label5.Text = "数据位:";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(20, 173);
            label6.Name = "label6";
            label6.Size = new Size(58, 20);
            label6.TabIndex = 12;
            label6.Text = "停止位:";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(20, 203);
            label7.Name = "label7";
            label7.Size = new Size(104, 20);
            label7.TabIndex = 13;
            label7.Text = "超时时间(ms):";
            // 
            // btnOK
            // 
            btnOK.DialogResult = DialogResult.OK;
            btnOK.Location = new Point(52, 260);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(90, 30);
            btnOK.TabIndex = 14;
            btnOK.Text = "确定";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(162, 260);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(90, 30);
            btnCancel.TabIndex = 15;
            btnCancel.Text = "取消";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // ChamberSettingsForm
            // 
            AcceptButton = btnOK;
            CancelButton = btnCancel;
            ClientSize = new Size(321, 334);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(numTimeout);
            Controls.Add(cmbStopBits);
            Controls.Add(numDataBits);
            Controls.Add(cmbParity);
            Controls.Add(numBaudRate);
            Controls.Add(numSlaveId);
            Controls.Add(txtPortName);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ChamberSettingsForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "温箱参数配置";
            ((System.ComponentModel.ISupportInitialize)numSlaveId).EndInit();
            ((System.ComponentModel.ISupportInitialize)numBaudRate).EndInit();
            ((System.ComponentModel.ISupportInitialize)numDataBits).EndInit();
            ((System.ComponentModel.ISupportInitialize)numTimeout).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.TextBox txtPortName;
        private System.Windows.Forms.NumericUpDown numSlaveId;
        private System.Windows.Forms.NumericUpDown numBaudRate;
        private System.Windows.Forms.ComboBox cmbParity;
        private System.Windows.Forms.NumericUpDown numDataBits;
        private System.Windows.Forms.ComboBox cmbStopBits;
        private System.Windows.Forms.NumericUpDown numTimeout;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}
