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

```
NuciWeb.Automation.Selenium.Tests/
├── Unit/
│   ├── SeleniumWebProcessorTests.cs
│   └── WebDriverInitialiserTests.cs
├── Integration/
│   ├── FirefoxIntegrationTests.cs
│   └── ChromeIntegrationTests.cs
└── E2E/
    └── AutomationFlowTests.cs
```

### Test Naming Convention

```
[MethodName]_[Scenario]_[ExpectedResult]
```

Examples:
- `Click_ElementExists_ReturnsSuccess`
- `Click_ElementNotFound_ThrowsNoSuchElementException`
- `GoToUrl_ValidUrl_NavigatesSuccessfully`

## Unit Tests

### SeleniumWebProcessor Tests

```csharp
[TestClass]
public class SeleniumWebProcessorTests
{
    [TestMethod]
    public void Click_ElementExists_CallsWebDriverClick()
    {
        // Arrange
        var mockDriver = new Mock<IWebDriver>();
        var mockElement = new Mock<IWebElement>();
        mockDriver.Setup(d => d.FindElement(By.XPath("//button"))).Returns(mockElement.Object);
        
        var processor = new SeleniumWebProcessor(mockDriver.Object);
        
        // Act
        processor.Click("//button");
        
        // Assert
        mockElement.Verify(e => e.Click(), Times.Once);
    }
    
    [TestMethod]
    public void DoesElementExist_ElementExists_ReturnsTrue()
    {
        // Arrange
        var mockDriver = new Mock<IWebDriver>();
        mockDriver.Setup(d => d.FindElements(By.XPath("//div"))).Returns(new List<IWebElement> { new Mock<IWebElement>().Object });
        
        var processor = new SeleniumWebProcessor(mockDriver.Object);
        
        // Act
        var result = processor.DoesElementExist("//div");
        
        // Assert
        Assert.IsTrue(result);
    }
}
```

### WebDriverInitialiser Tests

```csharp
[TestClass]
public class WebDriverInitialiserTests
{
    [TestMethod]
    public void InitialiseFirefoxDriver_ValidParameters_ReturnsDriver()
    {
        // This test requires geckodriver to be installed
        // Skip if not available
        if (!File.Exists("/usr/bin/geckodriver"))
        {
            Assert.Inconclusive("geckodriver not installed");
        }
        
        // Act
        var driver = WebDriverInitialiser.InitialiseFirefoxDriver(false, 90);
        
        // Assert
        Assert.IsNotNull(driver);
        driver.Quit();
    }
}
```

## Integration Tests

### Browser Integration Tests

```csharp
[TestClass]
public class FirefoxIntegrationTests
{
    private IWebDriver _driver;
    private IWebProcessor _processor;
    
    [TestInitialize]
    public void Setup()
    {
        _driver = WebDriverInitialiser.InitialiseFirefoxDriver(true, 30);
        _processor = new SeleniumWebProcessor(_driver);
    }
    
    [TestCleanup]
    public void Cleanup()
    {
        _driver?.Quit();
    }
    
    [TestMethod]
    public void GoToUrl_ValidUrl_LoadsPage()
    {
        // Act
        _processor.GoToUrl("https://example.com");
        
        // Assert
        Assert.IsTrue(_processor.IsElementVisible("//h1"));
    }
}
```

## E2E Tests

### Automation Flow Tests

```csharp
[TestClass]
public class AutomationFlowTests
{
    [TestMethod]
    public void LoginFlow_ValidCredentials_Succeeds()
    {
        using var driver = WebDriverInitialiser.InitialiseAvailableWebDriver(true, 60);
        var processor = new SeleniumWebProcessor(driver);
        
        // Navigate to login
        processor.GoToUrl("https://example.com/login");
        
        // Fill form
        processor.SetText("//input[@name='username']", "testuser");
        processor.SetText("//input[@name='password']", "testpass");
        
        // Submit
        processor.Click("//button[@type='submit']");
        
        // Verify
        Assert.IsTrue(processor.IsElementVisible("//div[@class='welcome']"));
    }
}
```

## Test Execution

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test NuciWeb.Automation.Selenium.Tests/

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
| **Line Coverage** | > 80% |
| **Branch Coverage** | > 70% |
| **Method Coverage** | > 90% |

## Related Documentation

- [architecture.md](./architecture.md) - Architecture overview
- [design-decisions.md](./design-decisions.md) - Design rationale
- [error-handling.md](./error-handling.md) - Error handling patterns
- [build-and-deployment.md](./build-and-deployment.md) - Build pipeline
- [change-guide.md](./change-guide.md) - Modification guidance