namespace Atc.Wpf.Forms.BaseControls;

public partial class FontPicker
{
    [DependencyProperty(
        DefaultValue = "Segoe UI",
        PropertyChangedCallback = nameof(OnSelectedFontFamilyChanged))]
    private FontFamily? selectedFontFamily;

    [DependencyProperty(
        DefaultValue = FontDescription.DefaultSize,
        PropertyChangedCallback = nameof(OnSelectedFontSizeChanged))]
    private double selectedFontSize;

    [DependencyProperty]
    private FontWeight selectedFontWeight;

    [DependencyProperty]
    private FontStyle selectedFontStyle;

    [DependencyProperty]
    private FontStretch selectedFontStretch;

    [DependencyProperty(DefaultValue = true)]
    private bool showFontFamily;

    [DependencyProperty(DefaultValue = true)]
    private bool showFontSize;

    [DependencyProperty(DefaultValue = true)]
    private bool showFontWeight;

    [DependencyProperty(DefaultValue = true)]
    private bool showFontStyle;

    [DependencyProperty(DefaultValue = true)]
    private bool showFontStretch;

    [DependencyProperty(DefaultValue = true)]
    private bool showForegroundColor;

    [DependencyProperty(DefaultValue = true)]
    private bool showBackgroundColor;

    [DependencyProperty(DefaultValue = true)]
    private bool showTextDecorations;

    [DependencyProperty(DefaultValue = true)]
    private bool showQuickToggles;

    [DependencyProperty(DefaultValue = true)]
    private bool showPreview;

    [DependencyProperty(DefaultValue = true)]
    private bool isFontFamilyEnabled;

    [DependencyProperty(DefaultValue = true)]
    private bool isFontSizeEnabled;

    [DependencyProperty(DefaultValue = true)]
    private bool isFontWeightEnabled;

    [DependencyProperty(DefaultValue = true)]
    private bool isFontStyleEnabled;

    [DependencyProperty(DefaultValue = true)]
    private bool isFontStretchEnabled;

    [DependencyProperty(DefaultValue = true)]
    private bool isForegroundColorEnabled;

    [DependencyProperty(DefaultValue = true)]
    private bool isBackgroundColorEnabled;

    [DependencyProperty(DefaultValue = true)]
    private bool isTextDecorationsEnabled;

    [DependencyProperty(DefaultValue = FontColorEditorMode.WellKnownColorSelector)]
    private FontColorEditorMode colorEditorMode;

    [DependencyProperty(DefaultValue = "Black")]
    private SolidColorBrush? selectedForegroundBrush;

    [DependencyProperty(DefaultValue = "Transparent")]
    private SolidColorBrush? selectedBackgroundBrush;

    [DependencyProperty]
    private TextDecorationCollection? selectedTextDecorations;

    [DependencyProperty(
        DefaultValue = "The quick brown fox jumps over the lazy dog 0123456789")]
    private string previewText;

