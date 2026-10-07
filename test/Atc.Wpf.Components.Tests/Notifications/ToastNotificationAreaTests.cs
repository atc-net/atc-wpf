namespace Atc.Wpf.Components.Tests.Notifications;

public sealed class ToastNotificationAreaTests
{
    [Fact]
    public void SelectOverflow_OneOverTheLimit_ClosesTheOldest()
    {
        var result = ToastNotificationArea.SelectOverflow(["oldest", "middle", "newest"], maxItems: 2);

        Assert.Equal(["oldest"], result);
    }

    [Fact]
    public void SelectOverflow_SeveralOverTheLimit_ClosesTheOldestOnes()
    {
        var result = ToastNotificationArea.SelectOverflow(["a", "b", "c", "d", "e"], maxItems: 2);

        Assert.Equal(["a", "b", "c"], result);
    }

    [Fact]
    public void SelectOverflow_WithinTheLimit_ClosesNothing()
    {
        var result = ToastNotificationArea.SelectOverflow(["a", "b"], maxItems: 2);

        Assert.Empty(result);
    }
}