namespace Atc.Wpf.Controls.LabelControls.Extractors;

public static class DictionaryToLabelControlExtractor
{
    public static IList<ILabelControlBase> Extract(
        Dictionary<string, LabelControlData> data,
        string groupIdentifier = "")
    {
        ArgumentNullException.ThrowIfNull(data);

        var labelControls = new List<ILabelControlBase>();
        foreach (var controlData in data)
        {
            var type = controlData.Value.DataType;

            var nonNullableType = Nullable.GetUnderlyingType(type) ?? type;
            if (nonNullableType.IsEnum)
            {
                // TODO: labelControls.Add(CreateLabelEnumPicker(propertyInfo, model, typeGroupIdentifier, isReadOnly));
                continue;
            }

            if (type == typeof(bool) ||
                type == typeof(bool?))
            {
                // TODO: labelControls.Add(CreateLabelCheckBox(propertyInfo, model, typeGroupIdentifier, isReadOnly));
                continue;
            }

            if (type == typeof(decimal) ||
                type == typeof(decimal?) ||
                type == typeof(double) ||
                type == typeof(double?) ||
                type == typeof(float) ||
                type == typeof(float?))
            {
                // TODO: labelControls.Add(CreateLabelDecimalBox(propertyInfo, model, typeGroupIdentifier, isReadOnly, type));
                continue;
            }

            if (type == typeof(Point2D) ||
                type == typeof(Point2D?))
            {
                // TODO: labelControls.Add(CreateLabelDecimalXyBox(propertyInfo, model, typeGroupIdentifier, isReadOnly, type));
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
                // TODO: labelControls.Add(CreateLabelIntegerBox(propertyInfo, model, typeGroupIdentifier, isReadOnly, type));
                continue;
            }

            if (type == typeof(Size) ||
                type == typeof(Size?))
            {
                // TODO: labelControls.Add(CreateLabelPixelSizeBox(propertyInfo, model, typeGroupIdentifier, isReadOnly, type));
                continue;
            }

            if (type == typeof(string) ||
                type == typeof(Guid) ||
                type == typeof(Guid?))
            {
                labelControls.Add(LabelControlFactory.CreateLabelTextBox(
                    groupIdentifier: GetTypeGroupIdentifier(type, groupIdentifier),
                    labelText: controlData.Value.LabelText,
                    isReadOnly: controlData.Value.IsReadOnly,
                    isMandatory: controlData.Value.IsMandatory,
                    watermarkText: controlData.Value.WatermarkText ?? string.Empty,
                    value: controlData.Value.LabelText,
                    minLength: controlData.Value.Minimum,
                    maxLength: controlData.Value.Minimum,
                    regexPattern: controlData.Value.RegexPattern));
                continue;
            }

            if (type == typeof(Color) ||
                type == typeof(Color?))
            {
                // TODO: labelControls.Add(CreateLabelColorSelector(propertyInfo, model, typeGroupIdentifier, isReadOnly));
                continue;
            }

            if (type == typeof(CultureInfo))
            {
                if (controlData.Key.Contains("Country", StringComparison.OrdinalIgnoreCase))
                {
                    // TODO: labelControls.Add(CreateLabelCountrySelector(propertyInfo, model, typeGroupIdentifier, isReadOnly));
                    continue;
                }

                if (controlData.Key.Contains("Language", StringComparison.OrdinalIgnoreCase))
                {
                    // TODO: labelControls.Add(CreateLabelLanguageSelector(propertyInfo, model, typeGroupIdentifier, isReadOnly));
                    continue;
                }
            }
        }

        return labelControls;
    }

    private static string GetTypeGroupIdentifier(
        MemberInfo type,
        string groupIdentifier)
    {
        ArgumentNullException.ThrowIfNull(type);

        return string.IsNullOrEmpty(groupIdentifier)
            ? type.Name
            : groupIdentifier;
    }
}