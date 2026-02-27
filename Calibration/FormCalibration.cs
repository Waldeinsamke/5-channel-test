using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using 五通道自动测试.Instruments;
using 五通道自动测试.Services;
using 五通道自动测试.Test;

namespace 五通道自动测试.Calibration
{
    /// <summary>
    /// 五通道自动测试系统的校准窗体
    /// 提供温度传感器连接、通道选择、频率选择和校准参数配置功能
    /// 支持常温测试和温度相关的校准地址计算
    /// </summary>
    public partial class FormCalibration : Form
    {
        #region 成员变量
        
        // 仪器和服务相关
        private readonly InstrumentManager _instrumentManager;
        private Form1? _mainForm;
        private VerificationService? _verificationService;
        
        // 状态变量
        private int _currentChannel = 1;
        private int _frequencyIndex = 1;
        private int _temperatureIndex = 1;
        private bool _isUlw2Mode = false;
        private bool _isPowerOn = false; // 电源状态
        private float _currentTemperature = 0;
        private string _currentMode = "normal"; // 当前模式：normal 或 antenna
        private string _channelMode = "CH5"; // 当前通道模式：CH5 或 CH8
        
        // 验证测试结果
        private List<TestResult> _verificationResults = new List<TestResult>();
        
        // 温度相关
        private readonly TemperatureSerialPortManager _temperatureSerialPortManager;
        private bool _autoUpdateTemp = false;
        
        // 校准逻辑和UI更新器
        private readonly CalibrationLogic _calibrationLogic;
        private readonly CalibrationUIUpdater _calibrationUIUpdater;
        private readonly CalibrationAddressCalculator8 _addressCalculator8;

        // 长按连续变化相关
        private readonly System.Windows.Forms.Timer _initialDelayTimer;
        private readonly System.Windows.Forms.Timer _repeatTimer;
        private TextBox? _targetTextBox;
        private int _repeatDirection;
        private bool _isLongPress;

        #endregion

