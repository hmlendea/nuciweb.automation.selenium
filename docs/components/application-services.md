# Application Services Layer

## Purpose

Documents the application services layer of NuciWeb.Automation.Selenium, describing how the library provides automation services to application code through the `IWebProcessor` interface.

## Services Provided

### Navigation Service

**Method:** `PerformGoToUrl(string url, int httpRetries, TimeSpan retryDelay)`

**Responsibility:** Navigate the browser to a specified URL with retry logic for error recovery.

**Contract:**
- Input: URL string, retry count, retry delay
- Output: Success/failure result
- Side effects: Browser navigation, page state changes
- Idempotency: Not idempotent - navigates even if already on target URL (early return if same URL)

**Error Handling:**
- Throws `Exception` after exhausting all retry attempts
- Retries on Chromium error pages by navigating to `about:blank`

### Element Interaction Service

**Methods:** `PerformClick`, `PerformSetText`, `PerformMoveToElement`, `PerformSwitchToIframe`

**Responsibility:** Perform interactions with web elements located by XPath.

**Contract:**
- Input: XPath selector, interaction parameters
- Output: Operation result
- Side effects: Element state changes
- Idempotency: Varies by operation type

**Error Handling:**
- Throws `NotFoundException` if element not found within timeout
- Retry logic for element discovery

### State Query Service

**Methods:** `PerformDoesElementExist`, `PerformIsElementVisible`, `PerformIsCheckboxChecked`, `PerformIsSelected`

**Responsibility:** Query element state without modifying it.

**Contract:**
- Input: XPath selector
- Output: Boolean state values
- Side effects: None (read-only)
- Idempotency: Always idempotent

### Text and Attribute Retrieval Service

**Methods:** `PerformGetText`, `PerformGetAttribute`, `PerformGetSelectedText`, `PerformGetSelectOptionsCount`

**Responsibility:** Retrieve text content and attribute values from elements.

**Contract:**
- Input: XPath selector, attribute name (for `PerformGetAttribute`)
- Output: Text or count values
- Side effects: None (read-only)
- Idempotency: Always idempotent

### Script Execution Service

**Method:** `PerformExecuteScript(string script)`

**Responsibility:** Execute JavaScript in the browser context.

**Contract:**
- Input: JavaScript code string
- Output: Script result as string, or `null` if result is null
- Side effects: Modifies page state based on script
- Idempotency: Varies by script

### Tab Management Service

**Methods:** `PerformNewTab`, `PerformSwitchToTab`, `PerformCloseTab`

**Responsibility:** Manage browser tabs and windows.

**Contract:**
- Input: Tab handle or URL (for new tab)
- Output: Tab handle (for new tab), void (for switch/close)
- Side effects: Tab switching, window management
- Idempotency: Varies by operation

### Alert Handling Service

**Methods:** `PerformAcceptAlert`, `PerformDismissAlert`

**Responsibility:** Handle browser alert dialogs.

**Contract:**
- Input: None
- Output: Void
- Side effects: Accept or dismiss alert dialog
- Idempotency: Not idempotent - only one alert at a time

### Page Source Service

**Method:** `PerformGetPageSource()`

**Responsibility:** Retrieve the current page source HTML.

**Contract:**
- Input: None
- Output: Page source string
- Side effects: None (read-only)
- Idempotency: Always idempotent

## Service Dependencies

| Service | Dependencies |
|---------|-------------|
| Navigation | `IWebDriver`, `WebDriverWait`, `Select` |
| Element Interaction | `IWebElement`, `Actions`, `SelectElement` |
| State Query | `IWebElement`, `IWebDriver` |
| Text Retrieval | `IWebElement`, `ReadOnlyCollection<IWebElement>` |
| Script Execution | `IJavaScriptExecutor` |
| Tab Management | `IWebDriver`, `IWindowHandles` |
| Alert Handling | `IAlert`, `IWebDriver` |
| Page Source | `IWebDriver` |

## Service Ordering

Services should be used in this order for typical automation workflows:

1. **Navigation** - Navigate to target page
2. **Element Location** - Locate elements via XPath
3. **State Queries** - Verify element state before interaction
4. **Element Interaction** - Perform actions on elements
5. **Tab Management** - Switch between tabs if needed
6. **Alert Handling** - Handle alerts if present
7. **Page Source** - Retrieve final page state

## Related Documentation

- [api-reference/selenium-web-processor.md](../api-reference/selenium-web-processor.md) - Processor API reference
- [flows/startup-and-rendering.md](../flows/startup-and-rendering.md) - Startup flow
- [components/integration-models.md](../components/integration-models.md) - Integration patterns
- [error-handling.md](../error-handling.md) - Error taxonomy