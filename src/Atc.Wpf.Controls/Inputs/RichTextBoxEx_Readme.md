# 📝 RichTextBoxEx

An enhanced RichTextBox with bindable text, theme support, and pluggable text formatters.

## 🔍 Overview

`RichTextBoxEx` extends the WPF `RichTextBox` with a bindable `Text` property, theme-aware styling, and a pluggable `ITextFormatter` for converting between the document and string representations (RTF by default). It includes a built-in context menu with copy-to-clipboard support.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Inputs;
```

## 🚀 Usage

```xml
<inputs:RichTextBoxEx
    Text="{Binding DocumentText}"
    ThemeMode="{Binding CurrentTheme}" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Text` | `string` | `""` | Bindable text content (two-way, updates on LostFocus) |
| `ThemeMode` | `ThemeMode` | `Light` | Color theme (two-way bindable) |
| `TextFormatter` | `ITextFormatter` | `RtfFormatter` | Formatter for text/document conversion |

## 🔧 Methods

| Method | Description |
|--------|-------------|
| `Clear()` | Clear all document content |

## 📝 Notes

- `Text` updates the document on set and reads from the document on get
- The default `RtfFormatter` converts between RTF strings and `FlowDocument`
- Theme changes trigger a re-render of the document

## 🔗 Related Controls

- **LabelTextBox** - Simple labeled text input
- **TerminalViewer** - Terminal-style output viewer

## 🎮 Sample Application

See the RichTextBoxEx sample in the Atc.Wpf.Sample application under **Wpf.Controls > Inputs > RichTextBoxEx** for interactive examples.
