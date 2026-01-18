namespace Atc.Wpf.Forms;

public partial class LabelEndpointBox : ILabelEndpointBox
{
    public bool IsDirty { get; private set; }

    [DependencyProperty(
        DefaultValue = NetworkProtocolType.Https,
        PropertyChangedCallback = nameof(OnNetworkProtocolLostFocus),
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal)]
    private NetworkProtocolType networkProtocol;

    [DependencyProperty(
        DefaultValue = "",
        PropertyChangedCallback = nameof(OnHostLostFocus),
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal)]
    private string host;

    [DependencyProperty(
        DefaultValue = 80,
        PropertyChangedCallback = nameof(OnPortLostFocus),
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal)]
    private int port;

    [DependencyProperty(DefaultValue = "localhost")]
    private string watermarkText;

    [DependencyProperty(DefaultValue = true)]
    private bool showClearTextButton;

    [DependencyProperty(DefaultValue = true)]
    private bool hideUpDownButtons;

    [DependencyProperty(
        DefaultValue = NetworkValidationRule.None,
        PropertyChangedCallback = nameof(OnNetworkValidationChanged))]
    private NetworkValidationRule networkValidation;

    [DependencyProperty(
        DefaultValue = 1,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal)]
    private int minimumPort;

    [DependencyProperty(
        DefaultValue = 65535,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal)]
    private int maximumPort;

    [DependencyProperty(
        PropertyChangedCallback = nameof(OnValueLostFocus),
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal)]
    private Uri? value;

    public event EventHandler<ValueChangedEventArgs<NetworkProtocolType?>>? NetworkProtocolLostFocus;

    public event EventHandler<ValueChangedEventArgs<string?>>? HostLostFocus;

    public event EventHandler<ValueChangedEventArgs<int?>>? PortLostFocus;

    public event EventHandler<ValueChangedEventArgs<Uri?>>? ValueLostFocus;

    public LabelEndpointBox()
    {
        InitializeComponent();
    }

    public override bool IsValid()
    {
        ValidateEndpoint();
        return string.IsNullOrEmpty(ValidationText);
    }

    private void ValidateEndpoint()
    {
        if (IsMandatory && string.IsNullOrWhiteSpace(Host))
        {
            ValidationText = IsDirty
                ? Validations.FieldIsRequired
                : string.Empty;
            return;
        }

        if (Port < MinimumPort || Port > MaximumPort)
        {
            ValidationText = string.Format(
                CultureInfo.InvariantCulture,
                "Port must be between {0} and {1}",
                MinimumPort,
                MaximumPort);
            return;
        }

        var (isValid, errorMessage) = TextBoxValidationHelper.Validate(
            EndpointBoxValidationHelper.MapNetworkValidationRuleToValidationType(NetworkValidation),
            Host,
            allowNull: false);

        if (!isValid)
        {
            ValidationText = errorMessage;
            return;
        }

        ValidationText = string.Empty;
    }

    private static void OnNetworkValidationChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelEndpointBox)d;
        control.ValidateEndpoint();
    }

    private static void OnNetworkProtocolLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelEndpointBox)d;

        if (e.NewValue is not NetworkProtocolType newValue)
        {
            return;
        }

        if (e.OldValue is not NetworkProtocolType oldValue)
        {
            return;
        }

        control.NetworkProtocolLostFocus?.Invoke(
            control,
            new ValueChangedEventArgs<NetworkProtocolType?>(
                control.Identifier,
                oldValue,
                newValue));

        control.ValidateEndpoint();
    }

    private static void OnHostLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelEndpointBox)d;

        if (e.NewValue is not string newValue)
        {
            return;
        }

        if (e.OldValue is not string oldValue)
        {
            return;
        }

        control.IsDirty = true;

        control.HostLostFocus?.Invoke(
            control,
            new ValueChangedEventArgs<string?>(
                control.Identifier,
                oldValue,
                newValue));

        control.ValidateEndpoint();
    }

    private static void OnPortLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelEndpointBox)d;

        if (e.NewValue is not int newValue)
        {
            control.ValidationText = Validations.ValueShouldBeAInteger;
            return;
        }

        if (e.OldValue is not int oldValue)
        {
            return;
        }

        control.PortLostFocus?.Invoke(
            control,
            new ValueChangedEventArgs<int?>(
                control.Identifier,
                oldValue,
                newValue));

        control.ValidateEndpoint();
    }

    private static void OnValueLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelEndpointBox)d;

        var oldValue = e.OldValue as Uri;
        var newValue = e.NewValue as Uri;

        control.ValueLostFocus?.Invoke(
            control,
            new ValueChangedEventArgs<Uri?>(
                control.Identifier,
                oldValue,
                newValue));

        control.ValidateEndpoint();
    }
}