// ReSharper disable ParameterTypeCanBeEnumerable.Local
namespace Atc.Wpf.Controls.Dialogs;

/// <summary>
/// Interaction logic for InputFormDialogBox.
/// </summary>
public partial class InputFormDialogBox
{
    private const int ScrollBarSize = 20;

    public InputFormDialogBox(
        Window owningWindow,
        ILabelControlsForm labelControlsForm)
    {
        ArgumentNullException.ThrowIfNull(labelControlsForm);

        this.OwningWindow = owningWindow;
        this.Data = labelControlsForm;
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

    public ILabelControlsForm Data { get; }

    private void SetContentControlSettings()
    {
        if (Data.Columns is null)
        {
            return;
        }

        foreach (var column in this.Data.Columns)
        {
            column.ControlOrientation = Settings.FromControlOrientation;
            column.ControlWidth = Settings.FromControlWidth;
        }
    }

    private void PopulateContentControl()
    {
        Height = HeaderControl is null
            ? Data.GetMaxHeight() +
              ScrollBarSize +
              ContentButton.Height
            : ContentTop.Height +
              Data.GetMaxHeight() +
              ScrollBarSize +
              ContentButton.Height;

        Width = ContentCenter.Padding.Left +
                ContentCenter.Padding.Right +
                Data.GetMaxWidth() +
                ScrollBarSize;

        ContentControl = new ContentControl
        {
            Content = Data.GeneratePanel(),
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