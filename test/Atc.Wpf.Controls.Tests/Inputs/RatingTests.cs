namespace Atc.Wpf.Controls.Tests.Inputs;

public sealed class RatingTests
{
    [StaFact]
    public void Constructor_SetsDefaultValues()
    {
        // Arrange & Act
        var control = new Rating();

        // Assert
        Assert.Equal(0.0, control.Value);
        Assert.Equal(5, control.Maximum);
        Assert.False(control.AllowHalfStars);
        Assert.False(control.IsReadOnly);
        Assert.True(control.ShowPreviewOnHover);
        Assert.Equal(24.0, control.ItemSize);
        Assert.Equal(4.0, control.ItemSpacing);
    }

    [StaTheory]
    [InlineData(0.0)]
    [InlineData(1.0)]
    [InlineData(2.5)]
    [InlineData(5.0)]
    public void Value_CanBeSet(double expectedValue)
    {
        // Arrange
        var control = new Rating { AllowHalfStars = true };

        // Act
        control.Value = expectedValue;

        // Assert
        Assert.Equal(expectedValue, control.Value);
    }

    [StaFact]
    public void Value_IsCoercedToMinimum_WhenBelowZero()
    {
        // Arrange
        var control = new Rating();

        // Act
        control.Value = -1.0;

        // Assert
        Assert.Equal(0.0, control.Value);
    }

    [StaFact]
    public void Value_IsCoercedToMaximum_WhenAboveMaximum()
    {
        // Arrange
        var control = new Rating { Maximum = 5 };

        // Act
        control.Value = 10.0;

        // Assert
        Assert.Equal(5.0, control.Value);
    }

    [StaTheory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void Maximum_CanBeSet(int expectedMaximum)
    {
        // Arrange
        var control = new Rating();

        // Act
        control.Maximum = expectedMaximum;

        // Assert
        Assert.Equal(expectedMaximum, control.Maximum);
    }

    [StaFact]
    public void Maximum_IsCoercedToMinimum_WhenBelowOne()
    {
        // Arrange
        var control = new Rating();

        // Act
        control.Maximum = 0;

        // Assert
        Assert.Equal(1, control.Maximum);
    }

    [StaFact]
    public void Maximum_IsCoercedToMaximum_WhenAboveTen()
    {
        // Arrange
        var control = new Rating();

        // Act
        control.Maximum = 15;

        // Assert
        Assert.Equal(10, control.Maximum);
    }

    [StaFact]
    public void Value_IsAdjusted_WhenMaximumIsDecreased()
    {
        // Arrange
        var control = new Rating
        {
            Maximum = 10,
            Value = 8,
        };

        // Act
        control.Maximum = 5;

        // Assert
        Assert.Equal(5.0, control.Value);
    }

    [StaTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void AllowHalfStars_CanBeSet(bool expectedValue)
    {
        // Arrange
        var control = new Rating();

        // Act
        control.AllowHalfStars = expectedValue;

        // Assert
        Assert.Equal(expectedValue, control.AllowHalfStars);
    }

    [StaTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void IsReadOnly_CanBeSet(bool expectedValue)
    {
        // Arrange
        var control = new Rating();

        // Act
        control.IsReadOnly = expectedValue;

        // Assert
        Assert.Equal(expectedValue, control.IsReadOnly);
    }

    [StaTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ShowPreviewOnHover_CanBeSet(bool expectedValue)
    {
        // Arrange
        var control = new Rating();

        // Act
        control.ShowPreviewOnHover = expectedValue;

        // Assert
        Assert.Equal(expectedValue, control.ShowPreviewOnHover);
    }

    [StaTheory]
    [InlineData(16.0)]
    [InlineData(24.0)]
    [InlineData(48.0)]
    public void ItemSize_CanBeSet(double expectedValue)
    {
        // Arrange
        var control = new Rating();

        // Act
        control.ItemSize = expectedValue;

        // Assert
        Assert.Equal(expectedValue, control.ItemSize);
    }

    [StaTheory]
    [InlineData(0.0)]
    [InlineData(4.0)]
    [InlineData(8.0)]
    public void ItemSpacing_CanBeSet(double expectedValue)
    {
        // Arrange
        var control = new Rating();

        // Act
        control.ItemSpacing = expectedValue;

        // Assert
        Assert.Equal(expectedValue, control.ItemSpacing);
    }

    [StaFact]
    public void FilledIcon_CanBeSet()
    {
        // Arrange
        var control = new Rating();
        const string expectedIcon = "❤";

        // Act
        control.FilledIcon = expectedIcon;

        // Assert
        Assert.Equal(expectedIcon, control.FilledIcon);
    }

    [StaFact]
    public void EmptyIcon_CanBeSet()
    {
        // Arrange
        var control = new Rating();
        const string expectedIcon = "♡";

        // Act
        control.EmptyIcon = expectedIcon;

        // Assert
        Assert.Equal(expectedIcon, control.EmptyIcon);
    }

    [StaFact]
    public void HalfFilledIcon_CanBeSet()
    {
        // Arrange
        var control = new Rating();
        const string expectedIcon = "⯨";

        // Act
        control.HalfFilledIcon = expectedIcon;

        // Assert
        Assert.Equal(expectedIcon, control.HalfFilledIcon);
    }

    [StaFact]
    public void ValueChanged_EventIsRaised_WhenValueChanges()
    {
        // Arrange
        var control = new Rating();
        var eventRaised = false;
        double oldValue = 0;
        double newValue = 0;

        control.ValueChanged += (sender, e) =>
        {
            eventRaised = true;
            oldValue = e.OldValue;
            newValue = e.NewValue;
        };

        // Act
        control.Value = 3.0;

        // Assert
        Assert.True(eventRaised);
        Assert.Equal(0.0, oldValue);
        Assert.Equal(3.0, newValue);
    }

    [StaFact]
    public void ValueChanged_EventIsNotRaised_WhenValueIsSame()
    {
        // Arrange
        var control = new Rating { Value = 3.0 };
        var eventRaised = false;

        control.ValueChanged += (_, _) => eventRaised = true;

        // Act
        control.Value = 3.0;

        // Assert
        Assert.False(eventRaised);
    }
}