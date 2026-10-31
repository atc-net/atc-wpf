namespace Atc.Wpf.Components.Tests.Monitoring;

public sealed class ApplicationMonitorViewViewModelTests
{
    [StaFact]
    public void DataContext_ViewModelSettings_AreKeptAndShownByTheView()
    {
        using var viewModel = new ApplicationMonitorViewModel
        {
            ListenOnToastNotificationMessage = false,
            MaxEntries = 500,
            AutoScroll = false,
            IsPaused = true,
        };

        var view = new ApplicationMonitorView { DataContext = viewModel };

        Assert.Equal(500, viewModel.MaxEntries);
        Assert.False(viewModel.AutoScroll);
        Assert.True(viewModel.IsPaused);
        Assert.Equal(500, view.MaxEntries);
        Assert.False(view.AutoScroll);
        Assert.True(view.IsPaused);
    }

    [StaFact]
    public void DataContext_ValuesSetOnTheView_ArePushedToTheViewModel()
    {
        using var viewModel = new ApplicationMonitorViewModel { ListenOnToastNotificationMessage = false };

        var view = new ApplicationMonitorView
        {
            MaxEntries = 200,
        };
        view.DataContext = viewModel;

        Assert.Equal(200, viewModel.MaxEntries);
        Assert.True(viewModel.AutoScroll);
    }

    [StaFact]
    public void DataContext_Swapped_TheNewViewModelKeepsItsSettings()
    {
        using var first = new ApplicationMonitorViewModel { ListenOnToastNotificationMessage = false, MaxEntries = 300 };
        using var second = new ApplicationMonitorViewModel { ListenOnToastNotificationMessage = false, MaxEntries = 700 };
        var view = new ApplicationMonitorView { DataContext = first };

        view.DataContext = second;

        Assert.Equal(300, first.MaxEntries);
        Assert.Equal(700, second.MaxEntries);
        Assert.Equal(700, view.MaxEntries);
    }

    [StaFact]
    public void AddEntry_RaisesScrollRequestedOnlyOnThatViewModel()
    {
        using var first = new ApplicationMonitorViewModel { ListenOnToastNotificationMessage = false };
        using var second = new ApplicationMonitorViewModel { ListenOnToastNotificationMessage = false };
        var firstRaised = 0;
        var secondRaised = 0;
        first.ScrollRequested += (_, _) => firstRaised++;
        second.ScrollRequested += (_, _) => secondRaised++;

        first.AddEntry(new ApplicationEventEntry("Test", "message"));
        DispatcherPump.Until(() => first.Entries.Count > 0);

        Assert.Single(first.Entries);
        Assert.Equal(1, firstRaised);
        Assert.Equal(0, secondRaised);
    }
}