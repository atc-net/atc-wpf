namespace Atc.Wpf.Controls.BaseControls;

/// <summary>
/// RichTextBoxEx is an extension of the <see cref="RichTextBox" />.
/// </summary>
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "OK.")]
public class RichTextBoxEx : RichTextBox
{
    /// <summary>
    /// The ThemeMode property.
    /// </summary>
    public static readonly DependencyProperty ThemeModeProperty = DependencyProperty.Register(
        nameof(ThemeMode),
        typeof(ThemeMode),
        typeof(RichTextBoxEx),
        new FrameworkPropertyMetadata(
            ThemeMode.Light,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
            OnThemeModeChanged));

    /// <summary>
    /// The text property.
    /// </summary>
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        nameof(Text),
        typeof(string),
        typeof(RichTextBoxEx),
        new FrameworkPropertyMetadata(
            string.Empty,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            OnTextChanged,
            CoerceText,
            isAnimationProhibited: true,
            UpdateSourceTrigger.LostFocus));

    /// <summary>
    /// The text formatter property.
    /// </summary>
    public static readonly DependencyProperty TextFormatterProperty = DependencyProperty.Register(
        nameof(TextFormatter),
        typeof(ITextFormatter),
        typeof(RichTextBox),
        new FrameworkPropertyMetadata(
            new RtfFormatter(),
            OnTextFormatterChanged));

    private bool preventDocumentUpdate;
    private bool preventTextUpdate;

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
    /// Gets or sets the ThemeMode.
    /// </summary>
    /// <value>
    /// The text.
    /// </value>
    public ThemeMode ThemeMode
    {
        get => (ThemeMode)GetValue(ThemeModeProperty);
        set => SetValue(ThemeModeProperty, value);
    }

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    /// <value>
    /// The text.
    /// </value>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// Gets or sets the text formatter.
    /// </summary>
    /// <value>
    /// The text formatter.
    /// </value>
    public ITextFormatter TextFormatter
    {
        get => (ITextFormatter)GetValue(TextFormatterProperty);
        set => SetValue(TextFormatterProperty, value);
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
    protected override void OnTextChanged(
        TextChangedEventArgs e)
    {
        base.OnTextChanged(e);
        UpdateTextFromDocument();
    }

    private void OnCopyToClipboardClick(
        object sender,
        RoutedEventArgs e)
        => Clipboard.SetText(Text);

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
    {
        return value ?? string.Empty;
    }

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