using System.Diagnostics;
using System.Threading;
using System.Timers;

namespace HW_12
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Random random = new Random();
            int[] arr = new int[random.Next(100,10000000)];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = i+1;
            }
            Stopwatch sw = new Stopwatch();
            ulong result = 0;
            sw.Start();
            for (int i = 0; i < arr.Length; i++)
            {
                result += (ulong)arr[i];
            }

            //result /= (ulong)arr.Length;
            sw.Stop();
            Console.WriteLine($"Result is {result}; Time is {sw.Elapsed}");


            var tr = new ThreadsUse(arr);
            tr.ThreadDo(new SumThreadParam());

            tr.Print();
        }
    }
}