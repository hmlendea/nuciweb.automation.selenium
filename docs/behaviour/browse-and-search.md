# Browse and Search Behaviour

## Purpose

Defines the user-facing behaviours for browsing web content and searching for elements within the NuciWeb.Automation.Selenium library.

## Scope

This documentation covers the core browsing and search behaviours that are implemented through the `IWebProcessor` interface and executed by the `SeleniumWebProcessor` class.

## Key Flows

### Browsing Flow

1. **Navigation:** Application calls `GoToUrl` to navigate to a URL
2. **Element Location:** Application calls `FindElement` or related methods to locate elements
3. **Element Interaction:** Application performs actions like clicking, typing, or selecting on located elements
4. **Navigation Return:** Results flow back through the abstraction layer to the application

### Search Flow

1. **Search Request:** Application requests element location via XPath or other selectors
2. **Element Discovery:** Processor locates element(s) using Selenium WebDriver
3. **State Verification:** Checks element visibility, existence, and state before interaction
4. **Action Execution:** Performs the requested interaction (click, type, select, etc.)
5. **Result Return:** Returns result to application through abstraction layer

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

- **SeleniumWebProcessor:** Implements all browsing and search behaviours
- **WebDriverInitialiser:** Creates browser driver instances used by SeleniumWebProcessor
- **IWebProcessor:** Defines the abstraction layer for all browsing operations

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