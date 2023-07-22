namespace Atc.Wpf.Controls.LabelControls.Factories;

public static class LabelControlsFormToModelFactory
{
    public static T Create<T>(
        ILabelControlsForm labelControlsForm)
        where T : new()
    {
        ArgumentNullException.ThrowIfNull(labelControlsForm);

        var instance = new T();

        return LabelControlsFormToModelWriter.Update<T>(
            instance,
            labelControlsForm.GetKeyValues());
    }

    public static T Create<T>(
        Dictionary<string, object> keyValues)
        where T : new()
    {
        ArgumentNullException.ThrowIfNull(keyValues);

        var instance = new T();

        return LabelControlsFormToModelWriter.Update<T>(
            instance,
            keyValues);
    }
}