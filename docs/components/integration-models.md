# Integration Models

## Purpose

Documents the integration models and patterns used by NuciWeb.Automation.Selenium for interacting with external systems, including Selenium WebDriver, browser drivers, and the NuciWeb.Automation abstraction layer.

## Integration Boundaries

### NuciWeb.Automation Abstraction Layer

**Direction:** Inbound (Application → Library)

**Contract:** `IWebProcessor` interface

**Implementation:** `SeleniumWebProcessor` class

**Integration Pattern:** Interface implementation with base class inheritance

**Details:**
- Application code depends only on `IWebProcessor` interface
- Library implements interface through `SeleniumWebProcessor`
- Base class `WebProcessor` provides common functionality
- No Selenium types exposed to application code

**Failure Semantics:**
- Selenium exceptions translated to abstraction-level failures
- `NotFoundException` for element not found scenarios
- Generic `Exception` for navigation failures

### Selenium WebDriver Integration

**Direction:** Outbound (Library → Selenium)

**Contract:** WebDriver protocol via HTTP

**Implementation:** Direct usage of `IWebDriver`, `IWebElement`, `IAlert`, etc.

**Integration Pattern:** Direct API consumption with wrapper methods

**Details:**
- `SeleniumWebProcessor` wraps Selenium WebDriver calls
- Element location uses `By.XPath` exclusively
- Retry logic implemented for element discovery
- Tab management uses `IWebDriver.WindowHandles`

**Failure Semantics:**
- Retry, fallback, or termination based on command type
- Element not found: retry with timeout, then `NotFoundException`
- Navigation: retry with error page detection, then `Exception`
- Alert: retry with timeout, then return `null`

### Browser Driver Integration

**Direction:** Outbound (Library → External Executables)

**Contract:** WebDriver protocol via HTTP

**Implementation:** `FirefoxDriverService`, `ChromeDriverService`

**Integration Pattern:** Process management via Selenium driver services

**Details:**
- Firefox: Requires `geckodriver` at `/usr/bin/geckodriver`
- Chrome: Uses default Selenium Chrome driver resolution
- Driver services configured with suppressed diagnostics
- Log level set to Error for quieter operation

**Failure Semantics:**
- Driver not found: Selenium throws exception
- Driver version mismatch: Runtime failures
- Process startup failures: Propagated as exceptions

### NuciWeb.HTTP Integration

**Direction:** Outbound (Library → NuciWeb.HTTP)

**Contract:** `IUserAgentFetcher` interface

**Implementation:** `UserAgentFetcher` class

**Integration Pattern:** Dependency injection via direct instantiation

**Details:**
- Used only in `InitialiseChromeDriver` for user agent fetching
- Fetches user agent to avoid headless browser detection
- Async operation called synchronously via `.Result`

**Failure Semantics:**
- Network failures: Propagated as exceptions
- Service unavailable: Propagated as exceptions
- Timeout: Propagated as exceptions

## Integration Patterns

### Factory Pattern

**WebDriverInitialiser** acts as a factory for browser driver instances:

```csharp
IWebDriver driver = WebDriverInitialiser.InitialiseAvailableWebDriver(
    isDebugModeEnabled: false,
    pageLoadTimeout: 90);
```

### Adapter Pattern

**SeleniumWebProcessor** adapts Selenium WebDriver to `IWebProcessor`:

```csharp
IWebProcessor processor = new SeleniumWebProcessor(driver);
```

### Strategy Pattern

**Browser Selection Strategy** in `WebDriverInitialiser`:

```csharp
if (File.Exists("/usr/bin/geckodriver"))
    return InitialiseFirefoxDriver(...);
else
    return InitialiseChromeDriver(...);
```

## Data Flow Across Boundaries

### Application → Library

```
Application Code
    ↓ IWebProcessor method call
SeleniumWebProcessor
    ↓ Selenium WebDriver API call
IWebDriver
    ↓ HTTP/WebDriver protocol
Browser Driver (geckodriver/ChromeDriver)
    ↓ Browser automation protocol
Browser (Firefox/Chrome)
```

### Library → External Services

```
WebDriverInitialiser
    ↓ IUserAgentFetcher.GetUserAgent()
NuciWeb.HTTP
    ↓ HTTP request
External User Agent Service
    ↓ HTTP response
User Agent String
    ↓ ChromeOptions.AddArgument
ChromeDriver
```

## Configuration Across Boundaries

### Browser Selection

- **Source:** Runtime file system check (`/usr/bin/geckodriver`)
- **Override:** None - automatic detection only
- **Scope:** Per driver initialisation call

### Debug Mode

- **Source:** Method parameter (`isDebugModeEnabled`)
- **Override:** Explicit parameter control
- **Scope:** Per driver initialisation call

### Page Load Timeout

- **Source:** Method parameter (`pageLoadTimeout`)
- **Override:** Explicit parameter control
- **Scope:** Per driver initialisation call

### Window Size

- **Source:** Constant (`DefaultBrowserSize = 1920x1080`)
- **Override:** None - fixed constant
- **Scope:** All driver initialisations

## Error Propagation

### Selenium Exceptions

| Selenium Exception | Library Translation | Application Visibility |
|-------------------|---------------------|------------------------|
| `NoSuchElementException` | `NotFoundException` | Through `IWebProcessor` |
| `TimeoutException` | `NotFoundException` | Through `IWebProcessor` |
| `WebDriverException` | `Exception` | Through `IWebProcessor` |
| `NoAlertPresentException` | `null` return | Through `IWebProcessor` |

### Driver Service Exceptions

| Exception | Library Handling | Application Visibility |
|-----------|------------------|------------------------|
| `FileNotFoundException` (driver) | Propagated | Direct to application |
| `InvalidOperationException` (version) | Propagated | Direct to application |
| `WebDriverException` (startup) | Propagated | Direct to application |

## Testing Integration Points

### Mocking Strategy

- **IWebDriver:** Mock for unit tests of `SeleniumWebProcessor`
- **IWebElement:** Mock for element interaction tests
- **IAlert:** Mock for alert handling tests
- **IUserAgentFetcher:** Mock for Chrome driver tests

### Integration Test Requirements

- Actual browser instances (Firefox/Chrome)
- Matching browser drivers (`geckodriver`/ChromeDriver)
- Network access for user agent fetching (Chrome)
- Test URLs for navigation and element interaction

## Related Documentation

- [api-reference/selenium-web-processor.md](../api-reference/selenium-web-processor.md) - Processor API
- [api-reference/web-driver-initialiser.md](../api-reference/web-driver-initialiser.md) - Initialiser API
- [components/host-and-composition.md](../components/host-and-composition.md) - Host and composition
- [architecture.md](../architecture.md) - High-level architecture
- [dependencies.md](../dependencies.md) - External dependencies