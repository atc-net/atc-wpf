namespace Atc.Wpf.Controls.Tests.Layouts;

public sealed class ResponsivePanelTests
{
    #region Constructor Tests

    [StaFact]
    public void Constructor_SetsDefaultValues()
    {
        // Arrange & Act
        var panel = new ResponsivePanel();

        // Assert
        Assert.Equal(0.0, panel.Gap);
        Assert.True(double.IsNaN(panel.RowGap));
        Assert.True(double.IsNaN(panel.ColumnGap));
        Assert.True(double.IsNaN(panel.MinItemWidth));
        Assert.Equal(1, panel.ColumnsXs);
        Assert.Equal(2, panel.ColumnsSm);
        Assert.Equal(3, panel.ColumnsMd);
        Assert.Equal(4, panel.ColumnsLg);
        Assert.Equal(6, panel.ColumnsXl);
    }

    #endregion

    #region Breakpoint Constants

    [Fact]
    public void BreakpointConstants_HaveExpectedValues()
    {
        Assert.Equal(576, ResponsivePanel.XsMaxWidth);
        Assert.Equal(768, ResponsivePanel.SmMaxWidth);
        Assert.Equal(992, ResponsivePanel.MdMaxWidth);
        Assert.Equal(1200, ResponsivePanel.LgMaxWidth);
        Assert.Equal(12, ResponsivePanel.MaxColumns);
    }

    #endregion

    #region GetBreakpoint Tests

    [Theory]
    [InlineData(0, ResponsiveBreakpoint.Xs)]
    [InlineData(400, ResponsiveBreakpoint.Xs)]
    [InlineData(575, ResponsiveBreakpoint.Xs)]
    [InlineData(576, ResponsiveBreakpoint.Sm)]
    [InlineData(700, ResponsiveBreakpoint.Sm)]
    [InlineData(767, ResponsiveBreakpoint.Sm)]
    [InlineData(768, ResponsiveBreakpoint.Md)]
    [InlineData(900, ResponsiveBreakpoint.Md)]
    [InlineData(991, ResponsiveBreakpoint.Md)]
    [InlineData(992, ResponsiveBreakpoint.Lg)]
    [InlineData(1100, ResponsiveBreakpoint.Lg)]
    [InlineData(1199, ResponsiveBreakpoint.Lg)]
    [InlineData(1200, ResponsiveBreakpoint.Xl)]
    [InlineData(1920, ResponsiveBreakpoint.Xl)]
    [InlineData(2560, ResponsiveBreakpoint.Xl)]
    public void GetBreakpoint_ReturnsCorrectBreakpoint(
        double width,
        ResponsiveBreakpoint expected)
    {
        // Act
        var actual = ResponsivePanel.GetBreakpoint(width);

        // Assert
        Assert.Equal(expected, actual);
    }

    #endregion

    #region Gap Property Tests

    [StaTheory]
    [InlineData(0.0)]
    [InlineData(8.0)]
    [InlineData(16.0)]
    [InlineData(24.5)]
    public void Gap_CanBeSet(double expected)
    {
        // Arrange
        var panel = new ResponsivePanel();

        // Act
        panel.Gap = expected;

        // Assert
        Assert.Equal(expected, panel.Gap);
    }

    [StaTheory]
    [InlineData(0.0)]
    [InlineData(8.0)]
    [InlineData(16.0)]
    public void RowGap_CanBeSet(double expected)
    {
        // Arrange
        var panel = new ResponsivePanel();

        // Act
        panel.RowGap = expected;

        // Assert
        Assert.Equal(expected, panel.RowGap);
    }

    [StaTheory]
    [InlineData(0.0)]
    [InlineData(8.0)]
    [InlineData(16.0)]
    public void ColumnGap_CanBeSet(double expected)
    {
        // Arrange
        var panel = new ResponsivePanel();

        // Act
        panel.ColumnGap = expected;

        // Assert
        Assert.Equal(expected, panel.ColumnGap);
    }

    #endregion

    #region MinItemWidth Property Tests

    [StaTheory]
    [InlineData(100.0)]
    [InlineData(200.0)]
    [InlineData(280.0)]
    public void MinItemWidth_CanBeSet(double expected)
    {
        // Arrange
        var panel = new ResponsivePanel();

        // Act
        panel.MinItemWidth = expected;

        // Assert
        Assert.Equal(expected, panel.MinItemWidth);
    }

    #endregion

    #region Column Count Property Tests

