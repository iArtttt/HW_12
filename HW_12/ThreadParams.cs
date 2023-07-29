using System;
using System.Numerics;
using System.Text;

namespace HW_12
{
    internal class SumThreadStrategy<T> : IThreadStrategy<T, ulong> where T : INumber<T>
    {
        public void ThreadMethod(object? obj)
        {
            var param = (ThreadParam<int, ulong>)obj!;
            var span = param.Data.Span;

            var sum = 0uL;
            for (int i = 0; i < span.Length; i++)
            {
                //TODO: need recalculate indexes from the relative to absolute values, or redesign a progress handling
                //param.ProgressUpdate(range, i);
                sum += (ulong)span[i];
            }
            param.Result = sum;
        }

        public ulong ThreadResult(ThreadParam<T, ulong>[] threadParam)
        {
            ulong result = 0;
            foreach (var thread in threadParam)
                result += thread.Result;
            return result;
        }
    }

    internal class AverageThreadStrategy<T> : IThreadStrategy<T, ulong> where T : INumber<T>
    {
        public void ThreadMethod(object? obj)
        {
            var param = (ThreadParam<ulong, ulong>)obj!;
            var span = param.Data.Span;

            ulong result = 0;
            for (int i = 0; i < span.Length; i++)
            {
                //TODO: need recalculate indexes from the relative to absolute values, or redesign a progress handling
                //param.ProgressUpdate(range, i);
                result += span[i];
            }
            param.Result = result / (ulong)span.Length;
        }

        public ulong ThreadResult(ThreadParam<T, ulong>[] threadParams)
        {
            ulong result = 0;
            foreach (var thread in threadParams)
                result += thread.Result;
            return result / (ulong)threadParams.Length;
        }
    }

    internal abstract class MaxMinThreadStrategy<T> : IThreadStrategy<T, T> where T : struct, INumber<T>
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

                //TODO: need recalculate indexes from the relative to absolute values, or redesign a progress handling
                //param.ProgressUpdate(range, i);
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

    internal class MaxThreadStrategy<T> : MaxMinThreadStrategy<T> where T : struct, INumber<T>
    {
        protected override int Compare(T result, T param) => result > param ? 1 : -1;
    }

    internal class MinThreadStrategy<T> : MaxMinThreadStrategy<T> where T : struct, INumber<T>
    {
        protected override int Compare(T result, T param) => result < param? -1 : 1;
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
                //TODO: need recalculate indexes from the relative to absolute values, or redesign a progress handling
                //param.ProgressUpdate(range, i);
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

    internal abstract class FrequencyDictionaryThreadStrategy<T> : IThreadStrategy<T, Dictionary<T, int>> where T : notnull
    {
        public void ThreadMethod(object? obj)
        {
            var param = (ThreadParam<T, Dictionary<T, int>>)obj!;
            var span = param.Data.Span;

            Dictionary<T, int> result = new();

            for (int i = 0; i < span.Length; i++)
            {
                if (result.ContainsKey(span[i]))
                    result[span[i]]++;
                else
                    result.Add(span[i], 1);

                //TODO: need recalculate indexes from the relative to absolute values, or redesign a progress handling
                //param.ProgressUpdate(range, i);
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
