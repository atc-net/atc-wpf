// ReSharper disable ParameterTypeCanBeEnumerable.Local
namespace Atc.Wpf.Components.Dialogs;

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
        => Settings.TitleBarText = titleBarText;

    public InputFormDialogBox(
        Window owningWindow,
        string titleBarText,
        string headerText,
        ILabelControlsForm labelControlsForm)
        : this(
            owningWindow,
            titleBarText,
            labelControlsForm)
    {
        HeaderControl = Helpers.DialogBoxHelper.CreateHeaderControl(headerText);

        UpdateWidthAndHeight();
    }

    public InputFormDialogBox(
        Window owningWindow,
        LabelInputFormPanelSettings formPanelSettings,
        ILabelControlsForm labelControlsForm)
    {
        ArgumentNullException.ThrowIfNull(labelControlsForm);

        OwningWindow = owningWindow;
        Settings = DialogBoxSettings.Create(DialogBoxType.OkCancel);
        Width = Settings.Width;
        Height = Settings.Height;
        Settings.Form = formPanelSettings;

        InitializeDialogBox(labelControlsForm);
    }

    public InputFormDialogBox(
        Window owningWindow,
        DialogBoxSettings settings,
        ILabelControlsForm labelControlsForm)
    {
        ArgumentNullException.ThrowIfNull(labelControlsForm);

        OwningWindow = owningWindow;
        Settings = settings;
        Width = Settings.Width;
        Height = Settings.Height;
        Settings.Form.UseGroupBox = labelControlsForm.HasMultiGroupIdentifiers();

        InitializeDialogBox(labelControlsForm);
    }

    public Window OwningWindow { get; private set; }

    public DialogBoxSettings Settings { get; }

    public ContentControl? HeaderControl { get; set; }

    public LabelInputFormPanel LabelInputFormPanel { get; } = new();

    public ILabelControlsForm Data => LabelInputFormPanel.Data;

    public void ReRender()
    {
        LabelInputFormPanel.ReRender();

        UpdateWidthAndHeight();
    }

    private void InitializeDialogBox(ILabelControlsForm labelControlsForm)
    {
        InitializeComponent();

        DataContext = this;

        PopulateLabelInputFormPanel(labelControlsForm);
    }

    private void PopulateLabelInputFormPanel(
        ILabelControlsForm labelControlsForm)
    {
        LabelInputFormPanel.Render(
            Settings.Form,
            labelControlsForm);

        UpdateWidthAndHeight();
    }

    private void UpdateWidthAndHeight()
    {
        Width = ContentCenter.Padding.Left +
                ContentCenter.Padding.Right +
                Data.GetMaxWidth() +
                ScrollBarSize;

        if (Width > Settings.Form.MaxSize.Width)
        {
            Width = Settings.Form.MaxSize.Width;
        }

        Height = ContentCenter.Padding.Top +
                 ContentCenter.Padding.Bottom +
                 ContentButton.Height +
                 Data.GetMaxHeight() +
                 ScrollBarSize;

        if (HeaderControl is not null)
        {
            Height += ContentTop.Height;
        }

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