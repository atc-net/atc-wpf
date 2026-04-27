namespace Atc.Wpf.Sample.SamplesWpfControls.FontEditing;

public partial class AdvancedFontPickerDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Font Family", "Show", 1)]
    [ObservableProperty]
    private bool showFontFamily = true;

    [PropertyDisplay("Font Size", "Show", 2)]
    [ObservableProperty]
    private bool showFontSize = true;

    [PropertyDisplay("Font Weight", "Show", 3)]
    [ObservableProperty]
    private bool showFontWeight = true;

    [PropertyDisplay("Font Style", "Show", 4)]
    [ObservableProperty]
    private bool showFontStyle = true;

    [PropertyDisplay("Font Stretch", "Show", 5)]
    [ObservableProperty]
    private bool showFontStretch = true;

    [PropertyDisplay("Foreground", "Show", 6)]
    [ObservableProperty]
    private bool showForegroundColor = true;

    [PropertyDisplay("Background", "Show", 7)]
    [ObservableProperty]
    private bool showBackgroundColor = true;

    [PropertyDisplay("Text Decorations", "Show", 8)]
    [ObservableProperty]
    private bool showTextDecorations = true;

    [PropertyDisplay("Quick Toggles (B/I/U/S)", "Show", 9)]
    [ObservableProperty]
    private bool showQuickToggles = true;

    [PropertyDisplay("Preview", "Show", 10)]
    [ObservableProperty]
    private bool showPreview = true;

    [PropertyDisplay("Font Family", "Enable", 1)]
    [ObservableProperty]
    private bool isFontFamilyEnabled = true;

    [PropertyDisplay("Font Size", "Enable", 2)]
    [ObservableProperty]
    private bool isFontSizeEnabled = true;

    [PropertyDisplay("Font Weight", "Enable", 3)]
    [ObservableProperty]
    private bool isFontWeightEnabled = true;

    [PropertyDisplay("Font Style", "Enable", 4)]
    [ObservableProperty]
    private bool isFontStyleEnabled = true;

    [PropertyDisplay("Font Stretch", "Enable", 5)]
    [ObservableProperty]
    private bool isFontStretchEnabled = true;

    [PropertyDisplay("Foreground", "Enable", 6)]
    [ObservableProperty]
    private bool isForegroundColorEnabled = true;

    [PropertyDisplay("Background", "Enable", 7)]
    [ObservableProperty]
    private bool isBackgroundColorEnabled = true;

    [PropertyDisplay("Text Decorations", "Enable", 8)]
    [ObservableProperty]
    private bool isTextDecorationsEnabled = true;

    [PropertyDisplay("Color Editor", "Other", 1)]
    [ObservableProperty]
    private FontColorEditorMode colorEditorMode = FontColorEditorMode.ColorPicker;

    [PropertyDisplay("Preview Text", "Other", 2)]
    [ObservableProperty]
    private string previewText = "The quick brown fox jumps over the lazy dog 0123456789";
}