using Microsoft.Extensions.Logging;
using System.Diagnostics;


namespace ResilientSharp;

/// <summary>
/// Provides the implementation of a Circuit Breaker pattern.
/// This class provides a way to execute actions and commands with 
/// fault tolerance and latency management.
/// </summary>
public class CircuitBreaker : IDisposable
{
    private CircuitBreakerConfig _config;
    private ICircuitStateHandler _currentStateHandler;

    public int IsolateThresholdCount { get; set; }
    public bool IsManuallyClosed { get; private set; }
    public int SlowRequestCount { get; private set; }

    public int FailureCount { get; private set; }
    public int SuccessCount { get; set; }
    public int RequiredSuccessCount { get; set; }
    public int TotalRequestCount { get; private set; }

    public Func<Exception, bool> ExceptionFilter { get; set; }

    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    private readonly SemaphoreSlim _bulkheadSemaphore;

    private Dictionary<CircuitBreakerState, string> customErrorMessages = new Dictionary<CircuitBreakerState, string>();

    private DateTime _lastFailureTime;

    public ILogger Logger { get; }
    public DateTime LastStateChanged { get; private set; }

    private Timer _autoResetTimer;
    private readonly TimeSpan _autoResetInterval;
    private bool _isAutoResetEnabled;

    public CircuitBreakerState CurrentState => _currentStateHandler switch
    {
        ClosedStateHandler => CircuitBreakerState.Closed,
        OpenStateHandler => CircuitBreakerState.Open,
        HalfOpenStateHandler => CircuitBreakerState.HalfOpen,
        _ => throw new InvalidOperationException("Unknown state"),
    };

    public event Action<CircuitBreakerState, CircuitBreakerState> CircuitStateChanged;

    /// <summary>
    /// Initializes a new instance of the CircuitBreaker class with 
    /// the specified configuration, logger, and auto-reset interval.
    /// </summary>
    /// <param name="configAction">Configuration settings.</param>
    /// <param name="logger">Logging interface.</param>
    /// <param name="autoResetInterval">Auto reset interval time.</param>
    public CircuitBreaker(Action<CircuitBreakerConfig> configAction, ILogger logger = null, TimeSpan? autoResetInterval = null)
    {
        _config = new CircuitBreakerConfig();
        configAction(_config);

        _currentStateHandler = new ClosedStateHandler(this, _config);

        _bulkheadSemaphore = new SemaphoreSlim(_config.MaxConcurrentRequests, _config.MaxConcurrentRequests);

        Logger = logger;

        _autoResetInterval = autoResetInterval ?? TimeSpan.FromMinutes(5);
        _isAutoResetEnabled = autoResetInterval is not null;

        if (_isAutoResetEnabled)
        {
            StartAutoResetTimer();
        }

        _lastFailureTime = DateTime.MinValue;
    }

    /// <summary>
    /// Starts the timer for automatic resets of the circuit breaker.
    /// </summary>
    private void StartAutoResetTimer()
    {
        _autoResetTimer = new Timer(AutoReset, null, _autoResetInterval, Timeout.InfiniteTimeSpan);
    }

    /// <summary>
    /// Manually enables the automatic reset feature and starts the auto-reset timer.
    /// </summary>
    public void ManuallyEnableAutoReset()
    {
        _isAutoResetEnabled = true;
        StartAutoResetTimer();
    }

    /// <summary>
    /// Manually disables the automatic reset feature and stops the auto-reset timer.
    /// </summary>
    public void ManuallyDisableAutoReset()
    {
        _isAutoResetEnabled = false;
        StopAutoResetTimer();
    }

    /// <summary>
    /// Handles the timer callback to automatically reset the circuit breaker.
    /// </summary>
    /// <param name="state">The state object passed to the timer callback. Not used in this implementation.</param>
    private async void AutoReset(object state)
    {
        await ResetIsolate();

        await Log("Circuit breaker automatically reset.", LogLevel.Information);

        if (_isAutoResetEnabled)
        {
            StartAutoResetTimer();
        }
    }

    /// <summary>
    /// Stops the timer for automatic resets and disposes of the timer resource.
    /// </summary>
    private void StopAutoResetTimer()
    {
        _autoResetTimer?.Dispose();
    }

