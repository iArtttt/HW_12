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

            if (_strategy.HasIndex)
            {
                if (_threads.Length == 1)
                    _threadParams[0] = ThreadParam<T, TResult>.Create(_arr, new Range(_strategy.Range.Start.Value, _strategy.Range.End.Value));
                else RangeSetRecursion(_threads, _arr, 0, _strategy);
            }
            else
            {
                if (_threads.Length == 1)
                    _threadParams[0] = ThreadParam<T, TResult>.Create(_arr, new Range(0, _arr.Length));
                else
                    RangeSetRecursion(_threads, _arr, 0);
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

        private void RangeSetRecursion(Thread[] threads, T[] arr, int thread)
        {
            if (thread == 0)
            {
                _threadParams![thread] = ThreadParam<T, TResult>.Create(arr, new Range(0, arr.Length / threads.Length), thread);

                thread++;
                RangeSetRecursion(threads, arr, thread);
            }
            else if (thread < threads.Length - 1)
            {
                _threadParams![thread] = ThreadParam<T, TResult>.Create(arr, new Range(
                    arr.Length / threads.Length * thread,
                    (arr.Length / threads.Length * thread) + (arr.Length / threads.Length)), thread);

                thread++;
                RangeSetRecursion(threads, arr, thread);
            }
            else
            {
                _threadParams![thread] = ThreadParam<T, TResult>.Create(arr, new Range(arr.Length / threads.Length * thread, arr.Length), thread);
            }
        }
        private void RangeSetRecursion(Thread[] threads, T[] arr, int thread, IThreadStrategy<T, TResult> length)
        {
            if (thread == 0)
            {
                _threadParams![thread] = ThreadParam<T, TResult>.Create(arr, new Range(
                    length.Range.Start.Value, length.Range.Start.Value + ((length.Range.End.Value - length.Range.Start.Value) / threads.Length)), thread);

                thread++;
                RangeSetRecursion(threads, arr, thread, length);
            }
            else if (thread < threads.Length - 1)
            {
                _threadParams![thread] = ThreadParam<T, TResult>.Create(arr, new Range(
                    length.Range.Start.Value + ((length.Range.End.Value- length.Range.Start.Value) / threads.Length * thread),
                    length.Range.Start.Value + ((length.Range.End.Value - length.Range.Start.Value) / threads.Length) * (thread + 1)), thread);

                thread++;
                RangeSetRecursion(threads, arr, thread, length);
            }
            else
            {
                _threadParams![thread] = ThreadParam<T, TResult>.Create(arr, new Range(
                    length.Range.End.Value / threads.Length * thread, length.Range.End.Value), thread);
            }
        }
    }
}