// ReSharper disable InvertIf
namespace Atc.Wpf.Controls.LabelControls;

/// <summary>
/// Interaction logic for LabelDirectoryPicker.
/// </summary>
[SuppressMessage("Naming", "CA1721:Property names should not match get methods", Justification = "OK.")]
[SuppressMessage("Usage", "MA0091:Sender should be 'this' for instance events", Justification = "OK.")]
public partial class LabelDirectoryPicker : ILabelDirectoryPicker
{
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value),
        typeof(DirectoryInfo),
        typeof(LabelDirectoryPicker),
        new PropertyMetadata(default(DirectoryInfo?)));

    public DirectoryInfo? Value
    {
        get => (DirectoryInfo?)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public static readonly DependencyProperty ShowClearTextButtonProperty = DependencyProperty.Register(
        nameof(ShowClearTextButton),
        typeof(bool),
        typeof(LabelDirectoryPicker),
        new PropertyMetadata(default(bool)));

    public bool ShowClearTextButton
    {
        get => (bool)GetValue(ShowClearTextButtonProperty);
        set => SetValue(ShowClearTextButtonProperty, value);
    }

    public static readonly DependencyProperty AllowOnlyExistingProperty = DependencyProperty.Register(
        nameof(AllowOnlyExisting),
        typeof(bool),
        typeof(LabelDirectoryPicker),
        new PropertyMetadata(default(bool)));

    public bool AllowOnlyExisting
    {
        get => (bool)GetValue(AllowOnlyExistingProperty);
        set => SetValue(AllowOnlyExistingProperty, value);
    }

    public static readonly DependencyProperty DefaultDirectoryProperty = DependencyProperty.Register(
        nameof(DefaultDirectory),
        typeof(string),
        typeof(LabelDirectoryPicker),
        new PropertyMetadata(string.Empty));

    public string DefaultDirectory
    {
        get => (string)GetValue(DefaultDirectoryProperty);
        set => SetValue(DefaultDirectoryProperty, value);
    }

    public static readonly DependencyProperty InitialDirectoryProperty = DependencyProperty.Register(
        nameof(InitialDirectory),
        typeof(string),
        typeof(LabelDirectoryPicker),
        new PropertyMetadata(string.Empty));

    public string InitialDirectory
    {
        get => (string)GetValue(InitialDirectoryProperty);
        set => SetValue(InitialDirectoryProperty, value);
    }

    public static readonly DependencyProperty RootDirectoryProperty = DependencyProperty.Register(
        nameof(RootDirectory),
        typeof(string),
        typeof(LabelDirectoryPicker),
        new PropertyMetadata(string.Empty));

    public string RootDirectory
    {
        get => (string)GetValue(RootDirectoryProperty);
        set => SetValue(RootDirectoryProperty, value);
    }

    public event EventHandler<ChangedDirectoryInfoEventArgs>? LostFocusValid;

    public event EventHandler<ChangedDirectoryInfoEventArgs>? LostFocusInvalid;

    public LabelDirectoryPicker()
    {
        InitializeComponent();
    }

    private void OnValueChanged(
        object sender,
        RoutedPropertyChangedEventArgs<DirectoryInfo?> e)
    {
        Validate(this, e, raiseEvents: true);
    }

    private static void Validate(
        LabelDirectoryPicker control,
        RoutedPropertyChangedEventArgs<DirectoryInfo?> e,
        bool raiseEvents)
    {
        control.ValidationText = string.Empty;

        if (control is { IsMandatory: true, Value: null })
        {
            control.ValidationText = Validations.FieldIsRequired;
            if (raiseEvents)
            {
                OnLostFocusFireInvalidEvent(control, e);
            }

            return;
        }

        if (control.Value is not null && control.AllowOnlyExisting && !Directory.Exists(control.Value.FullName))
        {
            control.ValidationText = "Directory don't exist"; // TODO: Translation
            if (raiseEvents)
            {
                OnLostFocusFireInvalidEvent(control, e);
            }

            return;
        }

        OnLostFocusFireValidEvent(control, e);
    }

    private static void OnLostFocusFireValidEvent(
        LabelDirectoryPicker control,
        RoutedPropertyChangedEventArgs<DirectoryInfo?> e)
    {
        control.LostFocusValid?.Invoke(
            control,
            new ChangedDirectoryInfoEventArgs(
                ControlHelper.GetIdentifier(control),
                e.OldValue,
                e.NewValue));
    }

    private static void OnLostFocusFireInvalidEvent(
        LabelDirectoryPicker control,
        RoutedPropertyChangedEventArgs<DirectoryInfo?> e)
    {
        control.LostFocusInvalid?.Invoke(
            control,
            new ChangedDirectoryInfoEventArgs(
                ControlHelper.GetIdentifier(control),
                e.OldValue,
                e.NewValue));
    }
}