namespace Atc.Wpf.Controls.Tests.Selectors;

/// <summary>
/// <c>MaxSelectionCount</c> is <see langword="null"/> (no limit) or at least one across the multi-select family; a
/// lower value is rejected when it is set.
/// </summary>
[Collection(nameof(PackResourceTestGroup))]
public sealed class MultiSelectLimitValidationTests : IDisposable
{
    public static TheoryData<Type> ControlTypes
        => new(
            typeof(CheckListBox),
            typeof(MultiSelectComboBox),
            typeof(TokenBox),
            typeof(ChipGroup),
            typeof(CheckTreeView),
            typeof(TreeSelectComboBox),
            typeof(CascaderComboBox),
            typeof(MultiColumnComboBox),
            typeof(CardSelector));

    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaTheory]
    [MemberData(nameof(ControlTypes))]
    public void A_limit_below_one_is_rejected(Type controlType)
    {
        ArgumentNullException.ThrowIfNull(controlType);
        var sut = (DependencyObject)Activator.CreateInstance(controlType)!;
        var property = GetLimitProperty(controlType);

        Assert.Throws<ArgumentException>(() => sut.SetValue(property, -1));
        Assert.Throws<ArgumentException>(() => sut.SetValue(property, 0));
        Assert.Null(sut.GetValue(property));

        sut.SetValue(property, 1);
        Assert.Equal(1, sut.GetValue(property));

        sut.SetValue(property, null);
        Assert.Null(sut.GetValue(property));
    }

    private static DependencyProperty GetLimitProperty(Type controlType)
        => (DependencyProperty)controlType
            .GetField("MaxSelectionCountProperty", BindingFlags.Public | BindingFlags.Static)!
            .GetValue(null)!;
}