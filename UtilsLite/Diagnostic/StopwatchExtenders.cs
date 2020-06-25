using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace UtilsLite.Diagnostic
{
    public class ExecutionResult<T>
    {
        public T Data { get; set; }
        public TimeSpan Elapsed { get; set; }
    }

    public static class StopwatchExtenders
    {
        public static ExecutionResult<T> MeasureExecution<T>(this Stopwatch sw, Func<T> func)
        {
            sw.Restart();
            var result = func.Invoke();
            sw.Stop();
            return new ExecutionResult<T>
            {
                Data = result,
                Elapsed = sw.Elapsed
            };
        }

        public static async Task<ExecutionResult<T>> MeasureExecutionAsync<T>(this Stopwatch sw, Func<Task<T>> func)
        {
            sw.Restart();
            var result = await func.Invoke();
            sw.Stop();
            return new ExecutionResult<T>
            {
                Data = result,
                Elapsed = sw.Elapsed
            };
        }
    }
}