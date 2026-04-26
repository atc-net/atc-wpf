# Contributing to Atc.Wpf

Thanks for your interest in contributing! Atc.Wpf is part of the [ATC.Net](https://atc-net.github.io) family of open-source libraries. This file is a quick local pointer; the canonical contribution and coding guidelines live at the ATC umbrella site.

## 📚 Authoritative guidelines

- **[ATC contribution guidelines](https://atc-net.github.io/introduction/about-atc#how-to-contribute)** — what to expect from PRs, branch naming, etc.
- **[ATC coding guidelines](https://atc-net.github.io/introduction/about-atc#coding-guidelines)** — code style, naming, testing.
- **[Code of conduct](CODE_OF_CONDUCT.md)** — community standards.
- **[Security policy](SECURITY.md)** — how to report vulnerabilities.

## 🛠️ Getting started

```bash
git clone https://github.com/atc-net/atc-wpf.git
cd atc-wpf
dotnet build
dotnet test
dotnet run --project sample/Atc.Wpf.Sample
```

> Requires **.NET 10 Desktop Runtime** on Windows 10 or later.

## ✅ Before opening a PR

1. **Build clean in Release.** Release builds treat warnings as errors:
   ```bash
   dotnet build -c Release
   ```
2. **Run the affected test suites.** Microsoft Testing Platform requires `dotnet run`:
   ```bash
   dotnet run --project test/Atc.Wpf.Tests -c Release --no-build
   dotnet run --project test/Atc.Wpf.Controls.Tests -c Release --no-build
   # …etc per affected library
   ```
   See [`CLAUDE.md`](CLAUDE.md#testing) for the full list of test projects and per-project filter syntax.
3. **Add or update tests** for the change. Functional coverage is uneven across the libraries — new tests are always welcome.
4. **Update relevant readmes.** Each control may have a `[Control]_Readme.md` next to its source — update it if behaviour or properties change. See the **Control Readme Conventions** section in [`CLAUDE.md`](CLAUDE.md) for the discovery rules.
5. **Add a `CHANGELOG.md` entry** under the `[Unreleased]` section using Keep-a-Changelog phrasing.
6. **Use Conventional Commits** for commit messages (e.g. `fix(controls): …`, `feat(theming): …`, `perf(layouts): …`, `docs(forms): …`).

## 🧱 Project structure

The repo is organised as a four-tier control library:

```
src/
├── Atc.Wpf/                # Core: MVVM glue, layouts, value converters
├── Atc.Wpf.Controls/       # Atomic controls (buttons, color, inputs)
├── Atc.Wpf.Forms/          # Labelled form fields with validation
├── Atc.Wpf.Components/     # Composite components (dialogs, viewers, …)
├── Atc.Wpf.FontIcons/      # Font-icon rendering
├── Atc.Wpf.Theming/        # Light/dark theme infra + themed standard controls
├── Atc.Wpf.Network/        # Network scanner controls
└── Atc.Wpf.UndoRedo/       # Undo/redo UI
sample/Atc.Wpf.Sample/      # Control explorer
test/                       # xUnit 3 / Microsoft Testing Platform tests
```

Roadmap and ongoing improvement notes live in `plans/roadmap.md`.

## 🆘 Need help?

- 🐛 **Bug reports / feature requests** → [GitHub Issues](https://github.com/atc-net/atc-wpf/issues)
- 💬 **Questions about ATC.Net in general** → [atc-net.github.io](https://atc-net.github.io)
