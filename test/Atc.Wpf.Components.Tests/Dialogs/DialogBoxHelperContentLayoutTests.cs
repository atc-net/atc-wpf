namespace Atc.Wpf.Components.Tests.Dialogs;

/// <summary>
/// Lays out the text content of the information and question dialogs the way the dialogs host it (a ScrollViewer
/// with 30 padding and no horizontal scrolling) in plain windows: the text wraps to the dialog width instead of
/// overflowing sideways, keeps explicit line breaks, breaks long words, and sits next to the icon in both directions.
/// </summary>
/// <remarks>
/// The real dialog boxes need theme resources that are not available in a test host; the UI tests open them in the
/// sample application.
/// </remarks>
public sealed class DialogBoxHelperContentLayoutTests
{
    private const double IconSize = 32;
    private const double IconGap = 20;
    private const double ContentPadding = 30;
    private const double Tolerance = 1;

    private const string ShortText = "Are you sure to delete item?";

    private const string OneLongLine =
        "4 of 4 cameras record or wait for motion to record (3 recording now). Closing CameraWall stops this. Close anyway?";

    private const string SeveralLongLines =
        "The first line is long enough to wrap at the default dialog width of the information dialog.\n" +
        "The second line is also long enough to wrap at the default dialog width of the information dialog.\n" +
        "The third line ends the message.";

    private const string LongPathWithoutSpaces =
        @"C:\ProgramData\AtcSoft\Video\Surveillance\Recordings\Camera-0001\2026\11\04\19\recording-0001-segment-000123456789.mp4";

    public static TheoryData<string, double> LongTexts
        => new()
        {
            { OneLongLine, 350 },
            { OneLongLine, 520 },
            { SeveralLongLines, 350 },
            { SeveralLongLines, 520 },
            { LongPathWithoutSpaces, 350 },
            { LongPathWithoutSpaces, 520 },
        };

    [StaTheory]
    [MemberData(nameof(LongTexts))]
    public void LongText_WrapsInsideTheViewport_WithoutHorizontalOverflow(
        string text,
        double dialogWidth)
    {
        using var host = new DialogContentHost(text, dialogWidth, FlowDirection.LeftToRight);

        var viewer = host.ScrollViewer;
        Assert.True(
            viewer.ExtentWidth <= viewer.ViewportWidth + Tolerance,
            $"Extent width {viewer.ExtentWidth} should fit the viewport width {viewer.ViewportWidth}.");
        Assert.NotEqual(Visibility.Visible, viewer.ComputedHorizontalScrollBarVisibility);
        Assert.Equal(0, viewer.ScrollableWidth);

        var textRight = host.TextBlock.TranslatePoint(new Point(host.TextBlock.ActualWidth, 0), viewer).X;
        Assert.True(
            textRight <= viewer.ActualWidth - ContentPadding + Tolerance,
            $"The text ends at {textRight}, beyond the padded viewport ({viewer.ActualWidth - ContentPadding}).");

        Assert.True(
            host.TextBlock.ActualHeight > host.SingleLineHeight * 1.5,
            $"The text should wrap onto more than one line (height {host.TextBlock.ActualHeight}, one line {host.SingleLineHeight}).");
    }

    [StaFact]
    public void ExplicitLineBreaks_AreKept()
    {
        using var host = new DialogContentHost("One\nTwo\nThree", 520, FlowDirection.LeftToRight);

        Assert.True(
            host.TextBlock.ActualHeight >= (host.SingleLineHeight * 3) - Tolerance,
            $"Three explicit lines should give at least three line heights (height {host.TextBlock.ActualHeight}, one line {host.SingleLineHeight}).");
    }

    [StaFact]
    public void ShortText_IsOneLine_NextToTheIcon_AndVerticallyCentredOnIt()
    {
        using var host = new DialogContentHost(ShortText, 350, FlowDirection.LeftToRight);

        Assert.Equal(host.SingleLineHeight, host.TextBlock.ActualHeight, precision: 1);

        var iconLeft = host.Icon.TranslatePoint(default, host.ScrollViewer).X;
        var textLeft = host.TextBlock.TranslatePoint(default, host.ScrollViewer).X;
        Assert.Equal(ContentPadding, iconLeft, precision: 1);
        Assert.Equal(ContentPadding + IconSize + IconGap, textLeft, precision: 1);

        var iconMiddle = host.Icon.TranslatePoint(new Point(0, IconSize / 2), host.ScrollViewer).Y;
        var textMiddle = host.TextBlock.TranslatePoint(new Point(0, host.TextBlock.ActualHeight / 2), host.ScrollViewer).Y;
        Assert.Equal(iconMiddle, textMiddle, precision: 1);
    }

    [StaFact]
    public void TallText_StartsLevelWithTheIcon_WhichStaysAtTheTop()
    {
        using var host = new DialogContentHost(SeveralLongLines, 350, FlowDirection.LeftToRight);

        var iconTop = host.Icon.TranslatePoint(default, host.ScrollViewer).Y;
        var textTop = host.TextBlock.TranslatePoint(default, host.ScrollViewer).Y;
        Assert.Equal(iconTop, textTop, precision: 1);
    }

    [StaFact]
    public void ScrollingText_KeepsTheIconAtTheTopOfTheContent()
    {
        var text = string.Join('\n', Enumerable.Repeat("A line.", 100));
        using var host = new DialogContentHost(text, 350, FlowDirection.LeftToRight);

        Assert.True(host.ScrollViewer.ScrollableHeight > 0);
        Assert.Equal(ContentPadding, host.Icon.TranslatePoint(default, host.ScrollViewer).Y, precision: 1);
        Assert.Equal(ContentPadding, host.TextBlock.TranslatePoint(default, host.ScrollViewer).Y, precision: 1);
    }

