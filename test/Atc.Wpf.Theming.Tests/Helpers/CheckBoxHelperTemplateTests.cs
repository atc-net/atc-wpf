namespace Atc.Wpf.Theming.Tests.Helpers;

/// <summary>
/// The themed CheckBox template reads its look from the <see cref="CheckBoxHelper"/> attached properties:
/// one set per check state (unchecked, checked, indeterminate) and interaction (normal, mouse over,
/// pressed, disabled).
/// </summary>
public sealed class CheckBoxHelperTemplateTests
{
    private const string Dictionary = "Styles/BaseControls/CheckBox.xaml";
    private const string StyleKey = "AtcApps.Styles.CheckBox";

    private static readonly string[] CheckTokens = ["Unchecked", "Checked", "Indeterminate"];
    private static readonly string[] InteractionTokens = ["MouseOver", "Pressed", "Disabled"];

    [StaFact]
    public void EveryTrigger_ReadsTheHelperPropertiesOfItsOwnState()
    {
        var template = TemplateTriggerBindings.LoadTemplate(Dictionary, StyleKey);
        var triggers = TemplateTriggerBindings.Read(template, typeof(CheckBoxHelper));
        var mismatches = new List<string>();

        foreach (var trigger in triggers)
        {
            var checkToken = CheckToken(trigger.Conditions);
            var interaction = InteractionToken(trigger.Conditions);
            foreach (var setter in trigger.Setters)
            {
                var name = setter.HelperProperty;
                var isGlyph = setter.TargetProperty == ContentControl.ContentProperty ||
                              setter.TargetProperty == ContentControl.ContentTemplateProperty;
                var wrongCheck = checkToken is not null &&
                                 (!TemplateTriggerBindings.HasWord(name, checkToken) ||
                                  CheckTokens.Any(t => t != checkToken && TemplateTriggerBindings.HasWord(name, t)));
                var wrongInteraction = !isGlyph &&
                                       InteractionTokens.Any(t => TemplateTriggerBindings.HasWord(name, t) != (t == interaction));
                if (wrongCheck || wrongInteraction)
                {
                    mismatches.Add($"[{checkToken}/{interaction ?? "Normal"}] {setter.TargetName}.{setter.TargetProperty.Name} <- {name}");
                }
            }
        }

        Assert.True(triggers.Count >= 12, $"Expected the 12 state triggers, found {triggers.Count}.");
        Assert.Empty(mismatches);
    }

    [StaTheory]
    [InlineData(false, true, "Unchecked")]
    [InlineData(true, true, "Checked")]
    [InlineData(null, true, "Indeterminate")]
    [InlineData(false, false, "UncheckedDisabled")]
    [InlineData(true, false, "CheckedDisabled")]
    [InlineData(null, false, "IndeterminateDisabled")]
    public void RenderedCheckBox_UsesTheBrushesOfItsState(
        bool? isChecked,
        bool isEnabled,
        string state)
    {
        var checkBox = new CheckBox
        {
            IsThreeState = true,
            IsChecked = isChecked,
            IsEnabled = isEnabled,
            Content = "Option",
        };
        var brushes = new Dictionary<string, SolidColorBrush>(StringComparer.Ordinal);
        foreach (var part in new[] { "Foreground", "Background", "BorderBrush", "CheckBackgroundFill", "CheckBackgroundStroke", "CheckGlyphForeground" })
        {
            var brush = new SolidColorBrush(Color.FromRgb((byte)(brushes.Count + 1), 0x22, 0x33));
            brushes[part] = brush;
            checkBox.SetValue(HelperProperty(part + state), brush);
        }

        var window = Show(checkBox);
        try
        {
            Assert.Same(brushes["Foreground"], TextElement.GetForeground(Part<ContentPresenter>(checkBox, "ContentPresenter")));
            Assert.Same(brushes["Background"], Part<Border>(checkBox, "RootBorder").Background);
            Assert.Same(brushes["BorderBrush"], Part<Border>(checkBox, "RootBorder").BorderBrush);
            Assert.Same(brushes["CheckBackgroundFill"], Part<Rectangle>(checkBox, "NormalRectangle").Fill);
            Assert.Same(brushes["CheckBackgroundStroke"], Part<Rectangle>(checkBox, "NormalRectangle").Stroke);
            Assert.Same(brushes["CheckGlyphForeground"], Part<ContentControl>(checkBox, "CheckGlyph").Foreground);
        }
        finally
        {
            window.Close();
        }
    }

