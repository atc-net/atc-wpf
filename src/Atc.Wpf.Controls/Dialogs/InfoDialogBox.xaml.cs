namespace Atc.Wpf.Controls.Dialogs;

/// <summary>
/// Interaction logic for InfoDialogBox.
/// </summary>
public partial class InfoDialogBox
{
    public InfoDialogBox(
        Window owningWindow)
    {
        this.OwningWindow = owningWindow;
        this.Settings = DialogBoxSettings.Create(DialogBoxType.Ok);

        InitializeComponent();
        DataContext = this;
    }

    public InfoDialogBox(
        Window owningWindow,
        string contentText)
        : this(owningWindow)
    {
        PopulateContentControl(contentText);
    }

    public Window OwningWindow { get; private set; }

    public DialogBoxSettings Settings { get; }

    public ContentControl? HeaderControl { get; set; }

    public ContentControl ContentControl { get; set; } = new();

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