    [StaTheory]
    [InlineData(ShortText)]
    [InlineData(OneLongLine)]
    [InlineData(LongPathWithoutSpaces)]
    public void RightToLeft_PutsTheIconOnTheRight_AndTheTextFitsToItsLeft(
        string text)
    {
        using var host = new DialogContentHost(text, 350, FlowDirection.RightToLeft);

        var viewer = host.ScrollViewer;
        Assert.NotEqual(Visibility.Visible, viewer.ComputedHorizontalScrollBarVisibility);
        Assert.Equal(0, viewer.ScrollableWidth);

        var viewerScreen = ScreenBounds(viewer);
        var iconScreen = ScreenBounds(host.Icon);
        var textScreen = ScreenBounds(host.TextBlock);

        Assert.Equal(viewerScreen.Right - ContentPadding, iconScreen.Right, precision: 1);
        Assert.True(
            textScreen.Right <= iconScreen.Left - IconGap + Tolerance,
            $"The text ({textScreen}) should end left of the icon ({iconScreen}) with the gap.");
        Assert.True(
            textScreen.Left >= viewerScreen.Left + ContentPadding - Tolerance,
            $"The text ({textScreen}) should start inside the padded viewport ({viewerScreen}).");
    }

    [StaFact]
    public void WithoutIcon_TheTextUsesTheWholeWidth()
    {
        using var host = new DialogContentHost(OneLongLine, 350, FlowDirection.LeftToRight, withIcon: false);

        var textLeft = host.TextBlock.TranslatePoint(default, host.ScrollViewer).X;
        Assert.Equal(ContentPadding, textLeft, precision: 1);
        Assert.Equal(0, host.ScrollViewer.ScrollableWidth);
    }

    [StaFact]
    public void TheSameIcon_GivesEachDialogItsOwnCopy()
    {
        var icon = new Border { Width = IconSize, Height = IconSize, Background = Brushes.Red, Margin = new Thickness(1, 2, 3, 4) };

        var first = ((Grid)DialogBoxHelper.CreateContentControl(ShortText, icon).Content).Children.OfType<Border>().Single();
        var second = ((Grid)DialogBoxHelper.CreateContentControl(ShortText, icon).Content).Children.OfType<Border>().Single();

        Assert.NotSame(icon, first);
        Assert.NotSame(first, second);
        Assert.Null(icon.Parent);
        Assert.Equal(IconSize, second.Width);
        Assert.Same(Brushes.Red, second.Background);
        Assert.Equal(new Thickness(1, 2, 3, 4), second.Margin);
    }

    [StaFact]
    public void ACopiedIcon_KeepsItsBindings()
    {
        var source = new Border { Width = 40 };
        var icon = new Border();
        icon.SetBinding(FrameworkElement.WidthProperty, new Binding(nameof(FrameworkElement.Width)) { Source = source });

        var copy = DialogBoxHelper.CopyElement(icon);

        Assert.NotNull(BindingOperations.GetBinding(copy, FrameworkElement.WidthProperty));
        Assert.Equal(40, copy.Width);
    }

    /// <summary>
    /// Gets the bounds of an element on the screen in device-independent units (left to right whatever the flow direction).
    /// </summary>
    private static Rect ScreenBounds(FrameworkElement element)
    {
        var fromDevice = PresentationSource.FromVisual(element)!.CompositionTarget!.TransformFromDevice;
        return new Rect(
            fromDevice.Transform(element.PointToScreen(default)),
            fromDevice.Transform(element.PointToScreen(new Point(element.ActualWidth, element.ActualHeight))));
    }

    /// <summary>
    /// A window shaped like the information dialog: a 30-padded ScrollViewer without horizontal scrolling
    /// holding the content from <see cref="DialogBoxHelper.CreateContentControl"/>.
    /// </summary>
    private sealed class DialogContentHost : IDisposable
    {
        private readonly Window window;

        public DialogContentHost(
            string text,
            double width,
            FlowDirection flowDirection,
            bool withIcon = true)
        {
            var icon = new Border
            {
                Width = IconSize,
                Height = IconSize,
                Margin = new Thickness(0, 0, IconGap, 0),
                Background = Brushes.Gray,
            };

            var content = DialogBoxHelper.CreateContentControl(text, withIcon ? icon : null);
            TextBlock = ((Grid)content.Content).Children.OfType<TextBlock>().Single();

            // The dialog shows its own copy of the icon.
            Icon = ((Grid)content.Content).Children.OfType<Border>().SingleOrDefault() ?? icon;

            ScrollViewer = new ScrollViewer
            {
                Padding = new Thickness(ContentPadding),
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Content = content,
            };

            window = new Window
            {
                Width = width,
                Height = 400,
                Left = -10000,
                Top = -10000,
                WindowStartupLocation = WindowStartupLocation.Manual,
                WindowStyle = WindowStyle.None,
                ResizeMode = ResizeMode.NoResize,
                ShowInTaskbar = false,
                ShowActivated = false,
                FlowDirection = flowDirection,
                Content = ScrollViewer,
            };

            window.Show();
            window.UpdateLayout();

            var probe = new TextBlock { FontFamily = TextBlock.FontFamily, FontSize = TextBlock.FontSize, Text = "Xy" };
            probe.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            SingleLineHeight = probe.DesiredSize.Height;
        }

        public Border Icon { get; }

        public TextBlock TextBlock { get; }

        public ScrollViewer ScrollViewer { get; }

        public double SingleLineHeight { get; }

        public void Dispose()
            => window.Close();
    }
}