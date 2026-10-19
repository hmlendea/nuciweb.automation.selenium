# WebDriverInitialiser

Base path: N/A (class library, not a web API)

## WebDriverInitialiser Class

**Namespace:** `NuciWeb.Automation.Selenium`
**File:** `NuciWeb.Automation.Selenium/WebDriverInitialiser.cs`
**Declaration:** `public sealed class WebDriverInitialiser`

### Purpose

Provides methods to initialise Selenium WebDriver instances based on the available browser drivers on the system. This class abstracts the complexity of browser driver setup and configuration, supporting both Firefox and Chrome browsers with appropriate defaults for debug and production scenarios.

### Static Properties

**`DefaultBrowserSize`**

- **Type:** `Size`
- **Value:** `new Size(1920, 1080)`
- **Purpose:** Default browser window size used across all driver initialisers

### Public Methods

#### `InitialiseAvailableWebDriver(bool isDebugModeEnabled = true, int pageLoadTimeout = 90)`

Initialises a web driver based on the available browser drivers on the system.

- **Parameters:**
  - `isDebugModeEnabled` (`bool`): Indicates whether the driver should run in debug mode. If false, enables headless mode and disables image loading.
  - `pageLoadTimeout` (`int`): Maximum time to wait for a page to load, in seconds.
- **Returns:** `IWebDriver`
- **Behaviour:**
  1. Checks for `geckodriver` at `/usr/bin/geckodriver`
  2. If found, calls `InitialiseFirefoxDriver(isDebugModeEnabled, pageLoadTimeout)`
  3. Otherwise, calls `InitialiseChromeDriver(isDebugModeEnabled, pageLoadTimeout)`
- **Configuration:**
  - `PageLoadStrategy.None`
  - Configurable page load timeout
  - Maximized browser window
  - Quieter driver logging
  - Headless mode when debug mode is disabled

#### `InitialiseFirefoxDriver(bool isDebugModeEnabled = true, int pageLoadTimeout = 90)`

Initialises a Firefox web driver with specified settings.

- **Parameters:**
  - `isDebugModeEnabled` (`bool`): Controls headless mode and image loading
  - `pageLoadTimeout` (`int`): Page load timeout in seconds
- **Returns:** `IWebDriver`
- **Configuration:**
  - `PageLoadStrategy.None`
  - Window size: `1920x1080`
  - Privacy preferences: `privacy.firstparty.isolate = false`
  - Headless mode when debug mode is disabled
  - Image loading disabled when debug mode is disabled
  - User agent and sandbox restrictions when debug mode is disabled
  - Error-level logging only
  - Maximized window

#### `InitialiseChromeDriver(bool isDebugModeEnabled = true, int pageLoadTimeout = 90)`

Initialises a Chrome web driver with specified settings.

- **Parameters:**
  - `isDebugModeEnabled` (`bool`): Controls headless mode and image loading
  - `pageLoadTimeout` (`int`): Page load timeout in seconds
- **Returns:** `IWebDriver`
- **Configuration:**
  - `PageLoadStrategy.None`
  - Window size: `1920x1080`
  - User agent fetched from external service
  - Headless mode when debug mode is disabled
  - GPU disabled when debug mode is disabled
  - Image loading disabled when debug mode is disabled
  - Sandbox and translate disabled
  - Logging disabled
  - Maximized window

### Private Methods

#### `SetWindowSize(IWebDriver driver)`

Configures the browser window size and position.

- **Parameters:**
  - `driver` (`IWebDriver`): WebDriver instance to configure
- **Behaviour:**
  - Sets window position to `(0, 0)`
  - Sets window size to `DefaultBrowserSize`

### Dependencies

| Dependency | Purpose |
|------------|---------|
| `Selenium.WebDriver` | Browser automation via `IWebDriver`, `FirefoxDriver`, `ChromeDriver` |
| `Selenium.Support` | Driver service utilities (`FirefoxDriverService`, `ChromeDriverService`) |
| `NuciWeb.HTTP` | User agent fetching via `IUserAgentFetcher` and `UserAgentFetcher` |
| `System.Drawing` | `Size` type for window dimensions |
| `System.IO` | File system operations for driver detection |

### Error Handling

- No explicit error handling in driver initialisation methods
- Exceptions from Selenium driver creation propagate to callers
- File system operations (`File.Exists`) may throw `IOException`

### Flow References

- [Browser Navigation Flow](../flows/startup-and-rendering.md)
- [Driver Setup Flow](../flows/startup-and-rendering.md)

### Configuration Notes

#### Browser Selection

- **Primary:** Firefox (if `geckodriver` exists at `/usr/bin/geckodriver`)
- **Fallback:** Chrome (when Firefox driver is unavailable)
- **No Override:** Browser selection is automatic based on driver availability

#### Debug Mode Configuration

When `isDebugModeEnabled = false`:

- **Firefox:** Enables headless mode, disables content and graphics sandboxes, disables image loading
- **Chrome:** Enables headless mode, disables GPU, disables image loading via blink settings

#### Page Load Timeout

- **Default:** 90 seconds
- **Configurable:** Via method parameter
- **Strategy:** `PageLoadStrategy.None` (no automatic waiting)

#### Window Management

- **Size:** Fixed at `1920x1080`
- **Position:** Top-left corner `(0, 0)`
- **Maximization:** Applied after driver creation

### Security Considerations

- **Headless Mode:** Reduces attack surface in production environments
- **User Agent:** Fetched from external service (NuciWeb.HTTP)
- **Sandbox Restrictions:** Enabled when debug mode is disabled
- **Logging:** Limited to error level to reduce information disclosure

### Performance Considerations

- **Resource Usage:** Headless mode reduces resource consumption
- **Image Loading:** Disabled in headless mode for faster page loads
- **Window Size:** Fixed size for predictable resource allocation
- **Driver Services:** Suppress initial diagnostic information for cleaner startup