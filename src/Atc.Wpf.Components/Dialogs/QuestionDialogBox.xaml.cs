namespace Atc.Wpf.Components.Dialogs;

public partial class QuestionDialogBox
{
    /// <summary>
    /// Identifies the <see cref="HeaderControl"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HeaderControlProperty = DependencyProperty.Register(
        nameof(HeaderControl),
        typeof(ContentControl),
        typeof(QuestionDialogBox),
        new PropertyMetadata(defaultValue: null));

    /// <summary>
    /// Identifies the <see cref="ContentControl"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ContentControlProperty = DependencyProperty.Register(
        nameof(ContentControl),
        typeof(ContentControl),
        typeof(QuestionDialogBox),
        new PropertyMetadata(defaultValue: null));

    /// <summary>
    /// Initializes a new instance of the <see cref="QuestionDialogBox"/> class with Yes/No buttons and the given content text.
    /// </summary>
    public QuestionDialogBox(
        Window owningWindow,
        string contentText)
        : this(
            owningWindow,
            DialogBoxSettings.Create(DialogBoxType.YesNo),
            contentText)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QuestionDialogBox"/> class with Yes/No buttons, a title bar text and the given content text.
    /// </summary>
    public QuestionDialogBox(
        Window owningWindow,
        string titleBarText,
        string contentText)
        : this(
            owningWindow,
            DialogBoxSettings.Create(DialogBoxType.YesNo),
            contentText)
    {
        Settings.TitleBarText = titleBarText;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QuestionDialogBox"/> class with Yes/No buttons, a title bar text, a header text and the given content text.
    /// </summary>
    public QuestionDialogBox(
        Window owningWindow,
        string titleBarText,
        string headerText,
        string contentText)
        : this(
            owningWindow,
            titleBarText,
            contentText)
        => HeaderControl = Helpers.DialogBoxHelper.CreateHeaderControl(headerText);

    /// <summary>
    /// Initializes a new instance of the <see cref="QuestionDialogBox"/> class with the given dialog settings and content text.
    /// </summary>
    public QuestionDialogBox(
        Window owningWindow,
        DialogBoxSettings settings,
        string contentText)
    {
        OwningWindow = owningWindow;
        if (owningWindow is not null)
        {
            FlowDirection = owningWindow.FlowDirection;
        }

        Settings = settings;
        Width = Settings.Width;
        Height = Settings.Height;

        InitializeDialogBox(contentText);

        if (Settings.AutoHeight)
        {
            Helpers.DialogBoxHelper.ApplyAutoHeight(this, owningWindow, Settings.Height);
        }

        Helpers.DialogBoxHelper.ApplyOwner(this, owningWindow);
    }

    /// <summary>
    /// Gets the window that owns the dialog box.
    /// </summary>
    public Window OwningWindow { get; private set; }

    /// <summary>
    /// Gets the settings that control the dialog box appearance and buttons.
    /// </summary>
    public DialogBoxSettings Settings { get; }

    /// <summary>
    /// Gets or sets the optional header control shown above the content.
    /// </summary>
    public ContentControl? HeaderControl
    {
        get => (ContentControl?)GetValue(HeaderControlProperty);
        set => SetValue(HeaderControlProperty, value);
    }

    /// <summary>
    /// Gets or sets the control that displays the question content.
    /// </summary>
    public ContentControl ContentControl
    {
        get => (ContentControl)GetValue(ContentControlProperty);
        set => SetValue(ContentControlProperty, value);
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(
        DependencyPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        Helpers.DialogBoxHelper.OnDialogPropertyChanged(this, e);
    }

    private void InitializeDialogBox(string contentText)
    {
        InitializeComponent();

        DataContext = this;

        PopulateContentControl(contentText);
    }

    private void PopulateContentControl(string contentText)
        => ContentControl = Helpers.DialogBoxHelper.CreateContentControl(contentText, Settings.ContentSvgImage);

    private void OnOkClick(
        object sender,
        RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void OnOkCancel(
        object sender,
        RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}