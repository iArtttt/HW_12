using System.Diagnostics;

namespace HW_12
{
    internal class ThreadsUse<T, TResult>
    {
        private readonly IThreadStrategy<T, TResult> _strategy;
        private readonly T[] _arr;
        private readonly Stopwatch _timer = new();

        private ThreadParam<T, TResult>[]? _threadParams;
        private Thread[] _threads;

        public TResult? Result { get; private set; } = default;

        public ThreadsUse(int threadCount, T[] arr, IThreadStrategy<T, TResult> strategy)
        {
            if (threadCount <= 0) throw new ArgumentException("thread count must be more than 0", nameof(threadCount));
            if (arr == null) throw new ArgumentNullException(nameof(arr));
            if (arr.Length <= 0 ) throw new ArgumentOutOfRangeException(nameof(arr));
            if (strategy == null) throw new ArgumentNullException(nameof(strategy));

            _arr = arr;
            _strategy = strategy;
            _threads = new Thread[threadCount];
        }

        public void ThreadDo()
        {
            FeelThread();
            if (_threadParams == null || _strategy == default) return;

            if (_strategy is IInitParams initParams)
            {
                _threadParams = initParams.Init<T, TResult>(_arr.AsMemory(), _threads.Length);
            }
            else
            {
                var data = _arr.AsMemory();
                var itemsCount = data.Length / _threads.Length;
                for (int i = 0; i < _threadParams.Length; i++)
                {
                    _threadParams[i] = ThreadParam<T, TResult>.Create(data.Slice(i * itemsCount, itemsCount), i);
                }
            }

            _timer.Start();
            for (int i = 0; i < _threads.Length; i++)
                _threads[i].Start(_threadParams[i]);
            for (int i = 0; i < _threads.Length; i++)
                _threads[i].Join();
            Result = _strategy.ThreadResult(_threadParams);
            _timer.Stop();
        }

        public void Print()
        {
            Console.WriteLine($"Result is {Result}; Time is {_timer.Elapsed}");
        }

        private void FeelThread()
        {
            for (int i = 0; i < _threads.Length; i++)
            {
                _threads[i] = new Thread(_strategy.ThreadMethod);
            }
            _threadParams = new ThreadParam<T, TResult>[_threads.Length];
        }
    }
}