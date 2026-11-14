namespace Atc.Wpf.Theming.Tests.Helpers;

/// <summary>
/// The themed RadioButton template reads its look from the <see cref="RadioButtonHelper"/> attached
/// properties: the plain properties for the normal state and a <c>PointerOver</c>, <c>Pressed</c> and
/// <c>Disabled</c> variant for each.
/// </summary>
public sealed class RadioButtonHelperTemplateTests
{
    private const string Dictionary = "Styles/BaseControls/RadioButton.xaml";
    private const string StyleKey = "AtcApps.Styles.RadioButton";

    private static readonly string[] InteractionTokens = ["PointerOver", "Pressed", "Disabled"];

    [StaFact]
    public void EveryTrigger_ReadsTheHelperPropertiesOfItsOwnState()
    {
        var template = TemplateTriggerBindings.LoadTemplate(Dictionary, StyleKey);
        var triggers = TemplateTriggerBindings.Read(template, typeof(RadioButtonHelper));
        var mismatches = new List<string>();

        foreach (var trigger in triggers)
        {
            var condition = Assert.Single(trigger.Conditions);
            var interaction = condition switch
            {
                { Property.Name: "IsMouseOver", Value: true } => "PointerOver",
                { Property.Name: "IsPressed", Value: true } => "Pressed",
                { Property.Name: "IsEnabled", Value: false } => "Disabled",
                _ => null,
            };

            mismatches.AddRange(trigger.Setters
                .Where(s => InteractionTokens.Any(t => TemplateTriggerBindings.HasWord(s.HelperProperty, t) != (t == interaction)))
                .Select(s => $"[{interaction ?? condition.Property.Name}] {s.TargetName}.{s.TargetProperty.Name} <- {s.HelperProperty}"));
        }

        Assert.Equal(3, triggers.Count);
        Assert.Empty(mismatches);
    }

    [StaTheory]
    [InlineData(true, "")]
    [InlineData(false, "Disabled")]
    public void RenderedRadioButton_UsesTheBrushesOfItsState(
        bool isEnabled,
        string suffix)
    {
        var radioButton = new RadioButton { IsEnabled = isEnabled, Content = "Option" };
        var parts = new (string Property, string Part, DependencyProperty Target)[]
        {
            ("OuterEllipseFill", "OuterEllipse", Shape.FillProperty),
            ("OuterEllipseStroke", "OuterEllipse", Shape.StrokeProperty),
            ("OuterEllipseCheckedFill", "CheckOuterEllipse", Shape.FillProperty),
            ("OuterEllipseCheckedStroke", "CheckOuterEllipse", Shape.StrokeProperty),
            ("CheckGlyphFill", "CheckGlyph", Shape.FillProperty),
            ("CheckGlyphStroke", "CheckGlyph", Shape.StrokeProperty),
        };
        var brushes = parts
            .Select((p, i) => (p.Part, p.Target, Brush: new SolidColorBrush(Color.FromRgb((byte)(i + 1), 0x44, 0x55))))
            .ToList();
        for (var i = 0; i < parts.Length; i++)
        {
            radioButton.SetValue(HelperProperty(parts[i].Property + suffix), brushes[i].Brush);
        }

        var window = Show(radioButton);
        try
        {
            foreach (var (part, target, brush) in brushes)
            {
                Assert.Same(brush, Part<Ellipse>(radioButton, part).GetValue(target));
            }
        }
        finally
        {
            window.Close();
        }
    }

    [StaFact]
    public void RenderedRadioButton_Disabled_UsesTheDisabledTextAndBorderBrushes()
    {
        var radioButton = new RadioButton { IsEnabled = false, Content = "Option" };
        var foreground = new SolidColorBrush(Colors.Olive);
        var background = new SolidColorBrush(Colors.Teal);
        var border = new SolidColorBrush(Colors.Maroon);
        RadioButtonHelper.SetForegroundDisabled(radioButton, foreground);
        RadioButtonHelper.SetBackgroundDisabled(radioButton, background);
        RadioButtonHelper.SetBorderBrushDisabled(radioButton, border);

        var window = Show(radioButton);
        try
        {
            Assert.Same(foreground, TextElement.GetForeground(Part<ContentPresenter>(radioButton, "ContentPresenter")));
            Assert.Same(background, Part<Border>(radioButton, "RootBorder").Background);
            Assert.Same(border, Part<Border>(radioButton, "RootBorder").BorderBrush);
        }
        finally
        {
            window.Close();
        }
    }

    [StaTheory]
    [InlineData(false, 0.0, 1.0)]
    [InlineData(true, 1.0, 0.0)]
    public void RenderedRadioButton_ShowsTheCheckGlyphOnlyWhenChecked(
        bool isChecked,
        double glyphOpacity,
        double outerOpacity)
    {
        var radioButton = new RadioButton { IsChecked = isChecked };

        var window = Show(radioButton);
        try
        {
            Assert.Equal(glyphOpacity, Part<Ellipse>(radioButton, "CheckGlyph").Opacity);
            Assert.Equal(outerOpacity, Part<Ellipse>(radioButton, "OuterEllipse").Opacity);
        }
        finally
        {
            window.Close();
        }
    }

    [StaFact]
    public void RenderedRadioButton_SizesTheRadioFromTheHelper()
    {
        var radioButton = new RadioButton();
        RadioButtonHelper.SetRadioSize(radioButton, 30);
        RadioButtonHelper.SetRadioCheckSize(radioButton, 12);
        RadioButtonHelper.SetRadioStrokeThickness(radioButton, 3);

        var window = Show(radioButton);
        try
        {
            var outer = Part<Ellipse>(radioButton, "OuterEllipse");
            Assert.Equal(30, outer.Width);
            Assert.Equal(30, Part<Ellipse>(radioButton, "CheckOuterEllipse").Height);
            Assert.Equal(3, outer.StrokeThickness);
            Assert.Equal(12, Part<Ellipse>(radioButton, "CheckGlyph").Width);
        }
        finally
        {
            window.Close();
        }
    }

    private static DependencyProperty HelperProperty(string name)
        => (DependencyProperty)typeof(RadioButtonHelper)
            .GetField(name + "Property", BindingFlags.Public | BindingFlags.Static)!
            .GetValue(null)!;

    private static Window Show(RadioButton radioButton)
    {
        var resources = ThemedWindow.CreateResources();
        resources.MergedDictionaries.Add(new ResourceDictionary
        {
            Source = new Uri($"pack://application:,,,/Atc.Wpf.Theming;component/{Dictionary}", UriKind.Absolute),
        });
        radioButton.Style = (Style)resources[StyleKey];
        return ThemedWindow.Show(resources, radioButton);
    }

    private static T Part<T>(
        Control control,
        string name)
        where T : class
        => Assert.IsType<T>(control.Template.FindName(name, control), exactMatch: false);
}