namespace Atc.Wpf.Controls.LabelControls.Abstractions;

public interface ILabelTextBox : ILabelTextControl
{
    string WatermarkText { get; set; }

    TextAlignment WatermarkAlignment { get; set; }

    uint MaxLength { get; set; }

    uint MinLength { get; set; }

    bool UseDefaultNotAllowedCharacters { get; set; }

    string CharactersNotAllowed { get; set; }

    bool ShowClearTextButton { get; set; }

    string Text { get; set; }
}