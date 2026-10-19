# Host and Composition Architecture

## Purpose

Documents the host and composition architecture of NuciWeb.Automation.Selenium, describing how the library integrates with application code and manages component composition.

## Composition Model

### Application Integration

The library integrates with application code through a clean abstraction boundary:

```
Application Code
    ↓ uses interface
IWebProcessor (NuciWeb.Automation)
    ↓ implemented by
SeleniumWebProcessor (NuciWeb.Automation.Selenium)
    ↓ uses
IWebDriver (Selenium.WebDriver)
    ↓ controls
Browser (Firefox/Chrome)
```

### Component Composition

The library provides two main entry points for application composition:

1. **Driver Initialisation:** `WebDriverInitialiser.InitialiseAvailableWebDriver()` or dedicated initialisers
2. **Processor Creation:** `new SeleniumWebProcessor(driver)` after driver initialisation

### Configuration Flow

1. Application calls `WebDriverInitialiser.InitialiseAvailableWebDriver(isDebugModeEnabled, pageLoadTimeout)`
2. Initialiser detects browser driver availability (`geckodriver` at `/usr/bin/geckodriver`)
3. Returns configured Firefox or Chrome `IWebDriver` instance
4. Application creates `SeleniumWebProcessor(driver)` with the driver
5. Application uses `IWebProcessor` methods for automation operations

### Dependency Injection Considerations

The library is designed for manual composition rather than automatic DI container integration:

- **SeleniumWebProcessor** requires explicit `IWebDriver` constructor parameter
- **WebDriverInitialiser** provides static factory methods
- No built-in DI registration; applications must integrate with their preferred DI container

### Browser Selection Logic

```
if File.Exists("/usr/bin/geckodriver")
    → InitialiseFirefoxDriver(isDebugModeEnabled, pageLoadTimeout)
else
    → InitialiseChromeDriver(isDebugModeEnabled, pageLoadTimeout)
```

This logic is encapsulated within `WebDriverInitialiser.InitialiseAvailableWebDriver()`.

## Composition Best Practices

### Single Responsibility

- Application code should depend only on `IWebProcessor`, not on Selenium types directly
- Driver initialisation and processor creation should be separated concerns
- Browser selection should be configured at application startup, not scattered throughout code

### Error Handling at Composition Boundary

- Validate driver initialisation result before creating processor
- Handle cases where neither Firefox nor Chrome drivers are available
- Provide clear error messages for missing dependencies

### Testing Composition

- Mock `IWebDriver` for unit tests of processor logic
- Use actual browser instances for integration tests
- Verify browser selection logic works correctly in different environments

## Related Documentation

- [api-reference/selenium-web-processor.md](../api-reference/selenium-web-processor.md) - Processor API reference
- [api-reference/web-driver-initialiser.md](../api-reference/web-driver-initialiser.md) - Initialiser API reference
- [architecture.md](../architecture.md) - High-level architecture overview
- [components/integration-models.md](../components/integration-models.md) - Integration patterns