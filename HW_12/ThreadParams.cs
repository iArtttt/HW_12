using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HW_12
{
    internal class SumThreadParam<T> : ThreadsUse<T, ulong>, IThreadParamStrategy<T, ulong> where T : INumber<T>
    {
        public SumThreadParam(T[] arr) : base(arr)
        {
        }

        public void ThreadMethod(object? obj)
        {
            var param = (ThreadParam<int, ulong>)obj!;
            var range = param.Range;

            var sum = 0uL;
            for (int i = range.Start.Value; i < range.End.Value; i++)
            {
                param.Pr(range, i);
                sum += (ulong)param[i];
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
    internal class AverageThreadParam<T> : ThreadsUse<T, ulong>, IThreadParamStrategy<T, ulong> where T : INumber<T>
    {
        public AverageThreadParam(T[] arr) : base(arr)
        {
        }

        public void ThreadMethod(object? obj)
        {
            var param = (ThreadParam<ulong, ulong>)obj!;
            var range = param.Range;
            ulong result = 0;

            for (int i = range.Start.Value; i < range.End.Value; i++)
            {
                result += (ulong)param[i];
                param.Pr(range, i);
            }
            param.Result = result / (ulong)(range.End.Value - range.Start.Value);
        }

        public ulong ThreadResult(ThreadParam<T, ulong>[] threadParams)
        {
            ulong result = 0;
            foreach (var thread in threadParams)
                result += thread.Result;
            return result / (ulong)threadParams.Length;
        }
    }

    internal abstract class MaxMin<T> : ThreadsUse<T, T>, IThreadParamStrategy<T, T> where T : INumber<T>
    {
        public MaxMin(T[] arr) : base(arr)
        {
        }
        public void ThreadMethod(object? obj)
        {
            var param = (ThreadParam<T, T>)obj!;
            var range = param.Range;
            T result = default;
            for (int i = range.Start.Value; i < range.End.Value; i++)
            {
                if (Compare(result, param[i]) > 0)
                    result = param[i];
                param.Pr(range, i);                
            }
            param.Result = result;
        }
        protected virtual int Compare(T result, T compare)
        {
            throw new NotImplementedException();
        }
        public T ThreadResult(ThreadParam<T, T>[] threadParams)
        {
            T result = default;
            foreach (var thread in threadParams)
            {
                if (Compare(result, thread.Result) > 0)
                    result = thread.Result;
            }
            return result;
        }
    }
    internal class MaxThreadParam<T> : MaxMin<T> where T : INumber<T>
    {
        public MaxThreadParam(T[] arr) : base(arr)
        {
        }

        protected override int Compare(T result, T param) => result > param ? 1 : -1;
    }
    internal class MinThreadParam<T> : MaxMin<T> where T : INumber<T>
    {
        public MinThreadParam(T[] arr) : base(arr)
        {
        }

        protected override int Compare(T result, T param) => result < param? -1 : 1;
        
    }

    internal class CopyThreadParam<T> : ThreadsUse<T, T[]>, IThreadParamStrategy<T, T[]>
    {
        public bool HasIndex => true;
        public Range Range { get; }
        public CopyThreadParam(T[] arr, int startIndex, int lastIndex) : base(arr)
        {
            Range = new Range(startIndex, lastIndex);
        }
        public void ThreadMethod(object? obj)
        {
            var param = (ThreadParam<T, T[]>)obj!;
            var range = param.Range;
            T[] result = new T[range.End.Value - range.Start.Value];

            for (int i = range.Start.Value, j = 0; i < range.End.Value; i++, j++)
            {
                result[j] = param[i];
                param.Pr(range, i);
            }
            param.Result = result;
        }
        public T[] ThreadResult(ThreadParam<T, T[]>[] threadParams) => threadParams.SelectMany(s => s.Result!).ToArray();
    }

    internal abstract class FrequencyDictionaryThreadParam<T> : ThreadsUse<T, Dictionary<T, int>>, IThreadParamStrategy<T, Dictionary<T, int>>
    {
        protected FrequencyDictionaryThreadParam(T[] arr) : base(arr)
        {
        }

        public void ThreadMethod(object? obj)
        {
            var param = (ThreadParam<T, Dictionary<T, int>>)obj!;
            var range = param.Range;
            Dictionary<T, int> result = new();

            for (int i = range.Start.Value; i < range.End.Value; i++)
            {
                if (result.ContainsKey(param[i]))
                {
                    result[param[i]]++;
                    param.Pr(range, i);
                }
                else
                {
                    result.Add(param[i], 1);
                    param.Pr(range, i);
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
    internal class FrequencyStringDictionaryThreadParam : FrequencyDictionaryThreadParam<string>
    {
        public FrequencyStringDictionaryThreadParam(string[] arr) : base(arr)
        {
        }
    }
    internal class FrequencyCharDictionaryThreadParam : FrequencyDictionaryThreadParam<char>
    {
        public FrequencyCharDictionaryThreadParam(char[] arr) : base(arr)
        {
        }
    }
}
