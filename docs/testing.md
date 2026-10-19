# Testing

## Purpose

Documents the test strategy, organisation, and coverage for the NuciWeb.Automation.Selenium library.

## Test Strategy

### Test Pyramid

```
        /\
       /  \     E2E Tests (Few)
      /----\
     /      \   Integration Tests (Some)
    /--------\
   /          \ Unit Tests (Many)
  /------------\
```

### Test Categories

| Category | Count | Purpose |
|----------|-------|---------|
| **Unit Tests** | Many | Test individual methods in isolation |
| **Integration Tests** | Some | Test WebDriver interactions |
| **E2E Tests** | Few | Test complete user flows |

## Test Organisation

### Project Structure

The NUnit 4 unit tests are in [NuciWeb.Automation.Selenium.UnitTests](../NuciWeb.Automation.Selenium.UnitTests/). The suite is divided between [SeleniumWebProcessorTests.cs](../NuciWeb.Automation.Selenium.UnitTests/SeleniumWebProcessorTests.cs) and [WebDriverInitialiserTests.cs](../NuciWeb.Automation.Selenium.UnitTests/WebDriverInitialiserTests.cs). Browser integration tests belong in a separate integration-test project and are not part of this unit-test project.

### Test Naming Convention

```text
Given[Context]_When[Action]_Then[ExpectedResult]
```

Examples:
- `GivenElementExists_WhenClicking_ThenCallsWebDriverClick`
- `GivenElementCannotBeFound_WhenClicking_ThenThrowsNotFoundException`
- `GivenDebugModeSetting_WhenInitialisingChrome_ThenConfiguresChrome`

## Unit Tests

### SeleniumWebProcessor Tests

Tests use Moq to model Selenium drivers, elements, actions, alerts, tabs, and select options. They cover public processor behavior, including retry exhaustion and navigation error handling.

### WebDriverInitialiser Tests

The initializer checks call the static API and launch real browser processes. They are marked NUnit `Explicit` and require the matching browser and driver, so the normal unit-test run does not execute them. Browser startup coverage requires running these checks in an environment with the required binaries.

## Integration Tests

### Browser Integration Tests

```csharp
[TestFixture]
public class FirefoxIntegrationTests
{
    private IWebDriver driver = null!;
    private IWebProcessor processor = null!;

    [SetUp]
    public void SetUp()
    {
        driver = WebDriverInitialiser.InitialiseFirefoxDriver(true, 30);
        processor = new SeleniumWebProcessor(driver);
    }

    [TearDown]
    public void TearDown()
    {
        driver?.Quit();
    }

    [Test]
    public void GivenValidUrl_WhenNavigating_ThenLoadsPage()
    {
        processor.GoToUrl("https://example.com");

        Assert.That(processor.IsElementVisible("//h1"), Is.EqualTo(true));
    }
}
```

## E2E Tests

### Automation Flow Tests

```csharp
[TestFixture]
public class AutomationFlowTests
{
    [Test]
    public void GivenValidCredentials_WhenLoggingIn_ThenDisplaysWelcome()
    {
        using IWebDriver driver = WebDriverInitialiser.InitialiseAvailableWebDriver(true, 60);
        SeleniumWebProcessor processor = new(driver);

        // Navigate to login
        processor.GoToUrl("https://example.com/login");

        // Fill form
        processor.SetText("//input[@name='username']", "testuser");
        processor.SetText("//input[@name='password']", "testpass");

        // Submit
        processor.Click("//button[@type='submit']");

        // Verify
        Assert.That(processor.IsElementVisible("//div[@class='welcome']"), Is.EqualTo(true));
    }
}
```

## Test Execution

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test NuciWeb.Automation.Selenium.UnitTests/

# Run with filter
dotnet test --filter "FullyQualifiedName~SeleniumWebProcessorTests"

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### CI/CD Integration

Tests run in GitHub Actions:

```yaml
- name: Test
  run: dotnet test --no-build --verbosity normal
```

## Test Data

### Test Values

Use standard test values from common.test-values.instructions.md:

- **Names:** "John Doe", "Jane Smith"
- **Emails:** "john.doe@example.com"
- **URLs:** "https://example.com"
- **Dates:** "2026-01-15"

### Test Fixtures

```csharp
public class TestFixtures
{
    public const string ValidUrl = "https://example.com";
    public const string ValidXPath = "//div[@id='content']";
    public const string ValidText = "Test content";
}
```

## Coverage Goals

| Metric | Target |
|--------|--------|
| **Line Coverage** | 100% for `SeleniumWebProcessor` |
| **Branch Coverage** | 100% for `SeleniumWebProcessor` |

The normal unit-test run does not cover [WebDriverInitialiser.cs](../NuciWeb.Automation.Selenium/WebDriverInitialiser.cs), because its checks launch external browsers.

## Related Documentation

- [architecture.md](./architecture.md) - Architecture overview
- [design-decisions.md](./design-decisions.md) - Design rationale
- [error-handling.md](./error-handling.md) - Error handling patterns
- [build-and-deployment.md](./build-and-deployment.md) - Build pipeline
- [change-guide.md](./change-guide.md) - Modification guidance