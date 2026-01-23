namespace Atc.Wpf.Components.Dialogs;

public partial class InfoDialogBox
{
    public InfoDialogBox(
        Window owningWindow,
        string contentText)
        : this(
            owningWindow,
            DialogBoxSettings.Create(DialogBoxType.Ok),
            contentText)
    {
    }

    public InfoDialogBox(
        Window owningWindow,
        string titleBarText,
        string contentText)
        : this(
            owningWindow,
            DialogBoxSettings.Create(DialogBoxType.Ok),
            contentText)
        => Settings.TitleBarText = titleBarText;

    public InfoDialogBox(
        Window owningWindow,
        string titleBarText,
        string headerText,
        string contentText)
        : this(
            owningWindow,
            titleBarText,
            contentText)
        => HeaderControl = Helpers.DialogBoxHelper.CreateHeaderControl(headerText);

    public InfoDialogBox(
        Window owningWindow,
        DialogBoxSettings settings,
        string contentText)
    {
        OwningWindow = owningWindow;
        Settings = settings;
        Width = Settings.Width;
        Height = Settings.Height;

        InitializeDialogBox(contentText);
    }

    public Window OwningWindow { get; private set; }

    public DialogBoxSettings Settings { get; }

    public ContentControl? HeaderControl { get; set; }

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
}