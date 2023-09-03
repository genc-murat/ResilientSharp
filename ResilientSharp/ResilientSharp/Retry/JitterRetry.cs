namespace ResilientSharp;

/// <summary>
/// Implements a jitter retry strategy, allowing random delays between retry attempts.
/// </summary>
public class JitterRetry : IRetryStrategy
{
    private readonly TimeSpan _initialDelay;
    private readonly Random _random;

    /// <summary>
    /// Initializes a new instance of the <see cref="JitterRetry"/> class.
    /// </summary>
    /// <param name="initialDelay">The initial delay that will be used as a basis for random jitter calculations.</param>
    public JitterRetry(TimeSpan initialDelay)
    {
        _initialDelay = initialDelay;
        _random = new Random();
    }

    /// <summary>
    /// Calculates a random delay time for a retry based on jitter.
    /// </summary>
    /// <param name="retryCount">The number of retries that have been attempted. Not used in this strategy.</param>
    /// <returns>A random delay time as a TimeSpan.</returns>
    public TimeSpan GetNextDelay(int retryCount)
    {
        var jitter = _random.Next((int)(_initialDelay.TotalMilliseconds * 0.5), (int)(_initialDelay.TotalMilliseconds * 1.5));
        return TimeSpan.FromMilliseconds(jitter);
    }
}
