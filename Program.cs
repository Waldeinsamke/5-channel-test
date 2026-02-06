using 五通道自动测试.Instruments;

namespace 五通道自动测试
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            InstrumentManager instrumentManager = new InstrumentManager();
            
            Form1 mainForm = new Form1(instrumentManager);
            Application.Run(mainForm);
        }
    }
}
