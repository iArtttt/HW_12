using System.Diagnostics;
using System.Threading;

namespace HW_12
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Console.Write("Please write how much threads do you want to use? --> ");
            //var threadsCount = int.Parse(Console.ReadLine());
            Thread[] threads = ThreadsUse.ThreadsCount();

            Random random = new Random();
            int[] arr = new int[100];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = i + 1;
            }

            ThreadsUse.SumThreadParamCreate(threads, arr);

            ThreadsUse.Print();
        }

        static class ThreadsUse
        {
            private static SumThreadParam[]? _SumThreadParams;
            private static ulong _result = 0;
            private static readonly Stopwatch _timer = new();

            public static Thread[] ThreadsCount()
            {
                Console.Write("Please write how much threads do you want to use? --> ");
                var threadsCount = int.Parse(Console.ReadLine());
                if (threadsCount <= 0) threadsCount = 1;

                Thread[] threads = new Thread[threadsCount];
                Feel(threads);

                return threads;
            }
            public static void Feel(Thread[] threads)
            {
                for (int i = 0; i < threads.Length; i++)
                {
                    threads[i] = new Thread(ProcessThread);
                }
                _SumThreadParams = new SumThreadParam[threads.Length];
            }
            public static void SumThreadParamCreate(Thread[] threads, int[] arr)
            {
                if (_SumThreadParams == null) return;

                if (threads.Length == 1) _SumThreadParams[0] = SumThreadParam.Create(arr, new Range(0, arr.Length));
                else RangeSet(threads, arr);


                _timer.Start();
                for (int i = 0; i < threads.Length; i++)
                    threads[i].Start(_SumThreadParams[i]);
                for (int i = 0; i < threads.Length; i++)
                    threads[i].Join();
                foreach (var thread in _SumThreadParams)
                    _result += thread.Result;
                _timer.Stop();
            }
            private static void RangeSet(Thread[] threads, int[] arr)
            {
                RangeSetReverse(threads, arr, 0);
            }
            private static void RangeSetReverse(Thread[] threads, int[] arr, int thread)
            {
                if (thread == 0)
                {
                    _SumThreadParams![thread] = SumThreadParam.Create(arr, new Range(0, arr.Length / threads.Length));

                    thread++;
                    RangeSetReverse(threads, arr, thread);
                }
                else if (thread < threads.Length - 1)
                {
                    _SumThreadParams![thread] = SumThreadParam.Create(arr, new Range(
                        arr.Length / threads.Length * thread,
                        (arr.Length / threads.Length * thread) + (arr.Length / threads.Length)));

                    thread++;
                    RangeSetReverse(threads, arr, thread);
                }
                else
                {
                    _SumThreadParams![thread] = SumThreadParam.Create(arr, new Range(arr.Length / threads.Length * thread, arr.Length));
                }
            }
            public static void Print()
            {
                Console.WriteLine($"Result is {_result}; Time is {_timer.Elapsed}");
            }
        }

        class SumThreadParam
        {
            private readonly int[] _arr;
            public Range Range { get; }
            public static object SyncRoot => new();
            public int this[int index] => _arr[index];
            public ulong Result { get; set; } = 0;

            private SumThreadParam(int[] arr, Range range)
            {
                _arr = arr;
                Range = range;
            }
            public static SumThreadParam Create(int[] arr, Range range)
            {
                return new SumThreadParam(arr, range);
            }
        }

        static void ProcessThread(object? obj)
        {
            var param = (SumThreadParam)obj!;
            var range = param.Range;

            var sum = 0uL;
            for (int i = range.Start.Value; i < range.End.Value; i++)
            {
                sum += (ulong)param[i];
            }
            param.Result = sum;
        }
    }
}