using PluginSystem;

namespace CircularPlugins
{
    [PluginLoad("CircularPlugin1", "CircularPlugin2")]
    public class CircularPlugin1 : ICommand
    {
        public void Execute()
        {
            System.Console.WriteLine("CircularPlugin1 executed");
        }
    }
}