namespace Atc.Wpf.Theming.Tests.TestSupport;

/// <summary>
/// Reads which helper attached properties the setters of a control template's triggers bind to, so a test
/// can check that each trigger uses the properties named for its state (e.g. a pressed trigger reads the
/// <c>…Pressed</c> brushes, not the <c>…MouseOver</c> ones).
/// </summary>
internal static class TemplateTriggerBindings
{
    public static ControlTemplate LoadTemplate(
        string dictionaryPath,
        string key)
    {
        var resources = ThemedWindow.CreateResources();
        var dictionary = new ResourceDictionary
        {
            Source = new Uri($"pack://application:,,,/Atc.Wpf.Theming;component/{dictionaryPath}", UriKind.Absolute),
        };
        resources.MergedDictionaries.Add(dictionary);

        return resources[key] switch
        {
            ControlTemplate template => template,
            Style style => style.Setters
                .OfType<Setter>()
                .Where(x => x.Property == Control.TemplateProperty)
                .Select(x => x.Value)
                .OfType<ControlTemplate>()
                .Single(),
            var other => throw new InvalidOperationException($"{key} is a {other?.GetType().Name ?? "missing resource"}."),
        };
    }

    // For each trigger of the template: its conditions and the setters that bind to a property of helperType.
    public static IReadOnlyList<TriggerBindings> Read(
        ControlTemplate template,
        Type helperType)
    {
        var result = new List<TriggerBindings>();
        foreach (var trigger in template.Triggers)
        {
            var (conditions, setters) = trigger switch
            {
                Trigger t => ([new TriggerCondition(t.SourceName, t.Property, t.Value)], t.Setters),
                MultiTrigger m => (m.Conditions.Select(c => new TriggerCondition(c.SourceName, c.Property, c.Value)).ToList(), m.Setters),
                _ => (new List<TriggerCondition>(), new SetterBaseCollection()),
            };

            var bindings = setters
                .OfType<Setter>()
                .Where(s => s.Value is Binding)
                .Select(s => (Setter: s, Helper: FindHelperProperty((Binding)s.Value, helperType)))
                .Where(x => x.Helper is not null)
                .Select(x => new SetterBinding(x.Setter.TargetName, x.Setter.Property, x.Helper!))
                .ToList();

            if (bindings.Count > 0)
            {
                result.Add(new TriggerBindings(conditions, bindings));
            }
        }

        return result;
    }

    // True when propertyName contains word as whole Pascal-case words
    // ("Checked" matches "CheckGlyphChecked" and "CheckedMouseOver" but not "Unchecked").
    public static bool HasWord(
        string propertyName,
        string word)
        => Regex.IsMatch(propertyName, $"{word}(?![a-z])", RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1));

    private static string? FindHelperProperty(
        Binding binding,
        Type helperType)
        => binding.Path?.PathParameters.OfType<DependencyProperty>().FirstOrDefault(p => p.OwnerType == helperType)?.Name;

    internal sealed record TriggerCondition(
        string? SourceName,
        DependencyProperty Property,
        object? Value);

    internal sealed record SetterBinding(
        string? TargetName,
        DependencyProperty TargetProperty,
        string HelperProperty);

    internal sealed record TriggerBindings(
        IReadOnlyList<TriggerCondition> Conditions,
        IReadOnlyList<SetterBinding> Setters);
}