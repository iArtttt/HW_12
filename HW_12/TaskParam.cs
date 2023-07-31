namespace HW_12
{
    class TaskParam<T, TResult>
    {
        public Memory<T> Data { get; }
        public TResult? Result { get; set; } = default;
        public int Position { get; }

        private TaskParam(Memory<T> arr)
        {
            Data = arr;
        }

        private TaskParam(Memory<T> arr, int position)
        {
            Data = arr;
            Position = position;
        }
        public static TaskParam<T, TResult> Create(Memory<T> arr) => new TaskParam<T, TResult>(arr);
        public static TaskParam<T, TResult> Create(Memory<T> arr, int position) => new TaskParam<T, TResult>(arr, position);
    }
    
}