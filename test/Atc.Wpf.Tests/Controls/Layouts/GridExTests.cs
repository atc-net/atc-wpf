namespace Atc.Wpf.Tests.Controls.Layouts;

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

        // Assert - each gap is split over the cells on both sides; the outer edges get none
        Assert.Equal(new Thickness(0, 0, 5, 5), child1.Margin);
        Assert.Equal(new Thickness(5, 0, 0, 5), child2.Margin);
        Assert.Equal(new Thickness(0, 5, 5, 0), child3.Margin);
        Assert.Equal(new Thickness(5, 5, 0, 0), child4.Margin);
    }

    [StaFact]
    public void HorizontalSpacing_AppliedToChildren_SplitsTheGapBetweenTheColumns()
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

        // Assert
        Assert.Equal(new Thickness(0, 0, 7.5, 0), child1.Margin);
        Assert.Equal(new Thickness(7.5, 0, 0, 0), child2.Margin);
    }

    [StaFact]
    public void VerticalSpacing_AppliedToChildren_SplitsTheGapBetweenTheRows()
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

        // Assert
        Assert.Equal(new Thickness(0, 0, 0, 10), child1.Margin);
        Assert.Equal(new Thickness(0, 10, 0, 0), child2.Margin);
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
        Assert.Equal(10, child1.Margin.Right);
        Assert.Equal(2.5, child1.Margin.Bottom);
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

    [StaFact]
    public void Spacing_ChangedAfterFirstMeasure_IsReapplied()
    {
        // Arrange
        var grid = new GridEx
        {
            Columns = "*,*",
            Rows = "*,*",
            Spacing = 10,
        };

        var child = new Border();
        grid.Children.Add(child);
        grid.Measure(new Size(200, 200));
        Assert.Equal(new Thickness(0, 0, 5, 5), child.Margin);

        // Act
        grid.Spacing = 20;
        grid.Measure(new Size(200, 200));

        // Assert
        Assert.Equal(new Thickness(0, 0, 10, 10), child.Margin);
    }

    [StaFact]
    public void Spacing_ChildMovedToLastCell_GetsOnlyLeadingSpacing()
    {
        // Arrange
        var grid = new GridEx
        {
            Columns = "*,*",
            Rows = "*,*",
            Spacing = 10,
        };

        var child = new Border();
        grid.Children.Add(child);
        grid.Measure(new Size(200, 200));

        // Act
        Grid.SetColumn(child, 1);
        Grid.SetRow(child, 1);
        grid.Measure(new Size(200, 200));

        // Assert
        Assert.Equal(new Thickness(5, 5, 0, 0), child.Margin);
    }

    [StaFact]
    public void Spacing_RemovedAfterFirstMeasure_ClearsAppliedMargin()
    {
        // Arrange
        var grid = new GridEx
        {
            Columns = "*,*",
            Spacing = 10,
        };

        var child = new Border();
        grid.Children.Add(child);
        grid.Measure(new Size(200, 200));

        // Act
        grid.Spacing = double.NaN;
        grid.Measure(new Size(200, 200));

        // Assert
        Assert.Equal(new Thickness(0), child.Margin);
    }

    [StaFact]
    public void Spacing_UserMargin_IsPreserved()
    {
        // Arrange
        var grid = new GridEx
        {
            Columns = "*,*",
            Rows = "*,*",
            Spacing = 10,
        };

        var userChild = new Border { Margin = new Thickness(5) };
        grid.Children.Add(userChild);
        grid.Measure(new Size(200, 200));

        // Act
        grid.Spacing = 20;
        grid.Measure(new Size(200, 200));

        // Assert
        Assert.Equal(new Thickness(5), userChild.Margin);
    }

    [StaFact]
    public void Spacing_UserMarginSetAfterSpacingApplied_IsPreserved()
    {
        // Arrange
        var grid = new GridEx
        {
            Columns = "*,*",
            Spacing = 10,
        };

        var child = new Border();
        grid.Children.Add(child);
        grid.Measure(new Size(200, 200));

        // Act
        child.Margin = new Thickness(3);
        grid.Spacing = 20;
        grid.Measure(new Size(200, 200));

        // Assert
        Assert.Equal(new Thickness(3), child.Margin);
    }

    [StaFact]
    public void Spacing_EqualStarColumns_GiveTheContentEqualWidths()
    {
        // Arrange
        var grid = new GridEx
        {
            Columns = "*,*",
            Spacing = 10,
        };

        var left = new Border();
        var right = new Border();
        Grid.SetColumn(right, 1);
        grid.Children.Add(left);
        grid.Children.Add(right);

        // Act
        grid.Measure(new Size(210, 50));
        grid.Arrange(new Rect(0, 0, 210, 50));

        // Assert
        Assert.Equal(100, left.ActualWidth, precision: 6);
        Assert.Equal(100, right.ActualWidth, precision: 6);
        Assert.Equal(10, right.TranslatePoint(default, grid).X - left.ActualWidth, precision: 6);
    }

    [StaFact]
    public void Spacing_ThreeColumnsAndASpan_KeepEqualCellsAndGaps()
    {
        // Arrange
        var grid = new GridEx
        {
            Columns = "*,*,*",
            Rows = "Auto,Auto",
            Spacing = 10,
        };

        var cells = Enumerable.Range(0, 3).Select(_ => new Border { Height = 10 }).ToList();
        for (var i = 0; i < cells.Count; i++)
        {
            Grid.SetColumn(cells[i], i);
            grid.Children.Add(cells[i]);
        }

        var spanning = new Border { Height = 10 };
        Grid.SetRow(spanning, 1);
        Grid.SetColumnSpan(spanning, 2);
        grid.Children.Add(spanning);

        // Act
        grid.Measure(new Size(320, 100));
        grid.Arrange(new Rect(0, 0, 320, 100));

        // Assert
        Assert.All(cells, x => Assert.Equal(100, x.ActualWidth, precision: 6));
        Assert.Equal(0, cells[0].TranslatePoint(default, grid).X, precision: 6);
        Assert.Equal(110, cells[1].TranslatePoint(default, grid).X, precision: 6);
        Assert.Equal(220, cells[2].TranslatePoint(default, grid).X, precision: 6);
        Assert.Equal(210, spanning.ActualWidth, precision: 6);
    }

    [Theory]
    [InlineData(10, 0, 1, 2, 0, 5)]
    [InlineData(10, 1, 1, 2, 5, 0)]
    [InlineData(9, 1, 1, 3, 3, 3)]
    [InlineData(9, 0, 2, 3, 0, 3)]
    [InlineData(10, 0, 1, 1, 0, 0)]
    [InlineData(10, 5, 1, 2, 5, 0)]
    public void SplitGap_SharesEachGapBetweenNeighbours(
        double spacing,
        int index,
        int span,
        int trackCount,
        double expectedLeading,
        double expectedTrailing)
    {
        // Act
        var (leading, trailing) = GridEx.SplitGap(spacing, index, span, trackCount);

        // Assert
        Assert.Equal(expectedLeading, leading, precision: 6);
        Assert.Equal(expectedTrailing, trailing, precision: 6);
    }
}