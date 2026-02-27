using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace 五通道自动测试.Calibration
{
    /// <summary>
    /// 校准UI更新器，负责处理UI更新和控件管理
    /// </summary>
    public class CalibrationUIUpdater
    {
        private readonly Form _form;
        private readonly Action<string> _logMessageCallback;
        private readonly CalibrationLogic _calibrationLogic;
        private readonly CalibrationAddressCalculator8 _addressCalculator8;

        // 控件引用
        private TextBox _txtCalDacHigh;
        private TextBox _txtCalDacLow;
        private TextBox _txtCalXndHigh;
        private TextBox _txtCalXndLow;
        private Label _lblAddrDacHigh;
        private Label _lblAddrDacLow;
        private Label _lblAddrXndHigh;
        private Label _lblAddrXndLow;
        private ComboBox _comboBoxtemp;
        private Label _labeltemp;

        // DACLow相关控件（用于天线模式禁用）
        private Button _btnUpDacLow;
        private Button _btnDownDacLow;
        private Label _lblDacLow;
        private Button _read2;
        private Button _write2;

        // 地址缓存，用于检测地址变化
        private readonly Dictionary<Control, string> _addressCache;
        
        // 当前通道
        private int _currentChannel = 1;
        
        // 当前模式：normal 或 antenna
        private string _currentMode = "normal";
        
        // 当前通道模式：CH5 或 CH8
        private string _channelMode = "CH5";

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="form">主窗体</param>
        /// <param name="logMessageCallback">日志回调</param>
        /// <param name="calibrationLogic">校准逻辑实例</param>
        /// <param name="addressCalculator8">八通道地址计算器实例</param>
        public CalibrationUIUpdater(
            Form form,
            Action<string> logMessageCallback,
            CalibrationLogic calibrationLogic,
            CalibrationAddressCalculator8 addressCalculator8)
        {
            _form = form;
            _logMessageCallback = logMessageCallback;
            _calibrationLogic = calibrationLogic;
            _addressCalculator8 = addressCalculator8;
            _addressCache = new Dictionary<Control, string>();
            _currentChannel = 1;
        }

        /// <summary>
        /// 设置当前通道
        /// </summary>
        /// <param name="channel">通道号</param>
        public void SetCurrentChannel(int channel)
        {
            _currentChannel = channel;
        }

        /// <summary>
        /// 获取当前通道
        /// </summary>
        /// <returns>当前通道号</returns>
        public int GetCurrentChannel()
        {
            return _currentChannel;
        }
        
        /// <summary>
        /// 设置当前模式
        /// </summary>
        /// <param name="mode">模式名称：normal 或 antenna</param>
        public void SetCurrentMode(string mode)
        {
            _currentMode = mode;
        }
        
        /// <summary>
        /// 获取当前模式
        /// </summary>
        /// <returns>当前模式名称</returns>
        public string GetCurrentMode()
        {
            return _currentMode;
        }
        
        /// <summary>
        /// 设置通道模式
        /// </summary>
        /// <param name="channelMode">通道模式："CH5" 或 "CH8"</param>
        public void SetChannelMode(string channelMode)
        {
            _channelMode = channelMode;
        }
        
        /// <summary>
        /// 获取当前通道模式
        /// </summary>
        /// <returns>当前通道模式</returns>
        public string GetChannelMode()
        {
            return _channelMode;
        }

        /// <summary>
        /// 初始化控件引用
        /// </summary>
        /// <param name="txtCalDacHigh">DAC高位文本框</param>
        /// <param name="txtCalDacLow">DAC低位文本框</param>
        /// <param name="txtCalXndHigh">XND高位文本框</param>
        /// <param name="txtCalXndLow">XND低位文本框</param>
        /// <param name="lblAddrDacHigh">DAC高位地址标签</param>
        /// <param name="lblAddrDacLow">DAC低位地址标签</param>
        /// <param name="lblAddrXndHigh">XND高位地址标签</param>
        /// <param name="lblAddrXndLow">XND低位地址标签</param>
        /// <param name="comboBoxtemp">温度范围下拉框</param>
        /// <param name="labeltemp">温度显示标签</param>
        /// <param name="btnUpDacLow">DACLow向上按钮</param>
        /// <param name="btnDownDacLow">DACLow向下按钮</param>
        /// <param name="lblDacLow">DACLow标签</param>
        /// <param name="read2">读取按钮2</param>
        /// <param name="write2">写入按钮2</param>
        public void InitializeControls(
            TextBox txtCalDacHigh,
            TextBox txtCalDacLow,
            TextBox txtCalXndHigh,
            TextBox txtCalXndLow,
            Label lblAddrDacHigh,
            Label lblAddrDacLow,
            Label lblAddrXndHigh,
            Label lblAddrXndLow,
            ComboBox comboBoxtemp,
            Label labeltemp,
            Button btnUpDacLow,
            Button btnDownDacLow,
            Label lblDacLow,
            Button read2,
            Button write2)
        {
            _txtCalDacHigh = txtCalDacHigh;
            _txtCalDacLow = txtCalDacLow;
            _txtCalXndHigh = txtCalXndHigh;
            _txtCalXndLow = txtCalXndLow;
            _lblAddrDacHigh = lblAddrDacHigh;
            _lblAddrDacLow = lblAddrDacLow;
            _lblAddrXndHigh = lblAddrXndHigh;
            _lblAddrXndLow = lblAddrXndLow;
            _comboBoxtemp = comboBoxtemp;
            _labeltemp = labeltemp;

            _btnUpDacLow = btnUpDacLow;
            _btnDownDacLow = btnDownDacLow;
            _lblDacLow = lblDacLow;
            _read2 = read2;
            _write2 = write2;

            // 初始化温度范围下拉框
            InitializeComboBoxTemp();
        }

        /// <summary>
        /// 初始化温度范围下拉框
        /// </summary>
        private void InitializeComboBoxTemp()
        {
            if (_comboBoxtemp != null)
            {
                _comboBoxtemp.Items.AddRange(CalibrationLogic.TemperatureRanges);
                if (_comboBoxtemp.Items.Count > 0)
                {
                    _comboBoxtemp.SelectedIndex = 0;
                }
            }
        }

        /// <summary>
        /// 更新温度显示
        /// </summary>
        /// <param name="temperature">温度值</param>
        public void UpdateTemperatureDisplay(float temperature)
        {
            if (_labeltemp != null)
            {
                _labeltemp.Text = $"温度: {temperature:F2} ℃";
            }
        }

        /// <summary>
        /// 根据温度自动更新温度范围下拉框
        /// </summary>
        /// <param name="temperature">温度值</param>
        public void UpdateTempComboBoxByTemperature(float temperature)
        {
            if (_comboBoxtemp == null) return;

            if (_comboBoxtemp.InvokeRequired)
            {
                _comboBoxtemp.Invoke(new Action<float>(UpdateTempComboBoxByTemperature), temperature);
                return;
            }

            string rangeToSelect = _calibrationLogic.GetTemperatureRange(temperature);

            for (int i = 0; i < _comboBoxtemp.Items.Count; i++)
            {
                if (_comboBoxtemp.Items[i]?.ToString() == rangeToSelect)
                {
                    _comboBoxtemp.SelectedIndex = i;
                    break;
                }
            }
        }

        /// <summary>
        /// 更新校准控件显示
        /// </summary>
        /// <param name="currentChannel">当前通道</param>
        /// <param name="forceUpdate">是否强制更新所有控件</param>
        public void UpdateCalibrationControls(int currentChannel, bool forceUpdate = false)
        {
            if (_form.InvokeRequired)
            {
                _form.Invoke(new Action<int, bool>(UpdateCalibrationControls), currentChannel, forceUpdate);
                return;
            }

            // 存储当前通道号
            _currentChannel = currentChannel;

            string addrDacHighStr = "0x0000";
            string addrDacLowStr = "0x0000";
            string addrXndHighStr = "0x0000";
            string addrXndLowStr = "0x0000";

            if (_channelMode == "CH5")
            {
                // 五通道模式：使用现有逻辑
                if (_currentMode == "normal")
                {
                    int startIndex = _calibrationLogic.GetStartIndexByChannel(currentChannel);

                    if (startIndex >= 1 && startIndex <= 9)
                    {
                        // 计算新的地址字符串
                        addrDacHighStr = _calibrationLogic.GetNormalAddress(startIndex);
                        addrDacLowStr = _calibrationLogic.GetNormalAddress(startIndex + 1);
                        addrXndHighStr = _calibrationLogic.GetNormalAddress(startIndex + 10);
                        addrXndLowStr = _calibrationLogic.GetNormalAddress(startIndex + 11);
                    }
                }
                else if (_currentMode == "antenna")
                {
                    // 天线模式下，通道对应不同的地址索引
                    if (currentChannel >= 1 && currentChannel <= 5)
                    {
                        int dacHighIndex = 0;
                        int xndHighIndex = 0;
                        int xndLowIndex = 0;
                        
                        switch (currentChannel)
                        {
                            case 3:
                                    dacHighIndex = 1;
                                    xndHighIndex = 6;
                                    xndLowIndex = 7;
                                    break;
                                case 1:
                                    dacHighIndex = 2;
                                    xndHighIndex = 8;
                                    xndLowIndex = 9;
                                    break;
                                case 2:
                                    dacHighIndex = 3;
                                    xndHighIndex = 10;
                                    xndLowIndex = 11;
                                    break;
                                case 4:
                                    dacHighIndex = 4;
                                    xndHighIndex = 12;
                                    xndLowIndex = 13;
                                    break;
                                case 5:
                                    dacHighIndex = 5;
                                    xndHighIndex = 14;
                                    xndLowIndex = 15;
                                    break;
                        }
                        
                        if (dacHighIndex > 0)
                        {
                            addrDacHighStr = _calibrationLogic.GetAntennaAddress(dacHighIndex);
                        }
                        // 天线模式下没有daclow，使用默认值
                        addrDacLowStr = "0x0000";
                        if (xndHighIndex > 0)
                        {
                            addrXndHighStr = _calibrationLogic.GetAntennaAddress(xndHighIndex);
                        }
                        if (xndLowIndex > 0)
                        {
                            addrXndLowStr = _calibrationLogic.GetAntennaAddress(xndLowIndex);
                        }
                    }
                }
            }
            else if (_channelMode == "CH8")
            {
                // 八通道模式：使用新的地址计算逻辑
                if (_currentMode == "normal")
                {
                    // 八通道正常模式：每个通道占用4个地址
                    if (currentChannel >= 1 && currentChannel <= 8)
                    {
                        int dacHighIndex = _addressCalculator8.GetNormalAddressIndex(currentChannel, 1);
                        int dacLowIndex = _addressCalculator8.GetNormalAddressIndex(currentChannel, 2);
                        int xndHighIndex = _addressCalculator8.GetNormalAddressIndex(currentChannel, 3);
                        int xndLowIndex = _addressCalculator8.GetNormalAddressIndex(currentChannel, 4);
                        
                        addrDacHighStr = _addressCalculator8.GetNormalAddress(dacHighIndex);
                        addrDacLowStr = _addressCalculator8.GetNormalAddress(dacLowIndex);
                        addrXndHighStr = _addressCalculator8.GetNormalAddress(xndHighIndex);
                        addrXndLowStr = _addressCalculator8.GetNormalAddress(xndLowIndex);
                    }
                }
                else if (_currentMode == "antenna")
                {
                    // 八通道天线模式：每个通道占用3个地址（没有DAClow）
                    if (currentChannel >= 1 && currentChannel <= 8)
                    {
                        int dacHighIndex = _addressCalculator8.GetAntennaAddressIndex(currentChannel, 1);
                        int xndHighIndex = _addressCalculator8.GetAntennaAddressIndex(currentChannel, 3);
                        int xndLowIndex = _addressCalculator8.GetAntennaAddressIndex(currentChannel, 4);
                        
                        addrDacHighStr = _addressCalculator8.GetAntennaAddress(dacHighIndex);
                        // 天线模式下没有daclow，使用默认值
                        addrDacLowStr = "0x0000";
                        addrXndHighStr = _addressCalculator8.GetAntennaAddress(xndHighIndex);
                        addrXndLowStr = _addressCalculator8.GetAntennaAddress(xndLowIndex);
                    }
                }
            }

            // 更新地址显示
            if (_lblAddrDacHigh != null) _lblAddrDacHigh.Text = addrDacHighStr;
            if (_lblAddrDacLow != null) _lblAddrDacLow.Text = addrDacLowStr;
            if (_lblAddrXndHigh != null) _lblAddrXndHigh.Text = addrXndHighStr;
            if (_lblAddrXndLow != null) _lblAddrXndLow.Text = addrXndLowStr;

            if (forceUpdate)
            {
                // 强制更新所有文本框，使用缓存中的最新值
                UpdateTextBoxFromCache(_txtCalDacHigh);
                UpdateTextBoxFromCache(_txtCalDacLow);
                UpdateTextBoxFromCache(_txtCalXndHigh);
                UpdateTextBoxFromCache(_txtCalXndLow);
                
                // 更新地址缓存
                _addressCache[_txtCalDacHigh] = addrDacHighStr;
                _addressCache[_txtCalDacLow] = addrDacLowStr;
                _addressCache[_txtCalXndHigh] = addrXndHighStr;
                _addressCache[_txtCalXndLow] = addrXndLowStr;
            }
            else
            {
                // 检查地址是否发生变化并更新对应文本框
                UpdateTextBoxIfAddressChanged(_txtCalDacHigh, addrDacHighStr);
                UpdateTextBoxIfAddressChanged(_txtCalDacLow, addrDacLowStr);
                UpdateTextBoxIfAddressChanged(_txtCalXndHigh, addrXndHighStr);
                UpdateTextBoxIfAddressChanged(_txtCalXndLow, addrXndLowStr);
            }
            
            // 更新控件状态
            UpdateControlVisibility();
        }
        
        /// <summary>
        /// 根据当前模式更新控件的可见性和启用状态
        /// </summary>
        public void UpdateControlVisibility()
        {
            if (_form.InvokeRequired)
            {
                _form.Invoke(new Action(UpdateControlVisibility));
                return;
            }
            
            bool isAntennaMode = _currentMode == "antenna";
            
            // 天线模式下禁用daclow相关控件
            if (_txtCalDacLow != null)
            {
                _txtCalDacLow.Enabled = !isAntennaMode;
            }
            if (_lblAddrDacLow != null)
            {
                _lblAddrDacLow.Enabled = !isAntennaMode;
            }
            if (_lblDacLow != null)
            {
                _lblDacLow.Enabled = !isAntennaMode;
            }
            if (_btnUpDacLow != null)
            {
                _btnUpDacLow.Enabled = !isAntennaMode;
            }
            if (_btnDownDacLow != null)
            {
                _btnDownDacLow.Enabled = !isAntennaMode;
            }
            if (_read2 != null)
            {
                _read2.Enabled = !isAntennaMode;
            }
            if (_write2 != null)
            {
                _write2.Enabled = !isAntennaMode;
            }
        }

        /// <summary>
        /// 从缓存中更新文本框
        /// </summary>
        /// <param name="textBox">文本框</param>
        public void UpdateTextBoxFromCache(TextBox textBox)
        {
            try
            {
                if (textBox == null) return;

                // 获取当前控件对应的地址
                ushort address = GetCurrentAddressForControl(textBox);

                // 从缓存中查找对应地址的值
                byte value;
                bool found = _calibrationLogic.TryGetCalibrationValue(address, out value);

                if (found)
                {
                    // 更新文本框的值
                    if (textBox.InvokeRequired)
                    {
                        textBox.Invoke(new Action(() =>
                        {
                            textBox.Text = value.ToString("X2");
                        }));
                    }
                    else
                    {
                        textBox.Text = value.ToString("X2");
                    }
                }
            }
            catch (Exception ex)
            {
                _logMessageCallback?.Invoke($"更新文本框失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 检查地址是否发生变化并更新对应文本框
        /// </summary>
        /// <param name="textBox">文本框</param>
        /// <param name="newAddressStr">新地址字符串</param>
        private void UpdateTextBoxIfAddressChanged(TextBox textBox, string newAddressStr)
        {
            try
            {
                if (textBox == null) return;

                // 检查地址是否发生变化
                string oldAddressStr;
                bool addressChanged = !_addressCache.TryGetValue(textBox, out oldAddressStr) || oldAddressStr != newAddressStr;

                if (addressChanged)
                {
                    // 地址发生变化，更新文本框
                    UpdateTextBoxFromCache(textBox);
                    
                    // 更新地址缓存
                    _addressCache[textBox] = newAddressStr;
                    
                    _logMessageCallback?.Invoke($"地址变化 - 控件: {textBox.Name}, 新地址: {newAddressStr}");
                }
            }
            catch (Exception ex)
            {
                _logMessageCallback?.Invoke($"检查地址变化失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取当前控件对应的地址
        /// </summary>
        /// <param name="control">控件</param>
        /// <returns>地址</returns>
        public ushort GetCurrentAddressForControl(Control control)
        {
            return GetCurrentAddressForControl(control, _currentChannel);
        }

        /// <summary>
        /// 获取指定通道的控件对应的地址
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="channel">通道号</param>
        /// <returns>地址</returns>
        public ushort GetCurrentAddressForControl(Control control, int channel)
        {
            try
            {
                ushort address = 0;
                
                if (_channelMode == "CH5")
                {
                    // 五通道模式：使用现有逻辑
                    if (_currentMode == "normal")
                    {
                        int startIndex = _calibrationLogic.GetStartIndexByChannel(channel);
                            
                        // 根据控件类型确定对应的地址索引
                        if (control == _txtCalDacHigh)
                        {
                            string addressStr = _calibrationLogic.GetNormalAddress(startIndex);
                            address = _calibrationLogic.ParseAddressString(addressStr);
                        }
                        else if (control == _txtCalDacLow)
                        {
                            string addressStr = _calibrationLogic.GetNormalAddress(startIndex + 1);
                            address = _calibrationLogic.ParseAddressString(addressStr);
                        }
                        else if (control == _txtCalXndHigh)
                        {
                            string addressStr = _calibrationLogic.GetNormalAddress(startIndex + 10);
                            address = _calibrationLogic.ParseAddressString(addressStr);
                        }
                        else if (control == _txtCalXndLow)
                        {
                            string addressStr = _calibrationLogic.GetNormalAddress(startIndex + 11);
                            address = _calibrationLogic.ParseAddressString(addressStr);
                        }
                    }
                    else if (_currentMode == "antenna")
                    {
                        // 天线模式下，通道对应不同的地址索引
                        int antennaIndex = 0;
                        if (control == _txtCalDacHigh)
                        {
                            switch (channel)
                            {
                                case 3:
                                        antennaIndex = 1;
                                        break;
                                    case 1:
                                        antennaIndex = 2;
                                        break;
                                    case 2:
                                        antennaIndex = 3;
                                        break;
                                    case 4:
                                        antennaIndex = 4;
                                        break;
                                    case 5:
                                        antennaIndex = 5;
                                        break;
                            }
                        }
                        else if (control == _txtCalDacLow)
                        {
                            // 天线模式下没有daclow，返回0
                            address = 0;
                        }
                        else if (control == _txtCalXndHigh)
                        {
                            switch (channel)
                            {
                                case 3:
                                        antennaIndex = 6;
                                        break;
                                    case 1:
                                        antennaIndex = 8;
                                        break;
                                    case 2:
                                        antennaIndex = 10;
                                        break;
                                    case 4:
                                        antennaIndex = 12;
                                        break;
                                    case 5:
                                        antennaIndex = 14;
                                        break;
                            }
                        }
                        else if (control == _txtCalXndLow)
                        {
                            switch (channel)
                            {
                                case 3:
                                        antennaIndex = 7;
                                        break;
                                    case 1:
                                        antennaIndex = 9;
                                        break;
                                    case 2:
                                        antennaIndex = 11;
                                        break;
                                    case 4:
                                        antennaIndex = 13;
                                        break;
                                    case 5:
                                        antennaIndex = 15;
                                        break;
                            }
                        }
                        
                        if (antennaIndex > 0)
                        {
                            string addressStr = _calibrationLogic.GetAntennaAddress(antennaIndex);
                            address = _calibrationLogic.ParseAddressString(addressStr);
                        }
                    }
                }
                else if (_channelMode == "CH8")
                {
                    // 八通道模式：使用新的地址计算逻辑
                    if (_currentMode == "normal")
                    {
                        // 八通道正常模式：每个通道占用4个地址
                        int dacHighIndex = _addressCalculator8.GetNormalAddressIndex(channel, 1);
                        int dacLowIndex = _addressCalculator8.GetNormalAddressIndex(channel, 2);
                        int xndHighIndex = _addressCalculator8.GetNormalAddressIndex(channel, 3);
                        int xndLowIndex = _addressCalculator8.GetNormalAddressIndex(channel, 4);
                        
                        if (control == _txtCalDacHigh)
                        {
                            string addressStr = _addressCalculator8.GetNormalAddress(dacHighIndex);
                            address = _addressCalculator8.ParseAddressString(addressStr);
                        }
                        else if (control == _txtCalDacLow)
                        {
                            string addressStr = _addressCalculator8.GetNormalAddress(dacLowIndex);
                            address = _addressCalculator8.ParseAddressString(addressStr);
                        }
                        else if (control == _txtCalXndHigh)
                        {
                            string addressStr = _addressCalculator8.GetNormalAddress(xndHighIndex);
                            address = _addressCalculator8.ParseAddressString(addressStr);
                        }
                        else if (control == _txtCalXndLow)
                        {
                            string addressStr = _addressCalculator8.GetNormalAddress(xndLowIndex);
                            address = _addressCalculator8.ParseAddressString(addressStr);
                        }
                    }
                    else if (_currentMode == "antenna")
                    {
                        // 八通道天线模式：每个通道占用3个地址（没有DAClow）
                        int dacHighIndex = _addressCalculator8.GetAntennaAddressIndex(channel, 1);
                        int xndHighIndex = _addressCalculator8.GetAntennaAddressIndex(channel, 3);
                        int xndLowIndex = _addressCalculator8.GetAntennaAddressIndex(channel, 4);
                        
                        if (control == _txtCalDacHigh)
                        {
                            string addressStr = _addressCalculator8.GetAntennaAddress(dacHighIndex);
                            address = _addressCalculator8.ParseAddressString(addressStr);
                        }
                        else if (control == _txtCalDacLow)
                        {
                            // 天线模式下没有daclow，返回0
                            address = 0;
                        }
                        else if (control == _txtCalXndHigh)
                        {
                            string addressStr = _addressCalculator8.GetAntennaAddress(xndHighIndex);
                            address = _addressCalculator8.ParseAddressString(addressStr);
                        }
                        else if (control == _txtCalXndLow)
                        {
                            string addressStr = _addressCalculator8.GetAntennaAddress(xndLowIndex);
                            address = _addressCalculator8.ParseAddressString(addressStr);
                        }
                    }
                }
                
                return address;
            }
            catch (Exception ex)
            {
                _logMessageCallback?.Invoke($"获取地址失败: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// 调整数值增减
        /// </summary>
        /// <param name="textBox">文本框</param>
        /// <param name="step">步长</param>
        public void AdjustNumericValue(TextBox textBox, int step)
        {
            if (textBox == null) return;

            if (int.TryParse(textBox.Text, System.Globalization.NumberStyles.HexNumber, null, out int currentValue))
            {
                // 计算新值
                int newValue = currentValue + step;
                // 实现边界检查：0-FF之间
                if (newValue > 255)
                {
                    newValue = 255;
                }
                else if (newValue < 0)
                {
                    newValue = 0;
                }
                // 设置新值回文本框，使用两位数大写十六进制格式
                textBox.Text = newValue.ToString("X2");
            } 
            else 
            {
                // 如果文本框中的值不是有效的十六进制数，设置为默认值00
                textBox.Text = "00";
            }
        }

        /// <summary>
        /// 更新EEPROM数据到UI
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        public void UpdateEEPROMDataToUI(ushort address, byte value)
        {
            if (_form.InvokeRequired)
            {
                _form.Invoke(new Action<ushort, byte>(UpdateEEPROMDataToUI), address, value);
                return;
            }

            // 确定哪个控件对应这个地址
            Control targetControl = null;
            
            // 获取当前所有控件对应的地址
            ushort addrDacHigh = GetCurrentAddressForControl(_txtCalDacHigh);
            ushort addrDacLow = GetCurrentAddressForControl(_txtCalDacLow);
            ushort addrXndHigh = GetCurrentAddressForControl(_txtCalXndHigh);
            ushort addrXndLow = GetCurrentAddressForControl(_txtCalXndLow);
            
            // 找到匹配的控件
            if (address == addrDacHigh)
            {
                targetControl = _txtCalDacHigh;
            }
            else if (address == addrDacLow)
            {
                targetControl = _txtCalDacLow;
            }
            else if (address == addrXndHigh)
            {
                targetControl = _txtCalXndHigh;
            }
            else if (address == addrXndLow)
            {
                targetControl = _txtCalXndLow;
            }
            
            // 更新对应的文本框
            if (targetControl != null && targetControl is TextBox textBox)
            {
                string hexValue = value.ToString("X").PadLeft(2, '0');
                textBox.Text = hexValue;
            }
        }
    }
}
