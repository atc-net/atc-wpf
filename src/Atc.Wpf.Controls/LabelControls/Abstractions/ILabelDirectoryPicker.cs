namespace Atc.Wpf.Controls.LabelControls.Abstractions;

public interface ILabelDirectoryPicker : ILabelControl
{
    DirectoryInfo? Value { get; set; }

    bool ShowClearTextButton { get; set; }

    bool AllowOnlyExisting { get; set; }

    string DefaultDirectory { get; set; }

    string InitialDirectory { get; set; }

    string RootDirectory { get; set; }
}