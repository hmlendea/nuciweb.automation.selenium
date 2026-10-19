# SeleniumWebProcessor

Base path: N/A (class library, not a web API)

## SeleniumWebProcessor Class

**Namespace:** `NuciWeb.Automation.Selenium`
**File:** `NuciWeb.Automation.Selenium/SeleniumWebProcessor.cs`
**Declaration:** `public sealed class SeleniumWebProcessor(IWebDriver driver) : WebProcessor, IWebProcessor`

### Purpose

Implements the `IWebProcessor` interface using Selenium WebDriver to perform web automation tasks. This class translates abstraction-level automation calls into Selenium WebDriver commands against a real browser instance.

### Constructor

| Parameter | Type | Description |
|-----------|------|-------------|
| `driver` | `IWebDriver` | The Selenium WebDriver instance used to interact with the web browser |

### Methods

The class implements the `IWebProcessor` interface through protected override methods. Each method corresponds to an abstraction-level automation operation.

#### Navigation

**`PerformGoToUrl(string url, int httpRetries, TimeSpan retryDelay)`**

Navigates the browser to the specified URL with retry logic.

- **Parameters:**
  - `url` (`string`): The URL to navigate to
  - `httpRetries` (`int`): Number of retry attempts
  - `retryDelay` (`TimeSpan`): Delay between retry attempts
- **Behaviour:**
  1. If the current URL already matches the requested URL, returns immediately
  2. Navigates to the URL using `driver.Navigate().GoToUrl(url)`
  3. Waits for any element to exist on the page (up to 3 attempts)
  4. Checks for Chromium error pages (`.error-code` class)
  5. If an error page is detected, navigates to `about:blank` and retries
  6. Throws `Exception` if all retry attempts are exhausted
- **Error Scenarios:** Throws `Exception($"Failed to load the requested URL after {httpRetries} attempts")` when navigation fails after all retries

**`PerformRefresh()`**

Refreshes the current page.

- **Behaviour:** Calls `driver.Navigate().Refresh()`
- **Error Scenarios:** Propagates Selenium WebDriver exceptions

#### Element Interaction

**`PerformClick(string xpath)`**

Clicks an element located by XPath.

- **Parameters:**
  - `xpath` (`string`): XPath selector for the target element
- **Behaviour:** Switches to current tab, locates element via `GetElement(xpath)`, calls `Click()`
- **Error Scenarios:** Throws `NotFoundException` if element not found within `DefaultTimeout`

**`PerformSetText(string xpath, string text)`**

Sets text in an input element located by XPath.

- **Parameters:**
  - `xpath` (`string`): XPath selector for the target element
  - `text` (`string`): Text to input
- **Behaviour:** Switches to current tab, locates element, calls `Clear()` then `SendKeys(text)`
- **Error Scenarios:** Throws `NotFoundException` if element not found within `DefaultTimeout`

**`PerformMoveToElement(string xpath)`**

Moves the mouse to an element located by XPath.

- **Parameters:**
  - `xpath` (`string`): XPath selector for the target element
- **Behaviour:** Switches to current tab, locates element, uses `Actions` class to move to element
- **Error Scenarios:** Throws `NotFoundException` if element not found within `DefaultTimeout`

**`PerformSwitchToIframe(string xpath)`**

Switches context to an iframe located by XPath.

- **Parameters:**
  - `xpath` (`string`): XPath selector for the iframe element
- **Behaviour:** Switches to current tab, locates element, calls `driver.SwitchTo().Frame(element)`
- **Error Scenarios:** Throws `NotFoundException` if element not found within `DefaultTimeout`

#### Element State Queries

**`PerformDoesElementExist(string xpath)`**

Checks whether an element exists at the given XPath.

- **Parameters:**
  - `xpath` (`string`): XPath selector to check
- **Returns:** `bool` - `true` if element exists, `false` otherwise
- **Behaviour:** Switches to current tab, attempts `FindElement`, returns `true` on success, `false` on exception

**`PerformIsElementVisible(string xpath)`**

Checks whether an element is visible at the given XPath.

- **Parameters:**
  - `xpath` (`string`): XPath selector to check
- **Returns:** `bool` - `true` if element is visible, `false` otherwise
- **Behaviour:** Switches to current tab, attempts `FindElement`, returns `element.Displayed` on success, `false` on exception

**`PerformIsCheckboxChecked(string xpath)`**

Checks whether a checkbox element is checked.

- **Parameters:**
  - `xpath` (`string`): XPath selector for the checkbox element
- **Returns:** `bool` - `true` if checked, `false` otherwise
- **Behaviour:** Gets element via `GetElement(xpath)`, returns `element.Selected.Equals(true)`

**`PerformIsSelected(string xpath)`**

Checks whether an element is selected.

- **Parameters:**
  - `xpath` (`string`): XPath selector for the element
- **Returns:** `bool` - `true` if selected, `false` otherwise
- **Behaviour:** Gets element via `GetElement(xpath)`, returns `element.Selected`

#### Text and Attribute Retrieval

**`PerformGetText(string xpath)`**

Retrieves text content from elements matching the XPath.

- **Parameters:**
  - `xpath` (`string`): XPath selector for target elements
- **Returns:** `IEnumerable<string>` - Collection of text values from matching elements
- **Behaviour:** Switches to current tab, gets all matching elements, returns their `.Text` values

