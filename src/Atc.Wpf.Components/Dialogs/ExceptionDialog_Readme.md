# 🧯 ExceptionDialog

Shows an exception in the themed error dialog, with a report that can be copied for a bug report or an AI agent.

## 🔍 Overview

`ExceptionDialog.Show(exception, title)` shows the exception messages (the exception and its inner exceptions) in an error `InfoDialogBox`. The full report sits in the collapsed **Details** area: the title, time, app, OS and .NET versions and `exception.ToString()` with every stack trace. **Ctrl+C** or the **copy button** copies the dialog text and the report. The report is also saved to `%LOCALAPPDATA%\<app>\Crashes\crash-<time>.txt` first.

It can be called from any thread. When the themed dialog cannot be shown (no application, the dispatcher is shutting down, or the dialog itself fails), it falls back to `ExceptionReport.ShowMessageBox` in Atc.Wpf: the native Windows message box, with the report saved and copied to the clipboard.

What the app does next (close, go on, restart) and logging stay with the caller.

## 📍 Namespace

```csharp
using Atc.Wpf.Components.Dialogs; // ExceptionDialog
using Atc.Wpf.Dialogs;            // ExceptionReport (no dependencies beyond .NET and WPF)
```

## 🚀 Usage

```csharp
protected override void OnStartup(StartupEventArgs e)
{
    AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
    Current.DispatcherUnhandledException += ApplicationOnDispatcherUnhandledException;
    base.OnStartup(e);
}

private static void CurrentDomainUnhandledException(
    object sender,
    UnhandledExceptionEventArgs e)
{
    if (e is not { ExceptionObject: Exception ex })
    {
        return;
    }

    // The process ends; the UI thread may be gone, so use the native message box.
    _ = ExceptionReport.ShowMessageBox(ex, "CurrentDomain Unhandled Exception");
}

private void ApplicationOnDispatcherUnhandledException(
    object sender,
    DispatcherUnhandledExceptionEventArgs e)
{
    if (ExceptionReport.IsKnownHarmlessWpfBindingError(e.Exception))
    {
        e.Handled = true;
        return;
    }

    logger.LogError(e.Exception, "Dispatcher Unhandled Exception");
    ExceptionDialog.Show(e.Exception, "Dispatcher Unhandled Exception");

    e.Handled = true;
    Shutdown(-1);
}
```

## ⚙️ ExceptionReport (Atc.Wpf)

| Member | Description |
|--------|-------------|
| `CreateText(exception, title)` | The report text: title, time, app, OS and .NET versions, `exception.ToString()` |
| `TryWriteToFile(text, folder?)` | Saves the report; returns the file path or `null` |
| `TryCopyToClipboard(text)` | Copies from any thread (own STA thread, 2 s limit); returns `true` when copied |
| `ShowMessageBox(exception, title)` | Saves, copies and shows the native message box; returns the file path |
| `GetDefaultFolder()` | `%LOCALAPPDATA%\<app>\Crashes` |
| `IsKnownHarmlessWpfBindingError(exception)` | The `ComboBoxItem` `HorizontalContentAlignment` binding error that apps ignore |

None of these throw.

## 📝 Notes

- `DialogBoxSettings.ShowCopyButton` adds the same copy button to any info or question dialog
- The message box texts of the last-resort fallback are in English, so they never depend on resources

## 🔗 Related Controls

- **InfoDialogBox** - The dialog it uses (details area, Ctrl+C, copy button)
- **DialogService** - Dialogs from view models

## 🎮 Sample Application

**Wpf.Components > Dialogs > Long text DialogBox**, button **Exception dialog**. The sample app's `App.xaml.cs` uses both handlers above.