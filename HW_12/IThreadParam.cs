namespace HW_12
{
    internal interface IThreadParam
    {
        public void ThreadMethod(object? obj);
        public ulong ThreadResult(ThreadParam[] threadParams);
    }

    internal class SumThreadParam : IThreadParam
    {
        public void ThreadMethod(object? obj)
        {
            var param = (ThreadParam)obj!;
            var range = param.Range;

            var sum = 0uL;
            for (int i = range.Start.Value; i < range.End.Value; i++)
            {
                sum += (ulong)param[i];
            }
            param.Result = sum;
        }

        public ulong ThreadResult(ThreadParam[] threadParam)
        {
            ulong result = 0;
            foreach (var thread in threadParam)
                result += thread.Result;
            return result;
        }
    }

    internal class MaxThreadParam : IThreadParam
    {
        public void ThreadMethod(object? obj)
        {
            var param = (ThreadParam)obj!;
            var range = param.Range;
            var result = 0;

            for (int i = range.Start.Value; i < range.End.Value; i++)
            {
                if (result < param[i]) result = param[i];
            }
            param.Result = (ulong)result;
        }

        public ulong ThreadResult(ThreadParam[] threadParams)
        {
            ulong result = 0;
            foreach (var thread in threadParams)
                result = result > thread.Result ? result : thread.Result;
            return result;
        }
    }

    internal class MinThreadParam : IThreadParam
    {
        public void ThreadMethod(object? obj)
        {
            var param = (ThreadParam)obj!;
            var range = param.Range;
            var result = 0;

            for (int i = range.Start.Value; i < range.End.Value; i++)
            {
                if (result > param[i]) result = param[i];
            }
            param.Result = (ulong)result;
        }

        public ulong ThreadResult(ThreadParam[] threadParams)
        {
            ulong result = 0;
            foreach (var thread in threadParams)
                result = result < thread.Result ? result : thread.Result;
            return result;
        }
    }

    internal class AverageThreadParam : IThreadParam
    {
        public void ThreadMethod(object? obj)
        {
            var param = (ThreadParam)obj!;
            var range = param.Range;
            ulong result = 0;

            for (int i = range.Start.Value; i < range.End.Value; i++)
            {
                result += (ulong)param[i];
            }
            param.Result = result / (ulong)(range.End.Value - range.Start.Value);
        }

        public ulong ThreadResult(ThreadParam[] threadParams)
        {
            ulong result = 0;
            foreach (var thread in threadParams)
                result += thread.Result;
            return result / (ulong)threadParams.Length;
        }
    }

}