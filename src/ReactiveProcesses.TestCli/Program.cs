using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveProcesses.TestCli
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine($"Watching {Environment.CurrentDirectory}");
            
            var factory = new ReactiveProcessFactory();

            var x = await factory.Start("ls").ExitCode;
            Console.WriteLine(x);
            
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