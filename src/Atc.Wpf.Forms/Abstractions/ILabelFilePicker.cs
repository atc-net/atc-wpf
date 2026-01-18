namespace Atc.Wpf.Forms.Abstractions;

public interface ILabelFilePicker : ILabelControl
{
    FileInfo? Value { get; set; }

    bool ShowClearTextButton { get; set; }

    bool AllowOnlyExisting { get; set; }

    string Filter { get; set; }

    bool UsePreviewPane { get; set; }

    string DefaultDirectory { get; set; }

    string InitialDirectory { get; set; }

    string RootDirectory { get; set; }
}