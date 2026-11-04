// ReSharper disable ParameterTypeCanBeEnumerable.Local
namespace Atc.Wpf.Components.Dialogs;

public partial class InputFormDialogBox
{
    private const int ScrollBarSize = 20;

    /// <summary>
    /// Identifies the <see cref="HeaderControl"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HeaderControlProperty = DependencyProperty.Register(
        nameof(HeaderControl),
        typeof(ContentControl),
        typeof(InputFormDialogBox),
        new PropertyMetadata(defaultValue: null, OnHeaderControlChanged));

    /// <summary>
    /// Initializes a new instance of the <see cref="InputFormDialogBox"/> class with OK/Cancel buttons and the given form.
    /// </summary>
    public InputFormDialogBox(
        Window owningWindow,
        ILabelControlsForm labelControlsForm)
        : this(
            owningWindow,
            DialogBoxSettings.Create(DialogBoxType.OkCancel),
            labelControlsForm)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InputFormDialogBox"/> class with OK/Cancel buttons, a title bar text and the given form.
    /// </summary>
    public InputFormDialogBox(
        Window owningWindow,
        string titleBarText,
        ILabelControlsForm labelControlsForm)
        : this(
            owningWindow,
            DialogBoxSettings.Create(DialogBoxType.OkCancel),
            labelControlsForm)
        => Settings.TitleBarText = titleBarText;

    /// <summary>
    /// Initializes a new instance of the <see cref="InputFormDialogBox"/> class with OK/Cancel buttons, a title bar text, a header text and the given form.
    /// </summary>
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

    /// <summary>
    /// Initializes a new instance of the <see cref="InputFormDialogBox"/> class with OK/Cancel buttons, the given form panel settings and form.
    /// </summary>
    public InputFormDialogBox(
        Window owningWindow,
        LabelInputFormPanelSettings formPanelSettings,
        ILabelControlsForm labelControlsForm)
    {
        ArgumentNullException.ThrowIfNull(labelControlsForm);

        OwningWindow = owningWindow;
        if (owningWindow is not null)
        {
            FlowDirection = owningWindow.FlowDirection;
        }

        Settings = DialogBoxSettings.Create(DialogBoxType.OkCancel);
        Width = Settings.Width;
        Height = Settings.Height;
        Settings.Form = formPanelSettings;

        InitializeDialogBox(labelControlsForm);

        DialogBoxWindowHelper.ApplyOwner(this, owningWindow);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InputFormDialogBox"/> class with the given dialog settings and form.
    /// </summary>
    public InputFormDialogBox(
        Window owningWindow,
        DialogBoxSettings settings,
        ILabelControlsForm labelControlsForm)
    {
        ArgumentNullException.ThrowIfNull(labelControlsForm);

        OwningWindow = owningWindow;
        if (owningWindow is not null)
        {
            FlowDirection = owningWindow.FlowDirection;
        }

        Settings = settings;
        Width = Settings.Width;
        Height = Settings.Height;
        Settings.Form.UseGroupBox = labelControlsForm.HasMultiGroupIdentifiers();

        InitializeDialogBox(labelControlsForm);

        DialogBoxWindowHelper.ApplyOwner(this, owningWindow);
    }

    /// <summary>
    /// Gets the window that owns the dialog box.
    /// </summary>
    public Window OwningWindow { get; private set; }

    /// <summary>
    /// Gets the settings that control the dialog box appearance and buttons.
    /// </summary>
    public DialogBoxSettings Settings { get; }

    /// <summary>
    /// Gets or sets the optional header control shown above the form.
    /// </summary>
    public ContentControl? HeaderControl
    {
        get => (ContentControl?)GetValue(HeaderControlProperty);
        set => SetValue(HeaderControlProperty, value);
    }

    /// <summary>
    /// Gets the panel that renders the form.
    /// </summary>
    public LabelInputFormPanel LabelInputFormPanel { get; } = new();

    /// <summary>
    /// Gets the form whose values the user edits.
    /// </summary>
    public ILabelControlsForm Data => LabelInputFormPanel.Data;

    /// <summary>
    /// Re-renders the form panel and recalculates the dialog width and height.
    /// </summary>
    public void ReRender()
    {
        LabelInputFormPanel.ReRender();

        UpdateWidthAndHeight();
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(
        DependencyPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        Helpers.DialogBoxHelper.OnDialogPropertyChanged(this, e);
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

    /// <summary>
    /// Sizes the dialog to the form: the width from the form's widest row, the height from the content (header,
    /// form and buttons, so a wrapped header is included). Both stay within <see cref="LabelInputFormPanelSettings.MaxSize"/>
    /// and <see cref="DialogBoxWindowHelper.MaxWorkAreaRatio"/> of the work area; beyond that the form scrolls.
    /// </summary>
    private void UpdateWidthAndHeight()
    {
        var workArea = DialogBoxWindowHelper.GetWorkArea(OwningWindow);
        var maxWidth = System.Math.Min(Settings.Form.MaxSize.Width, workArea.Width * DialogBoxWindowHelper.MaxWorkAreaRatio);
        var maxHeight = System.Math.Min(Settings.Form.MaxSize.Height, workArea.Height * DialogBoxWindowHelper.MaxWorkAreaRatio);

        Width = System.Math.Min(
            ContentCenter.Padding.Left +
            ContentCenter.Padding.Right +
            Data.GetMaxWidth() +
            ScrollBarSize,
            maxWidth);

        MaxHeight = maxHeight;
        if (SizeToContent != SizeToContent.Height)
        {
            ClearValue(HeightProperty);
            SizeToContent = SizeToContent.Height;
        }
    }

    private static void OnHeaderControlChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var dialogBox = (InputFormDialogBox)d;
        if (dialogBox.ContentCenter is not null)
        {
            dialogBox.UpdateWidthAndHeight();
        }
    }

    private void OnButtonClicked(
        object? sender,
        DialogBoxButtonClickedEventArgs e)
    {
        if (e.Button == DialogBoxButton.Affirmative)
        {
            Confirm();
        }
        else
        {
            Decline();
        }
    }

    private void Confirm()
    {
        if (!LabelInputFormPanel.Data.IsValid())
        {
            return;
        }

        DialogResult = true;
        Close();
    }

    private void Decline()
    {
        DialogResult = false;
        Close();
    }
}