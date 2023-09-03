namespace ResilientSharp;

/// <summary>
/// Defines the contract for implementing different retry strategies.
/// </summary>
public interface IRetryStrategy
{
    /// <summary>
    /// Calculates the next delay time based on the given retry count.
    /// </summary>
    /// <param name="retryCount">The number of retries that have been attempted.</param>
    /// <returns>The delay time as a TimeSpan.</returns>
    TimeSpan GetNextDelay(int retryCount);
}
