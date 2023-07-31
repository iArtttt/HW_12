namespace HW_12
{
    internal static class Progress
    {
        public static void ProgressUpdate<T, TResult>(this TaskParam<T, TResult> param, int progress)
        {
            if ((progress + 1) % (param.Data.Span.Length / 100) == 0 || (progress + 1) % param.Data.Span.Length == 0)
            {
                Console.SetCursorPosition(0, param.Position + 6);
                Console.WriteLine($"Thread {param.Position + 1} Progress: {(int)((decimal) progress / param.Data.Span.Length * 100)}% of 100%");
            }
        }
    }
}