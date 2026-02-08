# 🖥️ TerminalViewer

A terminal-style output display with color-coded text based on keyword matching.

## 🔍 Overview

`TerminalViewer` displays streaming text output with automatic color coding. It matches configurable keyword lists against each line and applies corresponding colors (error, success, and three custom term sets). The viewer processes data asynchronously via channels and supports copy-to-clipboard and clear-screen commands.

## 📍 Namespace

```csharp
using Atc.Wpf.Components.Viewers;
```

## 🚀 Usage

### Basic Usage

```xml
<viewers:TerminalViewer
    TerminalBackground="Black"
    TerminalFontFamily="Consolas"
    TerminalFontSize="12"
    DefaultTextColor="Teal" />
```

### With Custom Term Colors

```xml
<viewers:TerminalViewer
    DefaultTextColor="Teal"
    ErrorTextColor="Red"
    SuccessfulTextColor="LimeGreen"
    Terms1TextColor="Chocolate"
    Terms2TextColor="DarkOrange"
    Terms3TextColor="CornflowerBlue">
</viewers:TerminalViewer>
```

### Setting Error/Success Terms

```csharp
terminalViewer.TermsError = new List<string> { "ERROR", "FAIL", "EXCEPTION" };
terminalViewer.TermsSuccessful = new List<string> { "OK", "SUCCESS", "PASSED" };
terminalViewer.Terms1 = new List<string> { "WARNING", "WARN" };
terminalViewer.Terms2 = new List<string> { "INFO", "DEBUG" };
terminalViewer.Terms3 = new List<string> { "TRACE", "VERBOSE" };
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `TerminalBackground` | `Brush` | Black | Terminal background color |
| `TerminalFontFamily` | `FontFamily` | Consolas | Font family for terminal text |
| `TerminalFontSize` | `double` | `12.0` | Font size |
| `DefaultTextColor` | `Brush` | Teal | Default text color |
| `ErrorTextColor` | `Brush` | Red | Color for lines matching error terms |
| `TermsError` | `IList<string>?` | `null` | Keywords that trigger error coloring |
| `SuccessfulTextColor` | `Brush` | LimeGreen | Color for lines matching success terms |
| `TermsSuccessful` | `IList<string>?` | `null` | Keywords that trigger success coloring |
| `Terms1TextColor` | `Brush` | Chocolate | Color for terms1 matches |
| `Terms1` | `IList<string>?` | `null` | First custom keyword list |
| `Terms2TextColor` | `Brush` | DarkOrange | Color for terms2 matches |
| `Terms2` | `IList<string>?` | `null` | Second custom keyword list |
| `Terms3TextColor` | `Brush` | CornflowerBlue | Color for terms3 matches |
| `Terms3` | `IList<string>?` | `null` | Third custom keyword list |

## 🔧 Commands

| Command | Description |
|---------|-------------|
| `CopyToClipboardCommand` | Copy all terminal content to clipboard |
| `ClearScreenCommand` | Clear all terminal output |

## 📝 Notes

- Data arrives via messenger events (`TerminalReceivedDataEventArgs`)
- Processing uses `System.Threading.Channels` for async queuing
- Term matching is word-boundary-aware (matches whole words, not substrings)
- The control implements `IDisposable` — dispose when no longer needed
- Priority order for color matching: Error > Success > Terms1 > Terms2 > Terms3 > Default

## 🔗 Related Controls

- **JsonViewer** - Structured data display with tree view
- **ApplicationMonitorView** - Application monitoring display

## 🎮 Sample Application

See the TerminalViewer sample in the Atc.Wpf.Sample application under **Wpf.Components > Viewers > TerminalViewer** for interactive examples.
