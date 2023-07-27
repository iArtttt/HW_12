using System.Numerics;

namespace HW_12
{
    internal interface IThreadParam<T, TResult>
    {
        public bool HasIndex => false;
        public int StartIndex => default;
        public int LastIndex => default;
        public void ThreadMethod(object? obj);
        public TResult ThreadResult(ThreadParam<T, TResult>[] threadParams);
    }

    internal class SumThreadParam<T> : IThreadParam<T, ulong> where T : INumber<T>
    {
        public void ThreadMethod(object? obj)
        {
            var param = (ThreadParam<int, ulong>)obj!;
            var range = param.Range;

            var sum = 0uL;
            for (int i = range.Start.Value; i < range.End.Value; i++)
            {
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

    internal class MaxThreadParam<T> : IThreadParam<T, ulong> where T : INumber<T>
    {
        public void ThreadMethod(object? obj)
        {
            var param = (ThreadParam<T, ulong>)obj!;
            var range = param.Range;
            var result = 0ul;

            for (int i = range.Start.Value; i < range.End.Value; i++)
            {
                if (param[i] is ulong item && item > result ) result = item;
            }
            param.Result = result;
        }

        public ulong ThreadResult(ThreadParam<T, ulong>[] threadParams)
        {
            ulong result = 0;
            foreach (var thread in threadParams)
                result = result > thread.Result ? result : thread.Result;
            return result;
        }
    }

    internal class MinThreadParam<T> : IThreadParam<T, ulong> where T : INumber<T>
    {
        public void ThreadMethod(object? obj)
        {
            var param = (ThreadParam<T,ulong>)obj!;
            var range = param.Range;
            var result = 0ul;

            for (int i = range.Start.Value; i < range.End.Value; i++)
            {
                if (param[i] is ulong item && item < result) result = item;
            }
            param.Result = result;
        }

        public ulong ThreadResult(ThreadParam<T, ulong>[] threadParams)
        {
            ulong result = 0;
            foreach (var thread in threadParams)
                result = result < thread.Result ? result : thread.Result;
            return result;
        }
    }

    internal class AverageThreadParam<T> : IThreadParam<T, ulong> where T : INumber<T>
    {
        public void ThreadMethod(object? obj)
        {
            var param = (ThreadParam<ulong, ulong>)obj!;
            var range = param.Range;
            ulong result = 0;

            for (int i = range.Start.Value; i < range.End.Value; i++)
            {
                result += (ulong)param[i];
            }
            param.Result = result / (ulong)(range.End.Value - range.Start.Value);
        }

        public ulong ThreadResult(ThreadParam<T,ulong>[] threadParams)
        {
            ulong result = 0;
            foreach (var thread in threadParams)
                result += thread.Result;
            return result / (ulong)threadParams.Length;
        }
    }

    internal class CopyThreadParam<T> : IThreadParam<T, T[]>
    {
        public bool HasIndex => true;
        private readonly int _startIndex;
        private readonly int _lastIndex;
        public int StartIndex => _startIndex;
        public int LastIndex => _lastIndex;
        public CopyThreadParam(int startIndex, int lastIndex)
        {
            _startIndex = startIndex;
            _lastIndex = lastIndex;
        }
        public void ThreadMethod(object? obj)
        {
            var param = (ThreadParam<T, T[]>)obj!;
            var range = param.Range;
            T[] result = new T[range.End.Value - range.Start.Value];

            for (int i = range.Start.Value, j = 0; i < range.End.Value; i++, j++)
            {
                result[j] = param[i];
            }
            param.Result = result;
        }

        public T[] ThreadResult(ThreadParam<T, T[]>[] threadParams)
        {
            List<T> result = new();
            foreach (var thread in threadParams)
                result.AddRange(thread.Result);
            return result.ToArray();
        }
    }

    internal class FrequencyStringDictionaryThreadParam : IThreadParam<string, Dictionary<string, int>>
    {
        public void ThreadMethod(object? obj)
        {
            var param = (ThreadParam<string, Dictionary<string, int>>)obj!;
            var range = param.Range;
            Dictionary<string, int> result = new();

            for (int i = range.Start.Value; i < range.End.Value; i++)
            {
                if (result.Keys.Contains(param[i]))
                {
                    result[param[i]]++;
                }
                else
                {
                    result.Add(param[i],1);
                }
            }
            param.Result = result;
        }

        public Dictionary<string, int> ThreadResult(ThreadParam<string, Dictionary<string, int>>[] threadParams)
        {
            Dictionary<string, int> result = new();
            foreach (var thread in threadParams)
            {
                foreach (var param in thread.Result)
                {
                    if (result.Keys.Contains(param.Key))
                    {
                        result[param.Key]+= param.Value;
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

    internal class FrequencyCharDictionaryThreadParam : IThreadParam<char, Dictionary<char, int>>
    {
        public void ThreadMethod(object? obj)
        {
            var param = (ThreadParam<char, Dictionary<char,int>>)obj!;
            var range = param.Range;
            Dictionary<char, int> result = new();

            for (int i = range.Start.Value; i < range.End.Value; i++)
            {
                if (result.Keys.Contains(param[i]))
                {
                    result[param[i]]++;
                }
                else
                {
                    result.Add(param[i],1);
                }
            }
            param.Result = result;
        }

        public Dictionary<char, int> ThreadResult(ThreadParam<char, Dictionary<char, int>>[] threadParams)
        {
            Dictionary<char, int> result = new();
            foreach (var thread in threadParams)
            {
                foreach (var param in thread.Result)
                {
                    if (result.Keys.Contains(param.Key))
                    {
                        result[param.Key]+= param.Value;
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