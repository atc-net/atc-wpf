// ReSharper disable IdentifierTypo
// ReSharper disable InvertIf
namespace Atc.Wpf.Helpers;

/// <summary>Provides attached properties for a <see cref="TextBlock"/>, such as a bindable collection of inline runs.</summary>
[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static class TextBlockHelper
{
    /// <summary>Identifies the Inlines attached property.</summary>
    public static readonly DependencyProperty InlinesProperty = DependencyProperty.RegisterAttached(
        "Inlines",
        typeof(ObservableCollection<Run>),
        typeof(TextBlockHelper),
        new UIPropertyMetadata(
            defaultValue: null,
            OnInlinesChanged));

    /// <summary>
    /// Holds the per-TextBlock CollectionChanged handler so it can be removed from the
    /// previous collection when <see cref="InlinesProperty"/> changes.
    /// </summary>
    private static readonly DependencyProperty InlinesCollectionChangedHandlerProperty = DependencyProperty.RegisterAttached(
        "InlinesCollectionChangedHandler",
        typeof(NotifyCollectionChangedEventHandler),
        typeof(TextBlockHelper),
        new PropertyMetadata(defaultValue: null));

    /// <summary>Gets the value of the Inlines attached property.</summary>
    public static ObservableCollection<Run> GetInlines(DependencyObject d)
        => (ObservableCollection<Run>)d.GetValue(InlinesProperty);

    /// <summary>Sets the value of the Inlines attached property.</summary>
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

        if (e.OldValue is ObservableCollection<Run> oldInlines &&
            textBlock.GetValue(InlinesCollectionChangedHandlerProperty) is NotifyCollectionChangedEventHandler oldHandler)
        {
            oldInlines.CollectionChanged -= oldHandler;
        }

        textBlock.ClearValue(InlinesCollectionChangedHandlerProperty);
        textBlock.Inlines.Clear();

        if (e.NewValue is not ObservableCollection<Run> inlines)
        {
            return;
        }

        textBlock.Inlines.AddRange(inlines);

        NotifyCollectionChangedEventHandler handler = (_, args) => OnCollectionChangedHandler(textBlock, inlines, args);
        textBlock.SetValue(InlinesCollectionChangedHandlerProperty, handler);
        inlines.CollectionChanged += handler;
    }

    private static void OnCollectionChangedHandler(
        TextBlock textBlock,
        ObservableCollection<Run> inlines,
        NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                InsertRuns(textBlock, e.NewItems, e.NewStartingIndex);
                break;
            case NotifyCollectionChangedAction.Remove:
                RemoveRuns(textBlock, e.OldItems);
                break;
            case NotifyCollectionChangedAction.Replace:
            case NotifyCollectionChangedAction.Move:
                RemoveRuns(textBlock, e.OldItems);
                InsertRuns(textBlock, e.NewItems, e.NewStartingIndex);
                break;
            case NotifyCollectionChangedAction.Reset:
                textBlock.Inlines.Clear();
                textBlock.Inlines.AddRange(inlines);
                break;
            default:
                throw new SwitchCaseDefaultException(e.Action);
        }
    }

    private static void InsertRuns(
        TextBlock textBlock,
        IList? runs,
        int index)
    {
        if (runs is null)
        {
            return;
        }

        var inlineCollection = textBlock.Inlines;
        var anchor = index >= 0 && index < inlineCollection.Count
            ? inlineCollection.ElementAt(index)
            : null;

        foreach (var item in runs)
        {
            if (item is not Run run)
            {
                continue;
            }

            if (anchor is null)
            {
                inlineCollection.Add(run);
            }
            else
            {
                inlineCollection.InsertBefore(anchor, run);
            }
        }
    }

    private static void RemoveRuns(
        TextBlock textBlock,
        IList? runs)
    {
        if (runs is null)
        {
            return;
        }

        foreach (var item in runs)
        {
            if (item is Run run)
            {
                textBlock.Inlines.Remove(run);
            }
        }
    }
}