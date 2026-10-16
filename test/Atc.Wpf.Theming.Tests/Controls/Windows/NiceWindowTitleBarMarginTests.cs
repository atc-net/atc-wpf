namespace Atc.Wpf.Theming.Tests.Controls.Windows;

public sealed class NiceWindowTitleBarMarginTests
{
    [Fact]
    public void CalculateCenteredTitleBarMargin_NoCommands_IsZero()
    {
        var margin = NiceWindow.CalculateCenteredTitleBarMargin(leftCommandsWidth: 0, rightCommandsWidth: 0);

        Assert.Equal(default(Thickness), margin);
    }

    [Fact]
    public void CalculateCenteredTitleBarMargin_EqualCommands_IsZero()
    {
        var margin = NiceWindow.CalculateCenteredTitleBarMargin(leftCommandsWidth: 50, rightCommandsWidth: 50);

        Assert.Equal(default(Thickness), margin);
    }

    // The title sits between the two command areas; the wider side is balanced by a margin on the other side.
    [Fact]
    public void CalculateCenteredTitleBarMargin_WiderLeftCommands_PadsTheRight()
    {
        var margin = NiceWindow.CalculateCenteredTitleBarMargin(leftCommandsWidth: 100, rightCommandsWidth: 40);

        Assert.Equal(new Thickness(0, 0, 60, 0), margin);
    }

    [Fact]
    public void CalculateCenteredTitleBarMargin_WiderRightCommands_PadsTheLeft()
    {
        var margin = NiceWindow.CalculateCenteredTitleBarMargin(leftCommandsWidth: 40, rightCommandsWidth: 100);

        Assert.Equal(new Thickness(60, 0, 0, 0), margin);
    }

    [Fact]
    public void CalculateCenteredTitleBarMargin_NeverGoesNegative()
    {
        var margin = NiceWindow.CalculateCenteredTitleBarMargin(leftCommandsWidth: 0, rightCommandsWidth: 120);

        Assert.True(margin.Left >= 0);
        Assert.True(margin.Right >= 0);
        Assert.Equal(0, margin.Top);
        Assert.Equal(0, margin.Bottom);
    }
}