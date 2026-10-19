# Security

## Purpose

Documents the security model, threats, and mitigations for the NuciWeb.Automation.Selenium library. Complements the root SECURITY.md file.

## Security Model

### Threat Assessment

The library operates with the following threat model:

| Threat | Likelihood | Impact | Mitigation |
|--------|------------|--------|------------|
| **Driver Execution** | Medium | High | Ensure driver executables have restricted permissions |
| **Browser Vulnerabilities** | Low | High | Keep browsers and drivers updated |
| **Network Exposure** | Medium | Medium | Use HTTPS for navigation; avoid sensitive data in URLs |
| **Script Injection** | Low | Medium | Sanitize any user-provided JavaScript |
| **Credential Exposure** | Low | High | Never log passwords or sensitive data |

### Attack Surface

The library's attack surface is limited to:

1. **XPath Selectors:** Malformed selectors may cause unexpected behaviour
2. **URL Navigation:** User-provided URLs may contain malicious content
3. **JavaScript Execution:** User-provided scripts may execute in browser context
4. **File Downloads:** Browser-driven downloads may expose system to malware

### Security Best Practices

```csharp
// Never log sensitive information
logger.Debug($"Navigated to: {url}"); // OK - URL is not sensitive

// Use HTTPS for sensitive pages
processor.GoToUrl("https://example.com/secure");

// Sanitize user input before using in selectors
string safeSelector = SanitizeXPath(userProvidedInput);

// Avoid executing arbitrary scripts
processor.ExecuteScript(userProvidedScript); // DANGEROUS - avoid
```

### Dependency Security

| Dependency | Version | Security Status |
|------------|---------|-----------------|
| Selenium.WebDriver | 4.5.1 | Review for CVEs regularly |
| Selenium.Support | 4.5.1 | Review for CVEs regularly |
| NuciWeb | 4.0.0 | Review for CVEs regularly |
| NuciWeb.Automation | 1.0.0 | Review for CVEs regularly |
| NuciWeb.HTTP | 1.5.0 | Review for CVEs regularly |

### Reporting Vulnerabilities

Please report security vulnerabilities following the guidelines in SECURITY.md at the repository root.

## Related Documentation

- [SECURITY.md](../SECURITY.md) - Root security policy
- [architecture.md](./architecture.md) - Architecture overview
- [design-decisions.md](./design-decisions.md) - Design rationale
- [error-handling.md](./error-handling.md) - Error handling patterns