    [StaTheory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(6)]
    [InlineData(12)]
    public void ColumnsXs_CanBeSet(int expected)
    {
        // Arrange
        var panel = new ResponsivePanel();

        // Act
        panel.ColumnsXs = expected;

        // Assert
        Assert.Equal(expected, panel.ColumnsXs);
    }

    [StaTheory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(6)]
    [InlineData(12)]
    public void ColumnsSm_CanBeSet(int expected)
    {
        // Arrange
        var panel = new ResponsivePanel();

        // Act
        panel.ColumnsSm = expected;

        // Assert
        Assert.Equal(expected, panel.ColumnsSm);
    }

    [StaTheory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(6)]
    [InlineData(12)]
    public void ColumnsMd_CanBeSet(int expected)
    {
        // Arrange
        var panel = new ResponsivePanel();

        // Act
        panel.ColumnsMd = expected;

        // Assert
        Assert.Equal(expected, panel.ColumnsMd);
    }

    [StaTheory]
    [InlineData(1)]
    [InlineData(4)]
    [InlineData(6)]
    [InlineData(12)]
    public void ColumnsLg_CanBeSet(int expected)
    {
        // Arrange
        var panel = new ResponsivePanel();

        // Act
        panel.ColumnsLg = expected;

        // Assert
        Assert.Equal(expected, panel.ColumnsLg);
    }

    [StaTheory]
    [InlineData(1)]
    [InlineData(6)]
    [InlineData(8)]
    [InlineData(12)]
    public void ColumnsXl_CanBeSet(int expected)
    {
        // Arrange
        var panel = new ResponsivePanel();

        // Act
        panel.ColumnsXl = expected;

        // Assert
        Assert.Equal(expected, panel.ColumnsXl);
    }

    #endregion

    #region Span Attached Property Tests

    [StaTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(6)]
    [InlineData(12)]
    public void SpanXs_AttachedProperty_CanBeGetAndSet(int expected)
    {
        // Arrange
        var child = new Border();

        // Act
        ResponsivePanel.SetSpanXs(child, expected);
        var actual = ResponsivePanel.GetSpanXs(child);

        // Assert
        Assert.Equal(expected, actual);
    }

    [StaTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(6)]
    [InlineData(12)]
    public void SpanSm_AttachedProperty_CanBeGetAndSet(int expected)
    {
        // Arrange
        var child = new Border();

        // Act
        ResponsivePanel.SetSpanSm(child, expected);
        var actual = ResponsivePanel.GetSpanSm(child);

        // Assert
        Assert.Equal(expected, actual);
    }

    [StaTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(6)]
    [InlineData(12)]
    public void SpanMd_AttachedProperty_CanBeGetAndSet(int expected)
    {
        // Arrange
        var child = new Border();

        // Act
        ResponsivePanel.SetSpanMd(child, expected);
        var actual = ResponsivePanel.GetSpanMd(child);

        // Assert
        Assert.Equal(expected, actual);
    }

    [StaTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(6)]
    [InlineData(12)]
    public void SpanLg_AttachedProperty_CanBeGetAndSet(int expected)
    {
        // Arrange
        var child = new Border();

        // Act
        ResponsivePanel.SetSpanLg(child, expected);
        var actual = ResponsivePanel.GetSpanLg(child);

        // Assert
        Assert.Equal(expected, actual);
    }

    [StaTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(6)]
    [InlineData(12)]
    public void SpanXl_AttachedProperty_CanBeGetAndSet(int expected)
    {
        // Arrange
        var child = new Border();

        // Act
        ResponsivePanel.SetSpanXl(child, expected);
        var actual = ResponsivePanel.GetSpanXl(child);

        // Assert
        Assert.Equal(expected, actual);
    }

    [StaFact]
    public void Span_AttachedProperties_DefaultToZero()
    {
        // Arrange
        var child = new Border();

        // Assert
        Assert.Equal(0, ResponsivePanel.GetSpanXs(child));
        Assert.Equal(0, ResponsivePanel.GetSpanSm(child));
        Assert.Equal(0, ResponsivePanel.GetSpanMd(child));
        Assert.Equal(0, ResponsivePanel.GetSpanLg(child));
        Assert.Equal(0, ResponsivePanel.GetSpanXl(child));
    }

    #endregion

    #region Visibility Attached Property Tests

