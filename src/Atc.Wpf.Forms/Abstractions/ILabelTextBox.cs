namespace Atc.Wpf.Forms.Abstractions;

public interface ILabelTextBox : ILabelControl
{
    string WatermarkText { get; set; }

    TextAlignment WatermarkAlignment { get; set; }

    TextTrimming WatermarkTrimming { get; set; }

    uint MaxLength { get; set; }

    uint MinLength { get; set; }

    bool UseDefaultNotAllowedCharacters { get; set; }

    string CharactersNotAllowed { get; set; }

    string? RegexPattern { get; set; }

    bool ShowClearTextButton { get; set; }

    string Text { get; set; }
}