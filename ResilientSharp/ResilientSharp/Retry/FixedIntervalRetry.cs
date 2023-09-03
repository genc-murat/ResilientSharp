namespace ResilientSharp;

/// <summary>
/// Implements a fixed interval retry strategy, where the time between each retry attempt remains constant.
/// </summary>
public class FixedIntervalRetry : IRetryStrategy
{
    private readonly TimeSpan _fixedDelay;

    /// <summary>
    /// Initializes a new instance of the <see cref="FixedIntervalRetry"/> class.
    /// </summary>
    /// <param name="fixedDelay">The fixed delay for all retries.</param>
    public FixedIntervalRetry(TimeSpan fixedDelay)
    {
        _fixedDelay = fixedDelay;
    }

    /// <summary>
    /// Calculates the next delay time for a retry, which is constant for all retries.
    /// </summary>
    /// <param name="retryCount">The number of retries that have been attempted. Not used in this strategy.</param>
    /// <returns>The delay time as a TimeSpan.</returns>
    public TimeSpan GetNextDelay(int retryCount)
    {
        return _fixedDelay;
    }
}