        /// <summary>
        /// 构造函数，初始化校准窗体
        /// 1. 接收仪器管理器和主窗体引用
        /// 2. 调用InitializeComponent加载UI布局
        /// 3. 初始化频率按钮、通道选择、温度下拉框等控件
        /// 4. 绑定窗体关闭事件
        /// 5. 计算并更新校准参数显示
        /// </summary>
        /// <param name="instrumentManager">仪器管理器实例，用于控制频谱仪和信号源</param>
        /// <param name="mainForm">主窗体引用（可选）</param>
        public FormCalibration(InstrumentManager instrumentManager, Form1? mainForm = null)
        {
            // 接收外部传入的仪器管理器和主窗体
            _instrumentManager = instrumentManager;
            _mainForm = mainForm;

            // 调用Windows Forms设计器生成的代码，初始化UI控件
            InitializeComponent();

            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;

            // 初始化校准逻辑
            _calibrationLogic = new CalibrationLogic();

            // 初始化八通道地址计算器
            _addressCalculator8 = new CalibrationAddressCalculator8();

            // 初始化温度串口管理器
            _temperatureSerialPortManager = new TemperatureSerialPortManager(
                OnTemperatureUpdated,
                OnLogMessage,
                OnEepromDataReceived);

            // 初始化校准UI更新器
            _calibrationUIUpdater = new CalibrationUIUpdater(
                this,
                LogMessage,
                _calibrationLogic,
                _addressCalculator8);

            // 初始化长按连续变化的 Timer
            _initialDelayTimer = new System.Windows.Forms.Timer();
            _initialDelayTimer.Interval = 300;
            _initialDelayTimer.Tick += OnInitialDelayTimerTick;

            _repeatTimer = new System.Windows.Forms.Timer();
            _repeatTimer.Interval = 50;
            _repeatTimer.Tick += OnRepeatTimerTick;

            _isLongPress = false;

            // 初始化验证服务
            _verificationService = new VerificationService(
                _instrumentManager,
                this,
                AddCalibrationResult,
                LogMessage,
                _channelMode);

            // 初始化各功能模块
            InitializeFrequencyButtons();    // 频率选择按钮 (3.33GHz/3.35GHz/3.37GHz/3.39GHz)
            InitializeChannelSelection();    // 通道选择复选框 (通道1-通道5)
            InitializeCalibrationUI();       // 初始化校准UI控件
            InitializeControls();            // 其他控件事件绑定

            // 订阅电源状态变化事件
            _instrumentManager.PowerStateChanged += OnPowerStateChanged;

            // 初始化校准结果表格（先初始化列，再添加行）
            InitializeCalibrationResultsGrid();

            // 订阅窗体关闭事件，确保资源被正确释放
            this.FormClosing += FormCalibration_FormClosing;

            // 初始化模式状态
            _calibrationUIUpdater.SetCurrentMode(_currentMode);
            
            // 初始化通道模式选择控件
            modechance.SelectedIndex = 0; // 默认选择CH5
            // 手动触发一次事件，初始化通道状态
            ModeChance_SelectedIndexChanged(modechance, EventArgs.Empty);
            
            // 根据当前频率和温度设置计算校准地址
            CalculateAddresses();

            // 更新UI控件显示当前通道的校准参数地址
            UpdateCalibrationControls();

            // 进入校准界面时，默认向信号源和工装板发送3330MHz频率
            double defaultFrequency = 3330.0;

            try
            {
                // 向工装板发送3330MHz频率切换命令
                _instrumentManager.SendFrequencyCommand(defaultFrequency);

                // 向信号源发送3330MHz频率切换命令
                _instrumentManager.SignalGenerator.SetFrequency(defaultFrequency);

                // 根据当前选中的通道向工装板发送通道切换命令
                int currentChannel = GetCurrentSelectedChannel();
                _instrumentManager.SwitchChannel(currentChannel);

                LogMessage($"初始化完成：频率设置为 {defaultFrequency} MHz，通道设置为 {currentChannel}");
            }
            catch (Exception ex)
            {
                string errorMsg = $"初始化失败：{ex.Message}";
                LogMessage(errorMsg);
                MessageBox.Show(errorMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 初始化校准结果表格
        /// 设置表格列和样式，仅显示测试项和测试值两列
        /// </summary>
        private void InitializeCalibrationResultsGrid()
        {
            // 添加空引用检查
            if (dgvCalibrationResults == null) return;

            // 清空现有列（如果有）
            dgvCalibrationResults.Columns.Clear();

            // 添加测试项列
            dgvCalibrationResults.Columns.Add("TestItem", "测试项");

            // 添加测试值列
            dgvCalibrationResults.Columns.Add("Value", "测试值");

            // 设置列宽自动调整
            dgvCalibrationResults.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // 设置列比例
            dgvCalibrationResults.Columns["TestItem"].FillWeight = 180;
            dgvCalibrationResults.Columns["Value"].FillWeight = 105;

            // 设置整列居中对齐
            foreach (DataGridViewColumn col in dgvCalibrationResults.Columns)
            {
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            // 设置列头居中对齐
            dgvCalibrationResults.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        /// <summary>
        /// 添加校准结果到表格
        /// </summary>
        /// <param name="testItem">测试项名称</param>
        /// <param name="value">测试值</param>
        private void AddCalibrationResult(string testItem, string value)
        {
            // 确保在UI线程中更新DataGridView控件
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string, string>(AddCalibrationResult), testItem, value);
                return;
            }

            // 添加空引用检查
            if (dgvCalibrationResults == null) return;

            // 检查是否有列，避免尝试向没有列的DataGridView添加行
            if (dgvCalibrationResults.Columns.Count == 0) return;

            dgvCalibrationResults.Rows.Add(testItem, value);

            // 自动滚动到最新行
            dgvCalibrationResults.FirstDisplayedScrollingRowIndex = dgvCalibrationResults.RowCount - 1;

            // 创建TestResult对象并存入列表
            try
            {
                // 从testItem中解析通道号和频率
                // 格式: "通道{channel} {frequency}MHz"
                var match = System.Text.RegularExpressions.Regex.Match(testItem, @"通道(\d+) (\d+)MHz");
                if (match.Success)
                {
                    int channel = int.Parse(match.Groups[1].Value);
                    double frequency = double.Parse(match.Groups[2].Value);
                    
                    // 尝试解析值
                    double testValue = 0;
                    if (double.TryParse(value, out testValue))
                    {
                        var result = new TestResult
                        {
                            Channel = channel,
                            TestItem = testItem,
                            Value = testValue,
                            Unit = "dBm",
                            IsPass = true, // 默认认为通过
                            TestTime = DateTime.Now,
                            StandardValue = 0,
                            ComparisonType = ""
                        };
                        _verificationResults.Add(result);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"创建测试结果对象失败: {ex.Message}");
            }
        }
        /// <summary>
        /// 初始化通道选择控件
        /// 绑定通道选择事件到通用处理方法
        /// </summary>
        private void InitializeChannelSelection()
        {
            jzchannel1.Checked = true;
            jzchannel1.CheckedChanged += Channel_CheckedChanged;
            jzchannel2.CheckedChanged += Channel_CheckedChanged;
            jzchannel3.CheckedChanged += Channel_CheckedChanged;
            jzchannel4.CheckedChanged += Channel_CheckedChanged;
            jzchannel5.CheckedChanged += Channel_CheckedChanged;
            jzchannel6.CheckedChanged += Channel_CheckedChanged;
            jzchannel7.CheckedChanged += Channel_CheckedChanged;
            jzchannel8.CheckedChanged += Channel_CheckedChanged;
        }

        /// <summary>
        /// 获取当前选中的通道号
        /// </summary>
        /// <returns>当前选中的通道号（1-5）</returns>
        private int GetCurrentSelectedChannel()
        {
            if (jzchannel1.Checked) return 1;
            if (jzchannel2.Checked) return 2;
            if (jzchannel3.Checked) return 3;
            if (jzchannel4.Checked) return 4;
            if (jzchannel5.Checked) return 5;
            if (jzchannel6.Checked) return 6;
            if (jzchannel7.Checked) return 7;
            if (jzchannel8.Checked) return 8;
            return 1; // 默认返回1通道
        }

        /// <summary>
        /// 获取天线模式下的通道锁定代码
        /// </summary>
        /// <param name="channel">UI通道号(1-8)</param>
        /// <param name="channelMode">通道模式(CH5或CH8)</param>
        /// <returns>锁定代码(1-7)，0表示无需锁定</returns>
        private int GetChannelLockCode(int channel, string channelMode)
        {
            if (channelMode == "CH5")
            {
                switch (channel)
                {
                    case 1: return 1;  // A
                    case 2: return 2;  // B
                    case 4: return 3;  // D
                    case 5: return 4;  // E
                    default: return 0; // 无效通道
                }
            }
            else // CH8
            {
                switch (channel)
                {
                    case 1: return 1;  // A
                    case 2: return 2;  // B
                    case 3: return 3;  // C
                    case 5: return 4;  // E
                    case 6: return 5;  // F
                    case 7: return 6;  // G
                    case 8: return 7;  // H
                    default: return 0; // 无效通道
                }
            }
        }

        /// <summary>
        /// 初始化校准UI控件
        /// </summary>
        private void InitializeCalibrationUI()
        {
            // 初始化校准UI更新器的控件引用
            _calibrationUIUpdater.InitializeControls(
                txtCalDacHigh,
                txtCalDacLow,
                txtCalXndHigh,
                txtCalXndLow,
                lblAddrDacHigh,
                lblAddrDacLow,
                lblAddrXndHigh,
                lblAddrXndLow,
                comboBoxtemp,
                labeltemp);

            // 绑定温度范围下拉框选择事件
            comboBoxtemp.SelectedIndexChanged += comboBoxtemp_SelectedIndexChanged;
        }

        /// <summary>
        /// 初始化控件事件绑定
        /// </summary>
        private void InitializeControls()
        {
            // 绑定打开/关闭温度传感器按钮事件
            openTempSerialPort.Click += openTempSerialPort_Click;

            // 绑定全部读取按钮事件
            AllRead.Click += AllRead_Click;

            // 绑定自动更新复选框事件
            checkBoxAuto.CheckedChanged += checkBoxAuto_CheckedChanged;

            // 绑定写按钮事件
            write1.Click += write1_Click;
            write2.Click += write2_Click;
            write3.Click += write3_Click;
            write4.Click += write4_Click;

            // 绑定模式选择控件事件
            normal.CheckedChanged += Mode_CheckedChanged;
            antenna.CheckedChanged += Mode_CheckedChanged;
            
            // 绑定模式选择下拉框事件
            modechance.SelectedIndexChanged += ModeChance_SelectedIndexChanged;

            // 设置normal控件默认选中状态
            normal.Checked = true;

            // 初始化温度串口下拉菜单
            TempSerialPort.Items.AddRange(SerialPort.GetPortNames());
            // 查找并选择特定串口（例如 COM9）
            int targetPortIndex = TempSerialPort.Items.IndexOf("COM9");
            if (targetPortIndex >= 0)
            {
                TempSerialPort.SelectedIndex = targetPortIndex;
            }
            else if (TempSerialPort.Items.Count > 0)
            {
                TempSerialPort.SelectedIndex = 0;
            }

            // 初始化SwitchPower按钮
            SwitchPower.BackColor = Color.LightGreen;
            SwitchPower.Text = "开启供电";
            SwitchPower.FlatStyle = FlatStyle.Flat;
            SwitchPower.FlatAppearance.BorderSize = 0;
            SwitchPower.Click += SwitchPower_Click;

            // 绑定DAC High上下箭头按钮的长按事件
            btnUpDacHigh.MouseDown += (s, e) => StartLongPress(txtCalDacHigh, 1);
            btnUpDacHigh.MouseUp += (s, e) => StopLongPress();
            btnUpDacHigh.MouseLeave += (s, e) => StopLongPress();
            btnDownDacHigh.MouseDown += (s, e) => StartLongPress(txtCalDacHigh, -1);
            btnDownDacHigh.MouseUp += (s, e) => StopLongPress();
            btnDownDacHigh.MouseLeave += (s, e) => StopLongPress();

            // 绑定DAC Low上下箭头按钮的长按事件
            btnUpDacLow.MouseDown += (s, e) => StartLongPress(txtCalDacLow, 16);
            btnUpDacLow.MouseUp += (s, e) => StopLongPress();
            btnUpDacLow.MouseLeave += (s, e) => StopLongPress();
            btnDownDacLow.MouseDown += (s, e) => StartLongPress(txtCalDacLow, -16);
            btnDownDacLow.MouseUp += (s, e) => StopLongPress();
            btnDownDacLow.MouseLeave += (s, e) => StopLongPress();

            // 绑定XND High上下箭头按钮的长按事件
            btnUpXndHigh.MouseDown += (s, e) => StartLongPress(txtCalXndHigh, 1);
            btnUpXndHigh.MouseUp += (s, e) => StopLongPress();
            btnUpXndHigh.MouseLeave += (s, e) => StopLongPress();
            btnDownXndHigh.MouseDown += (s, e) => StartLongPress(txtCalXndHigh, -1);
            btnDownXndHigh.MouseUp += (s, e) => StopLongPress();
            btnDownXndHigh.MouseLeave += (s, e) => StopLongPress();

            // 绑定XND Low上下箭头按钮的长按事件
            btnUpXndLow.MouseDown += (s, e) => StartLongPress(txtCalXndLow, 1);
            btnUpXndLow.MouseUp += (s, e) => StopLongPress();
            btnUpXndLow.MouseLeave += (s, e) => StopLongPress();
            btnDownXndLow.MouseDown += (s, e) => StartLongPress(txtCalXndLow, -1);
            btnDownXndLow.MouseUp += (s, e) => StopLongPress();
            btnDownXndLow.MouseLeave += (s, e) => StopLongPress();
        }

        /// <summary>
        /// 初始化频率选择按钮，绑定点击事件到通用处理方法
        /// </summary>
        private void InitializeFrequencyButtons()
        {
            // 设置默认选中状态
            button3330.Checked = true;
            
            // 绑定频率选择事件到通用处理方法
            button3330.Click += FrequencyButton_Click;
            button3350.Click += FrequencyButton_Click;
            button3370.Click += FrequencyButton_Click;
            button3390.Click += FrequencyButton_Click;
        }

        /// <summary>
        /// 频率选择按钮通用事件处理方法
        /// 根据点击的按钮确定频率索引
        /// </summary>
        private void FrequencyButton_Click(object sender, EventArgs e)
        {
            int frequencyIndex = 1;
            
            // 根据按钮确定频率索引
            if (sender == button3330)
            {
                frequencyIndex = 1;
            }
            else if (sender == button3350)
            {
                frequencyIndex = 2;
            }
            else if (sender == button3370)
            {
                frequencyIndex = 3;
            }
            else if (sender == button3390)
            {
                frequencyIndex = 4;
            }
            
            // 更新频率索引
            _frequencyIndex = frequencyIndex;
            
            // 获取对应的频率值
            double frequencyMHz = _calibrationLogic.GetFrequencyValue(frequencyIndex);
            
            try
            {
                // 向工装板发送频率切换命令
                _instrumentManager.SendFrequencyCommand(frequencyMHz);

                // 向信号源发送频率切换命令
                _instrumentManager.SignalGenerator.SetFrequency(frequencyMHz);

                // 计算并更新校准地址
                CalculateAddresses();
                UpdateCalibrationControls();

                LogMessage($"成功切换到频率：{frequencyMHz} MHz");
            }
            catch (Exception ex)
            {
                string errorMsg = $"频率切换失败：{ex.Message}";
                LogMessage(errorMsg);
                MessageBox.Show(errorMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 温度更新事件处理，当收到新的温度数据时触发
        /// </summary>
        /// <param name="temperature">最新温度值</param>
        private void OnTemperatureUpdated(float temperature)
        {
            // 确保在UI线程中更新温度显示
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<float>(OnTemperatureUpdated), temperature);
                return;
            }

            // 更新当前温度存储
            _currentTemperature = temperature;

            // 更新温度显示标签
            UpdateTemperatureDisplay(temperature);

            // 如果开启了自动更新，则根据温度值自动选择温度范围
            if (_autoUpdateTemp)
            {
                UpdateTempComboBoxByTemperature(temperature);
            }
        }

        /// <summary>
        /// 日志事件处理，将温度串口的日志信息显示到jztextlog控件上
        /// </summary>
        /// <param name="message">日志消息内容</param>
        private void OnLogMessage(string message)
        {
            // 确保在UI线程中更新日志控件
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(OnLogMessage), message);
                return;
            }

            // 检查日志控件是否已被释放，避免ObjectDisposedException
            if (jztextlog != null && !jztextlog.IsDisposed && jztextlog.IsHandleCreated)
            {
                // 将日志信息追加到jztextlog控件
                jztextlog.AppendText(message + Environment.NewLine);
                jztextlog.ScrollToCaret();
            }
        }

        /// <summary>
        /// 记录日志到校准界面的日志控件上
        /// </summary>
        /// <param name="message">日志信息</param>
        private void LogMessage(string message)
        {
            // 确保在UI线程中更新日志控件
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(LogMessage), message);
                return;
            }

            // 检查日志控件是否已被释放，避免ObjectDisposedException
            if (jztextlog != null && !jztextlog.IsDisposed && jztextlog.IsHandleCreated)
            {
                // 将日志信息追加到jztextlog控件
                jztextlog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
                jztextlog.ScrollToCaret();
            }
        }

        /// <summary>
        /// 更新温度显示标签
        /// </summary>
        /// <param name="temperature">温度值</param>
        private void UpdateTemperatureDisplay(float temperature)
        {
            labeltemp.Text = $"温度: {temperature:F2} ℃";
        }

        /// <summary>
        /// 根据当前温度自动更新温度范围下拉框的选中项
        /// </summary>
        /// <param name="temperature">当前温度值</param>
        private void UpdateTempComboBoxByTemperature(float temperature)
        {
            if (comboBoxtemp.InvokeRequired)
            {
                comboBoxtemp.Invoke(new Action<float>(UpdateTempComboBoxByTemperature), temperature);
                return;
            }

            string rangeToSelect = _calibrationLogic.GetTemperatureRange(temperature);

            for (int i = 0; i < comboBoxtemp.Items.Count; i++)
            {
                if (comboBoxtemp.Items[i]?.ToString() == rangeToSelect)
                {
                    comboBoxtemp.SelectedIndex = i;
                    OnTemperatureSelected(comboBoxtemp.SelectedIndex);
                    break;
                }
            }
        }

        /// <summary>
        /// 根据当前选择的频率和温度索引计算校准地址
        /// 调用CalibrationAddressCalculator计算地址
        /// </summary>
        private void CalculateNormalAddresses()
        {
            if (_channelMode == "CH5")
            {
                // 五通道模式：使用五通道计算器
                _calibrationLogic.CalculateNormalAddresses(_frequencyIndex, _temperatureIndex);
            }
            else if (_channelMode == "CH8")
            {
                // 八通道模式：使用八通道计算器
                _addressCalculator8.CalculateNormalAddresses(_frequencyIndex, _temperatureIndex);
            }
        }

        /// <summary>
        /// 更新校准相关控件的显示，包括DAC和XND地址
        /// 根据当前选中的通道显示对应的校准地址
        /// 只有当地址显示控件发生变化时，才更新对应文本框的值
        /// </summary>
        /// <param name="forceUpdate">是否强制更新所有控件</param>
        private void UpdateCalibrationControls(bool forceUpdate = false)
        {
            _calibrationUIUpdater.UpdateCalibrationControls(_currentChannel, forceUpdate);
        }

        /// <summary>
        /// 温度范围选择事件处理，更新温度索引并重新计算校准地址
        /// </summary>
        /// <param name="comboBoxIndex">下拉框选中索引</param>
        private void OnTemperatureSelected(int comboBoxIndex)
        {
            if (comboBoxIndex == 0)
                _temperatureIndex = 1;
            else if (comboBoxIndex == 1)
                _temperatureIndex = 2;
            else
                _temperatureIndex = 2 * comboBoxIndex;

            CalculateAddresses();
            UpdateCalibrationControls();
        }

        /// <summary>
        /// 窗体关闭事件处理，释放温度串口资源
        /// </summary>
        private void FormCalibration_FormClosing(object sender, FormClosingEventArgs e)
        {
            _initialDelayTimer?.Stop();
            _initialDelayTimer?.Dispose();
            _repeatTimer?.Stop();
            _repeatTimer?.Dispose();

            _instrumentManager.PowerStateChanged -= OnPowerStateChanged;
            _temperatureSerialPortManager?.Dispose();
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
            if (SwitchPower != null)
            {
                if (_isPowerOn)
                {
                    SwitchPower.BackColor = Color.LightCoral;
                    SwitchPower.Text = "关闭供电";
                }
                else
                {
                    SwitchPower.BackColor = Color.LightGreen;
                    SwitchPower.Text = "开启供电";
                }
            }
        }

        /// <summary>
        /// 打开/关闭温度串口按钮点击事件
        /// 点击"打开温度串口"后直接开始接收温度数据，无需额外操作
        /// </summary>
        private void openTempSerialPort_Click(object sender, EventArgs e)
        {
            string tempPort = TempSerialPort.SelectedItem?.ToString() ?? "";

            if (string.IsNullOrEmpty(tempPort))
            {
                MessageBox.Show("请选择温度传感器串口", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (openTempSerialPort.Text == "打开温度串口")
            {
                // 打开温度串口，打开后自动接收温度数据
                bool success = _temperatureSerialPortManager.OpenPort(tempPort);
                if (success)
                {
                    openTempSerialPort.Text = "关闭温度串口";
                    
                    // 开启串口时，自动选中自动更新复选框
                    checkBoxAuto.Checked = true;
                }
                else
                {
                    MessageBox.Show("打开温度串口失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // 关闭温度串口，停止接收温度数据
                _temperatureSerialPortManager.ClosePort();
                openTempSerialPort.Text = "打开温度串口";
            }
        }

        /// <summary>
        /// 温度范围下拉框选择改变事件
        /// </summary>
        private void comboBoxtemp_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxtemp.SelectedIndex >= 0)
            {
                OnTemperatureSelected(comboBoxtemp.SelectedIndex);
            }
        }
        
        /// <summary>
        /// 模式选择控件事件处理程序
        /// </summary>
        private async void Mode_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is RadioButton radioButton && radioButton.Checked)
            {
                if (radioButton == normal)
                {
                    _currentMode = "normal";
                    await _instrumentManager.SendNormalModeCommandsAsync();
                }
                else if (radioButton == antenna)
                {
                    _currentMode = "antenna";
                    await _instrumentManager.SendAntennaModeCommandsAsync();
                }
                
                // 更新 CalibrationUIUpdater 的模式状态
                _calibrationUIUpdater.SetCurrentMode(_currentMode);
                
                // 根据新的模式重新计算地址
                CalculateAddresses();
                
                // 更新 UI 控件显示
                UpdateCalibrationControls();
                
                LogMessage($"成功切换到模式：{_currentMode}");
            }
        }
        
        /// <summary>
        /// 模式选择下拉框事件处理程序
        /// 处理五通道(CH5)和八通道(CH8)模式切换
        /// </summary>
        private void ModeChance_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (modechance.SelectedItem == null) return;
            
            string selectedMode = modechance.SelectedItem.ToString();
            
            if (selectedMode == "CH5")
            {
                // 五通道模式：禁用通道6、7、8
                jzchannel6.Enabled = false;
                jzchannel7.Enabled = false;
                jzchannel8.Enabled = false;
                
                // 如果当前选中的是通道6、7或8，则自动切换到通道1
                if (jzchannel6.Checked || jzchannel7.Checked || jzchannel8.Checked)
                {
                    jzchannel1.Checked = true;
                }
                
                // 更新通道模式
                _channelMode = "CH5";
                
                // 更新UI更新器的通道模式
                _calibrationUIUpdater.SetChannelMode("CH5");
                
                // 重新创建验证服务实例
                _verificationService = new VerificationService(
                    _instrumentManager,
                    this,
                    AddCalibrationResult,
                    LogMessage,
                    _channelMode);
                
                LogMessage("已切换到五通道模式(CH5)，通道6、7、8已禁用");
                
                // 重新计算地址
                CalculateAddresses();
            }
            else if (selectedMode == "CH8")
            {
                // 八通道模式：启用所有通道
                jzchannel6.Enabled = true;
                jzchannel7.Enabled = true;
                jzchannel8.Enabled = true;
                
                // 更新通道模式
                _channelMode = "CH8";
                
                // 更新UI更新器的通道模式
                _calibrationUIUpdater.SetChannelMode("CH8");
                
                // 重新创建验证服务实例
                _verificationService = new VerificationService(
                    _instrumentManager,
                    this,
                    AddCalibrationResult,
                    LogMessage,
                    _channelMode);
                
                LogMessage("已切换到八通道模式(CH8)，所有通道已启用");
                
                // 重新计算地址
                CalculateAddresses();
            }
        }
        
