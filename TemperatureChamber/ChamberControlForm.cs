using TemperatureChamber;
using TemperatureChamber.Models;
using System;
using System.Windows.Forms;

namespace TemperatureChamber
{
    public partial class ChamberControlForm : Form
    {
        private ChamberController? _chamberController;
        private ChamberConfig? _chamberConfig;

        public ChamberControlForm()
        {
            InitializeComponent();
            InitializeChamberControl();
        }

        /// <summary>
        /// 初始化温箱控制
        /// </summary>
        private void InitializeChamberControl()
        {
            _chamberConfig = new ChamberConfig
            {
                PortName = "COM19",
                BaudRate = 38400,
                DataBits = 8,
                Parity = System.IO.Ports.Parity.Even,
                StopBits = System.IO.Ports.StopBits.One,
                SlaveId = 1,
                Timeout = 10000,
                MinTemperature = -40,
                MaxTemperature = 150,
                MinHumidity = 20,
                MaxHumidity = 95
            };

            _chamberController = new ChamberController(_chamberConfig);
            _chamberController.ConnectionChanged += OnChamberConnectionChanged;
            _chamberController.ErrorOccurred += OnChamberErrorOccurred;
            _chamberController.StatusUpdated += OnChamberStatusUpdated;
            _chamberController.Debug += OnChamberDebug;
        }

        /// <summary>
        /// 温箱连接状态变化事件处理
        /// </summary>
        private void OnChamberConnectionChanged(object? sender, bool isConnected)
        {
            if (txtTestLog != null)
            {
                string message = isConnected ? "温箱设备已连接" : "温箱设备已断开";
                txtTestLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\r\n");
                txtTestLog.ScrollToCaret();
            }
        }

        /// <summary>
        /// 温箱错误发生事件处理
        /// </summary>
        private void OnChamberErrorOccurred(object? sender, string errorMessage)
        {
            if (txtTestLog != null)
            {
                txtTestLog.AppendText($"[{DateTime.Now:HH:mm:ss}] 温箱错误：{errorMessage}\r\n");
                txtTestLog.ScrollToCaret();
            }
        }

        /// <summary>
        /// 温箱调试信息事件处理
        /// </summary>
        private void OnChamberDebug(object? sender, string debugMessage)
        {
            if (txtTestLog != null)
            {
                txtTestLog.AppendText($"[{DateTime.Now:HH:mm:ss}] [调试] {debugMessage}\r\n");
                txtTestLog.ScrollToCaret();
            }
        }

        /// <summary>
        /// 温箱状态更新事件处理
        /// </summary>
        private void OnChamberStatusUpdated(object? sender, DeviceStatus status)
        {
            try
            {
                if (txtTestLog != null)
                {
                    string message = $"温箱状态：温度={status.Temperature:F1}℃，湿度={status.Humidity:F1}%，运行状态={(status.IsRunning ? "运行中" : "停止")}";
                    txtTestLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\r\n");
                    txtTestLog.ScrollToCaret();
                }

                if (lblTemperatureValue != null)
                {
                    lblTemperatureValue.Text = $"{status.Temperature:F1}℃";
                }

                if (lblHumidityValue != null)
                {
                    lblHumidityValue.Text = $"{status.Humidity:F1}%RH";
                }

                if (lblRunningStatus != null)
                {
                    lblRunningStatus.Text = status.IsRunning ? "运行中" : "未运行";
                    lblRunningStatus.ForeColor = status.IsRunning ? Color.Green : Color.Red;
                }

                if (pbRunningStatus != null)
                {
                    pbRunningStatus.BackColor = status.IsRunning ? Color.Green : Color.Red;
                }

                if (txtChamberStatus != null)
                {
                    txtChamberStatus.AppendText($"[{DateTime.Now:HH:mm:ss}] 温度：{status.Temperature:F1}℃，湿度：{status.Humidity:F1}%RH，状态：{(status.IsRunning ? "运行中" : "停止")}\r\n");
                    txtChamberStatus.ScrollToCaret();
                }
            }
            catch (Exception ex)
            {
                if (txtTestLog != null)
                {
                    txtTestLog.AppendText($"[{DateTime.Now:HH:mm:ss}] 状态更新错误：{ex.Message}\r\n");
                    txtTestLog.ScrollToCaret();
                }
            }
        }

