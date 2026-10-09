namespace Atc.Wpf.MarkupExtensions;

/// <summary>
/// A markup extension that merges multiple styles, identified by space-separated resource keys, into a single <see cref="Style"/>.
/// </summary>
/// <remarks>
/// A key of <c>.</c> refers to the default style of the target object's type.
/// </remarks>
[MarkupExtensionReturnType(typeof(Style))]
public sealed class MultiStyleExtension : MarkupExtension
{
    private readonly string[] resourceKeys;

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiStyleExtension"/> class.
    /// </summary>
    /// <param name="inputResourceKeys">The space-separated resource keys of the styles to merge.</param>
    public MultiStyleExtension(string inputResourceKeys)
    {
        if (string.IsNullOrWhiteSpace(inputResourceKeys))
        {
            throw new ArgumentNullException(nameof(inputResourceKeys));
        }

        resourceKeys = inputResourceKeys.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        if (resourceKeys.Length == 0)
        {
            throw new DataException();
        }
    }

    /// <inheritdoc />
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        var resultStyle = new Style();

        foreach (var resourceKey in resourceKeys)
        {
            var key = (object)resourceKey;
            if (resourceKey == ".")
            {
                var service = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget))!;
                key = service.TargetObject.GetType();
            }

            if (new StaticResourceExtension(key).ProvideValue(serviceProvider) is not Style currentStyle)
            {
                continue;
            }

            resultStyle.Merge(currentStyle);
        }

        return resultStyle;
    }
}