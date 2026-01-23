namespace Atc.Wpf.Controls.Tests.Layouts;

public sealed class FlexPanelTests
{
    #region Constructor Tests

    [StaFact]
    public void Constructor_SetsDefaultValues()
    {
        // Arrange & Act
        var panel = new FlexPanel();

        // Assert
        Assert.Equal(FlexDirection.Row, panel.Direction);
        Assert.Equal(FlexJustify.Start, panel.JustifyContent);
        Assert.Equal(FlexAlign.Stretch, panel.AlignItems);
        Assert.Equal(FlexWrap.NoWrap, panel.Wrap);
        Assert.Equal(0.0, panel.Gap);
        Assert.True(double.IsNaN(panel.RowGap));
        Assert.True(double.IsNaN(panel.ColumnGap));
    }

    #endregion

    #region Direction Property Tests

    [StaTheory]
    [InlineData(FlexDirection.Row)]
    [InlineData(FlexDirection.Column)]
    [InlineData(FlexDirection.RowReverse)]
    [InlineData(FlexDirection.ColumnReverse)]
    public void Direction_CanBeSet(FlexDirection expected)
    {
        // Arrange
        var panel = new FlexPanel();

        // Act
        panel.Direction = expected;

        // Assert
        Assert.Equal(expected, panel.Direction);
    }

    #endregion

    #region JustifyContent Property Tests

    [StaTheory]
    [InlineData(FlexJustify.Start)]
    [InlineData(FlexJustify.End)]
    [InlineData(FlexJustify.Center)]
    [InlineData(FlexJustify.SpaceBetween)]
    [InlineData(FlexJustify.SpaceAround)]
    [InlineData(FlexJustify.SpaceEvenly)]
    public void JustifyContent_CanBeSet(FlexJustify expected)
    {
        // Arrange
        var panel = new FlexPanel();

        // Act
        panel.JustifyContent = expected;

        // Assert
        Assert.Equal(expected, panel.JustifyContent);
    }

    #endregion

    #region AlignItems Property Tests

    [StaTheory]
    [InlineData(FlexAlign.Auto)]
    [InlineData(FlexAlign.Stretch)]
    [InlineData(FlexAlign.Start)]
    [InlineData(FlexAlign.End)]
    [InlineData(FlexAlign.Center)]
    [InlineData(FlexAlign.Baseline)]
    public void AlignItems_CanBeSet(FlexAlign expected)
    {
        // Arrange
        var panel = new FlexPanel();

        // Act
        panel.AlignItems = expected;

        // Assert
        Assert.Equal(expected, panel.AlignItems);
    }

    #endregion

    #region Wrap Property Tests

    [StaTheory]
    [InlineData(FlexWrap.NoWrap)]
    [InlineData(FlexWrap.Wrap)]
    [InlineData(FlexWrap.WrapReverse)]
    public void Wrap_CanBeSet(FlexWrap expected)
    {
        // Arrange
        var panel = new FlexPanel();

        // Act
        panel.Wrap = expected;

        // Assert
        Assert.Equal(expected, panel.Wrap);
    }

    #endregion

    #region Gap Property Tests

    [StaTheory]
    [InlineData(0.0)]
    [InlineData(5.0)]
    [InlineData(10.0)]
    [InlineData(20.5)]
    public void Gap_CanBeSet(double expected)
    {
        // Arrange
        var panel = new FlexPanel();

        // Act
        panel.Gap = expected;

        // Assert
        Assert.Equal(expected, panel.Gap);
    }

    [StaTheory]
    [InlineData(0.0)]
    [InlineData(5.0)]
    [InlineData(10.0)]
    public void RowGap_CanBeSet(double expected)
    {
        // Arrange
        var panel = new FlexPanel();

        // Act
        panel.RowGap = expected;

        // Assert
        Assert.Equal(expected, panel.RowGap);
    }

    [StaTheory]
    [InlineData(0.0)]
    [InlineData(5.0)]
    [InlineData(10.0)]
    public void ColumnGap_CanBeSet(double expected)
    {
        // Arrange
        var panel = new FlexPanel();

        // Act
        panel.ColumnGap = expected;

        // Assert
        Assert.Equal(expected, panel.ColumnGap);
    }

