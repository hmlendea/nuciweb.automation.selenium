# Concurrency and Scheduling

## Purpose

Documents threading, async patterns, and scheduling considerations for the NuciWeb.Automation.Selenium library.

## Threading Model

### SeleniumWebProcessor

**Thread Safety:** Not thread-safe.

- Each `SeleniumWebProcessor` instance should be used by a single thread
- WebDriver instances are not thread-safe
- Concurrent access to the same processor instance may cause undefined behaviour

**Recommended Pattern:**

```csharp
// One processor per thread
IWebDriver driver = WebDriverInitialiser.InitialiseAvailableWebDriver(false, 90);
IWebProcessor processor = new SeleniumWebProcessor(driver);

// Use processor on this thread only
processor.GoToUrl("https://example.com");
```

### WebDriverInitialiser

**Thread Safety:** Thread-safe.

- Static methods can be called concurrently
- No shared mutable state
- Safe to call from multiple threads simultaneously

```csharp
// Safe concurrent calls
var task1 = Task.Run(() => WebDriverInitialiser.InitialiseFirefoxDriver(false, 90));
var task2 = Task.Run(() => WebDriverInitialiser.InitialiseChromeDriver(false, 90));
```

## Async Patterns

### Current State

The library uses synchronous operations exclusively:

- All `IWebProcessor` methods are synchronous
- `WebDriverInitialiser` methods are synchronous
- No `async`/`await` patterns implemented

### Implications

- Blocking operations may block the calling thread
- No built-in support for concurrent page operations
- Long-running operations (page loads, script execution) block the thread

### Future Considerations

If async support is added:

1. All methods should return `Task` or `Task<T>`
2. Use `WebDriverWait` for explicit waits
3. Consider `Task.Run` for CPU-bound operations
4. Maintain backward compatibility with synchronous overloads

## Scheduling

### Page Load Timeout

- **Default:** 90 seconds
- **Configurable:** Via `pageLoadTimeout` parameter
- **Scope:** Per WebDriver instance
- **Behaviour:** Throws `TimeoutException` if exceeded

### Implicit Waits

- **Not configured:** No implicit waits set
- **Rationale:** Explicit waits preferred for reliability
- **Impact:** Element lookups fail immediately if not found

### Script Timeout

- **Not configured:** No script timeout set
- **Default:** Selenium default (typically 0 = no timeout)
- **Impact:** Long-running scripts may block indefinitely

## Concurrent Operations

### Multiple Browser Instances

```csharp
// Create multiple independent instances
var drivers = new List<IWebDriver>();
var processors = new List<IWebProcessor>();

for (int i = 0; i < 3; i++)
{
    var driver = WebDriverInitialiser.InitialiseAvailableWebDriver(false, 90);
    var processor = new SeleniumWebProcessor(driver);
    drivers.Add(driver);
    processors.Add(processor);
}

// Each processor operates independently
// No shared state between instances
```

### Parallel Test Execution

For parallel test execution:

1. Each test should create its own WebDriver instance
2. Each test should create its own SeleniumWebProcessor instance
3. No shared state between tests
4. Use separate browser profiles if needed

## Resource Management

### WebDriver Lifecycle

- **Creation:** Via `WebDriverInitialiser`
- **Usage:** Through `SeleniumWebProcessor`
- **Disposal:** Call `driver.Quit()` when done
- **Cleanup:** Always use `try`/`finally` or `using` pattern

```csharp
IWebDriver driver = WebDriverInitialiser.InitialiseAvailableWebDriver(false, 90);
try
{
    IWebProcessor processor = new SeleniumWebProcessor(driver);
    processor.GoToUrl("https://example.com");
}
finally
{
    driver.Quit();
}
```

### Memory Considerations

- WebDriver instances consume significant memory
- Each browser instance runs as a separate process
- Consider limiting concurrent instances
- Monitor memory usage in long-running applications

## Lock Strategy

### Current Implementation

- No explicit locks used
- Relies on WebDriver's internal synchronisation
- Single-threaded usage assumed

### If Concurrency is Needed

```csharp
private readonly object _lock = new object();

public void SafeOperation(IWebProcessor processor)
{
    lock (_lock)
    {
        processor.GoToUrl("https://example.com");
        processor.Click("//button[@id='submit']");
    }
}
```

## Related Documentation

- [architecture.md](./architecture.md) - Architecture overview
- [api-reference/selenium-web-processor.md](./api-reference/selenium-web-processor.md) - Processor API
- [api-reference/web-driver-initialiser.md](./api-reference/web-driver-initialiser.md) - Initialiser API
- [error-handling.md](./error-handling.md) - Error handling patterns
- [testing.md](./testing.md) - Test strategy