        /// <summary>
        /// 根据当前模式计算地址
        /// </summary>
        private void CalculateAddresses()
        {
            if (_channelMode == "CH5")
            {
                // 五通道模式：使用现有计算器
                if (_currentMode == "normal")
                {
                    CalculateNormalAddresses();
                }
                else if (_currentMode == "antenna")
                {
                    _calibrationLogic.CalculateAntennaAddresses(_frequencyIndex, _temperatureIndex);
                }
            }
            else if (_channelMode == "CH8")
            {
                // 八通道模式：使用八通道计算器
                if (_currentMode == "normal")
                {
                    _addressCalculator8.CalculateNormalAddresses(_frequencyIndex, _temperatureIndex);
                }
                else if (_currentMode == "antenna")
                {
                    _addressCalculator8.CalculateAntennaAddresses(_frequencyIndex, _temperatureIndex);
                }
            }
        }

        /// <summary>
        /// 自动更新温度复选框状态改变事件
        /// </summary>
        private void checkBoxAuto_CheckedChanged(object sender, EventArgs e)
        {
            _autoUpdateTemp = checkBoxAuto.Checked;
        }

        /// <summary>
        /// 全部读取按钮点击事件，读取当前通道所有频点的校准参数
        /// </summary>
        private void AllRead_Click(object sender, EventArgs e)
        {
            AllRead.Enabled = false;
            LogMessage("开始读取当前通道所有频点校准参数...");

            System.Threading.Tasks.Task.Run(() =>
            {
                int totalReadCount = 0;
                int originalFreqIndex = _frequencyIndex;
                int originalChannel = _currentChannel;

                try
                {
                    _temperatureSerialPortManager.StopTemperatureReport();
                    System.Threading.Thread.Sleep(50);

                    _temperatureSerialPortManager.EnterROMMode();
                    System.Threading.Thread.Sleep(50);

                    string currentMode = _calibrationUIUpdater.GetCurrentMode();
                    LogMessage($"当前模式: {currentMode}, 通道: {originalChannel}");

                    for (int freqIndex = 1; freqIndex <= 4; freqIndex++)
                    {
                        LogMessage($"读取频点: {freqIndex}...");

                        _frequencyIndex = freqIndex;
                        _calibrationUIUpdater.SetCurrentChannel(originalChannel);
                        if (_channelMode == "CH5")
                        {
                            if (_currentMode == "normal")
                            {
                                _calibrationLogic.CalculateNormalAddresses(_frequencyIndex, _temperatureIndex);
                            }
                            else if (_currentMode == "antenna")
                            {
                                _calibrationLogic.CalculateAntennaAddresses(_frequencyIndex, _temperatureIndex);
                            }
                        }
                        else if (_channelMode == "CH8")
                        {
                            if (_currentMode == "normal")
                            {
                                _addressCalculator8.CalculateNormalAddresses(_frequencyIndex, _temperatureIndex);
                            }
                            else if (_currentMode == "antenna")
                            {
                                _addressCalculator8.CalculateAntennaAddresses(_frequencyIndex, _temperatureIndex);
                            }
                        }

                        ushort addrDacHigh = _calibrationUIUpdater.GetCurrentAddressForControl(txtCalDacHigh);
                        _temperatureSerialPortManager.ReadEEPROM(addrDacHigh);
                        System.Threading.Thread.Sleep(50);

                        if (currentMode != "antenna")
                        {
                            ushort addrDacLow = _calibrationUIUpdater.GetCurrentAddressForControl(txtCalDacLow);
                            _temperatureSerialPortManager.ReadEEPROM(addrDacLow);
                            System.Threading.Thread.Sleep(50);
                            totalReadCount++;
                        }

                        ushort addrXndHigh = _calibrationUIUpdater.GetCurrentAddressForControl(txtCalXndHigh);
                        _temperatureSerialPortManager.ReadEEPROM(addrXndHigh);
                        System.Threading.Thread.Sleep(50);

                        ushort addrXndLow = _calibrationUIUpdater.GetCurrentAddressForControl(txtCalXndLow);
                        _temperatureSerialPortManager.ReadEEPROM(addrXndLow);
                        System.Threading.Thread.Sleep(50);

                        totalReadCount += 3;
                    }

                    LogMessage($"读取完成，共读取 {totalReadCount} 个参数");
                }
                catch (System.Exception ex)
                {
                    LogMessage($"读取失败: {ex.Message}");
                }
                finally
                {
                    _frequencyIndex = originalFreqIndex;
                    _currentChannel = originalChannel;
                    _calibrationUIUpdater.SetCurrentChannel(originalChannel);
                    CalculateNormalAddresses();

                    try
                    {
                        _temperatureSerialPortManager.ExitROMMode();
                        System.Threading.Thread.Sleep(50);
                        _temperatureSerialPortManager.ResumeTemperatureReport();
                    }
                    catch (System.Exception)
                    {
                    }

                    this.Invoke(new System.Action(() =>
                    {
                        UpdateCalibrationControls(true);
                        AllRead.Enabled = true;
                    }));
                }
            });
        }

