# Security Policy

This document outlines the security policy for the NuciWeb.Automation.Selenium project, including supported versions, vulnerability reporting procedures, and disclosure expectations.

## 📑 Table of Contents

- [Supported Versions](#supported-versions)
- [Reporting a Vulnerability](#reporting-a-vulnerability)
- [Scope](#scope)
- [Disclosure Policy](#disclosure-policy)
- [Safe Harbour](#safe-harbour)
- [Recognition](#recognition)

## 🛡️ Supported Versions

Use this table to indicate which project versions currently receive security maintenance.

| Version | Distribution Channel | Supported |
|---------|--------------------|-----------|
| Latest version | NuGet | ✅ |
| Latest version | GitHub Releases | ✅ |
| Latest version | Unofficial third-party distribution channels | ❌ |
| Preceding versions | Any distribution channel | ❌ |

## 🚨 Reporting a Vulnerability

Please do not disclose suspected vulnerabilities publicly before maintainers have had an opportunity to validate and remediate them.

To report a vulnerability:
- [GitHub Security Advisories](https://github.com/hmlendea/nuciweb.automation.selenium/security/advisories)
- Contact the maintainers directly at [nuciweb@hmlendea.dev](mailto:nuciweb@hmlendea.dev)

## 📌 Scope

The subsequent report categories are in scope for this repository:
- Vulnerabilities in the Selenium WebDriver integration
- Vulnerabilities in the web processor implementation
- Vulnerabilities in the WebDriver initialisation logic

The subsequent categories are out of scope unless explicitly stated to the contrary:
- Vulnerabilities in third-party browser drivers or browsers
- Vulnerabilities in the underlying NuciWeb.Automation abstractions
- Vulnerabilities in third-party dependencies unless they affect this project's usage

## 📢 Disclosure Policy

This project follows coordinated disclosure:
1. Vulnerabilities are investigated privately.
2. A remediation plan is prepared and validated.
3. Public disclosure is published after a fix, mitigation, or agreed risk decision is available.
4. Credit is attributed in accordance with reporter preference and project policy.

## 🧾 Safe Harbour

If your research is conducted in good faith, confined to authorised scope, and disclosed responsibly, the maintainers will not pursue action for policy-compliant activity.

## 🙏 Recognition

We appreciate responsible disclosure. Reporters who desire public attribution may be acknowledged in release notes, advisories, or a dedicated acknowledgements section.