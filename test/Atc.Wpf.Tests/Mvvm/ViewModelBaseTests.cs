namespace Atc.Wpf.Tests.Mvvm;

public sealed class ViewModelBaseTests
{
    [Theory]
    [InlineData(true, true, null)]
    [InlineData(true, true, "")]
    [InlineData(true, false, "IsBoolProperty")]
    [InlineData(true, false, "IsBoolPropertyWithExpression")]
    [InlineData(true, false, "IsEnable")]
    [InlineData(true, false, "IsVisible")]
    [InlineData(true, false, "IsBusy")]
    [InlineData(true, false, "IsDirty")]
    [InlineData(true, false, "IsSelected")]
    [SuppressMessage("Design", "CA1030:Use events where appropriate", Justification = "OK.")]
    public void RaisePropertyChanged(bool expected, bool expectedAsEmpty, string? propertyName)
    {
        // Arrange
        var sut = new TestViewModel();
        var actual = false;
        sut.PropertyChanged += (_, e) =>
        {
            actual = TestHelper.HandlePropertyChangedEventArgs(e, expectedAsEmpty, propertyName!);
        };

        // Act
        sut.RaisePropertyChanged(propertyName);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void IsInDesignMode()
        => Assert.False(ViewModelBase.IsInDesignMode);
}