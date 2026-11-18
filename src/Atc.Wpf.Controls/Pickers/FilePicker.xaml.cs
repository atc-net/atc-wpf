// ReSharper disable UnusedParameter.Local
using SharedMisc = Atc.Wpf.Resources.Miscellaneous;

namespace Atc.Wpf.Controls.Pickers;

[SuppressMessage("Naming", "CA1721:Property names should not match get methods", Justification = "OK.")]
[SuppressMessage("Major Code Smell", "S1172:Unused method parameters should be removed", Justification = "OK.")]
public partial class FilePicker
{
    /// <summary>Occurs when the selected file changes.</summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<FileInfo?>))]
    private static readonly RoutedEvent valueChanged;

    /// <summary>Gets or sets the selected file.</summary>
    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        PropertyChangedCallback = nameof(OnValuePropertyChanged),
        CoerceValueCallback = nameof(CoerceValueProperty))]
    private FileInfo? value;

    /// <summary>Identifies the <see cref="DisplayValue"/> dependency property.</summary>
    public static readonly DependencyProperty FullNameProperty = DependencyProperty.Register(
        nameof(DisplayValue),
        typeof(string),
        typeof(FilePicker),
        new FrameworkPropertyMetadata(
            defaultValue: null,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            OnDisplayValuePropertyChanged,
            (o, value) => CoerceDisplayValue(o, value).Item1));

    /// <summary>Gets or sets the full path of the selected file, as shown in the text box.</summary>
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

    /// <summary>Initializes a new instance of the <see cref="FilePicker"/> class.</summary>
    public FilePicker()
    {
        InitializeComponent();

        // Only the inner layout binds to the picker, so the picker itself keeps the DataContext it inherits.
        LayoutRoot.DataContext = this;
    }

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer()
        => new FilePickerAutomationPeer(this);

    private static object? CoerceValueProperty(
        DependencyObject d,
        object? baseValue)
        => CoerceValue(d, baseValue).Item1;

    private static Tuple<FileInfo?, bool> CoerceValue(
        DependencyObject d,
        object? baseValue)
    {
        var file = (FileInfo?)baseValue;

        return new Tuple<FileInfo?, bool>(file, item2: false);
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

    /// <summary>Called when the selected file changes; updates <see cref="DisplayValue"/> and raises the value changed event.</summary>
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
        var text = (string?)baseValue;

        return new Tuple<string?, bool>(text, item2: false);
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

        var text = e.NewValue?.ToString();
        if (filePicker.Value?.ToString() != text)
        {
            filePicker.Value = string.IsNullOrEmpty(text)
                ? null
                : new FileInfo(text);
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
                ? SharedMisc.SelectFile
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