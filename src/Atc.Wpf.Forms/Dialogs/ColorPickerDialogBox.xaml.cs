using ControlsMiscellaneous = Atc.Wpf.Controls.Resources.Miscellaneous;

namespace Atc.Wpf.Forms.Dialogs;

public partial class ColorPickerDialogBox
{
    [DependencyProperty(
        DefaultValue = nameof(Brushes.Black))]
    private Color color;

    /// <summary>
    /// Initializes a new instance of the <see cref="ColorPickerDialogBox"/> class with default OK/Cancel settings.
    /// </summary>
    /// <param name="owningWindow">The window that owns the dialog.</param>
    /// <param name="color">The initial color.</param>
    public ColorPickerDialogBox(
        Window owningWindow,
        Color color)
        : this(
            owningWindow,
            CreateDefaultSettings(),
            color)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ColorPickerDialogBox"/> class.
    /// </summary>
    /// <param name="owningWindow">The window that owns the dialog.</param>
    /// <param name="settings">The dialog settings, such as size, title and button texts.</param>
    /// <param name="color">The initial color.</param>
    public ColorPickerDialogBox(
        Window owningWindow,
        DialogBoxSettings settings,
        Color color)
    {
        OwningWindow = owningWindow;
        if (owningWindow is not null)
        {
            FlowDirection = owningWindow.FlowDirection;
        }

        Settings = settings;
        Color = color;
        Width = Settings.Width;
        Height = Settings.Height;

        InitializeDialogBox();
    }

    /// <summary>
    /// Gets the window that owns the dialog.
    /// </summary>
    public Window OwningWindow { get; private set; }

    /// <summary>
    /// Gets the dialog settings.
    /// </summary>
    public DialogBoxSettings Settings { get; }

    /// <summary>
    /// Gets or sets an optional control shown in the dialog header.
    /// </summary>
    public ContentControl? HeaderControl { get; set; }

    /// <summary>
    /// Gets a new <see cref="SolidColorBrush"/> for the current color.
    /// </summary>
    public SolidColorBrush ColorAsBrush => new(Color);

    private void InitializeDialogBox()
    {
        InitializeComponent();

        DataContext = this;

        UcAdvancedColorPicker.OriginalColor = Color;
    }

    private void OnOkClick(
        object sender,
        RoutedEventArgs e)
    {
        Color = UcAdvancedColorPicker.Color;

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

    private static DialogBoxSettings CreateDefaultSettings()
    {
        var settings = DialogBoxSettings.Create(DialogBoxType.OkCancel);
        settings.Width = 770;
        settings.Height = 700;
        settings.TitleBarText = ControlsMiscellaneous.ColorPicker;
        return settings;
    }
}