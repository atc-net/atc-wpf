namespace Atc.Wpf.Forms.Writers;

/// <summary>
/// Writes the values of a label controls form back into a model, using a JSON round-trip.
/// </summary>
public static class LabelControlsFormToModelWriter
{
    /// <summary>
    /// Returns a copy of the model updated with the form's current values.
    /// </summary>
    /// <typeparam name="T">The model type.</typeparam>
    /// <param name="instance">The model to start from.</param>
    /// <param name="labelControlsForm">The form to read values from.</param>
    /// <returns>A new model instance with the form values applied.</returns>
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

    /// <summary>
    /// Returns a copy of the model updated with the given values, using default JSON options with converters for colors, cultures, files, directories and URIs.
    /// </summary>
    /// <typeparam name="T">The model type.</typeparam>
    /// <param name="instance">The model to start from.</param>
    /// <param name="keyValues">The values to apply, keyed by identifier; any group prefix before the first dot is ignored.</param>
    /// <returns>A new model instance with the values applied.</returns>
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

    /// <summary>
    /// Returns a copy of the model updated with the given values, using the specified JSON options.
    /// </summary>
    /// <typeparam name="T">The model type.</typeparam>
    /// <param name="instance">The model to start from.</param>
    /// <param name="keyValues">The values to apply, keyed by identifier; any group prefix before the first dot is ignored.</param>
    /// <param name="serializerOptions">The JSON options used to serialize and deserialize the model.</param>
    /// <returns>A new model instance with the values applied.</returns>
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