    [StaTheory]
    [InlineData(ResponsiveBreakpoint.Xs)]
    [InlineData(ResponsiveBreakpoint.Sm)]
    [InlineData(ResponsiveBreakpoint.Md)]
    [InlineData(ResponsiveBreakpoint.Lg)]
    [InlineData(ResponsiveBreakpoint.Xl)]
    public void VisibleFrom_AttachedProperty_CanBeGetAndSet(
        ResponsiveBreakpoint expected)
    {
        // Arrange
        var child = new Border();

        // Act
        ResponsivePanel.SetVisibleFrom(child, expected);
        var actual = ResponsivePanel.GetVisibleFrom(child);

        // Assert
        Assert.Equal(expected, actual);
    }

    [StaTheory]
    [InlineData(ResponsiveBreakpoint.Xs)]
    [InlineData(ResponsiveBreakpoint.Sm)]
    [InlineData(ResponsiveBreakpoint.Md)]
    [InlineData(ResponsiveBreakpoint.Lg)]
    [InlineData(ResponsiveBreakpoint.Xl)]
    public void HiddenFrom_AttachedProperty_CanBeGetAndSet(
        ResponsiveBreakpoint expected)
    {
        // Arrange
        var child = new Border();

        // Act
        ResponsivePanel.SetHiddenFrom(child, expected);
        var actual = ResponsivePanel.GetHiddenFrom(child);

        // Assert
        Assert.Equal(expected, actual);
    }

    [StaFact]
    public void VisibleFrom_AttachedProperty_DefaultsToNull()
    {
        // Arrange
        var child = new Border();

        // Act
        var actual = ResponsivePanel.GetVisibleFrom(child);

        // Assert
        Assert.Null(actual);
    }

    [StaFact]
    public void HiddenFrom_AttachedProperty_DefaultsToNull()
    {
        // Arrange
        var child = new Border();

        // Act
        var actual = ResponsivePanel.GetHiddenFrom(child);

        // Assert
        Assert.Null(actual);
    }

    #endregion

    #region Order Attached Property Tests

