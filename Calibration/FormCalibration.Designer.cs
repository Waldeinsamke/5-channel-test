namespace 五通道自动测试.Calibration
{
    partial class FormCalibration
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            gbChannels = new GroupBox();
            jzchannel8 = new RadioButton();
            jzchannel7 = new RadioButton();
            jzchannel6 = new RadioButton();
            jzchannel5 = new RadioButton();
            jzchannel4 = new RadioButton();
            jzchannel3 = new RadioButton();
            jzchannel2 = new RadioButton();
            jzchannel1 = new RadioButton();
            gbParameters = new GroupBox();
            write4 = new Button();
            read4 = new Button();
            write3 = new Button();
            read3 = new Button();
            write2 = new Button();
            read2 = new Button();
            lblDacLow = new Label();
            btnDownXndLow = new Button();
            lblXndHigh = new Label();
            btnUpXndLow = new Button();
            lblXndLow = new Label();
            lblDacHigh = new Label();
            btnUpXndHigh = new Button();
            lblAddrDacLow = new Label();
            txtCalDacHigh = new TextBox();
            lblAddrXndHigh = new Label();
            lblAddrXndLow = new Label();
            btnDownDacLow = new Button();
            lblAddrDacHigh = new Label();
            btnDownXndHigh = new Button();
            btnUpDacLow = new Button();
            btnUpDacHigh = new Button();
            txtCalDacLow = new TextBox();
            txtCalXndHigh = new TextBox();
            write1 = new Button();
            txtCalXndLow = new TextBox();
            btnDownDacHigh = new Button();
            read1 = new Button();
            menuStrip1 = new MenuStrip();
            常温测试ToolStripMenuItem = new ToolStripMenuItem();
            FreqItems = new GroupBox();
            button3390 = new RadioButton();
            button3330 = new RadioButton();
            button3370 = new RadioButton();
            button3350 = new RadioButton();
            labeltemp = new Label();
            comboBoxtemp = new ComboBox();
            checkBoxAuto = new CheckBox();
            jztextlog = new RichTextBox();
            dgvCalibrationResults = new DataGridView();
            TempSerialPort = new ComboBox();
            openTempSerialPort = new Button();
            AllRead = new Button();
            startPhase = new Button();
            button2 = new Button();
            ultraWork = new Button();
            ultraWork2 = new Button();
            normal = new RadioButton();
            antenna = new RadioButton();
            modechance = new ComboBox();
            SwitchPower = new Button();
            gbChannels.SuspendLayout();
            gbParameters.SuspendLayout();
            menuStrip1.SuspendLayout();
            FreqItems.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvCalibrationResults).BeginInit();
            SuspendLayout();
            // 
            // gbChannels
            // 
            gbChannels.Controls.Add(jzchannel8);
            gbChannels.Controls.Add(jzchannel7);
            gbChannels.Controls.Add(jzchannel6);
            gbChannels.Controls.Add(jzchannel5);
            gbChannels.Controls.Add(jzchannel4);
            gbChannels.Controls.Add(jzchannel3);
            gbChannels.Controls.Add(jzchannel2);
            gbChannels.Controls.Add(jzchannel1);
            gbChannels.Location = new Point(5, 382);
            gbChannels.Margin = new Padding(4);
            gbChannels.Name = "gbChannels";
            gbChannels.Padding = new Padding(4);
            gbChannels.Size = new Size(179, 190);
            gbChannels.TabIndex = 2;
            gbChannels.TabStop = false;
            gbChannels.Text = "通道选择";
            // 
            // jzchannel8
            // 
            jzchannel8.AutoSize = true;
            jzchannel8.Location = new Point(93, 133);
            jzchannel8.Margin = new Padding(4);
            jzchannel8.Name = "jzchannel8";
            jzchannel8.Size = new Size(72, 24);
            jzchannel8.TabIndex = 7;
            jzchannel8.Text = "通道H";
            jzchannel8.UseVisualStyleBackColor = true;
            // 
            // jzchannel7
            // 
            jzchannel7.AutoSize = true;
            jzchannel7.Location = new Point(93, 104);
            jzchannel7.Margin = new Padding(4);
            jzchannel7.Name = "jzchannel7";
            jzchannel7.Size = new Size(71, 24);
            jzchannel7.TabIndex = 6;
            jzchannel7.Text = "通道G";
            jzchannel7.UseVisualStyleBackColor = true;
            // 
            // jzchannel6
            // 
            jzchannel6.AutoSize = true;
            jzchannel6.Location = new Point(93, 75);
            jzchannel6.Margin = new Padding(4);
            jzchannel6.Name = "jzchannel6";
            jzchannel6.Size = new Size(68, 24);
            jzchannel6.TabIndex = 5;
            jzchannel6.Text = "通道F";
            jzchannel6.UseVisualStyleBackColor = true;
            // 
            // jzchannel5
            // 
            jzchannel5.AutoSize = true;
            jzchannel5.Location = new Point(93, 46);
            jzchannel5.Margin = new Padding(4);
            jzchannel5.Name = "jzchannel5";
            jzchannel5.Size = new Size(68, 24);
            jzchannel5.TabIndex = 4;
            jzchannel5.Text = "通道E";
            jzchannel5.UseVisualStyleBackColor = true;
            // 
            // jzchannel4
            // 
            jzchannel4.AutoSize = true;
            jzchannel4.Location = new Point(20, 133);
            jzchannel4.Margin = new Padding(4);
            jzchannel4.Name = "jzchannel4";
            jzchannel4.Size = new Size(71, 24);
            jzchannel4.TabIndex = 3;
            jzchannel4.Text = "通道D";
            jzchannel4.UseVisualStyleBackColor = true;
            // 
            // jzchannel3
            // 
            jzchannel3.AutoSize = true;
            jzchannel3.Location = new Point(20, 104);
            jzchannel3.Margin = new Padding(4);
            jzchannel3.Name = "jzchannel3";
            jzchannel3.Size = new Size(70, 24);
            jzchannel3.TabIndex = 2;
            jzchannel3.Text = "通道C";
            jzchannel3.UseVisualStyleBackColor = true;
            // 
            // jzchannel2
            // 
            jzchannel2.AutoSize = true;
            jzchannel2.Location = new Point(20, 75);
            jzchannel2.Margin = new Padding(4);
            jzchannel2.Name = "jzchannel2";
            jzchannel2.Size = new Size(69, 24);
            jzchannel2.TabIndex = 1;
            jzchannel2.Text = "通道B";
            jzchannel2.UseVisualStyleBackColor = true;
            // 
            // jzchannel1
            // 
            jzchannel1.AutoSize = true;
            jzchannel1.Checked = true;
            jzchannel1.Location = new Point(20, 46);
            jzchannel1.Margin = new Padding(4);
            jzchannel1.Name = "jzchannel1";
            jzchannel1.Size = new Size(71, 24);
            jzchannel1.TabIndex = 0;
            jzchannel1.TabStop = true;
            jzchannel1.Text = "通道A";
            jzchannel1.UseVisualStyleBackColor = true;
            // 
            // gbParameters
            // 
            gbParameters.Controls.Add(write4);
            gbParameters.Controls.Add(read4);
            gbParameters.Controls.Add(write3);
            gbParameters.Controls.Add(read3);
            gbParameters.Controls.Add(write2);
            gbParameters.Controls.Add(read2);
            gbParameters.Controls.Add(lblDacLow);
            gbParameters.Controls.Add(btnDownXndLow);
            gbParameters.Controls.Add(lblXndHigh);
            gbParameters.Controls.Add(btnUpXndLow);
            gbParameters.Controls.Add(lblXndLow);
            gbParameters.Controls.Add(lblDacHigh);
            gbParameters.Controls.Add(btnUpXndHigh);
            gbParameters.Controls.Add(lblAddrDacLow);
            gbParameters.Controls.Add(txtCalDacHigh);
            gbParameters.Controls.Add(lblAddrXndHigh);
            gbParameters.Controls.Add(lblAddrXndLow);
            gbParameters.Controls.Add(btnDownDacLow);
            gbParameters.Controls.Add(lblAddrDacHigh);
            gbParameters.Controls.Add(btnDownXndHigh);
            gbParameters.Controls.Add(btnUpDacLow);
            gbParameters.Controls.Add(btnUpDacHigh);
            gbParameters.Controls.Add(txtCalDacLow);
            gbParameters.Controls.Add(txtCalXndHigh);
            gbParameters.Controls.Add(write1);
            gbParameters.Controls.Add(txtCalXndLow);
            gbParameters.Controls.Add(btnDownDacHigh);
            gbParameters.Controls.Add(read1);
            gbParameters.Location = new Point(192, 73);
            gbParameters.Margin = new Padding(4);
            gbParameters.Name = "gbParameters";
            gbParameters.Padding = new Padding(4);
            gbParameters.Size = new Size(656, 499);
            gbParameters.TabIndex = 3;
            gbParameters.TabStop = false;
            gbParameters.Text = "校准参数";
            // 
            // write4
            // 
            write4.Location = new Point(439, 253);
            write4.Name = "write4";
            write4.Size = new Size(30, 30);
            write4.TabIndex = 25;
            write4.Text = "写";
            write4.UseVisualStyleBackColor = true;
            // 
            // read4
            // 
            read4.Location = new Point(473, 253);
            read4.Name = "read4";
            read4.Size = new Size(30, 30);
            read4.TabIndex = 24;
            read4.Text = "读";
            read4.UseVisualStyleBackColor = true;
            read4.Click += read4_Click;
            // 
            // write3
            // 
            write3.Location = new Point(439, 182);
            write3.Name = "write3";
            write3.Size = new Size(30, 30);
            write3.TabIndex = 23;
            write3.Text = "写";
            write3.UseVisualStyleBackColor = true;
            // 
            // read3
            // 
            read3.Location = new Point(473, 182);
            read3.Name = "read3";
            read3.Size = new Size(30, 30);
            read3.TabIndex = 22;
            read3.Text = "读";
            read3.UseVisualStyleBackColor = true;
            read3.Click += read3_Click;
            // 
            // write2
            // 
            write2.Location = new Point(439, 113);
            write2.Name = "write2";
            write2.Size = new Size(30, 30);
            write2.TabIndex = 21;
            write2.Text = "写";
            write2.UseVisualStyleBackColor = true;
            // 
            // read2
            // 
            read2.Location = new Point(473, 113);
            read2.Name = "read2";
            read2.Size = new Size(30, 30);
            read2.TabIndex = 20;
            read2.Text = "读";
            read2.UseVisualStyleBackColor = true;
            read2.Click += read2_Click;
            // 
            // lblDacLow
            // 
            lblDacLow.Anchor = AnchorStyles.Left;
            lblDacLow.AutoSize = true;
            lblDacLow.Location = new Point(138, 124);
            lblDacLow.Name = "lblDacLow";
            lblDacLow.Size = new Size(67, 20);
            lblDacLow.TabIndex = 1;
            lblDacLow.Text = "DAClow";
            // 
            // btnDownXndLow
            // 
            btnDownXndLow.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            btnDownXndLow.Location = new Point(409, 269);
            btnDownXndLow.Name = "btnDownXndLow";
            btnDownXndLow.Size = new Size(24, 24);
            btnDownXndLow.TabIndex = 19;
            btnDownXndLow.Text = "↓";
            btnDownXndLow.UseVisualStyleBackColor = true;
            btnDownXndLow.Click += btnDownXndLow_Click;
            // 
            // lblXndHigh
            // 
            lblXndHigh.Anchor = AnchorStyles.Left;
            lblXndHigh.AutoSize = true;
            lblXndHigh.Location = new Point(120, 193);
            lblXndHigh.Name = "lblXndHigh";
            lblXndHigh.Size = new Size(110, 20);
            lblXndHigh.TabIndex = 2;
            lblXndHigh.Text = "XND2261high";
            // 
            // btnUpXndLow
            // 
            btnUpXndLow.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            btnUpXndLow.Location = new Point(409, 245);
            btnUpXndLow.Name = "btnUpXndLow";
            btnUpXndLow.Size = new Size(24, 24);
            btnUpXndLow.TabIndex = 18;
            btnUpXndLow.Text = "↑";
            btnUpXndLow.UseVisualStyleBackColor = true;
            btnUpXndLow.Click += btnUpXndLow_Click;
            // 
            // lblXndLow
            // 
            lblXndLow.Anchor = AnchorStyles.Left;
            lblXndLow.AutoSize = true;
            lblXndLow.Location = new Point(120, 264);
            lblXndLow.Name = "lblXndLow";
            lblXndLow.Size = new Size(104, 20);
            lblXndLow.TabIndex = 3;
            lblXndLow.Text = "XND2261low";
            // 
            // lblDacHigh
            // 
            lblDacHigh.Anchor = AnchorStyles.Left;
            lblDacHigh.AutoSize = true;
            lblDacHigh.Location = new Point(138, 60);
            lblDacHigh.Name = "lblDacHigh";
            lblDacHigh.Size = new Size(73, 20);
            lblDacHigh.TabIndex = 0;
            lblDacHigh.Text = "DAChigh";
            // 
            // btnUpXndHigh
            // 
            btnUpXndHigh.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            btnUpXndHigh.Location = new Point(410, 173);
            btnUpXndHigh.Name = "btnUpXndHigh";
            btnUpXndHigh.Size = new Size(24, 24);
            btnUpXndHigh.TabIndex = 16;
            btnUpXndHigh.Text = "↑";
            btnUpXndHigh.UseVisualStyleBackColor = true;
            btnUpXndHigh.Click += btnUpXndHigh_Click;
            // 
            // lblAddrDacLow
            // 
            lblAddrDacLow.Anchor = AnchorStyles.Left;
            lblAddrDacLow.AutoSize = true;
            lblAddrDacLow.Location = new Point(243, 124);
            lblAddrDacLow.Name = "lblAddrDacLow";
            lblAddrDacLow.Size = new Size(62, 20);
            lblAddrDacLow.TabIndex = 5;
            lblAddrDacLow.Text = "0x0000";
            // 
            // txtCalDacHigh
            // 
            txtCalDacHigh.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtCalDacHigh.Location = new Point(318, 53);
            txtCalDacHigh.Name = "txtCalDacHigh";
            txtCalDacHigh.Size = new Size(88, 27);
            txtCalDacHigh.TabIndex = 8;
            // 
            // lblAddrXndHigh
            // 
            lblAddrXndHigh.Anchor = AnchorStyles.Left;
            lblAddrXndHigh.AutoSize = true;
            lblAddrXndHigh.Location = new Point(243, 193);
            lblAddrXndHigh.Name = "lblAddrXndHigh";
            lblAddrXndHigh.Size = new Size(62, 20);
            lblAddrXndHigh.TabIndex = 6;
            lblAddrXndHigh.Text = "0x0000";
            // 
            // lblAddrXndLow
            // 
            lblAddrXndLow.Anchor = AnchorStyles.Left;
            lblAddrXndLow.AutoSize = true;
            lblAddrXndLow.Location = new Point(243, 264);
            lblAddrXndLow.Name = "lblAddrXndLow";
            lblAddrXndLow.Size = new Size(62, 20);
            lblAddrXndLow.TabIndex = 7;
            lblAddrXndLow.Text = "0x0000";
            // 
            // btnDownDacLow
            // 
            btnDownDacLow.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            btnDownDacLow.Location = new Point(409, 130);
            btnDownDacLow.Name = "btnDownDacLow";
            btnDownDacLow.Size = new Size(24, 24);
            btnDownDacLow.TabIndex = 15;
            btnDownDacLow.Text = "↓";
            btnDownDacLow.UseVisualStyleBackColor = true;
            btnDownDacLow.Click += btnDownDacLow_Click;
            // 
            // lblAddrDacHigh
            // 
            lblAddrDacHigh.Anchor = AnchorStyles.Left;
            lblAddrDacHigh.AutoSize = true;
            lblAddrDacHigh.Location = new Point(243, 60);
            lblAddrDacHigh.Name = "lblAddrDacHigh";
            lblAddrDacHigh.Size = new Size(62, 20);
            lblAddrDacHigh.TabIndex = 4;
            lblAddrDacHigh.Text = "0x0000";
            // 
            // btnDownXndHigh
            // 
            btnDownXndHigh.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            btnDownXndHigh.Location = new Point(410, 197);
            btnDownXndHigh.Name = "btnDownXndHigh";
            btnDownXndHigh.Size = new Size(24, 24);
            btnDownXndHigh.TabIndex = 17;
            btnDownXndHigh.Text = "↓";
            btnDownXndHigh.UseVisualStyleBackColor = true;
            btnDownXndHigh.Click += btnDownXndHigh_Click;
            // 
            // btnUpDacLow
            // 
            btnUpDacLow.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            btnUpDacLow.Location = new Point(409, 106);
            btnUpDacLow.Name = "btnUpDacLow";
            btnUpDacLow.Size = new Size(24, 24);
            btnUpDacLow.TabIndex = 14;
            btnUpDacLow.Text = "↑";
            btnUpDacLow.UseVisualStyleBackColor = true;
            btnUpDacLow.Click += btnUpDacLow_Click;
            // 
            // btnUpDacHigh
            // 
            btnUpDacHigh.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            btnUpDacHigh.Location = new Point(409, 39);
            btnUpDacHigh.Name = "btnUpDacHigh";
            btnUpDacHigh.Size = new Size(24, 24);
            btnUpDacHigh.TabIndex = 12;
            btnUpDacHigh.Text = "↑";
            btnUpDacHigh.UseVisualStyleBackColor = true;
            btnUpDacHigh.Click += btnUpDacHigh_Click;
            // 
            // txtCalDacLow
            // 
            txtCalDacLow.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtCalDacLow.Location = new Point(318, 121);
            txtCalDacLow.Name = "txtCalDacLow";
            txtCalDacLow.Size = new Size(88, 27);
            txtCalDacLow.TabIndex = 9;
            // 
            // txtCalXndHigh
            // 
            txtCalXndHigh.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtCalXndHigh.Location = new Point(318, 190);
            txtCalXndHigh.Name = "txtCalXndHigh";
            txtCalXndHigh.Size = new Size(88, 27);
            txtCalXndHigh.TabIndex = 10;
            // 
            // write1
            // 
            write1.Location = new Point(439, 45);
            write1.Name = "write1";
            write1.Size = new Size(30, 30);
            write1.TabIndex = 2;
            write1.Text = "写";
            write1.UseVisualStyleBackColor = true;
            // 
            // txtCalXndLow
            // 
            txtCalXndLow.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtCalXndLow.Location = new Point(318, 261);
            txtCalXndLow.Name = "txtCalXndLow";
            txtCalXndLow.Size = new Size(88, 27);
            txtCalXndLow.TabIndex = 11;
            // 
            // btnDownDacHigh
            // 
            btnDownDacHigh.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            btnDownDacHigh.Location = new Point(409, 63);
            btnDownDacHigh.Name = "btnDownDacHigh";
            btnDownDacHigh.Size = new Size(24, 24);
            btnDownDacHigh.TabIndex = 13;
            btnDownDacHigh.Text = "↓";
            btnDownDacHigh.UseVisualStyleBackColor = true;
            btnDownDacHigh.Click += btnDownDacHigh_Click;
            // 
            // read1
            // 
            read1.Location = new Point(473, 45);
            read1.Name = "read1";
            read1.Size = new Size(30, 30);
            read1.TabIndex = 1;
            read1.Text = "读";
            read1.UseVisualStyleBackColor = true;
            read1.Click += read1_Click;
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { 常温测试ToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1157, 28);
            menuStrip1.TabIndex = 5;
            menuStrip1.Text = "menuStrip1";
            // 
            // 常温测试ToolStripMenuItem
            // 
            常温测试ToolStripMenuItem.Name = "常温测试ToolStripMenuItem";
            常温测试ToolStripMenuItem.Size = new Size(83, 24);
            常温测试ToolStripMenuItem.Text = "常温测试";
            常温测试ToolStripMenuItem.Click += 常温测试ToolStripMenuItem_Click;
            // 
            // FreqItems
            // 
            FreqItems.Controls.Add(button3390);
            FreqItems.Controls.Add(button3330);
            FreqItems.Controls.Add(button3370);
            FreqItems.Controls.Add(button3350);
            FreqItems.Location = new Point(5, 209);
            FreqItems.Margin = new Padding(4);
            FreqItems.Name = "FreqItems";
            FreqItems.Padding = new Padding(4);
            FreqItems.Size = new Size(179, 171);
            FreqItems.TabIndex = 1;
            FreqItems.TabStop = false;
            FreqItems.Text = "频率选择";
            // 
            // button3390
            // 
            button3390.AutoSize = true;
            button3390.Location = new Point(34, 120);
            button3390.Margin = new Padding(2);
            button3390.Name = "button3390";
            button3390.Size = new Size(100, 24);
            button3390.TabIndex = 3;
            button3390.Text = "3390MHz";
            button3390.UseVisualStyleBackColor = true;
            // 
            // button3330
            // 
            button3330.AutoSize = true;
            button3330.Checked = true;
            button3330.Location = new Point(34, 34);
            button3330.Margin = new Padding(2);
            button3330.Name = "button3330";
            button3330.Size = new Size(100, 24);
            button3330.TabIndex = 0;
            button3330.TabStop = true;
            button3330.Text = "3330MHz";
            button3330.UseVisualStyleBackColor = true;
            // 
            // button3370
            // 
            button3370.AutoSize = true;
            button3370.Location = new Point(34, 91);
            button3370.Margin = new Padding(2);
            button3370.Name = "button3370";
            button3370.Size = new Size(100, 24);
            button3370.TabIndex = 2;
            button3370.Text = "3370MHz";
            button3370.UseVisualStyleBackColor = true;
            // 
            // button3350
            // 
            button3350.AutoSize = true;
            button3350.Location = new Point(34, 63);
            button3350.Margin = new Padding(2);
            button3350.Name = "button3350";
            button3350.Size = new Size(100, 24);
            button3350.TabIndex = 1;
            button3350.Text = "3350MHz";
            button3350.UseVisualStyleBackColor = true;
            // 
            // labeltemp
            // 
            labeltemp.AutoSize = true;
            labeltemp.Location = new Point(781, 40);
            labeltemp.Margin = new Padding(2, 0, 2, 0);
            labeltemp.Name = "labeltemp";
            labeltemp.Size = new Size(88, 20);
            labeltemp.TabIndex = 4;
            labeltemp.Text = "温度: --.- ℃";
            // 
            // comboBoxtemp
            // 
            comboBoxtemp.FormattingEnabled = true;
            comboBoxtemp.Location = new Point(990, 37);
            comboBoxtemp.Margin = new Padding(2);
            comboBoxtemp.Name = "comboBoxtemp";
            comboBoxtemp.Size = new Size(150, 28);
            comboBoxtemp.TabIndex = 3;
            // 
            // checkBoxAuto
            // 
            checkBoxAuto.AutoSize = true;
            checkBoxAuto.Location = new Point(895, 39);
            checkBoxAuto.Margin = new Padding(2);
            checkBoxAuto.Name = "checkBoxAuto";
            checkBoxAuto.Size = new Size(91, 24);
            checkBoxAuto.TabIndex = 1;
            checkBoxAuto.Text = "自动更新";
            checkBoxAuto.UseVisualStyleBackColor = true;
            // 
            // jztextlog
            // 
            jztextlog.BorderStyle = BorderStyle.None;
            jztextlog.Location = new Point(855, 73);
            jztextlog.Name = "jztextlog";
            jztextlog.Size = new Size(297, 320);
            jztextlog.TabIndex = 7;
            jztextlog.Text = "";
            // 
            // dgvCalibrationResults
            // 
            dgvCalibrationResults.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvCalibrationResults.Location = new Point(855, 399);
            dgvCalibrationResults.Name = "dgvCalibrationResults";
            dgvCalibrationResults.RowHeadersWidth = 51;
            dgvCalibrationResults.Size = new Size(297, 241);
            dgvCalibrationResults.TabIndex = 8;
            // 
            // TempSerialPort
            // 
            TempSerialPort.FormattingEnabled = true;
            TempSerialPort.Location = new Point(5, 31);
            TempSerialPort.Name = "TempSerialPort";
            TempSerialPort.Size = new Size(94, 28);
            TempSerialPort.TabIndex = 9;
            TempSerialPort.Text = "COM9";
            // 
            // openTempSerialPort
            // 
            openTempSerialPort.Location = new Point(172, 31);
            openTempSerialPort.Name = "openTempSerialPort";
            openTempSerialPort.Size = new Size(112, 29);
            openTempSerialPort.TabIndex = 9;
            openTempSerialPort.Text = "打开温度串口";
            openTempSerialPort.UseVisualStyleBackColor = true;
            // 
            // AllRead
            // 
            AllRead.Location = new Point(5, 141);
            AllRead.Name = "AllRead";
            AllRead.Size = new Size(94, 29);
            AllRead.TabIndex = 11;
            AllRead.Text = "全部读取";
            AllRead.UseVisualStyleBackColor = true;
            // 
            // startPhase
            // 
            startPhase.Location = new Point(5, 177);
            startPhase.Name = "startPhase";
            startPhase.Size = new Size(94, 29);
            startPhase.TabIndex = 12;
            startPhase.Text = "开始验证";
            startPhase.UseVisualStyleBackColor = true;
            startPhase.Click += btnStartVerification_Click;
            // 
            // button2
            // 
            button2.Location = new Point(5, 579);
            button2.Name = "button2";
            button2.Size = new Size(843, 61);
            button2.TabIndex = 14;
            button2.Text = "button2";
            button2.UseVisualStyleBackColor = true;
            // 
            // ultraWork
            // 
            ultraWork.Location = new Point(111, 141);
            ultraWork.Name = "ultraWork";
            ultraWork.Size = new Size(75, 29);
            ultraWork.TabIndex = 15;
            ultraWork.Text = "ulw1";
            ultraWork.UseVisualStyleBackColor = true;
            // 
            // ultraWork2
            // 
            ultraWork2.Location = new Point(111, 177);
            ultraWork2.Name = "ultraWork2";
            ultraWork2.Size = new Size(75, 29);
            ultraWork2.TabIndex = 16;
            ultraWork2.Text = "ulw2";
            ultraWork2.UseVisualStyleBackColor = true;
            ultraWork2.Click += ultraWork2_Click;
            // 
            // normal
            // 
            normal.AutoSize = true;
            normal.Location = new Point(457, 42);
            normal.Name = "normal";
            normal.Size = new Size(60, 24);
            normal.TabIndex = 17;
            normal.TabStop = true;
            normal.Text = "正常";
            normal.UseVisualStyleBackColor = true;
            // 
            // antenna
            // 
            antenna.AutoSize = true;
            antenna.Location = new Point(536, 42);
            antenna.Name = "antenna";
            antenna.Size = new Size(60, 24);
            antenna.TabIndex = 18;
            antenna.TabStop = true;
            antenna.Text = "天线";
            antenna.UseVisualStyleBackColor = true;
            // 
            // modechance
            // 
            modechance.FormattingEnabled = true;
            modechance.Items.AddRange(new object[] { "CH5", "CH8" });
            modechance.Location = new Point(105, 31);
            modechance.Name = "modechance";
            modechance.Size = new Size(61, 28);
            modechance.TabIndex = 19;
            // 
            // SwitchPower
            // 
            SwitchPower.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            SwitchPower.Location = new Point(5, 73);
            SwitchPower.Name = "SwitchPower";
            SwitchPower.Size = new Size(181, 62);
            SwitchPower.TabIndex = 20;
            SwitchPower.Text = "开启供电";
            SwitchPower.UseVisualStyleBackColor = true;
            // 
            // FormCalibration
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            ClientSize = new Size(1157, 642);
            Controls.Add(SwitchPower);
            Controls.Add(modechance);
            Controls.Add(antenna);
            Controls.Add(normal);
            Controls.Add(ultraWork2);
            Controls.Add(ultraWork);
            Controls.Add(button2);
            Controls.Add(startPhase);
            Controls.Add(AllRead);
            Controls.Add(openTempSerialPort);
            Controls.Add(TempSerialPort);
            Controls.Add(dgvCalibrationResults);
            Controls.Add(jztextlog);
            Controls.Add(checkBoxAuto);
            Controls.Add(labeltemp);
            Controls.Add(comboBoxtemp);
            Controls.Add(menuStrip1);
            Controls.Add(gbParameters);
            Controls.Add(gbChannels);
            Controls.Add(FreqItems);
            MainMenuStrip = menuStrip1;
            Margin = new Padding(4);
            Name = "FormCalibration";
            Text = "校准界面";
            gbChannels.ResumeLayout(false);
            gbChannels.PerformLayout();
            gbParameters.ResumeLayout(false);
            gbParameters.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            FreqItems.ResumeLayout(false);
            FreqItems.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvCalibrationResults).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private GroupBox gbChannels;
        private RadioButton jzchannel5;
        private RadioButton jzchannel4;
        private RadioButton jzchannel3;
        private RadioButton jzchannel2;
        private RadioButton jzchannel1;
        private GroupBox gbParameters;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem 常温测试ToolStripMenuItem;
        private GroupBox FreqItems;
        public RadioButton button3390;
        public RadioButton button3330;
        public RadioButton button3370;
        public RadioButton button3350;
        private ComboBox comboBoxtemp;
        private CheckBox checkBoxAuto;
        private Label labeltemp;
        private RichTextBox jztextlog;
        private ComboBox TempSerialPort;
        private Button openTempSerialPort;
        private Button AllRead;
        private Button btnUpDacHigh;
        private Button btnDownDacHigh;
        private Button btnUpDacLow;
        private Button btnDownDacLow;
        private Button btnUpXndHigh;
        private Button btnDownXndHigh;
        private Button btnUpXndLow;
        private Button btnDownXndLow;
        private Button write1;
        private Button read1;
        private TextBox txtCalDacHigh;
        private TextBox txtCalDacLow;
        private TextBox txtCalXndHigh;
        private TextBox txtCalXndLow;
        private Label lblDacLow;
        private Label lblXndHigh;
        private Label lblXndLow;
        private Label lblAddrDacLow;
        private Label lblAddrXndHigh;
        private Label lblAddrXndLow;
        private Label lblAddrDacHigh;
        private Label lblDacHigh;
        private DataGridView dgvCalibrationResults;
        private Button startPhase;
        private Button write4;
        private Button read4;
        private Button write3;
        private Button read3;
        private Button write2;
        private Button read2;
        private Button button2;
        private Button ultraWork;
        private Button ultraWork2;
        private RadioButton normal;
        private RadioButton antenna;
        private RadioButton jzchannel6;
        private RadioButton jzchannel8;
        private RadioButton jzchannel7;
        private ComboBox modechance;
        private Button SwitchPower;
    }
}
