namespace Atc.Wpf.Forms.Abstractions;

public interface ILabelTextInfo : ILabelControlBase
{
    bool EnableCopyToClipboard { get; set; }

    string Text { get; set; }
}