    public static readonly DependencyProperty DisplayTextProperty = DependencyProperty.Register(
        nameof(DisplayText),
        typeof(string),
        typeof(FontPicker),
        new FrameworkPropertyMetadata(
            string.Empty,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public string? DisplayText
    {
        get => (string?)GetValue(DisplayTextProperty);
        set => SetValue(DisplayTextProperty, value);
    }

    public event EventHandler<ValueChangedEventArgs<FontDescription>>? FontChanged;

    public FontPicker()
    {
        SetCurrentValue(SelectedFontWeightProperty, FontWeights.Normal);
        SetCurrentValue(SelectedFontStyleProperty, FontStyles.Normal);
        SetCurrentValue(SelectedFontStretchProperty, FontStretches.Normal);

        InitializeComponent();

        Loaded += OnLoaded;
    }

    [SuppressMessage("Performance", "CA1024:Use properties where appropriate", Justification = "Returns a new instance per call.")]
    public FontDescription GetFontDescription()
        => new(
            SelectedFontFamily ?? new FontFamily("Segoe UI"),
            SelectedFontSize,
            SelectedFontWeight,
            SelectedFontStyle,
            SelectedFontStretch,
            SelectedForegroundBrush,
            SelectedBackgroundBrush,
            SelectedTextDecorations);

    public void SetFontDescription(FontDescription fontDescription)
    {
        ArgumentNullException.ThrowIfNull(fontDescription);

        SetCurrentValue(SelectedFontFamilyProperty, fontDescription.Family);
        SetCurrentValue(SelectedFontSizeProperty, fontDescription.Size);
        SetCurrentValue(SelectedFontWeightProperty, fontDescription.Weight);
        SetCurrentValue(SelectedFontStyleProperty, fontDescription.Style);
        SetCurrentValue(SelectedFontStretchProperty, fontDescription.Stretch);

        if (fontDescription.Foreground is not null)
        {
            SetCurrentValue(SelectedForegroundBrushProperty, fontDescription.Foreground);
        }

        if (fontDescription.Background is not null)
        {
            SetCurrentValue(SelectedBackgroundBrushProperty, fontDescription.Background);
        }

        if (fontDescription.TextDecorations is not null)
        {
            SetCurrentValue(SelectedTextDecorationsProperty, fontDescription.TextDecorations);
        }
    }

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        ThemeBrushHelper.ApplyDefaults(
            this,
            SelectedForegroundBrushProperty,
            SelectedBackgroundBrushProperty);
        UpdateDisplayText();
    }

    private static void OnSelectedFontFamilyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((FontPicker)d).UpdateDisplayText();

    private static void OnSelectedFontSizeChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((FontPicker)d).UpdateDisplayText();

    private void UpdateDisplayText()
    {
        var family = SelectedFontFamily?.Source ?? string.Empty;
        var size = SelectedFontSize.ToString("0.#", GlobalizationConstants.EnglishCultureInfo);

        var displayText = string.IsNullOrEmpty(family)
            ? string.Empty
            : $"{family}  {size}pt";

        SetCurrentValue(DisplayTextProperty, displayText);
    }

    private void OnEditClick(
        object sender,
        RoutedEventArgs e)
    {
        var ownerWindow = Window.GetWindow(this)
            ?? Application.Current?.MainWindow
            ?? throw new InvalidOperationException(
                "FontPicker could not determine an owner window for the FontPickerDialogBox.");

        var dialog = new FontPickerDialogBox(
            ownerWindow,
            GetFontDescription())
        {
            ShowFontFamily = ShowFontFamily,
            ShowFontSize = ShowFontSize,
            ShowFontWeight = ShowFontWeight,
            ShowFontStyle = ShowFontStyle,
            ShowFontStretch = ShowFontStretch,
            ShowForegroundColor = ShowForegroundColor,
            ShowBackgroundColor = ShowBackgroundColor,
            ShowTextDecorations = ShowTextDecorations,
            ShowQuickToggles = ShowQuickToggles,
            ShowPreview = ShowPreview,
            IsFontFamilyEnabled = IsFontFamilyEnabled,
            IsFontSizeEnabled = IsFontSizeEnabled,
            IsFontWeightEnabled = IsFontWeightEnabled,
            IsFontStyleEnabled = IsFontStyleEnabled,
            IsFontStretchEnabled = IsFontStretchEnabled,
            IsForegroundColorEnabled = IsForegroundColorEnabled,
            IsBackgroundColorEnabled = IsBackgroundColorEnabled,
            IsTextDecorationsEnabled = IsTextDecorationsEnabled,
            ColorEditorMode = ColorEditorMode,
            SelectedForegroundBrush = SelectedForegroundBrush,
            SelectedBackgroundBrush = SelectedBackgroundBrush,
            SelectedTextDecorations = SelectedTextDecorations,
            PreviewText = PreviewText,
        };

        var dialogResult = dialog.ShowDialog();
        if (!dialogResult.HasValue || !dialogResult.Value)
        {
            return;
        }

        var newFont = dialog.SelectedFontDescription;
        if (newFont is null)
        {
            return;
        }

        var oldFont = GetFontDescription();
        SetFontDescription(newFont);
        UpdateDisplayText();

        FontChanged?.Invoke(
            this,
            new ValueChangedEventArgs<FontDescription>(
                ControlHelper.GetIdentifier(this),
                oldValue: oldFont,
                newValue: newFont));
    }
}