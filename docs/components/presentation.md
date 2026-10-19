# Presentation Layer Components

## Purpose

Documents the presentation layer components of NuciWeb.Automation.Selenium, including how the library presents automation capabilities to application code through the `IWebProcessor` interface.

## Components

### SeleniumWebProcessor

**File:** `NuciWeb.Automation.Selenium/SeleniumWebProcessor.cs`
**Namespace:** `NuciWeb.Automation.Selenium`
**Declaration:** `public sealed class SeleniumWebProcessor(IWebDriver driver) : WebProcessor, IWebProcessor`

**Responsibility:** Implements the `IWebProcessor` interface using Selenium WebDriver to perform web automation tasks.

**Presentation Contract:** Presents all automation operations through the `IWebProcessor` interface, including navigation, element interaction, state queries, text/attribute retrieval, script execution, tab management, and alert handling.

**Dependencies:**
- `Selenium.WebDriver` - Browser automation via `IWebDriver`, `IWebElement`, `IAlert`
- `Selenium.Support` - `SelectElement`, `Actions`, `WebDriverWait` utilities
- `NuciWeb.Automation` - `IWebProcessor` interface, `WebProcessor` base class, `NotFoundException`

**Lifetime:** Transient - created per processor instance

### WebDriverInitialiser

**File:** `NuciWeb.Automation.Selenium/WebDriverInitialiser.cs`
**Namespace:** `NuciWeb.Automation.Selenium`
**Declaration:** `public sealed class WebDriverInitialiser`

**Responsibility:** Creates configured Firefox or Chrome WebDriver instances based on available browser drivers.

**Presentation Contract:** Presents static factory methods for driver creation, abstracting browser-specific configuration details from application code.

**Dependencies:**
- `Selenium.WebDriver` - Browser automation via `IWebDriver`, `FirefoxDriver`, `ChromeDriver`
- `Selenium.Support` - Driver service utilities
- `NuciWeb.HTTP` - User agent fetching
- `System.Drawing` - `Size` type for window dimensions
- `System.IO` - File system operations for driver detection

**Lifetime:** Static utility - no instantiation needed

## Component Relationships

```
Application Code
    ↓ uses
IWebProcessor Interface (NuciWeb.Automation)
    ↓ implemented by
SeleniumWebProcessor
    ↓ uses
IWebDriver (Selenium WebDriver)
    ↓ controls
Firefox/Chrome Browser
```

## Interface Boundaries

### Inbound Boundary

Application code interacts with the library exclusively through the `IWebProcessor` interface. Selenium types are not exposed to application code.

### Outbound Boundary

The library interacts with Selenium WebDriver through `IWebDriver`, `IWebElement`, `IAlert`, and related types. Browser drivers (`geckodriver`, ChromeDriver) are external executables.

## Component Ownership

| Component | Owner | Lifetime | Concurrency Model |
|-----------|-------|----------|-------------------|
| `SeleniumWebProcessor` | Application code | Transient | Single-threaded per instance |
| `WebDriverInitialiser` | Library | Static | Thread-safe static methods |
| `IWebDriver` | SeleniumWebProcessor | Per instance | Single-threaded per driver |

## Related Documentation

- [api-reference/selenium-web-processor.md](../api-reference/selenium-web-processor.md) - SeleniumWebProcessor API reference
- [api-reference/web-driver-initialiser.md](../api-reference/web-driver-initialiser.md) - WebDriverInitialiser API reference
- [flows/startup-and-rendering.md](../flows/startup-and-rendering.md) - Startup and rendering flows
- [architecture.md](../architecture.md) - High-level architecture overview