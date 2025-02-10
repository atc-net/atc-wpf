// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
namespace Atc.Wpf.Controls.Layouts.Grid;

public sealed class Col : ContentControl
{
    public static readonly DependencyProperty LayoutProperty = DependencyProperty.Register(
        nameof(Layout),
        typeof(ColLayout),
        typeof(Col),
        new PropertyMetadata(default(ColLayout)));

    public ColLayout Layout
    {
        get => (ColLayout)GetValue(LayoutProperty);
        set => SetValue(LayoutProperty, value);
    }

    public static readonly DependencyProperty OffsetProperty = DependencyProperty.Register(
        nameof(Offset),
        typeof(int),
        typeof(Col),
        new PropertyMetadata(0));

    public int Offset
    {
        get => (int)GetValue(OffsetProperty);
        set => SetValue(OffsetProperty, value);
    }

    public static readonly DependencyProperty SpanProperty = DependencyProperty.Register(
        nameof(Span),
        typeof(int),
        typeof(Col),
        new PropertyMetadata(24),
        OnSpanValidate);

    private static bool OnSpanValidate(object value)
    {
        var v = (int)value;
        return v is >= 1 and <= 24;
    }

    public int Span
    {
        get => (int)GetValue(SpanProperty);
        set => SetValue(SpanProperty, value);
    }

    public static readonly DependencyProperty IsFixedProperty = DependencyProperty.Register(
        nameof(IsFixed),
        typeof(bool),
        typeof(Col),
        new PropertyMetadata(defaultValue: BooleanBoxes.FalseBox));

    public bool IsFixed
    {
        get => (bool)GetValue(IsFixedProperty);
        set => SetValue(IsFixedProperty, value);
    }

    internal int GetLayoutCellCount(ColLayoutType status)
    {
        var result = 0;

        if (Layout == null)
        {
            result = Span;
        }
        else
        {
            if (IsFixed)
            {
                return result;
            }

            switch (status)
            {
                case ColLayoutType.Xs:
                    result = Layout.Xs;
                    break;
                case ColLayoutType.Sm:
                    result = Layout.Sm;
                    break;
                case ColLayoutType.Md:
                    result = Layout.Md;
                    break;
                case ColLayoutType.Lg:
                    result = Layout.Lg;
                    break;
                case ColLayoutType.Xl:
                    result = Layout.Xl;
                    break;
                case ColLayoutType.Xxl:
                    result = Layout.Xxl;
                    break;
                case ColLayoutType.Auto:
                    break;
                default:
                    throw new SwitchCaseDefaultException(status);
            }
        }

        return result;
    }
}