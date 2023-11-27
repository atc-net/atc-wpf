// ReSharper disable UnusedParameter.Local
namespace Atc.Wpf.Controls.BaseControls;

/// <summary>
/// Interaction logic for DirectoryPicker.
/// </summary>
[SuppressMessage("Naming", "CA1721:Property names should not match get methods", Justification = "OK.")]
[SuppressMessage("Major Code Smell", "S1172:Unused method parameters should be removed", Justification = "OK.")]
public partial class DirectoryPicker
{
    public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(
        nameof(ValueChanged),
        RoutingStrategy.Bubble,
        typeof(RoutedPropertyChangedEventHandler<DirectoryInfo?>),
        typeof(DirectoryPicker));

    public event RoutedPropertyChangedEventHandler<DirectoryInfo?> ValueChanged
    {
        add => AddHandler(ValueChangedEvent, value);
        remove => RemoveHandler(ValueChangedEvent, value);
    }

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value),
        typeof(DirectoryInfo),
        typeof(DirectoryPicker),
        new FrameworkPropertyMetadata(
            default(DirectoryInfo?),
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            OnValuePropertyChanged,
            (o, value) => CoerceValue(o, value).Item1));

    public DirectoryInfo? Value
    {
        get => (DirectoryInfo?)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public static readonly DependencyProperty FullNameProperty = DependencyProperty.Register(
        nameof(DisplayValue),
        typeof(string),
        typeof(DirectoryPicker),
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
        typeof(DirectoryPicker),
        new PropertyMetadata(default(string)));

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly DependencyProperty ShowClearTextButtonProperty = DependencyProperty.Register(
        nameof(ShowClearTextButton),
        typeof(bool),
        typeof(DirectoryPicker),
        new PropertyMetadata(default(bool)));

    public bool ShowClearTextButton
    {
        get => (bool)GetValue(ShowClearTextButtonProperty);
        set => SetValue(ShowClearTextButtonProperty, value);
    }

    public static readonly DependencyProperty DefaultDirectoryProperty = DependencyProperty.Register(
        nameof(DefaultDirectory),
        typeof(string),
        typeof(DirectoryPicker),
        new PropertyMetadata(string.Empty));

    public string DefaultDirectory
    {
        get => (string)GetValue(DefaultDirectoryProperty);
        set => SetValue(DefaultDirectoryProperty, value);
    }

    public static readonly DependencyProperty InitialDirectoryProperty = DependencyProperty.Register(
        nameof(InitialDirectory),
        typeof(string),
        typeof(DirectoryPicker),
        new PropertyMetadata(string.Empty));

    public string InitialDirectory
    {
        get => (string)GetValue(InitialDirectoryProperty);
        set => SetValue(InitialDirectoryProperty, value);
    }

    public static readonly DependencyProperty RootDirectoryProperty = DependencyProperty.Register(
        nameof(RootDirectory),
        typeof(string),
        typeof(DirectoryPicker),
        new PropertyMetadata(string.Empty));

    public string RootDirectory
    {
        get => (string)GetValue(RootDirectoryProperty);
        set => SetValue(RootDirectoryProperty, value);
    }

    public DirectoryPicker()
    {
        InitializeComponent();

        DataContext = this;
    }

    private static Tuple<DirectoryInfo?, bool> CoerceValue(
        DependencyObject d,
        object? baseValue)
    {
        var value = (DirectoryInfo?)baseValue;

        return new Tuple<DirectoryInfo?, bool>(value, item2: false);
    }

    private static void OnValuePropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue == e.NewValue)
        {
            return;
        }

        (d as DirectoryPicker)?.OnValueChanged((DirectoryInfo?)e.OldValue, (DirectoryInfo?)e.NewValue);
    }

    protected virtual void OnValueChanged(
        DirectoryInfo? oldValue,
        DirectoryInfo? newValue)
    {
        if (Equals(oldValue, newValue))
        {
            return;
        }

        DisplayValue = newValue?.FullName;

        RaiseEvent(new RoutedPropertyChangedEventArgs<DirectoryInfo?>(oldValue, newValue, ValueChangedEvent));
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

        if (d is not DirectoryPicker directoryPicker)
        {
            return;
        }

        var value = e.NewValue?.ToString();
        if (directoryPicker.Value?.ToString() != value)
        {
            directoryPicker.Value = string.IsNullOrEmpty(value)
                ? null
                : new DirectoryInfo(value);
        }
    }

    private void OnClick(
        object sender,
        RoutedEventArgs e)
    {
        var folderDialog = new OpenFolderDialog
        {
            Multiselect = false,
            Title = string.IsNullOrEmpty(Title)
                ? "Select Directory" // TODO: Translation
                : Title,
            DefaultDirectory = DefaultDirectory,
            InitialDirectory = InitialDirectory,
            RootDirectory = RootDirectory,
        };

        var dialogResult = folderDialog.ShowDialog();
        if (dialogResult.HasValue && dialogResult.Value)
        {
            Value = new DirectoryInfo(folderDialog.FolderName);
        }
    }
}