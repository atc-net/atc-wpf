// ReSharper disable SuggestBaseTypeForParameter
namespace Atc.Wpf.Controls.Sample;

public partial class SamplePropertyController
{
    private readonly Dictionary<string, (PropertyInfo Property, FrameworkElement Editor)> editorMap = new(StringComparer.Ordinal);
    private string? suppressedPropertyName;

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
        if (e.PropertyName is null ||
            e.PropertyName == suppressedPropertyName ||
            !editorMap.TryGetValue(e.PropertyName, out var entry))
        {
            return;
        }

        var previous = suppressedPropertyName;
        suppressedPropertyName = e.PropertyName;
        try
        {
            var value = entry.Property.GetValue(SourceObject);
            UpdateEditorValue(entry.Editor, value);
        }
        finally
        {
            suppressedPropertyName = previous;
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
        var editor = CreateEditor(info, propInfo);

        // Command buttons span the full width — no label needed.
        if (typeof(ICommand).IsAssignableFrom(propInfo.PropertyType))
        {
            var container = new Grid { Margin = new Thickness(0, 2, 0, 2) };
            container.Children.Add(editor);
            editorMap[info.PropertyName] = (propInfo, editor);
            return container;
        }

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

        // Commands must be checked before read-only, since get-only ICommand properties are marked IsReadOnly.
        if (typeof(ICommand).IsAssignableFrom(propInfo.PropertyType))
        {
            return CreateCommandEditor(info, propInfo);
        }

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
        => new TextBlock
        {
            Text = currentValue?.ToString() ?? string.Empty,
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
        var selector = new WellKnownColorSelector
        {
            VerticalAlignment = VerticalAlignment.Center,
        };

        if (currentValue is Color color)
        {
            var colorName = ColorHelper.GetColorNameFromColor(color);
            if (colorName is not null)
            {
                selector.SelectedKey = colorName;
            }
        }

        selector.SelectorChanged += (_, e) =>
        {
            if (e.NewValue is not null)
            {
                var newColor = ColorHelper.GetColorFromName(e.NewValue);
                if (newColor.HasValue)
                {
                    SetSourceValue(propInfo, newColor.Value);
                }
            }
        };

        return selector;
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

    private FrameworkElement CreateCommandEditor(
        PropertyMetadataInfo info,
        PropertyInfo propInfo)
    {
        var button = new Button
        {
            Content = info.DisplayName,
            Padding = new Thickness(8, 4, 8, 4),
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };

        button.Click += (_, _) =>
        {
            var command = propInfo.GetValue(SourceObject) as ICommand;
            if (command?.CanExecute(parameter: null) == true)
            {
                command.Execute(parameter: null);
            }
        };

        return button;
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
            case WellKnownColorSelector cs:
                if (value is Color c)
                {
                    var name = ColorHelper.GetColorNameFromColor(c);
                    if (name is not null)
                    {
                        cs.SelectedKey = name;
                    }
                }

                break;
            case ThicknessBox tb:
                tb.Value = value is Thickness t ? t : default;
                break;
            case TextBlock textBlock:
                textBlock.Text = value?.ToString() ?? string.Empty;
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
        if (propInfo.Name == suppressedPropertyName)
        {
            return;
        }

        var previous = suppressedPropertyName;
        suppressedPropertyName = propInfo.Name;
        try
        {
            propInfo.SetValue(SourceObject, value);
        }
        finally
        {
            suppressedPropertyName = previous;
        }
    }

    private void SetNumericSourceValue(
        PropertyInfo propInfo,
        double value)
    {
        if (propInfo.Name == suppressedPropertyName)
        {
            return;
        }

        var previous = suppressedPropertyName;
        suppressedPropertyName = propInfo.Name;
        try
        {
            var targetType = UnwrapNullable(propInfo.PropertyType);
            var converted = Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
            propInfo.SetValue(SourceObject, converted);
        }
        finally
        {
            suppressedPropertyName = previous;
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