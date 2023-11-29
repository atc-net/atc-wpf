// ReSharper disable UnusedParameter.Local
namespace Atc.Wpf.Controls.BaseControls;

/// <summary>
/// Interaction logic for DirectoryPicker.
/// </summary>
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

    [SuppressMessage("Naming", "CA1721:Property names should not match get methods", Justification = "OK.")]
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

    public DirectoryPicker()
    {
        InitializeComponent();

        DataContext = this;
    }

    [SuppressMessage("Major Code Smell", "S1172:Unused method parameters should be removed", Justification = "OK.")]
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

    [SuppressMessage("Major Code Smell", "S2589:Boolean expressions should not be gratuitous", Justification = "OK.")]
    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
    protected virtual void OnValueChanged(
        DirectoryInfo? oldValue,
        DirectoryInfo? newValue)
    {
        if (!Equals(oldValue, newValue))
        {
            DisplayValue = newValue?.FullName;

            RaiseEvent(new RoutedPropertyChangedEventArgs<DirectoryInfo?>(oldValue, newValue, ValueChangedEvent));
        }
    }

    [SuppressMessage("Major Code Smell", "S1172:Unused method parameters should be removed", Justification = "OK.")]
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
            if (string.IsNullOrEmpty(value))
            {
                directoryPicker.Value = null;
            }
            else
            {
                directoryPicker.Value = new DirectoryInfo(value);
            }
        }
    }

    private void OnClick(object sender, RoutedEventArgs e)
    {
        var folderDialog = new OpenFolderDialog
        {
            Title = "Select Directory",
            Multiselect = false,
        };

        var dialogResult = folderDialog.ShowDialog();
        if (dialogResult.HasValue && dialogResult.Value)
        {
            Value = new DirectoryInfo(folderDialog.FolderName);
        }
        else
        {
            Value = null;
        }
    }
}