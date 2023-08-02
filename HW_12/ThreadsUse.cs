using System.Diagnostics;

namespace HW_12
{
    internal class ThreadsUse<T, TResult>
    {
        private readonly IThreadStrategy<T, TResult> _strategy;
        private readonly T[] _arr;
        private readonly Stopwatch _timer = new();
        private readonly CancellationTokenSource _cancelTokenSource;
        private TaskParam<T, TResult>[]? _taskParams;
        private Task[] _tasks;
        public TResult? Result { get; private set; } = default;

        public ThreadsUse(int taskCount, T[] arr, IThreadStrategy<T, TResult> strategy)
        {
            if (taskCount <= 0) throw new ArgumentException("thread count must be more than 0", nameof(taskCount));
            if (arr == null) throw new ArgumentNullException(nameof(arr));
            if (arr.Length <= 0) throw new ArgumentOutOfRangeException(nameof(arr));
            if (strategy == null) throw new ArgumentNullException(nameof(strategy));

            _arr = arr;
            _strategy = strategy;
            _tasks = new Task[taskCount];
            _cancelTokenSource = new CancellationTokenSource();
        }

        public async Task<TResult?> ThreadDo()
        {
            _taskParams = new TaskParam<T, TResult>[_tasks.Length];

            if (_taskParams == null || _strategy == default) throw new InvalidOperationException();

            if (_strategy is IInitParams initParams)
            {
                _taskParams = initParams.Init<T, TResult>(_arr.AsMemory(), _tasks.Length);
                // FeelTask перет же _taskParams?!
                FeelTask();
            }
            else
            {
                FeelTask();
            }
            
            try
            {
                _timer.Start();

                for (int i = 0; i < _tasks.Length; i++)
                    _tasks[i].Start();

                var abortTask = ToAbort(); // just fire abort task and didn't wait it

                await Task.WhenAll(_tasks);
                _cancelTokenSource.Cancel(); // stop abort task (and other misc tasks)

                Result = _strategy.ThreadResult(_taskParams);
            }
            catch (Exception ex)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n{ex.Message}\n");
                Console.ResetColor();
            }
            finally 
            { 
                _timer.Stop(); 
                _cancelTokenSource.Dispose();
            }

            return Result;
        }
        public void Print()
        {
            Console.WriteLine($"Result is {Result}; Time is {_timer.Elapsed}");
        }
        private void FeelTask()
        {
            var data = _arr.AsMemory();
            var itemsCount = data.Length / _tasks.Length;
            
            for (int i = 0; i < _taskParams.Length; i++)
            {
                _taskParams[i] = TaskParam<T, TResult>.Create(data.Slice(i * itemsCount, itemsCount), i);
                var task = _taskParams[i];
                _tasks[i] = new Task(() => _strategy.ThreadMethod(task), _cancelTokenSource.Token);
            }
        }

        private Task ToAbort()
        {
            return Task.Run(() =>
            {
                while (!_tasks.Any(t => t.IsCompleted) && !_cancelTokenSource.IsCancellationRequested)
                {
                    Console.SetCursorPosition(10, 1);
                    Console.WriteLine("Press 'Escape' to interrupt threads");

                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                    {
                        if (!_cancelTokenSource.IsCancellationRequested)
                        {
                            _cancelTokenSource.Cancel();
                            Console.Clear();
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"\nTask was canseled\n");
                            Console.ResetColor();
                        }
                    }
                }
            });
        }

    }
}