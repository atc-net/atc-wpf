namespace Atc.Wpf.Sample.SamplesWpfComponents.Viewers;

public partial class TerminalViewerView
{
    private static readonly string[] AnsiSampleLines =
    [
        "[31mAn ANSI red error line[0m",
        "[1;32mBold green success[0m mixed with [33myellow[0m and [36mcyan[0m",
        "[4;35mUnderlined magenta[0m, [1mbold default[22m, [3mitalic default[0m",
        "Plain default-coloured line",
    ];

    private readonly DispatcherTimer dispatcherTimer;
    private readonly TerminalViewerDemoViewModel viewModel;
    private int counter;

    public TerminalViewerView()
    {
        InitializeComponent();

        viewModel = new TerminalViewerDemoViewModel();
        viewModel.EnableTimerChanged += OnEnableTimerChanged;

        DataContext = viewModel;

        dispatcherTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(300),
        };
        dispatcherTimer.Tick += TimerTick;
    }

    private void OnEnableTimerChanged(
        object? sender,
        bool enabled)
    {
        if (enabled)
        {
            dispatcherTimer.Start();
        }
        else
        {
            dispatcherTimer.Stop();
        }
    }

    private void TimerTick(
        object? sender,
        EventArgs e)
    {
        counter++;
        Messenger.Default.Send(
            new TerminalReceivedDataEventArgs([
                $"tick {counter:D5} – {DateTime.Now:HH:mm:ss.fff}",
            ]));
    }

    private void SendLineOnClick(
        object sender,
        RoutedEventArgs e)
    {
        Messenger.Default.Send(
            new TerminalReceivedDataEventArgs(["hello world"]));
    }

    private void SendLineWithErrorOnClick(
        object sender,
        RoutedEventArgs e)
    {
        Messenger.Default.Send(
            new TerminalReceivedDataEventArgs(["error"]));
    }

    private void SendLineWithSuccessfulOnClick(
        object sender,
        RoutedEventArgs e)
    {
        Messenger.Default.Send(
            new TerminalReceivedDataEventArgs(["100%"]));
    }

    private void SendLineWithOneOfTerms1OnClick(
        object sender,
        RoutedEventArgs e)
    {
        Messenger.Default.Send(
            new TerminalReceivedDataEventArgs(["+ xxx"]));
    }

    private void SendLineWithOneOfTerms2OnClick(
        object sender,
        RoutedEventArgs e)
    {
        Messenger.Default.Send(
            new TerminalReceivedDataEventArgs(["[sudo] xxx"]));
    }

    private void SendLineWithOneOfTerms3OnClick(
        object sender,
        RoutedEventArgs e)
    {
        Messenger.Default.Send(
            new TerminalReceivedDataEventArgs(["installed"]));
    }

    private void SendAnsiOnClick(
        object sender,
        RoutedEventArgs e)
    {
        Messenger.Default.Send(
            new TerminalReceivedDataEventArgs(AnsiSampleLines));
    }

    private void SendBurstOnClick(
        object sender,
        RoutedEventArgs e)
    {
        var lines = new string[100];
        for (var i = 0; i < lines.Length; i++)
        {
            counter++;
            lines[i] = $"burst {counter:D5} – {Guid.NewGuid():N}";
        }

        Messenger.Default.Send(
            new TerminalReceivedDataEventArgs(lines));
    }
}
