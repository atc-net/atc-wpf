namespace Atc.Wpf.Components.Dialogs;

public partial class InputDialogBox
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InputDialogBox"/> class with OK/Cancel buttons and the given input control.
    /// </summary>
    public InputDialogBox(
        Window owningWindow,
        ILabelControlBase labelControl)
        : this(
            owningWindow,
            DialogBoxSettings.Create(DialogBoxType.OkCancel),
            labelControl)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InputDialogBox"/> class with OK/Cancel buttons, a title bar text and the given input control.
    /// </summary>
    public InputDialogBox(
        Window owningWindow,
        string titleBarText,
        ILabelControlBase labelControl)
        : this(
            owningWindow,
            DialogBoxSettings.Create(DialogBoxType.OkCancel),
            labelControl)
        => Settings.TitleBarText = titleBarText;

    /// <summary>
    /// Initializes a new instance of the <see cref="InputDialogBox"/> class with OK/Cancel buttons, a title bar text, a header text and the given input control.
    /// </summary>
    public InputDialogBox(
        Window owningWindow,
        string titleBarText,
        string headerText,
        ILabelControlBase labelControl)
        : this(
            owningWindow,
            titleBarText,
            labelControl)
        => HeaderControl = Helpers.DialogBoxHelper.CreateHeaderControl(headerText);

    /// <summary>
    /// Initializes a new instance of the <see cref="InputDialogBox"/> class with the given dialog settings and input control.
    /// </summary>
    public InputDialogBox(
        Window owningWindow,
        DialogBoxSettings settings,
        ILabelControlBase labelControl)
    {
        OwningWindow = owningWindow;
        if (owningWindow is not null)
        {
            FlowDirection = owningWindow.FlowDirection;
        }

        Settings = settings;
        Width = Settings.Width;
        Height = Settings.Height;

        Data = labelControl;

        InitializeDialogBox();
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
    /// Gets or sets the optional header control shown above the input control.
    /// </summary>
    public ContentControl? HeaderControl { get; set; }

    /// <summary>
    /// Gets or sets the control that hosts the input control.
    /// </summary>
    public ContentControl ContentControl { get; set; } = new();

    /// <summary>
    /// Gets the input control whose value the user edits.
    /// </summary>
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