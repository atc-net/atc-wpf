namespace Atc.Wpf.Tests.Collections;

public sealed class LruCacheTests
{
    private const int Capacity = 256;

    [Fact]
    public void Set_BeyondCapacity_EvictsOnlyTheLeastRecentlyUsedEntry()
    {
        // Arrange
        var sut = CreateFullCache();

        // Act
        sut.Set(Capacity, "value-" + Capacity);

        // Assert
        Assert.Equal(Capacity, sut.Count);
        Assert.False(sut.ContainsKey(0));
        for (var i = 1; i <= Capacity; i++)
        {
            Assert.True(sut.ContainsKey(i), $"Entry {i} was evicted.");
        }
    }

    [Fact]
    public void TryGet_MarksTheEntryAsRecentlyUsed_SoItSurvivesTheNextEviction()
    {
        // Arrange
        var sut = CreateFullCache();
        Assert.True(sut.TryGet(0, out var value));
        Assert.Equal("value-0", value);

        // Act
        sut.Set(Capacity, "value-" + Capacity);

        // Assert
        Assert.True(sut.ContainsKey(0));
        Assert.False(sut.ContainsKey(1));
        Assert.Equal(Capacity, sut.Count);
    }

    [Fact]
    public void Set_ExistingKey_ReplacesTheValueAndMarksItRecentlyUsed_WithoutEvicting()
    {
        // Arrange
        var sut = CreateFullCache();

        // Act
        sut.Set(0, "replaced");
        sut.Set(Capacity, "value-" + Capacity);

        // Assert
        Assert.True(sut.TryGet(0, out var value));
        Assert.Equal("replaced", value);
        Assert.False(sut.ContainsKey(1));
        Assert.Equal(Capacity, sut.Count);
    }

    [Fact]
    public void TryGet_MissingKey_ReturnsFalse()
    {
        // Arrange
        var sut = new LruCache<int, string>(Capacity);

        // Act
        var found = sut.TryGet(42, out var value);

        // Assert
        Assert.False(found);
        Assert.Null(value);
    }

    [Fact]
    public void Clear_RemovesEveryEntry()
    {
        // Arrange
        var sut = CreateFullCache();

        // Act
        sut.Clear();

        // Assert
        Assert.Equal(0, sut.Count);
        Assert.False(sut.ContainsKey(0));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_CapacityBelowOne_Throws(int capacity)
        => Assert.Throws<ArgumentOutOfRangeException>(() => new LruCache<int, string>(capacity));

    [Fact]
    public async Task ConcurrentReadsAndWrites_NeverExceedTheCapacity()
    {
        // Arrange
        var sut = new LruCache<int, string>(64);

        // Act
        var workers = Enumerable.Range(0, 8)
            .Select(worker => Task.Run(
                () =>
                {
                    for (var i = 0; i < 5_000; i++)
                    {
                        var key = (worker * 31) + (i % 200);
                        if (!sut.TryGet(key, out _))
                        {
                            sut.Set(key, "value");
                        }
                    }
                },
                TestContext.Current.CancellationToken));
        await Task.WhenAll(workers);

        // Assert
        Assert.Equal(64, sut.Count);
    }

    private static LruCache<int, string> CreateFullCache()
    {
        var cache = new LruCache<int, string>(Capacity);
        for (var i = 0; i < Capacity; i++)
        {
            cache.Set(i, "value-" + i);
        }

        return cache;
    }
}