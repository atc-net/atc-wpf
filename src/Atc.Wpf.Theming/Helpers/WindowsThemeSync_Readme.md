# 🪟 WindowsThemeSync

Makes the application theme follow the Windows light/dark app mode and accent color.

## 🔍 Overview

`WindowsThemeSync` is a static helper on top of the ControlzEx `ThemeManager`. Turn on one or both parts of `Mode` and the current Windows setting is applied at once. While a part is on, the theme changes again whenever the user changes it in **Windows Settings > Personalization > Colors**.

Following the accent color creates a runtime theme from the Windows accent (for example `Dark.Runtime_#FF0078D4`), using the same color generator as the built-in themes.

## 📍 Namespace

```csharp
using Atc.Wpf.Theming;          // WindowsThemeSyncMode
using Atc.Wpf.Theming.Helpers;  // WindowsThemeSync
```

## 🚀 Usage

### Follow Windows at startup

```csharp
protected override void OnStartup(StartupEventArgs e)
{
    base.OnStartup(e);

    WindowsThemeSync.Mode = WindowsThemeSyncMode.AppModeAndAccent;
}
```

### Follow only light/dark mode

```csharp
WindowsThemeSync.Mode = WindowsThemeSyncMode.AppMode;
```

### Stop following

```csharp
WindowsThemeSync.Mode = WindowsThemeSyncMode.None;

// or stop one part and keep the other
WindowsThemeSync.Stop(WindowsThemeSyncMode.Accent);
```

### React to changes

```csharp
WindowsThemeSync.ModeChanged += (_, _) => UpdateSettingsUi(WindowsThemeSync.Mode);
```

## ⚙️ Members

| Member | Type | Description |
|--------|------|-------------|
| `Mode` | `WindowsThemeSyncMode` | Which Windows settings the theme follows. Turning a part on applies the current Windows setting immediately |
| `ModeChanged` | `event EventHandler` | Raised when `Mode` changes, including when a selector stops a part |
| `Stop(mode)` | method | Stops following the given part and keeps the rest |
| `IsWindowsAppModeLight` | `bool` | Whether Windows is set to light app mode |
| `WindowsAccentColor` | `Color?` | The Windows accent color, or `null` when it cannot be read |

### WindowsThemeSyncMode

| Value | Description |
|-------|-------------|
| `None` | The theme does not follow Windows (default) |
| `AppMode` | Follow the Windows light/dark app mode |
| `Accent` | Follow the Windows accent color |
| `AppModeAndAccent` | Follow both |

## 📝 Notes

- Picking a theme in `ThemeSelector` stops following the app mode, and picking an accent in `AccentColorSelector` stops following the accent color. Without this, the next Windows change would override the user's choice
- `ThemeSelector` and `AccentColorSelector` show theme changes made by the sync. A runtime accent theme is not in the accent list, so the accent selector shows no selection while the accent is followed
- Stopping a part leaves the current theme as it is; only turning a part on applies the Windows setting
- The sync applies to `Application.Current`; without an application only `Mode` is stored
- Windows high-contrast mode is not followed yet, because the library has no high-contrast themes
- The mode is not saved: set it again at startup, for example from your own settings

## 🔗 Related Controls

- **ThemeSelector** - Selects the base theme (Light / Dark)
- **AccentColorSelector** - Selects the accent color scheme
- **LabelThemeAndAccentColorSelectors** - Theme and accent selectors side by side

## 🎮 Sample Application

See **Wpf.Theming > Theme > Windows theme sync** in the sample application.