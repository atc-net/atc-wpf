namespace Atc.Wpf.Controls.LabelControls.Abstractions;

public interface ILabelTextBox : ILabelControl
{
    uint MinLength { get; set; }

    uint MaxLength { get; set; }

    bool UseDefaultNotAllowedCharacters { get; set; }

    string CharactersNotAllowed { get; set; }

    string Text { get; set; }

    string WatermarkText { get; set; }
}