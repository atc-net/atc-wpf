namespace Atc.Wpf.Theming.Tests.Styles;

/// <summary>
/// Runs a validation error through the themed error template (AtcApps.Templates.ValidationError) in a shown
/// window: the message must show while the error is present, and clearing the error must not make the
/// template's binding read past the end of the emptied Validation.Errors collection (Data Error 17).
/// </summary>
public sealed class ValidationErrorTemplateTests
{
    private const int CannotGetValueErrorId = 17;
    private const int PathErrorId = 40;

    [StaFact]
    public void TextBox_ConversionErrorThenValidValue_ShowsTheMessageAndClearsWithoutBindingErrors()
    {
        var resources = ThemedWindow.CreateResources();
        var model = new NumberModel();
        var textBox = new TextBox { Style = (Style)resources["AtcApps.Styles.TextBox"] };
        textBox.SetBinding(
            TextBox.TextProperty,
            new Binding(nameof(NumberModel.Value))
            {
                Source = model,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            });

        using var trace = BindingTraceCapture.Start();
        var window = ThemedWindow.Show(resources, WithCanary(textBox, model));
        try
        {
            textBox.Text = "abc";
            ThemedWindow.Pump();

            Assert.True(Validation.GetHasError(textBox));
            var message = ErrorTemplateText(textBox);
            Assert.False(string.IsNullOrEmpty(message));
            Assert.Equal(Validation.GetErrors(textBox)[0].ErrorContent?.ToString(), message);

            textBox.Text = "5";
            ThemedWindow.Pump();

            Assert.False(Validation.GetHasError(textBox));
            Assert.Equal(5, model.Value);
            AssertListenerHeardTheCanary(trace);
            AssertNoCannotGetItemErrors(trace);
        }
        finally
        {
            window.Close();
        }
    }

    [StaFact]
    public void ComboBox_ErrorThenCleared_ShowsTheMessageAndClearsWithoutBindingErrors()
    {
        var resources = ThemedWindow.CreateResources();
        var model = new NumberModel();
        var comboBox = new ComboBox
        {
            Style = (Style)resources["AtcApps.Styles.ComboBox"],
            ItemsSource = new[] { 1, 2, 3 },
        };
        comboBox.SetBinding(
            ComboBox.SelectedItemProperty,
            new Binding(nameof(NumberModel.Value)) { Source = model, Mode = BindingMode.TwoWay });

        using var trace = BindingTraceCapture.Start();
        var window = ThemedWindow.Show(resources, WithCanary(comboBox, model));
        try
        {
            var expression = comboBox.GetBindingExpression(ComboBox.SelectedItemProperty)!;
            Validation.MarkInvalid(expression, new ValidationError(new ExceptionValidationRule(), expression, "Pick a number", exception: null));
            ThemedWindow.Pump();

            Assert.True(Validation.GetHasError(comboBox));
            Assert.Equal("Pick a number", ErrorTemplateText(comboBox));

            Validation.ClearInvalid(expression);
            ThemedWindow.Pump();

            Assert.False(Validation.GetHasError(comboBox));
            AssertListenerHeardTheCanary(trace);
            AssertNoCannotGetItemErrors(trace);
        }
        finally
        {
            window.Close();
        }
    }

    // A binding to a missing property writes Data Error 40; seeing it proves the capture is wired up,
    // so an empty Error 17 list means the template is quiet rather than the listener being deaf.
    private static StackPanel WithCanary(
        UIElement control,
        NumberModel model)
    {
        var canary = new TextBlock();
        canary.SetBinding(TextBlock.TextProperty, new Binding("DoesNotExist") { Source = model });

        var panel = new StackPanel();
        panel.Children.Add(control);
        panel.Children.Add(canary);
        return panel;
    }

    private static void AssertListenerHeardTheCanary(BindingTraceCapture trace)
        => Assert.Contains(trace.Events, e => e.Id == PathErrorId && e.Message.Contains("DoesNotExist", StringComparison.Ordinal));

    private static void AssertNoCannotGetItemErrors(BindingTraceCapture trace)
    {
        var errors = trace.Events
            .Where(e => e.Id == CannotGetValueErrorId || e.Message.Contains("Item[]", StringComparison.Ordinal))
            .Select(e => $"{e.Id}: {e.Message}")
            .ToList();

        Assert.True(errors.Count == 0, "The validation error template wrote binding errors:" + Environment.NewLine + string.Join(Environment.NewLine, errors));
    }

    private static string? ErrorTemplateText(FrameworkElement element)
    {
        var layer = AdornerLayer.GetAdornerLayer(element);
        Assert.NotNull(layer);
        var adorners = layer.GetAdorners(element);
        Assert.NotNull(adorners);

        var textBlock = adorners.SelectMany(Descendants).OfType<TextBlock>().FirstOrDefault();
        Assert.NotNull(textBlock);
        return textBlock.Text;
    }

    private static IEnumerable<DependencyObject> Descendants(
        DependencyObject parent)
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            yield return child;
            foreach (var descendant in Descendants(child))
            {
                yield return descendant;
            }
        }
    }

    private sealed class NumberModel
    {
        public int Value { get; set; }
    }
}