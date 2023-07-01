namespace Atc.Wpf.Sample.SamplesWpfFontIcons;

internal sealed class ImageSet
{
    public ImageSet(
        ImageSource imageSource,
        string label,
        string toolTip)
    {
        ImageSource = imageSource;
        ImageSource.Freeze();
        Label = label;
        ToolTip = toolTip;
    }

    public ImageSource ImageSource { get; }

    public string Label { get; }

    public string ToolTip { get; }

    public override string ToString()
        => $"{nameof(Label)}: {Label}, {nameof(ToolTip)}: {ToolTip}";
}