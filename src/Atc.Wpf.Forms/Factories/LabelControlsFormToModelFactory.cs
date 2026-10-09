namespace Atc.Wpf.Forms.Factories;

/// <summary>
/// Creates model instances populated from the values of a label controls form.
/// </summary>
public static class LabelControlsFormToModelFactory
{
    /// <summary>
    /// Creates a new model and fills it with the form's current values.
    /// </summary>
    /// <typeparam name="T">The model type.</typeparam>
    /// <param name="labelControlsForm">The form to read values from.</param>
    /// <returns>The new, populated model.</returns>
    public static T Create<T>(ILabelControlsForm labelControlsForm)
        where T : new()
    {
        ArgumentNullException.ThrowIfNull(labelControlsForm);

        var instance = new T();

        return LabelControlsFormToModelWriter.Update<T>(
            instance,
            labelControlsForm.GetKeyValues());
    }

    /// <summary>
    /// Creates a new model and fills it from key/value pairs, where each key is a (group-prefixed) property path.
    /// </summary>
    /// <typeparam name="T">The model type.</typeparam>
    /// <param name="keyValues">The values to apply, keyed by identifier.</param>
    /// <returns>The new, populated model.</returns>
    public static T Create<T>(Dictionary<string, object> keyValues)
        where T : new()
    {
        ArgumentNullException.ThrowIfNull(keyValues);

        var instance = new T();

        return LabelControlsFormToModelWriter.Update<T>(
            instance,
            keyValues);
    }
}