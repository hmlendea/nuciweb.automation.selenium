# Inspect, Edit, and Delete Behaviour

## Purpose

Defines the user-facing behaviours for inspecting, editing, and deleting content within the NuciWeb.Automation.Selenium library.

## Scope

This documentation covers the core inspection, editing, and deletion behaviours that are implemented through the `IWebProcessor` interface and executed by the `SeleniumWebProcessor` class.

## Key Flows

### Inspect Flow

1. **Element Location:** Application calls `FindElement` or related methods to locate elements
2. **Element Inspection:** Application inspects element properties, attributes, and state
3. **Result Return:** Returns inspection data to application through abstraction layer

### Edit Flow

1. **Element Location:** Application calls `FindElement` or related methods to locate elements
2. **Element Inspection:** Application inspects element properties and current state
3. **Text Input:** Application calls `SetText` to modify element content
4. **Element State Update:** Application performs actions that modify element state
4. **Result Return:** Returns operation result to application through abstraction layer

### Delete Flow

1. **Element Location:** Application calls `FindElement` or related methods to locate elements
2. **Element Verification:** Application verifies element existence and state
3. **Deletion Action:** Application calls `Click` or other appropriate method to delete element
4. **Result Return:** Returns operation result to application through abstraction layer

## Implementation Details

### Element Location

- **Primary Selector:** XPath expressions are used throughout the processor API
- **Retry Logic:** Element location includes retry mechanism with timeouts
- **Visibility Check:** Many operations verify element visibility before interaction
- **Tab Management:** Operations typically switch to current tab before element location

### Error Handling

- **Element Not Found:** Throws `NotFoundException` after `DefaultTimeout` expires
- **Navigation Failures:** Throws `Exception` after exhausting retry attempts
- **Alert Handling:** Returns `null` if no alert is present within timeout
- **Script Execution:** Returns `null` if JavaScript result is null

### Performance Characteristics

- **Synchronous Operations:** All methods are synchronous
- **Blocking Calls:** WebDriver operations block until completion or timeout
- **Retry Mechanisms:** Navigation includes retry logic for error recovery
- **Timeouts:** All operations have configurable timeouts for element discovery

## Component References

- **SeleniumWebProcessor:** Implements all inspect, edit, and delete behaviours
- **WebDriverInitialiser:** Creates browser driver instances used by SeleniumWebProcessor
- **IWebProcessor:** Defines the abstraction layer for all inspect, edit, and delete operations

## Testing Considerations

- **Mocking:** WebDriver and browser interactions should be mocked for unit testing
- **Integration Tests:** Should use actual browser instances for end-to-end validation
- **Test Data:** XPath selectors should be parameterized to avoid hard-coded values
- **State Management:** Tests should verify proper cleanup of browser state between tests

## Related Documentation

- [Flows/startup-and-rendering.md](../flows/startup-and-rendering.md) - Application startup sequence
- [Flows/detail-mutation.md](../flows/detail-mutation.md) - Element interaction patterns
- [components/integration-models.md](../components/integration-models.md) - Integration patterns
- [testing.md](../testing.md) - Test strategy and coverage requirements
- [error-handling.md](../error-handling.md) - Error taxonomy and recovery patterns
- [behaviour/browse-and-search.md](../behaviour/browse-and-search.md) - Browsing and search behaviours