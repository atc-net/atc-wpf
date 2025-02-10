namespace Atc.Wpf.MarkupExtensions;

[MarkupExtensionReturnType(typeof(Style))]
public sealed class MultiStyleExtension : MarkupExtension
{
    private readonly string[] resourceKeys;

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