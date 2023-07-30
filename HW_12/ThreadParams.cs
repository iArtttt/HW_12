using System;
using System.Numerics;
using System.Text;
using System.Threading;

namespace HW_12
{
    internal abstract class ThreadStrategyBase<T, TResult> : IThreadStrategy<T, TResult>
    {
        protected abstract TResult? ProcessItem(T item, TResult? result, int index, ThreadParam<T, TResult> param);
        protected abstract TResult? ProcessResultItem(TResult? item, TResult? result, ThreadParam<T, TResult> param);
        protected abstract TResult? ProcessResult(TResult? result, params ThreadParam<T, TResult>[] threadParams);

        public void ThreadMethod(object? obj)
        {
            var param = (ThreadParam<T, TResult>)obj!;
            var span = param.Data.Span;

            var result = default(TResult);
            for (int i = 0; i < span.Length; i++)
            {
                //TODO: need recalculate indexes from the relative to absolute values, or redesign a progress handling
                //param.ProgressUpdate(range, i);
                result = ProcessItem(span[i], result, i, param);
            }

            param.Result = ProcessResult(result, param);
        }

        public TResult? ThreadResult(ThreadParam<T, TResult>[] threadParams)
        {
            TResult? result = default;
            foreach(var thread in threadParams)
                result = ProcessResultItem(thread.Result, result, thread);
            return ProcessResult(result, threadParams);
        }
    }

    internal abstract class SumAverageThreadStrategyBase<T, TResult> : ThreadStrategyBase<T, TResult>
        where T : struct, IAdditionOperators<T, TResult, TResult>, INumber<T>
        where TResult : struct, INumber<TResult>
    {
        protected override TResult ProcessItem(T item, TResult result, int index, ThreadParam<T, TResult> param)
            => item + result;

        protected override TResult ProcessResultItem(TResult item, TResult result, ThreadParam<T, TResult> param)
            => item + result;
    }

    internal class SumThreadStrategy<T> : SumAverageThreadStrategyBase<T, ulong>
        where T : struct, IAdditionOperators<T, ulong, ulong>, INumber<T>
    {
        protected override ulong ProcessResult(ulong result, params ThreadParam<T, ulong>[] threadParams)
            => result;
    }

    internal class AverageThreadStrategy<T> : SumAverageThreadStrategyBase<T, decimal>
        where T : struct, IAdditionOperators<T, decimal, decimal>, INumber<T>
    {
        protected override decimal ProcessResult(decimal result, params ThreadParam<T, decimal>[] threadParams)
        {
            if (threadParams.Length <= 0) throw new ArgumentException(nameof(threadParams));

            var length = threadParams.Length == 1 ? threadParams[0].Data.Length : threadParams.Length;
            return result / length;
        }
    }

    internal abstract class MaxMinThreadStrategyBase<T> : ThreadStrategyBase<T, T> where T : struct, INumber<T>
    {
        protected override T ProcessResult(T result, params ThreadParam<T, T>[] threadParams)
            => result;

        protected override T ProcessResultItem(T item, T result, ThreadParam<T, T> param)
            => result;
    }

    internal class MaxThreadStrategy<T> : MaxMinThreadStrategyBase<T> where T : struct, INumber<T>
    {
        protected override T ProcessItem(T item, T result, int index, ThreadParam<T, T> param)
            => result < item ? item : result;
    }

    internal class MinThreadStrategy<T> : MaxMinThreadStrategyBase<T> where T : struct, INumber<T>
    {
        protected override T ProcessItem(T item, T result, int index, ThreadParam<T, T> param)
            => result > item ? item : result;
    }

    internal class CopyThreadStrategy<T> : ThreadStrategyBase<T, T[]>, IInitParams
    {
        public Range Range { get; }

        public CopyThreadStrategy(int startIndex, int lastIndex)
        {
            Range = new Range(startIndex, lastIndex);
        }

        public ThreadParam<T1, TResult>[] Init<T1, TResult>(Memory<T1> data, int threadCount)
        {
            var result = new ThreadParam<T1, TResult>[threadCount];
            data = data[Range]; // slice all data to the initial copy range

            var itemsCount = data.Length / threadCount;
            for (int i = 0; i < threadCount; i++)
            {
                result[i] = ThreadParam<T1, TResult>.Create(
                    i == threadCount - 1 ? data[(i * itemsCount)..] : data.Slice(i * itemsCount, itemsCount)
                    , i);
            }

            return result;
        }

        protected override T[]? ProcessItem(T item, T[]? result, int index, ThreadParam<T, T[]> param)
        {
            result![index] = item;
            return result;
        }

        protected override T[]? ProcessResult(T[]? result, params ThreadParam<T, T[]>[] threadParams)
        {
            return threadParams.Length == 1 ? result : threadParams.SelectMany(s => s.Result!).ToArray();
        }

        protected override T[]? ProcessResultItem(T[]? item, T[]? result, ThreadParam<T, T[]> param)
        {
            return item;
        }
    }

    internal class FrequencyDictionaryThreadStrategy<T> : ThreadStrategyBase<T, Dictionary<T, int>> where T : notnull
    {
        protected override Dictionary<T, int>? ProcessItem(T item, Dictionary<T, int>? result, int index, ThreadParam<T, Dictionary<T, int>> param)
        {
            if (result!.ContainsKey(item))
                result[item]++;
            else
                result.Add(item, 1);
            return result;
        }

        protected override Dictionary<T, int>? ProcessResult(Dictionary<T, int>? result, params ThreadParam<T, Dictionary<T, int>>[] threadParams)
        {
            return result;
        }

        protected override Dictionary<T, int>? ProcessResultItem(Dictionary<T, int>? item, Dictionary<T, int>? result, ThreadParam<T, Dictionary<T, int>> param)
        {
            foreach (var value in item!)
            {
                if (result!.ContainsKey(value.Key))
                {
                    result[value.Key] += value.Value;
                }
                else
                {
                    result.Add(value.Key, value.Value);
                }
            }

            return result;
        }
    }
}
