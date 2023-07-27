using System.Diagnostics;

namespace HW_12
{
    class ThreadsUse<T, TResult>
    {
        private ThreadParam<T, TResult>[]? _ThreadParams;
        private IThreadParam<T, TResult> _threadParam;
        private Thread[] _threads;
        private TResult? _result = default;
        public TResult? Result => _result;
        private readonly T[] _arr;
        private readonly Stopwatch _timer = new();
        public ThreadsUse(T[] arr)
        {
            _arr = arr;
            ThreadsCount();
        }

        public void ThreadDo(IThreadParam<T, TResult> threadParams)
        {
            FeelThread(threadParams);
            if (_ThreadParams == null || _threadParam == default) return;

            if (_threadParam.HasIndex)
            {
                if (_threads.Length == 1) _ThreadParams[0] = ThreadParam<T, TResult>.Create(_arr, new Range(_threadParam.StartIndex, _threadParam.LastIndex));
                else RangeSetRecursion(_threads, _arr, 0, _threadParam);
            }
            else
            {
                if (_threads.Length == 1) _ThreadParams[0] = ThreadParam<T, TResult>.Create(_arr, new Range(0, _arr.Length));
                else RangeSetRecursion(_threads, _arr, 0);
            }


            _timer.Start();
            for (int i = 0; i < _threads.Length; i++)
                _threads[i].Start(_ThreadParams[i]);
            for (int i = 0; i < _threads.Length; i++)
                _threads[i].Join();
            _result = (TResult)_threadParam.ThreadResult(_ThreadParams);
            _timer.Stop();
        }
        public void Print()
        {
            Console.WriteLine($"Result is {Result}; Time is {_timer.Elapsed}");
        }
        private void FeelThread(IThreadParam<T, TResult> threadParams)
        {
            _threadParam = threadParams;
            for (int i = 0; i < _threads.Length; i++)
            {
                _threads[i] = new Thread(_threadParam.ThreadMethod);
            }
            _ThreadParams = new ThreadParam<T, TResult>[_threads.Length];
        }
        private void RangeSetRecursion(Thread[] threads, T[] arr, int thread)
        {
            if (thread == 0)
            {
                _ThreadParams![thread] = ThreadParam<T, TResult>.Create(arr, new Range(0, arr.Length / threads.Length));

                thread++;
                RangeSetRecursion(threads, arr, thread);
            }
            else if (thread < threads.Length - 1)
            {
                _ThreadParams![thread] = ThreadParam<T, TResult>.Create(arr, new Range(
                    arr.Length / threads.Length * thread,
                    (arr.Length / threads.Length * thread) + (arr.Length / threads.Length)));

                thread++;
                RangeSetRecursion(threads, arr, thread);
            }
            else
            {
                _ThreadParams![thread] = ThreadParam<T, TResult>.Create(arr, new Range(arr.Length / threads.Length * thread, arr.Length));
            }
        }
        private void RangeSetRecursion(Thread[] threads, T[] arr, int thread, IThreadParam<T, TResult> length)
        {
            if (thread == 0)
            {
                _ThreadParams![thread] = ThreadParam<T, TResult>.Create(arr, new Range(
                    length.StartIndex, length.StartIndex + ((length.LastIndex - length.StartIndex) / threads.Length)));

                thread++;
                RangeSetRecursion(threads, arr, thread, length);
            }
            else if (thread < threads.Length - 1)
            {
                _ThreadParams![thread] = ThreadParam<T, TResult>.Create(arr, new Range(
                    length.StartIndex + ((length.LastIndex - length.StartIndex) / threads.Length * thread),
                    length.StartIndex + ((length.LastIndex - length.StartIndex) / threads.Length) * (thread + 1)));

                thread++;
                RangeSetRecursion(threads, arr, thread, length);
            }
            else
            {
                _ThreadParams![thread] = ThreadParam<T, TResult>.Create(arr, new Range(
                    length.LastIndex / threads.Length * thread, length.LastIndex));
            }
        }
        private void ThreadsCount()
        {
            Console.Write("Please write how much threads do you want to use? --> ");
            var threadsCount = int.Parse(Console.ReadLine());
            if (threadsCount <= 0) threadsCount = 1;

            _threads = new Thread[threadsCount];
        }
    }
}