        /// <summary>
        /// 通道选择单选按钮通用事件处理方法
        /// 根据事件源判断当前通道号，执行通道切换逻辑
        /// </summary>
        private void Channel_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is RadioButton radioButton && radioButton.Checked)
            {
                int channel = 1;
                
                // 根据单选按钮名称判断通道号
                if (radioButton == jzchannel1)
                {
                    channel = 1;
                }
                else if (radioButton == jzchannel2)
                {
                    channel = 2;
                }
                else if (radioButton == jzchannel3)
                {
                    channel = 3;
                }
                else if (radioButton == jzchannel4)
                {
                    channel = 4;
                }
                else if (radioButton == jzchannel5)
                {
                    channel = 5;
                }
                else if (radioButton == jzchannel6)
                {
                    channel = 6;
                }
                else if (radioButton == jzchannel7)
                {
                    channel = 7;
                }
                else if (radioButton == jzchannel8)
                {
                    channel = 8;
                }
                try
                {
                    // 更新当前通道
                    _currentChannel = channel;
                    
                    // 更新CalibrationUIUpdater的当前通道
                    _calibrationUIUpdater.SetCurrentChannel(channel);
                    
                    // 执行通道切换
                    _instrumentManager.SwitchChannel(channel);

                    // 天线模式下发送通道锁定命令
                    if (_currentMode == "antenna")
                    {
                        int lockCode = GetChannelLockCode(channel, _channelMode);
                        if (lockCode > 0)
                        {
                            _instrumentManager.SetChannelLock(lockCode);
                            LogMessage($"天线模式：发送通道锁定命令，锁定代码 {lockCode}");
                        }
                    }
                    
                    // 更新校准控件显示
                    UpdateCalibrationControls();
                    
                    LogMessage($"成功切换到通道 {channel}");
                }
                catch (Exception ex)
                {
                    string errorMsg = $"通道切换失败：{ex.Message}";
                    LogMessage(errorMsg);
                    MessageBox.Show(errorMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// 通用校准参数读取方法
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="targetTextBox">目标文本框控件</param>
        /// <param name="parameterName">参数名称</param>
        private void ReadCalibrationParameter(object sender, TextBox targetTextBox, string parameterName)
        {
            try
            {
                ushort address = _calibrationUIUpdater.GetCurrentAddressForControl(targetTextBox);
                _temperatureSerialPortManager.ReadEEPROM(address);
                LogMessage($"读取{parameterName}校准值命令已发送");
            }
            catch (Exception ex)
            {
                LogMessage($"读取{parameterName}校准值失败: {ex.Message}");
            }
        }

        /// <summary>
        /// read1按钮点击事件，读取txtCalDacHigh对应的校准值
        /// </summary>
        private void read1_Click(object sender, EventArgs e)
        {
            ReadCalibrationParameter(sender, txtCalDacHigh, "DAChigh");
        }

        /// <summary>
        /// read2按钮点击事件，读取txtCalDacLow对应的校准值
        /// </summary>
        private void read2_Click(object sender, EventArgs e)
        {
            ReadCalibrationParameter(sender, txtCalDacLow, "DAClow");
        }

        /// <summary>
        /// read3按钮点击事件，读取txtCalXndHigh对应的校准值
        /// </summary>
        private void read3_Click(object sender, EventArgs e)
        {
            ReadCalibrationParameter(sender, txtCalXndHigh, "XNDhigh");
        }

        /// <summary>
        /// read4按钮点击事件，读取txtCalXndLow对应的校准值
        /// </summary>
        private void read4_Click(object sender, EventArgs e)
        {
            ReadCalibrationParameter(sender, txtCalXndLow, "XNDlow");
        }

        /// <summary>
        /// 通用校准参数写入方法
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="targetTextBox">目标文本框控件</param>
        /// <param name="parameterName">参数名称</param>
        private void WriteCalibrationParameter(object sender, TextBox targetTextBox, string parameterName)
        {
            try
            {
                string hexValue = targetTextBox.Text.Trim();
                if (byte.TryParse(hexValue, System.Globalization.NumberStyles.HexNumber, null, out byte value))
                {
                    ushort address = _calibrationUIUpdater.GetCurrentAddressForControl(targetTextBox);
                    _temperatureSerialPortManager.WriteEEPROM(address, value);
                    _temperatureSerialPortManager.ReadEEPROM(address);
                    LogMessage($"写入{parameterName}校准值成功: 0x{value:X2}");
                    
                    // 仅在normal模式下同时写入36db模式地址
                    if (_currentMode == "normal")
                    {
                        WriteDb36Address(targetTextBox, parameterName, value);
                    }
                }
                else
                {
                    LogMessage($"写入{parameterName}失败 - 无效的16进制值");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"写入{parameterName}校准值失败: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 写入36db模式地址
        /// 在normal模式下，写入校准值时同时写入36db模式对应的地址
        /// </summary>
        /// <param name="targetTextBox">目标文本框控件</param>
        /// <param name="parameterName">参数名称</param>
        /// <param name="value">校准值</param>
        private void WriteDb36Address(TextBox targetTextBox, string parameterName, byte value)
        {
            try
            {
                int addressIndex = 0;
                
                if (_channelMode == "CH5")
                {
                    int startIndex = _calibrationLogic.GetStartIndexByChannel(_currentChannel);
                    
                    if (targetTextBox == txtCalDacHigh)
                        addressIndex = startIndex;
                    else if (targetTextBox == txtCalDacLow)
                        addressIndex = startIndex + 1;
                    else if (targetTextBox == txtCalXndHigh)
                        addressIndex = startIndex + 10;
                    else if (targetTextBox == txtCalXndLow)
                        addressIndex = startIndex + 11;
                    
                    _calibrationLogic.CalculateDb36Addresses(_frequencyIndex, _temperatureIndex);
                    ushort db36Address = (ushort)_calibrationLogic.GetDb36AddressDecimal(addressIndex);
                    
                    if (db36Address > 0)
                    {
                        _temperatureSerialPortManager.WriteEEPROM(db36Address, value);
                        _temperatureSerialPortManager.ReadEEPROM(db36Address);
                        LogMessage($"写入36db[{parameterName}]校准值成功: 0x{value:X2} (地址: 0x{db36Address:X4})");
                    }
                }
                else if (_channelMode == "CH8")
                {
                    int paramType = 0;
                    if (targetTextBox == txtCalDacHigh)
                        paramType = 1;
                    else if (targetTextBox == txtCalDacLow)
                        paramType = 2;
                    else if (targetTextBox == txtCalXndHigh)
                        paramType = 3;
                    else if (targetTextBox == txtCalXndLow)
                        paramType = 4;
                    
                    if (paramType > 0)
                    {
                        addressIndex = _addressCalculator8.GetNormalAddressIndex(_currentChannel, paramType);
                        
                        _addressCalculator8.CalculateDb36Addresses(_frequencyIndex, _temperatureIndex);
                        ushort db36Address = (ushort)_addressCalculator8.GetDb36AddressDecimal(addressIndex);
                        
                        if (db36Address > 0)
                        {
                            _temperatureSerialPortManager.WriteEEPROM(db36Address, value);
                            _temperatureSerialPortManager.ReadEEPROM(db36Address);
                            LogMessage($"写入36db[{parameterName}]校准值成功: 0x{value:X2} (地址: 0x{db36Address:X4})");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"写入36db[{parameterName}]校准值失败: {ex.Message}");
            }
        }

        /// <summary>
        /// write1按钮点击事件，写入txtCalDacHigh对应的校准值
        /// </summary>
        private void write1_Click(object sender, EventArgs e)
        {
            WriteCalibrationParameter(sender, txtCalDacHigh, "DAChigh");
        }

        /// <summary>
        /// write2按钮点击事件，写入txtCalDacLow对应的校准值
        /// </summary>
        private void write2_Click(object sender, EventArgs e)
        {
            WriteCalibrationParameter(sender, txtCalDacLow, "DAClow");
        }

        /// <summary>
        /// write3按钮点击事件，写入txtCalXndHigh对应的校准值
        /// </summary>
        private void write3_Click(object sender, EventArgs e)
        {
            WriteCalibrationParameter(sender, txtCalXndHigh, "XNDhigh");
        }

        /// <summary>
        /// write4按钮点击事件，写入txtCalXndLow对应的校准值
        /// </summary>
        private void write4_Click(object sender, EventArgs e)
        {
            WriteCalibrationParameter(sender, txtCalXndLow, "XNDlow");
        }

        /// <summary>
        /// EEPROM数据接收事件处理方法
        /// 当收到EEPROM响应时触发，更新对应的文本框
        /// </summary>
        /// <param name="address">EEPROM地址</param>
        /// <param name="value">读取到的值</param>
        private void OnEepromDataReceived(ushort address, byte value)
        {
            try
            {
                LogMessage($"收到EEPROM数据 - 地址: 0x{address:X4}, 值: 0x{value:X2}");
                
                // 更新校准数据缓存
                _calibrationLogic.UpdateCalibrationCache(address, value);
                
                // 更新UI控件
                _calibrationUIUpdater.UpdateEEPROMDataToUI(address, value);
            }
            catch (Exception ex)
            {
                LogMessage($"处理EEPROM数据失败: {ex.Message}");
            }
        }

        /// <summary>
        /// DAChigh上箭头按钮点击事件
        /// </summary>
        private void btnUpDacHigh_Click(object sender, EventArgs e)
        {
            _calibrationUIUpdater.AdjustNumericValue(txtCalDacHigh, 1);
        }

        /// <summary>
        /// DAChigh下箭头按钮点击事件
        /// </summary>
        private void btnDownDacHigh_Click(object sender, EventArgs e)
        {
            _calibrationUIUpdater.AdjustNumericValue(txtCalDacHigh, -1);
        }

        /// <summary>
        /// DAClow上箭头按钮点击事件
        /// </summary>
        private void btnUpDacLow_Click(object sender, EventArgs e)
        {
            _calibrationUIUpdater.AdjustNumericValue(txtCalDacLow, 16);
        }

        /// <summary>
        /// DAClow下箭头按钮点击事件
        /// </summary>
        private void btnDownDacLow_Click(object sender, EventArgs e)
        {
            _calibrationUIUpdater.AdjustNumericValue(txtCalDacLow, -16);
        }

        /// <summary>
        /// XND2261high上箭头按钮点击事件
        /// </summary>
        private void btnUpXndHigh_Click(object sender, EventArgs e)
        {
            _calibrationUIUpdater.AdjustNumericValue(txtCalXndHigh, 1);
        }

        /// <summary>
        /// XND2261high下箭头按钮点击事件
        /// </summary>
        private void btnDownXndHigh_Click(object sender, EventArgs e)
        {
            _calibrationUIUpdater.AdjustNumericValue(txtCalXndHigh, -1);
        }

        /// <summary>
        /// XND2261low上箭头按钮点击事件
        /// </summary>
        private void btnUpXndLow_Click(object sender, EventArgs e)
        {
            _calibrationUIUpdater.AdjustNumericValue(txtCalXndLow, 1);
        }

        /// <summary>
        /// XND2261low下箭头按钮点击事件
        /// </summary>
        private void btnDownXndLow_Click(object sender, EventArgs e)
        {
            _calibrationUIUpdater.AdjustNumericValue(txtCalXndLow, -1);
        }

        /// <summary>
        /// SwitchPower按钮点击事件，控制ODP0603电源的开启和关闭
        /// </summary>
        private void SwitchPower_Click(object sender, EventArgs e)
        {
            try
            {
                _isPowerOn = !_isPowerOn;
                
                _instrumentManager.ODP3063.EnableChannel2Output(_isPowerOn);
                _instrumentManager.SetPowerState(_isPowerOn);
                
                if (_isPowerOn)
                {
                    SwitchPower.BackColor = Color.LightCoral;
                    SwitchPower.Text = "关闭供电";
                    LogMessage("ODP0603电源已开启供电");
                }
                else
                {
                    SwitchPower.BackColor = Color.LightGreen;
                    SwitchPower.Text = "开启供电";
                    LogMessage("ODP0603电源已关闭供电");
                }
            }
            catch (Exception ex)
            {
                _isPowerOn = !_isPowerOn;
                LogMessage($"控制ODP0603电源失败: {ex.Message}");
                MessageBox.Show($"控制电源失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 开始验证按钮点击事件
        /// 验证仪器连接状态后启动验证流程
        /// </summary>
        private async void btnStartVerification_Click(object sender, EventArgs e)
        {
            if (dgvCalibrationResults != null)
            {
                dgvCalibrationResults.Rows.Clear();
            }
            
            _verificationResults.Clear();

            startPhase.Enabled = false;

            try
            {
                LogMessage("重置模块状态：切换到3390MHz");
                _instrumentManager.SendFrequencyCommand(3390.0);
                _instrumentManager.SignalGenerator.SetFrequency(3390.0);
                
                LogMessage("重置模块状态：等待0.5秒");
                await Task.Delay(500);
                
                LogMessage("重置模块状态：切换回3330MHz");
                _instrumentManager.SendFrequencyCommand(3330.0);
                _instrumentManager.SignalGenerator.SetFrequency(3330.0);
                
                LogMessage("重置模块状态：等待2秒");
                await Task.Delay(2000);
            }
            catch (Exception ex)
            {
                LogMessage($"重置模块状态失败: {ex.Message}");
            }

            try
            {
                if (_verificationService != null)
                {
                    LogMessage("开始执行验证测试...");
                    await _verificationService.RunVerificationTest();
                    LogMessage("验证测试完成");
                }
            }
            catch (TimeoutException ex)
            {
                LogMessage($"[严重] 测试超时: {ex.Message}");
                MessageBox.Show($"测试超时，请检查仪器连接状态。\n\n详细信息: {ex.Message}", 
                    "测试超时", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (OperationCanceledException ex)
            {
                LogMessage($"[取消] 测试被取消: {ex.Message}");
            }
            catch (Exception ex)
            {
                LogMessage($"测试过程中发生错误: {ex.Message}");
                MessageBox.Show($"测试过程中发生错误:\n{ex.Message}", 
                    "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                startPhase.Enabled = true;
                
                if (_isUlw2Mode && _verificationResults.Count > 0)
                {
                    LogMessage("ulw2模式：开始自动导出验证结果...");
                    ExportVerificationResults();
                }
            }
        }

        /// <summary>
        /// ulw2按钮点击事件，实现ulw2模式切换功能
        /// </summary>
        private void ultraWork2_Click(object sender, EventArgs e)
        {
            try
            {
                // 切换ulw2模式状态
                _isUlw2Mode = !_isUlw2Mode;
                
                if (_isUlw2Mode)
                {
                    // 进入ulw2模式，改变窗体边框颜色为红色
                    this.FormBorderStyle = FormBorderStyle.Fixed3D;
                    this.BackColor = Color.FromArgb(255, 240, 240); // 浅红色背景作为提示
                    LogMessage("已进入ulw2模式，验证测试完成后将自动导出结果");
                }
                else
                {
                    // 退出ulw2模式，恢复默认颜色
                    this.FormBorderStyle = FormBorderStyle.Fixed3D;
                    this.BackColor = SystemColors.Control;
                    LogMessage("已退出ulw2模式");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"ulw2模式切换失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 常温测试菜单点击事件，关闭当前校准窗体
        /// </summary>
        private void 常温测试ToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 导出验证测试结果到Excel
        /// </summary>
        private void ExportVerificationResults()
        {
            if (_verificationResults.Count == 0)
            {
                LogMessage("没有验证测试结果可导出");
                return;
            }

            try
            {
                if (_isUlw2Mode)
                {
                    // ulw模式：直接导出到桌面，创建新Excel文件
                    LogMessage("ulw模式：开始直接导出验证结果到桌面");
                    CreateNewExcelFile();
                }
                else
                {
                    // 非ulw模式：保持原有逻辑
                    using var dialog = new SaveFileDialog();
                    dialog.Filter = "Excel文件|*.xlsx";
                    dialog.Title = "导出验证测试结果";
                    dialog.FileName = $"验证测试结果_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

                    if (dialog.ShowDialog() != DialogResult.OK)
                        return;

                    // 使用与主窗体相同的模板路径
                    var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "测试报告模板.xlsx");

                    // 检查模板文件是否存在
                    if (!File.Exists(templatePath))
                    {
                        LogMessage($"模板文件不存在: {templatePath}");
                        MessageBox.Show($"模板文件不存在: {templatePath}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var service = new ExcelExportService();
                    if (service.ExportToExcel(templatePath, dialog.FileName, _verificationResults))
                    {
                        LogMessage("验证测试结果导出成功");
                        MessageBox.Show("导出成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        LogMessage("验证测试结果导出失败");
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"导出过程中发生错误: {ex.Message}");
                if (!_isUlw2Mode)
                {
                    MessageBox.Show($"导出失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// 直接创建新的Excel文件或在现有文件中追加验证测试结果
        /// ulw模式下使用，不弹出提示框，直接导出到桌面
        /// </summary>
        private void CreateNewExcelFile()
        {
            try
            {
                // 获取桌面路径
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string fileName = "验证测试结果.xlsx";
                string outputPath = Path.Combine(desktopPath, fileName);

                LogMessage($"准备导出到桌面: {outputPath}");

                // 设置EPPlus许可证
                OfficeOpenXml.ExcelPackage.License.SetNonCommercialPersonal("Test User");

                OfficeOpenXml.ExcelPackage package;
                OfficeOpenXml.ExcelWorksheet worksheet;
                int startColumn = 1;

                if (System.IO.File.Exists(outputPath))
                {
                    // 打开现有文件
                    LogMessage("找到现有文件，准备在右侧追加数据");
                    var fileInfo = new System.IO.FileInfo(outputPath);
                    package = new OfficeOpenXml.ExcelPackage(fileInfo);
                    worksheet = package.Workbook.Worksheets["验证测试结果"];
                    
                    // 找到现有数据的最右列
                    if (worksheet.Dimension != null)
                    {
                        startColumn = worksheet.Dimension.End.Column + 1;
                        LogMessage($"现有数据最右列: {startColumn - 1}，将从列 {startColumn} 开始添加新数据");
                    }
                }
                else
                {
                    // 创建新文件
                    LogMessage("未找到现有文件，准备创建新文件");
                    package = new OfficeOpenXml.ExcelPackage();
                    worksheet = package.Workbook.Worksheets.Add("验证测试结果");
                }

                using (package)
                {
                    // 设置表头（新文件或需要新表头时）
                    if (startColumn == 1)
                    {
                        worksheet.Cells["A1"].Value = "测试值(dBm)";
                        worksheet.Cells["B1"].Value = "测试时间(温度)";

                        // 设置表头样式（加粗、居中）
                        using var headerRange = worksheet.Cells["A1:B1"];
                        headerRange.Style.Font.Bold = true;
                        headerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        headerRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        headerRange.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    }
                    else
                    {
                        // 在现有文件中添加新的表头
                        worksheet.Cells[1, startColumn].Value = "测试值(dBm)";
                        worksheet.Cells[1, startColumn + 1].Value = "测试时间(温度)";

                        // 设置新表头样式（加粗、居中）
                        using var headerRange = worksheet.Cells[1, startColumn, 1, startColumn + 1];
                        headerRange.Style.Font.Bold = true;
                        headerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        headerRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        headerRange.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    }

                    // 填充数据
                    int row = 2;
                    foreach (var result in _verificationResults)
                    {
                        // 只导出测试值和测试时间（包含温度）
                        worksheet.Cells[row, startColumn].Value = result.Value;
                        
                        // 简化测试时间，不显示年份，格式：MM-dd HH:mm:ss 温度℃
                        string timeFormat = $"{result.TestTime.ToString("MM-dd HH:mm:ss")} {_currentTemperature:F1}℃";
                        worksheet.Cells[row, startColumn + 1].Value = timeFormat;
                        
                        // 设置数据单元格居中显示
                        worksheet.Cells[row, startColumn].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[row, startColumn].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        worksheet.Cells[row, startColumn + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[row, startColumn + 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        
                        row++;
                    }

                    // 自动调整列宽
                    worksheet.Cells[1, startColumn, row - 1, startColumn + 1].AutoFitColumns();

                    // 保存文件
                    if (System.IO.File.Exists(outputPath))
                    {
                        package.Save();
                    }
                    else
                    {
                        package.SaveAs(new System.IO.FileInfo(outputPath));
                    }

                    LogMessage($"验证测试结果已成功导出到桌面: {fileName}");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"创建Excel文件失败: {ex.Message}");
            }
        }

        private void StartLongPress(TextBox targetTextBox, int direction)
        {
            _targetTextBox = targetTextBox;
            _repeatDirection = direction;
            _isLongPress = false;
            _initialDelayTimer.Start();
        }

        private void StopLongPress()
        {
            _initialDelayTimer.Stop();
            _repeatTimer.Stop();
            _isLongPress = false;
        }

        private void OnInitialDelayTimerTick(object? sender, EventArgs e)
        {
            _initialDelayTimer.Stop();
            _isLongPress = true;
            _repeatTimer.Start();
        }

        private void OnRepeatTimerTick(object? sender, EventArgs e)
        {
            if (_targetTextBox != null)
            {
                _calibrationUIUpdater.AdjustNumericValue(_targetTextBox, _repeatDirection);
            }
        }
    }
}
