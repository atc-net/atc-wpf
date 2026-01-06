namespace Atc.Wpf.Helpers;

public static class GridHelper
{
    public static int CalculatorRowCountByScreenFormat43(int itemCount)
        => CalculatorRowCountByScreenFormat(itemCount, 1);

    public static int CalculatorRowCountByScreenFormat169(int itemCount)
        => CalculatorRowCountByScreenFormat(itemCount, 2);

    private static int CalculatorRowCountByScreenFormat(
        int itemCount,
        int widthFactor)
    {
        if (itemCount <= 1)
        {
            return 1;
        }

        for (var colCount = itemCount; colCount > 0; colCount--)
        {
            var rowCount = (itemCount + colCount - 1) / colCount;

            if (colCount > rowCount + widthFactor)
            {
                continue;
            }

            return rowCount;
        }

        return 1;
    }
}