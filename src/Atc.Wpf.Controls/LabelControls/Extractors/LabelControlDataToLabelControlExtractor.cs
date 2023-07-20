// ReSharper disable InvertIf
// ReSharper disable RedundantJumpStatement
namespace Atc.Wpf.Controls.LabelControls.Extractors;

public static class LabelControlDataToLabelControlExtractor
{
    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
    public static IList<ILabelControlBase> Extract(
        IReadOnlyList<LabelControlData> data,
        string groupIdentifier = "")
    {
        ArgumentNullException.ThrowIfNull(data);

        var labelControls = new List<ILabelControlBase>();
        foreach (var controlData in data)
        {
            var type = controlData.DataType;

            var nonNullableType = type.GetNonNullableType();
            if (nonNullableType.IsEnum)
            {
                labelControls.Add(
                    LabelControlFactory.CreateLabelEnumPicker(
                        groupIdentifier: groupIdentifier,
                        labelText: controlData.LabelText,
                        isReadOnly: controlData.IsReadOnly,
                        isMandatory: controlData.IsMandatory,
                        selectedKey: controlData.Value?.ToString(),
                        nonNullableType));
                continue;
            }

            if (type == typeof(bool) ||
                type == typeof(bool?))
            {
                labelControls.Add(
                    LabelControlFactory.CreateLabelCheckBox(
                        groupIdentifier: groupIdentifier,
                        labelText: controlData.LabelText,
                        isReadOnly: controlData.IsReadOnly,
                        value: (bool?)controlData.Value));
                continue;
            }

            if (type == typeof(decimal) ||
                type == typeof(decimal?) ||
                type == typeof(double) ||
                type == typeof(double?) ||
                type == typeof(float) ||
                type == typeof(float?))
            {
                labelControls.Add(
                    LabelControlFactory.CreateLabelDecimalBox(
                        groupIdentifier: groupIdentifier,
                        labelText: controlData.LabelText,
                        isReadOnly: controlData.IsReadOnly,
                        isMandatory: controlData.IsMandatory,
                        watermarkText: controlData.WatermarkText ?? string.Empty,
                        inputDataType: type,
                        value: (int?)controlData.Value,
                        minimum: controlData.Minimum,
                        maximum: controlData.Maximum));
                continue;
            }

            if (type == typeof(Point2D) ||
                type == typeof(Point2D?))
            {
                decimal? minimum = null;
                if (controlData.Minimum is not null)
                {
                    minimum = (int)controlData.Minimum;
                }

                decimal? maximum = null;
                if (controlData.Maximum is not null)
                {
                    maximum = (int)controlData.Maximum;
                }

                labelControls.Add(
                    LabelControlFactory.CreateLabelDecimalXyBox(
                        groupIdentifier: groupIdentifier,
                        labelText: controlData.LabelText,
                        isReadOnly: controlData.IsReadOnly,
                        isMandatory: controlData.IsMandatory,
                        inputDataType: type,
                        valueX: (decimal?)controlData.Value,
                        valueY: (decimal?)controlData.Value2,
                        minimum: minimum,
                        maximum: maximum));
                continue;
            }

            if (type == typeof(int) ||
                type == typeof(int?) ||
                type == typeof(uint) ||
                type == typeof(uint?) ||
                type == typeof(long) ||
                type == typeof(long?) ||
                type == typeof(ulong) ||
                type == typeof(ulong?) ||
                type == typeof(short) ||
                type == typeof(short?) ||
                type == typeof(ushort) ||
                type == typeof(ushort?))
            {
                int? minimum = null;
                if (controlData.Minimum is not null)
                {
                    minimum = (int)controlData.Minimum;
                }

                int? maximum = null;
                if (controlData.Maximum is not null)
                {
                    maximum = (int)controlData.Maximum;
                }

                labelControls.Add(
                    LabelControlFactory.CreateLabelIntegerBox(
                        groupIdentifier: groupIdentifier,
                        labelText: controlData.LabelText,
                        isReadOnly: controlData.IsReadOnly,
                        isMandatory: controlData.IsMandatory,
                        watermarkText: controlData.WatermarkText ?? string.Empty,
                        inputDataType: type,
                        value: (int?)controlData.Value,
                        minimum: minimum,
                        maximum: maximum));
                continue;
            }

            if (type == typeof(Size) ||
                type == typeof(Size?))
            {
                int? minimum = null;
                if (controlData.Minimum is not null)
                {
                    minimum = (int)controlData.Minimum;
                }

                int? maximum = null;
                if (controlData.Maximum is not null)
                {
                    maximum = (int)controlData.Maximum;
                }

                labelControls.Add(
                    LabelControlFactory.CreateLabelPixelSizeBox(
                        groupIdentifier: groupIdentifier,
                        labelText: controlData.LabelText,
                        isReadOnly: controlData.IsReadOnly,
                        isMandatory: controlData.IsMandatory,
                        inputDataType: type,
                        valueWidth: (int?)controlData.Value,
                        valueHeight: (int?)controlData.Value2,
                        minimum: minimum,
                        maximum: maximum));
                continue;
            }

            if (type == typeof(string) ||
                type == typeof(Guid) ||
                type == typeof(Guid?))
            {
                uint? minLength = null;
                if (controlData.Minimum is not null)
                {
                    minLength = (uint)controlData.Minimum;
                }

                uint? maxLength = null;
                if (controlData.Maximum is not null)
                {
                    maxLength = (uint)controlData.Maximum;
                }

                labelControls.Add(
                    LabelControlFactory.CreateLabelTextBox(
                        groupIdentifier: groupIdentifier,
                        labelText: controlData.LabelText,
                        isReadOnly: controlData.IsReadOnly,
                        isMandatory: controlData.IsMandatory,
                        watermarkText: controlData.WatermarkText ?? string.Empty,
                        inputDataType: type,
                        value: controlData.Value?.ToString(),
                        minLength: minLength,
                        maxLength: maxLength,
                        regexPattern: controlData.RegexPattern));
                continue;
            }

            if (type == typeof(Color) ||
                type == typeof(Color?))
            {
                labelControls.Add(
                    LabelControlFactory.CreateLabelWellKnownColorSelector(
                        groupIdentifier: groupIdentifier,
                        labelText: controlData.LabelText,
                        isReadOnly: controlData.IsReadOnly,
                        isMandatory: controlData.IsMandatory,
                        inputDataType: type,
                        defaultColorName: controlData.Value?.ToString()));
                continue;
            }

            if (type == typeof(CultureInfo))
            {
                if (controlData.LabelText.Contains("Country", StringComparison.OrdinalIgnoreCase))
                {
                    labelControls.Add(
                        LabelControlFactory.CreateLabelCountrySelector(
                            groupIdentifier: groupIdentifier,
                            labelText: controlData.LabelText,
                            isReadOnly: controlData.IsReadOnly,
                            isMandatory: controlData.IsMandatory,
                            inputDataType: type,
                            defaultCultureIdentifier: controlData.Value?.ToString()));
                    continue;
                }

                if (controlData.LabelText.Contains("Language", StringComparison.OrdinalIgnoreCase))
                {
                    labelControls.Add(
                        LabelControlFactory.CreateLabelLanguageSelector(
                            groupIdentifier: groupIdentifier,
                            labelText: controlData.LabelText,
                            isReadOnly: controlData.IsReadOnly,
                            isMandatory: controlData.IsMandatory,
                            inputDataType: type,
                            defaultCultureIdentifier: controlData.Value?.ToString()));
                    continue;
                }
            }
        }

        return labelControls;
    }
}