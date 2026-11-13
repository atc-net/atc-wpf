namespace Atc.Wpf.Controls.Tests.Internal;

// Forgetting to merge Controls.xaml leaves the controls without a template; each such control type is reported once.
public sealed class StyleCheckTests
{
    private const string ControlsDictionary = "pack://application:,,,/Atc.Wpf.Controls;component/Styles/Controls.xaml";

    // Protected on FrameworkElement; read by reflection to find every control with its own default style.
    private static readonly DependencyProperty DefaultStyleKey = (DependencyProperty)typeof(FrameworkElement)
        .GetField("DefaultStyleKeyProperty", BindingFlags.Static | BindingFlags.NonPublic)!
        .GetValue(obj: null)!;

    [StaFact]
    public void A_control_without_its_style_is_reported_with_the_dictionary_to_merge()
    {
        var message = StyleCheck.FindMissingStyle(new NumericBox());

        Assert.NotNull(message);
        Assert.Contains(nameof(NumericBox), message, StringComparison.Ordinal);
        Assert.Contains(ControlsDictionary, message, StringComparison.Ordinal);
    }

    [StaFact]
    public void A_control_with_a_template_is_not_reported()
    {
        var control = new NumericBox { Template = new ControlTemplate(typeof(NumericBox)) };

        Assert.Null(StyleCheck.FindMissingStyle(control));
    }

    [Fact]
    public void Every_control_with_its_own_default_style_is_checked()
    {
        var controlTypes = typeof(NumericBox).Assembly
            .GetTypes()
            .Where(t => typeof(Control).IsAssignableFrom(t) && !t.IsAbstract && !t.ContainsGenericParameters)
            .Where(t =>
            {
                RuntimeHelpers.RunClassConstructor(t.TypeHandle);
                return t.Equals(DefaultStyleKey.GetMetadata(t).DefaultValue as Type);
            })
            .ToList();

        Assert.NotEmpty(controlTypes);
        Assert.All(controlTypes, t => Assert.True(StyleCheck.IsRegistered(t), t.Name));
    }
}