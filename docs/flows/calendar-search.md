# Calendar Search Flow

## Purpose

Documents the calendar search functionality flow in NuciWeb.Automation.Selenium, describing how the library supports calendar-based search operations through the `IWebProcessor` interface.

## Calendar Search Flow

### Sequence

1. **Navigation:** Application navigates to calendar page via `GoToUrl`
2. **Element Location:** Application locates calendar elements via XPath
3. **Date Selection:** Application interacts with date picker elements
4. **Search Execution:** Application triggers search operation
5. **Result Processing:** Application processes search results

### Implementation Details

The library provides the following capabilities for calendar search:

- **Navigation:** `PerformGoToUrl` for page navigation
- **Element Interaction:** `PerformClick` for date selection
- **Text Input:** `PerformSetText` for date input fields
- **Element State:** `PerformIsElementVisible`, `PerformDoesElementExist` for validation
- **Tab Management:** `PerformSwitchToTab` if calendar opens in new tab
- **Script Execution:** `PerformExecuteScript` for complex calendar interactions

### Typical Calendar Search Pattern

```csharp
// Navigate to calendar page
processor.GoToUrl("https://example.com/calendar");

// Locate and interact with date picker
processor.Click("//input[@id='date-picker']");
processor.SetText("//input[@id='date-picker']", "2026-10-19");

// Trigger search
processor.Click("//button[@id='search-button']");

// Process results
var results = processor.GetText("//div[@class='search-results']//li");
```

### Error Handling

- **Element Not Found:** `NotFoundException` for missing calendar elements
- **Navigation Failures:** `Exception` for page load failures
- **Timeout Handling:** Configurable timeouts for element discovery
- **Retry Logic:** Navigation includes retry for error pages

## Component References

- **SeleniumWebProcessor:** Implements all calendar search operations
- **WebDriverInitialiser:** Creates browser driver instances
- **IWebProcessor:** Defines the abstraction layer for all operations

## Related Documentation

- [flows/startup-and-rendering.md](./startup-and-rendering.md) - Startup flow
- [flows/detail-mutation.md](./detail-mutation.md) - Detail mutation flow
- [behaviour/browse-and-search.md](../behaviour/browse-and-search.md) - Browse and search behaviours
- [api-reference/selenium-web-processor.md](../api-reference/selenium-web-processor.md) - Processor API reference