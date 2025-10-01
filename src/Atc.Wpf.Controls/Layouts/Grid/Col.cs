// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
namespace Atc.Wpf.Controls.Layouts.Grid;

public sealed partial class Col : ContentControl
{
    [DependencyProperty]
    private ColLayout layout;

    [DependencyProperty(DefaultValue = 0)]
    private int offset;

    [DependencyProperty(
        DefaultValue = 24,
        ValidateValueCallback = nameof(OnSpanValidate))]
    private int span;

    [DependencyProperty(DefaultValue = false)]
    private bool isFixed;

    private static bool OnSpanValidate(object value)
    {
        var v = (int)value;
        return v is >= 1 and <= 24;
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