    [StaTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void OrderXs_AttachedProperty_CanBeGetAndSet(int expected)
    {
        // Arrange
        var child = new Border();

        // Act
        ResponsivePanel.SetOrderXs(child, expected);
        var actual = ResponsivePanel.GetOrderXs(child);

        // Assert
        Assert.Equal(expected, actual);
    }

    [StaTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void OrderSm_AttachedProperty_CanBeGetAndSet(int expected)
    {
        // Arrange
        var child = new Border();

        // Act
        ResponsivePanel.SetOrderSm(child, expected);
        var actual = ResponsivePanel.GetOrderSm(child);

        // Assert
        Assert.Equal(expected, actual);
    }

    [StaTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void OrderMd_AttachedProperty_CanBeGetAndSet(int expected)
    {
        // Arrange
        var child = new Border();

        // Act
        ResponsivePanel.SetOrderMd(child, expected);
        var actual = ResponsivePanel.GetOrderMd(child);

        // Assert
        Assert.Equal(expected, actual);
    }

    [StaTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void OrderLg_AttachedProperty_CanBeGetAndSet(int expected)
    {
        // Arrange
        var child = new Border();

        // Act
        ResponsivePanel.SetOrderLg(child, expected);
        var actual = ResponsivePanel.GetOrderLg(child);

        // Assert
        Assert.Equal(expected, actual);
    }

    [StaTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void OrderXl_AttachedProperty_CanBeGetAndSet(int expected)
    {
        // Arrange
        var child = new Border();

        // Act
        ResponsivePanel.SetOrderXl(child, expected);
        var actual = ResponsivePanel.GetOrderXl(child);

        // Assert
        Assert.Equal(expected, actual);
    }

    [StaFact]
    public void Order_AttachedProperties_DefaultToMaxValue()
    {
        // Arrange
        var child = new Border();

        // Assert
        Assert.Equal(int.MaxValue, ResponsivePanel.GetOrderXs(child));
        Assert.Equal(int.MaxValue, ResponsivePanel.GetOrderSm(child));
        Assert.Equal(int.MaxValue, ResponsivePanel.GetOrderMd(child));
        Assert.Equal(int.MaxValue, ResponsivePanel.GetOrderLg(child));
        Assert.Equal(int.MaxValue, ResponsivePanel.GetOrderXl(child));
    }

    #endregion

    #region Measure Tests

    [StaFact]
    public void MeasureOverride_EmptyPanel_ReturnsZeroSize()
    {
        // Arrange
        var panel = new ResponsivePanel();

        // Act
        panel.Measure(new Size(1200, 800));

        // Assert
        Assert.Equal(0, panel.DesiredSize.Width);
        Assert.Equal(0, panel.DesiredSize.Height);
    }

    [StaFact]
    public void MeasureOverride_SingleChild_ReturnsChildHeight()
    {
        // Arrange
        var panel = new ResponsivePanel { ColumnsXl = 4 };
        var child = new Border { Height = 100 };
        panel.Children.Add(child);

        // Act
        panel.Measure(new Size(1200, 800));

        // Assert
        Assert.Equal(1200, panel.DesiredSize.Width);
        Assert.Equal(100, panel.DesiredSize.Height);
    }

    [StaFact]
    public void MeasureOverride_MultipleChildren_SingleRow_ReturnsMaxHeight()
    {
        // Arrange
        var panel = new ResponsivePanel { ColumnsXl = 4 };
        panel.Children.Add(new Border { Height = 50 });
        panel.Children.Add(new Border { Height = 100 });
        panel.Children.Add(new Border { Height = 75 });
        panel.Children.Add(new Border { Height = 60 });

        // Act
        panel.Measure(new Size(1200, 800));

        // Assert
        Assert.Equal(1200, panel.DesiredSize.Width);
        Assert.Equal(100, panel.DesiredSize.Height);
    }

    [StaFact]
    public void MeasureOverride_MultipleChildren_MultipleRows_IncludesGap()
    {
        // Arrange
        var panel = new ResponsivePanel
        {
            ColumnsXl = 2,
            Gap = 16,
        };
        panel.Children.Add(new Border { Height = 50 });
        panel.Children.Add(new Border { Height = 50 });
        panel.Children.Add(new Border { Height = 50 });
        panel.Children.Add(new Border { Height = 50 });

        // Act
        panel.Measure(new Size(1200, 800));

        // Assert
        Assert.Equal(1200, panel.DesiredSize.Width);
        Assert.Equal(116, panel.DesiredSize.Height); // 50 + 16 + 50 = 116
    }

    #endregion

    #region Arrange Tests

    [StaFact]
    public void ArrangeOverride_SingleChild_ArrangesFullWidth()
    {
        // Arrange
        var panel = new ResponsivePanel { ColumnsXl = 1 };
        var child = new Border { Height = 100 };
        panel.Children.Add(child);

        // Act
        panel.Measure(new Size(1200, 800));
        panel.Arrange(new Rect(0, 0, 1200, 800));

        // Assert
        var transform = child.TransformToAncestor(panel);
        var point = transform.Transform(new Point(0, 0));

        Assert.Equal(0, point.X);
        Assert.Equal(0, point.Y);
        Assert.Equal(1200, child.RenderSize.Width);
    }

    [StaFact]
    public void ArrangeOverride_TwoColumns_ChildrenArrangedSideBySide()
    {
        // Arrange
        var panel = new ResponsivePanel { ColumnsXl = 2, Gap = 16 };
        var child1 = new Border { Height = 100 };
        var child2 = new Border { Height = 100 };
        panel.Children.Add(child1);
        panel.Children.Add(child2);

        // Act
        panel.Measure(new Size(1200, 800));
        panel.Arrange(new Rect(0, 0, 1200, 800));

        // Assert
        var transform1 = child1.TransformToAncestor(panel);
        var transform2 = child2.TransformToAncestor(panel);
        var point1 = transform1.Transform(new Point(0, 0));
        var point2 = transform2.Transform(new Point(0, 0));

        Assert.Equal(0, point1.X);
        Assert.True(point2.X > 0);
        Assert.Equal(0, point1.Y);
        Assert.Equal(0, point2.Y);
    }

    [StaFact]
    public void ArrangeOverride_MoreChildrenThanColumns_Wraps()
    {
        // Arrange
        var panel = new ResponsivePanel { ColumnsXl = 2, Gap = 16 };
        var child1 = new Border { Height = 50 };
        var child2 = new Border { Height = 50 };
        var child3 = new Border { Height = 50 };
        panel.Children.Add(child1);
        panel.Children.Add(child2);
        panel.Children.Add(child3);

        // Act
        panel.Measure(new Size(1200, 800));
        panel.Arrange(new Rect(0, 0, 1200, 800));

        // Assert
        var transform1 = child1.TransformToAncestor(panel);
        var transform3 = child3.TransformToAncestor(panel);
        var point1 = transform1.Transform(new Point(0, 0));
        var point3 = transform3.Transform(new Point(0, 0));

        Assert.Equal(0, point1.Y);
        Assert.True(point3.Y > 0); // Third child should be on second row
    }

    #endregion
}