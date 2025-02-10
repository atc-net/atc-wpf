namespace Atc.Wpf.Tests.Mvvm;

public sealed class ObservableObjectTests
{
    [Theory]
    [InlineData(true, true, null)]
    [InlineData(true, true, "")]
    [InlineData(true, false, "IsBoolProperty")]
    [InlineData(true, false, "IsBoolPropertyWithExpression")]
    [InlineData(true, false, "IsBoolPropertyWithSet")]
    [InlineData(true, false, "IsBoolPropertyWithSetAndExpression")]
    [SuppressMessage("Design", "CA1030:Use events where appropriate", Justification = "OK.")]
    public void RaisePropertyChanged(bool expected, bool expectedAsEmpty, string? propertyName)
    {
        // Arrange
        var sut = new TestObservableObject();
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
}