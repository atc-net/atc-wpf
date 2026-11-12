namespace Atc.Wpf.Network.Tests.Onvif;

public sealed class OnvifCameraViewModelTests
{
    private static readonly Uri DeviceServiceUrl = new("http://192.168.1.132:2020/onvif/device_service");

    private static readonly OnvifStreamInfo MainStream = new(
        new OnvifMediaProfile("profile_1", "mainStream", "H264", 2304, 1296, 15),
        new Uri("rtsp://192.168.1.132:554/stream1"));

    private static readonly OnvifStreamInfo SubStream = new(
        new OnvifMediaProfile("profile_2", "minorStream", "H264", 640, 360, 15),
        new Uri("rtsp://192.168.1.132:554/stream2"));

    [Fact]
    public async Task Read_FillsTheDeviceInformationAndStreams()
    {
        var reader = new FakeOnvifCameraReader();
        var sut = CreateSut(reader);

        await sut.ReadCommand.ExecuteAsync(null);

        Assert.Equal(DeviceServiceUrl, reader.ReadUrl);
        Assert.Equal("onvif-user", reader.ReadCredential?.UserName);
        Assert.Equal("secret", reader.ReadCredential?.Password);
        Assert.Equal("TP-Link", sut.Manufacturer);
        Assert.Equal("C210", sut.Model);
        Assert.Equal("1.3.4", sut.FirmwareVersion);
        Assert.Equal("SN1", sut.SerialNumber);
        Assert.Equal([MainStream, SubStream], sut.Streams);
        Assert.Same(MainStream, sut.SelectedStream);
        Assert.False(sut.HasError);
    }

    [Fact]
    public async Task Read_WithWrongCredentials_SaysSo()
    {
        var reader = new FakeOnvifCameraReader
        {
            Exception = new OnvifException("Not authorized", HttpStatusCode.Unauthorized, code: null, subcode: null, reason: null),
        };
        var sut = CreateSut(reader);

        await sut.ReadCommand.ExecuteAsync(null);

        Assert.Equal(Atc.Wpf.Network.Resources.Resources.OnvifNotAuthorized, sut.ErrorMessage);
        Assert.Empty(sut.Streams);
    }

    [Fact]
    public async Task Read_WhenTheCameraDoesNotAnswer_SaysSo()
    {
        var reader = new FakeOnvifCameraReader { Exception = new TimeoutException() };
        var sut = CreateSut(reader);

        await sut.ReadCommand.ExecuteAsync(null);

        Assert.Equal(Atc.Wpf.Network.Resources.Resources.OnvifTimeout, sut.ErrorMessage);
    }

    [Fact]
    public async Task Read_Again_ClearsTheEarlierResult()
    {
        var reader = new FakeOnvifCameraReader();
        var sut = CreateSut(reader);
        await sut.ReadCommand.ExecuteAsync(null);
        reader.Exception = new TimeoutException();

        await sut.ReadCommand.ExecuteAsync(null);

        Assert.Null(sut.Model);
        Assert.Empty(sut.Streams);
        Assert.Null(sut.SelectedStream);
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("not a url", false)]
    [InlineData("rtsp://192.168.1.132/stream1", false)]
    [InlineData("http://192.168.1.132:2020/onvif/device_service", true)]
    [InlineData("https://cam.local/onvif/device_service", true)]
    public void Read_NeedsAnHttpDeviceServiceUrl(
        string? text,
        bool expected)
    {
        var sut = new OnvifCameraViewModel(new FakeOnvifCameraReader()) { DeviceServiceUrl = text };

        Assert.Equal(expected, sut.ReadCommand.CanExecute(null));
    }

    [Fact]
    public async Task UseStream_RaisesStreamSelectedWithTheStreamAndCredential()
    {
        var sut = CreateSut(new FakeOnvifCameraReader());
        await sut.ReadCommand.ExecuteAsync(null);
        sut.SelectedStream = SubStream;
        OnvifStreamSelectedEventArgs? args = null;
        sut.StreamSelected += (_, e) => args = e;

        sut.UseStreamCommand.Execute(null);

        Assert.Same(SubStream, args!.Stream);
        Assert.Equal(DeviceServiceUrl, args.DeviceServiceUrl);
        Assert.Equal("onvif-user", args.Credential.UserName);
        Assert.Equal("secret", args.Credential.Password);
        Assert.Equal("C210", args.Model);
    }

    [Fact]
    public void UseStream_WithoutAStream_IsDisabled()
    {
        var sut = CreateSut(new FakeOnvifCameraReader());

        Assert.False(sut.UseStreamCommand.CanExecute(null));
    }

    [Fact]
    public void FromHost_TakesTheOnvifDeviceServiceUrl()
    {
        var result = new IPScanResult(IPAddress.Parse("192.168.1.132"));
        result.OnvifDevices.Add(new OnvifDevice(
            IPAddress.Parse("192.168.1.132"),
            "urn:uuid:1",
            ["dn:NetworkVideoTransmitter"],
            [],
            [DeviceServiceUrl]));
        var sut = new OnvifCameraViewModel(new FakeOnvifCameraReader());

        sut.UseHost(new NetworkHostViewModel(result));

        Assert.Equal(DeviceServiceUrl.ToString(), sut.DeviceServiceUrl);
    }

    private static OnvifCameraViewModel CreateSut(IOnvifCameraReader reader)
        => new(reader)
        {
            DeviceServiceUrl = DeviceServiceUrl.ToString(),
            UserName = "onvif-user",
            Password = "secret",
        };

    private sealed class FakeOnvifCameraReader : IOnvifCameraReader
    {
        public Exception? Exception { get; set; }

        public Uri? ReadUrl { get; private set; }

        public NetworkCredential? ReadCredential { get; private set; }

        public Task<OnvifCameraDetails> Read(
            Uri deviceServiceUrl,
            NetworkCredential credential,
            CancellationToken cancellationToken)
        {
            ReadUrl = deviceServiceUrl;
            ReadCredential = credential;
            if (Exception is not null)
            {
                return Task.FromException<OnvifCameraDetails>(Exception);
            }

            return Task.FromResult(new OnvifCameraDetails(
                new OnvifDeviceInformation("TP-Link", "C210", "1.3.4", "SN1", HardwareId: null),
                [MainStream, SubStream],
                []));
        }
    }
}