    /// <summary>
    /// Executes the provided action within the circuit breaker.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="timeoutInSeconds">The maximum time to allow the action to execute.</param>
    /// <param name="externalCancellationToken">Optional cancellation token.</param>
    public async Task ExecuteAsync(Func<Task> action, int timeoutInSeconds = 30, CancellationToken externalCancellationToken = default)
    {
        if (!_bulkheadSemaphore.Wait(0))
        {
            await Log("Bulkhead limit reached. Request is rejected.", LogLevel.Warning);
            return;
        }

        TotalRequestCount++;

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(externalCancellationToken);
        cts.CancelAfter(TimeSpan.FromSeconds(timeoutInSeconds));

        try
        {
            await _semaphore.WaitAsync(cts.Token);

            try
            {
                await _currentStateHandler.HandleAsync(this, cts.Token);
            }
            catch (CircuitBrokenException)
            {
                if (customErrorMessages.ContainsKey(CurrentState))
                {
                    throw new CircuitBrokenException(customErrorMessages[CurrentState]);
                }
                throw;
            }
            finally
            {
                _semaphore.Release();
            }

            var stopwatch = Stopwatch.StartNew();

            for (int i = 0; i <= _config.RetryCount; i++)
            {
                try
                {
                    await action();
                    stopwatch.Stop();
                    if (stopwatch.Elapsed > _config.SlowRequestThreshold)
                    {
                        SlowRequestCount++;
                        if (SlowRequestCount >= _config.SlowRequestThresholdCount)
                        {
                            await TransitionToState(new IsolatedStateHandler(this, _config));
                            await Log("Circuit is isolated due to too many slow requests.", LogLevel.Warning);
                        }
                        else
                        {
                            await Log($"Slow request detected. Elapsed time: {stopwatch.Elapsed}", LogLevel.Warning);
                        }
                    }
                    SuccessCount++;

                    if (CurrentState == CircuitBreakerState.HalfOpen && SuccessCount >= RequiredSuccessCount)
                    {
                        await TransitionToState(new ClosedStateHandler(this, _config));
                    }

                    FailureCount = 0;
                    await Log("Action succeeded.", LogLevel.Information);
                    break;
                }
                catch (Exception ex)
                {
                    if (DateTime.UtcNow - _lastFailureTime > _config.CoolDownPeriod)
                    {
                        FailureCount = 0;
                    }

                    _lastFailureTime = DateTime.UtcNow;

                    if (ExceptionFilter != null && !ExceptionFilter(ex))
                    {
                        await Log($"Action failed but exception is filtered: {ex.Message}", LogLevel.Warning);
                        break;
                    }

                    FailureCount++;
                    SuccessCount = 0;

                    if (FailureCount >= _config.MaxFailureCount)
                    {
                        await TransitionToState(new OpenStateHandler(this, _config));
                    }

                    await Log($"Action failed: {ex.Message}. Retry Count: {i + 1}", LogLevel.Error);

                    if (i < _config.RetryCount && _config.RetryStrategy is not null)
                    {
                        var nextDelay = _config.RetryStrategy.GetNextDelay(i);
                        await Task.Delay(nextDelay);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }
        finally
        {
            _semaphore.Release();
            _bulkheadSemaphore.Release();
        }
    }

    /// <summary>
    /// Disposes of the resources used by the CircuitBreaker.
    /// </summary>
    public void Dispose()
    {
        _semaphore?.Dispose();
        _bulkheadSemaphore?.Dispose();
        _autoResetTimer?.Dispose();
    }

    /// <summary>
    /// Manually closes the circuit breaker, moving it to the Closed state.
    /// </summary>
    public async Task ManuallyClose()
    {
        IsManuallyClosed = true;
        await TransitionToState(new ClosedStateHandler(this, _config));
    }

    /// <summary>
    /// Resets the circuit breaker from the Isolated state to the Closed state.
    /// </summary>
    public async Task ResetIsolate()
    {
        IsManuallyClosed = false;
        SlowRequestCount = 0;
        await TransitionToState(new ClosedStateHandler(this, _config));
        await Log("Circuit is manually reset from Isolated state.", LogLevel.Information);
    }

    /// <summary>
    /// Manually opens the circuit breaker, moving it to the Open state.
    /// </summary>
    /// <param name="reason">Reason for manually opening the circuit.</param>
    public async Task ManuallyOpen(string reason = null)
    {
        IsManuallyClosed = false;
        CircuitOpenReason = reason;
        await TransitionToState(new OpenStateHandler(this, _config));
    }

    private async Task Log(string message, LogLevel level)
    {
        await Task.Run(() => Logger?.Log(level, $"{message}, Time: {DateTime.UtcNow}, Total Requests: {TotalRequestCount}"));
    }

    public int TotalTransitions { get; private set; }
    public Action OnOpen { get; set; }
    public Action OnClose { get; set; }

    /// <summary>
    /// Handles the transition between different circuit breaker states.
    /// </summary>
    /// <param name="newStateHandler">The new state for the circuit breaker.</param>
    internal async Task TransitionToState(ICircuitStateHandler newStateHandler)
    {
        var oldState = CurrentState;
        _currentStateHandler = newStateHandler;
        LastStateChanged = DateTime.UtcNow;
        var newState = CurrentState;
        customErrorMessages[newState] = $"Circuit is now {newState}. Custom Error Message here.";
        CircuitStateChanged?.Invoke(oldState, CurrentState);
        await Log($"State changed from {oldState} to {newState}.", LogLevel.Information);
        TotalTransitions++;
        await Log($"Total state transitions: {TotalTransitions}", LogLevel.Information);
        if (newState == CircuitBreakerState.Open) OnOpen?.Invoke();
        if (newState == CircuitBreakerState.Closed) OnClose?.Invoke();
    }
    public string CircuitOpenReason { get; private set; }
}
