# Security Policy

Thank you for helping keep `Atc.Wpf` and its consumers safe.

## Supported versions

Security fixes are applied to the latest released **major** version on the [`main`](https://github.com/atc-net/atc-wpf) branch. Older major versions are not actively maintained for security fixes; please upgrade to the latest version when reporting an issue.

| Version line | Supported |
|---|---|
| Latest `main` / latest published NuGet | ✅ |
| Older major versions | ❌ |

## Reporting a vulnerability

**Please do not file a public GitHub issue for security-sensitive reports.**

Use one of these private channels instead:

1. **GitHub private vulnerability reporting** — open the repository's [Security tab](https://github.com/atc-net/atc-wpf/security) and choose **Report a vulnerability**. This routes the report directly to the maintainers and is the preferred path.
2. **Direct contact** — if private reporting is unavailable, contact the ATC.Net maintainers via [atc-net.github.io](https://atc-net.github.io) and request a private channel.

When reporting, please include:

- A description of the issue and the impact you've assessed.
- A minimal reproduction (project / control / API path / steps).
- The affected version(s) and target framework.
- Any relevant logs, stack traces, or sample code.

## What to expect

- An acknowledgement within a reasonable timeframe (typically a few business days).
- A coordinated disclosure timeline if the report is confirmed.
- Credit in the release notes once the fix ships, unless you prefer to remain anonymous.

## Scope

This policy covers issues in the `Atc.Wpf*` packages produced by this repository. Issues in upstream dependencies (e.g. `Atc`, `Atc.XamlToolkit`, `ControlzEx`, font assets) should be reported to those projects directly; if you're unsure, send the report here and we will help route it.
