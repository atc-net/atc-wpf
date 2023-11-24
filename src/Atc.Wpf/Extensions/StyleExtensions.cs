// ReSharper disable once CheckNamespace
namespace System.Windows;

public static class StyleExtensions
{
    public static void Merge(
        this Style style1,
        Style style2)
    {
        ArgumentNullException.ThrowIfNull(style1);
        ArgumentNullException.ThrowIfNull(style2);

        if (style1.TargetType.IsAssignableFrom(style2.TargetType))
        {
            style1.TargetType = style2.TargetType;
        }

        if (style2.BasedOn != null)
        {
            Merge(style1, style2.BasedOn);
        }

        foreach (var currentSetter in style2.Setters)
        {
            style1.Setters.Add(currentSetter);
        }

        foreach (var currentTrigger in style2.Triggers)
        {
            style1.Triggers.Add(currentTrigger);
        }

        // This code is only needed when using DynamicResources.
        foreach (var key in style2.Resources.Keys)
        {
            style1.Resources[key] = style2.Resources[key];
        }
    }
}