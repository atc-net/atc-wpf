namespace Atc.Wpf.Forms.Writers;

public static class LabelControlsFormToModelWriter
{
    public static T Update<T>(
        T instance,
        ILabelControlsForm labelControlsForm)
    {
        ArgumentNullException.ThrowIfNull(instance);
        ArgumentNullException.ThrowIfNull(labelControlsForm);

        return Update<T>(
            instance,
            labelControlsForm.GetKeyValues());
    }

    public static T Update<T>(
        T instance,
        Dictionary<string, object> keyValues)
    {
        ArgumentNullException.ThrowIfNull(instance);
        ArgumentNullException.ThrowIfNull(keyValues);

        var serializerOptions = JsonSerializerOptionsFactory.Create();
        serializerOptions.PropertyNamingPolicy = null;
        serializerOptions.Converters.Add(new ColorToHexJsonConverter());
        serializerOptions.Converters.Add(new CultureInfoToNameJsonConverter());
        serializerOptions.Converters.Add(new DirectoryInfoToFullNameJsonConverter());
        serializerOptions.Converters.Add(new FileInfoToFullNameJsonConverter());
        serializerOptions.Converters.Add(new UriToAbsoluteUriJsonConverter());

        return Update(instance, keyValues, serializerOptions);
    }

    public static T Update<T>(
        T instance,
        Dictionary<string, object> keyValues,
        JsonSerializerOptions serializerOptions)
    {
        ArgumentNullException.ThrowIfNull(instance);
        ArgumentNullException.ThrowIfNull(keyValues);
        ArgumentNullException.ThrowIfNull(serializerOptions);

        var json = JsonSerializer.Serialize(instance, serializerOptions);

        var dynamicJson = new DynamicJson(json);

        foreach (var item in keyValues)
        {
            var key = item.Key;
            var indexOfFirstDot = item.Key.IndexOf('.', StringComparison.Ordinal);
            if (indexOfFirstDot != -1)
            {
                key = key[(indexOfFirstDot + 1)..];
            }

            dynamicJson.SetValue(key, item.Value);
        }

        var newJson = dynamicJson.ToJson(serializerOptions);
        return JsonSerializer.Deserialize<T>(newJson, serializerOptions)!;
    }
}