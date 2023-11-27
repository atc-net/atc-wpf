// ReSharper disable UnusedParameter.Local
namespace Atc.Wpf.Controls.BaseControls;

/// <summary>
/// Interaction logic for FilePicker.
/// </summary>
[SuppressMessage("Naming", "CA1721:Property names should not match get methods", Justification = "OK.")]
[SuppressMessage("Major Code Smell", "S1172:Unused method parameters should be removed", Justification = "OK.")]
public partial class FilePicker
{
    public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(
        nameof(ValueChanged),
        RoutingStrategy.Bubble,
        typeof(RoutedPropertyChangedEventHandler<FileInfo?>),
        typeof(FilePicker));

    public event RoutedPropertyChangedEventHandler<FileInfo?> ValueChanged
    {
        add => AddHandler(ValueChangedEvent, value);
        remove => RemoveHandler(ValueChangedEvent, value);
    }

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value),
        typeof(FileInfo),
        typeof(FilePicker),
        new FrameworkPropertyMetadata(
            default(FileInfo?),
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
            default(string?),
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            OnDisplayValuePropertyChanged,
            (o, value) => CoerceDisplayValue(o, value).Item1));

    public string? DisplayValue
    {
        get => (string?)GetValue(FullNameProperty);
        set => SetValue(FullNameProperty, value);
    }

    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        nameof(Title),
        typeof(string),
        typeof(FilePicker),
        new PropertyMetadata(default(string)));

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly DependencyProperty ShowClearTextButtonProperty = DependencyProperty.Register(
        nameof(ShowClearTextButton),
        typeof(bool),
        typeof(FilePicker),
        new PropertyMetadata(default(bool)));

    public bool ShowClearTextButton
    {
        get => (bool)GetValue(ShowClearTextButtonProperty);
        set => SetValue(ShowClearTextButtonProperty, value);
    }

    public static readonly DependencyProperty AllowOnlyExistingProperty = DependencyProperty.Register(
        nameof(AllowOnlyExisting),
        typeof(bool),
        typeof(FilePicker),
        new PropertyMetadata(default(bool)));

    public bool AllowOnlyExisting
    {
        get => (bool)GetValue(AllowOnlyExistingProperty);
        set => SetValue(AllowOnlyExistingProperty, value);
    }

    public static readonly DependencyProperty UsePreviewPaneProperty = DependencyProperty.Register(
        nameof(UsePreviewPane),
        typeof(bool),
        typeof(FilePicker),
        new PropertyMetadata(default(bool)));

    public bool UsePreviewPane
    {
        get => (bool)GetValue(UsePreviewPaneProperty);
        set => SetValue(UsePreviewPaneProperty, value);
    }

    public static readonly DependencyProperty FilterProperty = DependencyProperty.Register(
        nameof(Filter),
        typeof(string),
        typeof(FilePicker),
        new PropertyMetadata(string.Empty));

    public string Filter
    {
        get => (string)GetValue(FilterProperty);
        set => SetValue(FilterProperty, value);
    }

    public static readonly DependencyProperty DefaultDirectoryProperty = DependencyProperty.Register(
        nameof(DefaultDirectory),
        typeof(string),
        typeof(FilePicker),
        new PropertyMetadata(string.Empty));

    public string DefaultDirectory
    {
        get => (string)GetValue(DefaultDirectoryProperty);
        set => SetValue(DefaultDirectoryProperty, value);
    }

    public static readonly DependencyProperty InitialDirectoryProperty = DependencyProperty.Register(
        nameof(InitialDirectory),
        typeof(string),
        typeof(FilePicker),
        new PropertyMetadata(string.Empty));

    public string InitialDirectory
    {
        get => (string)GetValue(InitialDirectoryProperty);
        set => SetValue(InitialDirectoryProperty, value);
    }

    public static readonly DependencyProperty RootDirectoryProperty = DependencyProperty.Register(
        nameof(RootDirectory),
        typeof(string),
        typeof(FilePicker),
        new PropertyMetadata(string.Empty));

    public string RootDirectory
    {
        get => (string)GetValue(RootDirectoryProperty);
        set => SetValue(RootDirectoryProperty, value);
    }

    public FilePicker()
    {
        InitializeComponent();

        DataContext = this;
    }

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
        var fileDialog = new OpenFileDialog
        {
            Multiselect = false,
            Title = string.IsNullOrEmpty(Title)
                ? "Select File" // TODO: Translation
                : Title,
            DefaultDirectory = DefaultDirectory,
            InitialDirectory = InitialDirectory,
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