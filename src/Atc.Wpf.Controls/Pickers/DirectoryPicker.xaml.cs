// ReSharper disable UnusedParameter.Local
// ReSharper disable InconsistentNaming
using SharedMisc = Atc.Wpf.Resources.Miscellaneous;

namespace Atc.Wpf.Controls.Pickers;

[SuppressMessage("Naming", "CA1721:Property names should not match get methods", Justification = "OK.")]
[SuppressMessage("Major Code Smell", "S1172:Unused method parameters should be removed", Justification = "OK.")]
public partial class DirectoryPicker
{
    /// <summary>Occurs when the selected directory changes.</summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<DirectoryInfo?>))]
    private static readonly RoutedEvent valueChanged;

    /// <summary>Identifies the <see cref="Value"/> dependency property.</summary>
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value),
        typeof(DirectoryInfo),
        typeof(DirectoryPicker),
        new FrameworkPropertyMetadata(
            defaultValue: null,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            OnValuePropertyChanged,
            (o, value) => CoerceValue(o, value).Item1));

    /// <summary>Gets or sets the selected directory.</summary>
    public DirectoryInfo? Value
    {
        get => (DirectoryInfo?)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>Identifies the <see cref="DisplayValue"/> dependency property.</summary>
    public static readonly DependencyProperty FullNameProperty = DependencyProperty.Register(
        nameof(DisplayValue),
        typeof(string),
        typeof(DirectoryPicker),
        new FrameworkPropertyMetadata(
            defaultValue: null,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            OnDisplayValuePropertyChanged,
            (o, value) => CoerceDisplayValue(o, value).Item1));

    /// <summary>Gets or sets the full path of the selected directory, as shown in the text box.</summary>
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

    [DependencyProperty(DefaultValue = "")]
    private string defaultDirectory;

    [DependencyProperty(DefaultValue = "")]
    private string initialDirectory;

    [DependencyProperty(DefaultValue = "")]
    private string rootDirectory;

    /// <summary>Initializes a new instance of the <see cref="DirectoryPicker"/> class.</summary>
    public DirectoryPicker()
    {
        InitializeComponent();

        // Only the inner layout binds to the picker, so the picker itself keeps the DataContext it inherits.
        LayoutRoot.DataContext = this;
    }

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer()
        => new DirectoryPickerAutomationPeer(this);

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

    /// <summary>Called when the selected directory changes; updates <see cref="DisplayValue"/> and raises the value changed event.</summary>
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
        var resolvedInitialDirectory = InitialDirectory;
        if (string.IsNullOrEmpty(resolvedInitialDirectory) &&
            Value is { Exists: true })
        {
            resolvedInitialDirectory = Value.FullName;
        }

        var folderDialog = new OpenFolderDialog
        {
            Multiselect = false,
            Title = string.IsNullOrEmpty(Title)
                ? SharedMisc.SelectDirectory
                : Title,
            DefaultDirectory = DefaultDirectory,
            InitialDirectory = resolvedInitialDirectory,
            RootDirectory = RootDirectory,
        };

        var dialogResult = folderDialog.ShowDialog();
        if (dialogResult.HasValue && dialogResult.Value)
        {
            Value = new DirectoryInfo(folderDialog.FolderName);
        }
    }
}