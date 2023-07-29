using System;
using System.Linq;
using System.Numerics;
using System.Threading;

namespace HW_12
{
    internal interface IThreadStrategy<T, TResult>
    {
        public void ThreadMethod(object? obj);
        public TResult ThreadResult(ThreadParam<T, TResult>[] threadParams);
    }

    internal interface IInitParams
    {
        public ThreadParam<T, TResult>[] Init<T, TResult>(Memory<T> data, int threadCount);
    }
}