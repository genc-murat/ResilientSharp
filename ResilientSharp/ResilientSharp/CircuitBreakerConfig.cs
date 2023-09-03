namespace ResilientSharp;

/// <summary>
/// Configuration class for Circuit Breaker. Provides various settings to fine-tune the behavior of a Circuit Breaker instance.
/// </summary>
public class CircuitBreakerConfig
{
    /// <summary>
    /// Gets or sets the wait time before transitioning from Open to Half-Open state.
    /// </summary>
    public TimeSpan OpenToHalfOpenWaitTime { get; set; } = TimeSpan.FromMilliseconds(500);

    /// <summary>
    /// Gets or sets the maximum number of allowed failures before transitioning to Open state.
    /// </summary>
    public int MaxFailureCount { get; set; } = 3;

    /// <summary>
    /// Gets or sets the number of retries to attempt when a request fails.
    /// </summary>
    public int RetryCount { get; set; } = 3;

    /// <summary>
    /// Gets or sets the retry strategy to use for retries.
    /// </summary>
    public IRetryStrategy? RetryStrategy { get; set; }

    /// <summary>
    /// Gets or sets the threshold time for considering a request as slow.
    /// </summary>
    public TimeSpan SlowRequestThreshold { get; set; } = TimeSpan.FromMilliseconds(5000);

    /// <summary>
    /// Gets or sets the number of slow requests that trigger transition to Open state.
    /// </summary>
    public int SlowRequestThresholdCount { get; set; } = 5;

    /// <summary>
    /// Gets or sets the maximum number of concurrent requests allowed.
    /// </summary>
    public int MaxConcurrentRequests { get; set; } = 3;

    /// <summary>
    /// Gets or sets the cool down period before resetting the failure and slow request counts.
    /// </summary>
    public TimeSpan CoolDownPeriod { get; set; } = TimeSpan.FromMilliseconds(10000);
}