    #endregion

    #region Attached Property Tests

    [StaTheory]
    [InlineData(0.0)]
    [InlineData(1.0)]
    [InlineData(2.0)]
    [InlineData(0.5)]
    public void Grow_AttachedProperty_CanBeGetAndSet(double expected)
    {
        // Arrange
        var child = new Border();

        // Act
        FlexPanel.SetGrow(child, expected);
        var actual = FlexPanel.GetGrow(child);

        // Assert
        Assert.Equal(expected, actual);
    }

    [StaTheory]
    [InlineData(0.0)]
    [InlineData(1.0)]
    [InlineData(2.0)]
    public void Shrink_AttachedProperty_CanBeGetAndSet(double expected)
    {
        // Arrange
        var child = new Border();

        // Act
        FlexPanel.SetShrink(child, expected);
        var actual = FlexPanel.GetShrink(child);

        // Assert
        Assert.Equal(expected, actual);
    }

    [StaFact]
    public void Shrink_AttachedProperty_DefaultIsOne()
    {
        // Arrange
        var child = new Border();

        // Act
        var actual = FlexPanel.GetShrink(child);

        // Assert
        Assert.Equal(1.0, actual);
    }

    [StaTheory]
    [InlineData(0.0)]
    [InlineData(50.0)]
    [InlineData(100.0)]
    public void Basis_AttachedProperty_CanBeGetAndSet(double expected)
    {
        // Arrange
        var child = new Border();

        // Act
        FlexPanel.SetBasis(child, expected);
        var actual = FlexPanel.GetBasis(child);

        // Assert
        Assert.Equal(expected, actual);
    }

    [StaFact]
    public void Basis_AttachedProperty_DefaultIsNaN()
    {
        // Arrange
        var child = new Border();

        // Act
        var actual = FlexPanel.GetBasis(child);

        // Assert
        Assert.True(double.IsNaN(actual));
    }

    [StaTheory]
    [InlineData(FlexAlign.Auto)]
    [InlineData(FlexAlign.Stretch)]
    [InlineData(FlexAlign.Start)]
    [InlineData(FlexAlign.End)]
    [InlineData(FlexAlign.Center)]
    public void AlignSelf_AttachedProperty_CanBeGetAndSet(FlexAlign expected)
    {
        // Arrange
        var child = new Border();

        // Act
        FlexPanel.SetAlignSelf(child, expected);
        var actual = FlexPanel.GetAlignSelf(child);

        // Assert
        Assert.Equal(expected, actual);
    }

    [StaFact]
    public void AlignSelf_AttachedProperty_DefaultIsAuto()
    {
        // Arrange
        var child = new Border();

        // Act
        var actual = FlexPanel.GetAlignSelf(child);

        // Assert
        Assert.Equal(FlexAlign.Auto, actual);
    }

    #endregion

    #region Measure Tests

    [StaFact]
    public void MeasureOverride_EmptyPanel_ReturnsZeroSize()
    {
        // Arrange
        var panel = new FlexPanel();

        // Act
        panel.Measure(new Size(100, 100));

        // Assert
        Assert.Equal(0, panel.DesiredSize.Width);
        Assert.Equal(0, panel.DesiredSize.Height);
    }

    [StaFact]
    public void MeasureOverride_SingleChild_ReturnChildSize()
    {
        // Arrange
        var panel = new FlexPanel();
        var child = new Border { Width = 50, Height = 30 };
        panel.Children.Add(child);

        // Act
        panel.Measure(new Size(200, 200));

        // Assert
        Assert.Equal(50, panel.DesiredSize.Width);
        Assert.Equal(30, panel.DesiredSize.Height);
    }

    [StaFact]
    public void MeasureOverride_MultipleChildren_Row_SumsWidthMaxHeight()
    {
        // Arrange
        var panel = new FlexPanel { Direction = FlexDirection.Row };
        panel.Children.Add(new Border { Width = 50, Height = 30 });
        panel.Children.Add(new Border { Width = 40, Height = 50 });

        // Act
        panel.Measure(new Size(200, 200));

        // Assert
        Assert.Equal(90, panel.DesiredSize.Width);
        Assert.Equal(50, panel.DesiredSize.Height);
    }

