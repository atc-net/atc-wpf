# 📋 ClipboardService

An MVVM-friendly clipboard service providing text, image, and file operations with history tracking and real-time change notifications.

## 🔍 Overview

`ClipboardService` implements `IClipboardService` to provide a testable abstraction over `System.Windows.Clipboard`. It wraps all clipboard operations with error handling for locked-clipboard scenarios, maintains a bounded history of recent clipboard entries, and can monitor system-wide clipboard changes via a Win32 clipboard format listener.

## 📍 Namespace

```csharp
using Atc.Wpf.Clipboard;
```

## 🚀 Usage

### Basic Text Operations

```csharp
IClipboardService clipboard = new ClipboardService();

// Copy text
clipboard.SetText("Hello, World!");

// Paste text
if (clipboard.ContainsText())
{
    string? text = clipboard.GetText();
}

// Clear clipboard
clipboard.Clear();
```

### Image Operations

```csharp
// Copy image
var bitmap = new BitmapImage(new Uri("pack://application:,,,/image.png"));
clipboard.SetImage(bitmap);

// Paste image
if (clipboard.ContainsImage())
{
    BitmapSource? image = clipboard.GetImage();
}
```

### File Drop List

```csharp
var files = new StringCollection { @"C:\file1.txt", @"C:\file2.txt" };
clipboard.SetFileDropList(files);

if (clipboard.ContainsFileDropList())
{
    StringCollection? droppedFiles = clipboard.GetFileDropList();
}
```

### Clipboard Monitoring

```csharp
clipboard.ClipboardChanged += (sender, e) =>
{
    Console.WriteLine($"Clipboard changed: {e.Entry.DataType} - {e.Entry.Summary}");
};

clipboard.StartMonitoring();
// ... clipboard changes are now reported via the event ...
clipboard.StopMonitoring();
```

### History

```csharp
clipboard.MaxHistorySize = 50; // Default is 25

// After performing clipboard operations...
IReadOnlyList<ClipboardEntry> entries = clipboard.History;
foreach (var entry in entries)
{
    Console.WriteLine($"{entry.Timestamp:HH:mm:ss} [{entry.DataType}] {entry.Summary}");
}

clipboard.ClearHistory();
```

## ⚙️ IClipboardService Members

| Member | Type | Description |
|--------|------|-------------|
| `SetText` | `void` | Copies text to the clipboard |
| `GetText` | `string?` | Gets text from the clipboard |
| `ContainsText` | `bool` | Checks if clipboard contains text |
| `SetImage` | `void` | Copies a BitmapSource to the clipboard |
| `GetImage` | `BitmapSource?` | Gets an image from the clipboard |
| `ContainsImage` | `bool` | Checks if clipboard contains an image |
| `SetFileDropList` | `void` | Copies a file list to the clipboard |
| `GetFileDropList` | `StringCollection?` | Gets a file list from the clipboard |
| `ContainsFileDropList` | `bool` | Checks if clipboard contains files |
| `SetData` | `void` | Copies data in a custom format |
| `GetData` | `object?` | Gets data in a custom format |
| `ContainsData` | `bool` | Checks for a custom format |
| `Clear` | `void` | Clears the clipboard |
| `History` | `IReadOnlyList<ClipboardEntry>` | Recent clipboard entries (newest first) |
| `MaxHistorySize` | `int` | Maximum history entries (default 25) |
| `ClearHistory` | `void` | Removes all history entries |
| `StartMonitoring` | `void` | Begins listening for clipboard changes |
| `StopMonitoring` | `void` | Stops the clipboard listener |
| `IsMonitoring` | `bool` | Whether the listener is active |
| `ClipboardChanged` | `event` | Raised when clipboard content changes |

## 📋 ClipboardEntry Properties

| Property | Type | Description |
|----------|------|-------------|
| `DataType` | `ClipboardDataType` | Text, Image, FileDropList, or Other |
| `Data` | `object?` | The actual clipboard data |
| `Timestamp` | `DateTime` | When the entry was captured |
| `Summary` | `string` | Human-readable summary (e.g., truncated text, "Image 800x600", "3 files") |

## 📝 Notes

- All `System.Windows.Clipboard` calls are wrapped in try/catch for `ExternalException` to handle cases where the clipboard is locked by another process
- The clipboard listener uses `AddClipboardFormatListener` (Win32) with a hidden message-only window
- History entries from monitoring are added automatically; entries from `SetText`/`SetImage`/`SetFileDropList` are also tracked
- Call `Dispose()` to stop monitoring and release the native listener window

## 🔗 Related

- **ClipboardDataType** — Enum for entry data types
- **ClipboardEntry** — Individual history entry
- **ClipboardChangedEventArgs** — Event data for change notifications

## 🎮 Sample Application

See the ClipboardService sample in the Atc.Wpf.Sample application under **Wpf > Clipboard > ClipboardService** for interactive examples.
