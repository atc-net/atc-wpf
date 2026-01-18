// ReSharper disable InconsistentNaming
namespace Atc.Wpf.Forms.BaseControls;

public partial class EndpointBox
{
    public bool IsDirty { get; private set; }

    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<NetworkProtocolType>))]
    private static readonly RoutedEvent networkProtocolChanged;

    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<string>))]
    private static readonly RoutedEvent hostChanged;

    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<int>))]
    private static readonly RoutedEvent portChanged;

    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<Uri?>))]
    private static readonly RoutedEvent valueChanged;

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

    public event EventHandler<ValueChangedEventArgs<string?>>? HostLostFocus;

    public event EventHandler<ValueChangedEventArgs<int?>>? PortLostFocus;

    public event EventHandler<ValueChangedEventArgs<NetworkProtocolType?>>? NetworkProtocolLostFocus;

    public event EventHandler<ValueChangedEventArgs<Uri?>>? ValueLostFocus;

    public EndpointBox()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    protected override AutomationPeer OnCreateAutomationPeer()
        => new EndpointBoxAutomationPeer(this);

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
        => UpdateValue();

    private static void OnNetworkProtocolChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (EndpointBox)d;

        if (e.NewValue is not NetworkProtocolType newValue)
        {
            return;
        }

        if (e.OldValue is not NetworkProtocolType oldValue)
        {
            return;
        }

        control.RaiseEvent(
            new RoutedPropertyChangedEventArgs<NetworkProtocolType>(
                oldValue,
                newValue,
                NetworkProtocolChangedEvent));

        control.NetworkProtocolLostFocus?.Invoke(
            control,
            new ValueChangedEventArgs<NetworkProtocolType?>(
                nameof(NetworkProtocol),
                oldValue,
                newValue));

        control.UpdateValue();
    }

    private static void OnHostChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (EndpointBox)d;

        if (e.NewValue is not string newValue)
        {
            return;
        }

        if (e.OldValue is not string oldValue)
        {
            return;
        }

        control.IsDirty = true;

        control.RaiseEvent(
            new RoutedPropertyChangedEventArgs<string>(
                oldValue,
                newValue,
                HostChangedEvent));

        control.HostLostFocus?.Invoke(
            control,
            new ValueChangedEventArgs<string?>(
                nameof(Host),
                oldValue,
                newValue));

        control.UpdateValue();
    }

    private static void OnPortChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (EndpointBox)d;

        if (e.NewValue is not int newValue)
        {
            return;
        }

        if (e.OldValue is not int oldValue)
        {
            return;
        }

        control.RaiseEvent(
            new RoutedPropertyChangedEventArgs<int>(
                oldValue,
                newValue,
                PortChangedEvent));

        control.PortLostFocus?.Invoke(
            control,
            new ValueChangedEventArgs<int?>(
                nameof(Port),
                oldValue,
                newValue));

        control.UpdateValue();
    }

    private static void OnNetworkValidationChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (EndpointBox)d;
        control.UpdateValue();
    }

    private static void OnValueChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (EndpointBox)d;

        if (e.NewValue is Uri newValue)
        {
            control.ParseUriToComponents(newValue);
        }

        var oldUri = e.OldValue as Uri;
        var newUri = e.NewValue as Uri;

        control.RaiseEvent(
            new RoutedPropertyChangedEventArgs<Uri?>(
                oldUri,
                newUri,
                ValueChangedEvent));

        control.ValueLostFocus?.Invoke(
            control,
            new ValueChangedEventArgs<Uri?>(
                nameof(Value),
                oldUri,
                newUri));
    }

    private void UpdateValue()
    {
        if (string.IsNullOrWhiteSpace(Host))
        {
            Value = null;
            return;
        }

        var (isValid, _) = EndpointBoxValidationHelper.Validate(
            NetworkProtocol,
            $"{NetworkProtocolHelper.GetSchemeFromProtocol(NetworkProtocol)}://{Host}",
            allowNull: false);

        if (!isValid)
        {
            Value = null;
            return;
        }

        var (isValidNetworkValidation, _) = TextBoxValidationHelper.Validate(
            EndpointBoxValidationHelper.MapNetworkValidationRuleToValidationType(NetworkValidation),
            Host,
            allowNull: false);

        if (!isValidNetworkValidation)
        {
            Value = null;
            return;
        }

        var scheme = NetworkProtocolHelper.GetSchemeFromProtocol(NetworkProtocol);
        if (string.IsNullOrEmpty(scheme))
        {
            Value = null;
            return;
        }

        try
        {
            var uriBuilder = new UriBuilder
            {
                Scheme = scheme,
                Host = Host.Trim(),
                Port = Port,
            };

            Value = uriBuilder.Uri;
        }
        catch (UriFormatException)
        {
            Value = null;
        }
        catch (ArgumentException)
        {
            Value = null;
        }
    }

    private void ParseUriToComponents(Uri uri)
    {
        if (uri is null)
        {
            return;
        }

        // Temporarily disable property change callbacks to avoid recursion
        Host = uri.Host;
        Port = uri.Port;
        NetworkProtocol = NetworkProtocolHelper.GetProtocolFromScheme(uri.Scheme);
    }
}