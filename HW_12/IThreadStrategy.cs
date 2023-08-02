using System;
using System.Linq;
using System.Numerics;
using System.Threading;

namespace HW_12
{
    internal interface IThreadStrategy<T, TResult>
    {
        public void ThreadMethod(object? obj);
        public TResult ThreadResult(TaskParam<T, TResult>[] threadParams);
    }
    internal interface IInitParams
    {
        public TaskParam<T, TResult>[] Init<T, TResult>(Memory<T> data, int threadCount);
    }
}