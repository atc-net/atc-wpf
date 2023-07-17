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
        this.Settings.FromUseGroupBox = labelControlsForm.HasMultiGroupIdentifiers();

        InitializeComponent();
        DataContext = this;

        SetContentControlSettings();
        PopulateContentControl();
    }

    public InputFormDialogBox(
        Window owningWindow,
        ILabelControlsForm labelControlsForm,
        DialogBoxSettings settings)
    {
        ArgumentNullException.ThrowIfNull(labelControlsForm);

        this.OwningWindow = owningWindow;
        this.Data = labelControlsForm;
        this.Settings = settings;

        InitializeComponent();
        DataContext = this;

        SetContentControlSettings();
        PopulateContentControl();
    }

    public Window OwningWindow { get; private set; }

    public DialogBoxSettings Settings { get; }

    public ContentControl? HeaderControl { get; set; }

    public ContentControl ContentControl { get; set; } = new();

    public ILabelControlsForm Data { get; }

    private void SetContentControlSettings()
    {
        if (Data.Rows is null)
        {
            return;
        }

        foreach (var row in Data.Rows)
        {
            if (row.Columns is null)
            {
                continue;
            }

            foreach (var column in row.Columns)
            {
                column.SetSettings(
                    Settings.FromUseGroupBox,
                    Settings.FromControlOrientation,
                    Settings.FromControlWidth);
            }
        }
    }

    private void PopulateContentControl()
    {
        Width = ContentCenter.Padding.Left +
                ContentCenter.Padding.Right +
                Data.GetMaxWidth() +
                ScrollBarSize;

        if (Width > Settings.FromMaxWidth)
        {
            Width = Settings.FromMaxWidth;
        }

        Height = HeaderControl is null
            ? Data.GetMaxHeight() +
              ScrollBarSize +
              ContentButton.Height
            : ContentTop.Height +
              Data.GetMaxHeight() +
              ScrollBarSize +
              ContentButton.Height;

        if (Height > Settings.FromMaxHeight)
        {
            Height = Settings.FromMaxHeight;
        }

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