// ReSharper disable InvertIf
namespace Atc.Wpf.Controls.LabelControls;

[SuppressMessage("Naming", "CA1721:Property names should not match get methods", Justification = "OK.")]
public partial class LabelFilePicker : ILabelFilePicker
{
    [DependencyProperty]
    private FileInfo? value;

    [DependencyProperty(DefaultValue = false)]
    private bool showClearTextButton;

    [DependencyProperty(DefaultValue = false)]
    private bool allowOnlyExisting;

    [DependencyProperty(DefaultValue = "")]
    private string filter;

    [DependencyProperty(DefaultValue = false)]
    private bool usePreviewPane;

    [DependencyProperty(DefaultValue = "")]
    private string defaultDirectory;

    [DependencyProperty(DefaultValue = "")]
    private string initialDirectory;

    [DependencyProperty(DefaultValue = "")]
    private string rootDirectory;

    [DependencyProperty(DefaultValue = "")]
    private string watermarkText;

    [DependencyProperty(DefaultValue = TextAlignment.Left)]
    private TextAlignment watermarkAlignment;

    [DependencyProperty(DefaultValue = TextTrimming.None)]
    private TextTrimming watermarkTrimming;

    public event EventHandler<ValueChangedEventArgs<FileInfo?>>? LostFocusValid;

    public event EventHandler<ValueChangedEventArgs<FileInfo?>>? LostFocusInvalid;

    public LabelFilePicker()
    {
        InitializeComponent();
    }

    public override bool IsValid()
    {
        Validate(
            this,
            new RoutedPropertyChangedEventArgs<FileInfo?>(oldValue: null, newValue: Value),
            raiseEvents: false);

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

        if (control.Value is not null &&
            control.AllowOnlyExisting &&
            !File.Exists(control.Value.FullName))
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