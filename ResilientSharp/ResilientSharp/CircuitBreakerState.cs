namespace ResilientSharp;

/// <summary>
/// Represents the different states that a Circuit Breaker can be in.
/// </summary>
public enum CircuitBreakerState
{
    /// <summary>
    /// Indicates that the circuit is closed and requests are flowing freely.
    /// </summary>
    Closed,

    /// <summary>
    /// Indicates that the circuit is open and no requests are allowed.
    /// </summary>
    Open,

    /// <summary>
    /// Indicates that the circuit is in a half-open state and is allowing a limited number of requests to test if the underlying operation is working.
    /// </summary>
    HalfOpen,

    /// <summary>
    /// Indicates that the circuit is in an isolated state, generally manually set to halt all requests regardless of prior failures.
    /// </summary>
    Isolated
}