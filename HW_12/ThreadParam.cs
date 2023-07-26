namespace HW_12
{    
    class ThreadParam
    {
        private readonly int[] _arr;
        public Range Range { get; }
        public static object SyncRoot => new();
        public int this[int index] => _arr[index];
        public ulong Result { get; set; } = 0;

        private ThreadParam(int[] arr, Range range)
        {
            _arr = arr;
            Range = range;
        }
        public static ThreadParam Create(int[] arr, Range range)
        {
            return new ThreadParam(arr, range);
        }
    }
    
}