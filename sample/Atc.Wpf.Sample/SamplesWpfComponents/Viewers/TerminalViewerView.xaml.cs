namespace Atc.Wpf.Sample.SamplesWpfComponents.Viewers;

public partial class TerminalViewerView
{
    public TerminalViewerView()
    {
        InitializeComponent();

        DataContext = this;
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
}