**`PerformGetAttribute(string xpath, string attribute)`**

Retrieves attribute values from elements matching the XPath.

- **Parameters:**
  - `xpath` (`string`): XPath selector for target elements
  - `attribute` (`string`): Attribute name to retrieve
- **Returns:** `IEnumerable<string>` - Collection of attribute values from matching elements
- **Behaviour:** Switches to current tab, gets all matching elements, returns their attribute values

**`PerformGetSelectedText(string xpath)`**

Retrieves selected option text from select elements matching the XPath.

- **Parameters:**
  - `xpath` (`string`): XPath selector for select elements
- **Returns:** `IEnumerable<string>` - Collection of selected option text values
- **Behaviour:** Gets select elements, returns `SelectedOption.Text` for each

**`PerformGetSelectOptionsCount(string xpath)`**

Retrieves the count of options in a select element.

- **Parameters:**
  - `xpath` (`string`): XPath selector for select element
- **Returns:** `int` - Number of options in the select element
- **Behaviour:** Gets select elements, returns `First().Options.Count`

#### Script Execution

**`PerformExecuteScript(string script)`**

Executes JavaScript in the browser context.

- **Parameters:**
  - `script` (`string`): JavaScript code to execute
- **Returns:** `string` - Script result as string, or `null` if result is null
- **Behaviour:** Casts driver to `IJavaScriptExecutor`, calls `ExecuteScript(script)`, casts result to string
- **Error Scenarios:** Returns `null` if script result is null

**`PerformGetPageSource()`**

Retrieves the current page source HTML.

- **Returns:** `string` - Page source HTML
- **Behaviour:** Saves current window handle, switches to current tab, reads `driver.PageSource`, restores original window handle

#### Tab Management

**`PerformNewTab(string url)`**

Opens a new tab with the specified URL.

- **Parameters:**
  - `url` (`string`): URL to open in the new tab
- **Returns:** `string` - The new tab's window handle
- **Behaviour:**
  1. Switches to first window handle
  2. Injects JavaScript to create and click an anchor element with `target="_blank"`
  3. Compares window handles before and after to identify the new tab
  4. Returns the new tab handle
- **Known Limitation:** Not covered by the retry mechanism (see TODO comment in source)

**`PerformSwitchToTab(string tab)`**

Switches to a specific tab by window handle.

- **Parameters:**
  - `tab` (`string`): Window handle of the target tab
- **Behaviour:** Calls `driver.SwitchTo().Window(tab)`

**`PerformCloseTab(string tab)`**

Closes a specific tab by window handle.

- **Parameters:**
  - `tab` (`string`): Window handle of the tab to close
- **Behaviour:** Switches to the tab, calls `Close()`

#### Alert Handling

**`PerformAcceptAlert()`**

Accepts the current browser alert dialog.

- **Behaviour:** Gets alert via `GetAlert()`, calls `Accept()`

**`PerformDismissAlert()`**

Dismisses the current browser alert dialog.

- **Behaviour:** Gets alert via `GetAlert()`, calls `Dismiss()`

### Private Helper Methods

**`GetElement(string xpath)`**

Locates a single visible element by XPath with retry logic.

- **Parameters:** `xpath` (`string`)
- **Returns:** `IWebElement`
- **Behaviour:** Switches to current tab, polls `FindElement` until element is found and displayed within `DefaultTimeout`
- **Error Scenarios:** Throws `NotFoundException($"No element with the `XPath {xpath}` exists!")` if not found within timeout

**`GetElements(string xpath)`**

Locates all elements matching an XPath with retry logic.

- **Parameters:** `xpath` (`string`)
- **Returns:** `ReadOnlyCollection<IWebElement>`
- **Behaviour:** Switches to current tab, polls `FindElements` until elements are found within `DefaultTimeout`
- **Error Scenarios:** Throws `NotFoundException($"No elements with the `XPath {xpath}` exist!")` if not found within timeout

**`GetSelectElements(string xpath)`**

Wraps matching elements as `SelectElement` instances.

- **Parameters:** `xpath` (`string`)
- **Returns:** `IList<SelectElement>`
- **Behaviour:** Gets elements via `GetElements(xpath)`, wraps each in `SelectElement`

**`GetAlert()`**

Retrieves the current browser alert with retry logic.

- **Returns:** `IAlert` or `null`
- **Behaviour:** Switches to current tab, polls `driver.SwitchTo().Alert()` within `DefaultTimeout`
- **Error Scenarios:** Returns `null` if no alert is present within timeout

### Dependencies

| Dependency | Purpose |
|------------|---------|
| `Selenium.WebDriver` | Browser automation via `IWebDriver`, `IWebElement`, `IAlert` |
| `Selenium.Support` | `SelectElement`, `Actions`, `WebDriverWait` utilities |
| `NuciWeb.Automation` | `IWebProcessor` interface, `WebProcessor` base class, `NotFoundException` |

### Error Handling

- Element not found operations throw `NotFoundException` after `DefaultTimeout` expires
- Navigation failures throw `Exception` after exhausting `httpRetries` attempts
- Alert operations return `null` if no alert is present within timeout
- JavaScript execution returns `null` if script result is null

### Flow References

- [Browser Navigation Flow](../flows/startup-and-rendering.md)
- [Element Interaction Flow](../flows/startup-and-rendering.md)