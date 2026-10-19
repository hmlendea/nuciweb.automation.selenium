# Quick Start

## Purpose

Provides a rapid introduction to the NuciWeb.Automation.Selenium library for new developers and users.

## Prerequisites

### System Requirements

- **Operating System:** Linux, macOS, or Windows
- **.NET Runtime:** .NET 10.0 SDK (for building) or .NET 10.0 runtime (for running)
- **Browser Drivers:** Either `geckodriver` (Firefox) or ChromeDriver executable

### Installation

#### Option 1: Using Package Manager

```bash
# Install the NuciWeb.Automation.Selenium package
dotnet add package NuciWeb.Automation.Selenium
```

#### Option 2: Clone the Repository

```bash
# Clone the repository
git clone https://github.com/your-org/nuciweb.automation.selenium.git
cd nuciweb.automation.selenium

# Build the project
dotnet build
```

## Basic Usage

### Step 1: Initialise a Web Driver

```csharp
using OpenQA.Selenium;
using NuciWeb.Automation;
using NuciWeb.Automation.Selenium;

// Automatic browser selection (Firefox preferred)
IWebDriver driver = WebDriverInitialiser.InitialiseAvailableWebDriver(
    isDebugModeEnabled: false,
    pageLoadTimeout: 90);
```

### Step 2: Create a Processor

```csharp
IWebProcessor processor = new SeleniumWebProcessor(driver);
```

### Step 3: Navigate to a Page

```csharp
processor.GoToUrl("https://example.com");
```

### Step 4: Interact with Elements

```csharp
// Click a button
processor.Click("//button[@id='submit']");

// Enter text
processor.SetText("//input[@name='username']", "myusername");

// Verify element exists
if (processor.DoesElementExist("//div[@id='content']"))
{
    Console.WriteLine("Content found!");
}
```

### Step 5: Clean Up

```csharp
driver.Quit();
```

## Complete Example

```csharp
using OpenQA.Selenium;
using NuciWeb.Automation;
using NuciWeb.Automation.Selenium;

public class QuickStartExample
{
    public void RunExample()
    {
        // Initialise driver
        IWebDriver driver = WebDriverInitialiser.InitialiseAvailableWebDriver(
            isDebugModeEnabled: false,
            pageLoadTimeout: 90);

        try
        {
            // Create processor
            IWebProcessor processor = new SeleniumWebProcessor(driver);

            // Navigate to login page
            processor.GoToUrl("https://example.com/login");

            // Fill and submit login form
            processor.SetText("//input[@name='username']", "testuser");
            processor.SetText("//input[@name='password']", "testpass");
            processor.Click("//button[@type='submit']");

            // Wait for navigation
            System.Threading.Thread.Sleep(2000);

            // Verify successful login
            if (processor.IsElementVisible("//div[@class='welcome-message']"))
            {
                Console.WriteLine("Login successful!");
            }
        }
        finally
        {
            // Always clean up
            driver.Quit();
        }
    }
}
```

## Common Tasks

### Browser Configuration

```csharp
// Initialise with debug mode (no headless)
IWebDriver debugDriver = WebDriverInitialiser.InitialiseAvailableWebDriver(
    isDebugModeEnabled: true,
    pageLoadTimeout: 90);

// Initialise with custom timeout
IWebDriver customDriver = WebDriverInitialiser.InitialiseAvailableWebDriver(
    isDebugModeEnabled: false,
    pageLoadTimeout: 120);
```

### Element State Checks

```csharp
// Check if element is visible
bool isVisible = processor.IsElementVisible("//div[@id='content']");

// Check if checkbox is checked
bool isChecked = processor.IsCheckboxChecked("//input[@type='checkbox']");

// Check if option is selected
bool isSelected = processor.IsSelected("//option[@value='option1']");
```

### Text Operations

```csharp
// Get text from elements
IEnumerable<string> texts = processor.GetText("//div[@class='item']");

// Get selected option text
IEnumerable<string> selectedTexts = processor.GetSelectedText("//select[@id='dropdown']");
```

## Troubleshooting

### Driver Not Found

If the browser driver is not found:

1. Ensure `geckodriver` or ChromeDriver is in your PATH
2. Download the appropriate driver from:
   - Firefox: https://github.com/mozilla/geckodriver/releases
   - Chrome: https://sites.google.com/a/chromium.org/chromedriver/

### Permission Denied

If you encounter permission errors:

1. Ensure the driver executable has execute permissions (`chmod +x`)
2. On Windows, run as administrator if necessary

### Network Issues

If pages fail to load:

1. Check internet connectivity
2. Verify the URL is correct
3. Increase `pageLoadTimeout` if needed

## Related Documentation

- [architecture.md](./architecture.md) - High-level architecture overview
- [api-reference/selenium-web-processor.md](./api-reference/selenium-web-processor.md) - Processor API reference
- [api-reference/web-driver-initialiser.md](./api-reference/web-driver-initialiser.md) - Initialiser API reference
- [error-handling.md](./error-handling.md) - Error handling patterns
- [troubleshooting.md](./troubleshooting.md) - Common issues and solutions