namespace ResilientSharp;

/// <summary>
/// Implements an exponential backoff retry strategy, allowing delays between retry attempts to grow exponentially.
/// </summary>
public class ExponentialBackoffRetry : IRetryStrategy
{
    private readonly TimeSpan _initialDelay;
    private readonly double _factor;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExponentialBackoffRetry"/> class.
    /// </summary>
    /// <param name="initialDelay">The initial delay for the first retry.</param>
    /// <param name="factor">The exponential factor by which the delay will increase.</param>
    public ExponentialBackoffRetry(TimeSpan initialDelay, double factor)
    {
        _initialDelay = initialDelay;
        _factor = factor;
    }

    /// <summary>
    /// Calculates the next delay time for a retry based on the given retry count using exponential backoff.
    /// </summary>
    /// <param name="retryCount">The number of retries that have been attempted.</param>
    /// <returns>The delay time as a TimeSpan.</returns>
    public TimeSpan GetNextDelay(int retryCount)
    {
        return TimeSpan.FromMilliseconds(_initialDelay.TotalMilliseconds * Math.Pow(_factor, retryCount));
    }
}
