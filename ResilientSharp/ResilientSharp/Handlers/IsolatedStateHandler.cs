namespace ResilientSharp;

/// <summary>
/// Manages the "Isolated" state of a Circuit Breaker pattern.
/// In the Isolated state, the circuit breaker prevents any actions from being executed.
/// </summary>
public class IsolatedStateHandler : ICircuitStateHandler
{
    private CircuitBreaker _circuitBreaker;
    private CircuitBreakerConfig _config;

    /// <summary>
    /// Initializes a new instance of the <see cref="IsolatedStateHandler"/> class.
    /// </summary>
    /// <param name="circuitBreaker">The circuit breaker instance to manage.</param>
    /// <param name="config">Configuration settings for the circuit breaker.</param>
    public IsolatedStateHandler(CircuitBreaker circuitBreaker, CircuitBreakerConfig config)
    {
        _circuitBreaker = circuitBreaker;
        _config = config;
    }

    /// <summary>
    /// Handles the behavior of the circuit breaker when it's in the Isolated state.
    /// All requests will fail immediately with a CircuitBrokenException.
    /// </summary>
    /// <param name="circuitBreaker">The circuit breaker instance.</param>
    /// <param name="token">Cancellation token for asynchronous operation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task HandleAsync(CircuitBreaker circuitBreaker, CancellationToken token)
    {
        throw new CircuitBrokenException("Circuit is isolated due to too many slow requests");
    }
}
