using System.Reactive.Concurrency;

namespace ReactiveProcesses
{
    public interface IReactiveProcessFactory
    {
        ReactiveProcess Start(string fileName, string arguments, IScheduler scheduler = null);
    }
}