using System.Diagnostics;
using System.Threading;
using System.Timers;

namespace HW_12
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] arr = new int[1_000_000];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = i + 1;
            }

            Console.Write("Please write how much threads do you want to use? --> ");
            var threadsCount = int.Parse(Console.ReadLine());
            if (threadsCount <= 0) threadsCount = 1;

            var threadProcessor = new ThreadsUse<int, int>(threadsCount, arr, new MaxThreadParam<int>());
            threadProcessor.ThreadDo();
            threadProcessor.Print();
        }
    }
}