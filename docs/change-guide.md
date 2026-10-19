# Change Guide

## Purpose

Provides guidance on how to safely modify common areas of the NuciWeb.Automation.Selenium codebase.

## Modification Areas

### SeleniumWebProcessor.cs

**Risk Level:** High - Core implementation of IWebProcessor

**Safe Modification Patterns:**

1. **Adding New Methods:**
   - Add to `IWebProcessor` interface first (in NuciWeb.Automation)
   - Implement in `SeleniumWebProcessor`
   - Follow existing naming conventions
   - Add XML documentation

2. **Modifying Existing Methods:**
   - Preserve method signatures
   - Maintain exception behaviour
   - Update XML documentation
   - Add tests for changed behaviour

3. **Internal Refactoring:**
   - Extract private helper methods
   - Maintain public API compatibility
   - Ensure thread-safety assumptions unchanged

**Dangerous Changes:**
- Changing method signatures
- Removing public methods
- Altering exception types thrown
- Modifying thread-safety guarantees

### WebDriverInitialiser.cs

**Risk Level:** Medium - Driver creation utility

**Safe Modification Patterns:**

1. **Adding Browser Support:**
   - Add new static method for new browser
   - Follow existing naming pattern
   - Maintain same parameter signature
   - Update `InitialiseAvailableWebDriver` logic

2. **Configuration Options:**
   - Add optional parameters with defaults
   - Preserve backward compatibility
   - Document new options

3. **Timeout/Window Size:**
   - Make configurable via parameters
   - Keep current values as defaults

**Dangerous Changes:**
- Removing existing methods
- Changing parameter order
- Removing debug mode support
- Hardcoding browser-specific logic

## Testing Requirements

### Before Any Change

1. Run existing tests: `dotnet test`
2. Verify build succeeds: `dotnet build`
3. Check for compiler warnings

### After Changes

1. Add tests for new functionality
2. Update tests for modified behaviour
3. Run full test suite
4. Verify no regressions

### Integration Testing

For driver-related changes:
1. Test with Firefox (geckodriver)
2. Test with Chrome (ChromeDriver)
3. Test headless mode
4. Test debug mode
4. Test timeout scenarios

## Versioning Impact

| Change Type | Version Bump |
|-------------|--------------|
| Bug fix | Patch |
| New method (non-breaking) | Minor |
| New parameter with default | Minor |
| Method signature change | Major |
| Method removal | Major |
| Exception type change | Major |
| Behaviour change | Major |

## Code Review Checklist

- [ ] Public API unchanged or properly versioned
- [ ] XML documentation updated
- [ ] Tests added/updated
- [ ] Build succeeds
- [ ] All tests pass
- [ ] No new compiler warnings
- [ ] Thread-safety documented
- [ ] Exception behaviour documented
- [ ] Backward compatibility maintained

## Related Documentation

- [architecture.md](./architecture.md) - Architecture overview
- [api-reference/selenium-web-processor.md](./api-reference/selenium-web-processor.md) - Processor API
- [api-reference/web-driver-initialiser.md](./api-reference/web-driver-initialiser.md) - Initialiser API
- [testing.md](./testing.md) - Test strategy
- [design-decisions.md](./design-decisions.md) - Design rationale