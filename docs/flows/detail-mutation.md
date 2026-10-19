# Detail Mutation Flow

## Purpose

Documents the detail mutation flow in NuciWeb.Automation.Selenium, describing how the library supports editing and modifying web page content through the `IWebProcessor` interface.

## Detail Mutation Flow

### Sequence

1. **Navigation:** Application navigates to target page via `GoToUrl`
2. **Element Location:** Application locates editable elements via XPath
3. **State Verification:** Application verifies element state before modification
4. **Content Modification:** Application modifies element content via `SetText` or `Click`
5. **Submission:** Application submits changes via `Click` on submit button
6. **Result Verification:** Application verifies modification was successful

### Implementation Details

The library provides the following capabilities for detail mutation:

- **Navigation:** `PerformGoToUrl` for page navigation
- **Element Interaction:** `PerformClick` for button clicks and form submission
- **Text Input:** `PerformSetText` for modifying text fields
- **Element State:** `PerformIsElementVisible`, `PerformDoesElementExist` for validation
- **Tab Management:** `PerformSwitchToTab` if editing occurs in new tab
- **Iframe Navigation:** `PerformSwitchToIframe` if editable content is in iframe
- **Alert Handling:** `PerformAcceptAlert`, `PerformDismissAlert` for confirmation dialogs

### Typical Detail Mutation Pattern

```csharp
// Navigate to edit page
processor.GoToUrl("https://example.com/edit");

// Locate and modify text field
processor.SetText("//input[@id='name']", "New Name");

// Submit changes
processor.Click("//button[@id='submit']");

// Verify changes
var result = processor.GetText("//div[@class='confirmation-message']");
```

### Error Handling

- **Element Not Found:** `NotFoundException` for missing editable elements
- **Navigation Failures:** `Exception` for page load failures
- **Timeout Handling:** Configurable timeouts for element discovery
- **Retry Logic:** Navigation includes retry for error pages
- **Alert Handling:** Returns `null` if no alert present within timeout

## Component References

- **SeleniumWebProcessor:** Implements all detail mutation operations
- **WebDriverInitialiser:** Creates browser driver instances
- **IWebProcessor:** Defines the abstraction layer for all operations

## Related Documentation

- [flows/startup-and-rendering.md](./startup-and-rendering.md) - Startup flow
- [flows/calendar-search.md](./calendar-search.md) - Calendar search flow
- [behaviour/inspect-edit-delete.md](../behaviour/inspect-edit-delete.md) - Inspect, edit, delete behaviours
- [api-reference/selenium-web-processor.md](../api-reference/selenium-web-processor.md) - Processor API reference