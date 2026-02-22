// ReSharper disable SuggestBaseTypeForParameter
namespace Atc.Wpf.Controls.Sample;

public partial class SamplePropertyController
{
    private readonly Dictionary<string, (PropertyInfo Property, FrameworkElement Editor)> editorMap = new(StringComparer.Ordinal);
    private bool suppressEditorUpdates;

    [DependencyProperty(PropertyChangedCallback = nameof(OnSourceObjectChanged))]
    private object? sourceObject;

    public SamplePropertyController()
    {
        InitializeComponent();
    }

    private static void OnSourceObjectChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (SamplePropertyController)d;

        if (e.OldValue is INotifyPropertyChanged oldInpc)
        {
            oldInpc.PropertyChanged -= control.OnSourcePropertyChanged;
        }

        control.RebuildPanel();

        if (e.NewValue is INotifyPropertyChanged newInpc)
        {
            newInpc.PropertyChanged += control.OnSourcePropertyChanged;
        }
    }

    private void OnSourcePropertyChanged(
        object? sender,
        PropertyChangedEventArgs e)
    {
        if (suppressEditorUpdates ||
            e.PropertyName is null ||
            !editorMap.TryGetValue(e.PropertyName, out var entry))
        {
            return;
        }

        suppressEditorUpdates = true;
        try
        {
            var value = entry.Property.GetValue(SourceObject);
            UpdateEditorValue(entry.Editor, value);
        }
        finally
        {
            suppressEditorUpdates = false;
        }
    }

    private void RebuildPanel()
    {
        ContentPanel.Children.Clear();
        editorMap.Clear();

        if (SourceObject is null)
        {
            return;
        }

        var groups = PropertyMetadataDescriptor.GetGroupedDescriptors(SourceObject.GetType());

        foreach (var (groupName, properties) in groups)
        {
            var expander = new Expander
            {
                Header = groupName,
                IsExpanded = true,
                Margin = new Thickness(0, 0, 0, 4),
            };

            var panel = new StackPanel();

            foreach (var info in properties)
            {
                var propInfo = SourceObject.GetType().GetProperty(info.PropertyName);
                if (propInfo is null)
                {
                    continue;
                }

                panel.Children.Add(CreatePropertyRow(info, propInfo));
            }

            expander.Content = panel;
            ContentPanel.Children.Add(expander);
        }
    }

    private UIElement CreatePropertyRow(
        PropertyMetadataInfo info,
        PropertyInfo propInfo)
    {
        var grid = new Grid { Margin = new Thickness(0, 2, 0, 2) };
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });

        var label = new TextBlock
        {
            Text = info.DisplayName,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(4, 0, 8, 0),
        };

        if (!string.IsNullOrEmpty(info.Description))
        {
            label.ToolTip = info.Description;
        }

        Grid.SetColumn(label, 0);
        grid.Children.Add(label);

        var editor = CreateEditor(info, propInfo);
        Grid.SetColumn(editor, 1);
        grid.Children.Add(editor);

        editorMap[info.PropertyName] = (propInfo, editor);

        return grid;
    }

    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK - editor type dispatch.")]
    private FrameworkElement CreateEditor(
        PropertyMetadataInfo info,
        PropertyInfo propInfo)
    {
        var currentValue = propInfo.GetValue(SourceObject);

        if (info.IsReadOnly || info.EditorHint == EditorHint.ReadOnly)
        {
            return CreateReadOnlyEditor(currentValue);
        }

        if (info.EditorHint == EditorHint.Slider && info.HasRange)
        {
            return CreateSliderEditor(info, propInfo, currentValue);
        }

        if (info.EditorHint == EditorHint.ColorPicker || UnwrapNullable(info.PropertyType) == typeof(Color))
        {
            return CreateColorEditor(propInfo, currentValue);
        }

        var type = UnwrapNullable(info.PropertyType);

        if (type == typeof(bool))
        {
            return CreateBoolEditor(propInfo, currentValue);
        }

        if (type.IsEnum)
        {
            return CreateEnumEditor(type, propInfo, currentValue);
        }

        if (IsIntegerType(type))
        {
            return CreateIntegerEditor(info, propInfo, currentValue);
        }

        if (IsDecimalType(type))
        {
            return CreateDecimalEditor(info, propInfo, currentValue);
        }

        if (type == typeof(string))
        {
            return info.EditorHint == EditorHint.MultiLineText
                ? CreateMultiLineTextEditor(propInfo, currentValue)
                : CreateTextEditor(propInfo, currentValue);
        }

        if (type == typeof(Thickness))
        {
            return CreateThicknessEditor(info, propInfo, currentValue);
        }

        if (type == typeof(TimeSpan))
        {
            return CreateTimeSpanEditor(propInfo, currentValue);
        }

        return CreateFallbackEditor(currentValue);
    }

    private static FrameworkElement CreateReadOnlyEditor(object? currentValue)
        => new TextBox
        {
            Text = currentValue?.ToString() ?? string.Empty,
            IsReadOnly = true,
            VerticalAlignment = VerticalAlignment.Center,
        };

    private FrameworkElement CreateBoolEditor(
        PropertyInfo propInfo,
        object? currentValue)
    {
        var checkBox = new CheckBox
        {
            IsChecked = currentValue is true,
            VerticalAlignment = VerticalAlignment.Center,
        };

        checkBox.Checked += (_, _) => SetSourceValue(propInfo, true);
        checkBox.Unchecked += (_, _) => SetSourceValue(propInfo, false);

        return checkBox;
    }

    private FrameworkElement CreateIntegerEditor(
        PropertyMetadataInfo info,
        PropertyInfo propInfo,
        object? currentValue)
    {
        var intBox = new IntegerBox
        {
            Value = currentValue is not null
                ? Convert.ToDouble(currentValue, CultureInfo.InvariantCulture)
                : 0,
            HideUpDownButtons = false,
            VerticalAlignment = VerticalAlignment.Center,
        };

        if (info.HasRange)
        {
            intBox.Minimum = info.RangeMinimum!.Value;
            intBox.Maximum = info.RangeMaximum!.Value;
        }

        if (info.RangeStep.HasValue)
        {
            intBox.Interval = info.RangeStep.Value;
        }

        intBox.ValueChanged += (_, e) =>
        {
            if (e.NewValue.HasValue)
            {
                SetNumericSourceValue(propInfo, e.NewValue.Value);
            }
        };

        return intBox;
    }

    private FrameworkElement CreateDecimalEditor(
        PropertyMetadataInfo info,
        PropertyInfo propInfo,
        object? currentValue)
    {
        var decBox = new DecimalBox
        {
            Value = currentValue is not null
                ? Convert.ToDouble(currentValue, CultureInfo.InvariantCulture)
                : 0,
            HideUpDownButtons = false,
            VerticalAlignment = VerticalAlignment.Center,
        };

        if (info.HasRange)
        {
            decBox.Minimum = info.RangeMinimum!.Value;
            decBox.Maximum = info.RangeMaximum!.Value;
        }

        if (info.RangeStep.HasValue)
        {
            decBox.Interval = info.RangeStep.Value;
        }

        decBox.ValueChanged += (_, e) =>
        {
            if (e.NewValue.HasValue)
            {
                SetNumericSourceValue(propInfo, e.NewValue.Value);
            }
        };

        return decBox;
    }

    private FrameworkElement CreateSliderEditor(
        PropertyMetadataInfo info,
        PropertyInfo propInfo,
        object? currentValue)
    {
        var slider = new Slider
        {
            Minimum = info.RangeMinimum!.Value,
            Maximum = info.RangeMaximum!.Value,
            Value = currentValue is not null
                ? Convert.ToDouble(currentValue, CultureInfo.InvariantCulture)
                : info.RangeMinimum!.Value,
            AutoToolTipPlacement = AutoToolTipPlacement.TopLeft,
            VerticalAlignment = VerticalAlignment.Center,
        };

        if (info.RangeStep.HasValue)
        {
            slider.SmallChange = info.RangeStep.Value;
            slider.LargeChange = info.RangeStep.Value * 10;
            slider.TickFrequency = info.RangeStep.Value;
            slider.IsSnapToTickEnabled = true;
        }

        slider.ValueChanged += (_, e) => SetNumericSourceValue(propInfo, e.NewValue);

        return slider;
    }

    private FrameworkElement CreateEnumEditor(
        Type enumType,
        PropertyInfo propInfo,
        object? currentValue)
    {
        var comboBox = new ComboBox
        {
            ItemsSource = Enum.GetValues(enumType),
            SelectedItem = currentValue,
            VerticalAlignment = VerticalAlignment.Center,
        };

        comboBox.SelectionChanged += (_, _) =>
        {
            if (comboBox.SelectedItem is not null)
            {
                SetSourceValue(propInfo, comboBox.SelectedItem);
            }
        };

        return comboBox;
    }

    private FrameworkElement CreateTextEditor(
        PropertyInfo propInfo,
        object? currentValue)
    {
        var textBox = new TextBox
        {
            Text = currentValue as string ?? string.Empty,
            VerticalAlignment = VerticalAlignment.Center,
        };

        textBox.TextChanged += (_, _) => SetSourceValue(propInfo, textBox.Text);

        return textBox;
    }

    private FrameworkElement CreateMultiLineTextEditor(
        PropertyInfo propInfo,
        object? currentValue)
    {
        var textBox = new TextBox
        {
            Text = currentValue as string ?? string.Empty,
            AcceptsReturn = true,
            TextWrapping = TextWrapping.Wrap,
            MinHeight = 60,
            MaxHeight = 120,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
        };

        textBox.TextChanged += (_, _) => SetSourceValue(propInfo, textBox.Text);

        return textBox;
    }

    private FrameworkElement CreateColorEditor(
        PropertyInfo propInfo,
        object? currentValue)
    {
        var picker = new WellKnownColorPicker
        {
            ShowOnlyBasicColors = false,
            VerticalAlignment = VerticalAlignment.Center,
        };

        if (currentValue is Color color)
        {
            picker.ColorBrush = new SolidColorBrush(color);
        }

        picker.ColorChanged += (_, e) => SetSourceValue(propInfo, e.NewValue);

        return picker;
    }

    private FrameworkElement CreateThicknessEditor(
        PropertyMetadataInfo info,
        PropertyInfo propInfo,
        object? currentValue)
    {
        var thicknessBox = new ThicknessBox
        {
            HideUpDownButtons = false,
            VerticalAlignment = VerticalAlignment.Center,
        };

        if (currentValue is Thickness thickness)
        {
            thicknessBox.Value = thickness;
        }

        if (info.HasRange)
        {
            thicknessBox.Minimum = info.RangeMinimum!.Value;
            thicknessBox.Maximum = info.RangeMaximum!.Value;
        }

        thicknessBox.ValueChanged += (_, _) => SetSourceValue(propInfo, thicknessBox.Value);

        return thicknessBox;
    }

    private FrameworkElement CreateTimeSpanEditor(
        PropertyInfo propInfo,
        object? currentValue)
    {
        var textBox = new TextBox
        {
            Text = currentValue is TimeSpan ts
                ? ts.ToString("c", CultureInfo.InvariantCulture)
                : string.Empty,
            VerticalAlignment = VerticalAlignment.Center,
        };

        textBox.LostFocus += (_, _) =>
        {
            if (TimeSpan.TryParse(textBox.Text, CultureInfo.InvariantCulture, out var parsed))
            {
                SetSourceValue(propInfo, parsed);
            }
        };

        return textBox;
    }

    private static FrameworkElement CreateFallbackEditor(object? currentValue)
        => new TextBlock
        {
            Text = currentValue?.ToString() ?? string.Empty,
            FontStyle = FontStyles.Italic,
            Foreground = Brushes.Gray,
            VerticalAlignment = VerticalAlignment.Center,
        };

    private static void UpdateEditorValue(
        FrameworkElement editor,
        object? value)
    {
        switch (editor)
        {
            case CheckBox cb:
                cb.IsChecked = value is true;
                break;
            case NumericBox nb:
                nb.Value = value is not null
                    ? Convert.ToDouble(value, CultureInfo.InvariantCulture)
                    : null;
                break;
            case Slider sl:
                sl.Value = value is not null
                    ? Convert.ToDouble(value, CultureInfo.InvariantCulture)
                    : 0;
                break;
            case ComboBox combo:
                combo.SelectedItem = value;
                break;
            case WellKnownColorPicker cp:
                cp.ColorBrush = value is Color c
                    ? new SolidColorBrush(c)
                    : Brushes.Transparent;
                break;
            case ThicknessBox tb:
                tb.Value = value is Thickness t ? t : default;
                break;
            case TextBox textBox:
                textBox.Text = value switch
                {
                    TimeSpan ts => ts.ToString("c", CultureInfo.InvariantCulture),
                    null => string.Empty,
                    _ => value.ToString() ?? string.Empty,
                };
                break;
        }
    }

    private void SetSourceValue(
        PropertyInfo propInfo,
        object? value)
    {
        if (suppressEditorUpdates)
        {
            return;
        }

        suppressEditorUpdates = true;
        try
        {
            propInfo.SetValue(SourceObject, value);
        }
        finally
        {
            suppressEditorUpdates = false;
        }
    }

    private void SetNumericSourceValue(
        PropertyInfo propInfo,
        double value)
    {
        if (suppressEditorUpdates)
        {
            return;
        }

        suppressEditorUpdates = true;
        try
        {
            var targetType = UnwrapNullable(propInfo.PropertyType);
            var converted = Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
            propInfo.SetValue(SourceObject, converted);
        }
        finally
        {
            suppressEditorUpdates = false;
        }
    }

    private static Type UnwrapNullable(Type type)
        => Nullable.GetUnderlyingType(type) ?? type;

    private static bool IsIntegerType(Type type)
    {
        var t = UnwrapNullable(type);
        return t == typeof(int)
            || t == typeof(long)
            || t == typeof(short)
            || t == typeof(byte)
            || t == typeof(sbyte)
            || t == typeof(ushort)
            || t == typeof(uint)
            || t == typeof(ulong);
    }

    private static bool IsDecimalType(Type type)
    {
        var t = UnwrapNullable(type);
        return t == typeof(double)
            || t == typeof(float)
            || t == typeof(decimal);
    }
}