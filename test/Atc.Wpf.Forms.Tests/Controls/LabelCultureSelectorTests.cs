namespace Atc.Wpf.Forms.Tests.Controls;

public sealed class LabelCultureSelectorTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void LabelCountrySelector_UpdateUiCultureOnChangeEvent_IsOffByDefaultLikeCountrySelector()
    {
        var sut = new LabelCountrySelector();

        Assert.False(sut.UpdateUiCultureOnChangeEvent);
        Assert.Equal(new CountrySelector().UpdateUiCultureOnChangeEvent, sut.UpdateUiCultureOnChangeEvent);
    }

    [StaFact]
    public void LabelLanguageSelector_UpdateUiCultureOnChangeEvent_IsOnByDefaultLikeLanguageSelector()
    {
        var sut = new LabelLanguageSelector();

        Assert.True(sut.UpdateUiCultureOnChangeEvent);
        Assert.Equal(new LanguageSelector().UpdateUiCultureOnChangeEvent, sut.UpdateUiCultureOnChangeEvent);
    }

    [StaFact]
    public void LabelCountrySelector_UpdateUiCultureOnChangeEvent_IsBoundToTheInnerSelector()
    {
        var sut = new LabelCountrySelector();

        var binding = BindingOperations.GetBinding(
            FindLogicalChild<CountrySelector>(sut),
            CountrySelector.UpdateUiCultureOnChangeEventProperty);

        Assert.NotNull(binding);
        Assert.Equal(nameof(LabelCountrySelector.UpdateUiCultureOnChangeEvent), binding.Path.Path);
        Assert.Equal(typeof(LabelCountrySelector), binding.RelativeSource.AncestorType);
    }

    [StaFact]
    public void LabelLanguageSelector_UpdateUiCultureOnChangeEvent_IsBoundToTheInnerSelector()
    {
        var sut = new LabelLanguageSelector();

        var binding = BindingOperations.GetBinding(
            FindLogicalChild<LanguageSelector>(sut),
            LanguageSelector.UpdateUiCultureOnChangeEventProperty);

        Assert.NotNull(binding);
        Assert.Equal(nameof(LabelLanguageSelector.UpdateUiCultureOnChangeEvent), binding.Path.Path);
        Assert.Equal(typeof(LabelLanguageSelector), binding.RelativeSource.AncestorType);
    }

    private static T FindLogicalChild<T>(DependencyObject parent)
        where T : DependencyObject
        => TryFindLogicalChild<T>(parent)
           ?? throw new InvalidOperationException($"No {typeof(T).Name} found.");

    private static T? TryFindLogicalChild<T>(DependencyObject parent)
        where T : DependencyObject
    {
        foreach (var child in LogicalTreeHelper.GetChildren(parent).OfType<DependencyObject>())
        {
            if (child is T match)
            {
                return match;
            }

            var found = TryFindLogicalChild<T>(child);
            if (found is not null)
            {
                return found;
            }
        }

        return null;
    }
}