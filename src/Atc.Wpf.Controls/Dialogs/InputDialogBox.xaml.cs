namespace Atc.Wpf.Controls.Dialogs;

public partial class InputDialogBox
{
    public InputDialogBox(
        Window owningWindow,
        ILabelControlBase labelControl)
        : this(
            owningWindow,
            DialogBoxSettings.Create(DialogBoxType.OkCancel),
            labelControl)
    {
    }

    public InputDialogBox(
        Window owningWindow,
        string titleBarText,
        ILabelControlBase labelControl)
        : this(
            owningWindow,
            DialogBoxSettings.Create(DialogBoxType.OkCancel),
            labelControl)
    {
        Settings.TitleBarText = titleBarText;
    }

    public InputDialogBox(
        Window owningWindow,
        string titleBarText,
        string headerText,
        ILabelControlBase labelControl)
        : this(
            owningWindow,
            titleBarText,
            labelControl)
    {
        HeaderControl = new ContentControl
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Content = new TextBlock
            {
                Text = headerText,
                FontSize = 24,
            },
        };
    }

    public InputDialogBox(
        Window owningWindow,
        DialogBoxSettings settings,
        ILabelControlBase labelControl)
    {
        OwningWindow = owningWindow;
        Settings = settings;
        Width = Settings.Width;
        Height = Settings.Height;

        Data = labelControl;

        InitializeDialogBox();
    }

    public Window OwningWindow { get; private set; }

    public DialogBoxSettings Settings { get; }

    public ContentControl? HeaderControl { get; set; }

    public ContentControl ContentControl { get; set; } = new();

    public ILabelControlBase Data { get; }

    private void InitializeDialogBox()
    {
        InitializeComponent();

        DataContext = this;

        PopulateContentControl();
    }

    private void PopulateContentControl()
    {
        Data.Orientation = Orientation.Vertical;

        ContentControl = new ContentControl
        {
            Content = Data,
        };
    }

    private void OnOkClick(
        object sender,
        RoutedEventArgs e)
    {
        if (!Data.IsValid())
        {
            return;
        }

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