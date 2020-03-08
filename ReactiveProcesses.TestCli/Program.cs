using System;

namespace ReactiveProcesses.TestCli
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Watching {Environment.CurrentDirectory}");
            
            var factory = new ReactiveProcessFactory();
            var proc = factory.Start("fswatch", $"-0 \"{Environment.CurrentDirectory}\"");
            proc.StandardOutput.Subscribe(x =>
            {
                Console.WriteLine(x);
            });
        }
    }
}