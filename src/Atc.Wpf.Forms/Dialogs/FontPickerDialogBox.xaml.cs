namespace Atc.Wpf.Forms.Dialogs;

public partial class FontPickerDialogBox
{
    [DependencyProperty]
    private FontDescription? selectedFontDescription;

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

    [DependencyProperty(DefaultValue = "Black")]
    private SolidColorBrush? selectedForegroundBrush;

    [DependencyProperty(DefaultValue = "Transparent")]
    private SolidColorBrush? selectedBackgroundBrush;

    [DependencyProperty]
    private TextDecorationCollection? selectedTextDecorations;

    [DependencyProperty(
        DefaultValue = "The quick brown fox jumps over the lazy dog 0123456789")]
    private string previewText;

    public FontPickerDialogBox(
        Window owningWindow,
        FontDescription fontDescription)
        : this(
            owningWindow,
            CreateDefaultSettings(),
            fontDescription)
    {
    }

    public FontPickerDialogBox(
        Window owningWindow,
        DialogBoxSettings settings,
        FontDescription fontDescription)
    {
        ArgumentNullException.ThrowIfNull(fontDescription);

        OwningWindow = owningWindow;
        Settings = settings;
        SelectedFontDescription = fontDescription;
        Width = Settings.Width;
        Height = Settings.Height;

        InitializeDialogBox();
    }

    public Window OwningWindow { get; private set; }

    public DialogBoxSettings Settings { get; }

    public ContentControl? HeaderControl { get; set; }

    private void InitializeDialogBox()
    {
        InitializeComponent();

        DataContext = this;

        if (SelectedFontDescription is not null)
        {
            UcAdvancedFontPicker.SetFontDescription(SelectedFontDescription);
        }

        ContentRendered += (_, _) => UcAdvancedFontPicker.Focus();
    }

    private void OnOkClick(
        object sender,
        RoutedEventArgs e)
    {
        SelectedFontDescription = UcAdvancedFontPicker.GetFontDescription();
        UcAdvancedFontPicker.RecordCurrentFamilyAsRecent();

        DialogResult = true;
        Close();
    }

    private void OnCancelClick(
        object sender,
        RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private static DialogBoxSettings CreateDefaultSettings()
    {
        var settings = DialogBoxSettings.Create(DialogBoxType.OkCancel);
        settings.Width = 925;
        settings.Height = 720;
        settings.TitleBarText = Miscellaneous.FontPicker;
        return settings;
    }
}