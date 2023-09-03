namespace ResilientSharp;

/// <summary>
/// Implements a Fibonacci backoff retry strategy, allowing delays between retry attempts to grow based on the Fibonacci sequence.
/// </summary>
public class FibonacciRetry : IRetryStrategy
{
    private readonly TimeSpan _initialDelay;

    /// <summary>
    /// Initializes a new instance of the <see cref="FibonacciRetry"/> class.
    /// </summary>
    /// <param name="initialDelay">The initial delay for the first retry.</param>
    public FibonacciRetry(TimeSpan initialDelay)
    {
        _initialDelay = initialDelay;
    }

    /// <summary>
    /// Calculates the next delay time for a retry based on the given retry count using the Fibonacci sequence.
    /// </summary>
    /// <param name="retryCount">The number of retries that have been attempted.</param>
    /// <returns>The delay time as a TimeSpan.</returns>
    public TimeSpan GetNextDelay(int retryCount)
    {
        long Fib(int n)
        {
            long a = 0;
            long b = 1;

            for (int i = 0; i < n; i++)
            {
                long temp = a;
                a = b;
                b = temp + b;
            }
            return a;
        }

        return TimeSpan.FromMilliseconds(_initialDelay.TotalMilliseconds * Fib(retryCount));
    }
}
