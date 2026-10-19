# Ambiguities and Open Questions

## Purpose

Documents known ambiguities, open questions, and unresolved items in the NuciWeb.Automation.Selenium project.

## Known Ambiguities

### Browser Selection Logic

**Question:** What happens when both `geckodriver` and ChromeDriver are available on the system?

**Current Behaviour:** `WebDriverInitialiser.InitialiseAvailableWebDriver()` prioritises Firefox if `geckodriver` exists at `/usr/bin/geckodriver`, otherwise falls back to Chrome.

**Open Question:** Should there be an explicit configuration option to force a specific browser, or is automatic detection sufficient?

### Headless Mode Configuration

**Question:** What is the expected behaviour when debug mode is enabled but headless arguments are still present?

**Current Behaviour:** When `isDebugModeEnabled = true`, headless mode is disabled for both Firefox and Chrome.

**Open Question:** Should there be a middle ground between full debug and full headless mode?

### Window Size Configuration

**Question:** Why is the window size fixed at `1920x1080` rather than being configurable?

**Current Behaviour:** `DefaultBrowserSize = new Size(1920, 1080)` is a constant used across all driver initialisers.

**Open Question:** Should window size be configurable via method parameters, or is the fixed size intentional for consistency?

### User Agent Fetching

**Question:** What happens when `NuciWeb.HTTP` service is unavailable during Chrome driver initialisation?

**Current Behaviour:** `UserAgentFetcher.GetUserAgent().Result` is called synchronously, which may throw if the service is unavailable.

**Open Question:** Should user agent fetching have a fallback mechanism, or should the service availability be the caller's responsibility?

## Open Questions for Future Consideration

- **Multi-Browser Support:** Should the library support browsers beyond Firefox and Chrome (Edge, Safari)?
- **Mobile Emulation:** Should there be support for mobile browser emulation?
- **Network Conditions:** Should there be support for simulating network conditions (offline, slow 3G, etc.)?
- **Performance Metrics:** Should the library expose performance metrics (page load times, element find times)?
- **Screenshot Capability:** Should the library support screenshot capture?
- **Network Interception:** Should the library support request/response interception?
- **Proxy Configuration:** Should the library support proxy configuration for browser drivers?
- **Authentication Handling:** Should the library provide built-in authentication handling?
- **Cookie Management:** Should the library provide explicit cookie management methods?
- **Local Storage Management:** Should the library provide explicit local storage management methods?
- **Console Log Capture:** Should the library support capturing browser console logs?
- **Network Error Handling:** Should the library provide more granular network error handling?
- **Timeout Configuration:** Should all timeouts be configurable via constructor or method parameters?
- **Retry Policies:** Should retry policies be configurable per-operation?
- **Error Detail Levels:** Should error messages include more detail about what failed?
- **Thread Safety:** Should `SeleniumWebProcessor` be thread-safe, or is single-threaded usage expected?
- **Session Management:** Should the library support session persistence across processor instances?
- **Browser Version Detection:** Should the library detect and report browser/version information?
- **Driver Version Management:** Should the library manage driver version compatibility?
- **CI/CD Integration:** Should the library provide specific guidance for CI/CD pipeline integration?
- **Docker Support:** Should the library provide Docker-specific configuration guidance?
- **WSL Support:** Should the library provide specific guidance for Windows Subsystem for Linux?
- **ARM Architecture:** Should the library provide specific guidance for ARM-based systems?

## Resolution Process

When ambiguities are resolved or open questions are answered:

1. Update this document with the resolution
2. Modify source code to implement the change
3. Update relevant documentation files
4. Add or update tests to verify the change
5. Bump version if breaking changes are introduced

## Related Documentation

- [architecture.md](../architecture.md) - High-level architecture overview
- [components/host-and-composition.md](../components/host-and-composition.md) - Host and composition
- [api-reference/selenium-web-processor.md](../api-reference/selenium-web-processor.md) - Processor API reference
- [api-reference/web-driver-initialiser.md](../api-reference/web-driver-initialiser.md) - Initialiser API reference
- [error-handling.md](../error-handling.md) - Error taxonomy and recovery patterns