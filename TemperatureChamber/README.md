# 高低温试验箱控制模块

## 功能说明

本模块提供了基于RS232接口的高低温试验箱控制功能，支持以下核心功能：

- **设备连接/断开** - 通过独立RS232串口(COM19)连接温箱
- **设备启停控制** - 启动/停止温箱运行
- **温度控制** - 读取/设置当前温度
- **湿度控制** - 读取/设置湿度（如果设备支持）
- **状态监控** - 读取设备运行状态
- **事件通知** - 连接状态、错误、状态更新事件

## 技术架构

### 文件夹结构

```
TemperatureChamber/
├── Communication/
│   ├── RS232SerialPort.cs          # 独立RS232串口管理类
│   ├── SimpleModbusMaster.cs      # Modbus通讯核心类
│   └── ModbusConstants.cs          # Modbus协议常量
├── Models/
│   ├── DeviceStatus.cs             # 设备状态信息类
│   └── ChamberConfig.cs            # 温箱配置类
├── ChamberController.cs            # 温箱控制核心类
├── IChamberController.cs           # 温箱控制接口
└── README.md                       # 模块说明文档
```

### 核心组件

1. **RS232SerialPort** - 独立的RS232串口管理类
   - 完全独立的串口操作
   - 支持标准串口参数配置
   - 线程安全设计

2. **SimpleModbusMaster** - Modbus通讯核心类
   - 支持RTU模式
   - 实现核心Modbus指令
   - 处理CRC校验
   - 提供异步接口

3. **ChamberController** - 温箱控制核心类
   - 实现IChamberController接口
   - 设备连接管理
   - 温度/湿度控制
   - 运行状态管理
   - 事件触发机制
   - 参数验证

## 使用示例

### 基本使用

```csharp
using TemperatureChamber;
using TemperatureChamber.Models;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // 创建RS232连接配置 (COM19端口)
        var config = new ChamberConfig
        {
            PortName = "COM19",          // RS232端口名称
            SlaveId = 1,                 // Modbus从站地址
            BaudRate = 38400,            // 波特率
            Parity = System.IO.Ports.Parity.Even,        // 偶校验
            DataBits = 8,                // 数据位
            StopBits = System.IO.Ports.StopBits.One,     // 停止位
            Timeout = 1000               // 超时时间(ms)
        };

        // 创建温箱控制器
        var chamber = new ChamberController(config);

        // 订阅事件
        chamber.ConnectionChanged += (sender, isConnected) =>
        {
            Console.WriteLine($"设备连接状态: {(isConnected ? "已连接" : "已断开")}");
        };

        chamber.ErrorOccurred += (sender, error) =>
        {
            Console.WriteLine($"错误: {error}");
        };

        chamber.StatusUpdated += (sender, status) =>
        {
            Console.WriteLine($"温度: {status.Temperature:F2}℃, 湿度: {status.Humidity:F1}%, 状态: {(status.IsRunning ? "运行中" : "停止")}");
        };

        try
        {
            // 连接设备
            Console.WriteLine("正在连接到温箱设备...");
            bool connected = await chamber.ConnectAsync();
            
            if (!connected)
            {
                Console.WriteLine("连接失败，程序退出");
                return;
            }
            
            Console.WriteLine("连接成功！");
            
            // 读取当前状态
            var status = await chamber.GetStatusAsync();
            Console.WriteLine($"当前温度: {status.Temperature:F2}℃");
            Console.WriteLine($"当前湿度: {status.Humidity:F1}%");
            Console.WriteLine($"运行状态: {(status.IsRunning ? "运行中" : "停止")}");
            
            // 设置温度
            Console.WriteLine("\n设置目标温度为50℃...");
            await chamber.SetTemperatureAsync(50.0);
            Console.WriteLine("温度设置完成");
            
            // 启动设备
            Console.WriteLine("\n启动设备...");
            await chamber.StartAsync();
            Console.WriteLine("设备已启动");
            
            // 开始监控
            Console.WriteLine("\n开始监控设备状态...");
            await chamber.StartMonitoringAsync(2000);
            
            // 运行一段时间
            Console.WriteLine("\n运行30秒后停止...");
            await Task.Delay(30000);
            
            // 停止设备
            Console.WriteLine("\n停止设备...");
            await chamber.StopAsync();
            Console.WriteLine("设备已停止");
            
            // 断开连接
            Console.WriteLine("\n断开连接...");
            chamber.Disconnect();
            
            Console.WriteLine("\n操作完成！");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"发生错误: {ex.Message}");
        }
        
        Console.WriteLine("\n按任意键退出...");
        Console.ReadKey();
    }
}
```

### 接口说明

#### IChamberController 接口

- **ConnectAsync()** - 连接到设备
- **Disconnect()** - 断开连接
- **GetStatusAsync()** - 获取设备状态
- **SetTemperatureAsync(double)** - 设置温度
- **SetHumidityAsync(double)** - 设置湿度
- **StartAsync()** - 启动设备
- **StopAsync()** - 停止设备
- **StartMonitoringAsync(int)** - 开始监控设备状态

#### 事件

- **ConnectionChanged** - 连接状态变化事件
- **ErrorOccurred** - 错误发生事件
- **StatusUpdated** - 状态更新事件

## 配置说明

### 默认配置

| 参数 | 默认值 | 说明 |
|------|--------|------|
| PortName | COM19 | RS232端口名称 |
| SlaveId | 1 | Modbus从站地址 |
| BaudRate | 38400 | 波特率 |
| Parity | Even | 校验位 |
| DataBits | 8 | 数据位 |
| StopBits | One | 停止位 |
| Timeout | 1000 | 超时时间(ms) |
| MinTemperature | -40.0 | 最低温度(℃) |
| MaxTemperature | 150.0 | 最高温度(℃) |
| MinHumidity | 20.0 | 最低湿度(%) |
| MaxHumidity | 98.0 | 最高湿度(%) |

## 注意事项

1. **RS232接口保护** - 确保正确连接，避免静电损坏
2. **串口参数匹配** - 确保与设备的串口参数一致
3. **连接稳定性** - 处理连接断开和重连逻辑
4. **错误处理** - 完善的异常处理机制
5. **资源释放** - 确保串口资源正确释放
6. **温度范围** - 操作时注意温度范围限制

## 依赖项

- **.NET版本**：支持.NET 6.0及以上
- **依赖库**：仅使用`System.IO.Ports`标准库

## 故障排除

### 常见问题

1. **连接失败**
   - 检查RS232端口是否正确（默认COM19）
   - 确认设备电源已打开
   - 检查通讯线是否连接正常

2. **通讯超时**
   - 确认波特率设置为38400
   - 确认校验位为偶校验
   - 确认从站地址匹配

3. **控制无响应**
   - 确认设备处于远程控制模式
   - 检查是否有报警信息阻止设备运行

4. **温度设置无效**
   - 确认温度值在有效范围内
   - 检查设备是否处于运行状态

## 版本历史

- **v1.0.0** - 初始版本
  - 实现基本连接和控制功能
  - 支持温度和湿度控制
  - 提供事件通知机制
  - 独立RS232串口管理
