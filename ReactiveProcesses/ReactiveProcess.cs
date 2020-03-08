using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace ReactiveProcesses
{
    public class ReactiveProcess : IObserver<string>
    {
        private Process _process;

        public ReactiveProcess(string fileName, string arguments, IScheduler scheduler = null)
        {
            if (scheduler == null)
            {
                scheduler = NewThreadScheduler.Default;
            }
            
            _process = Process.Start(new ProcessStartInfo(fileName, arguments)
            {
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
            });

            _process.EnableRaisingEvents = true;

            // TODO - come up with a better way of providing standard error and standard output that doesn't always assume
            // everything will be newline-delimited. E.g., fswatch uses NULL delimiters.
            
            StandardOutput = Observable.FromEventPattern<DataReceivedEventHandler, DataReceivedEventArgs>(
                    handler => _process.OutputDataReceived += handler,
                    handler => _process.OutputDataReceived -= handler)
                .Select(x => x.EventArgs.Data);

            StandardError = Observable.FromEventPattern<DataReceivedEventHandler, DataReceivedEventArgs>(
                    handler => _process.ErrorDataReceived += handler,
                    handler => _process.ErrorDataReceived -= handler)
                .Select(x => x.EventArgs.Data);

            // StandardOutput = ReadLines(_process.StandardOutput).ToObservable(scheduler);
            // StandardError = ReadLines(_process.StandardError).ToObservable(scheduler);
            ExitCode = Observable.FromEventPattern<EventHandler, EventArgs>(handler => _process.Exited += handler,
                    handler => _process.Exited -= handler, scheduler).Select(ep => _process.ExitCode)
                .Take(1);
        }

        private IEnumerable<string> ReadLines(StreamReader reader)
        {
            while (!reader.EndOfStream)
            {
                yield return reader.ReadLine();
            }
        }
        
        public IObservable<string> StandardOutput { get; }
        public IObservable<string> StandardError { get; }
        public IObservable<int> ExitCode { get; }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(string value)
        {
            _process.StandardInput.Write(value);
        }
    }
}