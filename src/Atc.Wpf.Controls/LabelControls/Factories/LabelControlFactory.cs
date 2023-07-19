// ReSharper disable InvertIf
namespace Atc.Wpf.Controls.LabelControls.Factories;

public static class LabelControlFactory
{
    public static LabelComboBox CreateLabelEnumPicker(
        PropertyInfo propertyInfo,
        string groupIdentifier,
        bool isReadOnly,
        string? selectedKey)
    {
        ArgumentNullException.ThrowIfNull(propertyInfo);

        var control = new LabelComboBox
        {
            GroupIdentifier = groupIdentifier,
            LabelText = propertyInfo.Name.NormalizePascalCase(),
            IsEnabled = !isReadOnly,
            IsMandatory = propertyInfo.HasRequiredAttribute(),
            Items = new Dictionary<string, string>(StringComparer.Ordinal),
        };

        const DropDownFirstItemType firstItem = DropDownFirstItemType.PleaseSelect;
        control.Items.Add(
            DropDownFirstItemTypeHelper.GetEnumGuid(firstItem).ToString(),
            firstItem.ToString().NormalizePascalCase());

        var nonNullableType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
        foreach (var enumValue in nonNullableType.GetEnumValues())
        {
            var s = enumValue.ToString()!;
            if (s.Equals("None", StringComparison.Ordinal) ||
                s.Equals("Unknown", StringComparison.Ordinal) ||
                s.Equals("Default", StringComparison.Ordinal))
            {
                continue;
            }

            control.Items.Add(s, s.NormalizePascalCase());
        }

        if (selectedKey is not null)
        {
            control.SelectedKey = selectedKey;
        }

        return control;
    }

    public static LabelCheckBox CreateLabelCheckBox(
        PropertyInfo propertyInfo,
        string groupIdentifier,
        bool isReadOnly,
        bool? value)
    {
        ArgumentNullException.ThrowIfNull(propertyInfo);

        var control = new LabelCheckBox
        {
            GroupIdentifier = groupIdentifier,
            LabelText = propertyInfo.Name.NormalizePascalCase(),
            IsEnabled = !isReadOnly,
            IsChecked = value ?? false,
        };

        return control;
    }

    public static LabelDecimalBox CreateLabelDecimalBox(
        PropertyInfo propertyInfo,
        string groupIdentifier,
        bool isReadOnly,
        Type inputType,
        decimal value)
    {
        ArgumentNullException.ThrowIfNull(propertyInfo);

        var control = new LabelDecimalBox
        {
            GroupIdentifier = groupIdentifier,
            InputDataType = inputType,
            LabelText = propertyInfo.Name.NormalizePascalCase(),
            IsEnabled = !isReadOnly,
            IsMandatory = propertyInfo.HasRequiredAttribute(),
            WatermarkText = propertyInfo.GetDescription().NormalizePascalCase(),
            Value = value,
        };

        var customAttributes = propertyInfo
            .GetCustomAttributes()
            .ToArray();

        if (!customAttributes.Any())
        {
            return control;
        }

        var rangeLengthAttribute = customAttributes.FirstOrDefault(x => x.GetType() == typeof(RangeAttribute));
        if (rangeLengthAttribute is not null)
        {
            control.Minimum = (decimal)((RangeAttribute)rangeLengthAttribute).Minimum;
            control.Maximum = (decimal)((RangeAttribute)rangeLengthAttribute).Maximum;
        }

        return control;
    }

    public static LabelDecimalXyBox CreateLabelDecimalXyBox(
        PropertyInfo propertyInfo,
        string groupIdentifier,
        bool isReadOnly,
        Type inputType,
        decimal valueX,
        decimal valueY)
    {
        ArgumentNullException.ThrowIfNull(propertyInfo);

        var control = new LabelDecimalXyBox
        {
            GroupIdentifier = groupIdentifier,
            InputDataType = inputType,
            LabelText = propertyInfo.Name.NormalizePascalCase(),
            IsEnabled = !isReadOnly,
            IsMandatory = propertyInfo.HasRequiredAttribute(),
            ValueX = valueX,
            ValueY = valueY,
        };

        var customAttributes = propertyInfo
            .GetCustomAttributes()
            .ToArray();

        if (!customAttributes.Any())
        {
            return control;
        }

        var rangeLengthAttribute = customAttributes.FirstOrDefault(x => x.GetType() == typeof(RangeAttribute));
        if (rangeLengthAttribute is not null)
        {
            control.Minimum = (decimal)((RangeAttribute)rangeLengthAttribute).Minimum;
            control.Maximum = (decimal)((RangeAttribute)rangeLengthAttribute).Maximum;
        }

        return control;
    }

    public static LabelIntegerBox CreateLabelIntegerBox(
        PropertyInfo propertyInfo,
        string groupIdentifier,
        bool isReadOnly,
        Type inputType,
        int? value)
    {
        ArgumentNullException.ThrowIfNull(propertyInfo);

        var control = new LabelIntegerBox
        {
            GroupIdentifier = groupIdentifier,
            InputDataType = inputType,
            LabelText = propertyInfo.Name.NormalizePascalCase(),
            IsEnabled = !isReadOnly,
            IsMandatory = propertyInfo.HasRequiredAttribute(),
            Value = value ?? 0,
            WatermarkText = propertyInfo.GetDescription().NormalizePascalCase(),
        };

        var customAttributes = propertyInfo
            .GetCustomAttributes()
            .ToArray();

        if (!customAttributes.Any())
        {
            return control;
        }

        var rangeLengthAttribute = customAttributes.FirstOrDefault(x => x.GetType() == typeof(RangeAttribute));
        if (rangeLengthAttribute is not null)
        {
            control.Minimum = (int)((RangeAttribute)rangeLengthAttribute).Minimum;
            control.Maximum = (int)((RangeAttribute)rangeLengthAttribute).Maximum;
        }

        return control;
    }

