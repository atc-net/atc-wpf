namespace Atc.Wpf.Controls.Tests.DataDisplay;

public sealed class CarouselTests
{
    #region Constructor Tests

    [StaFact]
    public void Constructor_SetsDefaultValues()
    {
        // Arrange & Act
        var carousel = new Carousel();

        // Assert
        Assert.True(carousel.ShowNavigationArrows);
        Assert.True(carousel.ShowIndicators);
        Assert.Equal(IndicatorPosition.Bottom, carousel.IndicatorPosition);
        Assert.False(carousel.AutoPlay);
        Assert.Equal(5000.0, carousel.AutoPlayInterval);
        Assert.True(carousel.PauseOnHover);
        Assert.True(carousel.IsInfiniteLoop);
        Assert.Equal(CarouselTransitionType.Slide, carousel.TransitionType);
        Assert.Equal(300.0, carousel.TransitionDuration);
        Assert.Equal(10.0, carousel.IndicatorSize);
        Assert.Equal(8.0, carousel.IndicatorSpacing);
        Assert.True(carousel.IsDragEnabled);
        Assert.Equal(0.2, carousel.DragThreshold);
    }

    #endregion

    #region ShowNavigationArrows Property Tests

    [StaTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ShowNavigationArrows_CanBeSet(bool expected)
    {
        // Arrange
        var carousel = new Carousel();

        // Act
        carousel.ShowNavigationArrows = expected;

        // Assert
        Assert.Equal(expected, carousel.ShowNavigationArrows);
    }

    #endregion

    #region ShowIndicators Property Tests

    [StaTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ShowIndicators_CanBeSet(bool expected)
    {
        // Arrange
        var carousel = new Carousel();

        // Act
        carousel.ShowIndicators = expected;

        // Assert
        Assert.Equal(expected, carousel.ShowIndicators);
    }

    #endregion

    #region IndicatorPosition Property Tests

    [StaTheory]
    [InlineData(IndicatorPosition.Bottom)]
    [InlineData(IndicatorPosition.Top)]
    [InlineData(IndicatorPosition.Left)]
    [InlineData(IndicatorPosition.Right)]
    public void IndicatorPosition_CanBeSet(IndicatorPosition expected)
    {
        // Arrange
        var carousel = new Carousel();

        // Act
        carousel.IndicatorPosition = expected;

        // Assert
        Assert.Equal(expected, carousel.IndicatorPosition);
    }

    #endregion

    #region AutoPlay Property Tests

    [StaTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void AutoPlay_CanBeSet(bool expected)
    {
        // Arrange
        var carousel = new Carousel();

        // Act
        carousel.AutoPlay = expected;

        // Assert
        Assert.Equal(expected, carousel.AutoPlay);
    }

    #endregion

    #region AutoPlayInterval Property Tests

    [StaTheory]
    [InlineData(1000.0)]
    [InlineData(3000.0)]
    [InlineData(5000.0)]
    [InlineData(10000.0)]
    public void AutoPlayInterval_CanBeSet(double expected)
    {
        // Arrange
        var carousel = new Carousel();

        // Act
        carousel.AutoPlayInterval = expected;

        // Assert
        Assert.Equal(expected, carousel.AutoPlayInterval);
    }

    #endregion

    #region PauseOnHover Property Tests

    [StaTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void PauseOnHover_CanBeSet(bool expected)
    {
        // Arrange
        var carousel = new Carousel();

        // Act
        carousel.PauseOnHover = expected;

        // Assert
        Assert.Equal(expected, carousel.PauseOnHover);
    }

    #endregion

    #region IsInfiniteLoop Property Tests

    [StaTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void IsInfiniteLoop_CanBeSet(bool expected)
    {
        // Arrange
        var carousel = new Carousel();

        // Act
        carousel.IsInfiniteLoop = expected;

        // Assert
        Assert.Equal(expected, carousel.IsInfiniteLoop);
    }

    #endregion

    #region TransitionType Property Tests

    [StaTheory]
    [InlineData(CarouselTransitionType.None)]
    [InlineData(CarouselTransitionType.Slide)]
    [InlineData(CarouselTransitionType.Fade)]
    [InlineData(CarouselTransitionType.SlideAndFade)]
    public void TransitionType_CanBeSet(CarouselTransitionType expected)
    {
        // Arrange
        var carousel = new Carousel();

        // Act
        carousel.TransitionType = expected;

        // Assert
        Assert.Equal(expected, carousel.TransitionType);
    }

    #endregion

    #region TransitionDuration Property Tests

    [StaTheory]
    [InlineData(100.0)]
    [InlineData(300.0)]
    [InlineData(500.0)]
    [InlineData(1000.0)]
    public void TransitionDuration_CanBeSet(double expected)
    {
        // Arrange
        var carousel = new Carousel();

        // Act
        carousel.TransitionDuration = expected;

        // Assert
        Assert.Equal(expected, carousel.TransitionDuration);
    }

    #endregion

    #region IndicatorSize Property Tests

    [StaTheory]
    [InlineData(8.0)]
    [InlineData(10.0)]
    [InlineData(12.0)]
    [InlineData(16.0)]
    public void IndicatorSize_CanBeSet(double expected)
    {
        // Arrange
        var carousel = new Carousel();

        // Act
        carousel.IndicatorSize = expected;

        // Assert
        Assert.Equal(expected, carousel.IndicatorSize);
    }

    #endregion

    #region IndicatorSpacing Property Tests

    [StaTheory]
    [InlineData(4.0)]
    [InlineData(8.0)]
    [InlineData(12.0)]
    [InlineData(16.0)]
    public void IndicatorSpacing_CanBeSet(double expected)
    {
        // Arrange
        var carousel = new Carousel();

        // Act
        carousel.IndicatorSpacing = expected;

        // Assert
        Assert.Equal(expected, carousel.IndicatorSpacing);
    }

    #endregion

    #region CornerRadius Property Tests

    [StaFact]
    public void CornerRadius_CanBeSet()
    {
        // Arrange
        var carousel = new Carousel();
        var expected = new CornerRadius(8);

        // Act
        carousel.CornerRadius = expected;

        // Assert
        Assert.Equal(expected, carousel.CornerRadius);
    }

    [StaFact]
    public void CornerRadius_CanBeSetWithDifferentValues()
    {
        // Arrange
        var carousel = new Carousel();
        var expected = new CornerRadius(4, 8, 12, 16);

        // Act
        carousel.CornerRadius = expected;

        // Assert
        Assert.Equal(expected, carousel.CornerRadius);
    }

    #endregion

    #region IsDragEnabled Property Tests

    [StaTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void IsDragEnabled_CanBeSet(bool expected)
    {
        // Arrange
        var carousel = new Carousel();

        // Act
        carousel.IsDragEnabled = expected;

        // Assert
        Assert.Equal(expected, carousel.IsDragEnabled);
    }

    #endregion

    #region DragThreshold Property Tests

    [StaTheory]
    [InlineData(0.1)]
    [InlineData(0.2)]
    [InlineData(0.3)]
    [InlineData(0.5)]
    public void DragThreshold_CanBeSet(double expected)
    {
        // Arrange
        var carousel = new Carousel();

        // Act
        carousel.DragThreshold = expected;

        // Assert
        Assert.Equal(expected, carousel.DragThreshold);
    }

    #endregion

    #region Navigation Tests

    [StaFact]
    public void Next_WithNoItems_DoesNotChangeIndex()
    {
        // Arrange
        var carousel = new Carousel();

        // Act
        carousel.Next();

        // Assert
        Assert.Equal(-1, carousel.SelectedIndex);
    }

    [StaFact]
    public void Previous_WithNoItems_DoesNotChangeIndex()
    {
        // Arrange
        var carousel = new Carousel();

        // Act
        carousel.Previous();

        // Assert
        Assert.Equal(-1, carousel.SelectedIndex);
    }

    [StaFact]
    public void GoToSlide_WithNoItems_DoesNotChangeIndex()
    {
        // Arrange
        var carousel = new Carousel();

        // Act
        carousel.GoToSlide(2);

        // Assert
        Assert.Equal(-1, carousel.SelectedIndex);
    }

    [StaFact]
    public void GoToSlide_WithNegativeIndex_ClampsToZero()
    {
        // Arrange
        var carousel = new Carousel
        {
            TransitionType = CarouselTransitionType.None,
        };
        carousel.Items.Add(new Border());
        carousel.Items.Add(new Border());
        carousel.SelectedIndex = 1;

        // Act
        carousel.GoToSlide(-5);

        // Assert
        Assert.Equal(0, carousel.SelectedIndex);
    }

    [StaFact]
    public void GoToSlide_WithIndexBeyondCount_ClampsToLastIndex()
    {
        // Arrange
        var carousel = new Carousel
        {
            TransitionType = CarouselTransitionType.None,
        };
        carousel.Items.Add(new Border());
        carousel.Items.Add(new Border());
        carousel.Items.Add(new Border());
        carousel.SelectedIndex = 0;

        // Act
        carousel.GoToSlide(100);

        // Assert
        Assert.Equal(2, carousel.SelectedIndex);
    }

    [StaFact]
    public void GoToSlide_WithValidIndex_NavigatesToSlide()
    {
        // Arrange
        var carousel = new Carousel
        {
            TransitionType = CarouselTransitionType.None,
        };
        carousel.Items.Add(new Border());
        carousel.Items.Add(new Border());
        carousel.Items.Add(new Border());
        carousel.SelectedIndex = 0;

        // Act
        carousel.GoToSlide(2);

        // Assert
        Assert.Equal(2, carousel.SelectedIndex);
    }

    #endregion

    #region Infinite Loop Tests

    [StaFact]
    public void Next_AtLastSlide_WithInfiniteLoop_WrapsToFirst()
    {
        // Arrange
        var carousel = new Carousel
        {
            IsInfiniteLoop = true,
            TransitionType = CarouselTransitionType.None,
        };
        carousel.Items.Add(new Border());
        carousel.Items.Add(new Border());
        carousel.Items.Add(new Border());
        carousel.SelectedIndex = 2;

        // Act
        carousel.Next();

        // Assert
        Assert.Equal(0, carousel.SelectedIndex);
    }

    [StaFact]
    public void Previous_AtFirstSlide_WithInfiniteLoop_WrapsToLast()
    {
        // Arrange
        var carousel = new Carousel
        {
            IsInfiniteLoop = true,
            TransitionType = CarouselTransitionType.None,
        };
        carousel.Items.Add(new Border());
        carousel.Items.Add(new Border());
        carousel.Items.Add(new Border());
        carousel.SelectedIndex = 0;

        // Act
        carousel.Previous();

        // Assert
        Assert.Equal(2, carousel.SelectedIndex);
    }

    [StaFact]
    public void Next_AtLastSlide_WithoutInfiniteLoop_StaysAtLast()
    {
        // Arrange
        var carousel = new Carousel
        {
            IsInfiniteLoop = false,
            TransitionType = CarouselTransitionType.None,
        };
        carousel.Items.Add(new Border());
        carousel.Items.Add(new Border());
        carousel.Items.Add(new Border());
        carousel.SelectedIndex = 2;

        // Act
        carousel.Next();

        // Assert
        Assert.Equal(2, carousel.SelectedIndex);
    }

    [StaFact]
    public void Previous_AtFirstSlide_WithoutInfiniteLoop_StaysAtFirst()
    {
        // Arrange
        var carousel = new Carousel
        {
            IsInfiniteLoop = false,
            TransitionType = CarouselTransitionType.None,
        };
        carousel.Items.Add(new Border());
        carousel.Items.Add(new Border());
        carousel.Items.Add(new Border());
        carousel.SelectedIndex = 0;

        // Act
        carousel.Previous();

        // Assert
        Assert.Equal(0, carousel.SelectedIndex);
    }

    #endregion

    #region Event Args Tests

    [Fact]
    public void CarouselSlideChangedEventArgs_StoresIndices()
    {
        // Arrange & Act
        var args = new CarouselSlideChangedEventArgs(1, 2);

        // Assert
        Assert.Equal(1, args.OldIndex);
        Assert.Equal(2, args.NewIndex);
    }

    [Fact]
    public void CarouselSlideChangingEventArgs_StoresIndices()
    {
        // Arrange & Act
        var args = new CarouselSlideChangingEventArgs(1, 2);

        // Assert
        Assert.Equal(1, args.CurrentIndex);
        Assert.Equal(2, args.TargetIndex);
        Assert.False(args.Cancel);
    }

    [Fact]
    public void CarouselSlideChangingEventArgs_CancelCanBeSet()
    {
        // Arrange
        var args = new CarouselSlideChangingEventArgs(1, 2);

        // Act
        args.Cancel = true;

        // Assert
        Assert.True(args.Cancel);
    }

    #endregion
}