namespace Atc.Wpf.Controls.Tests.Layouts;

public sealed class AutoGridTests
{
    [StaFact]
    public void Constructor_SetsDefaultValues()
    {
        // Arrange & Act
        var grid = new AutoGrid();

        // Assert
        Assert.True(double.IsNaN(grid.Spacing));
        Assert.True(double.IsNaN(grid.HorizontalSpacing));
        Assert.True(double.IsNaN(grid.VerticalSpacing));
        Assert.Null(grid.ChildMargin);
        Assert.Null(grid.ChildHorizontalAlignment);
        Assert.Null(grid.ChildVerticalAlignment);
        Assert.True(grid.IsAutoIndexing);
    }

    [StaTheory]
    [InlineData(0.0)]
    [InlineData(5.0)]
    [InlineData(10.0)]
    [InlineData(20.5)]
    public void Spacing_CanBeSet(double expected)
    {
        // Arrange
        var grid = new AutoGrid();

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
        var grid = new AutoGrid();

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
        var grid = new AutoGrid();

        // Act
        grid.VerticalSpacing = expected;

        // Assert
        Assert.Equal(expected, grid.VerticalSpacing);
    }

    [StaFact]
    public void Parse_SingleStar_ReturnsStarGridLength()
    {
        // Act
        var result = AutoGrid.Parse("*");

        // Assert
        Assert.Single(result);
        Assert.True(result[0].IsStar);
        Assert.Equal(1.0, result[0].Value);
    }

    [StaFact]
    public void Parse_MultipleStars_ReturnsProportionalGridLengths()
    {
        // Act
        var result = AutoGrid.Parse("2*,1*");

        // Assert
        Assert.Equal(2, result.Length);
        Assert.True(result[0].IsStar);
        Assert.Equal(2.0, result[0].Value);
        Assert.True(result[1].IsStar);
        Assert.Equal(1.0, result[1].Value);
    }

    [StaFact]
    public void Parse_PixelValues_ReturnsAbsoluteGridLengths()
    {
        // Act
        var result = AutoGrid.Parse("100,200");

        // Assert
        Assert.Equal(2, result.Length);
        Assert.True(result[0].IsAbsolute);
        Assert.Equal(100.0, result[0].Value);
        Assert.True(result[1].IsAbsolute);
        Assert.Equal(200.0, result[1].Value);
    }

    [StaFact]
    public void Parse_AutoValue_ReturnsAutoGridLength()
    {
        // Act
        var result = AutoGrid.Parse("Auto");

        // Assert
        Assert.Single(result);
        Assert.True(result[0].IsAuto);
    }

    [StaFact]
    public void Parse_MixedValues_ReturnsCorrectGridLengths()
    {
        // Act
        var result = AutoGrid.Parse("Auto,*,100");

        // Assert
        Assert.Equal(3, result.Length);
        Assert.True(result[0].IsAuto);
        Assert.True(result[1].IsStar);
        Assert.True(result[2].IsAbsolute);
        Assert.Equal(100.0, result[2].Value);
    }

    [StaTheory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(5)]
    public void ColumnCount_SetsCorrectNumberOfColumns(int expected)
    {
        // Arrange
        var grid = new AutoGrid();

        // Act
        grid.ColumnCount = expected;

        // Assert
        Assert.Equal(expected, grid.ColumnDefinitions.Count);
    }

    [StaTheory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(5)]
    public void RowCount_SetsCorrectNumberOfRows(int expected)
    {
        // Arrange
        var grid = new AutoGrid();

        // Act
        grid.RowCount = expected;

        // Assert
        Assert.Equal(expected, grid.RowDefinitions.Count);
    }

    [StaFact]
    public void Columns_SetsColumnDefinitions()
    {
        // Arrange
        var grid = new AutoGrid();

        // Act
        grid.Columns = "*,*,*";

        // Assert
        Assert.Equal(3, grid.ColumnDefinitions.Count);
        Assert.True(grid.ColumnDefinitions[0].Width.IsStar);
        Assert.True(grid.ColumnDefinitions[1].Width.IsStar);
        Assert.True(grid.ColumnDefinitions[2].Width.IsStar);
    }

    [StaFact]
    public void Rows_SetsRowDefinitions()
    {
        // Arrange
        var grid = new AutoGrid();

        // Act
        grid.Rows = "Auto,*";

        // Assert
        Assert.Equal(2, grid.RowDefinitions.Count);
        Assert.True(grid.RowDefinitions[0].Height.IsAuto);
        Assert.True(grid.RowDefinitions[1].Height.IsStar);
    }

    [StaFact]
    public void Spacing_AppliedToChildren_AddsMargin()
    {
        // Arrange
        var grid = new AutoGrid
        {
            Columns = "*,*",
            Rows = "*,*",
            Spacing = 10,
        };

        var child1 = new Border();
        var child2 = new Border();
        var child3 = new Border();
        var child4 = new Border();

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
        var grid = new AutoGrid
        {
            Columns = "*,*",
            Rows = "*",
            HorizontalSpacing = 15,
        };

        var child1 = new Border();
        var child2 = new Border();

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
        var grid = new AutoGrid
        {
            Columns = "*",
            Rows = "*,*",
            VerticalSpacing = 20,
        };

        var child1 = new Border();
        var child2 = new Border();

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
        var grid = new AutoGrid
        {
            Columns = "*,*",
            Rows = "*,*",
            Spacing = 10,
            HorizontalSpacing = 20,
            VerticalSpacing = 5,
        };

        var child1 = new Border();
        grid.Children.Add(child1);

        // Act
        grid.Measure(new Size(200, 200));

        // Assert - individual spacing should override unified spacing
        Assert.Equal(20, child1.Margin.Right);
        Assert.Equal(5, child1.Margin.Bottom);
    }
}