    [StaFact]
    public void MeasureOverride_MultipleChildren_Column_SumsHeightMaxWidth()
    {
        // Arrange
        var panel = new FlexPanel { Direction = FlexDirection.Column };
        panel.Children.Add(new Border { Width = 50, Height = 30 });
        panel.Children.Add(new Border { Width = 40, Height = 50 });

        // Act
        panel.Measure(new Size(200, 200));

        // Assert
        Assert.Equal(50, panel.DesiredSize.Width);
        Assert.Equal(80, panel.DesiredSize.Height);
    }

    [StaFact]
    public void MeasureOverride_WithGap_IncludesGapInSize()
    {
        // Arrange
        var panel = new FlexPanel { Direction = FlexDirection.Row, Gap = 10 };
        panel.Children.Add(new Border { Width = 50, Height = 30 });
        panel.Children.Add(new Border { Width = 40, Height = 30 });

        // Act
        panel.Measure(new Size(200, 200));

        // Assert
        Assert.Equal(100, panel.DesiredSize.Width); // 50 + 10 + 40
    }

    #endregion

    #region Arrange Tests

    [StaFact]
    public void ArrangeOverride_JustifyStart_ChildrenAtStart()
    {
        // Arrange
        var panel = new FlexPanel
        {
            Direction = FlexDirection.Row,
            JustifyContent = FlexJustify.Start,
        };
        var child1 = new Border { Width = 50, Height = 30 };
        var child2 = new Border { Width = 40, Height = 30 };
        panel.Children.Add(child1);
        panel.Children.Add(child2);

        // Act
        panel.Measure(new Size(200, 100));
        panel.Arrange(new Rect(0, 0, 200, 100));

        // Assert
        var transform1 = child1.TransformToAncestor(panel);
        var transform2 = child2.TransformToAncestor(panel);
        var point1 = transform1.Transform(new Point(0, 0));
        var point2 = transform2.Transform(new Point(0, 0));

        Assert.Equal(0, point1.X);
        Assert.Equal(50, point2.X);
    }

    [StaFact]
    public void ArrangeOverride_JustifyEnd_ChildrenAtEnd()
    {
        // Arrange
        var panel = new FlexPanel
        {
            Direction = FlexDirection.Row,
            JustifyContent = FlexJustify.End,
        };
        var child1 = new Border { Width = 50, Height = 30 };
        var child2 = new Border { Width = 40, Height = 30 };
        panel.Children.Add(child1);
        panel.Children.Add(child2);

        // Act
        panel.Measure(new Size(200, 100));
        panel.Arrange(new Rect(0, 0, 200, 100));

        // Assert
        var transform1 = child1.TransformToAncestor(panel);
        var transform2 = child2.TransformToAncestor(panel);
        var point1 = transform1.Transform(new Point(0, 0));
        var point2 = transform2.Transform(new Point(0, 0));

        Assert.Equal(110, point1.X); // 200 - 90 = 110
        Assert.Equal(160, point2.X); // 200 - 40 = 160
    }

    [StaFact]
    public void ArrangeOverride_JustifyCenter_ChildrenCentered()
    {
        // Arrange
        var panel = new FlexPanel
        {
            Direction = FlexDirection.Row,
            JustifyContent = FlexJustify.Center,
        };
        var child1 = new Border { Width = 50, Height = 30 };
        var child2 = new Border { Width = 40, Height = 30 };
        panel.Children.Add(child1);
        panel.Children.Add(child2);

        // Act
        panel.Measure(new Size(200, 100));
        panel.Arrange(new Rect(0, 0, 200, 100));

        // Assert
        var transform1 = child1.TransformToAncestor(panel);
        var point1 = transform1.Transform(new Point(0, 0));

        Assert.Equal(55, point1.X); // (200 - 90) / 2 = 55
    }

    #endregion
}