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
            Height += GetHeaderHeight(HeaderControl);
        }

        if (Height > Settings.Form.MaxSize.Height)
        {
            Height = Settings.Form.MaxSize.Height;
        }
    }

    /// <summary>
    /// Gets the height of the header area: the header measured at the dialog width (a long header wraps), at least
    /// the minimum height of the area.
    /// </summary>
    private double GetHeaderHeight(ContentControl headerControl)
    {
        headerControl.Measure(new Size(Width, double.PositiveInfinity));
        return System.Math.Max(
            ContentTop.MinHeight,
            headerControl.DesiredSize.Height + ContentTop.BorderThickness.Top + ContentTop.BorderThickness.Bottom);
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