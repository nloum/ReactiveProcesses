using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Text;

namespace ReactiveProcesses.TestCli
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Watching {Environment.CurrentDirectory}");
            
            var factory = new ReactiveProcessFactory();
            var proc = factory.Start("fswatch", $"-0 \"{Environment.CurrentDirectory}\"");
            proc.StandardOutput
                .Lines()
                .Subscribe(x =>
                {
                    Console.WriteLine(x);
                });
        }
    }
}