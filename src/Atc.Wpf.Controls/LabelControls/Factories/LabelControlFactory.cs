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

        return CreateLabelEnumPicker(
            groupIdentifier,
            propertyInfo.Name,
            isReadOnly,
            propertyInfo.HasRequiredAttribute(),
            selectedKey,
            propertyInfo.PropertyType);
    }

    public static LabelComboBox CreateLabelEnumPicker(
        string groupIdentifier,
        string labelText,
        bool isReadOnly,
        bool isMandatory,
        string? selectedKey,
        Type inputDataType)
    {
        var control = new LabelComboBox
        {
            GroupIdentifier = groupIdentifier,
            LabelText = labelText.NormalizePascalCase(),
            IsEnabled = !isReadOnly,
            IsMandatory = isMandatory,
            Items = new Dictionary<string, string>(StringComparer.Ordinal),
        };

        const DropDownFirstItemType firstItem = DropDownFirstItemType.PleaseSelect;
        control.Items.Add(
            DropDownFirstItemTypeHelper.GetEnumGuid(firstItem).ToString(),
            firstItem.ToString().NormalizePascalCase());

        var nonNullableType = inputDataType.GetNonNullableType();
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

    public static LabelCheckBox CreateLabelCheckBox(
        string groupIdentifier,
        string labelText,
        bool isReadOnly,
        bool? value)
    {
        var control = new LabelCheckBox
        {
            GroupIdentifier = groupIdentifier,
            LabelText = labelText.NormalizePascalCase(),
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

        decimal? minimum = null;
        decimal? maximum = null;

        var customAttributes = propertyInfo
            .GetCustomAttributes()
            .ToArray();

        if (customAttributes.Any())
        {
            var rangeLengthAttribute = customAttributes.FirstOrDefault(x => x.GetType() == typeof(RangeAttribute));
            if (rangeLengthAttribute is not null)
            {
                minimum = (decimal)((RangeAttribute)rangeLengthAttribute).Minimum;
                maximum = (decimal)((RangeAttribute)rangeLengthAttribute).Maximum;
            }
        }

        return CreateLabelDecimalBox(
            groupIdentifier,
            propertyInfo.Name,
            isReadOnly,
            propertyInfo.HasRequiredAttribute(),
            propertyInfo.GetDescription(),
            inputType,
            value,
            minimum,
            maximum);
    }

    public static LabelDecimalBox CreateLabelDecimalBox(
        string groupIdentifier,
        string labelText,
        bool isReadOnly,
        bool isMandatory,
        string watermarkText,
        Type inputDataType,
        decimal? value,
        decimal? minimum,
        decimal? maximum)
    {
        var control = new LabelDecimalBox
        {
            GroupIdentifier = groupIdentifier,
            LabelText = labelText.NormalizePascalCase(),
            IsEnabled = !isReadOnly,
            IsMandatory = isMandatory,
            InputDataType = inputDataType,
            Value = value ?? 0,
            WatermarkText = watermarkText.NormalizePascalCase(),
        };

        if (minimum.HasValue)
        {
            control.Minimum = minimum.Value;
        }

        if (maximum.HasValue)
        {
            control.Maximum = maximum.Value;
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

        decimal? minimum = null;
        decimal? maximum = null;

        var customAttributes = propertyInfo
            .GetCustomAttributes()
            .ToArray();

        if (customAttributes.Any())
        {
            var rangeLengthAttribute = customAttributes.FirstOrDefault(x => x.GetType() == typeof(RangeAttribute));
            if (rangeLengthAttribute is not null)
            {
                minimum = (decimal)((RangeAttribute)rangeLengthAttribute).Minimum;
                maximum = (decimal)((RangeAttribute)rangeLengthAttribute).Maximum;
            }
        }

        return CreateLabelDecimalXyBox(
            groupIdentifier,
            propertyInfo.Name,
            isReadOnly,
            propertyInfo.HasRequiredAttribute(),
            inputType,
            valueX,
            valueY,
            minimum,
            maximum);
    }

    public static LabelDecimalXyBox CreateLabelDecimalXyBox(
        string groupIdentifier,
        string labelText,
        bool isReadOnly,
        bool isMandatory,
        Type inputDataType,
        decimal? valueX,
        decimal? valueY,
        decimal? minimum,
        decimal? maximum)
    {
        var control = new LabelDecimalXyBox
        {
            GroupIdentifier = groupIdentifier,
            LabelText = labelText.NormalizePascalCase(),
            IsEnabled = !isReadOnly,
            IsMandatory = isMandatory,
            InputDataType = inputDataType,
            ValueX = valueX ?? 0,
            ValueY = valueY ?? 0,
        };

        if (minimum.HasValue)
        {
            control.Minimum = minimum.Value;
        }

        if (maximum.HasValue)
        {
            control.Maximum = maximum.Value;
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

        int? minimum = null;
        int? maximum = null;

        var customAttributes = propertyInfo
            .GetCustomAttributes()
            .ToArray();

        if (customAttributes.Any())
        {
            var rangeLengthAttribute = customAttributes.FirstOrDefault(x => x.GetType() == typeof(RangeAttribute));
            if (rangeLengthAttribute is not null)
            {
                minimum = (int)((RangeAttribute)rangeLengthAttribute).Minimum;
                maximum = (int)((RangeAttribute)rangeLengthAttribute).Maximum;
            }
        }

        return CreateLabelIntegerBox(
            groupIdentifier,
            propertyInfo.Name,
            isReadOnly,
            propertyInfo.HasRequiredAttribute(),
            propertyInfo.GetDescription(),
            inputType,
            value,
            minimum,
            maximum);
    }

    public static LabelIntegerBox CreateLabelIntegerBox(
        string groupIdentifier,
        string labelText,
        bool isReadOnly,
        bool isMandatory,
        string watermarkText,
        Type inputDataType,
        int? value,
        int? minimum,
        int? maximum)
    {
        var control = new LabelIntegerBox
        {
            GroupIdentifier = groupIdentifier,
            LabelText = labelText.NormalizePascalCase(),
            IsEnabled = !isReadOnly,
            IsMandatory = isMandatory,
            InputDataType = inputDataType,
            Value = value ?? 0,
            WatermarkText = watermarkText.NormalizePascalCase(),
        };

        if (minimum.HasValue)
        {
            control.Minimum = minimum.Value;
        }

        if (maximum.HasValue)
        {
            control.Maximum = maximum.Value;
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

        int? minimum = null;
        int? maximum = null;

        var customAttributes = propertyInfo
            .GetCustomAttributes()
            .ToArray();

        if (customAttributes.Any())
        {
            var rangeLengthAttribute = customAttributes.FirstOrDefault(x => x.GetType() == typeof(RangeAttribute));
            if (rangeLengthAttribute is not null)
            {
                minimum = (int)((RangeAttribute)rangeLengthAttribute).Minimum;
                maximum = (int)((RangeAttribute)rangeLengthAttribute).Maximum;
            }
        }

        return CreateLabelPixelSizeBox(
            groupIdentifier,
            propertyInfo.Name,
            isReadOnly,
            propertyInfo.HasRequiredAttribute(),
            inputType,
            valueWidth,
            valueHeight,
            minimum,
            maximum);
    }

    public static LabelPixelSizeBox CreateLabelPixelSizeBox(
        string groupIdentifier,
        string labelText,
        bool isReadOnly,
        bool isMandatory,
        Type inputDataType,
        int? valueWidth,
        int? valueHeight,
        int? minimum,
        int? maximum)
    {
        var control = new LabelPixelSizeBox
        {
            GroupIdentifier = groupIdentifier,
            LabelText = labelText.NormalizePascalCase(),
            IsEnabled = !isReadOnly,
            IsMandatory = isMandatory,
            InputDataType = inputDataType,
            ValueWidth = valueWidth ?? 0,
            ValueHeight = valueHeight ?? 0,
            Minimum = minimum ?? 0,
            Maximum = maximum ?? 0,
        };

        if (minimum.HasValue)
        {
            control.Minimum = minimum.Value;
        }

        if (maximum.HasValue)
        {
            control.Maximum = maximum.Value;
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

        uint? minLength = null;
        uint? maxLength = null;
        string? regexPattern = null;

        var customAttributes = propertyInfo
            .GetCustomAttributes()
            .ToArray();

        if (customAttributes.Any())
        {
            var minLengthAttribute = customAttributes.FirstOrDefault(x => x.GetType() == typeof(MinLengthAttribute));
            if (minLengthAttribute is not null)
            {
                minLength = (uint)((MinLengthAttribute)minLengthAttribute).Length;
            }

            var maxLengthAttribute = customAttributes.FirstOrDefault(x => x.GetType() == typeof(MaxLengthAttribute));
            if (maxLengthAttribute is not null)
            {
                maxLength = (uint)((MaxLengthAttribute)maxLengthAttribute).Length;
            }

            var regexAttribute =
                customAttributes.FirstOrDefault(x => x.GetType() == typeof(RegularExpressionAttribute));
            if (regexAttribute is not null)
            {
                regexPattern = ((RegularExpressionAttribute)regexAttribute).Pattern;
            }
        }

        return CreateLabelTextBox(
            groupIdentifier,
            propertyInfo.Name,
            isReadOnly,
            propertyInfo.HasRequiredAttribute(),
            propertyInfo.GetDescription(),
            propertyInfo.PropertyType,
            value,
            minLength,
            maxLength,
            regexPattern);
    }

    public static LabelTextBox CreateLabelTextBox(
        string groupIdentifier,
        string labelText,
        bool isReadOnly,
        bool isMandatory,
        string watermarkText,
        Type inputDataType,
        string? value,
        uint? minLength,
        uint? maxLength,
        string? regexPattern)
    {
        var control = new LabelTextBox
        {
            GroupIdentifier = groupIdentifier,
            LabelText = labelText.NormalizePascalCase(),
            IsEnabled = !isReadOnly,
            IsMandatory = isMandatory,
            InputDataType = inputDataType,
            Text = value ?? string.Empty,
            WatermarkText = watermarkText.NormalizePascalCase(),
        };

        if (minLength.HasValue)
        {
            control.MinLength = minLength.Value;
        }

        if (maxLength.HasValue)
        {
            control.MaxLength = maxLength.Value;
        }

        if (regexPattern is not null)
        {
            control.RegexPattern = regexPattern;
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

        return CreateLabelWellKnownColorSelector(
            groupIdentifier,
            propertyInfo.Name,
            isReadOnly,
            propertyInfo.HasRequiredAttribute() || !propertyInfo.IsNullable(),
            propertyInfo.PropertyType,
            defaultColorName);
    }

    public static LabelWellKnownColorSelector CreateLabelWellKnownColorSelector(
        string groupIdentifier,
        string labelText,
        bool isReadOnly,
        bool isMandatory,
        Type inputDataType,
        string? defaultColorName)
    {
        var control = new LabelWellKnownColorSelector
        {
            GroupIdentifier = groupIdentifier,
            LabelText = labelText.NormalizePascalCase(),
            IsEnabled = !isReadOnly,
            IsMandatory = isMandatory,
            InputDataType = inputDataType,
            DropDownFirstItemType = DropDownFirstItemType.PleaseSelect,
            DefaultColorName = defaultColorName,
        };

        return control;
    }

    public static LabelCountrySelector CreateLabelCountrySelector(
        PropertyInfo propertyInfo,
        string groupIdentifier,
        bool isReadOnly,
        string? defaultCultureIdentifier)
    {
        ArgumentNullException.ThrowIfNull(propertyInfo);

        return CreateLabelCountrySelector(
            groupIdentifier,
            propertyInfo.Name,
            isReadOnly,
            propertyInfo.HasRequiredAttribute() || !propertyInfo.IsNullable(),
            propertyInfo.PropertyType,
            defaultCultureIdentifier);
    }

    public static LabelCountrySelector CreateLabelCountrySelector(
        string groupIdentifier,
        string labelText,
        bool isReadOnly,
        bool isMandatory,
        Type inputDataType,
        string? defaultCultureIdentifier)
    {
        var control = new LabelCountrySelector
        {
            GroupIdentifier = groupIdentifier,
            LabelText = labelText.NormalizePascalCase(),
            IsEnabled = !isReadOnly,
            IsMandatory = isMandatory,
            InputDataType = inputDataType,
            DropDownFirstItemType = DropDownFirstItemType.PleaseSelect,
            DefaultCultureIdentifier = defaultCultureIdentifier,
            UseOnlySupportedCountries = false,
        };

        return control;
    }

    public static LabelLanguageSelector CreateLabelLanguageSelector(
        PropertyInfo propertyInfo,
        string groupIdentifier,
        bool isReadOnly,
        string? defaultCultureIdentifier)
    {
        ArgumentNullException.ThrowIfNull(propertyInfo);

        return CreateLabelLanguageSelector(
            groupIdentifier,
            propertyInfo.Name,
            isReadOnly,
            propertyInfo.HasRequiredAttribute() || !propertyInfo.IsNullable(),
            propertyInfo.PropertyType,
            defaultCultureIdentifier);
    }

    public static LabelLanguageSelector CreateLabelLanguageSelector(
        string groupIdentifier,
        string labelText,
        bool isReadOnly,
        bool isMandatory,
        Type inputDataType,
        string? defaultCultureIdentifier)
    {
        var control = new LabelLanguageSelector
        {
            GroupIdentifier = groupIdentifier,
            LabelText = labelText.NormalizePascalCase(),
            IsEnabled = !isReadOnly,
            IsMandatory = isMandatory,
            InputDataType = inputDataType,
            DropDownFirstItemType = DropDownFirstItemType.PleaseSelect,
            DefaultCultureIdentifier = defaultCultureIdentifier,
            UseOnlySupportedLanguages = false,
            UpdateUiCultureOnChangeEvent = false,
        };

        return control;
    }
}