using PluginSystem;

namespace TestPlugins
{
    [PluginLoad("BasicPlugin")]
    public class BasicPlugin : ICommand
    {
        public bool WasExecuted { get; private set; }
        
        public void Execute()
        {
            WasExecuted = true;
            System.Console.WriteLine("BasicPlugin executed");
        }
    }
}