    public static LabelPixelSizeBox CreateLabelPixelSizeBox(
        PropertyInfo propertyInfo,
        string groupIdentifier,
        bool isReadOnly,
        Type inputType,
        int valueWidth,
        int valueHeight)
    {
        ArgumentNullException.ThrowIfNull(propertyInfo);

        var control = new LabelPixelSizeBox
        {
            GroupIdentifier = groupIdentifier,
            InputDataType = inputType,
            LabelText = propertyInfo.Name.NormalizePascalCase(),
            IsEnabled = !isReadOnly,
            IsMandatory = propertyInfo.HasRequiredAttribute(),
            ValueWidth = valueWidth,
            ValueHeight = valueHeight,
        };

        var customAttributes = propertyInfo
            .GetCustomAttributes()
            .ToArray();

        if (!customAttributes.Any())
        {
            return control;
        }

        var rangeLengthAttribute = customAttributes.FirstOrDefault(x => x.GetType() == typeof(RangeAttribute));
        if (rangeLengthAttribute is not null)
        {
            control.Minimum = (int)((RangeAttribute)rangeLengthAttribute).Minimum;
            control.Maximum = (int)((RangeAttribute)rangeLengthAttribute).Maximum;
        }

        return control;
    }

    public static LabelTextBox CreateLabelTextBox(
        PropertyInfo propertyInfo,
        string groupIdentifier,
        bool isReadOnly,
        string value)
    {
        ArgumentNullException.ThrowIfNull(propertyInfo);

        var control = new LabelTextBox
        {
            GroupIdentifier = groupIdentifier,
            LabelText = propertyInfo.Name.NormalizePascalCase(),
            IsEnabled = !isReadOnly,
            IsMandatory = propertyInfo.HasRequiredAttribute(),
            Text = value,
            WatermarkText = propertyInfo.GetDescription().NormalizePascalCase(),
        };

        var customAttributes = propertyInfo
            .GetCustomAttributes()
            .ToArray();

        if (!customAttributes.Any())
        {
            return control;
        }

        var minLengthAttribute = customAttributes.FirstOrDefault(x => x.GetType() == typeof(MinLengthAttribute));
        if (minLengthAttribute is not null)
        {
            control.MinLength = (uint)((MinLengthAttribute)minLengthAttribute).Length;
        }

        var maxLengthAttribute = customAttributes.FirstOrDefault(x => x.GetType() == typeof(MaxLengthAttribute));
        if (maxLengthAttribute is not null)
        {
            control.MaxLength = (uint)((MaxLengthAttribute)maxLengthAttribute).Length;
        }

        var regexAttribute = customAttributes.FirstOrDefault(x => x.GetType() == typeof(RegularExpressionAttribute));
        if (regexAttribute is not null)
        {
            control.RegexPattern = ((RegularExpressionAttribute)regexAttribute).Pattern;
        }

        return control;
    }

    public static LabelWellKnownColorSelector CreateLabelWellKnownColorSelector(
        PropertyInfo propertyInfo,
        string groupIdentifier,
        bool isReadOnly,
        string? defaultColorName)
    {
        ArgumentNullException.ThrowIfNull(propertyInfo);

        var control = new LabelWellKnownColorSelector
        {
            GroupIdentifier = groupIdentifier,
            LabelText = propertyInfo.Name.NormalizePascalCase(),
            IsEnabled = !isReadOnly,
            DropDownFirstItemType = DropDownFirstItemType.PleaseSelect,
            IsMandatory = propertyInfo.HasRequiredAttribute() || !propertyInfo.IsNullable(),
            DefaultColorName = defaultColorName,
        };

        return control;
    }

    public static LabelCountrySelector CreateLabelCountrySelector(
        PropertyInfo propertyInfo,
        string groupIdentifier,
        bool isReadOnly,
        string? selectedKey)
    {
        ArgumentNullException.ThrowIfNull(propertyInfo);

        var control = new LabelCountrySelector
        {
            GroupIdentifier = groupIdentifier,
            LabelText = propertyInfo.Name.NormalizePascalCase(),
            IsEnabled = !isReadOnly,
            DropDownFirstItemType = DropDownFirstItemType.PleaseSelect,
            UseOnlySupportedCountries = false,
            IsMandatory = propertyInfo.HasRequiredAttribute() || !propertyInfo.IsNullable(),
        };

        if (selectedKey is not null)
        {
            control.SelectedKey = selectedKey;
        }

        return control;
    }

    public static LabelLanguageSelector CreateLabelLanguageSelector(
        PropertyInfo propertyInfo,
        string groupIdentifier,
        bool isReadOnly,
        string? selectedKey)
    {
        ArgumentNullException.ThrowIfNull(propertyInfo);

        var control = new LabelLanguageSelector
        {
            GroupIdentifier = groupIdentifier,
            LabelText = propertyInfo.Name.NormalizePascalCase(),
            IsEnabled = !isReadOnly,
            DropDownFirstItemType = DropDownFirstItemType.PleaseSelect,
            UseOnlySupportedLanguages = false,
            UpdateUiCultureOnChangeEvent = false,
            IsMandatory = propertyInfo.HasRequiredAttribute() || !propertyInfo.IsNullable(),
        };

        if (selectedKey is not null)
        {
            control.SelectedKey = selectedKey;
        }

        return control;
    }
}