# Startup and Rendering Flows

## Purpose

Documents the end-to-end execution flows for application startup and rendering in NuciWeb.Automation.Selenium.

## Startup Flow

### Sequence

1. **Application Initiation:** Application creates `SeleniumWebProcessor` instance
2. **Driver Initialisation:** `WebDriverInitialiser.InitialiseAvailableWebDriver()` is called
3. **Browser Selection:** Runtime check for `geckodriver` at `/usr/bin/geckodriver`
4. **Driver Creation:**
   - If Firefox driver found: `InitialiseFirefoxDriver()`
   - Otherwise: `InitialiseChromeDriver()`
5. **Processor Creation:** Application creates `SeleniumWebProcessor(driver)` instance
6. **Ready State:** Processor is ready for automation operations

### Configuration Flow

1. **Method Call:** `WebDriverInitialiser.InitialiseAvailableWebDriver(isDebugModeEnabled, pageLoadTimeout)`
2. **Parameter Evaluation:**
   - `isDebugModeEnabled`: Controls headless mode and resource loading
   - `pageLoadTimeout`: Configures WebDriver page load timeout
3. **Browser Detection:**
   - Checks for `geckodriver` at `/usr/bin/geckodriver`
   - If exists: Firefox driver creation
   - Else: Chrome driver creation
4. **Driver Configuration:**
   - `PageLoadStrategy.None`
   - Window size: `1920x1080`
   - Maximized window
   - Headless mode when debug mode is disabled
   - Reduced resource usage in headless mode

### Error Handling

- Driver creation exceptions propagate to caller
- Invalid parameters may cause argument validation exceptions
- File system access errors during driver detection

## Rendering Flow

### Page Load Process

1. **Navigation Command:** `PerformGoToUrl(url, httpRetries, retryDelay)`
2. **URL Processing:**
   - If current URL matches target: return immediately
   - Otherwise: navigate to new URL
3. **Retry Logic:**
   - Up to `httpRetries` attempts
   - Each attempt includes 3 sub-attempts for element existence check
   - Error page detection via `.error-code` class
   - On error page: navigate to `about:blank`, wait `retryDelay`, retry
4. **Success Criteria:**
   - Page loads without error page detection
   - `driver.Url` matches requested URL
   - No exceptions thrown during navigation

### Element Discovery

- **Default Timeout:** `DefaultTimeout` (configured internally)
- **Polling:** Periodic `FindElement` calls until element found or timeout
- **Visibility Check:** Many operations verify `element.Displayed` before proceeding
- **Error Response:** `NotFoundException` when element not found within timeout

### Performance Characteristics

- **Synchronous Operations:** All methods block until completion
- **Timeouts:** Configurable element discovery timeouts
- **Retry Mechanisms:** Navigation includes retry logic for error recovery
- **Resource Usage:** Headless mode reduces resource consumption

## Component References

- **WebDriverInitialiser:** Driver creation and configuration
- **SeleniumWebProcessor:** Automation operation execution
- **IWebDriver:** Browser interaction interface
- **IWebDriver.WindowHandles:** Tab management

## Related Documentation

- [api-reference/web-driver-initialiser.md](../api-reference/web-driver-initialiser.md) - Driver initialisation API
- [api-reference/selenium-web-processor.md](../api-reference/selenium-web-processor.md) - Processor API reference
- [flows/detail-mutation.md](../flows/detail-mutation.md) - Element interaction flows
- [components/host-and-composition.md](../components/host-and-composition.md) - Component composition
- [architecture.md](../architecture.md) - High-level architecture overview