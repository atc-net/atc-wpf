namespace Atc.Wpf.Components.Tests.Monitoring;

public sealed class ApplicationMonitorViewModelDisposeTests
{
    [Fact]
    public void Dispose_CalledTwice_DoesNotThrow()
    {
        var viewModel = new ApplicationMonitorViewModel();

        viewModel.Dispose();
        var secondDispose = Record.Exception(viewModel.Dispose);

        Assert.Null(secondDispose);
    }
}