        /// <summary>
        /// 温箱连接按钮点击事件
        /// </summary>
        private void btnConnectChamber_Click(object sender, EventArgs e)
        {
            if (_chamberController == null)
            {
                MessageBox.Show("温箱控制器未初始化");
                return;
            }

            try
            {
                txtTestLog?.AppendText($"[{DateTime.Now:HH:mm:ss}] 开始连接温箱设备...\r\n");
                txtTestLog?.AppendText($"[{DateTime.Now:HH:mm:ss}] 使用参数：端口={_chamberConfig?.PortName}, 波特率={_chamberConfig?.BaudRate}, 校验位={_chamberConfig?.Parity}, 从站地址={_chamberConfig?.SlaveId}\r\n");
                txtTestLog?.ScrollToCaret();

                bool success = _chamberController.Connect();

                if (success)
                {
                    txtTestLog?.AppendText($"[{DateTime.Now:HH:mm:ss}] 温箱设备连接成功\r\n");
                    txtTestLog?.ScrollToCaret();

                    double temperature = _chamberController.ReadTemperature();
                    txtTestLog?.AppendText($"[{DateTime.Now:HH:mm:ss}] 当前温度：{temperature:F1}℃\r\n");
                    txtTestLog?.ScrollToCaret();

                    UpdateChamberStatusUI(true);
                }
                else
                {
                    UpdateChamberStatusUI(false);
                    txtTestLog?.AppendText($"[{DateTime.Now:HH:mm:ss}] 温箱设备连接失败\r\n");
                    txtTestLog?.ScrollToCaret();
                }
            }
            catch (Exception ex)
            {
                UpdateChamberStatusUI(false);
                txtTestLog?.AppendText($"[{DateTime.Now:HH:mm:ss}] 连接错误：{ex.Message}\r\n");
                txtTestLog?.ScrollToCaret();
                MessageBox.Show($"连接失败：{ex.Message}", "连接错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 测试连接按钮点击事件
        /// </summary>
        private void btnTestChamberConnection_Click(object sender, EventArgs e)
        {
            if (_chamberController == null)
            {
                MessageBox.Show("温箱控制器未初始化");
                return;
            }

            try
            {
                txtTestLog?.AppendText($"[{DateTime.Now:HH:mm:ss}] 开始测试温箱连接...\r\n");
                txtTestLog?.ScrollToCaret();

                bool isConnected = _chamberController.IsConnected;
                txtTestLog?.AppendText($"[{DateTime.Now:HH:mm:ss}] 当前连接状态：{(isConnected ? "已连接" : "未连接")}\r\n");
                txtTestLog?.ScrollToCaret();

                if (!isConnected)
                {
                    bool success = _chamberController.Connect();
                    if (success)
                    {
                        txtTestLog?.AppendText($"[{DateTime.Now:HH:mm:ss}] 温箱设备连接成功\r\n");
                        txtTestLog?.ScrollToCaret();

                        // 读取运行状态测试连通性
                        ushort status = _chamberController.ReadDeviceStatus();
                        txtTestLog?.AppendText($"[{DateTime.Now:HH:mm:ss}] 运行状态: {status}\r\n");
                        txtTestLog?.ScrollToCaret();

                        double temperature = _chamberController.ReadTemperature();
                        txtTestLog?.AppendText($"[{DateTime.Now:HH:mm:ss}] 当前温度：{temperature:F1}℃\r\n");
                        txtTestLog?.ScrollToCaret();

                        UpdateChamberStatusUI(true);
                        MessageBox.Show("温箱设备连接测试成功！\n\n运行状态: " + status + "\n当前温度：" + temperature.ToString("F1") + "℃", "测试成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        UpdateChamberStatusUI(false);
                        txtTestLog?.AppendText($"[{DateTime.Now:HH:mm:ss}] 温箱设备连接失败\r\n");
                        txtTestLog?.ScrollToCaret();
                        MessageBox.Show("温箱设备连接测试失败，请检查设备电源和串口连接", "测试失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    // 读取运行状态测试连通性
                    ushort status = _chamberController.ReadDeviceStatus();
                    txtTestLog?.AppendText($"[{DateTime.Now:HH:mm:ss}] 运行状态: {status}\r\n");
                    txtTestLog?.ScrollToCaret();

                    double temperature = _chamberController.ReadTemperature();
                    txtTestLog?.AppendText($"[{DateTime.Now:HH:mm:ss}] 当前温度：{temperature:F1}℃\r\n");
                    txtTestLog?.ScrollToCaret();

                    MessageBox.Show("温箱设备连接测试成功！\n\n运行状态: " + status + "\n当前温度：" + temperature.ToString("F1") + "℃", "测试成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                txtTestLog?.AppendText($"[{DateTime.Now:HH:mm:ss}] 测试错误：{ex.Message}\r\n");
                txtTestLog?.ScrollToCaret();
                UpdateChamberStatusUI(false);
                MessageBox.Show($"测试过程中发生错误：{ex.Message}", "测试错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 温箱断开按钮点击事件
        /// </summary>
        private void btnDisconnectChamber_Click(object sender, EventArgs e)
        {
            if (_chamberController == null)
            {
                return;
            }

            try
            {
                _chamberController.Disconnect();
                UpdateChamberStatusUI(false);
                txtTestLog?.AppendText($"[{DateTime.Now:HH:mm:ss}] 温箱设备已断开连接\r\n");
                txtTestLog?.ScrollToCaret();
            }
            catch (Exception ex)
            {
                txtTestLog?.AppendText($"[{DateTime.Now:HH:mm:ss}] 断开连接错误：{ex.Message}\r\n");
                txtTestLog?.ScrollToCaret();
                MessageBox.Show($"断开连接失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 更新温箱连接状态UI
        /// </summary>
        private void UpdateChamberStatusUI(bool isConnected)
        {
            if (lblChamberStatus != null)
            {
                lblChamberStatus.Text = isConnected ? "已连接" : "未连接";
                lblChamberStatus.ForeColor = isConnected ? Color.Green : Color.Red;
                lblChamberStatus.Font = new Font(lblChamberStatus.Font, isConnected ? FontStyle.Bold : FontStyle.Regular);
            }

            if (pbChamberStatus != null)
            {
                pbChamberStatus.BackColor = isConnected ? Color.Green : Color.Red;
                pbChamberStatus.Visible = false;
                pbChamberStatus.Visible = true;
            }

            if (btnStartChamber != null)
            {
                btnStartChamber.Enabled = isConnected;
                btnStartChamber.BackColor = isConnected ? Color.LightGreen : SystemColors.Control;
            }

            if (btnStopChamber != null)
            {
                btnStopChamber.Enabled = isConnected;
                btnStopChamber.BackColor = isConnected ? Color.LightCoral : SystemColors.Control;
            }

            if (nudTargetTemperature != null)
            {
                nudTargetTemperature.Enabled = isConnected;
                nudTargetTemperature.BackColor = isConnected ? Color.White : SystemColors.Control;
            }

            if (btnSetTemperature != null)
            {
                btnSetTemperature.Enabled = isConnected;
                btnSetTemperature.BackColor = isConnected ? Color.LightYellow : SystemColors.Control;
            }
        }

        /// <summary>
        /// 刷新状态按钮点击事件
        /// </summary>
        private void btnRefreshStatus_Click(object sender, EventArgs e)
        {
            if (_chamberController == null || !_chamberController.IsConnected)
            {
                MessageBox.Show("设备未连接");
                return;
            }

            try
            {
                double temperature = _chamberController.ReadTemperature();
                txtTestLog?.AppendText($"[{DateTime.Now:HH:mm:ss}] 当前温度：{temperature:F1}℃\r\n");
                txtTestLog?.ScrollToCaret();
            }
            catch (Exception ex)
            {
                txtTestLog?.AppendText($"[{DateTime.Now:HH:mm:ss}] 刷新状态错误：{ex.Message}\r\n");
                txtTestLog?.ScrollToCaret();
                MessageBox.Show($"刷新状态失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 设置温度按钮点击事件
        /// </summary>
        private void btnSetTemperature_Click(object sender, EventArgs e)
        {
            if (_chamberController == null || !_chamberController.IsConnected)
            {
                MessageBox.Show("设备未连接");
                return;
            }

            if (nudTargetTemperature == null)
            {
                return;
            }

            try
            {
                double targetTemp = (double)nudTargetTemperature.Value;
                _chamberController.SetTemperature(targetTemp);
                txtTestLog?.AppendText($"[{DateTime.Now:HH:mm:ss}] 温度已设置为：{targetTemp:F1}℃\r\n");
                txtTestLog?.ScrollToCaret();
                MessageBox.Show($"温度已设置为：{targetTemp:F1}℃");
            }
            catch (Exception ex)
            {
                txtTestLog?.AppendText($"[{DateTime.Now:HH:mm:ss}] 设置温度错误：{ex.Message}\r\n");
                txtTestLog?.ScrollToCaret();
                MessageBox.Show($"设置温度失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 参数配置按钮点击事件
        /// </summary>
        private void btnChamberSettings_Click(object sender, EventArgs e)
        {
            if (_chamberConfig == null)
            {
                MessageBox.Show("配置未初始化");
                return;
            }

            try
            {
                bool wasConnected = _chamberController?.IsConnected ?? false;

                using (var settingsForm = new ChamberSettingsForm(_chamberConfig))
                {
                    if (settingsForm.ShowDialog() == DialogResult.OK)
                    {
                        txtTestLog?.AppendText($"[{DateTime.Now:HH:mm:ss}] 配置已更新：端口={_chamberConfig.PortName}, 波特率={_chamberConfig.BaudRate}, 从站地址={_chamberConfig.SlaveId}\r\n");
                        txtTestLog?.ScrollToCaret();

                        if (wasConnected)
                        {
                            txtTestLog?.AppendText($"[{DateTime.Now:HH:mm:ss}] 配置已更改，正在重新连接设备...\r\n");
                            txtTestLog?.ScrollToCaret();

                            _chamberController?.Disconnect();
                            UpdateChamberStatusUI(false);

                            bool success = _chamberController?.Connect() ?? false;
                            if (success)
                            {
                                txtTestLog?.AppendText($"[{DateTime.Now:HH:mm:ss}] 设备重新连接成功\r\n");
                                txtTestLog?.ScrollToCaret();
                                UpdateChamberStatusUI(true);
                            }
                            else
                            {
                                txtTestLog?.AppendText($"[{DateTime.Now:HH:mm:ss}] 设备重新连接失败\r\n");
                                txtTestLog?.ScrollToCaret();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                txtTestLog?.AppendText($"[{DateTime.Now:HH:mm:ss}] 配置错误：{ex.Message}\r\n");
                txtTestLog?.ScrollToCaret();
                MessageBox.Show($"配置失败：{ex.Message}", "配置错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 启动设备按钮点击事件
        /// </summary>
        private void btnStartChamber_Click(object sender, EventArgs e)
        {
            if (_chamberController == null || !_chamberController.IsConnected)
            {
                MessageBox.Show("设备未连接");
                return;
            }

            try
            {
                _chamberController.StartDevice();
                txtTestLog?.AppendText($"[{DateTime.Now:HH:mm:ss}] 温箱设备已启动\r\n");
                txtTestLog?.ScrollToCaret();
                MessageBox.Show("温箱设备已启动");
            }
            catch (Exception ex)
            {
                txtTestLog?.AppendText($"[{DateTime.Now:HH:mm:ss}] 启动设备错误：{ex.Message}\r\n");
                txtTestLog?.ScrollToCaret();
                MessageBox.Show($"启动设备失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 停止设备按钮点击事件
        /// </summary>
        private void btnStopChamber_Click(object sender, EventArgs e)
        {
            if (_chamberController == null || !_chamberController.IsConnected)
            {
                MessageBox.Show("设备未连接");
                return;
            }

            try
            {
                _chamberController.StopDevice();
                txtTestLog?.AppendText($"[{DateTime.Now:HH:mm:ss}] 温箱设备已停止\r\n");
                txtTestLog?.ScrollToCaret();
                MessageBox.Show("温箱设备已停止");
            }
            catch (Exception ex)
            {
                txtTestLog?.AppendText($"[{DateTime.Now:HH:mm:ss}] 停止设备错误：{ex.Message}\r\n");
                txtTestLog?.ScrollToCaret();
                MessageBox.Show($"停止设备失败：{ex.Message}");
            }
        }

        private void ChamberControlForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_chamberController != null)
            {
                _chamberController.Disconnect();
                _chamberController.ConnectionChanged -= OnChamberConnectionChanged;
                _chamberController.ErrorOccurred -= OnChamberErrorOccurred;
                _chamberController.StatusUpdated -= OnChamberStatusUpdated;
                _chamberController.Debug -= OnChamberDebug;
                _chamberController.Dispose();
            }
        }
    }
}