    [StaTheory]
    [InlineData(false, "CheckGlyphUnchecked")]
    [InlineData(true, "CheckGlyphChecked")]
    [InlineData(null, "CheckGlyphIndeterminate")]
    public void RenderedCheckBox_ShowsTheGlyphOfItsCheckState(
        bool? isChecked,
        string glyphProperty)
    {
        var checkBox = new CheckBox { IsThreeState = true, IsChecked = isChecked };
        var glyph = new object();
        checkBox.SetValue(HelperProperty(glyphProperty), glyph);

        var window = Show(checkBox);
        try
        {
            Assert.Same(glyph, Part<ContentControl>(checkBox, "CheckGlyph").Content);
        }
        finally
        {
            window.Close();
        }
    }

    [StaFact]
    public void RenderedCheckBox_SizesAndRoundsTheCheckFromTheHelper()
    {
        var checkBox = new CheckBox { IsChecked = false };
        CheckBoxHelper.SetCheckSize(checkBox, 26);
        CheckBoxHelper.SetCheckStrokeThickness(checkBox, 3);
        CheckBoxHelper.SetCheckCornerRadius(checkBox, new CornerRadius(4, 0, 7, 0));

        var window = Show(checkBox);
        try
        {
            var check = Part<FrameworkElement>(checkBox, "Check");
            var rectangle = Part<Rectangle>(checkBox, "NormalRectangle");
            Assert.Equal(26, check.Width);
            Assert.Equal(26, check.Height);
            Assert.Equal(3, rectangle.StrokeThickness);
            Assert.Equal(4, rectangle.RadiusX);
            Assert.Equal(7, rectangle.RadiusY);
        }
        finally
        {
            window.Close();
        }
    }

    private static string? CheckToken(
        IReadOnlyList<TemplateTriggerBindings.TriggerCondition> conditions)
    {
        var condition = conditions.FirstOrDefault(c => c.Property == ToggleButton.IsCheckedProperty);
        return condition is null
            ? null
            : condition.Value switch
            {
                false => "Unchecked",
                true => "Checked",
                _ => "Indeterminate",
            };
    }

    private static string? InteractionToken(
        IReadOnlyList<TemplateTriggerBindings.TriggerCondition> conditions)
    {
        if (conditions.Any(c => c.Property == UIElement.IsEnabledProperty && Equals(c.Value, false)))
        {
            return "Disabled";
        }

        if (conditions.Any(c => c.Property == ButtonBase.IsPressedProperty && Equals(c.Value, true)))
        {
            return "Pressed";
        }

        return conditions.Any(c => c.Property == UIElement.IsMouseOverProperty && Equals(c.Value, true))
            ? "MouseOver"
            : null;
    }

    private static DependencyProperty HelperProperty(string name)
        => (DependencyProperty)typeof(CheckBoxHelper)
            .GetField(name + "Property", BindingFlags.Public | BindingFlags.Static)!
            .GetValue(null)!;

    private static Window Show(CheckBox checkBox)
    {
        var resources = ThemedWindow.CreateResources();
        resources.MergedDictionaries.Add(new ResourceDictionary
        {
            Source = new Uri($"pack://application:,,,/Atc.Wpf.Theming;component/{Dictionary}", UriKind.Absolute),
        });
        checkBox.Style = (Style)resources[StyleKey];
        return ThemedWindow.Show(resources, checkBox);
    }

    private static T Part<T>(
        Control control,
        string name)
        where T : class
        => Assert.IsType<T>(control.Template.FindName(name, control), exactMatch: false);
}