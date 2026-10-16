namespace Atc.Wpf.Tests.Helpers;

public sealed class TextBlockHelperTests
{
    [StaFact]
    public void SetInlines_CopiesRunsIntoTextBlock()
    {
        // Arrange
        var textBlock = new TextBlock();
        var runs = CreateRuns("A", "B", "C");

        // Act
        TextBlockHelper.SetInlines(textBlock, runs);

        // Assert
        Assert.Equal(["A", "B", "C"], GetTexts(textBlock), StringComparer.Ordinal);
    }

    [StaFact]
    public void Insert_AtStart_KeepsOrder()
    {
        // Arrange
        var textBlock = new TextBlock();
        var runs = CreateRuns("B", "C");
        TextBlockHelper.SetInlines(textBlock, runs);

        // Act
        runs.Insert(0, new Run("A"));

        // Assert
        Assert.Equal(["A", "B", "C"], GetTexts(textBlock), StringComparer.Ordinal);
    }

    [StaFact]
    public void Insert_InMiddle_KeepsOrder()
    {
        // Arrange
        var textBlock = new TextBlock();
        var runs = CreateRuns("A", "C");
        TextBlockHelper.SetInlines(textBlock, runs);

        // Act
        runs.Insert(1, new Run("B"));

        // Assert
        Assert.Equal(["A", "B", "C"], GetTexts(textBlock), StringComparer.Ordinal);
    }

    [StaFact]
    public void Add_AppendsAtEnd()
    {
        // Arrange
        var textBlock = new TextBlock();
        var runs = CreateRuns("A", "B");
        TextBlockHelper.SetInlines(textBlock, runs);

        // Act
        runs.Add(new Run("C"));

        // Assert
        Assert.Equal(["A", "B", "C"], GetTexts(textBlock), StringComparer.Ordinal);
    }

    [StaFact]
    public void Move_ReordersWithoutThrowing()
    {
        // Arrange
        var textBlock = new TextBlock();
        var runs = CreateRuns("A", "B", "C");
        TextBlockHelper.SetInlines(textBlock, runs);

        // Act
        runs.Move(0, 2);

        // Assert
        Assert.Equal(["B", "C", "A"], GetTexts(textBlock), StringComparer.Ordinal);
    }

    [StaFact]
    public void Move_TowardsStart_Reorders()
    {
        // Arrange
        var textBlock = new TextBlock();
        var runs = CreateRuns("A", "B", "C");
        TextBlockHelper.SetInlines(textBlock, runs);

        // Act
        runs.Move(2, 0);

        // Assert
        Assert.Equal(["C", "A", "B"], GetTexts(textBlock), StringComparer.Ordinal);
    }

    [StaFact]
    public void Replace_SwapsRunInPlace()
    {
        // Arrange
        var textBlock = new TextBlock();
        var runs = CreateRuns("A", "B", "C");
        TextBlockHelper.SetInlines(textBlock, runs);

        // Act
        runs[1] = new Run("X");

        // Assert
        Assert.Equal(["A", "X", "C"], GetTexts(textBlock), StringComparer.Ordinal);
    }

    [StaFact]
    public void Remove_RemovesRun()
    {
        // Arrange
        var textBlock = new TextBlock();
        var runs = CreateRuns("A", "B", "C");
        TextBlockHelper.SetInlines(textBlock, runs);

        // Act
        runs.RemoveAt(1);

        // Assert
        Assert.Equal(["A", "C"], GetTexts(textBlock), StringComparer.Ordinal);
    }

    [StaFact]
    public void Clear_EmptiesTextBlock()
    {
        // Arrange
        var textBlock = new TextBlock();
        var runs = CreateRuns("A", "B", "C");
        TextBlockHelper.SetInlines(textBlock, runs);

        // Act
        runs.Clear();

        // Assert
        Assert.Empty(textBlock.Inlines);
    }

    [StaFact]
    public void SetInlines_ReplacingCollection_DetachesPreviousCollection()
    {
        // Arrange
        var textBlock = new TextBlock();
        var oldRuns = CreateRuns("A", "B");
        var newRuns = CreateRuns("X");
        TextBlockHelper.SetInlines(textBlock, oldRuns);

        // Act
        TextBlockHelper.SetInlines(textBlock, newRuns);
        oldRuns.Add(new Run("C"));
        oldRuns.RemoveAt(0);

        // Assert
        Assert.Equal(["X"], GetTexts(textBlock), StringComparer.Ordinal);
    }

    [StaFact]
    public void SetInlines_Null_ClearsAndDetaches()
    {
        // Arrange
        var textBlock = new TextBlock();
        var runs = CreateRuns("A", "B");
        TextBlockHelper.SetInlines(textBlock, runs);

        // Act
        TextBlockHelper.SetInlines(textBlock, null!);
        runs.Add(new Run("C"));

        // Assert
        Assert.Empty(textBlock.Inlines);
    }

    private static ObservableCollection<Run> CreateRuns(params string[] texts)
        => new(texts.Select(text => new Run(text)));

    private static string[] GetTexts(TextBlock textBlock)
        => textBlock.Inlines.OfType<Run>().Select(run => run.Text).ToArray();
}