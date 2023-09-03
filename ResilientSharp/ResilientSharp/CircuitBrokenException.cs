namespace ResilientSharp;

/// <summary>
/// Exception thrown when a Circuit Breaker is in the open state and does not permit further operations.
/// </summary>
public class CircuitBrokenException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CircuitBrokenException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public CircuitBrokenException(string message) : base(message) { }
}
