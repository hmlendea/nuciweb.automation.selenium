# State and Persistence

## Purpose

Documents state management, stores, caches, and migrations for the NuciWeb.Automation.Selenium library.

## State Management

### Overview

The library does not implement its own state management. State is managed by:

1. **Browser State:** Handled by Selenium WebDriver
2. **NuciWeb Framework:** May provide application-level state
3. **Application Code:** Caller manages application state

### State Types

| State Type | Managed By | Description |
|------------|------------|-------------|
| **Session State** | Selenium WebDriver | Browser session, cookies, local storage |
| **Application State** | Application | Business logic, user data |
| **Configuration State** | Application | Driver settings, timeouts |

### State Lifecycle

1. **Creation:** Driver initialised via `WebDriverInitialiser`
2. **Usage:** Processor operations modify browser state
3. **Persistence:** Browser maintains state across operations
4. **Cleanup:** Driver disposed via `driver.Quit()`

## Stores

The library does not implement data stores. All data is managed by:

- **Selenium WebDriver:** Browser storage (cookies, localStorage, sessionStorage)
- **NuciWeb Framework:** Application data stores
- **Application Code:** Custom data stores

## Caches

The library does not implement caching. However, the following are used:

### Selenium WebDriver Cache

- **Element Cache:** Not implemented - elements are re-located each time
- **Page Cache:** Not implemented - pages are reloaded
- **Cookie Cache:** Browser manages cookies

### Application-Level Cache

Callers may implement caching:

```csharp
// Example: Cache element references
private readonly Dictionary<string, IWebElement> _elementCache = new();

public IWebElement GetElement(string selector)
{
    if (!_elementCache.TryGetValue(selector, out var element))
    {
        element = driver.FindElement(By.XPath(selector));
        _elementCache[selector] = element;
    }
    return element;
}
```

## Migrations

The library does not implement data migrations. However, consider:

### Browser Version Migration

When upgrading browser versions:

1. **Update Drivers:** Ensure geckodriver/ChromeDriver compatibility
2. **Test Compatibility:** Verify all operations work
3. **Update User Agents:** Refresh user agent strings
4. **Document Changes:** Update documentation

### Configuration Migration

When changing configuration:

1. **Update Defaults:** Modify `WebDriverInitialiser` defaults
2. **Document Changes:** Update configuration documentation
3. **Test Migration:** Verify existing configurations work
4. **Deprecate Old Settings:** Mark old settings as deprecated

## State Recovery

### Session Recovery

The library does not implement session recovery. However:

1. **Cookies:** Browser manages cookies automatically
2. **Local Storage:** Browser manages local storage
3. **Session Storage:** Browser manages session storage

### Error Recovery

When errors occur:

1. **Page Refresh:** `processor.Refresh()`
2. **Navigation:** `processor.GoToUrl(url)`
3. **Tab Management:** `processor.SwitchToTab(tabId)`
4. **Alert Handling:** `processor.AcceptAlert()` or `processor.DismissAlert()`

## State Monitoring

### State Indicators

Monitor these indicators:

- **Driver State:** Connected/disconnected
- **Browser State:** Page loaded/not loaded
- **Element State:** Present/absent, visible/hidden
- **Session State:** Active/inactive

### State Validation

Validate state before operations:

```csharp
// Validate driver state
if (driver == null || !driver.HasSession)
{
    throw new InvalidOperationException("Driver not properly initialised");
}

// Validate page state
if (!processor.IsElementVisible("//div[@id='content']"))
{
    // Page may not be loaded
    processor.Refresh();
}
```

## Related Documentation

- [architecture.md](./architecture.md) - Architecture overview
- [design-decisions.md](./design-decisions.md) - Design rationale
- [error-handling.md](./error-handling.md) - Error handling patterns
- [concurrency-and-scheduling.md](./concurrency-and-scheduling.md) - Threading model
- [invariants.md](./invariants.md) - System invariants