using System;
using System.Linq;
using System.Numerics;
using System.Threading;

namespace HW_12
{
    internal interface IThreadParamStrategy<T, TResult>
    {
        public bool HasIndex => false;
        public Range Range => default;
        public void ThreadMethod(object? obj);
        public TResult ThreadResult(ThreadParam<T, TResult>[] threadParams);
    }
}