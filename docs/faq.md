# FAQ

## Purpose

Provides answers to frequently asked questions about the NuciWeb.Automation.Selenium library.

## General Questions

### What is this library for?

NuciWeb.Automation.Selenium provides a Selenium-based implementation of the `IWebProcessor` abstraction from the NuciWeb.Automation package. It enables applications to automate web browser interactions through a clean, testable interface.

### Which browsers are supported?

The library supports:
- **Firefox** (via geckodriver) - Preferred by default
- **Chrome** (via ChromeDriver) - Fallback when Firefox driver not available

### What .NET version is required?

.NET 10.0 SDK (for building) or .NET 10.0 runtime (for running).

### How do I install it?

```bash
# Option 1: NuGet package
dotnet add package NuciWeb.Automation.Selenium

# Option 2: Clone and build
git clone https://github.com/your-org/nuciweb.automation.selenium.git
cd nuciweb.automation.selenium
dotnet build
```

### Do I need to install browser drivers separately?

Yes. You need either:
- `geckodriver` at `/usr/bin/geckodriver` (for Firefox)
- `chromedriver` in PATH (for Chrome)

Download from:
- Firefox: https://github.com/mozilla/geckodriver/releases
- Chrome: https://sites.google.com/a/chromium.org/chromedriver/

## Usage Questions

### How do I initialise a driver?

```csharp
IWebDriver driver = WebDriverInitialiser.InitialiseAvailableWebDriver(
    isDebugModeEnabled: false,
    pageLoadTimeout: 90);
```

### How do I create a processor?

```csharp
IWebProcessor processor = new SeleniumWebProcessor(driver);
```

### How do I navigate to a page?

```csharp
processor.GoToUrl("https://example.com");
```

### How do I interact with elements?

```csharp
// Click
processor.Click("//button[@id='submit']");

// Set text
processor.SetText("//input[@name='username']", "myuser");

// Check element exists
bool exists = processor.DoesElementExist("//div[@id='content']");
```

### How do I handle errors?

```csharp
try
{
    processor.Click("//button[@id='missing']");
}
catch (NoSuchElementException ex)
{
    Console.WriteLine($"Element not found: {ex.Message}");
}
```

### How do I clean up resources?

```csharp
try
{
    // Use driver and processor
}
finally
{
    driver.Quit();
}
```

## Configuration Questions

### How do I enable debug mode?

```csharp
IWebDriver driver = WebDriverInitialiser.InitialiseAvailableWebDriver(
    isDebugModeEnabled: true,  // Enables visible browser
    pageLoadTimeout: 90);
```

### How do I change the timeout?

```csharp
IWebDriver driver = WebDriverInitialiser.InitialiseAvailableWebDriver(
    isDebugModeEnabled: false,
    pageLoadTimeout: 120); // 2 minutes
```

### How does browser selection work?

The library checks for `geckodriver` at `/usr/bin/geckodriver`. If found, Firefox is used. Otherwise, Chrome is used as fallback.

### Can I use explicit browser creation?

Yes:

```csharp
// Firefox
IWebDriver firefox = WebDriverInitialiser.InitialiseFirefoxDriver(false, 90);

// Chrome
IWebDriver chrome = WebDriverInitialiser.InitialiseChromeDriver(false, 90);
```

## Troubleshooting Questions

### Driver not found

1. Ensure `geckodriver` or `chromedriver` is in PATH
2. Download the appropriate driver
3. Verify executable permissions (`chmod +x`)

### Permission denied

1. Check file permissions on driver executable
2. On Windows, run as administrator if needed

### Page won't load

1. Check internet connectivity
2. Verify URL is correct
3. Increase `pageLoadTimeout`
4. Try debug mode to see errors

### Elements not found

1. Verify XPath selector is correct
2. Ensure page has fully loaded
3. Use `DoesElementExist()` before interacting
4. Check for dynamic content loading

### Headless mode issues

If headless mode fails:
1. Try debug mode (visible browser)
2. Check driver compatibility with browser version
3. Update geckodriver/ChromeDriver

## Related Documentation

- [quick-start.md](./quick-start.md) - Getting started guide
- [architecture.md](./architecture.md) - Architecture overview
- [api-reference/selenium-web-processor.md](./api-reference/selenium-web-processor.md) - Processor API
- [api-reference/web-driver-initialiser.md](./api-reference/web-driver-initialiser.md) - Initialiser API
- [error-handling.md](./error-handling.md) - Error handling patterns
- [troubleshooting.md](./troubleshooting.md) - Common issues