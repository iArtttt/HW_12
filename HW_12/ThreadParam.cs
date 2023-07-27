namespace HW_12
{    
    class ThreadParam<T,TResult>
    {
        private readonly T[] _arr;
        public Range Range { get; }
        public static object SyncRoot => new();
        public T this[int index] => _arr[index];
        public TResult Result { get; set; } = default;

        private ThreadParam(T[] arr, Range range)
        {
            _arr = arr;
            Range = range;
        }
        public static ThreadParam<T, TResult> Create(T[] arr, Range range)
        {
            return new ThreadParam<T, TResult>(arr, range);
        }
    }
    
}