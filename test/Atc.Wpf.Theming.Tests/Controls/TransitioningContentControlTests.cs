namespace Atc.Wpf.Theming.Tests.Controls;

/// <summary>
/// <see cref="TransitioningContentControl"/> with a template of the documented structure: the new content goes
/// to the current presenter, the old to the previous presenter, the transition's visual state plays, and when it
/// completes the old content is released.
/// </summary>
public sealed class TransitioningContentControlTests
{
    private const string Template = """
        <ControlTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                         xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                         TargetType="ContentControl">
            <Grid>
                <VisualStateManager.VisualStateGroups>
                    <VisualStateGroup x:Name="PresentationStates">
                        <VisualState x:Name="Hidden" />
                        <VisualState x:Name="DefaultTransition">
                            <Storyboard>
                                <DoubleAnimation Storyboard.TargetName="CurrentContentPresentationSite" Storyboard.TargetProperty="Opacity" From="0" To="1" Duration="0:0:0.05" />
                            </Storyboard>
                        </VisualState>
                        <VisualState x:Name="LeftTransition">
                            <Storyboard>
                                <DoubleAnimation Storyboard.TargetName="CurrentContentPresentationSite" Storyboard.TargetProperty="Opacity" From="0" To="1" Duration="0:0:0.05" />
                            </Storyboard>
                        </VisualState>
                    </VisualStateGroup>
                </VisualStateManager.VisualStateGroups>
                <ContentPresenter x:Name="PreviousContentPresentationSite" />
                <ContentPresenter x:Name="CurrentContentPresentationSite" />
            </Grid>
        </ControlTemplate>
        """;

    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);

    [StaFact]
    public void WithoutATemplate_ChangingContent_DoesNotTransition()
    {
        var sut = new TransitioningContentControl { Content = "first" };

        sut.Content = "second";

        Assert.False(sut.IsTransitioning);
    }

    [StaFact]
    public void ChangingContent_ShowsTheNewContentAndKeepsTheOldWhileTransitioning()
    {
        var sut = Create("first");
        var window = Show(sut);
        try
        {
            sut.Content = "second";

            Assert.True(sut.IsTransitioning);
            Assert.Equal("second", Presenter(sut, "CurrentContentPresentationSite").Content);
            Assert.Equal("first", Presenter(sut, "PreviousContentPresentationSite").Content);
        }
        finally
        {
            window.Close();
        }
    }

    [StaFact]
    public void TransitionCompletes_RaisesTransitionCompleted_AndReleasesTheOldContent()
    {
        var sut = Create("first");
        var completed = 0;
        sut.TransitionCompleted += (_, _) => completed++;
        var window = Show(sut);
        try
        {
            sut.Content = "second";

            Assert.True(WaitUntil(() => completed > 0), "TransitionCompleted was not raised.");
            Assert.Equal(1, completed);
            Assert.False(sut.IsTransitioning);
            Assert.Null(Presenter(sut, "PreviousContentPresentationSite").Content);
            Assert.Equal("second", Presenter(sut, "CurrentContentPresentationSite").Content);
        }
        finally
        {
            window.Close();
        }
    }

    [StaFact]
    public void AbortTransition_StopsAndReleasesTheOldContent()
    {
        var sut = Create("first");
        var window = Show(sut);
        try
        {
            sut.Content = "second";

            sut.AbortTransition();

            Assert.False(sut.IsTransitioning);
            Assert.Null(Presenter(sut, "PreviousContentPresentationSite").Content);
        }
        finally
        {
            window.Close();
        }
    }

    [StaFact]
    public void ChangingTheTransitionWhileTransitioning_AbortsTheRunningTransition()
    {
        var sut = Create("first");
        var window = Show(sut);
        try
        {
            sut.Content = "second";

            sut.Transition = TransitionType.Left;

            Assert.False(sut.IsTransitioning);
            Assert.Equal(TransitionType.Left, sut.Transition);
        }
        finally
        {
            window.Close();
        }
    }

    [StaFact]
    public void ATransitionTheTemplateDoesNotDefine_IsRefused_AndTheOldOneKept()
    {
        var sut = Create("first");
        var window = Show(sut);
        try
        {
            sut.Transition = TransitionType.Up;

            Assert.Equal(TransitionType.Default, sut.Transition);
        }
        finally
        {
            window.Close();
        }
    }

    [StaFact]
    public void ApplyingATemplateWithoutTheSelectedTransition_Throws_AndResetsToDefault()
    {
        var sut = Create("first");
        sut.Transition = TransitionType.Up;

        Assert.Throws<AtcAppsException>(() => sut.ApplyTemplate());
        Assert.Equal(TransitionType.Default, sut.Transition);
    }

    [StaFact]
    public void CustomTransition_UsesTheStateFromCustomVisualStates()
    {
        var sut = Create("first");
        var custom = new VisualState
        {
            Name = "Spin",
            Storyboard = new Storyboard
            {
                Children = { new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(50))) },
            },
        };
        Storyboard.SetTargetName(custom.Storyboard.Children[0], "CurrentContentPresentationSite");
        Storyboard.SetTargetProperty(custom.Storyboard.Children[0], new PropertyPath(UIElement.OpacityProperty));
        sut.CustomVisualStates!.Add(custom);
        sut.CustomVisualStatesName = "Spin";
        var completed = 0;
        sut.TransitionCompleted += (_, _) => completed++;
        var window = Show(sut);
        try
        {
            sut.Transition = TransitionType.Custom;
            Assert.Equal(TransitionType.Custom, sut.Transition);

            sut.Content = "second";

            Assert.True(WaitUntil(() => completed > 0), "The custom transition did not complete.");
        }
        finally
        {
            window.Close();
        }
    }

    [StaFact]
    public void IsTransitioning_CannotBeSetFromOutside()
    {
        var sut = new TransitioningContentControl();

        Assert.Throws<InvalidOperationException>(() => sut.SetValue(TransitioningContentControl.IsTransitioningProperty, true));
        Assert.False(sut.IsTransitioning);
    }

    [StaFact]
    public void ReloadTransition_ReplaysTheTransitionForTheSameContent()
    {
        var sut = Create("first");
        var completed = 0;
        sut.TransitionCompleted += (_, _) => completed++;
        var window = Show(sut);
        try
        {
            sut.ReloadTransition();

            Assert.True(sut.IsTransitioning);
            Assert.True(WaitUntil(() => completed > 0), "The reloaded transition did not complete.");
            Assert.False(sut.IsTransitioning);
        }
        finally
        {
            window.Close();
        }
    }

    private static TransitioningContentControl Create(object content)
        => new()
        {
            Template = (ControlTemplate)XamlReader.Parse(Template),
            Content = content,
        };

    private static ContentPresenter Presenter(
        Control control,
        string name)
        => Assert.IsType<ContentPresenter>(control.Template.FindName(name, control));

    private static Window Show(TransitioningContentControl sut)
    {
        var window = new Window
        {
            Content = sut,
            Width = 200,
            Height = 100,
            Left = -10000,
            Top = -10000,
            ShowInTaskbar = false,
            ShowActivated = false,
            WindowStyle = WindowStyle.None,
        };

        window.Show();
        ThemedWindow.Pump();
        return window;
    }

    // Runs the dispatcher (animations included) until the condition holds; the timeout only guards a hang.
    private static bool WaitUntil(Func<bool> condition)
    {
        var frame = new DispatcherFrame();
        var timer = new DispatcherTimer(DispatcherPriority.Background) { Interval = TimeSpan.FromMilliseconds(10) };
        var deadline = DateTime.UtcNow + Timeout;
        timer.Tick += (_, _) =>
        {
            if (condition() || DateTime.UtcNow > deadline)
            {
                timer.Stop();
                frame.Continue = false;
            }
        };

        timer.Start();
        Dispatcher.PushFrame(frame);
        return condition();
    }
}