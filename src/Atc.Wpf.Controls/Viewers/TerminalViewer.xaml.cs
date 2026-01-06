namespace Atc.Wpf.Controls.Viewers;

public sealed partial class TerminalViewer : IDisposable
{
    private readonly Channel<TerminalReceivedDataEventArgs> receivedDataChannel =
        Channel.CreateUnbounded<TerminalReceivedDataEventArgs>(
            new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false,
                AllowSynchronousContinuations = false,
            });

    private readonly CancellationTokenSource cts = new();
    private readonly Task? queueProcessingTask;

    public TerminalViewer()
    {
        InitializeComponent();

        DataContext = this;

        Messenger.Default.Register<TerminalReceivedDataEventArgs>(
            this,
            TerminalReceivedDataHandle);
        Messenger.Default.Register<TerminalClearEventArgs>(
            this,
            TerminalClearEventArgsHandle);

        queueProcessingTask = Task.Run(
            () => ProcessQueueContinuously(cts.Token),
            cts.Token);
    }

    [DependencyProperty(DefaultValue = "Black")]
    private Brush terminalBackground;

    [DependencyProperty(DefaultValue = "Consolas")]
    private FontFamily terminalFontFamily;

    [DependencyProperty(DefaultValue = "12.0")]
    private double terminalFontSize;

    [DependencyProperty(DefaultValue = "Teal")]
    private Brush defaultTextColor;

    [DependencyProperty(DefaultValue = "Red")]
    private Brush errorTextColor;

    [DependencyProperty]
    private IList<string>? termsError;

    [DependencyProperty(DefaultValue = "LimeGreen")]
    private Brush successfulTextColor;

    [DependencyProperty]
    private IList<string>? termsSuccessful;

    [DependencyProperty(DefaultValue = "Chocolate")]
    private Brush terms1TextColor;

    [DependencyProperty]
    private IList<string>? terms1;

    [DependencyProperty(DefaultValue = "DarkOrange")]
    private Brush terms2TextColor;

    [DependencyProperty]
    private IList<string>? terms2;

    [DependencyProperty(DefaultValue = "CornflowerBlue")]
    private Brush terms3TextColor;

    [DependencyProperty]
    private IList<string>? terms3;

    public void Dispose()
    {
        cts.Dispose();
        queueProcessingTask?.Dispose();
    }

    private bool CanExecuteHasItems()
        => ListViewTerminal.Items.Count > 0;

    [RelayCommand(CanExecute = nameof(CanExecuteHasItems))]
    private void CopyToClipboard()
    {
        var sb = new StringBuilder();
        foreach (var item in ListViewTerminal.Items)
        {
            var terminalLineItem = (TerminalLineItem)item;
            sb.AppendLine(terminalLineItem.Text);
        }

        Clipboard.SetText(sb.ToString());
    }

    [RelayCommand(CanExecute = nameof(CanExecuteHasItems))]
    private void ClearScreen()
        => ListViewTerminal.Items.Clear();

    private void TerminalReceivedDataHandle(TerminalReceivedDataEventArgs obj)
        => receivedDataChannel.Writer.TryWrite(obj);

    private void TerminalClearEventArgsHandle(TerminalClearEventArgs obj)
    {
        while (receivedDataChannel.Reader.TryRead(out _))
        {
            // Skip, just clear receivedDataChannel
        }

        ListViewTerminal.Dispatcher.Invoke(
            () => ListViewTerminal.Items.Clear(),
            DispatcherPriority.Render,
            CancellationToken.None);
    }

    private async Task ProcessQueueContinuously(CancellationToken token)
    {
        var reader = receivedDataChannel.Reader;

        while (await reader.WaitToReadAsync(token))
        {
            var batch = new List<TerminalReceivedDataEventArgs>();

            while (reader.TryRead(out var data))
            {
                batch.Add(data);
            }

            if (batch.Count == 0)
            {
                continue;
            }

            await ListViewTerminal.Dispatcher.InvokeAsync(
                () =>
                {
                    foreach (var line in batch.SelectMany(e => e.Lines))
                    {
                        var item = new TerminalLineItem(
                            line,
                            GetColorForLine(line));
                        ListViewTerminal.Items.Add(item);
                    }

                    ListViewTerminal.UpdateLayout();
                },
                DispatcherPriority.Render,
                token);

            await ListViewTerminal.Dispatcher.InvokeAsync(
                () =>
                {
                    var scrollViewer = VisualTreeHelperEx.FindChild<ScrollViewer>(ListViewTerminal);

                    if (scrollViewer is null)
                    {
                        ListViewTerminal.ScrollIntoView(ListViewTerminal.Items[^1]!);
                    }
                    else
                    {
                        scrollViewer.ScrollToBottom();
                    }
                },
                DispatcherPriority.Background,
                token);
        }
    }

    private Brush GetColorForLine(string line)
    {
        var lineSpan = line.AsSpan();

        var checks = new (
            IEnumerable<string>? Terms,
            Brush Colour,
            StringComparison Comparison)[]
            {
                (TermsError,
                    ErrorTextColor,
                    StringComparison.OrdinalIgnoreCase),
                (TermsSuccessful,
                    SuccessfulTextColor,
                    StringComparison.OrdinalIgnoreCase),
                (Terms1,
                    Terms1TextColor,
                    StringComparison.OrdinalIgnoreCase),
                (Terms2,
                    Terms2TextColor,
                    StringComparison.OrdinalIgnoreCase),
                (Terms3,
                    Terms3TextColor,
                    StringComparison.OrdinalIgnoreCase),
            };

        foreach (var (terms, colour, cmp) in checks)
        {
            if (terms is null)
            {
                continue;
            }

            foreach (var term in terms)
            {
                if (ContainsTermAsWord(
                        lineSpan,
                        term.AsSpan(),
                        cmp))
                {
                    return colour;
                }
            }
        }

        return DefaultTextColor;
    }

    private static bool ContainsTermAsWord(
        ReadOnlySpan<char> line,
        ReadOnlySpan<char> term,
        StringComparison comparison)
    {
        int index;
        while ((index = line.IndexOf(
                   term,
                   comparison)) >= 0)
        {
            var atStart = index == 0 || char.IsWhiteSpace(line[index - 1]);
            var end = index + term.Length;
            var atEnd = end == line.Length || (end < line.Length && char.IsWhiteSpace(line[end]));

            if (atStart && atEnd)
            {
                return true;
            }

            line = line[(index + 1)..];
        }

        return false;
    }
}