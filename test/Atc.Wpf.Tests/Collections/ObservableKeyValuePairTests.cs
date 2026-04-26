namespace Atc.Wpf.Tests.Collections;

public sealed class ObservableKeyValuePairTests
{
    [Fact]
    public void Defaults_to_default_TKey_and_TValue()
    {
        var sut = new ObservableKeyValuePair<int, string>();

        Assert.Equal(0, sut.Key);
        Assert.Null(sut.Value);
    }

    [Fact]
    public void Setting_Key_raises_PropertyChanged_for_Key()
    {
        var sut = new ObservableKeyValuePair<int, string>();
        var captured = new List<string?>();
        sut.PropertyChanged += (_, e) => captured.Add(e.PropertyName);

        sut.Key = 42;

        Assert.Equal(42, sut.Key);
        Assert.Single(captured);
        Assert.Equal(nameof(ObservableKeyValuePair<int, string>.Key), captured[0]);
    }

    [Fact]
    public void Setting_Value_raises_PropertyChanged_for_Value()
    {
        var sut = new ObservableKeyValuePair<int, string>();
        var captured = new List<string?>();
        sut.PropertyChanged += (_, e) => captured.Add(e.PropertyName);

        sut.Value = "hello";

        Assert.Equal("hello", sut.Value);
        Assert.Single(captured);
        Assert.Equal(nameof(ObservableKeyValuePair<int, string>.Value), captured[0]);
    }

    [Fact]
    public void Setting_each_property_raises_one_event_per_assignment()
    {
        var sut = new ObservableKeyValuePair<string, int>();
        var captured = new List<string?>();
        sut.PropertyChanged += (_, e) => captured.Add(e.PropertyName);

        sut.Key = "a";
        sut.Value = 1;
        sut.Key = "b";

        Assert.Equal(
            new[] { "Key", "Value", "Key" },
            captured);
    }

    [Fact]
    public void OnPropertyChanged_can_be_invoked_directly_with_a_custom_name()
    {
        var sut = new ObservableKeyValuePair<int, string>();
        string? captured = null;
        sut.PropertyChanged += (_, e) => captured = e.PropertyName;

        sut.OnPropertyChanged("Custom");

        Assert.Equal("Custom", captured);
    }
}