using System;
using System.Drawing;
using System.Windows.Forms;

namespace 五通道自动测试.Calibration
{
    partial class FormTemperatureSelection
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
            flowLayoutPanel = new FlowLayoutPanel();
            btnTemp0 = new Button();
            btnTemp1 = new Button();
            btnTemp2 = new Button();
            btnTemp3 = new Button();
            btnTemp4 = new Button();
            btnTemp5 = new Button();
            btnTemp6 = new Button();
            btnTemp7 = new Button();
            btnTemp8 = new Button();
            btnTemp9 = new Button();
            btnTemp10 = new Button();
            btnTemp11 = new Button();
            btnTemp12 = new Button();
            btnTemp13 = new Button();
            btnTemp14 = new Button();
            btnTemp15 = new Button();
            labelTitle = new Label();
            flowLayoutPanel.SuspendLayout();
            SuspendLayout();
            // 
            // flowLayoutPanel
            // 
            flowLayoutPanel.Controls.Add(btnTemp0);
            flowLayoutPanel.Controls.Add(btnTemp1);
            flowLayoutPanel.Controls.Add(btnTemp2);
            flowLayoutPanel.Controls.Add(btnTemp3);
            flowLayoutPanel.Controls.Add(btnTemp4);
            flowLayoutPanel.Controls.Add(btnTemp5);
            flowLayoutPanel.Controls.Add(btnTemp6);
            flowLayoutPanel.Controls.Add(btnTemp7);
            flowLayoutPanel.Controls.Add(btnTemp8);
            flowLayoutPanel.Controls.Add(btnTemp9);
            flowLayoutPanel.Controls.Add(btnTemp10);
            flowLayoutPanel.Controls.Add(btnTemp11);
            flowLayoutPanel.Controls.Add(btnTemp12);
            flowLayoutPanel.Controls.Add(btnTemp13);
            flowLayoutPanel.Controls.Add(btnTemp14);
            flowLayoutPanel.Controls.Add(btnTemp15);
            flowLayoutPanel.Dock = DockStyle.Fill;
            flowLayoutPanel.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel.Location = new Point(0, 0);
            flowLayoutPanel.Margin = new Padding(4);
            flowLayoutPanel.Name = "flowLayoutPanel";
            flowLayoutPanel.Size = new Size(681, 330);
            flowLayoutPanel.TabIndex = 0;
            // 
            // btnTemp0
            // 
            btnTemp0.BackColor = Color.White;
            btnTemp0.FlatStyle = FlatStyle.Flat;
            btnTemp0.Location = new Point(4, 4);
            btnTemp0.Margin = new Padding(4);
            btnTemp0.Name = "btnTemp0";
            btnTemp0.Size = new Size(219, 47);
            btnTemp0.TabIndex = 2;
            btnTemp0.Text = "T<=-50";
            btnTemp0.UseVisualStyleBackColor = false;
            btnTemp0.Click += BtnTemp_Click;
            // 
            // btnTemp1
            // 
            btnTemp1.BackColor = Color.White;
            btnTemp1.FlatStyle = FlatStyle.Flat;
            btnTemp1.Location = new Point(4, 59);
            btnTemp1.Margin = new Padding(4);
            btnTemp1.Name = "btnTemp1";
            btnTemp1.Size = new Size(219, 47);
            btnTemp1.TabIndex = 3;
            btnTemp1.Text = "-50<T<=-40";
            btnTemp1.UseVisualStyleBackColor = false;
            btnTemp1.Click += BtnTemp_Click;
            // 
            // btnTemp2
            // 
            btnTemp2.BackColor = Color.White;
            btnTemp2.FlatStyle = FlatStyle.Flat;
            btnTemp2.Location = new Point(4, 114);
            btnTemp2.Margin = new Padding(4);
            btnTemp2.Name = "btnTemp2";
            btnTemp2.Size = new Size(219, 47);
            btnTemp2.TabIndex = 4;
            btnTemp2.Text = "-40<T<=-30";
            btnTemp2.UseVisualStyleBackColor = false;
            btnTemp2.Click += BtnTemp_Click;
            // 
            // btnTemp3
            // 
            btnTemp3.BackColor = Color.White;
            btnTemp3.FlatStyle = FlatStyle.Flat;
            btnTemp3.Location = new Point(4, 169);
            btnTemp3.Margin = new Padding(4);
            btnTemp3.Name = "btnTemp3";
            btnTemp3.Size = new Size(219, 47);
            btnTemp3.TabIndex = 5;
            btnTemp3.Text = "-30<T<=-20";
            btnTemp3.UseVisualStyleBackColor = false;
            btnTemp3.Click += BtnTemp_Click;
            // 
            // btnTemp4
            // 
            btnTemp4.BackColor = Color.White;
            btnTemp4.FlatStyle = FlatStyle.Flat;
            btnTemp4.Location = new Point(4, 224);
            btnTemp4.Margin = new Padding(4);
            btnTemp4.Name = "btnTemp4";
            btnTemp4.Size = new Size(219, 47);
            btnTemp4.TabIndex = 6;
            btnTemp4.Text = "-20<T<=-10";
            btnTemp4.UseVisualStyleBackColor = false;
            btnTemp4.Click += BtnTemp_Click;
            // 
            // btnTemp5
            // 
            btnTemp5.BackColor = Color.White;
            btnTemp5.FlatStyle = FlatStyle.Flat;
            btnTemp5.Location = new Point(4, 279);
            btnTemp5.Margin = new Padding(4);
            btnTemp5.Name = "btnTemp5";
            btnTemp5.Size = new Size(219, 47);
            btnTemp5.TabIndex = 7;
            btnTemp5.Text = "-10<T<=0";
            btnTemp5.UseVisualStyleBackColor = false;
            btnTemp5.Click += BtnTemp_Click;
            // 
            // btnTemp6
            // 
            btnTemp6.BackColor = Color.White;
            btnTemp6.FlatStyle = FlatStyle.Flat;
            btnTemp6.Location = new Point(231, 4);
            btnTemp6.Margin = new Padding(4);
            btnTemp6.Name = "btnTemp6";
            btnTemp6.Size = new Size(219, 47);
            btnTemp6.TabIndex = 8;
            btnTemp6.Text = "0<T<=10";
            btnTemp6.UseVisualStyleBackColor = false;
            btnTemp6.Click += BtnTemp_Click;
            // 
            // btnTemp7
            // 
            btnTemp7.BackColor = Color.White;
            btnTemp7.FlatStyle = FlatStyle.Flat;
            btnTemp7.Location = new Point(231, 59);
            btnTemp7.Margin = new Padding(4);
            btnTemp7.Name = "btnTemp7";
            btnTemp7.Size = new Size(219, 47);
            btnTemp7.TabIndex = 9;
            btnTemp7.Text = "10<T<=20";
            btnTemp7.UseVisualStyleBackColor = false;
            btnTemp7.Click += BtnTemp_Click;
            // 
            // btnTemp8
            // 
            btnTemp8.BackColor = Color.White;
            btnTemp8.FlatStyle = FlatStyle.Flat;
            btnTemp8.Location = new Point(231, 114);
            btnTemp8.Margin = new Padding(4);
            btnTemp8.Name = "btnTemp8";
            btnTemp8.Size = new Size(219, 47);
            btnTemp8.TabIndex = 10;
            btnTemp8.Text = "20<T<=30";
            btnTemp8.UseVisualStyleBackColor = false;
            btnTemp8.Click += BtnTemp_Click;
            // 
            // btnTemp9
            // 
            btnTemp9.BackColor = Color.White;
            btnTemp9.FlatStyle = FlatStyle.Flat;
            btnTemp9.Location = new Point(231, 169);
            btnTemp9.Margin = new Padding(4);
            btnTemp9.Name = "btnTemp9";
            btnTemp9.Size = new Size(219, 47);
            btnTemp9.TabIndex = 11;
            btnTemp9.Text = "30<T<=40";
            btnTemp9.UseVisualStyleBackColor = false;
            btnTemp9.Click += BtnTemp_Click;
            // 
            // btnTemp10
            // 
            btnTemp10.BackColor = Color.White;
            btnTemp10.FlatStyle = FlatStyle.Flat;
            btnTemp10.Location = new Point(231, 224);
            btnTemp10.Margin = new Padding(4);
            btnTemp10.Name = "btnTemp10";
            btnTemp10.Size = new Size(219, 47);
            btnTemp10.TabIndex = 12;
            btnTemp10.Text = "40<T<=50";
            btnTemp10.UseVisualStyleBackColor = false;
            btnTemp10.Click += BtnTemp_Click;
            // 
            // btnTemp11
            // 
            btnTemp11.BackColor = Color.White;
            btnTemp11.FlatStyle = FlatStyle.Flat;
            btnTemp11.Location = new Point(231, 279);
            btnTemp11.Margin = new Padding(4);
            btnTemp11.Name = "btnTemp11";
            btnTemp11.Size = new Size(219, 47);
            btnTemp11.TabIndex = 13;
            btnTemp11.Text = "50<T<=60";
            btnTemp11.UseVisualStyleBackColor = false;
            btnTemp11.Click += BtnTemp_Click;
            // 
            // btnTemp12
            // 
            btnTemp12.BackColor = Color.White;
            btnTemp12.FlatStyle = FlatStyle.Flat;
            btnTemp12.Location = new Point(458, 4);
            btnTemp12.Margin = new Padding(4);
            btnTemp12.Name = "btnTemp12";
            btnTemp12.Size = new Size(219, 47);
            btnTemp12.TabIndex = 14;
            btnTemp12.Text = "60<T<=70";
            btnTemp12.UseVisualStyleBackColor = false;
            btnTemp12.Click += BtnTemp_Click;
            // 
            // btnTemp13
            // 
            btnTemp13.BackColor = Color.White;
            btnTemp13.FlatStyle = FlatStyle.Flat;
            btnTemp13.Location = new Point(458, 59);
            btnTemp13.Margin = new Padding(4);
            btnTemp13.Name = "btnTemp13";
            btnTemp13.Size = new Size(219, 47);
            btnTemp13.TabIndex = 15;
            btnTemp13.Text = "70<T<=80";
            btnTemp13.UseVisualStyleBackColor = false;
            btnTemp13.Click += BtnTemp_Click;
            // 
            // btnTemp14
            // 
            btnTemp14.BackColor = Color.White;
            btnTemp14.FlatStyle = FlatStyle.Flat;
            btnTemp14.Location = new Point(458, 114);
            btnTemp14.Margin = new Padding(4);
            btnTemp14.Name = "btnTemp14";
            btnTemp14.Size = new Size(219, 47);
            btnTemp14.TabIndex = 16;
            btnTemp14.Text = "80<T<=90";
            btnTemp14.UseVisualStyleBackColor = false;
            btnTemp14.Click += BtnTemp_Click;
            // 
            // btnTemp15
            // 
            btnTemp15.BackColor = Color.White;
            btnTemp15.FlatStyle = FlatStyle.Flat;
            btnTemp15.Location = new Point(458, 169);
            btnTemp15.Margin = new Padding(4);
            btnTemp15.Name = "btnTemp15";
            btnTemp15.Size = new Size(219, 47);
            btnTemp15.TabIndex = 17;
            btnTemp15.Text = "90<T";
            btnTemp15.UseVisualStyleBackColor = false;
            btnTemp15.Click += BtnTemp_Click;
            // 
            // labelTitle
            // 
            labelTitle.AutoSize = true;
            labelTitle.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
            labelTitle.Location = new Point(13, 12);
            labelTitle.Margin = new Padding(4, 0, 4, 0);
            labelTitle.Name = "labelTitle";
            labelTitle.Size = new Size(172, 27);
            labelTitle.TabIndex = 1;
            labelTitle.Text = "选择目标温度区间";
            // 
            // FormTemperatureSelection
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(681, 330);
            Controls.Add(flowLayoutPanel);
            Controls.Add(labelTitle);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormTemperatureSelection";
            StartPosition = FormStartPosition.CenterParent;
            Text = "选择温度区间";
            flowLayoutPanel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Button btnTemp0;
        private System.Windows.Forms.Button btnTemp1;
        private System.Windows.Forms.Button btnTemp2;
        private System.Windows.Forms.Button btnTemp3;
        private System.Windows.Forms.Button btnTemp4;
        private System.Windows.Forms.Button btnTemp5;
        private System.Windows.Forms.Button btnTemp6;
        private System.Windows.Forms.Button btnTemp7;
        private System.Windows.Forms.Button btnTemp8;
        private System.Windows.Forms.Button btnTemp9;
        private System.Windows.Forms.Button btnTemp10;
        private System.Windows.Forms.Button btnTemp11;
        private System.Windows.Forms.Button btnTemp12;
        private System.Windows.Forms.Button btnTemp13;
        private System.Windows.Forms.Button btnTemp14;
        private System.Windows.Forms.Button btnTemp15;
    }
}
