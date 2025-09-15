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
            firstItem.GetDescription());

        var nonNullableType = inputDataType.GetNonNullableType();
        foreach (Enum enumValue in nonNullableType.GetEnumValues())
        {
            var key = enumValue.ToString();
            if (key.Equals("None", StringComparison.Ordinal) ||
                key.Equals("Unknown", StringComparison.Ordinal) ||
                key.Equals("Default", StringComparison.Ordinal))
            {
                continue;
            }

            control.Items.Add(key, enumValue.GetDescription().NormalizePascalCase());
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

    public static LabelDateTimePicker CreateLabelDateTimePicker(
        PropertyInfo propertyInfo,
        string groupIdentifier,
        bool isReadOnly,
        DateTime? value)
    {
        ArgumentNullException.ThrowIfNull(propertyInfo);

        return CreateLabelDateTimePicker(
            groupIdentifier,
            propertyInfo.Name,
            isReadOnly,
            propertyInfo.HasRequiredAttribute(),
            propertyInfo.GetDescription(),
            propertyInfo.PropertyType,
            value);
    }

    public static LabelDateTimePicker CreateLabelDateTimePicker(
        string groupIdentifier,
        string labelText,
        bool isReadOnly,
        bool isMandatory,
        string watermarkText,
        Type inputDataType,
        DateTime? value,
        DateTime? minDateTime = null,
        DateTime? maxDateTime = null)
    {
        var control = new LabelDateTimePicker
        {
            GroupIdentifier = groupIdentifier,
            LabelText = labelText.NormalizePascalCase(),
            IsEnabled = !isReadOnly,
            IsMandatory = isMandatory,
            InputDataType = inputDataType,
            SelectedDate = value,
            WatermarkDateText = watermarkText.NormalizePascalCase(),
            WatermarkTimeText = string.Empty,
        };

        if (minDateTime.HasValue)
        {
            control.DisplayDateStart = minDateTime;
        }

        if (maxDateTime.HasValue)
        {
            control.DisplayDateEnd = maxDateTime;
        }

        return control;
    }

    public static LabelDatePicker CreateLabelDatePicker(
        PropertyInfo propertyInfo,
        string groupIdentifier,
        bool isReadOnly,
        DateOnly? value)
    {
        ArgumentNullException.ThrowIfNull(propertyInfo);

        return CreateLabelDatePicker(
            groupIdentifier,
            propertyInfo.Name,
            isReadOnly,
            propertyInfo.HasRequiredAttribute(),
            propertyInfo.GetDescription(),
            propertyInfo.PropertyType,
            value);
    }

    public static LabelDatePicker CreateLabelDatePicker(
        string groupIdentifier,
        string labelText,
        bool isReadOnly,
        bool isMandatory,
        string watermarkText,
        Type inputDataType,
        DateOnly? value,
        DateOnly? minDate = null,
        DateOnly? maxDate = null)
    {
        var control = new LabelDatePicker
        {
            GroupIdentifier = groupIdentifier,
            LabelText = labelText.NormalizePascalCase(),
            IsEnabled = !isReadOnly,
            IsMandatory = isMandatory,
            InputDataType = inputDataType,
            SelectedDate = value is null
                ? null
                : new DateTime(
                    value.Value.Year,
                    value.Value.Month,
                    value.Value.Day,
                    0,
                    0,
                    0,
                    DateTimeKind.Unspecified),
            WatermarkText = watermarkText.NormalizePascalCase(),
        };

        if (minDate.HasValue)
        {
            control.DisplayDateStart = new DateTime(
                minDate.Value.Year,
                minDate.Value.Month,
                minDate.Value.Day,
                0,
                0,
                0,
                DateTimeKind.Unspecified);
        }

        if (maxDate.HasValue)
        {
            control.DisplayDateEnd = new DateTime(
                maxDate.Value.Year,
                maxDate.Value.Month,
                maxDate.Value.Day,
                0,
                0,
                0,
                DateTimeKind.Unspecified);
        }

        return control;
    }

    public static LabelTimePicker CreateLabelTimePicker(
        PropertyInfo propertyInfo,
        string groupIdentifier,
        bool isReadOnly,
        TimeOnly? value)
    {
        ArgumentNullException.ThrowIfNull(propertyInfo);

        return CreateLabelTimePicker(
            groupIdentifier,
            propertyInfo.Name,
            isReadOnly,
            propertyInfo.HasRequiredAttribute(),
            propertyInfo.GetDescription(),
            propertyInfo.PropertyType,
            value);
    }

    public static LabelTimePicker CreateLabelTimePicker(
        string groupIdentifier,
        string labelText,
        bool isReadOnly,
        bool isMandatory,
        string watermarkText,
        Type inputDataType,
        TimeOnly? value)
    {
        var control = new LabelTimePicker
        {
            GroupIdentifier = groupIdentifier,
            LabelText = labelText.NormalizePascalCase(),
            IsEnabled = !isReadOnly,
            IsMandatory = isMandatory,
            InputDataType = inputDataType,
            SelectedTime = value is null
                ? null
                : new DateTime(
                    DateTime.MinValue.Year,
                    DateTime.MinValue.Month,
                    DateTime.MinValue.Day,
                    value.Value.Hour,
                    value.Value.Minute,
                    value.Value.Second,
                    DateTimeKind.Unspecified),
            WatermarkText = watermarkText.NormalizePascalCase(),
        };

        return control;
    }

    public static LabelDirectoryPicker CreateLabelDirectoryPicker(
        PropertyInfo propertyInfo,
        string groupIdentifier,
        bool isReadOnly,
        DirectoryInfo? value)
    {
        ArgumentNullException.ThrowIfNull(propertyInfo);

        return CreateLabelDirectoryPicker(
            groupIdentifier,
            propertyInfo.Name,
            isReadOnly,
            propertyInfo.HasRequiredAttribute(),
            propertyInfo.GetDescription(),
            propertyInfo.PropertyType,
            value);
    }

    public static LabelDirectoryPicker CreateLabelDirectoryPicker(
        string groupIdentifier,
        string labelText,
        bool isReadOnly,
        bool isMandatory,
        string watermarkText,
        Type inputDataType,
        DirectoryInfo? value)
    {
        var control = new LabelDirectoryPicker
        {
            GroupIdentifier = groupIdentifier,
            LabelText = labelText.NormalizePascalCase(),
            IsEnabled = !isReadOnly,
            IsMandatory = isMandatory,
            InputDataType = inputDataType,
            Value = value,
            WatermarkText = watermarkText.NormalizePascalCase(),
        };

        return control;
    }

    public static LabelFilePicker CreateLabelFilePicker(
        PropertyInfo propertyInfo,
        string groupIdentifier,
        bool isReadOnly,
        FileInfo? value)
    {
        ArgumentNullException.ThrowIfNull(propertyInfo);

        return CreateLabelFilePicker(
            groupIdentifier,
            propertyInfo.Name,
            isReadOnly,
            propertyInfo.HasRequiredAttribute(),
            propertyInfo.GetDescription(),
            propertyInfo.PropertyType,
            value);
    }

    public static LabelFilePicker CreateLabelFilePicker(
        string groupIdentifier,
        string labelText,
        bool isReadOnly,
        bool isMandatory,
        string watermarkText,
        Type inputDataType,
        FileInfo? value)
    {
        var control = new LabelFilePicker
        {
            GroupIdentifier = groupIdentifier,
            LabelText = labelText.NormalizePascalCase(),
            IsEnabled = !isReadOnly,
            IsMandatory = isMandatory,
            InputDataType = inputDataType,
            Value = value,
            WatermarkText = watermarkText.NormalizePascalCase(),
        };

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
            DefaultColorName = defaultColorName ?? string.Empty,
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
            DefaultCultureIdentifier = defaultCultureIdentifier ?? string.Empty,
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
            DefaultCultureIdentifier = defaultCultureIdentifier ?? string.Empty,
            UseOnlySupportedLanguages = false,
            UpdateUiCultureOnChangeEvent = false,
        };

        return control;
    }
}