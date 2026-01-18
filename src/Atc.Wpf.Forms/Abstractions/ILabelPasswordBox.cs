namespace Atc.Wpf.Forms.Abstractions;

public interface ILabelPasswordBox : ILabelControl
{
    string WatermarkText { get; set; }

    TextAlignment WatermarkAlignment { get; set; }

    TextTrimming WatermarkTrimming { get; set; }

    uint MaxLength { get; set; }

    uint MinLength { get; set; }

    bool ShowClearTextButton { get; set; }

    string Text { get; set; }
}