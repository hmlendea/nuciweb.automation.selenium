# Error Handling

## Purpose

Documents the error taxonomy, handling patterns, and recovery strategies for the NuciWeb.Automation.Selenium library.

## Error Taxonomy

### WebDriver Exceptions

| Exception Type | Scenario | Recovery |
|----------------|----------|----------|
| `NoSuchElementException` | Element not found in DOM | Retry with explicit wait, verify selector, check page load |
| `StaleElementReferenceException` | Element removed from DOM | Re-find element, retry operation |
| `TimeoutException` | Operation exceeded timeout | Increase timeout, check network, verify page state |
| `WebDriverException` | General WebDriver error | Check browser/driver compatibility, review logs |
| `InvalidSelectorException` | Invalid XPath/CSS selector | Validate selector syntax, test manually |
| `SessionNotCreatedException` | Browser driver not found | Install driver, verify PATH, check browser version |

### Library-Specific Exceptions

The library does not define custom exceptions. It surfaces Selenium exceptions directly.

## Handling Patterns

### Try/Catch Pattern

```csharp
try
{
    processor.Click("//button[@id='submit']");
}
catch (NoSuchElementException ex)
{
    Console.WriteLine($"Element not found: {ex.Message}");
    // Handle missing element gracefully
}
```

### Validation Before Operation

```csharp
// Check element exists before interacting
if (processor.DoesElementExist("//input[@name='username']"))
{
    processor.SetText("//input[@name='username']", "myuser");
    processor.Click("//button[@type='submit']");
}
else
{
    Console.WriteLine("Username field not found on page");
}
```

### Timeout Configuration

```csharp
// Configure appropriate timeout
IWebDriver driver = WebDriverInitialiser.InitialiseAvailableWebDriver(
    isDebugModeEnabled: false,
    pageLoadTimeout: 120); // 2 minutes
```

### Error Context

Always include context when logging errors:

```csharp
try
{
    processor.GoToUrl("https://example.com/login");
}
catch (Exception ex)
{
    Log.Error($"Failed to navigate to login page: {ex.Message}");
    Log.Debug($"URL: https://example.com/login");
    Log.Debug($"Processor state: {processor.GetPageSource()}");
}
```

## Recovery Strategies

### Element Not Found

1. **Verify selector:** Check XPath/CSS is correct
2. **Wait for page:** Ensure page has loaded
3. **Retry:** Attempt operation again
4. **Fallback:** Use alternative selector
5. **Abort:** Log error and continue gracefully

### Page Load Failure

1. **Increase timeout:** Use longer pageLoadTimeout
2. **Check network:** Verify connectivity
3. **Retry navigation:** Navigate again
4. **Check URL:** Validate target URL
5. **Abort:** Log error and handle gracefully

### Alert Not Present

1. **Check for alert:** Verify alert is present
2. **Wait for alert:** Use explicit wait pattern
3. **Dismiss gracefully:** Handle absence
4. **Retry:** Attempt again after condition met

## Logging Integration

The library integrates with NuciWeb logging framework:

```csharp
// Error logging
processor.Click("//button[@id='submit']");

// On success: no logging needed
// On failure: exception is thrown and should be logged by caller
```

## Error Examples

### Successful Operation

```csharp
// No exception - operation succeeded
processor.GoToUrl("https://example.com");
Console.WriteLine("Navigation successful");
```

### Element Not Found

```csharp
try
{
    processor.Click("//button[@id='missing']");
}
catch (NoSuchElementException ex)
{
    // Expected handling
    Console.WriteLine($"Button not found: {ex.Message}");
}
```

### Navigation Timeout

```csharp
try
{
    processor.GoToUrl("https://slow-page.com");
}
catch (TimeoutException ex)
{
    // Handle timeout
    Console.WriteLine($"Page load timed out: {ex.Message}");
}
```

## Best Practices

1. **Always wrap WebDriver operations** in try/catch when user interaction may fail
2. **Include context** in error messages (URL, selector, operation)
3. **Use appropriate timeouts** for the environment
4. **Validate preconditions** before operations (element exists, page loaded)
5. **Log errors with context** for debugging
6. **Handle gracefully** - don't crash user application
7. **Document expected exceptions** in method documentation
8. **Provide fallback behavior** when elements not found

## Related Documentation

- [architecture.md](./architecture.md) - Architecture overview
- [design-decisions.md](./design-decisions.md) - Design rationale
- [concurrency-and-scheduling.md](./concurrency-and-scheduling.md) - Threading model
- [quick-start.md](./quick-start.md) - Getting started guide
- [troubleshooting.md](./troubleshooting.md) - Common issues