namespace ResilientSharp;

/// <summary>
/// Manages the "Closed" state of a Circuit Breaker pattern.
/// In the Closed state, the circuit breaker executes actions normally.
/// </summary>
public class ClosedStateHandler : ICircuitStateHandler
{
    private CircuitBreaker _circuitBreaker;
    private CircuitBreakerConfig _config;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClosedStateHandler"/> class.
    /// </summary>
    /// <param name="circuitBreaker">The circuit breaker instance to manage.</param>
    /// <param name="config">Configuration settings for the circuit breaker.</param>
    public ClosedStateHandler(CircuitBreaker circuitBreaker, CircuitBreakerConfig config)
    {
        _circuitBreaker = circuitBreaker;
        _config = config;
    }

    /// <summary>
    /// Handles the behavior of the circuit breaker when it's in the Closed state.
    /// </summary>
    /// <param name="circuitBreaker">The circuit breaker instance.</param>
    /// <param name="token">Cancellation token for asynchronous operation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task HandleAsync(CircuitBreaker circuitBreaker, CancellationToken token)
    {
        // Currently, it does nothing and completes immediately.
        await Task.CompletedTask;
    }
}
