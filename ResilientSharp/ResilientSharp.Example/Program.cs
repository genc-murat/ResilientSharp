using Microsoft.Extensions.Logging;
using ResilientSharp;

var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<CircuitBreaker>();
var circuitBreaker = new CircuitBreaker(config => {
    config.MaxFailureCount = 3;
    config.MaxConcurrentRequests = 10;
    config.RetryCount = 2;
    config.SlowRequestThresholdCount = 4;
    config.SlowRequestThreshold = TimeSpan.FromSeconds(5);
    config.RetryStrategy = new ExponentialBackoffRetry(TimeSpan.FromSeconds(1),2);
}, logger);

try
{
    await circuitBreaker.ExecuteAsync(async () => {
        //await Task.Delay(TimeSpan.FromMilliseconds(230));
        throw new Exception("ddddd");
    });
}
catch (CircuitBrokenException e)
{
    Console.WriteLine($"Circuit is broken: {e.Message}");
}