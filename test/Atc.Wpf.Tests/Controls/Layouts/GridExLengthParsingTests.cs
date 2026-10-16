namespace Atc.Wpf.Tests.Controls.Layouts;

public sealed class GridExLengthParsingTests
{
    [StaFact]
    public void Rows_WithDecimalStar_IsParsedInvariantOfTheCurrentCulture()
    {
        RunWithCulture("da-DK", () =>
        {
            var grid = new GridEx();

            grid.Rows = "1.5*,*";

            Assert.Equal(2, grid.RowDefinitions.Count);
            Assert.True(grid.RowDefinitions[0].Height.IsStar);
            Assert.Equal(1.5, grid.RowDefinitions[0].Height.Value);
            Assert.Equal(1.0, grid.RowDefinitions[1].Height.Value);
        });
    }

    [StaFact]
    public void Columns_WithDecimalPixels_IsParsedInvariantOfTheCurrentCulture()
    {
        RunWithCulture("da-DK", () =>
        {
            var grid = new GridEx();

            grid.Columns = "12.5,Auto";

            Assert.Equal(2, grid.ColumnDefinitions.Count);
            Assert.True(grid.ColumnDefinitions[0].Width.IsAbsolute);
            Assert.Equal(12.5, grid.ColumnDefinitions[0].Width.Value);
            Assert.True(grid.ColumnDefinitions[1].Width.IsAuto);
        });
    }

    [StaFact]
    public void Rows_WithTrailingComma_IgnoresTheEmptyToken()
    {
        var grid = new GridEx();

        grid.Rows = "Auto,*,";

        Assert.Equal(2, grid.RowDefinitions.Count);
        Assert.True(grid.RowDefinitions[0].Height.IsAuto);
        Assert.True(grid.RowDefinitions[1].Height.IsStar);
    }

    [StaFact]
    public void Columns_WithSpacesAroundTokens_TrimsTheTokens()
    {
        var grid = new GridEx();

        grid.Columns = " 2* , Auto , 100 ";

        Assert.Equal(3, grid.ColumnDefinitions.Count);
        Assert.Equal(2.0, grid.ColumnDefinitions[0].Width.Value);
        Assert.True(grid.ColumnDefinitions[1].Width.IsAuto);
        Assert.Equal(100.0, grid.ColumnDefinitions[2].Width.Value);
    }

    [StaFact]
    public void Rows_WithInvalidToken_FallsBackToAutoInsteadOfThrowing()
    {
        var grid = new GridEx();

        grid.Rows = "*,bogus,2*";

        Assert.Equal(3, grid.RowDefinitions.Count);
        Assert.True(grid.RowDefinitions[1].Height.IsAuto);
        Assert.Equal(2.0, grid.RowDefinitions[2].Height.Value);
    }

    private static void RunWithCulture(
        string cultureName,
        Action action)
    {
        var previousCulture = CultureInfo.CurrentCulture;
        var previousUiCulture = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo(cultureName);
            CultureInfo.CurrentUICulture = new CultureInfo(cultureName);
            action();
        }
        finally
        {
            CultureInfo.CurrentCulture = previousCulture;
            CultureInfo.CurrentUICulture = previousUiCulture;
        }
    }
}