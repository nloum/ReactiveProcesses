using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace ReactiveProcesses
{
    public class ReactiveProcess
    {
        private Process _process;
        private Subject<string> _standardInput = new Subject<string>();

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

            // TODO - validate performance on sending each character from StandardOutput and StandardError as separate events.
            // The other choice is to use ReadLine or the events, both of which assume valid string data.

            // StandardOutput = Observable.FromEventPattern<DataReceivedEventHandler, DataReceivedEventArgs>(
            //         handler => _process.OutputDataReceived += handler,
            //         handler => _process.OutputDataReceived -= handler)
            //     .Select(x => x.EventArgs.Data);
            //
            // StandardError = Observable.FromEventPattern<DataReceivedEventHandler, DataReceivedEventArgs>(
            //         handler => _process.ErrorDataReceived += handler,
            //         handler => _process.ErrorDataReceived -= handler)
            //     .Select(x => x.EventArgs.Data);

            StandardOutput = ReadLines(_process.StandardOutput).ToObservable(scheduler);
            StandardError = ReadLines(_process.StandardError).ToObservable(scheduler);

            ExitCode = Wait();

            _standardInput.Subscribe(text => _process.StandardInput.Write(text));
        }

        private Task<int> Wait()
        {
            return Task.Factory.StartNew(() =>
            {
                _process.WaitForExit();
                return _process.ExitCode;
            }, TaskCreationOptions.LongRunning);
        }

        private IEnumerable<char> ReadLines(StreamReader reader)
        {
            while (!reader.EndOfStream)
            {
                yield return (char) reader.Read();
            }
        }
        
        public IObservable<char> StandardOutput { get; }
        public IObservable<char> StandardError { get; }
        public Task<int> ExitCode { get; }
        public IObserver<string> StandardInput => _standardInput;
    }
}