namespace HW_12
{
    class ThreadParam<T, TResult>
    {
        private readonly T[] _arr;
        public Range Range { get; }
        public static object SyncRoot => new();
        public T this[int index] => _arr[index];
        public TResult? Result { get; set; } = default;
        public int Position { get; }
        private readonly IThreadStrategy<T, TResult>? _param;

        private ThreadParam(T[] arr, Range range)
        {
            _arr = arr;
            Range = range;
        }
        private ThreadParam(T[] arr, Range range, int position)
        {
            _arr = arr;
            Range = range;
            Position = position;
        }
        public static ThreadParam<T, TResult> Create(T[] arr, Range range)
        {
            return new ThreadParam<T, TResult>(arr, range);
        }
        public static ThreadParam<T, TResult> Create(T[] arr, Range range, int position)
        {
            return new ThreadParam<T, TResult>(arr, range, position);
        }
        /// <summary>
        /// IThreadParamStrategy<T, TResult> _param
        /// </summary>
        public void Do()
        {
            _param.ThreadMethod(this);
        }
    }
    
}