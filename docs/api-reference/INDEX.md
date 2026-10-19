# API Reference Index

## Summary Table

| Method | Path | Controller | Action | Authentication |
|--------|------|------------|--------|----------------|
| N/A | N/A | N/A | N/A | N/A |

*Note: This is a class library, not a web API. The API reference documents the public classes and methods available for consumption.*

## Global Information

- **Library Type:** .NET Class Library
- **Target Framework:** .NET 10.0
- **Package ID:** NuciWeb.Automation.Selenium
- **Versioning:** Semantic Versioning
- **Namespace:** NuciWeb.Automation.Selenium

## Related Documentation

- [Architecture](./architecture.md) - High-level architecture overview
- [Data Model](./../data-model.md) - Domain entities and relationships
- [Components](./../components/) - Component-level documentation
- [Flows](./../flows/) - End-to-end execution flows
- [Quick Start](./../quick-start.md) - Getting started guide

## Public API Classes

### SeleniumWebProcessor
- **File:** `SeleniumWebProcessor.cs`
- **Namespace:** `NuciWeb.Automation.Selenium`
- **Description:** Implements the `IWebProcessor` interface using Selenium WebDriver for browser automation tasks
- **Documentation:** [selenium-web-processor.md](./selenium-web-processor.md)

### WebDriverInitialiser
- **File:** `WebDriverInitialiser.cs`
- **Namespace:** `NuciWeb.Automation.Selenium`
- **Description:** Provides methods to initialise Selenium WebDriver instances based on available browser drivers
- **Documentation:** [web-driver-initialiser.md](./web-driver-initialiser.md)

## Interface Contracts

### IWebProcessor (from NuciWeb.Automation)
- **Defined in:** NuciWeb.Automation package
- **Implemented by:** `SeleniumWebProcessor`
- **Description:** Defines the web automation processor abstraction that SeleniumWebProcessor implements