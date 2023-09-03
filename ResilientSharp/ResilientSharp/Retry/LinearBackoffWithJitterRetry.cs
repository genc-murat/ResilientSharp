namespace ResilientSharp;

/// <summary>
/// Implements a linear backoff with jitter retry strategy, allowing delays between retry attempts to increase linearly while introducing randomness.
/// </summary>
public class LinearBackoffWithJitterRetry : IRetryStrategy
{
    private readonly TimeSpan _initialDelay;
    private readonly TimeSpan _increment;
    private readonly Random _random;

    /// <summary>
    /// Initializes a new instance of the <see cref="LinearBackoffWithJitterRetry"/> class.
    /// </summary>
    /// <param name="initialDelay">The initial delay for the first retry.</param>
    /// <param name="increment">The increment added to the initial delay for subsequent retries.</param>
    public LinearBackoffWithJitterRetry(TimeSpan initialDelay, TimeSpan increment)
    {
        _initialDelay = initialDelay;
        _increment = increment;
        _random = new Random();
    }

    /// <summary>
    /// Calculates the next delay time for a retry based on the given retry count using a linear backoff with jitter strategy.
    /// </summary>
    /// <param name="retryCount">The number of retries that have been attempted.</param>
    /// <returns>The delay time as a TimeSpan.</returns>
    public TimeSpan GetNextDelay(int retryCount)
    {
        var baseDelay = _initialDelay.TotalMilliseconds + _increment.TotalMilliseconds * retryCount;
        var jitter = _random.Next((int)(baseDelay * 0.9), (int)(baseDelay * 1.1));
        return TimeSpan.FromMilliseconds(jitter);
    }
}
