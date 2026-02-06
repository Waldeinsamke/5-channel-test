using NationalInstruments.Visa;
using System.Threading;
using System.Threading.Tasks;

namespace 五通道自动测试.Instruments
{
    public abstract class InstrumentBase
    {
        protected MessageBasedSession? _session;
        public string Address { get; set; } = string.Empty;
        public bool IsConnected { get; protected set; }
        
        /// <summary>
        /// 超时时间（毫秒）
        /// </summary>
        protected int TimeoutMilliseconds { get; set; } = 30000;

        /// <summary>
        /// 异步操作超时时间（毫秒）
        /// </summary>
        protected int AsyncTimeoutMilliseconds { get; set; } = 15000;

        /// <summary>
        /// 是否启用发送命令后的延时
        /// </summary>
        protected virtual bool EnableDelay => false;
        
        public virtual bool Connect()
        {
            try
            {
                if (string.IsNullOrEmpty(Address))
                {
                    IsConnected = false;
                    return false;
                }
                
                using var resourceManager = new ResourceManager();
                _session = (MessageBasedSession)resourceManager.Open(Address);
                _session.TimeoutMilliseconds = TimeoutMilliseconds;
                IsConnected = true;
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] 仪表连接成功：{Address}");
                return true;
            }
            catch (Exception ex)
            {
                IsConnected = false;
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] 仪表连接失败：{Address}，错误：{ex.Message}");
                return false;
            }
        }
        
        public virtual void Disconnect()
        {
            if (_session != null)
            {
                _session.Dispose();
                _session = null;
                IsConnected = false;
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] 仪表已断开连接");
            }
        }
        
        /// <summary>
        /// 检查连接状态
        /// </summary>
        /// <returns>连接是否有效</returns>
        public virtual bool CheckConnection()
        {
            if (_session == null)
            {
                IsConnected = false;
                return false;
            }
            
            try
            {
                // 发送一个简单的查询命令来验证连接
                _session.RawIO.Write("*IDN?\n");
                var response = _session.RawIO.ReadString();
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] 连接状态检查成功：{response.Trim()}");
                IsConnected = true;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] 连接状态检查失败：{ex.Message}");
                IsConnected = false;
                return false;
            }
        }
        
        /// <summary>
        /// 尝试重新连接仪表
        /// </summary>
        /// <returns>重连是否成功</returns>
        public virtual bool Reconnect()
        {
            try
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] 尝试重新连接仪表：{Address}");
                Disconnect();
                return Connect();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] 重连失败：{ex.Message}");
                IsConnected = false;
                return false;
            }
        }
        
        /// <summary>
        /// 确保连接有效，如果无效则尝试重连
        /// </summary>
        /// <returns>连接是否有效</returns>
        public virtual bool EnsureConnection()
        {
            if (IsConnected && _session != null)
            {
                if (CheckConnection())
                    return true;
            }
            
            return Reconnect();
        }
        
        /// <summary>
        /// 仪表复位
        /// </summary>
        public virtual void Reset()
        {
            if (_session == null || !IsConnected)
                throw new InvalidOperationException("Instrument not connected");
            
            Write("*RST");
            Thread.Sleep(100);
            Write("*CLS");
            Thread.Sleep(100);
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] 仪表已复位");
        }
        
        /// <summary>
        /// 查询仪表
        /// </summary>
        /// <param name="command">SCPI命令</param>
        /// <returns>仪表响应</returns>
        public virtual string Query(string command)
        {
            if (!EnsureConnection() || _session == null)
                throw new InvalidOperationException("Instrument connection failed");
            
            try
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] 发送命令：{command}");
                _session.RawIO.Write(command + "\n");
                var result = _session.RawIO.ReadString().Trim();
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] 接收数据：{result}");
                return result;
            }
            catch (Ivi.Visa.IOTimeoutException)
            {
                var errorMsg = $"仪表通信超时，请检查连接。命令：{command}";
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {errorMsg}");
                IsConnected = false;
                throw new InvalidOperationException(errorMsg);
            }
            catch (Ivi.Visa.NativeVisaException ex)
            {
                var errorMsg = $"Visa 异常：{ex.Message}。命令：{command}";
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {errorMsg}");
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] 异常详情：{ex.ToString()}");
                IsConnected = false;
                throw new InvalidOperationException(errorMsg);
            }
            catch (Exception ex)
            {
                var errorMsg = $"查询命令失败：{ex.Message}。命令：{command}";
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {errorMsg}");
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] 异常详情：{ex.ToString()}");
                IsConnected = false;
                throw new InvalidOperationException(errorMsg);
            }
        }
        
        /// <summary>
        /// 查询仪表（异步版本，带超时保护）
        /// </summary>
        /// <param name="command">SCPI命令</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>仪表响应</returns>
        public virtual async Task<string> QueryAsync(string command, CancellationToken cancellationToken = default)
        {
            if (!EnsureConnection() || _session == null)
                throw new InvalidOperationException("Instrument connection failed");
            
            try
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] 异步发送命令：{command}");
                
                var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                timeoutCts.CancelAfter(AsyncTimeoutMilliseconds);
                
                return await Task.Run(async () =>
                {
                    try
                    {
                        _session.RawIO.Write(command + "\n");
                        var result = _session.RawIO.ReadString().Trim();
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] 异步接收数据：{result}");
                        return result;
                    }
                    catch (OperationCanceledException)
                    {
                        if (timeoutCts.IsCancellationRequested)
                        {
                            var errorMsg = $"仪表通信超时（{AsyncTimeoutMilliseconds}ms），请检查连接。命令：{command}";
                            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {errorMsg}");
                            IsConnected = false;
                            throw new TimeoutException(errorMsg);
                        }
                        throw;
                    }
                }, timeoutCts.Token);
            }
            catch (Ivi.Visa.IOTimeoutException)
            {
                var errorMsg = $"仪表通信超时，请检查连接。命令：{command}";
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {errorMsg}");
                IsConnected = false;
                throw new TimeoutException(errorMsg);
            }
            catch (Ivi.Visa.NativeVisaException ex)
            {
                var errorMsg = $"Visa 异常：{ex.Message}。命令：{command}";
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {errorMsg}");
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] 异常详情：{ex.ToString()}");
                IsConnected = false;
                throw new InvalidOperationException(errorMsg);
            }
            catch (TimeoutException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var errorMsg = $"查询命令失败：{ex.Message}。命令：{command}";
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {errorMsg}");
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] 异常详情：{ex.ToString()}");
                IsConnected = false;
                throw new InvalidOperationException(errorMsg);
            }
        }
        
        /// <summary>
        /// 发送命令到仪表（异步版本，带超时保护）
        /// </summary>
        /// <param name="command">SCPI命令</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>异步任务</returns>
        public virtual async Task WriteAsync(string command, CancellationToken cancellationToken = default)
        {
            if (!EnsureConnection() || _session == null)
                throw new InvalidOperationException("Instrument connection failed");
            
            try
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] 异步发送命令：{command}");
                
                var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                timeoutCts.CancelAfter(AsyncTimeoutMilliseconds);
                
                await Task.Run(() =>
                {
                    try
                    {
                        _session.RawIO.Write(command + "\n");
                        
                        if (EnableDelay)
                            Thread.Sleep(100);
                    }
                    catch (OperationCanceledException)
                    {
                        if (timeoutCts.IsCancellationRequested)
                        {
                            var errorMsg = $"发送命令超时（{AsyncTimeoutMilliseconds}ms），请检查连接。命令：{command}";
                            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {errorMsg}");
                            IsConnected = false;
                            throw new TimeoutException(errorMsg);
                        }
                        throw;
                    }
                }, timeoutCts.Token);
            }
            catch (Ivi.Visa.IOTimeoutException)
            {
                var errorMsg = $"仪表通信超时，请检查连接。命令：{command}";
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {errorMsg}");
                IsConnected = false;
                throw new TimeoutException(errorMsg);
            }
            catch (Ivi.Visa.NativeVisaException ex)
            {
                var errorMsg = $"Visa 异常：{ex.Message}。命令：{command}";
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {errorMsg}");
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] 异常详情：{ex.ToString()}");
                IsConnected = false;
                throw new InvalidOperationException(errorMsg);
            }
            catch (TimeoutException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var errorMsg = $"发送命令失败：{ex.Message}。命令：{command}";
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {errorMsg}");
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] 异常详情：{ex.ToString()}");
                IsConnected = false;
                throw new InvalidOperationException(errorMsg);
            }
        }
        
        public virtual void Write(string command)
        {
            if (!EnsureConnection() || _session == null)
                throw new InvalidOperationException("Instrument connection failed");
            
            try
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] 发送命令：{command}");
                _session.RawIO.Write(command + "\n");
                
                if (EnableDelay)
                    Thread.Sleep(100);
            }
            catch (Ivi.Visa.IOTimeoutException)
            {
                var errorMsg = $"仪表通信超时，请检查连接。命令：{command}";
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {errorMsg}");
                IsConnected = false;
                throw new InvalidOperationException(errorMsg);
            }
            catch (Ivi.Visa.NativeVisaException ex)
            {
                var errorMsg = $"Visa 异常：{ex.Message}。命令：{command}";
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {errorMsg}");
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] 异常详情：{ex.ToString()}");
                IsConnected = false;
                throw new InvalidOperationException(errorMsg);
            }
            catch (Exception ex)
            {
                var errorMsg = $"发送命令失败：{ex.Message}。命令：{command}";
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {errorMsg}");
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] 异常详情：{ex.ToString()}");
                IsConnected = false;
                throw new InvalidOperationException(errorMsg);
            }
        }
    }
}