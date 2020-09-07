using System;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace ReactiveProcesses
{
    public class ReactiveProcess
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

            StandardOutput = new StreamReaderObservableAdapter(_process.StandardOutput).ObserveOn(scheduler)
                .Publish().RefCount();
            StandardError = new StreamReaderObservableAdapter(_process.StandardError).ObserveOn(scheduler)
                .Publish().RefCount();
            StandardInput = new StreamWriterObserverAdapter(_process.StandardInput);

            ExitCode = Wait();
        }

        private Task<int> Wait()
        {
            return Task.Factory.StartNew(() =>
            {
                _process.WaitForExit();
                return _process.ExitCode;
            }, TaskCreationOptions.LongRunning);
        }

        public IObservable<char> StandardOutput { get; }
        public IObservable<char> StandardError { get; }
        public Task<int> ExitCode { get; }
        public IObserver<string> StandardInput { get; }
    }
}