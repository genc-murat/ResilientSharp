# ResilientSharp: A Circuit Breaker Library

[![Build Status](https://travis-ci.com/yourusername/ResilientSharp.svg?branch=main)](https://travis-ci.com/yourusername/ResilientSharp)
[![NuGet](https://img.shields.io/nuget/v/ResilientSharp.svg)](https://www.nuget.org/packages/ResilientSharp/)

## Overview

ResilientSharp is a library that provides a simple yet flexible way to add resilience and fault-tolerance to your application using the Circuit Breaker pattern.

## Features

- **Fault Tolerance**: Automatically handles faults by encapsulating the logic of preventing a failure from constantly recurring.
  
- **Bulkhead Isolation**: Limits concurrent requests, providing isolation during failure.

- **State Management**: Closed, Open, and Half-Open states managed automatically.

- **Auto-Reset**: Option to automatically reset the state after a configurable timeout.

- **Retries**: Configurable retry counts and delay strategy.

- **Custom Exception Handling**: Define what exceptions should be treated as failures.

- **Logging**: Integrated with `Microsoft.Extensions.Logging` for detailed logging.

## Installation

```sh
dotnet add package ResilientSharp
```

## Usage

### Basic Usage

```csharp
var config = new CircuitBreakerConfig
{
    MaxFailureCount = 5,
    CoolDownPeriod = TimeSpan.FromSeconds(15),
    //... other settings
};

var circuitBreaker = new CircuitBreaker(config => 
{
    config.MaxFailureCount = 5;
    config.CoolDownPeriod = TimeSpan.FromSeconds(15);
});

await circuitBreaker.ExecuteAsync(async () => {
    // Your logic here
});
```

### With Logging

```csharp
var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("CircuitBreaker");
var circuitBreaker = new CircuitBreaker(config, logger);
```

### Exception Handling

```csharp
circuitBreaker.ExceptionFilter = (ex) => {
    return ex is CustomException;
};
```