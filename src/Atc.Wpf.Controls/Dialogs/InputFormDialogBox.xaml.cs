// ReSharper disable ParameterTypeCanBeEnumerable.Local
namespace Atc.Wpf.Controls.Dialogs;

/// <summary>
/// Interaction logic for InputFormDialogBox.
/// </summary>
public partial class InputFormDialogBox
{
    private const int ScrollBarSize = 20;
    private readonly ILabelControlsForm labelControlsForm;

    public InputFormDialogBox(
        Window owningWindow,
        ILabelControlsForm labelControlsForm)
    {
        ArgumentNullException.ThrowIfNull(labelControlsForm);

        this.OwningWindow = owningWindow;
        this.labelControlsForm = labelControlsForm;
        this.Settings = DialogBoxSettings.Create(DialogBoxType.OkCancel);

        SetContentControlSettings();

        InitializeComponent();
        DataContext = this;

        PopulateContentControl();
    }

    public Window OwningWindow { get; private set; }

    public DialogBoxSettings Settings { get; }

    public ContentControl? HeaderControl { get; set; }

    public ContentControl ContentControl { get; set; } = new();

    private void SetContentControlSettings()
    {
        if (labelControlsForm.Columns is null)
        {
            return;
        }

        foreach (var column in this.labelControlsForm.Columns)
        {
            column.ControlOrientation = Settings.FromControlOrientation;
            column.ControlWidth = Settings.FromControlWidth;
        }
    }

    private void PopulateContentControl()
    {
        Height = HeaderControl is null
            ? labelControlsForm.GetMaxHeight() +
              ScrollBarSize +
              ContentButton.Height
            : ContentTop.Height +
              labelControlsForm.GetMaxHeight() +
              ScrollBarSize +
              ContentButton.Height;

        Width = ContentCenter.Padding.Left +
                ContentCenter.Padding.Right +
                labelControlsForm.GetMaxWidth() +
                ScrollBarSize;

        ContentControl = new ContentControl
        {
            Content = labelControlsForm.GeneratePanel(),
        };
    }

    private void OnOkClick(
        object sender,
        RoutedEventArgs e)
    {
        if (!labelControlsForm.IsValid())
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