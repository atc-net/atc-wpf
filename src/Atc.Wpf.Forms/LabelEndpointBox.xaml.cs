namespace Atc.Wpf.Forms;

public partial class LabelEndpointBox : ILabelEndpointBox
{
    public bool IsDirty { get; private set; }

    [DependencyProperty(
        DefaultValue = NetworkProtocolType.Https,
        PropertyChangedCallback = nameof(OnNetworkProtocolChanged),
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal)]
    private NetworkProtocolType networkProtocol;

    [DependencyProperty(
        DefaultValue = "",
        PropertyChangedCallback = nameof(OnHostChanged),
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal)]
    private string host;

    [DependencyProperty(
        DefaultValue = 80,
        PropertyChangedCallback = nameof(OnPortChanged),
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
        PropertyChangedCallback = nameof(OnValueChanged),
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal)]
    private Uri? value;

    public event EventHandler<ValueChangedEventArgs<NetworkProtocolType?>>? NetworkProtocolLostFocus;

    public event EventHandler<ValueChangedEventArgs<string?>>? HostLostFocus;

    public event EventHandler<ValueChangedEventArgs<int?>>? PortLostFocus;

    public event EventHandler<ValueChangedEventArgs<Uri?>>? ValueLostFocus;

    public LabelEndpointBox()
    {
        InitializeComponent();
        InnerEndpointBox = (EndpointBox)((LabelContent)Content).Content;

        // The inner box raises these when one of its editors loses focus, not on every change.
        InnerEndpointBox.NetworkProtocolLostFocus += (_, e) => NetworkProtocolLostFocus?.Invoke(
            this,
            new ValueChangedEventArgs<NetworkProtocolType?>(Identifier, e.OldValue, e.NewValue));
        InnerEndpointBox.HostLostFocus += (_, e) => HostLostFocus?.Invoke(
            this,
            new ValueChangedEventArgs<string?>(Identifier, e.OldValue, e.NewValue));
        InnerEndpointBox.PortLostFocus += (_, e) => PortLostFocus?.Invoke(
            this,
            new ValueChangedEventArgs<int?>(Identifier, e.OldValue, e.NewValue));
        InnerEndpointBox.ValueLostFocus += (_, e) => ValueLostFocus?.Invoke(
            this,
            new ValueChangedEventArgs<Uri?>(Identifier, e.OldValue, e.NewValue));
    }

    internal EndpointBox InnerEndpointBox { get; }

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

    private static void OnNetworkProtocolChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelEndpointBox)d;

        if (e.NewValue is not NetworkProtocolType ||
            e.OldValue is not NetworkProtocolType)
        {
            return;
        }

        control.ValidateEndpoint();
    }

    private static void OnHostChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelEndpointBox)d;

        if (e.NewValue is not string ||
            e.OldValue is not string)
        {
            return;
        }

        control.IsDirty = true;
        control.ValidateEndpoint();
    }

    private static void OnPortChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelEndpointBox)d;

        if (e.NewValue is not int)
        {
            control.ValidationText = Validations.ValueShouldBeAInteger;
            return;
        }

        if (e.OldValue is not int)
        {
            return;
        }

        control.ValidateEndpoint();
    }

    private static void OnValueChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelEndpointBox)d;
        control.ValidateEndpoint();
    }
}