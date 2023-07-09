namespace Atc.Wpf.Extensions;

public static class ObservableCollectionRunExtensions
{
    public static void HighlightText(
        this ObservableCollection<Run> collection,
        string? text,
        string? highlightText,
        Brush highlightColor,
        bool useBoldOnHighlightText = false)
    {
        var fontWeight = FontWeights.Normal;
        if (useBoldOnHighlightText)
        {
            fontWeight = FontWeights.Bold;
        }

        collection.HighlightText(
            text,
            highlightText,
            highlightColor,
            fontWeight);
    }

    public static void HighlightText(
        this ObservableCollection<Run> collection,
        string? text,
        string? highlightText,
        Brush highlightColor,
        FontWeight highlightFontWeight)
    {
        ArgumentNullException.ThrowIfNull(collection);

        collection.Clear();

        if (string.IsNullOrEmpty(highlightText) ||
            string.IsNullOrEmpty(text))
        {
            collection.Add(new Run(text));
        }
        else
        {
            var index = 0;
            while (index < text.Length)
            {
                var foundIndex = text.IndexOf(highlightText, index, StringComparison.OrdinalIgnoreCase);
                if (foundIndex >= 0)
                {
                    if (foundIndex > index)
                    {
                        collection.Add(
                            new Run(text.Substring(index, foundIndex - index)));
                    }

                    collection.Add(
                        new Run(text.Substring(foundIndex, highlightText.Length))
                        {
                            FontWeight = highlightFontWeight,
                            Foreground = highlightColor,
                        });

                    index = foundIndex + highlightText.Length;
                }
                else
                {
                    collection.Add(
                        new Run(text[index..]));
                    break;
                }
            }
        }
    }
}