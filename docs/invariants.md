# Invariants

## Purpose

Documents system-wide invariants and contracts for the NuciWeb.Automation.Selenium library.

## Invariants

### WebDriverInitialiser

1. **Browser Selection:** When both geckodriver and ChromeDriver are available, Firefox is preferred.
2. **Debug Mode:** When `isDebugModeEnabled = true`, headless mode is disabled.
3. **Window Size:** Default browser window size is 1920x1080.
4. **Page Load Timeout:** Configurable via `pageLoadTimeout` parameter; default is 90 seconds.
5. **Driver Path:** Firefox driver expected at `/usr/bin/geckodriver`.

### SeleniumWebProcessor

1. **Thread Safety:** Not thread-safe. Each instance should be used by a single thread.
2. **WebDriver Ownership:** The processor does not own the WebDriver instance; caller is responsible for disposal.
3. **XPath Selectors:** All element location uses XPath expressions.
4. **Return Types:** Methods returning multiple values return `IEnumerable<string>`.
5. **Exception Propagation:** Selenium exceptions are propagated directly to callers.

### General

1. **No Persistence:** The library does not persist any data to disk or database.
2. **No Session Management:** Session state is managed by the browser, not the library.
3. **No Caching:** Element references are not cached between operations.
4. **Synchronous Operations:** All operations are synchronous; no async/await patterns.
5. **No Implicit Waits:** No implicit waits are configured; elements must be found immediately.

## Contracts

### WebDriverInitialiser Contracts

**Preconditions:**
- `pageLoadTimeout` must be positive (> 0)
- Browser driver must be available on system

**Postconditions:**
- Returns a valid `IWebDriver` instance
- Driver is configured with specified timeout
- Driver is configured with specified debug mode

**Invariants:**
- Window size is always 1920x1080
- User agent is fetched via NuciWeb.HTTP service

### SeleniumWebProcessor Contracts

**Preconditions:**
- `driver` parameter must not be null
- Driver must be connected to a browser

**Postconditions:**
- Operations affect the wrapped WebDriver instance
- Exceptions from WebDriver are propagated

**Invariants:**
- Processor wraps exactly one WebDriver instance
- Processor does not dispose the WebDriver

## Related Documentation

- [architecture.md](./architecture.md) - Architecture overview
- [design-decisions.md](./design-decisions.md) - Design rationale
- [error-handling.md](./error-handling.md) - Error handling patterns
- [concurrency-and-scheduling.md](./concurrency-and-scheduling.md) - Threading model