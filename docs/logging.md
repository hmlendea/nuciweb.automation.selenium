# Logging

## Purpose

Documents the logging framework, levels, structured fields, correlation, and sinks used by the NuciWeb.Automation.Selenium library.

## Logging Framework

The library uses the NuciWeb framework's logging infrastructure. Logging is not configured directly by this library but is inherited from the NuciWeb.Automation and NuciWeb packages.

### Logger Initialisation

Loggers are obtained through the NuciWeb logging system:

```csharp
// In application code using this library
ILogger logger = Log.ForContext<SeleniumWebProcessor>();
```

### Logger Scope

- **SeleniumWebProcessor:** Logs internal operations and errors
- **WebDriverInitialiser:** Logs driver creation and configuration

## Log Levels

The library uses standard NuciWeb log levels:

| Level | Usage |
|-------|-------|
| `Debug` | Detailed diagnostic information |
| `Information` | General operational information |
| `Warning` | Potential issues that don't prevent operation |
| `Error` | Error conditions that prevent operation |
| `Fatal` | Critical errors causing application failure |

## Structured Logging

The library supports structured logging with the following common properties:

### SeleniumWebProcessor Properties

| Property | Type | Description |
|----------|------|-------------|
| `Operation` | string | Name of the operation (Click, SetText, etc.) |
| `Selector` | string | XPath selector used |
| `ElementCount` | int | Number of matching elements |
| `Success` | bool | Whether operation succeeded |
| `DurationMs` | int | Operation duration in milliseconds |

### WebDriverInitialiser Properties

| Property | Type | Description |
|----------|------|-------------|
| `BrowserType` | string | Firefox or Chrome |
| `DebugMode` | bool | Whether debug mode is enabled |
| `PageLoadTimeout` | int | Configured timeout in seconds |
| `DriverPath` | string | Path to driver executable |
| `UserAgent` | string | Fetched user agent string |

## Correlation

The library does not implement correlation IDs directly. Correlation is handled by:

1. **Application Code:** Callers should provide correlation context
2. **NuciWeb Framework:** May provide correlation infrastructure
3. **Manual Logging:** Include operation context in log messages

### Example with Context

```csharp
// Application should provide correlation
logger.Information("Starting login flow", new {
    CorrelationId = correlationId,
    Username = username
});

try
{
    processor.GoToUrl("https://example.com/login");
    logger.Information("Navigated to login page", new {
        CorrelationId = correlationId,
        Url = "https://example.com/login"
    });
}
catch (Exception ex)
{
    logger.Error(ex, "Failed to navigate to login page", new {
        CorrelationId = correlationId,
        Url = "https://example.com/login"
    });
}
```

## Log Sinks

The library does not configure log sinks. Sinks are configured by the application or the NuciWeb framework:

### Common Sinks

- **Console:** Development and testing
- **File:** Production logging
- **Seq:** Structured logging server
- **Elasticsearch:** Log aggregation
- **Application Insights:** Azure monitoring

## Logging Examples

### Successful Operation

```csharp
// In SeleniumWebProcessor.cs
logger.Debug("Click operation started", new {
    Operation = "Click",
    Selector = "//button[@id='submit']"
});

processor.Click("//button[@id='submit']");

logger.Debug("Click operation completed", new {
    Operation = "Click",
    Selector = "//button[@id='submit']",
    Success = true
});
```

### Failed Operation

```csharp
// In SeleniumWebProcessor.cs
logger.Debug("SetText operation started", new {
    Operation = "SetText",
    Selector = "//input[@name='username']",
    Text = "myuser"
});

try
{
    processor.SetText("//input[@name='username']", "myuser");
    logger.Debug("SetText operation completed", new {
        Operation = "SetText",
        Selector = "//input[@name='username']",
        Success = true
    });
}
catch (Exception ex)
{
    logger.Error(ex, "SetText operation failed", new {
        Operation = "SetText",
        Selector = "//input[@name='username']",
        Success = false
    });
    throw;
}
```

### Driver Initialisation

```csharp
// In WebDriverInitialiser.cs
logger.Debug("Firefox driver creation started", new {
    BrowserType = "Firefox",
    DebugMode = isDebugModeEnabled,
    PageLoadTimeout = pageLoadTimeout
});

var driver = new FirefoxDriver(options);

logger.Debug("Firefox driver creation completed", new {
    BrowserType = "Firefox",
    DebugMode = isDebugModeEnabled,
    PageLoadTimeout = pageLoadTimeout,
    DriverPath = FirefoxDriverPath
});
```

## Configuration

Logging configuration is inherited from the application or NuciWeb framework:

### Minimum Log Level

Configured by the application:

```csharp
// Application configuration
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();
```

### Enrichers

Common enrichers used:

- **ThreadId:** Current thread identifier
- **MachineName:** Host machine name
- **ProcessId:** Current process ID
- **ApplicationName:** Name of the application

## Related Documentation

- [architecture.md](./architecture.md) - Architecture overview
- [design-decisions.md](./design-decisions.md) - Design rationale
- [error-handling.md](./error-handling.md) - Error handling patterns
- [quick-start.md](./quick-start.md) - Getting started guide
- [troubleshooting.md](./troubleshooting.md) - Logging troubleshooting