using System;
using System.Windows.Forms;

namespace 五通道自动测试.Calibration
{
    public partial class FormTemperatureSelection : Form
    {
        public int SelectedTemperatureIndex { get; private set; } = -1;

        public FormTemperatureSelection()
        {
            InitializeComponent();
        }

        public FormTemperatureSelection(int currentTempIndex)
        {
            InitializeComponent();
            HighlightCurrentTemperature(currentTempIndex);
        }

        private void HighlightCurrentTemperature(int currentIndex)
        {
            if (currentIndex < 0 || currentIndex > 15) return;

            Button[] buttons = new Button[]
            {
                btnTemp0, btnTemp1, btnTemp2, btnTemp3, btnTemp4, btnTemp5,
                btnTemp6, btnTemp7, btnTemp8, btnTemp9, btnTemp10, btnTemp11,
                btnTemp12, btnTemp13, btnTemp14, btnTemp15
            };

            if (currentIndex >= 0 && currentIndex < buttons.Length)
            {
                buttons[currentIndex].BackColor = System.Drawing.Color.LightBlue;
                buttons[currentIndex].Text += " (当前)";
            }
        }

        private void BtnTemp_Click(object sender, EventArgs e)
        {
            if (sender is Button clickedButton)
            {
                int index = GetButtonIndex(clickedButton);
                if (index >= 0)
                {
                    SelectedTemperatureIndex = index;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }

        private int GetButtonIndex(Button button)
        {
            if (button == btnTemp0) return 0;
            if (button == btnTemp1) return 1;
            if (button == btnTemp2) return 2;
            if (button == btnTemp3) return 3;
            if (button == btnTemp4) return 4;
            if (button == btnTemp5) return 5;
            if (button == btnTemp6) return 6;
            if (button == btnTemp7) return 7;
            if (button == btnTemp8) return 8;
            if (button == btnTemp9) return 9;
            if (button == btnTemp10) return 10;
            if (button == btnTemp11) return 11;
            if (button == btnTemp12) return 12;
            if (button == btnTemp13) return 13;
            if (button == btnTemp14) return 14;
            if (button == btnTemp15) return 15;
            return -1;
        }
    }
}
