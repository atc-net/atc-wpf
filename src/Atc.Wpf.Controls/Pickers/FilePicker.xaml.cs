// ReSharper disable UnusedParameter.Local
namespace Atc.Wpf.Controls.Pickers;

[SuppressMessage("Naming", "CA1721:Property names should not match get methods", Justification = "OK.")]
[SuppressMessage("Major Code Smell", "S1172:Unused method parameters should be removed", Justification = "OK.")]
public partial class FilePicker
{
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<FileInfo?>))]
    private static readonly RoutedEvent valueChanged;

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value),
        typeof(FileInfo),
        typeof(FilePicker),
        new FrameworkPropertyMetadata(
            defaultValue: null,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            OnValuePropertyChanged,
            (o, value) => CoerceValue(o, value).Item1));

    public FileInfo? Value
    {
        get => (FileInfo?)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public static readonly DependencyProperty FullNameProperty = DependencyProperty.Register(
        nameof(DisplayValue),
        typeof(string),
        typeof(FilePicker),
        new FrameworkPropertyMetadata(
            defaultValue: null,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            OnDisplayValuePropertyChanged,
            (o, value) => CoerceDisplayValue(o, value).Item1));

    public string? DisplayValue
    {
        get => (string?)GetValue(FullNameProperty);
        set => SetValue(FullNameProperty, value);
    }

    [DependencyProperty]
    private string title;

    [DependencyProperty]
    private bool showClearTextButton;

    [DependencyProperty(DefaultValue = "")]
    private string watermarkText;

    [DependencyProperty(DefaultValue = TextAlignment.Left)]
    private TextAlignment watermarkAlignment;

    [DependencyProperty(DefaultValue = TextTrimming.None)]
    private TextTrimming watermarkTrimming;

    [DependencyProperty]
    private bool allowOnlyExisting;

    [DependencyProperty]
    private bool usePreviewPane;

    [DependencyProperty(DefaultValue = "")]
    private string filter;

    [DependencyProperty(DefaultValue = "")]
    private string defaultDirectory;

    [DependencyProperty(DefaultValue = "")]
    private string initialDirectory;

    [DependencyProperty(DefaultValue = "")]
    private string rootDirectory;
    public FilePicker()
    {
        InitializeComponent();

        DataContext = this;
    }

    protected override AutomationPeer OnCreateAutomationPeer()
        => new FilePickerAutomationPeer(this);

    private static Tuple<FileInfo?, bool> CoerceValue(
        DependencyObject d,
        object? baseValue)
    {
        var value = (FileInfo?)baseValue;

        return new Tuple<FileInfo?, bool>(value, item2: false);
    }

    private static void OnValuePropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue == e.NewValue)
        {
            return;
        }

        (d as FilePicker)?.OnValueChanged((FileInfo?)e.OldValue, (FileInfo?)e.NewValue);
    }

    protected virtual void OnValueChanged(
        FileInfo? oldValue,
        FileInfo? newValue)
    {
        if (Equals(oldValue, newValue))
        {
            return;
        }

        DisplayValue = newValue?.FullName;

        RaiseEvent(new RoutedPropertyChangedEventArgs<FileInfo?>(oldValue, newValue, ValueChangedEvent));
    }

    private static Tuple<string?, bool> CoerceDisplayValue(
        DependencyObject d,
        object? baseValue)
    {
        var value = (string?)baseValue;

        return new Tuple<string?, bool>(value, item2: false);
    }

    private static void OnDisplayValuePropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue == e.NewValue)
        {
            return;
        }

        if (d is not FilePicker filePicker)
        {
            return;
        }

        var value = e.NewValue?.ToString();
        if (filePicker.Value?.ToString() != value)
        {
            filePicker.Value = string.IsNullOrEmpty(value)
                ? null
                : new FileInfo(value);
        }
    }

    private void OnClick(
        object sender,
        RoutedEventArgs e)
    {
        var resolvedInitialDirectory = InitialDirectory;
        if (string.IsNullOrEmpty(resolvedInitialDirectory) &&
            Value?.Directory is { Exists: true })
        {
            resolvedInitialDirectory = Value.Directory.FullName;
        }

        var fileDialog = new OpenFileDialog
        {
            Multiselect = false,
            Title = string.IsNullOrEmpty(Title)
                ? Miscellaneous.SelectFile
                : Title,
            DefaultDirectory = DefaultDirectory,
            InitialDirectory = resolvedInitialDirectory,
            RootDirectory = RootDirectory,
            CheckFileExists = AllowOnlyExisting,
            CheckPathExists = AllowOnlyExisting,
            ForcePreviewPane = UsePreviewPane,
            Filter = Filter,
        };

        var dialogResult = fileDialog.ShowDialog();
        if (dialogResult.HasValue && dialogResult.Value)
        {
            Value = new FileInfo(fileDialog.FileName);
        }
    }
}