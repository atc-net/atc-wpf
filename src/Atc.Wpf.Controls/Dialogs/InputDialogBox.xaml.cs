namespace Atc.Wpf.Controls.Dialogs;

/// <summary>
/// Interaction logic for InputDialogBox.
/// </summary>
public partial class InputDialogBox
{
    private readonly ILabelControlBase labelControl;

    public InputDialogBox(
        Window owningWindow,
        ILabelControlBase labelControl)
    {
        ArgumentNullException.ThrowIfNull(labelControl);

        this.OwningWindow = owningWindow;
        this.Settings = DialogBoxSettings.Create(DialogBoxType.OkCancel);

        this.labelControl = labelControl;

        PopulateContentControl();

        InitializeComponent();
        DataContext = this;
    }

    public Window OwningWindow { get; private set; }

    public DialogBoxSettings Settings { get; }

    public ContentControl? HeaderControl { get; set; }

    public ContentControl ContentControl { get; set; } = new();

    private void PopulateContentControl()
    {
        labelControl.Orientation = Orientation.Vertical;

        ContentControl = new ContentControl
        {
            Content = labelControl,
        };
    }

    private void OnOkClick(
        object sender,
        RoutedEventArgs e)
    {
        if (!labelControl.IsValid())
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