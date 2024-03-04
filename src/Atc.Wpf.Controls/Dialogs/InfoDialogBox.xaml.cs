namespace Atc.Wpf.Controls.Dialogs;

/// <summary>
/// Interaction logic for InfoDialogBox.
/// </summary>
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
    {
        Settings.TitleBarText = titleBarText;
    }

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

    private void InitializeDialogBox(
        string contentText)
    {
        InitializeComponent();

        DataContext = this;

        PopulateContentControl(contentText);
    }

    private void PopulateContentControl(
        string contentText)
    {
        var stackPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
        };

        if (Settings.ContentSvgImage is not null)
        {
            stackPanel.Children.Add(Settings.ContentSvgImage);
        }

        stackPanel.Children.Add(
            new TextBlock
            {
                Text = contentText,
                VerticalAlignment = VerticalAlignment.Center,
            });

        ContentControl = new ContentControl
        {
            Content = stackPanel,
        };
    }

    private void OnOkClick(
        object sender,
        RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }
}