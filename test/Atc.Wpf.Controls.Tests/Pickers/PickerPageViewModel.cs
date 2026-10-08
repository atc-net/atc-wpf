namespace Atc.Wpf.Controls.Tests.Pickers;

// Bindings need a public type, so the page view model used by the picker tests lives in its own file.
public sealed class PickerPageViewModel
{
    public const string WatermarkValue = "From the page";

    public string Watermark => WatermarkValue;
}