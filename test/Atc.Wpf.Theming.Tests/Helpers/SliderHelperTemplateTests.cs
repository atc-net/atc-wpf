namespace Atc.Wpf.Theming.Tests.Helpers;

/// <summary>
/// The themed Slider templates (horizontal and vertical) read the thumb and track brushes from the
/// <see cref="SliderHelper"/> attached properties: a plain, <c>Hover</c>, <c>Pressed</c> and <c>Disabled</c>
/// variant of each.
/// </summary>
public sealed class SliderHelperTemplateTests
{
    private const string Dictionary = "Styles/BaseControls/Slider.xaml";

    private static readonly string[] InteractionTokens = ["Hover", "Pressed", "Disabled"];

    [StaTheory]
    [InlineData("AtcApps.Templates.Slider.Horizontal")]
    [InlineData("AtcApps.Templates.Slider.Vertical")]
    public void EveryTrigger_ReadsTheHelperPropertiesOfItsOwnState(
        string templateKey)
    {
        var template = TemplateTriggerBindings.LoadTemplate(Dictionary, templateKey);
        var triggers = TemplateTriggerBindings.Read(template, typeof(SliderHelper));
        var mismatches = new List<string>();

        foreach (var trigger in triggers)
        {
            var condition = Assert.Single(trigger.Conditions);
            var interaction = condition switch
            {
                { Property.Name: "IsMouseOver", Value: true } => "Hover",
                { Property.Name: "IsPressed" or "IsDragging", Value: true } => "Pressed",
                { Property.Name: "IsEnabled", Value: false } => "Disabled",
                _ => null,
            };

            mismatches.AddRange(trigger.Setters
                .Where(s => InteractionTokens.Any(t => TemplateTriggerBindings.HasWord(s.HelperProperty, t) != (t == interaction)))
                .Select(s => $"[{interaction ?? condition.Property.Name}] {s.TargetName}.{s.TargetProperty.Name} <- {s.HelperProperty}"));
        }

        Assert.Equal(5, triggers.Count);
        Assert.Empty(mismatches);
    }

    [StaTheory]
    [InlineData(Orientation.Horizontal, true, "")]
    [InlineData(Orientation.Horizontal, false, "Disabled")]
    [InlineData(Orientation.Vertical, true, "")]
    [InlineData(Orientation.Vertical, false, "Disabled")]
    public void RenderedSlider_UsesTheThumbAndTrackBrushesOfItsState(
        Orientation orientation,
        bool isEnabled,
        string state)
    {
        var slider = new Slider { Orientation = orientation, IsEnabled = isEnabled, Value = 40 };
        var thumb = new SolidColorBrush(Colors.Olive);
        var value = new SolidColorBrush(Colors.Teal);
        var track = new SolidColorBrush(Colors.Maroon);
        slider.SetValue(HelperProperty($"ThumbFill{state}Brush"), thumb);
        slider.SetValue(HelperProperty($"TrackValueFill{state}Brush"), value);
        slider.SetValue(HelperProperty($"TrackFill{state}Brush"), track);
        var prefix = orientation.ToString();

        var window = Show(slider);
        try
        {
            Assert.Same(thumb, Part<Thumb>(slider, prefix + "Thumb").Background);
            Assert.Same(value, Part<RepeatButton>(slider, prefix + "TrackDecrease").Foreground);
            Assert.Same(track, Part<RepeatButton>(slider, prefix + "TrackIncrease").Foreground);
        }
        finally
        {
            window.Close();
        }
    }

    private static DependencyProperty HelperProperty(string name)
        => (DependencyProperty)typeof(SliderHelper)
            .GetField(name + "Property", BindingFlags.Public | BindingFlags.Static)!
            .GetValue(null)!;

    private static Window Show(Slider slider)
    {
        var resources = ThemedWindow.CreateResources();
        resources.MergedDictionaries.Add(new ResourceDictionary
        {
            Source = new Uri($"pack://application:,,,/Atc.Wpf.Theming;component/{Dictionary}", UriKind.Absolute),
        });
        slider.Style = (Style)resources["AtcApps.Styles.Slider"];
        return ThemedWindow.Show(resources, slider);
    }

    private static T Part<T>(
        Control control,
        string name)
        where T : class
        => Assert.IsType<T>(control.Template.FindName(name, control), exactMatch: false);
}