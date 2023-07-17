namespace Atc.Wpf.Controls.LabelControls.Abstractions;

public interface ILabelControlBase
{
    string Identifier { get; }

    string? GroupIdentifier { get; set; }

    Type? InputDataType { get; set; }

    double Width { get; set; }

    Brush Background { get; set; }

    LabelControlHideAreasType HideAreas { get; set; }

    Orientation Orientation { get; set; }

    string LabelText { get; set; }

    int LabelWidthNumber { get; set; }

    SizeDefinitionType LabelWidthSizeDefinition { get; set; }

    string InformationText { get; set; }

    Color InformationColor { get; set; }
}