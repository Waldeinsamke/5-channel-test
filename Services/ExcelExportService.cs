using OfficeOpenXml;
using System.Text.RegularExpressions;
using 五通道自动测试.Test;

namespace 五通道自动测试.Services
{
    public class ExcelExportService
    {
        private readonly Dictionary<string, ExcelRange> _indexCells = new();
        
        public bool ExportToExcel(string templatePath, string outputPath, List<TestResult> testResults)
        {
            try
            {
                var templateFile = new FileInfo(templatePath);
                var outputFile = new FileInfo(outputPath);
                
                if (!templateFile.Exists)
                {
                    MessageBox.Show($"模板文件不存在：{templatePath}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                
                // 设置 EPPlus 非商业个人许可证
                ExcelPackage.License.SetNonCommercialPersonal("Test User");
                
                using var package = new ExcelPackage(templateFile);
                var ws = package.Workbook.Worksheets[0];
                
                BuildIndexCache(ws);
                
                // 显示测试结果数量
                MessageBox.Show($"准备导出 {testResults.Count} 条测试结果", "测试结果数量", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // 收集调试信息
                var debugMessages = new List<string>();
                
                foreach (var result in testResults)
                {
                    // 直接使用原始测试项名称进行判断，不进行提取
                    string rawTestItem = result.TestItem;
                    double frequency = ExtractFrequency(result.TestItem);
                    
                    string testItem = ExtractTestItemName(rawTestItem);
                    
                    string channelChar = GetChannelChar(result.Channel);
                    string freqNum = GetFreqNum(frequency);
                    
                    if (rawTestItem.Contains("验证中频输出"))
                    {
                        // 中频输出测试
                        string prefix = GetPrefix("中频输出");
                        string index = $"{prefix}{channelChar}{freqNum}";
                        bool matched = _indexCells.TryGetValue(index, out var cell);
                        if (matched)
                        {
                            cell.Value = FormatValue(result.Value);
                        }
                        
                        // 收集调试信息
                        debugMessages.Add($"中频输出: rawTestItem='{rawTestItem}', channel={result.Channel}, freq={frequency}, index='{index}', matched={matched}, value={result.Value}");
                    }
                    else if (rawTestItem.Contains("动态范围"))
                    {
                        // 动态范围测试
                        string index = GetExcelIndex(testItem, result.Channel, frequency, rawTestItem);
                        bool matched = _indexCells.TryGetValue(index, out var cell);
                        if (matched)
                        {
                            cell.Value = FormatValue(result.Value);
                        }
                        
                        // 收集调试信息
                        debugMessages.Add($"动态范围: rawTestItem='{rawTestItem}', channel={result.Channel}, freq={frequency}, index='{index}', matched={matched}, value={result.Value}");
                    }
                    else if (rawTestItem.Contains("带外抑制"))
                    {
                        // 带外抑制测试 - 同时处理dw1和dw2
                        string indexDw1 = $"dw1{channelChar}{freqNum}";
                        bool matchedDw1 = _indexCells.TryGetValue(indexDw1, out var cellDw1);
                        if (matchedDw1)
                        {
                            cellDw1.Value = FormatValue(Math.Abs(result.Value));
                        }
                        
                        // 处理dw2前缀，使用相同的值
                        string indexDw2 = $"dw2{channelChar}{freqNum}";
                        bool matchedDw2 = _indexCells.TryGetValue(indexDw2, out var cellDw2);
                        if (matchedDw2)
                        {
                            cellDw2.Value = FormatValue(Math.Abs(result.Value));
                        }
                        
                        // 收集调试信息
                        debugMessages.Add($"带外抑制: rawTestItem='{rawTestItem}', channel={result.Channel}, freq={frequency}, indexDw1='{indexDw1}', matchedDw1={matchedDw1}, indexDw2='{indexDw2}', matchedDw2={matchedDw2}, value={result.Value}");
                    }
                    else
                    {
                        // 其他测试项
                        string index = GetExcelIndex(testItem, result.Channel, frequency, rawTestItem);
                        bool matched = _indexCells.TryGetValue(index, out var cell);
                        if (matched)
                        {
                            double exportValue = result.Value;
                            // 镜像抑制、谐波抑制、邻道抑制取绝对值
                            if (testItem == "镜像抑制测试" || testItem == "谐波抑制测试" || testItem == "邻道抑制测试")
                            {
                                exportValue = Math.Abs(result.Value);
                            }
                            cell.Value = FormatValue(exportValue);
                        }
                        
                        // 收集调试信息
                        debugMessages.Add($"其他测试项: rawTestItem='{rawTestItem}', testItem='{testItem}', channel={result.Channel}, freq={frequency}, index='{index}', matched={matched}, value={result.Value}");
                    }
                }
                
                // 显示前20条调试信息
                string debugText = string.Join(Environment.NewLine, debugMessages.Take(20));
                if (debugMessages.Count > 20)
                {
                    debugText += Environment.NewLine + $"... 共 {debugMessages.Count} 条调试信息";
                }
                MessageBox.Show(debugText, "测试结果调试", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                package.SaveAs(outputFile);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"导出失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        
        private void BuildIndexCache(ExcelWorksheet ws)
        {
            _indexCells.Clear();
            
            int maxRow = ws.Dimension?.End.Row ?? 100;
            int maxCol = ws.Dimension?.End.Column ?? 50;
            
            for (int row = 1; row <= maxRow; row++) // 检查所有行
            {
                for (int col = 1; col <= maxCol; col++) // 检查所有列
                {
                    var cell = ws.Cells[row, col];
                    var value = cell.Value?.ToString() ?? "";
                    
                    if (IsExcelIndex(value))
                    {
                        var index = ExtractIndex(value);
                        if (!string.IsNullOrEmpty(index))
                        {
                            _indexCells[index] = cell;
                        }
                    }
                }
            }
            
            // 调试信息：显示模板中提取的索引数量和部分索引
            if (_indexCells.Any())
            {
                var indexSample = string.Join(", ", _indexCells.Keys.Take(20));
                MessageBox.Show($"模板中提取到 {_indexCells.Count} 个索引\n索引样本: {indexSample}", "索引提取调试", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("模板中未提取到任何索引", "索引提取错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private bool IsExcelIndex(string text)
        {
            return Regex.IsMatch(text, @"^\[([a-z0-9-]+)\]", RegexOptions.IgnoreCase);
        }
        
        private string ExtractIndex(string text)
        {
            var match = Regex.Match(text, @"\[([a-z0-9-]+)\]", RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value.ToLower() : "";
        }
        
        private string ExtractTestItemName(string testItemWithDesc)
        {
            // 提取测试项核心名称，移除条件描述
            if (testItemWithDesc.Contains("动态范围"))
            {
                return "动态范围";
            }
            else if (testItemWithDesc.Contains("中频输出"))
            {
                return "中频输出";
            }
            else if (testItemWithDesc.Contains("带外抑制"))
            {
                return "带外抑制";
            }
            else if (testItemWithDesc.Contains("校准开关隔离度"))
            {
                return "校准开关隔离度";
            }
            
            // 对于其他测试项，使用正则表达式提取核心名称
            var match = Regex.Match(testItemWithDesc, @"^([^\s-]+)");
            return match.Success ? match.Groups[1].Value : testItemWithDesc;
        }
        
        private double ExtractFrequency(string testItemWithDesc)
        {
            var match = Regex.Match(testItemWithDesc, @"(\d{4})MHz");
            if (match.Success && double.TryParse(match.Groups[1].Value, out double freq))
            {
                return freq;
            }
            return 3330;
        }
        
        private string ExtractInputPower(string testItemWithDesc)
        {
            // 从测试项描述中提取输入功率信息，只返回数值，不包含负号
            var match = Regex.Match(testItemWithDesc, @"输入功率[-+]?(\d+)dBm");
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            return "";
        }
        
        private string GetExcelIndex(string testItem, int channel, double frequency, string testItemWithDesc = "")
        {
            string prefix = GetPrefix(testItem);
            string channelChar = GetChannelChar(channel);
            string freqNum = GetFreqNum(frequency);
            
            // 对于动态范围测试，提取输入功率信息
            if (testItem == "动态范围")
            {
                string inputPower = ExtractInputPower(testItemWithDesc);
                if (!string.IsNullOrEmpty(inputPower))
                {
                    return $"{prefix}{channelChar}{freqNum}-{inputPower}";
                }
            }
            
            return $"{prefix}{channelChar}{freqNum}";
        }
        
        private string GetPrefix(string testItem)
        {
            return testItem switch
            {
                "动态范围" => "dt",
                "动态范围及中频输出信号测试" => "dt",
                "端口驻波测试（输入）" => "zb",
                "通道增益测试" => "zy",
                "带内增益波动测试" => "dn",
                "镜像抑制测试" => "jx",
                "谐波抑制测试" => "xb",
                "邻道抑制测试" => "ld",
                "带外抑制" => "dw1",
                "校准开关隔离度" => "jz",
                "中频输出" => "zp",
                "中频信号输出" => "zp",
                _ => ""
            };
        }
        
        private string GetChannelChar(int channel)
        {
            return ((char)('a' + channel - 1)).ToString();
        }
        
        private string GetFreqNum(double frequency)
        {
            return frequency switch
            {
                3330 => "30",
                3350 => "50",
                3370 => "70",
                3390 => "90",
                _ => "30"
            };
        }

        private string FormatValue(double value)
        {
            double rounded = Math.Round(value, 1);
            if (rounded % 1 == 0)
            {
                return ((int)rounded).ToString();
            }
            return rounded.ToString("0.0");
        }
    }
}
