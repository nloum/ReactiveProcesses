using System;
using System.Collections.Immutable;
using System.Linq;
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
                .Scan(new { StringBuilder = new StringBuilder(), BuiltString = (string)null },
                    (state, ch) =>
                    {
                        if (ch == 0)
                        {
                            return new { StringBuilder = new StringBuilder(), BuiltString = state.StringBuilder.ToString() };
                        }

                        state.StringBuilder.Append(ch);
                        return new { state.StringBuilder, BuiltString = (string)null };
                    }).Where(state => state.BuiltString != null).Select(state => state.BuiltString)
                .Subscribe(x =>
            {
                Console.WriteLine(x);
            });
        }
    }
}