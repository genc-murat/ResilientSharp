namespace ResilientSharp;

/// <summary>
/// Implements a composite retry strategy, allowing for different retry strategies to be used in combination.
/// </summary>
public class CompositeRetry : IRetryStrategy
{
    private readonly IRetryStrategy[] _strategies;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompositeRetry"/> class.
    /// </summary>
    /// <param name="strategies">An array of IRetryStrategy implementations that should be used for the retry logic.</param>
    public CompositeRetry(params IRetryStrategy[] strategies)
    {
        _strategies = strategies;
    }

    /// <summary>
    /// Calculates the next delay time for a retry based on the given retry count.
    /// </summary>
    /// <param name="retryCount">The number of retries that have been attempted.</param>
    /// <returns>The delay time as a TimeSpan.</returns>
    public TimeSpan GetNextDelay(int retryCount)
    {
        var index = retryCount % _strategies.Length;
        return _strategies[index].GetNextDelay(retryCount);
    }
}
