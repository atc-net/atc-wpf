namespace Atc.Wpf.Controls.Inputs.Internal;

internal static class ControlHelper
{
    public static string GetIdentifier(
        FrameworkElement frameworkElement,
        string? labelText = "")
    {
        if (!string.IsNullOrEmpty(frameworkElement.Name))
        {
            return frameworkElement.Name;
        }

        if (frameworkElement.Tag is not null)
        {
            return frameworkElement.Tag.ToString()!;
        }

        if (string.IsNullOrEmpty(labelText))
        {
            return $"-Control-{frameworkElement.GetType().Name}_{Guid.NewGuid()}";
        }

        return Constants.DefaultLabelControlLabel.Equals(labelText, StringComparison.Ordinal)
            ? $"{labelText}_{Guid.NewGuid()}"
            : labelText;
    }
}