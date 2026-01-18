// ReSharper disable InvertIf
namespace Atc.Wpf.Forms;

[SuppressMessage("Naming", "CA1721:Property names should not match get methods", Justification = "OK.")]
public partial class LabelDirectoryPicker : ILabelDirectoryPicker
{
    [DependencyProperty]
    private DirectoryInfo? value;

    [DependencyProperty(DefaultValue = false)]
    private bool showClearTextButton;

    [DependencyProperty(DefaultValue = false)]
    private bool allowOnlyExisting;

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

    public event EventHandler<ValueChangedEventArgs<DirectoryInfo?>>? LostFocusValid;

    public event EventHandler<ValueChangedEventArgs<DirectoryInfo?>>? LostFocusInvalid;

    public LabelDirectoryPicker()
    {
        InitializeComponent();
    }

    public override bool IsValid()
    {
        Validate(
            this,
            new RoutedPropertyChangedEventArgs<DirectoryInfo?>(oldValue: null, newValue: Value),
            raiseEvents: false);

        return string.IsNullOrEmpty(ValidationText);
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
            control.ValidationText = Validations.DirectoryDoNotExist;
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
            new ValueChangedEventArgs<DirectoryInfo?>(
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
            new ValueChangedEventArgs<DirectoryInfo?>(
                ControlHelper.GetIdentifier(control),
                e.OldValue,
                e.NewValue));
    }
}