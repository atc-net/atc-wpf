namespace Atc.Wpf.Tests.Controls.Layouts;

using Atc.Wpf.Controls.Layouts;

public sealed class GridExTests
{
    [StaFact]
    public void Constructor_SetsDefaultValues()
    {
        // Arrange & Act
        var grid = new GridEx();

        // Assert
        Assert.True(double.IsNaN(grid.Spacing));
        Assert.True(double.IsNaN(grid.HorizontalSpacing));
        Assert.True(double.IsNaN(grid.VerticalSpacing));
        Assert.Null(grid.Rows);
        Assert.Null(grid.Columns);
    }

    [StaTheory]
    [InlineData(0.0)]
    [InlineData(5.0)]
    [InlineData(10.0)]
    [InlineData(20.5)]
    public void Spacing_CanBeSet(double expected)
    {
        // Arrange
        var grid = new GridEx();

        // Act
        grid.Spacing = expected;

        // Assert
        Assert.Equal(expected, grid.Spacing);
    }

    [StaTheory]
    [InlineData(0.0)]
    [InlineData(5.0)]
    [InlineData(10.0)]
    public void HorizontalSpacing_CanBeSet(double expected)
    {
        // Arrange
        var grid = new GridEx();

        // Act
        grid.HorizontalSpacing = expected;

        // Assert
        Assert.Equal(expected, grid.HorizontalSpacing);
    }

    [StaTheory]
    [InlineData(0.0)]
    [InlineData(5.0)]
    [InlineData(10.0)]
    public void VerticalSpacing_CanBeSet(double expected)
    {
        // Arrange
        var grid = new GridEx();

        // Act
        grid.VerticalSpacing = expected;

        // Assert
        Assert.Equal(expected, grid.VerticalSpacing);
    }

    [StaFact]
    public void Rows_SetsRowDefinitions()
    {
        // Arrange
        var grid = new GridEx();

        // Act
        grid.Rows = "*,*,*";

        // Assert
        Assert.Equal(3, grid.RowDefinitions.Count);
        Assert.True(grid.RowDefinitions[0].Height.IsStar);
        Assert.True(grid.RowDefinitions[1].Height.IsStar);
        Assert.True(grid.RowDefinitions[2].Height.IsStar);
    }

    [StaFact]
    public void Columns_SetsColumnDefinitions()
    {
        // Arrange
        var grid = new GridEx();

        // Act
        grid.Columns = "Auto,*,200";

        // Assert
        Assert.Equal(3, grid.ColumnDefinitions.Count);
        Assert.True(grid.ColumnDefinitions[0].Width.IsAuto);
        Assert.True(grid.ColumnDefinitions[1].Width.IsStar);
        Assert.True(grid.ColumnDefinitions[2].Width.IsAbsolute);
        Assert.Equal(200.0, grid.ColumnDefinitions[2].Width.Value);
    }

    [StaFact]
    public void Rows_WithProportionalValues_SetsCorrectWeights()
    {
        // Arrange
        var grid = new GridEx();

        // Act
        grid.Rows = "2*,1*";

        // Assert
        Assert.Equal(2, grid.RowDefinitions.Count);
        Assert.Equal(2.0, grid.RowDefinitions[0].Height.Value);
        Assert.Equal(1.0, grid.RowDefinitions[1].Height.Value);
    }

