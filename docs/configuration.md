# Configuration

## Purpose

Documents the configuration schema, sources, and precedence for the NuciWeb.Automation.Selenium library.

## Configuration Sources

The library supports configuration through method parameters and system-level settings.

### Method Parameters

Configuration is primarily provided through method parameters at call time.

#### WebDriverInitialiser Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `isDebugModeEnabled` | `bool` | `false` | When `true`, disables headless mode for interactive debugging |
| `pageLoadTimeout` | `int` | `90` | Maximum seconds to wait for page to load |

#### SeleniumWebProcessor Constructor

| Parameter | Type | Description |
|-----------|------|-------------|
| `driver` | `IWebDriver` | Selenium WebDriver instance to wrap |

## Configuration Schema

### WebDriverInitialiser Configuration

```csharp
// Debug mode disabled (headless)
IWebDriver driver = WebDriverInitialiser.InitialiseAvailableWebDriver(
    isDebugModeEnabled: false,
    pageLoadTimeout: 90);

// Debug mode enabled (visible browser)
IWebDriver driver = WebDriverInitialiser.InitialiseAvailableWebDriver(
    isDebugModeEnabled: true,
    pageLoadTimeout: 90);

// Custom timeout
IWebDriver driver = WebDriverInitialiser.InitialiseAvailableWebDriver(
    isDebugModeEnabled: false,
    pageLoadTimeout: 120);
```

### Browser Selection

Browser selection is automatic based on available drivers:

1. **Firefox:** Checked at `/usr/bin/geckodriver`
2. **Chrome:** Fallback if Firefox driver not found

```csharp
// Explicit Firefox
IWebDriver driver = WebDriverInitialiser.InitialiseFirefoxDriver(
    isDebugModeEnabled: false,
    pageLoadTimeout: 90);

// Explicit Chrome
IWebDriver driver = WebDriverInitialiser.InitialiseChromeDriver(
    isDebugModeEnabled: false,
    pageLoadTimeout: 90);
```

## Configuration Precedence

1. **Explicit method parameters** (highest priority)
2. **Default values** (lowest priority)

There are no external configuration files or environment variables used by this library.

## Environment Variables

The library does not read environment variables directly. However, Selenium WebDriver may use the following:

| Variable | Purpose |
|----------|---------|
| `PATH` | Used to locate browser driver executables |
| `SE_NODE_*` | Selenium Grid configuration (if used) |

## Browser Driver Configuration

### Firefox (geckodriver)

- **Executable:** `geckodriver`
- **Default Location:** `/usr/bin/geckodriver`
- **Headless Mode:** Disabled when `isDebugModeEnabled = true`
- **Window Size:** 1920x1080

### Chrome (ChromeDriver)

- **Executable:** `chromedriver`
- **Headless Mode:** Disabled when `isDebugModeEnabled = true`
- **Window Size:** 1920x1080
- **User Agent:** Fetched via NuciWeb.HTTP service

## Timeout Configuration

| Timeout | Default | Description |
|---------|---------|-------------|
| Page Load | 90 seconds | Maximum time to wait for page load |
| Implicit Wait | Not set | No implicit waits configured |
| Script Timeout | Not set | No script timeout configured |

## Logging Configuration

The library does not configure logging directly. Logging is handled by:

- **NuciWeb framework** - Core logging infrastructure
- **Selenium WebDriver** - Browser-level logging

## Related Documentation

- [architecture.md](./architecture.md) - Architecture overview
- [api-reference/web-driver-initialiser.md](./api-reference/web-driver-initialiser.md) - Initialiser API
- [quick-start.md](./quick-start.md) - Getting started guide
- [logging.md](./logging.md) - Logging configuration
- [troubleshooting.md](./troubleshooting.md) - Configuration troubleshooting