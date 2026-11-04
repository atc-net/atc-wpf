namespace Atc.Wpf.Components.Tests.Dialogs;

/// <summary>
/// <see cref="DialogBoxHelper.ApplyAutoHeight"/> keeps the set height for short text, grows the window for wrapped
/// text and stops at a share of the work area, where the text scrolls.
/// </summary>
public sealed class DialogBoxHelperAutoHeightTests
{
    private const double MinimumHeight = 200;
    private const double Width = 350;

    [StaFact]
    public void ApplyAutoHeight_SizesTheHeightToContent_BetweenTheMinimumAndTheWorkAreaShare()
    {
        var window = new Window { Width = Width, Height = MinimumHeight };

        DialogBoxHelper.ApplyAutoHeight(window, owningWindow: null, MinimumHeight);

        Assert.Equal(SizeToContent.Height, window.SizeToContent);
        Assert.True(double.IsNaN(window.Height));
        Assert.Equal(Width, window.Width);
        Assert.Equal(MinimumHeight, window.MinHeight);
        Assert.Equal(
            System.Math.Max(MinimumHeight, SystemParameters.WorkArea.Height * DialogBoxHelper.MaxWorkAreaHeightRatio),
            window.MaxHeight,
            precision: 1);
    }

    [StaFact]
    public void HeightSetAfterwards_KeepsTheWindowAtThatHeight()
    {
        var (window, _) = CreateDialogLikeWindow("Are you sure to delete item?", show: false);
        try
        {
            window.Height = 300;
            window.Show();
            window.UpdateLayout();

            Assert.Equal(300, window.ActualHeight, precision: 1);
        }
        finally
        {
            window.Close();
        }
    }

    [StaFact]
    public void ShortText_KeepsTheMinimumHeight()
    {
        var (window, viewer) = CreateDialogLikeWindow("Are you sure to delete item?");
        try
        {
            Assert.Equal(MinimumHeight, window.ActualHeight, precision: 1);
            Assert.NotEqual(Visibility.Visible, viewer.ComputedVerticalScrollBarVisibility);
        }
        finally
        {
            window.Close();
        }
    }

    [StaFact]
    public void WrappedText_GrowsTheWindow_WithoutScrolling()
    {
        var text = string.Join(' ', Enumerable.Repeat("This sentence is repeated to give a few wrapped lines.", 6));
        var (window, viewer) = CreateDialogLikeWindow(text);
        try
        {
            Assert.True(window.ActualHeight > MinimumHeight, $"Height {window.ActualHeight} should grow above {MinimumHeight}.");
            Assert.True(window.ActualHeight < window.MaxHeight, $"Height {window.ActualHeight} should stay below {window.MaxHeight}.");
            Assert.Equal(Width, window.ActualWidth, precision: 1);
            Assert.Equal(0, viewer.ScrollableHeight);
        }
        finally
        {
            window.Close();
        }
    }

    [StaFact]
    public void VeryLongText_StopsAtTheMaximumHeight_AndScrolls()
    {
        var text = string.Join('\n', Enumerable.Repeat("A line.", 400));
        var (window, viewer) = CreateDialogLikeWindow(text);
        try
        {
            Assert.Equal(window.MaxHeight, window.ActualHeight, precision: 1);
            Assert.True(viewer.ScrollableHeight > 0);
            Assert.Equal(0, viewer.ScrollableWidth);
        }
        finally
        {
            window.Close();
        }
    }

    private static (Window Window, ScrollViewer Viewer) CreateDialogLikeWindow(
        string text,
        bool show = true)
    {
        var viewer = new ScrollViewer
        {
            Padding = new Thickness(30),
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            Content = DialogBoxHelper.CreateContentControl(text, new Border { Width = 32, Height = 32 }),
        };

        var buttonBar = new Border { Height = 50 };
        DockPanel.SetDock(buttonBar, Dock.Bottom);
        var dockPanel = new DockPanel();
        dockPanel.Children.Add(buttonBar);
        dockPanel.Children.Add(viewer);

        var window = new DialogLikeWindow
        {
            Width = Width,
            Height = MinimumHeight,
            Left = -10000,
            Top = -10000,
            WindowStartupLocation = WindowStartupLocation.Manual,
            WindowStyle = WindowStyle.None,
            ResizeMode = ResizeMode.NoResize,
            ShowInTaskbar = false,
            ShowActivated = false,
            Content = dockPanel,
        };

        DialogBoxHelper.ApplyAutoHeight(window, owningWindow: null, MinimumHeight);
        if (show)
        {
            window.Show();
            window.UpdateLayout();
        }

        return (window, viewer);
    }

    /// <summary>
    /// Forwards property changes to the helper, as the information and question dialogs do.
    /// </summary>
    private sealed class DialogLikeWindow : Window
    {
        protected override void OnPropertyChanged(
        DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            DialogBoxHelper.OnDialogPropertyChanged(this, e);
        }
    }
}