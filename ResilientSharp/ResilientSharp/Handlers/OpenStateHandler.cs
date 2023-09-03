using System.Threading;
using System.Threading.Tasks;

namespace ResilientSharp;

/// <summary>
/// Manages the "Open" state of a Circuit Breaker pattern.
/// In the Open state, the circuit breaker prevents any actions from being executed and delays the transition to the Half-Open state.
/// </summary>
public class OpenStateHandler : ICircuitStateHandler
{
    private CircuitBreaker _circuitBreaker;
    private CircuitBreakerConfig _config;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenStateHandler"/> class.
    /// </summary>
    /// <param name="circuitBreaker">The circuit breaker instance to manage.</param>
    /// <param name="config">Configuration settings for the circuit breaker.</param>
    public OpenStateHandler(CircuitBreaker circuitBreaker, CircuitBreakerConfig config)
    {
        _circuitBreaker = circuitBreaker;
        _config = config;
    }

    /// <summary>
    /// Handles the behavior of the circuit breaker when it's in the Open state.
    /// Waits for a configurable duration before throwing a CircuitBrokenException.
    /// </summary>
    /// <param name="circuitBreaker">The circuit breaker instance.</param>
    /// <param name="token">Cancellation token for the asynchronous operation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task HandleAsync(CircuitBreaker circuitBreaker, CancellationToken token)
    {
        await Task.Delay(_config.OpenToHalfOpenWaitTime, token);
        var reason = string.IsNullOrWhiteSpace(_circuitBreaker.CircuitOpenReason) ? "" : $" Reason: {_circuitBreaker.CircuitOpenReason}.";
        throw new CircuitBrokenException($"Circuit is open.{reason}");
    }
}
