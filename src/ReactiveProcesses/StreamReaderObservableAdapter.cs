using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;

namespace ReactiveProcesses
{
    public class StreamReaderObservableAdapter : IObservable<char>
    {
        private readonly StreamReader _reader;

        public StreamReaderObservableAdapter(StreamReader reader)
        {
            _reader = reader;
        }

        public IDisposable Subscribe(IObserver<char> observer)
        {
            return ReadCharacters(_reader).ToObservable().Subscribe(observer);
        }

        private static IEnumerable<char> ReadCharacters(StreamReader reader)
        {
            while (!reader.EndOfStream)
            {
                yield return (char) reader.Read();
            }
        }

    }
}