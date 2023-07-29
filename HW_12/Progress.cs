namespace HW_12
{
    internal static class Progress
    {
        public static void Pr<T, TResult>(this ThreadParam<T, TResult> param, Range range, int progress)
        {
            if ((progress + 1) % (((decimal)range.End.Value - range.Start.Value) / 100) == 0 || (progress + 1) % range.End.Value == 0)
            {
                Console.SetCursorPosition(0, param.Position + 6);
                Console.WriteLine($"Thread {param.Position + 1} Progress: {(int)((decimal)(progress - range.Start.Value) / (range.End.Value - range.Start.Value) * 100)}% of 100%");
            }
        }
    }
}