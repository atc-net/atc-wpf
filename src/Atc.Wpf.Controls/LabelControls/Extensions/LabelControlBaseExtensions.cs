// ReSharper disable CheckNamespace
namespace Atc.Wpf.Controls.LabelControls;

public static class LabelControlBaseExtensions
{
    public static bool IsValid(
        this ILabelControlBase labelControl)
        => labelControl is not ILabelControl control || control.IsValid();
}