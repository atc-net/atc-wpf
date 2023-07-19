namespace Atc.Wpf.Controls.Dialogs;

/// <summary>
/// Interaction logic for InputDialogBox.
/// </summary>
public partial class InputDialogBox
{
    public InputDialogBox(
        Window owningWindow,
        DialogBoxSettings settings,
        ILabelControlBase labelControl)
    {
        this.OwningWindow = owningWindow;
        this.Settings = settings;

        this.Data = labelControl;

        InitializeComponent();
        DataContext = this;

        PopulateContentControl();
    }

    public InputDialogBox(
        Window owningWindow,
        ILabelControlBase labelControl)
        : this(
            owningWindow,
            DialogBoxSettings.Create(DialogBoxType.OkCancel),
            labelControl)
    {
    }

    public Window OwningWindow { get; private set; }

    public DialogBoxSettings Settings { get; }

    public ContentControl? HeaderControl { get; set; }

    public ContentControl ContentControl { get; set; } = new();

    public ILabelControlBase Data { get; }

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