    [StaFact]
    public void Spacing_AppliedToChildren_AddsMargin()
    {
        // Arrange
        var grid = new GridEx
        {
            Columns = "*,*",
            Rows = "*,*",
            Spacing = 10,
        };

        var child1 = new Border();
        var child2 = new Border();
        var child3 = new Border();
        var child4 = new Border();

        Grid.SetRow(child1, 0);
        Grid.SetColumn(child1, 0);
        Grid.SetRow(child2, 0);
        Grid.SetColumn(child2, 1);
        Grid.SetRow(child3, 1);
        Grid.SetColumn(child3, 0);
        Grid.SetRow(child4, 1);
        Grid.SetColumn(child4, 1);

        grid.Children.Add(child1);
        grid.Children.Add(child2);
        grid.Children.Add(child3);
        grid.Children.Add(child4);

        // Act
        grid.Measure(new Size(200, 200));

        // Assert - first child [0,0] should have right and bottom margin
        Assert.Equal(10, child1.Margin.Right);
        Assert.Equal(10, child1.Margin.Bottom);

        // Assert - second child [0,1] should have bottom margin only (last column)
        Assert.Equal(0, child2.Margin.Right);
        Assert.Equal(10, child2.Margin.Bottom);

        // Assert - third child [1,0] should have right margin only (last row)
        Assert.Equal(10, child3.Margin.Right);
        Assert.Equal(0, child3.Margin.Bottom);

        // Assert - fourth child [1,1] should have no spacing margin (last row and column)
        Assert.Equal(0, child4.Margin.Right);
        Assert.Equal(0, child4.Margin.Bottom);
    }

    [StaFact]
    public void HorizontalSpacing_AppliedToChildren_AddsOnlyRightMargin()
    {
        // Arrange
        var grid = new GridEx
        {
            Columns = "*,*",
            Rows = "*",
            HorizontalSpacing = 15,
        };

        var child1 = new Border();
        var child2 = new Border();

        Grid.SetRow(child1, 0);
        Grid.SetColumn(child1, 0);
        Grid.SetRow(child2, 0);
        Grid.SetColumn(child2, 1);

        grid.Children.Add(child1);
        grid.Children.Add(child2);

        // Act
        grid.Measure(new Size(200, 100));

        // Assert - first child should have right margin
        Assert.Equal(15, child1.Margin.Right);
        Assert.Equal(0, child1.Margin.Bottom);

        // Assert - second child should have no margin (last column)
        Assert.Equal(0, child2.Margin.Right);
        Assert.Equal(0, child2.Margin.Bottom);
    }

    [StaFact]
    public void VerticalSpacing_AppliedToChildren_AddsOnlyBottomMargin()
    {
        // Arrange
        var grid = new GridEx
        {
            Columns = "*",
            Rows = "*,*",
            VerticalSpacing = 20,
        };

        var child1 = new Border();
        var child2 = new Border();

        Grid.SetRow(child1, 0);
        Grid.SetColumn(child1, 0);
        Grid.SetRow(child2, 1);
        Grid.SetColumn(child2, 0);

        grid.Children.Add(child1);
        grid.Children.Add(child2);

        // Act
        grid.Measure(new Size(100, 200));

        // Assert - first child should have bottom margin
        Assert.Equal(0, child1.Margin.Right);
        Assert.Equal(20, child1.Margin.Bottom);

        // Assert - second child should have no margin (last row)
        Assert.Equal(0, child2.Margin.Right);
        Assert.Equal(0, child2.Margin.Bottom);
    }

    [StaFact]
    public void IndividualSpacing_OverridesUnifiedSpacing()
    {
        // Arrange
        var grid = new GridEx
        {
            Columns = "*,*",
            Rows = "*,*",
            Spacing = 10,
            HorizontalSpacing = 20,
            VerticalSpacing = 5,
        };

        var child1 = new Border();
        Grid.SetRow(child1, 0);
        Grid.SetColumn(child1, 0);
        grid.Children.Add(child1);

        // Act
        grid.Measure(new Size(200, 200));

        // Assert - individual spacing should override unified spacing
        Assert.Equal(20, child1.Margin.Right);
        Assert.Equal(5, child1.Margin.Bottom);
    }

    [StaFact]
    public void NoSpacing_DoesNotModifyChildMargin()
    {
        // Arrange
        var grid = new GridEx
        {
            Columns = "*,*",
            Rows = "*,*",
        };

        var child1 = new Border { Margin = new Thickness(5) };
        Grid.SetRow(child1, 0);
        Grid.SetColumn(child1, 0);
        grid.Children.Add(child1);

        // Act
        grid.Measure(new Size(200, 200));

        // Assert - margin should remain unchanged
        Assert.Equal(5, child1.Margin.Left);
        Assert.Equal(5, child1.Margin.Top);
        Assert.Equal(5, child1.Margin.Right);
        Assert.Equal(5, child1.Margin.Bottom);
    }
}