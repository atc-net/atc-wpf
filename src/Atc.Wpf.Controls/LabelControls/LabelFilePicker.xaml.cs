// ReSharper disable InvertIf
namespace Atc.Wpf.Controls.LabelControls;

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

    public static readonly DependencyProperty WatermarkTextProperty = DependencyProperty.Register(
        nameof(WatermarkText),
        typeof(string),
        typeof(LabelFilePicker),
        new PropertyMetadata(defaultValue: string.Empty));

    public string WatermarkText
    {
        get => (string)GetValue(WatermarkTextProperty);
        set => SetValue(WatermarkTextProperty, value);
    }

    public static readonly DependencyProperty WatermarkAlignmentProperty = DependencyProperty.Register(
        nameof(WatermarkAlignment),
        typeof(TextAlignment),
        typeof(LabelFilePicker),
        new PropertyMetadata(default(TextAlignment)));

    public TextAlignment WatermarkAlignment
    {
        get => (TextAlignment)GetValue(WatermarkAlignmentProperty);
        set => SetValue(WatermarkAlignmentProperty, value);
    }

    public static readonly DependencyProperty WatermarkTrimmingProperty = DependencyProperty.Register(
        nameof(WatermarkTrimming),
        typeof(TextTrimming),
        typeof(LabelFilePicker),
        new PropertyMetadata(default(TextTrimming)));

    public TextTrimming WatermarkTrimming
    {
        get => (TextTrimming)GetValue(WatermarkTrimmingProperty);
        set => SetValue(WatermarkTrimmingProperty, value);
    }

    public event EventHandler<ValueChangedEventArgs<FileInfo?>>? LostFocusValid;

    public event EventHandler<ValueChangedEventArgs<FileInfo?>>? LostFocusInvalid;

    public LabelFilePicker()
    {
        InitializeComponent();
    }

    public override bool IsValid()
    {
        Validate(this, default!, raiseEvents: false);
        return string.IsNullOrEmpty(ValidationText);
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
            control.ValidationText = Validations.FileDoNotExist;
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
            new ValueChangedEventArgs<FileInfo?>(
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
            new ValueChangedEventArgs<FileInfo?>(
                ControlHelper.GetIdentifier(control),
                e.OldValue,
                e.NewValue));
    }
}