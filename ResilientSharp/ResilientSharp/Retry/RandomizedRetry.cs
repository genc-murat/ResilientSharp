namespace ResilientSharp;

/// <summary>
/// Implements a randomized retry strategy, allowing for a random delay between retry attempts.
/// </summary>
public class RandomizedRetry : IRetryStrategy
{
    private readonly TimeSpan _minDelay;
    private readonly TimeSpan _maxDelay;
    private readonly Random _random;

    /// <summary>
    /// Initializes a new instance of the <see cref="RandomizedRetry"/> class.
    /// </summary>
    /// <param name="minDelay">The minimum delay for a retry attempt.</param>
    /// <param name="maxDelay">The maximum delay for a retry attempt.</param>
    public RandomizedRetry(TimeSpan minDelay, TimeSpan maxDelay)
    {
        _minDelay = minDelay;
        _maxDelay = maxDelay;
        _random = new Random();
    }

    /// <summary>
    /// Calculates the next delay time for a retry based on random selection within the min and max delay range.
    /// </summary>
    /// <param name="retryCount">The number of retries that have been attempted. This parameter is not used in this strategy.</param>
    /// <returns>A random delay time as a TimeSpan.</returns>
    public TimeSpan GetNextDelay(int retryCount)
    {
        var randomDelay = _random.Next((int)_minDelay.TotalMilliseconds, (int)_maxDelay.TotalMilliseconds);
        return TimeSpan.FromMilliseconds(randomDelay);
    }
}
