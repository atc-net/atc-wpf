namespace Atc.Wpf.Tests.Helpers;

[SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
public class GridHelperTests
{
    [Fact]
    public void CalculatorRowCountByScreenFormat43()
    {
        var testData = GetTestDataByScreenFormat43();
        foreach (var (expectedRows, itemCount) in testData)
        {
            Assert.Equal(expectedRows, GridHelper.CalculatorRowCountByScreenFormat43(itemCount));
        }
    }

    [Fact]
    public void CalculatorRowCountByScreenFormat169()
    {
        var testData = GetTestDataByScreenFormat169();
        foreach (var (expectedRows, itemCount) in testData)
        {
            Assert.Equal(expectedRows, GridHelper.CalculatorRowCountByScreenFormat169(itemCount));
        }
    }

    private static IEnumerable<Tuple<int, int>> GetTestDataByScreenFormat43()
    {
        const string data = @"
#
----------
##
----------
##
#
----------
##
##
----------
###
##
----------
###
###
----------
###
###
#
----------
###
###
##
----------
###
###
###
----------
####
####
##
----------
####
####
###
----------
####
####
####
----------
####
####
####
#
----------
####
####
####
##
----------
####
####
####
###
----------
####
####
####
####
----------
#####
#####
#####
##
----------
#####
#####
#####
###
----------
#####
#####
#####
###
----------
#####
#####
#####
####
----------
#####
#####
#####
#####
----------
#####
#####
#####
#####
#
----------
#####
#####
#####
#####
##
----------
#####
#####
#####
#####
###
----------
#####
#####
#####
#####
####
----------
#####
#####
#####
#####
#####
";
        return GetTestDataByScreenFormat(data);
    }

    private static IEnumerable<Tuple<int, int>> GetTestDataByScreenFormat169()
    {
        const string data = @"
#
----------
##
----------
###
----------
###
#
----------
####
#
----------
####
##
----------
####
###
----------
####
####
----------
####
####
#
----------
####
####
##
----------
#####
#####
#
----------
#####
#####
##
----------
#####
#####
###
----------
#####
#####
####
----------
#####
#####
#####
----------
#####
#####
#####
#
----------
#####
#####
#####
##
----------
#####
#####
#####
###
----------
######
######
######
#
----------
######
######
######
##
----------
######
######
######
###
----------
######
######
######
####
----------
######
######
######
#####
----------
######
######
######
######
----------
######
######
######
######
#";
        return GetTestDataByScreenFormat(data);
    }

    private static IEnumerable<Tuple<int, int>> GetTestDataByScreenFormat(
        string data)
    {
        var testCases = data.Split("----------");

        var list = new List<Tuple<int, int>>();
        foreach (var testCase in testCases)
        {
            var gridLines = testCase.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            int rows = gridLines.Length;
            int elements = testCase.Count(f => f == '#');

            list.Add(new Tuple<int, int>(rows, elements));
        }

        return list;
    }
}