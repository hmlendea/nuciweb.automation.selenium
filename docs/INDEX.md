# Repository Overview

## Purpose

NuciWeb.Automation.Selenium provides a Selenium-based implementation for the `NuciWeb.Automation` abstractions. It enables projects to drive real browsers through a clean abstraction while keeping higher-level automation logic independent from Selenium-specific details. The library targets .NET 10.0 and supports Firefox and Chrome browsers.

## System Context

The library operates as a dependency for applications requiring web automation. It interacts with:

- **Application Code:** Uses the library through the `IWebProcessor` interface
- **Selenium WebDriver:** Direct dependency for browser automation
- **Browser Drivers:** External `geckodriver` (Firefox) or ChromeDriver executables
- **Web Browsers:** Firefox or Chrome browsers controlled via WebDriver

## Documentation Catalogue

### Root Documents

- [ARCHITECTURE.md](./ARCHITECTURE.md) - High-level architecture documentation
- [SECURITY.md](./SECURITY.md) - Security policy and vulnerability reporting
- [ROADMAP.md](./ROADMAP.md) - Project roadmap and future plans
- [PRIVACY.md](./PRIVACY.md) - Privacy and data handling practices
- [LICENSE](./LICENSE) - Project license terms

### Generated Documentation

#### API Reference

- [api-reference/INDEX.md](./api-reference/INDEX.md) - API reference index and endpoint summaries
- [api-reference/selenium-web-processor.md](./api-reference/selenium-web-processor.md) - `SeleniumWebProcessor` class documentation
- [api-reference/web-driver-initialiser.md](./api-reference/web-driver-initialiser.md) - `WebDriverInitialiser` class documentation

#### Component Deep Dives

- [components/presentation.md](./components/presentation.md) - Presentation layer components
- [components/host-and-composition.md](./components/host-and-composition.md) - Host and composition architecture
- [components/application-services.md](./components/application-services.md) - Application services layer
- [components/browser-state-and-localisation.md](./components/browser-state-and-localisation.md) - Browser state and localization
- [components/integration-models.md](./components/integration-models.md) - Integration models and patterns

#### Execution Flows

- [flows/startup-and-rendering.md](./flows/startup-and-rendering.md) - Application startup and rendering flows
- [flows/calendar-search.md](./flows/calendar-search.md) - Calendar search functionality flows
- [flows/detail-mutation.md](./flows/detail-mutation.md) - Detail mutation and editing flows

#### User-Facing Behaviours

- [behaviour/browse-and-search.md](./behaviour/browse-and-search.md) - Browsing and search behaviours
- [behaviour/inspect-edit-delete.md](./behaviour/inspect-edit-delete.md) - Inspect, edit, and delete behaviours

#### Technical Documentation

- [architecture.md](./architecture.md) - High-level architecture (complements root ARCHITECTURE.md)
- [build-and-deployment.md](./build-and-deployment.md) - Build pipeline and deployment processes
- [change-guide.md](./change-guide.md) - How to modify common areas safely
- [configuration.md](./configuration.md) - Configuration schema and sources
- [concurrency-and-scheduling.md](./concurrency-and-scheduling.md) - Threading, async, and scheduling
- [data-model.md](./data-model.md) - Domain entities and relationships
- [dependencies.md](./dependencies.md) - External and internal dependencies
- [design-decisions.md](./design-decisions.md) - Key architectural and design choices
- [documentation-maintenance.md](./documentation-maintenance.md) - Documentation maintenance guidelines
- [error-handling.md](./error-handling.md) - Error taxonomy and handling patterns
- [faq.md](./faq.md) - Frequently asked questions
- [invariants.md](./invariants.md) - System-wide invariants and contracts
- [integrations.md](./integrations.md) - External system integrations
- [logging.md](./logging.md) - Logging framework and practices
- [quick-start.md](./quick-start.md) - Getting started guide for newcomers
- [repository-structure.md](./repository-structure.md) - Source tree layout and module organization
- [security.md](./security.md) - Security model and mitigations (complements root SECURITY.md)
- [state-and-persistence.md](./state-and-persistence.md) - State management and persistence
- [testing.md](./testing.md) - Test strategy and organization
- [troubleshooting.md](./troubleshooting.md) - Common issues and solutions

## Start Here

For newcomers to the project, begin with:

1. **[Quick Start](./quick-start.md)** - Getting started guide for new developers
2. **[Architecture](./architecture.md)** - High-level system architecture overview
3. **[Repository Structure](./repository-structure.md)** - Understanding the codebase layout
4. **[API Reference](./api-reference/INDEX.md)** - Reference documentation for all public APIs

## Deep Dive

For component owners and maintainers, explore:

1. **[Components](./components/)** - Detailed component architecture and implementation
2. **[Flows](./flows/)** - End-to-end execution flows and user journeys
3. **[Behaviour](./behaviour/)** - User-facing behaviour documentation
4. **[Design Decisions](./design-decisions.md)** - Rationale behind architectural choices

## For Debuggers

For developers troubleshooting issues:

1. **[Troubleshooting](./troubleshooting.md)** - Common problems and solutions
2. **[Error Handling](./error-handling.md)** - Error taxonomy and recovery patterns
3. **[Testing](./testing.md)** - Test strategy and verification approaches
4. **[Logging](./logging.md)** - Logging framework and correlation

## Maintenance Metadata

- **Last Reviewed:** 2026-10-19
- **Owner:** NuciWeb.Automation.Selenium Project Team
- **Coverage Status:** Comprehensive - All components documented
- **Next Review:** 2026-12-19