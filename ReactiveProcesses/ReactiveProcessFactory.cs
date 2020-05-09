using System.Collections.ObjectModel;
using System.Reactive.Concurrency;

namespace ReactiveProcesses
{
    public class ReactiveProcessFactory : IReactiveProcessFactory
    {
        public ReactiveProcess Start(string fileName, string arguments = "", IScheduler scheduler = null)
        {
            return new ReactiveProcess(fileName, arguments, scheduler);
        }
    }
}