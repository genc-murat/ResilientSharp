namespace ResilientSharp;

/// <summary>
/// Implements an incremental retry strategy, allowing delays between retry attempts to grow incrementally.
/// </summary>
public class IncrementalRetry : IRetryStrategy
{
    private readonly TimeSpan _initialDelay;
    private readonly TimeSpan _incrementalDelay;

    /// <summary>
    /// Initializes a new instance of the <see cref="IncrementalRetry"/> class.
    /// </summary>
    /// <param name="initialDelay">The initial delay for the first retry.</param>
    /// <param name="incrementalDelay">The incremental delay that will be added to the initial delay for subsequent retries.</param>
    public IncrementalRetry(TimeSpan initialDelay, TimeSpan incrementalDelay)
    {
        _initialDelay = initialDelay;
        _incrementalDelay = incrementalDelay;
    }

    /// <summary>
    /// Calculates the next delay time for a retry based on the given retry count using an incremental strategy.
    /// </summary>
    /// <param name="retryCount">The number of retries that have been attempted.</param>
    /// <returns>The delay time as a TimeSpan.</returns>
    public TimeSpan GetNextDelay(int retryCount)
    {
        return _initialDelay + TimeSpan.FromMilliseconds(_incrementalDelay.TotalMilliseconds * retryCount);
    }
}
