using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HW_12
{
    internal abstract class AvarageSumStrategy<T, TResult> : IThreadStrategy<T, TResult> where T : struct, INumber<T> //where TResult : INumber<T>
    {
        protected abstract TResult MethodResult(ulong result, ThreadParam<T, TResult> param);
        protected abstract TResult FinalAction(TResult result, ThreadParam<T, TResult> param);
        protected abstract TResult FinalResult(TResult result, ThreadParam<T, TResult>[] param);
        public void ThreadMethod(object? obj)
        {
            var param = (ThreadParam<T, TResult>)obj!;
            var span = param.Data.Span;
            ulong result = default;

            for (int i = 0; i < span.Length; i++)
            {
                result += Convert.ToUInt64(span[i]);
                param.ProgressUpdate(i);
            }

            param.Result = MethodResult(result, param);
        }

        public TResult ThreadResult(ThreadParam<T, TResult>[] threadParams)
        {
            TResult result = default;
            foreach (var thread in threadParams)
                result = FinalAction(result, thread);

            return FinalResult(result, threadParams);
        }
    }
    internal class SumThreadStrategy<T> : AvarageSumStrategy<T, ulong> where T : struct, INumber<T>
    {
        protected override ulong MethodResult(ulong result, ThreadParam<T, ulong> param) => result;
        protected override ulong FinalAction(ulong result, ThreadParam<T, ulong> param) => result + param.Result;
        protected override ulong FinalResult(ulong result, ThreadParam<T, ulong>[] param) => result;
    }
    internal class AverageThreadStrategy<T> : AvarageSumStrategy<T, decimal> where T : struct, INumber<T>
    {
        protected override decimal MethodResult(ulong result, ThreadParam<T, decimal> param) => result / (ulong)param.Data.Span.Length;        
        protected override decimal FinalAction(decimal result, ThreadParam<T, decimal> param) => result + param.Result;
        protected override decimal FinalResult(decimal result, ThreadParam<T, decimal>[] param) => (decimal)(result / param.Length);        
    }

    internal abstract class MaxMinStrategy<T> : IThreadStrategy<T, T> where T : struct, INumber<T>
    {
        public void ThreadMethod(object? obj)
        {
            var param = (ThreadParam<T, T>)obj!;
            var span = param.Data.Span;
            T result = default;
            for (int i = 0; i < span.Length; i++)
            {
                if (Compare(result, span[i]) < 0)
                    result = span[i];
                param.ProgressUpdate(i);                
            }
            param.Result = result;
        }
        protected abstract int Compare(T result, T compare);
        public T ThreadResult(ThreadParam<T, T>[] threadParams)
        {
            T result = default;
            foreach (var thread in threadParams)
            {
                if (Compare(result, thread.Result) < 0)
                    result = thread.Result;
            }
            return result;
        }
    }
    internal class MaxThreadStrategy<T> : MaxMinStrategy<T> where T : struct, INumber<T>
    {
        protected override int Compare(T result, T param) => result > param ? 1 : -1;
    }
    internal class MinThreadStrategy<T> : MaxMinStrategy<T> where T : struct, INumber<T>
    {
        protected override int Compare(T result, T param) => result > param? -1 : 1;
        
    }

    internal class CopyThreadStrategy<T> : IInitParams, IThreadStrategy<T, T[]>
    {
        public Range Range { get; }
        public CopyThreadStrategy(int startIndex, int lastIndex)
        {
            Range = new Range(startIndex, lastIndex);
        }

        public void ThreadMethod(object? obj)
        {
            var param = (ThreadParam<T, T[]>)obj!;
            var span = param.Data.Span;
            T[] result = new T[span.Length];

            for (int i = 0, j = 0; i < span.Length; i++, j++)
            {
                result[j] = span[i];
                param.ProgressUpdate(i);
            }
            param.Result = result;
        }
        public T[] ThreadResult(ThreadParam<T, T[]>[] threadParams) => threadParams.SelectMany(s => s.Result!).ToArray();
        public ThreadParam<T1, TResult>[] Init<T1, TResult>(Memory<T1> data, int threadCount)
        {
            var result = new ThreadParam<T1, TResult>[threadCount];
            data = data[Range]; // slice all data to the initial copy range

            var itemsCount = data.Length / threadCount;
            for (int i = 0; i < threadCount; i++)
            {
                result[i] = ThreadParam<T1, TResult>.Create(data.Slice(i * itemsCount, itemsCount), i);
            }

            return result;
        }
    }

    internal class FrequencyDictionaryThreadStrategy<T> : IThreadStrategy<T, Dictionary<T, int>> where T: notnull
    {
        public void ThreadMethod(object? obj)
        {
            var param = (ThreadParam<T, Dictionary<T, int>>)obj!;
            var span = param.Data.Span;
            Dictionary<T, int> result = new();

            for (int i = 0; i < span.Length; i++)
            {
                if (result.ContainsKey(span[i]))
                {
                    result[span[i]]++;
                    param.ProgressUpdate(i);
                }
                else
                {
                    result.Add(span[i], 1);
                    param.ProgressUpdate(i);
                }
            }
            param.Result = result;
        }

        public Dictionary<T, int> ThreadResult(ThreadParam<T, Dictionary<T, int>>[] threadParams)
        {
            Dictionary<T, int> result = new();
            foreach (var thread in threadParams)
            {
                foreach (var param in thread.Result!)
                {
                    if (result.ContainsKey(param.Key))
                    {
                        result[param.Key] += param.Value;
                    }
                    else
                    {
                        result.Add(param.Key, param.Value);
                    }
                }
            }
            return result;
        }
    }
}
