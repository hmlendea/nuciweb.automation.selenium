# API Usage Examples

## Purpose

Provides example requests and responses for using the NuciWeb.Automation.Selenium library through the `IWebProcessor` interface.

## Basic Usage

### Driver Initialisation

```csharp
using OpenQA.Selenium;
using NuciWeb.Automation;
using NuciWeb.Automation.Selenium;

// Automatic browser selection
IWebDriver driver = WebDriverInitialiser.InitialiseAvailableWebDriver(
    isDebugModeEnabled: false,
    pageLoadTimeout: 90);

// Explicit Firefox
IWebDriver firefox = WebDriverInitialiser.InitialiseFirefoxDriver(
    isDebugModeEnabled: false,
    pageLoadTimeout: 90);

// Explicit Chrome
IWebDriver chrome = WebDriverInitialiser.InitialiseChromeDriver(
    isDebugModeEnabled: false,
    pageLoadTimeout: 90);
```

### Processor Creation

```csharp
IWebProcessor processor = new SeleniumWebProcessor(driver);
```

### Navigation

```csharp
// Navigate to URL
processor.GoToUrl("https://example.com");

// Refresh page
processor.Refresh();
```

### Element Interaction

```csharp
// Click element
processor.Click("//button[@id='submit']");

// Set text in input field
processor.SetText("//input[@name='username']", "myusername");

// Move mouse to element
processor.MoveToElement("//div[@class='menu']");
```

### Element State Queries

```csharp
// Check if element exists
bool exists = processor.DoesElementExist("//div[@id='content']");

// Check if element is visible
bool visible = processor.IsElementVisible("//div[@id='content']");

// Check if checkbox is checked
bool checked = processor.IsCheckboxChecked("//input[@type='checkbox']");

// Check if element is selected
bool selected = processor.IsSelected("//option[@value='option1']");
```

### Text and Attribute Retrieval

```csharp
// Get text from elements
IEnumerable<string> texts = processor.GetText("//div[@class='item']");

// Get attribute values
IEnumerable<string> hrefs = processor.GetAttribute("//a", "href");

// Get selected option text
IEnumerable<string> selectedTexts = processor.GetSelectedText("//select[@id='dropdown']");

// Get select options count
int count = processor.GetSelectOptionsCount("//select[@id='dropdown']");
```

### Script Execution

```csharp
// Execute JavaScript
string result = processor.ExecuteScript("return document.title;");

// Get page source
string pageSource = processor.GetPageSource();
```

### Tab Management

```csharp
// Open new tab
string newTab = processor.NewTab("https://example.com");

// Switch to tab
processor.SwitchToTab(newTab);

// Close tab
processor.CloseTab(newTab);
```

### Alert Handling

```csharp
// Accept alert
processor.AcceptAlert();

// Dismiss alert
processor.DismissAlert();
```

### Iframe Navigation

```csharp
// Switch to iframe
processor.SwitchToIframe("//iframe[@id='content-frame']");
```

## Complete Example

```csharp
using OpenQA.Selenium;
using NuciWeb.Automation;
using NuciWeb.Automation.Selenium;

public class AutomationExample
{
    public void RunAutomation()
    {
        // Initialise driver
        IWebDriver driver = WebDriverInitialiser.InitialiseAvailableWebDriver(
            isDebugModeEnabled: false,
            pageLoadTimeout: 90);

        try
        {
            // Create processor
            IWebProcessor processor = new SeleniumWebProcessor(driver);

            // Navigate to page
            processor.GoToUrl("https://example.com/login");

            // Fill login form
            processor.SetText("//input[@name='username']", "myuser");
            processor.SetText("//input[@name='password']", "mypassword");

            // Submit form
            processor.Click("//button[@type='submit']");

            // Verify login success
            if (processor.IsElementVisible("//div[@class='welcome-message']"))
            {
                Console.WriteLine("Login successful!");
            }

            // Navigate to dashboard
            processor.GoToUrl("https://example.com/dashboard");

            // Extract data
            var items = processor.GetText("//div[@class='dashboard-item']");
            foreach (var item in items)
            {
                Console.WriteLine($"Item: {item}");
            }
        }
        finally
        {
            // Clean up
            driver.Quit();
        }
    }
}
```

## Error Handling Examples

```csharp
try
{
    processor.GoToUrl("https://example.com");
}
catch (Exception ex) when (ex.Message.Contains("Failed to load"))
{
    Console.WriteLine("Navigation failed after retries");
}

try
{
    processor.Click("//button[@id='missing']");
}
catch (NotFoundException ex)
{
    Console.WriteLine($"Element not found: {ex.Message}");
}
```

## Related Documentation

- [api-reference/selenium-web-processor.md](./api-reference/selenium-web-processor.md) - Processor API reference
- [api-reference/web-driver-initialiser.md](./api-reference/web-driver-initialiser.md) - Initialiser API reference
- [quick-start.md](./quick-start.md) - Getting started guide
- [flows/startup-and-rendering.md](./flows/startup-and-rendering.md) - Startup flow