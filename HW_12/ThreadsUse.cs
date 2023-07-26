using System.Diagnostics;

namespace HW_12
{
    class ThreadsUse
    {
        private ThreadParam[]? _ThreadParams;
        private IThreadParam _threadParam;
        private Thread[] _threads;
        private ulong _result = 0;
        private readonly int[] _arr;
        private readonly Stopwatch _timer = new();
        public ThreadsUse(int[] arr)
        {
            _arr = arr;
            ThreadsCount();
        }

        public void ThreadDo(IThreadParam threadParams)
        {
            FeelThread(threadParams);
            if (_ThreadParams == null || _threadParam == default) return;

            if (_threads.Length == 1) _ThreadParams[0] = ThreadParam.Create(_arr, new Range(0, _arr.Length));
            else RangeSetRecursion(_threads, _arr, 0);


            _timer.Start();
            for (int i = 0; i < _threads.Length; i++)
                _threads[i].Start(_ThreadParams[i]);
            for (int i = 0; i < _threads.Length; i++)
                _threads[i].Join();
            _result = _threadParam.ThreadResult(_ThreadParams);
            _timer.Stop();
        }
        public void Print()
        {
            Console.WriteLine($"Result is {_result}; Time is {_timer.Elapsed}");
        }
        private void FeelThread(IThreadParam threadParams)
        {
            _threadParam = threadParams;
            for (int i = 0; i < _threads.Length; i++)
            {
                _threads[i] = new Thread(threadParams.ThreadMethod);
            }
            _ThreadParams = new ThreadParam[_threads.Length];
        }
        private void RangeSetRecursion(Thread[] threads, int[] arr, int thread)
        {
            if (thread == 0)
            {
                _ThreadParams![thread] = ThreadParam.Create(arr, new Range(0, arr.Length / threads.Length));

                thread++;
                RangeSetRecursion(threads, arr, thread);
            }
            else if (thread < threads.Length - 1)
            {
                _ThreadParams![thread] = ThreadParam.Create(arr, new Range(
                    arr.Length / threads.Length * thread,
                    (arr.Length / threads.Length * thread) + (arr.Length / threads.Length)));

                thread++;
                RangeSetRecursion(threads, arr, thread);
            }
            else
            {
                _ThreadParams![thread] = ThreadParam.Create(arr, new Range(arr.Length / threads.Length * thread, arr.Length));
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