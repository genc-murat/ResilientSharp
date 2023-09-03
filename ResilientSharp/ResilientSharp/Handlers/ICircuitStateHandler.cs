namespace ResilientSharp;

/// <summary>
/// Defines the behavior for handling the different states of a Circuit Breaker.
/// </summary>
public interface ICircuitStateHandler
{
    /// <summary>
    /// Handles the behavior of the circuit breaker when it is in a specific state.
    /// </summary>
    /// <param name="circuitBreaker">The circuit breaker instance being managed.</param>
    /// <param name="token">Cancellation token for asynchronous operation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task HandleAsync(CircuitBreaker circuitBreaker, CancellationToken token);
}
