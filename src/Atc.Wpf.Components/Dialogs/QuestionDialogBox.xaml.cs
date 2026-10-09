namespace Atc.Wpf.Components.Dialogs;

public partial class QuestionDialogBox
{
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
    public ContentControl? HeaderControl { get; set; }

    /// <summary>
    /// Gets or sets the control that displays the question content.
    /// </summary>
    public ContentControl ContentControl { get; set; } = new();

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