using System.Diagnostics;

namespace HW_12
{
    internal abstract class ThreadsUse<T, TResult>
    {
        private ThreadParam<T, TResult>[]? _ThreadParams;
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

        public void ThreadDo(IThreadParamStrategy<T, TResult> threadParams)
        {
            FeelThread(threadParams);
            if (_ThreadParams == null || threadParams == default) return;

            if (threadParams.HasIndex)
            {
                if (_threads.Length == 1) _ThreadParams[0] = ThreadParam<T, TResult>.Create(_arr, new Range(threadParams.Range.Start.Value, threadParams.Range.End.Value));
                else RangeSetRecursion(_threads, _arr, 0, threadParams);
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
            _result = (TResult)threadParams.ThreadResult(_ThreadParams);
            _timer.Stop();
        }
        public void Print()
        {
            Console.WriteLine($"Result is {Result}; Time is {_timer.Elapsed}");
        }
        private void FeelThread(IThreadParamStrategy<T, TResult> threadParams)
        {
            for (int i = 0; i < _threads.Length; i++)
            {
                _threads[i] = new Thread(threadParams.ThreadMethod);
            }
            _ThreadParams = new ThreadParam<T, TResult>[_threads.Length];
        }
        private void RangeSetRecursion(Thread[] threads, T[] arr, int thread)
        {
            if (thread == 0)
            {
                _ThreadParams![thread] = ThreadParam<T, TResult>.Create(arr, new Range(0, arr.Length / threads.Length), thread);

                thread++;
                RangeSetRecursion(threads, arr, thread);
            }
            else if (thread < threads.Length - 1)
            {
                _ThreadParams![thread] = ThreadParam<T, TResult>.Create(arr, new Range(
                    arr.Length / threads.Length * thread,
                    (arr.Length / threads.Length * thread) + (arr.Length / threads.Length)), thread);

                thread++;
                RangeSetRecursion(threads, arr, thread);
            }
            else
            {
                _ThreadParams![thread] = ThreadParam<T, TResult>.Create(arr, new Range(arr.Length / threads.Length * thread, arr.Length), thread);
            }
        }
        private void RangeSetRecursion(Thread[] threads, T[] arr, int thread, IThreadParamStrategy<T, TResult> length)
        {
            if (thread == 0)
            {
                _ThreadParams![thread] = ThreadParam<T, TResult>.Create(arr, new Range(
                    length.Range.Start.Value, length.Range.Start.Value + ((length.Range.End.Value - length.Range.Start.Value) / threads.Length)), thread);

                thread++;
                RangeSetRecursion(threads, arr, thread, length);
            }
            else if (thread < threads.Length - 1)
            {
                _ThreadParams![thread] = ThreadParam<T, TResult>.Create(arr, new Range(
                    length.Range.Start.Value + ((length.Range.End.Value- length.Range.Start.Value) / threads.Length * thread),
                    length.Range.Start.Value + ((length.Range.End.Value - length.Range.Start.Value) / threads.Length) * (thread + 1)), thread);

                thread++;
                RangeSetRecursion(threads, arr, thread, length);
            }
            else
            {
                _ThreadParams![thread] = ThreadParam<T, TResult>.Create(arr, new Range(
                    length.Range.End.Value / threads.Length * thread, length.Range.End.Value), thread);
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