[![Donate](https://img.shields.io/badge/-%E2%99%A5%20Donate-%23ff69b4)](https://hmlendea.go.ro/fund.html) [![Build Status](https://github.com/hmlendea/nuciweb.automation.selenium/actions/workflows/dotnet.yml/badge.svg)](https://github.com/hmlendea/nuciweb.automation.selenium/actions/workflows/dotnet.yml) [![Latest Release](https://img.shields.io/github/v/release/hmlendea/nuciweb.automation.selenium)](https://github.com/hmlendea/nuciweb.automation.selenium/releases/latest)

# NuciWeb.Automation.Selenium

## About

NuciWeb.Automation.Selenium provides a Selenium-based implementation for the `NuciWeb.Automation` abstractions.
It is intended for projects that want to drive a real browser through the `IWebProcessor` interface while keeping the higher-level automation logic independent from Selenium itself.

The package currently exposes two main building blocks:

- `SeleniumWebProcessor`, an `IWebProcessor` implementation backed by `IWebDriver`
- `WebDriverInitialiser`, a helper that creates a configured Firefox or Chrome driver

## Features

- Implements browser automation operations through the `NuciWeb.Automation` processor model
- Supports navigation, clicking, text input, selection, alerts, tabs, iframes, and script execution
- Includes automatic WebDriver initialisation for Firefox and Chrome
- Supports headless execution for non-debug scenarios
- Applies practical browser defaults such as reduced logging and optional image disabling

## Requirements

- .NET 10 or newer
- A compatible browser installed locally
- A matching Selenium driver available on the machine:
	- Firefox via `geckodriver` at `/usr/bin/geckodriver`
	- otherwise Chrome via the default Selenium Chrome driver resolution

## Installation

[![Get it from NuGet](https://raw.githubusercontent.com/hmlendea/readme-assets/master/badges/stores/nuget.png)](https://nuget.org/packages/NuciWeb.Automation.Selenium)

**.NET CLI**:
```bash
dotnet add package NuciWeb.Automation.Selenium
```

**Package Manager**:
```powershell
Install-Package NuciWeb.Automation.Selenium
```

## Usage

Create a driver, pass it to `SeleniumWebProcessor`, and use the processor through the `NuciWeb.Automation` API:

```csharp
using OpenQA.Selenium;
using NuciWeb.Automation;
using NuciWeb.Automation.Selenium;

IWebDriver driver = WebDriverInitialiser.InitialiseAvailableWebDriver(
		isDebugModeEnabled: false,
		pageLoadTimeout: 90);

IWebProcessor processor = new SeleniumWebProcessor(driver);

processor.GoToUrl("https://example.com");
processor.SetText("//input[@name='q']", "nuciweb");
processor.Click("//button[@type='submit']");
```

If you want to target a specific browser explicitly, use one of the dedicated initialisers:

```csharp
IWebDriver firefox = WebDriverInitialiser.InitialiseFirefoxDriver();
IWebDriver chrome = WebDriverInitialiser.InitialiseChromeDriver(isDebugModeEnabled: false);
```

## Driver Initialisation

`WebDriverInitialiser.InitialiseAvailableWebDriver()` chooses the browser as follows:

1. If `/usr/bin/geckodriver` exists, it creates a Firefox driver.
2. Otherwise, it creates a Chrome driver.

Both drivers are configured with:

- `PageLoadStrategy.None`
- a configurable page load timeout
- maximized browser window
- quieter driver logging

When debug mode is disabled, the initialiser also enables headless execution and reduces unnecessary resource loading.

## Notes

- XPath selectors are used throughout the processor API.
- The processor switches to the current tab before resolving elements.
- Navigation includes retry logic and a basic Chromium error-page check before failing.
- For deterministic automation runs, make sure the installed browser version matches the driver version available in the environment.


## License

This project is licensed under the `GNU General Public License v3.0` or later. See [LICENSE](./LICENSE) for details.
