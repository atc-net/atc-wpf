// ReSharper disable InvertIf
// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
namespace Atc.Wpf.Controls.LabelControls.Extractors;

[SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
public static class ModelToLabelControlExtractor
{
    public static IList<ILabelControlBase> Extract<T>(
        [DisallowNull] T model,
        bool includeReadOnly = true,
        string groupIdentifier = "")
        => ExtractType(
            model,
            includeReadOnly,
            groupIdentifier,
            string.Empty);

    private static IList<ILabelControlBase> ExtractType<T>(
        [DisallowNull] T model,
        bool includeReadOnly,
        string groupIdentifier,
        string parentGroupIdentifier)
    {
        ArgumentNullException.ThrowIfNull(model);

        var labelControls = new List<ILabelControlBase>();

        var type = model.GetType();
        var typeGroupIdentifier = GetTypeGroupIdentifier(model, groupIdentifier, parentGroupIdentifier);

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

            if (Extract(model, propertyInfo, labelControls, typeGroupIdentifier, isReadOnly))
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

            if (!propertyInfo.PropertyType.IsPrimitive &&
                ExtractComplexType(model, includeReadOnly, propertyInfo, labelControls, typeGroupIdentifier))
            {
                continue;
            }

            Trace.WriteLine($"ModelToLabelControlHelper is not supporting dataType yet: {propertyInfo.PropertyType}");
        }

        return labelControls;
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK - errorHandler will handle it")]
    private static bool ExtractComplexType<T>(
        [DisallowNull] T model,
        bool includeReadOnly,
        PropertyInfo propertyInfo,
        List<ILabelControlBase> labelControls,
        string typeGroupIdentifier)
    {
        var subModel = propertyInfo.GetValue(model);
        if (subModel is not null)
        {
            labelControls.AddRange(
                ExtractType(
                    subModel,
                    includeReadOnly,
                    propertyInfo.Name,
                    typeGroupIdentifier));

            return true;
        }

        var nonNullableType = propertyInfo.PropertyType.GetNonNullableType();
        if (nonNullableType is not null)
        {
            object? newSubModel = null;
            try
            {
                newSubModel = Activator.CreateInstance(nonNullableType);
            }
            catch
            {
                // Skip
            }

            if (newSubModel is not null)
            {
                labelControls.AddRange(
                    ExtractType(
                        newSubModel,
                        includeReadOnly,
                        propertyInfo.Name,
                        typeGroupIdentifier));

                return true;
            }
        }

        return false;
    }

    private static bool Extract<T>(
        [DisallowNull] T model,
        PropertyInfo propertyInfo,
        ICollection<ILabelControlBase> labelControls,
        string typeGroupIdentifier,
        bool isReadOnly)
    {
        var nonNullableType = propertyInfo.PropertyType.GetNonNullableType();
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

        if (propertyInfo.PropertyType == typeof(DateTime) ||
            propertyInfo.PropertyType == typeof(DateTime?) ||
            propertyInfo.PropertyType == typeof(DateTimeOffset) ||
            propertyInfo.PropertyType == typeof(DateTimeOffset?))
        {
            labelControls.Add(CreateLabelDateTimePicker(propertyInfo, model, typeGroupIdentifier, isReadOnly));
            return true;
        }

        if (propertyInfo.PropertyType == typeof(DateOnly) ||
            propertyInfo.PropertyType == typeof(DateOnly?))
        {
            labelControls.Add(CreateLabelDatePicker(propertyInfo, model, typeGroupIdentifier, isReadOnly));
            return true;
        }

        if (propertyInfo.PropertyType == typeof(TimeOnly) ||
            propertyInfo.PropertyType == typeof(TimeOnly?))
        {
            labelControls.Add(CreateLabelTimePicker(propertyInfo, model, typeGroupIdentifier, isReadOnly));
            return true;
        }

        if (propertyInfo.PropertyType == typeof(DirectoryInfo))
        {
            labelControls.Add(CreateLabelDirectoryPicker(propertyInfo, model, typeGroupIdentifier, isReadOnly));
            return true;
        }

        if (propertyInfo.PropertyType == typeof(FileInfo))
        {
            labelControls.Add(CreateLabelFilePicker(propertyInfo, model, typeGroupIdentifier, isReadOnly));
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

    private static object? GetPropertyValue(
        object target,
        string fieldName)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(fieldName);

        return GetPropertyInfo(target, fieldName)?.GetValue(target);
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

    private static LabelComboBox CreateLabelEnumPicker<T>(
        PropertyInfo propertyInfo,
        T model,
        string groupIdentifier,
        bool isReadOnly)
    {
        ArgumentNullException.ThrowIfNull(model);

        string? selectedKey = null;
        var propertyObjectValue = GetPropertyValue(model, propertyInfo.Name);
        if (propertyObjectValue is not null)
        {
            selectedKey = propertyObjectValue.ToString()!;
        }

        return LabelControlFactory.CreateLabelEnumPicker(
            propertyInfo,
            groupIdentifier,
            isReadOnly,
            selectedKey);
    }

    private static LabelCheckBox CreateLabelCheckBox<T>(
        PropertyInfo propertyInfo,
        T model,
        string groupIdentifier,
        bool isReadOnly)
    {
        ArgumentNullException.ThrowIfNull(model);

        bool? value = null;
        var propertyObjectValue = GetPropertyValue(model, propertyInfo.Name);
        if (propertyObjectValue is not null &&
            bool.TryParse(
                propertyObjectValue.ToString(),
                out var result))
        {
            value = result;
        }

        return LabelControlFactory.CreateLabelCheckBox(
            propertyInfo,
            groupIdentifier,
            isReadOnly,
            value);
    }

    private static LabelDecimalBox CreateLabelDecimalBox<T>(
        PropertyInfo propertyInfo,
        T model,
        string groupIdentifier,
        bool isReadOnly,
        Type inputType)
    {
        ArgumentNullException.ThrowIfNull(model);

        var propertyObjectValue = GetPropertyValue(model, propertyInfo.Name);
        var value = propertyObjectValue switch
        {
            decimal dec => dec,
            double dub => (decimal)dub,
            float flo => (decimal)flo,
            _ => 0,
        };

        return LabelControlFactory.CreateLabelDecimalBox(
            propertyInfo,
            groupIdentifier,
            isReadOnly,
            inputType,
            value);
    }

    private static LabelDecimalXyBox CreateLabelDecimalXyBox<T>(
        PropertyInfo propertyInfo,
        T model,
        string groupIdentifier,
        bool isReadOnly,
        Type inputType)
    {
        ArgumentNullException.ThrowIfNull(model);

        decimal valueX = 0;
        decimal valueY = 0;
        var propertyObjectValue = GetPropertyValue(model, propertyInfo.Name);
        switch (propertyObjectValue)
        {
            case Size size:
                valueX = (decimal)size.Width;
                valueY = (decimal)size.Height;
                break;
            case Point2D point2D:
                valueX = (decimal)point2D.X;
                valueY = (decimal)point2D.Y;
                break;
        }

        return LabelControlFactory.CreateLabelDecimalXyBox(
            propertyInfo,
            groupIdentifier,
            isReadOnly,
            inputType,
            valueX,
            valueY);
    }

    private static LabelIntegerBox CreateLabelIntegerBox<T>(
        PropertyInfo propertyInfo,
        T model,
        string groupIdentifier,
        bool isReadOnly,
        Type inputType)
    {
        ArgumentNullException.ThrowIfNull(model);

        int? value = null;
        var propertyObjectValue = GetPropertyValue(model, propertyInfo.Name);
        if (propertyObjectValue is not null &&
            int.TryParse(
                propertyObjectValue.ToString(),
                NumberStyles.Any,
                GlobalizationConstants.EnglishCultureInfo,
                out var result))
        {
            value = result;
        }

        return LabelControlFactory.CreateLabelIntegerBox(
            propertyInfo,
            groupIdentifier,
            isReadOnly,
            inputType,
            value);
    }

    private static LabelPixelSizeBox CreateLabelPixelSizeBox<T>(
        PropertyInfo propertyInfo,
        T model,
        string groupIdentifier,
        bool isReadOnly,
        Type inputType)
    {
        ArgumentNullException.ThrowIfNull(model);

        var valueWidth = 0;
        var valueHeight = 0;
        var propertyObjectValue = GetPropertyValue(model, propertyInfo.Name);
        switch (propertyObjectValue)
        {
            case Size size:
                valueWidth = (int)size.Width;
                valueHeight = (int)size.Height;
                break;
        }

        return LabelControlFactory.CreateLabelPixelSizeBox(
            propertyInfo,
            groupIdentifier,
            isReadOnly,
            inputType,
            valueWidth,
            valueHeight);
    }

    private static LabelTextBox CreateLabelTextBox<T>(
        PropertyInfo propertyInfo,
        T model,
        string groupIdentifier,
        bool isReadOnly)
    {
        ArgumentNullException.ThrowIfNull(model);

        var value = string.Empty;
        var propertyObjectValue = GetPropertyValue(model, propertyInfo.Name);
        if (propertyObjectValue is not null)
        {
            value = propertyObjectValue.ToString()!;
        }

        return LabelControlFactory.CreateLabelTextBox(
            propertyInfo,
            groupIdentifier,
            isReadOnly,
            value);
    }

    private static LabelDateTimePicker CreateLabelDateTimePicker<T>(
        PropertyInfo propertyInfo,
        T model,
        string groupIdentifier,
        bool isReadOnly)
    {
        ArgumentNullException.ThrowIfNull(model);

        DateTime? value = null;
        var propertyObjectValue = GetPropertyValue(model, propertyInfo.Name);
        if (propertyObjectValue is not null)
        {
            value = propertyObjectValue as DateTime?;
        }

        return LabelControlFactory.CreateLabelDateTimePicker(
            propertyInfo,
            groupIdentifier,
            isReadOnly,
            value);
    }

    private static LabelDatePicker CreateLabelDatePicker<T>(
        PropertyInfo propertyInfo,
        T model,
        string groupIdentifier,
        bool isReadOnly)
    {
        ArgumentNullException.ThrowIfNull(model);

        DateOnly? value = null;
        var propertyObjectValue = GetPropertyValue(model, propertyInfo.Name);
        if (propertyObjectValue is not null)
        {
            value = propertyObjectValue as DateOnly?;
        }

        return LabelControlFactory.CreateLabelDatePicker(
            propertyInfo,
            groupIdentifier,
            isReadOnly,
            value);
    }

    private static LabelTimePicker CreateLabelTimePicker<T>(
        PropertyInfo propertyInfo,
        T model,
        string groupIdentifier,
        bool isReadOnly)
    {
        ArgumentNullException.ThrowIfNull(model);

        TimeOnly? value = null;
        var propertyObjectValue = GetPropertyValue(model, propertyInfo.Name);
        if (propertyObjectValue is not null)
        {
            value = propertyObjectValue as TimeOnly?;
        }

        return LabelControlFactory.CreateLabelTimePicker(
            propertyInfo,
            groupIdentifier,
            isReadOnly,
            value);
    }

    private static LabelDirectoryPicker CreateLabelDirectoryPicker<T>(
        PropertyInfo propertyInfo,
        T model,
        string groupIdentifier,
        bool isReadOnly)
    {
        ArgumentNullException.ThrowIfNull(model);

        DirectoryInfo? value = null;
        var propertyObjectValue = GetPropertyValue(model, propertyInfo.Name);
        if (propertyObjectValue is not null)
        {
            value = propertyObjectValue as DirectoryInfo;
        }

        return LabelControlFactory.CreateLabelDirectoryPicker(
            propertyInfo,
            groupIdentifier,
            isReadOnly,
            value);
    }

    private static LabelFilePicker CreateLabelFilePicker<T>(
        PropertyInfo propertyInfo,
        T model,
        string groupIdentifier,
        bool isReadOnly)
    {
        ArgumentNullException.ThrowIfNull(model);

        FileInfo? value = null;
        var propertyObjectValue = GetPropertyValue(model, propertyInfo.Name);
        if (propertyObjectValue is not null)
        {
            value = propertyObjectValue as FileInfo;
        }

        return LabelControlFactory.CreateLabelFilePicker(
            propertyInfo,
            groupIdentifier,
            isReadOnly,
            value);
    }

    private static LabelWellKnownColorSelector CreateLabelColorSelector<T>(
        PropertyInfo propertyInfo,
        T model,
        string groupIdentifier,
        bool isReadOnly)
    {
        ArgumentNullException.ThrowIfNull(model);

        string? defaultColorName = null;
        var propertyObjectValue = GetPropertyValue(model, propertyInfo.Name);
        if (propertyObjectValue is not null)
        {
            var colorCode = propertyObjectValue.ToString()!;
            if ("#00000000".Equals(colorCode, StringComparison.Ordinal))
            {
                defaultColorName = ((int)DropDownFirstItemType.PleaseSelect).ToString(GlobalizationConstants.EnglishCultureInfo);
            }
            else
            {
                var colorName = ColorHelper.GetColorNameFromHex(colorCode);
                defaultColorName = colorName ?? ((int)DropDownFirstItemType.PleaseSelect).ToString(GlobalizationConstants.EnglishCultureInfo);
            }
        }

        return LabelControlFactory.CreateLabelWellKnownColorSelector(
            propertyInfo,
            groupIdentifier,
            isReadOnly,
            defaultColorName);
    }

    private static LabelCountrySelector CreateLabelCountrySelector<T>(
        PropertyInfo propertyInfo,
        T model,
        string groupIdentifier,
        bool isReadOnly)
    {
        ArgumentNullException.ThrowIfNull(model);

        string? selectedKey = null;
        var propertyObjectValue = GetPropertyValue(model, propertyInfo.Name);
        if (propertyObjectValue is not null)
        {
            var cultureInfoName = propertyObjectValue.ToString()!;
            var cultureInfo = new CultureInfo(cultureInfoName);
            selectedKey = cultureInfo.LCID.ToString(GlobalizationConstants.EnglishCultureInfo);
        }

        return LabelControlFactory.CreateLabelCountrySelector(
            propertyInfo,
            groupIdentifier,
            isReadOnly,
            selectedKey);
    }

    private static LabelLanguageSelector CreateLabelLanguageSelector<T>(
        PropertyInfo propertyInfo,
        T model,
        string groupIdentifier,
        bool isReadOnly)
    {
        ArgumentNullException.ThrowIfNull(model);

        string? selectedKey = null;
        var propertyObjectValue = GetPropertyValue(model, propertyInfo.Name);
        if (propertyObjectValue is not null)
        {
            var cultureInfoName = propertyObjectValue.ToString()!;
            var cultureInfo = new CultureInfo(cultureInfoName);
            selectedKey = cultureInfo.LCID.ToString(GlobalizationConstants.EnglishCultureInfo);
        }

        return LabelControlFactory.CreateLabelLanguageSelector(
            propertyInfo,
            groupIdentifier,
            isReadOnly,
            selectedKey);
    }
}