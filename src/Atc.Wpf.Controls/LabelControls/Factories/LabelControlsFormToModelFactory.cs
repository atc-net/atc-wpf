namespace Atc.Wpf.Controls.LabelControls.Factories;

public static class LabelControlsFormToModelFactory
{
    public static T Create<T>(
        ILabelControlsForm labelControlsForm)
    {
        ArgumentNullException.ThrowIfNull(labelControlsForm);

        return Create<T>(labelControlsForm.GetKeyValues());
    }

    public static T Create<T>(
        Dictionary<string, object> keyValues)
    {
        ArgumentNullException.ThrowIfNull(keyValues);

        throw new NotImplementedException();
    }
}