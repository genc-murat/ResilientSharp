namespace ResilientSharp;

/// <summary>
/// Manages the "Half-Open" state of a Circuit Breaker pattern.
/// In the Half-Open state, the circuit breaker allows a limited number of actions to proceed
/// to test if the underlying system has recovered.
/// </summary>
public class HalfOpenStateHandler : ICircuitStateHandler
{
    private CircuitBreaker _circuitBreaker;
    private CircuitBreakerConfig _config;

    /// <summary>
    /// Initializes a new instance of the <see cref="HalfOpenStateHandler"/> class.
    /// </summary>
    /// <param name="circuitBreaker">The circuit breaker instance to manage.</param>
    /// <param name="config">Configuration settings for the circuit breaker.</param>
    public HalfOpenStateHandler(CircuitBreaker circuitBreaker, CircuitBreakerConfig config)
    {
        _circuitBreaker = circuitBreaker;
        _config = config;
    }

    /// <summary>
    /// Handles the behavior of the circuit breaker when it's in the Half-Open state.
    /// Checks if enough successful actions have occurred to transition to the Closed state.
    /// </summary>
    /// <param name="circuitBreaker">The circuit breaker instance.</param>
    /// <param name="token">Cancellation token for asynchronous operation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task HandleAsync(CircuitBreaker circuitBreaker, CancellationToken token)
    {
        if (_circuitBreaker.SuccessCount >= _circuitBreaker.RequiredSuccessCount)
        {
            await _circuitBreaker.TransitionToState(new ClosedStateHandler(_circuitBreaker, _config));
            _circuitBreaker.SuccessCount = 0;
        }
        await Task.CompletedTask;
    }
}
