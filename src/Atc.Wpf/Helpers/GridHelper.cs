namespace Atc.Wpf.Helpers;

/// <summary>Provides helper methods for grid layout calculations.</summary>
public static class GridHelper
{
    /// <summary>Calculates the number of rows needed to arrange the given number of items in a grid suited for a 4:3 screen format.</summary>
    public static int CalculatorRowCountByScreenFormat43(int itemCount)
        => CalculatorRowCountByScreenFormat(itemCount, 1);

    /// <summary>Calculates the number of rows needed to arrange the given number of items in a grid suited for a 16:9 screen format.</summary>
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