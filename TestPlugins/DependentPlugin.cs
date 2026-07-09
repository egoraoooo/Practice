using PluginSystem;

namespace TestPlugins
{
    [PluginLoad("DependentPlugin", "BasicPlugin")]
    public class DependentPlugin : ICommand
    {
        public bool WasExecuted { get; private set; }
        
        public void Execute()
        {
            WasExecuted = true;
            System.Console.WriteLine("DependentPlugin executed");
        }
    }
}