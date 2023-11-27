// ReSharper disable InvertIf
namespace Atc.Wpf.Controls.LabelControls;

/// <summary>
/// Interaction logic for LabelFilePicker.
/// </summary>
[SuppressMessage("Naming", "CA1721:Property names should not match get methods", Justification = "OK.")]
[SuppressMessage("Usage", "MA0091:Sender should be 'this' for instance events", Justification = "OK.")]
public partial class LabelFilePicker : ILabelFilePicker
{
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value),
        typeof(FileInfo),
        typeof(LabelFilePicker),
        new PropertyMetadata(default(FileInfo?)));

    public FileInfo? Value
    {
        get => (FileInfo?)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public static readonly DependencyProperty ShowClearTextButtonProperty = DependencyProperty.Register(
        nameof(ShowClearTextButton),
        typeof(bool),
        typeof(LabelFilePicker),
        new PropertyMetadata(default(bool)));

    public bool ShowClearTextButton
    {
        get => (bool)GetValue(ShowClearTextButtonProperty);
        set => SetValue(ShowClearTextButtonProperty, value);
    }

    public static readonly DependencyProperty AllowOnlyExistingProperty = DependencyProperty.Register(
        nameof(AllowOnlyExisting),
        typeof(bool),
        typeof(LabelFilePicker),
        new PropertyMetadata(default(bool)));

    public bool AllowOnlyExisting
    {
        get => (bool)GetValue(AllowOnlyExistingProperty);
        set => SetValue(AllowOnlyExistingProperty, value);
    }

    public static readonly DependencyProperty FilterProperty = DependencyProperty.Register(
        nameof(Filter),
        typeof(string),
        typeof(LabelFilePicker),
        new PropertyMetadata(string.Empty));

    public string Filter
    {
        get => (string)GetValue(FilterProperty);
        set => SetValue(FilterProperty, value);
    }

    public static readonly DependencyProperty UsePreviewPaneProperty = DependencyProperty.Register(
        nameof(UsePreviewPane),
        typeof(bool),
        typeof(LabelFilePicker),
        new PropertyMetadata(default(bool)));

    public bool UsePreviewPane
    {
        get => (bool)GetValue(UsePreviewPaneProperty);
        set => SetValue(UsePreviewPaneProperty, value);
    }

    public static readonly DependencyProperty DefaultDirectoryProperty = DependencyProperty.Register(
        nameof(DefaultDirectory),
        typeof(string),
        typeof(LabelFilePicker),
        new PropertyMetadata(string.Empty));

    public string DefaultDirectory
    {
        get => (string)GetValue(DefaultDirectoryProperty);
        set => SetValue(DefaultDirectoryProperty, value);
    }

    public static readonly DependencyProperty InitialDirectoryProperty = DependencyProperty.Register(
        nameof(InitialDirectory),
        typeof(string),
        typeof(LabelFilePicker),
        new PropertyMetadata(string.Empty));

    public string InitialDirectory
    {
        get => (string)GetValue(InitialDirectoryProperty);
        set => SetValue(InitialDirectoryProperty, value);
    }

    public static readonly DependencyProperty RootDirectoryProperty = DependencyProperty.Register(
        nameof(RootDirectory),
        typeof(string),
        typeof(LabelFilePicker),
        new PropertyMetadata(string.Empty));

    public string RootDirectory
    {
        get => (string)GetValue(RootDirectoryProperty);
        set => SetValue(RootDirectoryProperty, value);
    }

    public event EventHandler<ChangedFileInfoEventArgs>? LostFocusValid;

    public event EventHandler<ChangedFileInfoEventArgs>? LostFocusInvalid;

    public LabelFilePicker()
    {
        InitializeComponent();
    }

    private void OnValueChanged(
        object sender,
        RoutedPropertyChangedEventArgs<FileInfo?> e)
    {
        Validate(this, e, raiseEvents: true);
    }

    private static void Validate(
        LabelFilePicker control,
        RoutedPropertyChangedEventArgs<FileInfo?> e,
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

        if (control.Value is not null && control.AllowOnlyExisting && !File.Exists(control.Value.FullName))
        {
            control.ValidationText = "File don't exist"; // TODO: Translation
            if (raiseEvents)
            {
                OnLostFocusFireInvalidEvent(control, e);
            }

            return;
        }

        OnLostFocusFireValidEvent(control, e);
    }

    private static void OnLostFocusFireValidEvent(
        LabelFilePicker control,
        RoutedPropertyChangedEventArgs<FileInfo?> e)
    {
        control.LostFocusValid?.Invoke(
            control,
            new ChangedFileInfoEventArgs(
                ControlHelper.GetIdentifier(control),
                e.OldValue,
                e.NewValue));
    }

    private static void OnLostFocusFireInvalidEvent(
        LabelFilePicker control,
        RoutedPropertyChangedEventArgs<FileInfo?> e)
    {
        control.LostFocusInvalid?.Invoke(
            control,
            new ChangedFileInfoEventArgs(
                ControlHelper.GetIdentifier(control),
                e.OldValue,
                e.NewValue));
    }
}