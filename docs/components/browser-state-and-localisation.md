# Browser State and Localisation

## Purpose

Documents how NuciWeb.Automation.Selenium manages browser state and localization, including window management, tab switching, iframe navigation, and user agent configuration.

## Browser State Management

### Window Handles

The library manages browser windows through window handles (strings) provided by Selenium WebDriver:

- **Primary Window:** The first window handle is used as the reference point
- **Tab Switching:** `PerformSwitchToTab` switches to a specific tab by handle
- **Tab Closing:** `PerformCloseTab` closes a specific tab by handle
- **New Tab Creation:** `PerformNewTab` creates a new tab and returns its handle

### Window Configuration

- **Size:** Fixed at `1920x1080` (defined as `DefaultBrowserSize`)
- **Position:** Top-left corner `(0, 0)`
- **Maximization:** Applied after driver creation
- **Consistency:** Same window size across all driver initialisers

### Tab Management Flow

```
Application
    ↓ calls
PerformSwitchToTab(tabHandle)
    ↓ calls
driver.SwitchTo().Window(tabHandle)
    ↓ returns
IWebDriver context switched to tab
```

### Iframe Navigation

- **Method:** `PerformSwitchToIframe(string xpath)`
- **Behaviour:** Switches driver context to iframe element
- **Scope:** All subsequent element operations occur within iframe context
- **Exit:** To return to main document, switch to default content or parent iframe

## Localisation Configuration

### User Agent Management

**Firefox:**
- No custom user agent configuration
- Uses Firefox default user agent

**Chrome:**
- User agent fetched from `NuciWeb.HTTP` via `IUserAgentFetcher`
- Applied via `--user-agent` argument
- Purpose: Avoid detection by websites that block headless browsers

### Language Preferences

**Firefox:**
- No explicit language preferences configured
- Uses browser defaults

**Chrome:**
- No explicit language preferences configured
- Uses browser defaults

### Regional Settings

The library does not explicitly configure regional settings. Regional behavior depends on:
- Browser defaults
- Operating system locale settings
- Target website localization

## State Persistence

### Driver Lifecycle

- **Creation:** `WebDriverInitialiser` creates driver with configured settings
- **Usage:** `SeleniumWebProcessor` uses driver for automation operations
- **Destruction:** Application code is responsible for driver cleanup (`driver.Dispose()`)
- **State:** All state is maintained by the browser instance

### Cookie Management

The library does not provide explicit cookie management methods. Cookie operations must be performed through:
- JavaScript execution (`PerformExecuteScript`)
- Direct WebDriver access (not exposed through `IWebProcessor`)

### Local Storage

The library does not provide explicit local storage management. Local storage operations must be performed through:
- JavaScript execution (`PerformExecuteScript`)
- Direct WebDriver access (not exposed through `IWebProcessor`)

## Headless Mode Configuration

### Firefox Headless

- **Environment Variables:** `MOZ_HEADLESS=1`, `MOZ_DISABLE_CONTENT_SANDBOX=1`, `MOZ_DISABLE_GFX_SANDBOX=1`
- **Arguments:** `--headless`
- **Preferences:** `permissions.default.image = 2` (disable images)
- **Additional:** `media.navigator.enabled = false`, `media.navigator.video.enabled = false`

### Chrome Headless

- **Arguments:** `--headless`, `--disable-gpu`, `--blink-settings=imagesEnabled=false`
- **Preferences:** `profile.default_content_setting_values.images = 2` (disable images)
- **User Agent:** Modified to avoid detection

## Related Documentation

- [api-reference/web-driver-initialiser.md](../api-reference/web-driver-initialiser.md) - Driver initialisation API
- [api-reference/selenium-web-processor.md](../api-reference/selenium-web-processor.md) - Processor API reference
- [components/host-and-composition.md](../components/host-and-composition.md) - Host and composition
- [architecture.md](../architecture.md) - High-level architecture overview