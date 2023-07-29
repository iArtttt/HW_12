namespace HW_12
{
    class ThreadParam<T, TResult>
    {
        public static ThreadParam<T, TResult> Create(Memory<T> arr)
        {
            return new ThreadParam<T, TResult>(arr);
        }

        public static ThreadParam<T, TResult> Create(Memory<T> arr, int position)
        {
            return new ThreadParam<T, TResult>(arr, position);
        }

        public Memory<T> Data { get; }

        public TResult? Result { get; set; } = default;

        public int Position { get; }

        private ThreadParam(Memory<T> arr)
        {
            Data = arr;
        }

        private ThreadParam(Memory<T> arr, int position)
        {
            Data = arr;
            Position = position;
        }
    }
}