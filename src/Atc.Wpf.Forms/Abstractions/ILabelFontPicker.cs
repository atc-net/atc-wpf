namespace Atc.Wpf.Forms.Abstractions;

public interface ILabelFontPicker : ILabelControl
{
    FontFamily? SelectedFontFamily { get; set; }

    double SelectedFontSize { get; set; }

    FontWeight SelectedFontWeight { get; set; }

    FontStyle SelectedFontStyle { get; set; }

    FontStretch SelectedFontStretch { get; set; }

    SolidColorBrush? SelectedForegroundBrush { get; set; }

    SolidColorBrush? SelectedBackgroundBrush { get; set; }

    TextDecorationCollection? SelectedTextDecorations { get; set; }
}