namespace Atc.Wpf.Controls.Inputs;

/// <summary>
/// RichTextBoxEx is an extension of the <see cref="RichTextBox" />.
/// </summary>
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "OK.")]
public partial class RichTextBoxEx : RichTextBox
{
    private bool preventDocumentUpdate;
    private bool preventTextUpdate;

    /// <summary>
    /// The ThemeMode property.
    /// </summary>
    [DependencyProperty(
        DefaultValue = ThemeMode.Light,
        PropertyChangedCallback = nameof(OnThemeModeChanged),
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal)]
    private ThemeMode themeMode;

    /// <summary>
    /// The text property.
    /// </summary>
    [DependencyProperty(
        DefaultValue = "",
        PropertyChangedCallback = nameof(OnTextChanged),
        CoerceValueCallback = nameof(CoerceText),
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        IsAnimationProhibited = true,
        DefaultUpdateSourceTrigger = UpdateSourceTrigger.LostFocus)]
    private string text;

    /// <summary>
    /// The text formatter property.
    /// </summary>
    [DependencyProperty(
        DefaultValue = "new RtfFormatter()",
        PropertyChangedCallback = nameof(OnTextFormatterChanged),
        Flags = FrameworkPropertyMetadataOptions.None)]
    private ITextFormatter textFormatter;

    /// <summary>
    /// Initializes a new instance of the <see cref="RichTextBoxEx" /> class.
    /// </summary>
    public RichTextBoxEx()
    {
        var contextMenu = new ContextMenu();
        var copyMenuItem = new MenuItem
        {
            Header = Miscellaneous.CopyToClipboard,
            Icon = new SvgImage
            {
                Width = 16,
                Height = 16,
                Source = "/Atc.Wpf.Controls;component/Resources/Icons/clipboard.svg",
            },
        };
        copyMenuItem.Click += OnCopyToClipboardClick;
        contextMenu.Items.Add(copyMenuItem);
        ContextMenu = contextMenu;

        var detectTheme = ThemeManager.Current.DetectTheme();
        if (detectTheme is not null &&
            Enum<ThemeMode>.TryParse(detectTheme.BaseColorScheme, ignoreCase: false, out var themeModeValue))
        {
            ThemeMode = themeModeValue;
        }

        ThemeManager.Current.ThemeChanged += OnThemeChanged;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RichTextBoxEx" /> class.
    /// </summary>
    /// <param name="document">A <see cref="FlowDocument" /> to be added as the initial contents of the new <see cref="RichTextBox" />.</param>
    public RichTextBoxEx(FlowDocument document)
        : base(document)
    {
    }

    /// <summary>
    /// Clears the content of the RichTextBox.
    /// </summary>
    public void Clear()
    {
        Document.Blocks.Clear();
    }

    /// <summary>
    /// Starts the initialization process for this element.
    /// </summary>
    public override void BeginInit()
    {
        base.BeginInit();

        // Do not update anything while initializing. See EndInit
        preventTextUpdate = true;
        preventDocumentUpdate = true;
    }

    /// <summary>
    /// Indicates that the initialization process for the element is complete.
    /// </summary>
    public override void EndInit()
    {
        base.EndInit();
        preventTextUpdate = false;
        preventDocumentUpdate = false;

        // Possible conflict here if the user specifies
        // the document AND the text at the same time
        // in XAML. Text has priority.
        if (!string.IsNullOrEmpty(Text))
        {
            UpdateDocumentFromText();
        }
        else
        {
            UpdateTextFromDocument();
        }
    }

    /// <summary>
    /// Called when [text formatter property changed].
    /// </summary>
    /// <param name="oldValue">The old value.</param>
    /// <param name="newValue">The new value.</param>
    protected virtual void OnTextFormatterPropertyChanged(
        ITextFormatter oldValue,
        ITextFormatter newValue)
    {
        UpdateTextFromDocument();
    }

    /// <summary>
    /// Is called when content in this editing control changes.
    /// </summary>
    /// <param name="e">The arguments that are associated with the <see cref="TextBoxBase.TextChanged" /> event.</param>
    protected override void OnTextChanged(TextChangedEventArgs e)
    {
        base.OnTextChanged(e);
        UpdateTextFromDocument();
    }

    private void OnCopyToClipboardClick(
        object sender,
        RoutedEventArgs e)
        => System.Windows.Clipboard.SetText(Text);

    private void OnThemeChanged(
        object? sender,
        ThemeChangedEventArgs e)
    {
        if (Enum<ThemeMode>.TryParse(e.NewTheme.BaseColorScheme, ignoreCase: false, out var themeModeValue))
        {
            ThemeMode = themeModeValue;
        }
    }

    private static void OnThemeModeChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not RichTextBoxEx richTextBoxEx)
        {
            return;
        }

        richTextBoxEx.UpdateDocumentFromText();
    }

    private static void OnTextChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not RichTextBoxEx richTextBoxEx)
        {
            return;
        }

        richTextBoxEx.UpdateDocumentFromText();
    }

    private static object CoerceText(
        DependencyObject d,
        object? value)
        => value ?? string.Empty;

    private static void OnTextFormatterChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not RichTextBoxEx richTextBoxEx)
        {
            return;
        }

        richTextBoxEx.OnTextFormatterPropertyChanged(
            (ITextFormatter)e.OldValue,
            (ITextFormatter)e.NewValue);
    }

    private void UpdateTextFromDocument()
    {
        if (preventTextUpdate)
        {
            return;
        }

        preventDocumentUpdate = true;
        SetCurrentValue(TextProperty, TextFormatter.GetText(Document));
        preventDocumentUpdate = false;
    }

    private void UpdateDocumentFromText()
    {
        if (preventDocumentUpdate)
        {
            return;
        }

        preventTextUpdate = true;
        TextFormatter.SetText(Document, Text, ThemeMode);
        preventTextUpdate = false;
    }
}