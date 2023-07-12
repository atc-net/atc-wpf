// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable InvertIf
namespace Atc.Wpf.Controls.LabelControls.Helpers;

[SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
public static class ModelToLabelControlHelper
{
    public static IList<ILabelControlBase> GetLabelControls<T>(
        T model,
        bool includeReadOnly = false)
    {
        ArgumentNullException.ThrowIfNull(model);

        var labelControls = new List<ILabelControlBase>();

        var type = model.GetType();
        var properties = type.GetProperties();
        foreach (var propertyInfo in properties)
        {
            if (propertyInfo.HasIgnoreDisplayAttribute())
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

            if (propertyInfo.PropertyType.IsEnum)
            {
                labelControls.Add(CreateLabelEnumPicker(propertyInfo, model, isReadOnly));
            }
            else if (propertyInfo.PropertyType == typeof(bool) ||
                     propertyInfo.PropertyType == typeof(bool?))
            {
                labelControls.Add(CreateLabelCheckBox(propertyInfo, model, isReadOnly));
            }
            else if (propertyInfo.PropertyType == typeof(decimal) ||
                     propertyInfo.PropertyType == typeof(decimal?))
            {
                labelControls.Add(CreateLabelDecimalBox(propertyInfo, model, isReadOnly));
            }
            else if (propertyInfo.PropertyType == typeof(int) ||
                     propertyInfo.PropertyType == typeof(int?))
            {
                labelControls.Add(CreateLabelIntegerBox(propertyInfo, model, isReadOnly));
            }
            else if (propertyInfo.PropertyType == typeof(string))
            {
                labelControls.Add(CreateLabelTextBox(propertyInfo, model, isReadOnly));
            }
            else if (propertyInfo.PropertyType == typeof(CultureInfo))
            {
                if (propertyInfo.GetName().Contains("Country", StringComparison.OrdinalIgnoreCase))
                {
                    labelControls.Add(CreateLabelCountryPicker(propertyInfo, model, isReadOnly));
                }
                else if (propertyInfo.GetName().Contains("Language", StringComparison.OrdinalIgnoreCase))
                {
                    labelControls.Add(CreateLabelLanguagePicker(propertyInfo, model, isReadOnly));
                }
            }
            else
            {
                Trace.WriteLine($"ModelToLabelControlHelper is not supporting dataType yet: {propertyInfo.PropertyType}");
            }
        }

        return labelControls;
    }

    private static LabelComboBox CreateLabelEnumPicker<T>(
        PropertyInfo propertyInfo,
        T model,
        bool isReadOnly)
    {
        ArgumentNullException.ThrowIfNull(model);

        var control = new LabelComboBox
        {
            LabelText = propertyInfo.Name.NormalizePascalCase(),
            IsEnabled = !isReadOnly,
            IsMandatory = propertyInfo.HasRequiredAttribute(),
            Items = new Dictionary<string, string>(StringComparer.Ordinal),
        };

        const DropDownFirstItemType firstItem = DropDownFirstItemType.PleaseSelect;
        control.Items.Add(
            DropDownFirstItemTypeHelper.GetEnumGuid(firstItem).ToString(),
            firstItem.ToString().NormalizePascalCase());

        foreach (var enumValue in propertyInfo.PropertyType.GetEnumValues())
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
            LabelText = propertyInfo.Name.NormalizePascalCase(),
            IsEnabled = !isReadOnly,
            IsChecked = propertyValue ?? false,
        };

        return control;
    }

    private static LabelDecimalBox CreateLabelDecimalBox<T>(
        PropertyInfo propertyInfo,
        T model,
        bool isReadOnly)
    {
        ArgumentNullException.ThrowIfNull(model);

        decimal? propertyValue = null;
        var propertyObjectValue = GetPropertyValue(model, propertyInfo.Name);
        if (propertyObjectValue is not null &&
            decimal.TryParse(
                propertyObjectValue.ToString(),
                NumberStyles.Any,
                GlobalizationConstants.EnglishCultureInfo,
                out var result))
        {
            propertyValue = result;
        }

        var control = new LabelDecimalBox
        {
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
            control.Minimum = (decimal)((RangeAttribute)rangeLengthAttribute).Minimum;
            control.Maximum = (decimal)((RangeAttribute)rangeLengthAttribute).Maximum;
        }

        return control;
    }

    private static LabelIntegerBox CreateLabelIntegerBox<T>(
        PropertyInfo propertyInfo,
        T model,
        bool isReadOnly)
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

    private static LabelTextBox CreateLabelTextBox<T>(
        PropertyInfo propertyInfo,
        T model,
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

    private static LabelComboBox CreateLabelCountryPicker<T>(
        PropertyInfo propertyInfo,
        T model,
        bool isReadOnly)
    {
        ArgumentNullException.ThrowIfNull(model);

        var control = new LabelComboBox
        {
            LabelText = propertyInfo.Name.NormalizePascalCase(),
            IsEnabled = !isReadOnly,
            IsMandatory = propertyInfo.HasRequiredAttribute(),
            Items = new Dictionary<string, string>(StringComparer.Ordinal),
        };

        const DropDownFirstItemType firstItem = DropDownFirstItemType.PleaseSelect;
        control.Items.Add(
            DropDownFirstItemTypeHelper.GetEnumGuid(firstItem).ToString(),
            firstItem.ToString().NormalizePascalCase());

        var countryNames = CultureHelper.GetCountryNames();
        foreach (var country in countryNames)
        {
            control.Items.Add(country.Key.ToString(GlobalizationConstants.EnglishCultureInfo), country.Value);
        }

        var propertyObjectValue = GetPropertyValue(model, propertyInfo.Name);
        if (propertyObjectValue is not null)
        {
            var cultureInfoName = propertyObjectValue.ToString()!;
            var cultureInfo = new CultureInfo(cultureInfoName);
            control.SelectedKey = cultureInfo.LCID.ToString(GlobalizationConstants.EnglishCultureInfo);
        }

        return control;
    }

    private static LabelComboBox CreateLabelLanguagePicker<T>(
        PropertyInfo propertyInfo,
        T model,
        bool isReadOnly)
    {
        ArgumentNullException.ThrowIfNull(model);

        var control = new LabelComboBox
        {
            LabelText = propertyInfo.Name.NormalizePascalCase(),
            IsEnabled = !isReadOnly,
            IsMandatory = propertyInfo.HasRequiredAttribute(),
            Items = new Dictionary<string, string>(StringComparer.Ordinal),
        };

        const DropDownFirstItemType firstItem = DropDownFirstItemType.PleaseSelect;
        control.Items.Add(
            DropDownFirstItemTypeHelper.GetEnumGuid(firstItem).ToString(),
            firstItem.ToString().NormalizePascalCase());

        var languageNames = CultureHelper.GetLanguageNames();
        foreach (var language in languageNames)
        {
            control.Items.Add(language.Key.ToString(GlobalizationConstants.EnglishCultureInfo), language.Value);
        }

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