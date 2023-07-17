// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable InvertIf
namespace Atc.Wpf.Controls.LabelControls.Helpers;

[SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
public static class ModelToLabelControlHelper
{
    public static IList<ILabelControlBase> GetLabelControls<T>(
        T model,
        bool includeReadOnly = true,
        string groupIdentifier = "")
        => ExtractLabelControls(
            model,
            includeReadOnly,
            groupIdentifier,
            string.Empty);

    private static IList<ILabelControlBase> ExtractLabelControls<T>(
        T model,
        bool includeReadOnly,
        string groupIdentifier,
        string parentGroupIdentifier)
    {
        ArgumentNullException.ThrowIfNull(model);

        var labelControls = new List<ILabelControlBase>();

        var type = model.GetType();
        var typeGroupIdentifier = GetTypeGroupIdentifier<T>(model, groupIdentifier, parentGroupIdentifier);

        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var propertyInfo in properties)
        {
            if (propertyInfo.GetMethod == null ||
                propertyInfo.HasIgnoreDisplayAttribute())
            {
                continue;
            }

            var isReadOnly = false;
            if (includeReadOnly)
            {
                if (propertyInfo.GetSetMethod() == null)
                {
                    isReadOnly = true;
                }
            }
            else
            {
                if (propertyInfo.GetSetMethod() == null)
                {
                    continue;
                }
            }

            if (ExtractLabelControl(model, propertyInfo, labelControls, typeGroupIdentifier, isReadOnly))
            {
                continue;
            }

            if (propertyInfo.PropertyType == typeof(byte?) ||
                propertyInfo.PropertyType == typeof(sbyte?) ||
                propertyInfo.PropertyType == typeof(char?) ||
                propertyInfo.PropertyType == typeof(DateTime) ||
                propertyInfo.PropertyType == typeof(DateTime?) ||
                propertyInfo.PropertyType == typeof(DateOnly) ||
                propertyInfo.PropertyType == typeof(DateOnly?) ||
                propertyInfo.PropertyType == typeof(TimeOnly) ||
                propertyInfo.PropertyType == typeof(TimeOnly?) ||
                propertyInfo.PropertyType == typeof(DateTimeOffset) ||
                propertyInfo.PropertyType == typeof(DateTimeOffset?))
            {
                Trace.WriteLine($"ModelToLabelControlHelper is not supporting dataType yet: {propertyInfo.PropertyType}");
                continue;
            }

            if (!propertyInfo.PropertyType.IsPrimitive)
            {
                var subModel = propertyInfo.GetValue(model);
                if (subModel is not null)
                {
                    labelControls.AddRange(
                        ExtractLabelControls(
                            subModel,
                            includeReadOnly,
                            propertyInfo.Name,
                            typeGroupIdentifier));

                    continue;
                }
            }

            Trace.WriteLine($"ModelToLabelControlHelper is not supporting dataType yet: {propertyInfo.PropertyType}");
        }

        return labelControls;
    }

    private static bool ExtractLabelControl<T>(
        [DisallowNull] T model,
        PropertyInfo propertyInfo,
        List<ILabelControlBase> labelControls,
        string typeGroupIdentifier,
        bool isReadOnly)
    {
        var nonNullableType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
        if (nonNullableType.IsEnum)
        {
            labelControls.Add(CreateLabelEnumPicker(propertyInfo, model, typeGroupIdentifier, isReadOnly));
            return true;
        }

        if (propertyInfo.PropertyType == typeof(bool) ||
            propertyInfo.PropertyType == typeof(bool?))
        {
            labelControls.Add(CreateLabelCheckBox(propertyInfo, model, typeGroupIdentifier, isReadOnly));
            return true;
        }

        if (propertyInfo.PropertyType == typeof(decimal) ||
            propertyInfo.PropertyType == typeof(decimal?) ||
            propertyInfo.PropertyType == typeof(double) ||
            propertyInfo.PropertyType == typeof(double?) ||
            propertyInfo.PropertyType == typeof(float) ||
            propertyInfo.PropertyType == typeof(float?))
        {
            labelControls.Add(CreateLabelDecimalBox(propertyInfo, model, typeGroupIdentifier, isReadOnly, propertyInfo.PropertyType));
            return true;
        }

        if (propertyInfo.PropertyType == typeof(Point2D) ||
            propertyInfo.PropertyType == typeof(Point2D?))
        {
            labelControls.Add(CreateLabelDecimalXyBox(propertyInfo, model, typeGroupIdentifier, isReadOnly, propertyInfo.PropertyType));
            return true;
        }

        if (propertyInfo.PropertyType == typeof(int) ||
            propertyInfo.PropertyType == typeof(int?) ||
            propertyInfo.PropertyType == typeof(uint) ||
            propertyInfo.PropertyType == typeof(uint?) ||
            propertyInfo.PropertyType == typeof(long) ||
            propertyInfo.PropertyType == typeof(long?) ||
            propertyInfo.PropertyType == typeof(ulong) ||
            propertyInfo.PropertyType == typeof(ulong?) ||
            propertyInfo.PropertyType == typeof(short) ||
            propertyInfo.PropertyType == typeof(short?) ||
            propertyInfo.PropertyType == typeof(ushort) ||
            propertyInfo.PropertyType == typeof(ushort?))
        {
            labelControls.Add(CreateLabelIntegerBox(propertyInfo, model, typeGroupIdentifier, isReadOnly, propertyInfo.PropertyType));
            return true;
        }

        if (propertyInfo.PropertyType == typeof(Size) ||
            propertyInfo.PropertyType == typeof(Size?))
        {
            labelControls.Add(CreateLabelPixelSizeBox(propertyInfo, model, typeGroupIdentifier, isReadOnly, propertyInfo.PropertyType));
            return true;
        }

        if (propertyInfo.PropertyType == typeof(string) ||
            propertyInfo.PropertyType == typeof(Guid) ||
            propertyInfo.PropertyType == typeof(Guid?))
        {
            labelControls.Add(CreateLabelTextBox(propertyInfo, model, typeGroupIdentifier, isReadOnly));
            return true;
        }

        if (propertyInfo.PropertyType == typeof(Color) ||
            propertyInfo.PropertyType == typeof(Color?))
        {
            labelControls.Add(CreateLabelColorSelector(propertyInfo, model, typeGroupIdentifier, isReadOnly));
            return true;
        }

        if (propertyInfo.PropertyType == typeof(CultureInfo))
        {
            if (propertyInfo.GetName().Contains("Country", StringComparison.OrdinalIgnoreCase))
            {
                labelControls.Add(CreateLabelCountrySelector(propertyInfo, model, typeGroupIdentifier, isReadOnly));
                return true;
            }

            if (propertyInfo.GetName().Contains("Language", StringComparison.OrdinalIgnoreCase))
            {
                labelControls.Add(CreateLabelLanguageSelector(propertyInfo, model, typeGroupIdentifier, isReadOnly));
                return true;
            }
        }

        return false;
    }

    private static string GetTypeGroupIdentifier<T>(
        T model,
        string groupIdentifier,
        string parentGroupIdentifier)
    {
        ArgumentNullException.ThrowIfNull(model);

        var type = model.GetType();

        if (string.IsNullOrEmpty(parentGroupIdentifier))
        {
            return string.IsNullOrEmpty(groupIdentifier)
                ? type.Name
                : groupIdentifier;
        }

        return string.IsNullOrEmpty(groupIdentifier)
            ? $"{parentGroupIdentifier}.{type.Name}"
            : $"{parentGroupIdentifier}.{groupIdentifier}";
    }

    private static LabelComboBox CreateLabelEnumPicker<T>(
        PropertyInfo propertyInfo,
        T model,
        string groupIdentifier,
        bool isReadOnly)
    {
        ArgumentNullException.ThrowIfNull(model);

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

        var propertyObjectValue = GetPropertyValue(model, propertyInfo.Name);
        if (propertyObjectValue is not null)
        {
            control.SelectedKey = propertyObjectValue.ToString()!;
        }

        return control;
    }

    private static LabelCheckBox CreateLabelCheckBox<T>(
        PropertyInfo propertyInfo,
        T model,
        string groupIdentifier,
        bool isReadOnly)
    {
        ArgumentNullException.ThrowIfNull(model);

        bool? propertyValue = null;
        var propertyObjectValue = GetPropertyValue(model, propertyInfo.Name);
        if (propertyObjectValue is not null &&
            bool.TryParse(
                propertyObjectValue.ToString(),
                out var result))
        {
            propertyValue = result;
        }

        var control = new LabelCheckBox
        {
            GroupIdentifier = groupIdentifier,
            LabelText = propertyInfo.Name.NormalizePascalCase(),
            IsEnabled = !isReadOnly,
            IsChecked = propertyValue ?? false,
        };

        return control;
    }

    private static LabelDecimalBox CreateLabelDecimalBox<T>(
        PropertyInfo propertyInfo,
        T model,
        string groupIdentifier,
        bool isReadOnly,
        Type inputType)
    {
        ArgumentNullException.ThrowIfNull(model);

        var control = new LabelDecimalBox
        {
            GroupIdentifier = groupIdentifier,
            InputDataType = inputType,
            LabelText = propertyInfo.Name.NormalizePascalCase(),
            IsEnabled = !isReadOnly,
            IsMandatory = propertyInfo.HasRequiredAttribute(),
            WatermarkText = propertyInfo.GetDescription().NormalizePascalCase(),
        };

        var propertyObjectValue = GetPropertyValue(model, propertyInfo.Name);
        control.Value = propertyObjectValue switch
        {
            decimal dec => dec,
            double dub => (decimal)dub,
            float flo => (decimal)flo,
            _ => 0,
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

    private static LabelDecimalXyBox CreateLabelDecimalXyBox<T>(
        PropertyInfo propertyInfo,
        T model,
        string groupIdentifier,
        bool isReadOnly,
        Type inputType)
    {
        ArgumentNullException.ThrowIfNull(model);

        var control = new LabelDecimalXyBox
        {
            GroupIdentifier = groupIdentifier,
            InputDataType = inputType,
            LabelText = propertyInfo.Name.NormalizePascalCase(),
            IsEnabled = !isReadOnly,
            IsMandatory = propertyInfo.HasRequiredAttribute(),
        };

        var propertyObjectValue = GetPropertyValue(model, propertyInfo.Name);
        switch (propertyObjectValue)
        {
            case Size size:
                control.ValueX = (decimal)size.Width;
                control.ValueY = (decimal)size.Height;
                break;
            case Point2D point2D:
                control.ValueX = (decimal)point2D.X;
                control.ValueY = (decimal)point2D.Y;
                break;
        }

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

    private static LabelIntegerBox CreateLabelIntegerBox<T>(
        PropertyInfo propertyInfo,
        T model,
        string groupIdentifier,
        bool isReadOnly,
        Type inputType)
    {
        ArgumentNullException.ThrowIfNull(model);

        int? propertyValue = null;
        var propertyObjectValue = GetPropertyValue(model, propertyInfo.Name);
        if (propertyObjectValue is not null &&
            int.TryParse(
                propertyObjectValue.ToString(),
                NumberStyles.Any,
                GlobalizationConstants.EnglishCultureInfo,
                out var result))
        {
            propertyValue = result;
        }

        var control = new LabelIntegerBox
        {
            GroupIdentifier = groupIdentifier,
            InputDataType = inputType,
            LabelText = propertyInfo.Name.NormalizePascalCase(),
            IsEnabled = !isReadOnly,
            IsMandatory = propertyInfo.HasRequiredAttribute(),
            Value = propertyValue ?? 0,
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

    private static LabelPixelSizeBox CreateLabelPixelSizeBox<T>(
        PropertyInfo propertyInfo,
        T model,
        string groupIdentifier,
        bool isReadOnly,
        Type inputType)
    {
        ArgumentNullException.ThrowIfNull(model);

        var control = new LabelPixelSizeBox
        {
            GroupIdentifier = groupIdentifier,
            InputDataType = inputType,
            LabelText = propertyInfo.Name.NormalizePascalCase(),
            IsEnabled = !isReadOnly,
            IsMandatory = propertyInfo.HasRequiredAttribute(),
        };

        var propertyObjectValue = GetPropertyValue(model, propertyInfo.Name);
        switch (propertyObjectValue)
        {
            case Size size:
                control.ValueWidth = (int)size.Width;
                control.ValueHeight = (int)size.Height;
                break;
        }

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

    private static LabelTextBox CreateLabelTextBox<T>(
        PropertyInfo propertyInfo,
        T model,
        string groupIdentifier,
        bool isReadOnly)
    {
        ArgumentNullException.ThrowIfNull(model);

        var propertyValue = string.Empty;
        var propertyObjectValue = GetPropertyValue(model, propertyInfo.Name);
        if (propertyObjectValue is not null)
        {
            propertyValue = propertyObjectValue.ToString()!;
        }

        var control = new LabelTextBox
        {
            GroupIdentifier = groupIdentifier,
            LabelText = propertyInfo.Name.NormalizePascalCase(),
            IsEnabled = !isReadOnly,
            IsMandatory = propertyInfo.HasRequiredAttribute(),
            Text = propertyValue,
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

    private static LabelWellKnownColorSelector CreateLabelColorSelector<T>(
        PropertyInfo propertyInfo,
        T model,
        string groupIdentifier,
        bool isReadOnly)
    {
        ArgumentNullException.ThrowIfNull(model);

        var control = new LabelWellKnownColorSelector
        {
            GroupIdentifier = groupIdentifier,
            LabelText = propertyInfo.Name.NormalizePascalCase(),
            IsEnabled = !isReadOnly,
            DropDownFirstItemType = DropDownFirstItemType.PleaseSelect,
            IsMandatory = propertyInfo.HasRequiredAttribute() || !propertyInfo.IsNullable(),
        };

        var propertyObjectValue = GetPropertyValue(model, propertyInfo.Name);
        if (propertyObjectValue is not null)
        {
            var colorCode = propertyObjectValue.ToString()!;
            if ("#00000000".Equals(colorCode, StringComparison.Ordinal))
            {
                control.DefaultColorName = ((int)DropDownFirstItemType.PleaseSelect).ToString(GlobalizationConstants.EnglishCultureInfo);
            }
            else
            {
                var colorName = ColorUtil.GetColorNameFromHex(colorCode);
                control.DefaultColorName = colorName ?? ((int)DropDownFirstItemType.PleaseSelect).ToString(GlobalizationConstants.EnglishCultureInfo);
            }
        }

        return control;
    }

    private static LabelCountrySelector CreateLabelCountrySelector<T>(
        PropertyInfo propertyInfo,
        T model,
        string groupIdentifier,
        bool isReadOnly)
    {
        ArgumentNullException.ThrowIfNull(model);

        var control = new LabelCountrySelector
        {
            GroupIdentifier = groupIdentifier,
            LabelText = propertyInfo.Name.NormalizePascalCase(),
            IsEnabled = !isReadOnly,
            DropDownFirstItemType = DropDownFirstItemType.PleaseSelect,
            UseOnlySupportedCountries = false,
            IsMandatory = propertyInfo.HasRequiredAttribute() || !propertyInfo.IsNullable(),
        };

        var propertyObjectValue = GetPropertyValue(model, propertyInfo.Name);
        if (propertyObjectValue is not null)
        {
            var cultureInfoName = propertyObjectValue.ToString()!;
            var cultureInfo = new CultureInfo(cultureInfoName);
            control.SelectedKey = cultureInfo.LCID.ToString(GlobalizationConstants.EnglishCultureInfo);
        }

        return control;
    }

    private static LabelLanguageSelector CreateLabelLanguageSelector<T>(
        PropertyInfo propertyInfo,
        T model,
        string groupIdentifier,
        bool isReadOnly)
    {
        ArgumentNullException.ThrowIfNull(model);

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

        var propertyObjectValue = GetPropertyValue(model, propertyInfo.Name);
        if (propertyObjectValue is not null)
        {
            var cultureInfoName = propertyObjectValue.ToString()!;
            var cultureInfo = new CultureInfo(cultureInfoName);
            control.SelectedKey = cultureInfo.LCID.ToString(GlobalizationConstants.EnglishCultureInfo);
        }

        return control;
    }

    private static object? GetPropertyValue(
        object target,
        string fieldName)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(fieldName);

        var propertyInfo = GetPropertyInfo(target, fieldName);
        return propertyInfo == null
            ? null
            : propertyInfo.GetValue(target);
    }

    private static PropertyInfo? GetPropertyInfo(
        object target,
        string fieldName)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(fieldName);

        var type = target.GetType();
        var properties = type.GetProperties();
        return properties.SingleOrDefault(x => x.Name == fieldName);
    }
}