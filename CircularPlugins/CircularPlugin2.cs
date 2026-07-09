using PluginSystem;

namespace CircularPlugins
{
    [PluginLoad("CircularPlugin2", "CircularPlugin1")]
    public class CircularPlugin2 : ICommand
    {
        public void Execute()
        {
            System.Console.WriteLine("CircularPlugin2 executed");
        }
    }
}