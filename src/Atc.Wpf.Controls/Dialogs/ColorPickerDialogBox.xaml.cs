namespace Atc.Wpf.Controls.Dialogs;

/// <summary>
/// Interaction logic for ColorPickerDialogBox.
/// </summary>
public partial class ColorPickerDialogBox
{
    public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
        nameof(Color),
        typeof(Color),
        typeof(ColorPickerDialogBox),
        new PropertyMetadata(Colors.Black));

    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    public ColorPickerDialogBox(
        Window owningWindow,
        Color color)
        : this(
            owningWindow,
            CreateDefaultSettings(),
            color)
    {
    }

    public ColorPickerDialogBox(
        Window owningWindow,
        DialogBoxSettings settings,
        Color color)
    {
        OwningWindow = owningWindow;
        Settings = settings;
        Color = color;
        Width = Settings.Width;
        Height = Settings.Height;

        InitializeDialogBox();
    }

    public Window OwningWindow { get; private set; }

    public DialogBoxSettings Settings { get; }

    public ContentControl? HeaderControl { get; set; }

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
        settings.TitleBarText = Miscellaneous.ColorPicker;
        return settings;
    }
}