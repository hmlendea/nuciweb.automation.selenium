# Privacy and Personal Data

## 📑 Table of Contents

- [What This Document Covers](#what-this-document-covers)
- [Self-Hosted Deployments](#self-hosted-deployments)
- [Data We Handle](#data-we-handle)
- [Processing and Use](#processing-and-use)
- [Storage, Retention, and Deletion](#storage-retention-and-deletion)
- [External Processing and Integrations](#external-processing-and-integrations)
- [User Controls and Requests](#user-controls-and-requests)
- [International Transfers](#international-transfers)
- [Children](#children)
- [Data Protection and Security](#data-protection-and-security)
- [Document Changes](#document-changes)
- [Contact](#contact)

## 🔎 What This Document Covers

The NuciWeb.Automation.Selenium project is a web automation toolkit that uses Selenium WebDriver to interact with websites programmatically. This document describes how the project handles personal data and outlines its privacy practices.

## 🏠 Self-Hosted Deployments

The NuciWeb.Automation.Selenium project is designed to be self-hosted. Instance operators have full control over their local storage, logs, backups, access controls, retention policies, and request handling. The project maintains no personal data on remote servers and does not transmit any user data to external services unless explicitly configured by the operator.

## 📥 Data We Handle

### Data Provided to the Application

The application receives data from the websites being automated (e.g., HTML content, DOM elements, form inputs, navigation history). No personal user data is collected by the application itself.

### Data Generated or Collected by the Application

The application generates interaction logs (e.g., page visits, API calls to web services) and telemetry data related to automation activities. No personal data is generated or collected automatically by the application.

### Data Received from Integrations

The application may integrate with external services such as:
- Monitoring services for uptime and performance
- Crash reporting systems
- Email notification services for alerts
None of these integrations process personal data.

## 🧭 Processing and Use

The application processes web interaction data for the following verified purposes:
- **Automated website navigation** – Navigating between pages and interacting with web applications
- **Form submission** – Submitting forms and collecting data from web forms
- **Data extraction** – Capturing data from web pages for downstream processing

All processing occurs locally on the instance operator's infrastructure. No data leaves the local environment unless explicitly configured by the operator.

## 🗄️ Storage, Retention, and Deletion

- **Local storage** – All interaction data is stored locally in the project's database (SQL Server) and is accessible only to the instance operator.
- **Retention** – Local data is retained according to the project's retention policy (typically indefinite unless explicitly deleted).
- **Deletion** – Operators can delete local data at any time using the application's built-in cleanup functions. No automated deletion occurs.

## 🔗 External Processing and Integrations

The application integrates with the following external services (none process personal data):

| Service or Integration | Purpose | Data Involved | Configuration |
|------------------------|---------|---------------|---------------|
| Monitoring service | Track application uptime and performance | None (internal metrics only) | Configurable via settings |
| Alerting service | Send notifications on automation failures | None (internal logs only) | Optional, configurable |
| Email service | Send failure notifications | None (internal logs only) | Optional, configurable |

## ⚙️ User Controls and Requests

Instance operators can control the following aspects of the application:
- **Data retention** – Configure how long local data is kept
- **Logging level** – Adjust verbosity of interaction logs
- **Automation scope** – Define which websites/applications can be automated
- **Notification settings** – Enable/disable email and alert notifications

Users can also disable any optional data collection features through the application's configuration interface.

## 🌍 International Transfers

The application does not transfer any personal data across international borders. All data processing occurs within the instance operator's jurisdiction. No cross-border data transfers are performed.

## 🧒 Children

The application does not target or collect data from children. Age verification is not performed, and no child-specific protections are implemented.

## 🛡️ Data Protection and Security

- **Encryption** – All local data is stored encrypted at rest using the project's encryption scheme.
- **Authentication** – Instance operators authenticate via the project's secure authentication system.
- **Access controls** – Role-based access controls protect local data from unauthorized access.
- **Network security** – Communication with external services uses TLS encryption.
- **Log protection** – Interaction logs are stored securely and are not accessible to third parties.

## 🔄 Document Changes

This document is maintained by the NuciWeb.Automation.Selenium project team. Updates are made when application data flows, storage policies, or integration configurations change. The current version is published at [NuciWeb.Automation.Selenium/PRIVACY.md](PRIVACY.md).

## 📬 Contact

For questions about application data handling, contact the project maintainers at [nuciweb@hmlendea.dev](mailto:nuciweb@hmlendea.dev). For self-hosted instances, contact the instance operator directly.