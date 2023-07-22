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
        : this(
            owningWindow,
            DialogBoxSettings.Create(DialogBoxType.OkCancel),
            labelControlsForm)
    {
    }

    public InputFormDialogBox(
        Window owningWindow,
        string titleBarText,
        ILabelControlsForm labelControlsForm)
        : this(
            owningWindow,
            DialogBoxSettings.Create(DialogBoxType.OkCancel),
            labelControlsForm)
    {
        this.Settings.TitleBarText = titleBarText;
    }

    public InputFormDialogBox(
        Window owningWindow,
        DialogBoxSettings settings,
        ILabelControlsForm labelControlsForm)
    {
        ArgumentNullException.ThrowIfNull(labelControlsForm);

        this.OwningWindow = owningWindow;
        this.Settings = settings;
        this.Settings.Form.UseGroupBox = labelControlsForm.HasMultiGroupIdentifiers();

        InitializeDialogBox(labelControlsForm);
    }

    public Window OwningWindow { get; private set; }

    public DialogBoxSettings Settings { get; }

    public ContentControl? HeaderControl { get; set; }

    public LabelInputFormPanel LabelInputFormPanel { get; } = new();

    public ILabelControlsForm Data => LabelInputFormPanel.Data;

    private void InitializeDialogBox(
        ILabelControlsForm labelControlsForm)
    {
        InitializeComponent();

        DataContext = this;

        PopulateLabelInputFormPanel(labelControlsForm);
    }

    private void PopulateLabelInputFormPanel(
        ILabelControlsForm labelControlsForm)
    {
        LabelInputFormPanel.Render(
            this.Settings.Form,
            labelControlsForm);

        Width = ContentCenter.Padding.Left +
                ContentCenter.Padding.Right +
                Data.GetMaxWidth() +
                ScrollBarSize;

        if (Width > Settings.Form.MaxSize.Width)
        {
            Width = Settings.Form.MaxSize.Width;
        }

        Height = HeaderControl is null
            ? Data.GetMaxHeight() +
              ScrollBarSize +
              ContentButton.Height
            : ContentTop.Height +
              Data.GetMaxHeight() +
              ScrollBarSize +
              ContentButton.Height;

        if (Height > Settings.Form.MaxSize.Height)
        {
            Height = Settings.Form.MaxSize.Height;
        }
    }

    private void OnOkClick(
        object sender,
        RoutedEventArgs e)
    {
        if (!LabelInputFormPanel.Data.IsValid())
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