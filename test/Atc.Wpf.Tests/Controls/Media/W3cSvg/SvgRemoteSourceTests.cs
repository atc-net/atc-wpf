namespace Atc.Wpf.Tests.Controls.Media.W3cSvg;

// An SVG source on http(s) must never make the control open a network connection.
public sealed class SvgRemoteSourceTests : IDisposable
{
    private readonly System.Net.Sockets.TcpListener listener = new(System.Net.IPAddress.Loopback, 0);
    private readonly CancellationTokenSource cts = new();
    private int connections;

    public SvgRemoteSourceTests()
    {
        listener.Start();

        // Count and drop every connection, so code that does connect fails fast instead of waiting.
        _ = Task.Run(AcceptAndDropAsync, cts.Token);
    }

    private string Url
        => $"http://127.0.0.1:{((System.Net.IPEndPoint)listener.LocalEndpoint).Port}/icon.svg";

    public void Dispose()
    {
        cts.Cancel();
        listener.Dispose();
        cts.Dispose();
    }

    private async Task AcceptAndDropAsync()
    {
        while (!cts.IsCancellationRequested)
        {
            try
            {
                using var client = await listener.AcceptTcpClientAsync(cts.Token).ConfigureAwait(false);
                Interlocked.Increment(ref connections);
            }
            catch (Exception ex) when (ex is OperationCanceledException or System.Net.Sockets.SocketException)
            {
                return;
            }
        }
    }

    [Fact]
    public void Svg_Load_with_an_http_uri_is_not_supported()
    {
        var svg = new Svg(new FileSystemLoader());

        Assert.Throws<NotSupportedException>(() => svg.Load(new Uri(Url)));
        Assert.Equal(0, Volatile.Read(ref connections));
    }

    // A file path that is not an existing file loads nothing; an http string must stay on that path.
    [Fact]
    public void Svg_Load_with_an_http_path_loads_nothing_without_a_request()
    {
        var svg = new Svg(Url, new FileSystemLoader());

        Assert.Equal(string.Empty, svg.Filename);
        Assert.Equal(0, Volatile.Read(ref connections));
    }

    [StaFact]
    public void SvgIcon_with_an_http_source_renders_nothing_without_a_request()
    {
        var sut = new TestSvgIcon();

        var drawing = sut.Render(new Uri(Url));

        Assert.Null(drawing);
        Assert.Equal(0, Volatile.Read(ref connections));
    }

    private sealed class TestSvgIcon : SvgIconBase
    {
        public DrawingGroup? Render(Uri source)
        {
            try
            {
                return GetDrawing(source);
            }
            catch (Exception ex) when (ex is not NotSupportedException)
            {
                // Today's code throws after it has connected; the connection count is what the test checks.
                return null;
            }
        }

        public override object? ProvideValue(IServiceProvider serviceProvider)
            => null;
    }
}