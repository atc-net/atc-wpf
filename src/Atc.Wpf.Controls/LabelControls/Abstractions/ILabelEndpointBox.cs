namespace Atc.Wpf.Controls.LabelControls.Abstractions;

public interface ILabelEndpointBox : ILabelControl
{
    string WatermarkText { get; set; }

    bool ShowClearTextButton { get; set; }

    bool HideUpDownButtons { get; set; }

    int MinimumPort { get; set; }

    int MaximumPort { get; set; }

    NetworkProtocolType NetworkProtocol { get; set; }

    NetworkValidationRule NetworkValidation { get; set; }

    string Host { get; set; }

    int Port { get; set; }

    Uri? Value { get; set; }
}