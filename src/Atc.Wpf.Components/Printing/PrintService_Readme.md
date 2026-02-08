# PrintService

## Overview

`IPrintService` / `PrintService` provides MVVM-friendly printing with preview support. It handles pagination of visual elements, scaling, headers/footers with token replacement, and both `Visual` and `FlowDocument` printing.

## Namespace

`Atc.Wpf.Printing` (interface, settings, result)
`Atc.Wpf.Components.Printing` (implementation)

## Usage

### Basic printing

```csharp
IPrintService printService = new PrintService();

// Print a visual element with system print dialog
var result = printService.Print(myPanel, "My Document");

// Print with custom settings
var settings = new PrintSettings
{
    Title = "Invoice",
    Orientation = PrintOrientation.Landscape,
    ScaleMode = PrintScaleMode.ShrinkToFit,
};
var result = printService.Print(myPanel, settings);
```

### Print preview

```csharp
// Show preview window, then print if user clicks Print
var result = printService.PrintWithPreview(myPanel, settings);
```

### Headers and footers

```csharp
var settings = new PrintSettings
{
    Title = "Monthly Report",
    HeaderFooter = HeaderFooterSettings.CreateDefault("Monthly Report"),
};

// Or customize individually
settings.HeaderFooter = new HeaderFooterSettings
{
    HeaderLeft = "Confidential",
    HeaderRight = "{Date}",
    FooterCenter = "Page {PageNumber} of {TotalPages}",
};
```

**Supported tokens:** `{PageNumber}`, `{TotalPages}`, `{Date}`, `{DateTime}`, `{Title}`

### FlowDocument printing

```csharp
var result = printService.PrintDocument(myFlowDocument, settings);
var result = printService.PrintDocumentWithPreview(myFlowDocument, settings);
```

### Checking results

```csharp
var result = printService.PrintWithPreview(visual);
if (result.IsSuccess)
    Log($"Printed {result.PageCount} pages");
else if (result.IsCancelled)
    Log("User cancelled");
else
    Log($"Error: {result.ErrorMessage}");
```

## Properties

### PrintSettings

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| Title | string | "Print Document" | Document title in print queue |
| Orientation | PrintOrientation | Portrait | Page orientation |
| Margins | Thickness | 48 (0.5in) | Page margins in DIPs |
| ScaleMode | PrintScaleMode | ShrinkToFit | Content scaling mode |
| HeaderFooter | HeaderFooterSettings? | null | Header/footer configuration |
| Copies | int | 1 | Number of copies |
| ShowPrintDialog | bool | true | Show system print dialog |

### PrintResult

| Property | Type | Description |
|----------|------|-------------|
| IsSuccess | bool | Print completed successfully |
| IsCancelled | bool | User cancelled the operation |
| PageCount | int | Number of pages printed |
| ErrorMessage | string? | Error details if failed |

## Notes

- All methods are synchronous (WPF printing APIs require the UI thread)
- The `PrintService` constructor accepts an optional `Func<Window?>` for owner window resolution, following the same pattern as `DialogService`
- `FlowDocument` is cloned before printing to avoid modifying the original
- Visual elements are rendered to bitmap and sliced into pages

## Related Controls

- [DialogService](../Dialogs/DialogService.cs) - Similar service pattern

## Sample Application

**Wpf.Components > Printing > PrintService**
