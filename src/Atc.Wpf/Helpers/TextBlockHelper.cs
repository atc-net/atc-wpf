// ReSharper disable IdentifierTypo
// ReSharper disable InvertIf
namespace Atc.Wpf.Helpers;

[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static class TextBlockHelper
{
    public static readonly DependencyProperty InlinesProperty = DependencyProperty.RegisterAttached(
        "Inlines",
        typeof(ObservableCollection<Run>),
        typeof(TextBlockHelper),
        new UIPropertyMetadata(
            defaultValue: null,
            OnInlinesChanged));

    public static ObservableCollection<Run> GetInlines(DependencyObject d)
        => (ObservableCollection<Run>)d.GetValue(InlinesProperty);

    public static void SetInlines(
        DependencyObject d,
        ObservableCollection<Run> value)
        => d.SetValue(InlinesProperty, value);

    private static void OnInlinesChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not TextBlock textBlock)
        {
            return;
        }

        textBlock.Inlines.Clear();

        if (e.NewValue is not ObservableCollection<Run> inlines)
        {
            return;
        }

        foreach (var inline in inlines)
        {
            textBlock.Inlines.Add(inline);
        }

        inlines.CollectionChanged += (_, args) =>
        {
            OnCollectionChangedHandler(
                textBlock,
                args);
        };
    }

    private static void OnCollectionChangedHandler(
        TextBlock textBlock,
        NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is null &&
            e.OldItems is null)
        {
            textBlock.Inlines.Clear();
            return;
        }

        if (e.NewItems is not null)
        {
            foreach (Run inline in e.NewItems)
            {
                textBlock.Inlines.Add(inline);
            }
        }

        if (e.OldItems is not null)
        {
            foreach (Run inline in e.OldItems)
            {
                textBlock.Inlines.Remove(inline);
            }
        }
    }
}