// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable LoopCanBeConvertedToQuery
namespace Atc.Wpf.Forms;

/// <summary>
/// A column of label controls in a <see cref="LabelControlsForm"/>.
/// </summary>
public sealed class LabelControlsFormColumn : ILabelControlsFormColumn
{
    private const int LabelControlsHeightForSpace = 20;
    private const int LabelControlsHeightForVertical = 98;
    private const int LabelControlsHeightForHorizontal = 72;

    /// <summary>
    /// Initializes a new instance of the <see cref="LabelControlsFormColumn"/> class.
    /// </summary>
    /// <param name="labelControls">The label controls in the column.</param>
    public LabelControlsFormColumn(IList<ILabelControlBase> labelControls)
    {
        ArgumentNullException.ThrowIfNull(labelControls);

        this.LabelControls = labelControls;
    }

    /// <inheritdoc />
    public bool UseGroupBox { get; set; }

    /// <inheritdoc />
    public Orientation ControlOrientation { get; set; }

    /// <inheritdoc />
    public int ControlWidth { get; set; }

    /// <inheritdoc />
    public IList<ILabelControlBase> LabelControls { get; }

    /// <inheritdoc />
    public void SetSettings(
        bool useGroupBox,
        Orientation controlOrientation,
        int controlWidth)
    {
        UseGroupBox = useGroupBox;
        ControlOrientation = controlOrientation;
        ControlWidth = controlWidth;
        foreach (var labelControl in LabelControls)
        {
            labelControl.Orientation = controlOrientation;
        }
    }

    /// <inheritdoc />
    public bool HasMultiGroupIdentifiers()
        => GetGroupIdentifiers()
            .Skip(1)
            .Any();

    /// <inheritdoc />
    public IList<string?> GetGroupIdentifiers()
        => LabelControls
            .Select(x => x.GroupIdentifier)
            .Distinct(StringComparer.Ordinal)
            .ToList();

    /// <inheritdoc />
    public IList<ILabelControlBase> GetLabelControlsByGroupIdentifier(
        string? groupIdentifier)
        => LabelControls
            .Where(x => x.GroupIdentifier == groupIdentifier)
            .ToList();

    /// <inheritdoc />
    public int CalculateHeight()
        => UseGroupBox
            ? CalculateHeightWithGroupBoxes()
            : CalculateHeightWithoutGroupBoxes();

    /// <inheritdoc />
    public Panel GeneratePanel()
        => UseGroupBox
            ? GeneratePanelWithGroupBoxes()
            : GeneratePanelWithoutGroupBoxes();

    /// <inheritdoc />
    public bool IsValid()
    {
        var isAllValid = true;
        foreach (var labelControl in LabelControls)
        {
            if (!labelControl.IsValid())
            {
                isAllValid = false;
            }
        }

        return isAllValid;
    }

    /// <inheritdoc />
    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
    public Dictionary<string, object> GetKeyValues()
    {
        if (LabelControls is null)
        {
            return new Dictionary<string, object>(StringComparer.Ordinal);
        }

        var result = new Dictionary<string, object>(StringComparer.Ordinal);
        foreach (var control in LabelControls)
        {
            if (!control.IsValid())
            {
                continue;
            }

            switch (control)
            {
                case LabelCheckBox labelCheckBox:
                    result.Add(control.GetFullIdentifier(), labelCheckBox.IsChecked);
                    break;
                case LabelToggleSwitch labelToggleSwitch:
                    result.Add(control.GetFullIdentifier(), labelToggleSwitch.IsOn);
                    break;
                case LabelComboBox labelComboBox:
                    AddComboBoxValue(result, labelComboBox);
                    break;
                case LabelCheckListBox labelCheckListBox:
                    result.Add(control.GetFullIdentifier(), GetSelectedKeysValue(control, labelCheckListBox.SelectedKeys));
                    break;
                case LabelMultiSelectComboBox labelMultiSelectComboBox:
                    result.Add(control.GetFullIdentifier(), GetSelectedKeysValue(control, labelMultiSelectComboBox.SelectedKeys));
                    break;
                case LabelChipGroup labelChipGroup:
                    result.Add(control.GetFullIdentifier(), GetSelectedKeysValue(control, labelChipGroup.SelectedKeys));
                    break;
                case LabelTokenBox labelTokenBox:
                    result.Add(control.GetFullIdentifier(), (labelTokenBox.SelectedKeys ?? []).ToList());
                    break;
                case LabelTreeSelectComboBox labelTreeSelectComboBox:
                    result.Add(control.GetFullIdentifier(), (labelTreeSelectComboBox.SelectedKeys ?? []).ToList());
                    break;
                case LabelCascaderComboBox labelCascaderComboBox:
                    result.Add(control.GetFullIdentifier(), (labelCascaderComboBox.SelectedKeys ?? []).ToList());
                    break;
                case LabelMultiColumnComboBox labelMultiColumnComboBox:
                    result.Add(control.GetFullIdentifier(), (labelMultiColumnComboBox.SelectedKeys ?? []).ToList());
                    break;
                case LabelDecimalBox labelDecimalBox:
                    AddNumberValue(result, control, labelDecimalBox.Value, labelDecimalBox.Value);
                    break;
                case LabelDecimalXyBox labelDecimalXyBox:
                    if (labelDecimalXyBox.InputDataType == typeof(Point2D) ||
                        labelDecimalXyBox.InputDataType == typeof(Point2D?))
                    {
                        result.Add(
                            control.GetFullIdentifier(),
                            new Point2D(
                                (double)labelDecimalXyBox.ValueX,
                                (double)labelDecimalXyBox.ValueY));
                    }

                    break;
                case LabelIntegerBox labelIntegerBox:
                    AddNumberValue(result, control, labelIntegerBox.Value, labelIntegerBox.Value);
                    break;
                case LabelPixelSizeBox labelPixelSizeBox:
                    result.Add(
                        control.GetFullIdentifier(),
                        new Size(
                            labelPixelSizeBox.ValueWidth,
                            labelPixelSizeBox.ValueHeight));
                    break;
                case LabelSlider labelSlider:
                    result.Add(control.GetFullIdentifier(), labelSlider.Value);
                    break;
                case LabelTextBox labelTextBox:
                    result.Add(control.GetFullIdentifier(), labelTextBox.Text);
                    break;
                case LabelDateTimePicker labelDateTimePicker:
                    AddDateValue(result, control, labelDateTimePicker.SelectedDate);
                    break;
                case LabelDatePicker labelDatePicker:
                    AddDateValue(result, control, labelDatePicker.SelectedDate);
                    break;
                case LabelTimePicker labelTimePicker:
                    AddDateValue(result, control, labelTimePicker.SelectedTime);
                    break;
                case LabelDirectoryPicker labelDirectoryPicker:
                    if (labelDirectoryPicker.Value is not null)
                    {
                        result.Add(control.GetFullIdentifier(), labelDirectoryPicker.Value);
                    }

                    break;
                case LabelFilePicker labelFilePicker:
                    if (labelFilePicker.Value is not null)
                    {
                        result.Add(control.GetFullIdentifier(), labelFilePicker.Value);
                    }

                    break;
                case LabelCountrySelector labelCountrySelector:
                    if (NumberHelper.IsInt(labelCountrySelector.GetKey()))
                    {
                        result.Add(control.GetFullIdentifier(), new CultureInfo(NumberHelper.ParseToInt(labelCountrySelector.GetKey())));
                    }

                    break;
                case LabelLanguageSelector labelLanguageSelector:
                    if (NumberHelper.IsInt(labelLanguageSelector.GetKey()))
                    {
                        result.Add(control.GetFullIdentifier(), new CultureInfo(NumberHelper.ParseToInt(labelLanguageSelector.GetKey())));
                    }

                    break;
                case LabelWellKnownColorSelector labelWellKnownColorSelector:
                    var color = ColorHelper.GetColorFromName(
                        labelWellKnownColorSelector.SelectedKey,
                        GlobalizationConstants.EnglishCultureInfo);
                    if (color is not null)
                    {
                        result.Add(control.GetFullIdentifier(), (Color)color);
                    }

                    break;
                case LabelContent labelContent:
                    if (labelContent.Content is FrameworkElement { DataContext: ViewModelBase } labelContentFrameworkElement)
                    {
                        var tag = labelContent.Tag?.ToString();
                        if (!string.IsNullOrEmpty(tag))
                        {
                            result.Add(tag, labelContentFrameworkElement.DataContext);
                        }
                    }

                    break;
            }
        }

        return result;
    }

    /// <summary>
    /// Adds the selected key of a combo box. For an enum property (see <see cref="ILabelControlBase.InputDataType"/>)
    /// an empty or placeholder selection gives <see langword="null"/> for a nullable property and no value at all for
    /// a non-nullable one, so the model keeps its current value.
    /// </summary>
    private static void AddComboBoxValue(
        Dictionary<string, object> result,
        LabelComboBox labelComboBox)
    {
        var inputDataType = labelComboBox.InputDataType;
        var nonNullableType = inputDataType is null
            ? null
            : Nullable.GetUnderlyingType(inputDataType) ?? inputDataType;
        var selectedKey = labelComboBox.SelectedKey;

        if (nonNullableType is { IsEnum: true } &&
            (string.IsNullOrWhiteSpace(selectedKey) ||
             Guid.TryParse(selectedKey, out _)))
        {
            if (nonNullableType != inputDataType)
            {
                result.Add(labelComboBox.GetFullIdentifier(), null!);
            }

            return;
        }

        result.Add(labelComboBox.GetFullIdentifier(), selectedKey);
    }

    /// <summary>
    /// Adds the value of a number box. For an integral property (see <see cref="ILabelControlBase.InputDataType"/>)
    /// the value is written as a whole number, and a value outside the type's range is left out, so the model
    /// keeps its current value. Any other property gets <paramref name="controlValue"/> as is.
    /// </summary>
    private static void AddNumberValue(
        Dictionary<string, object> result,
        ILabelControlBase control,
        decimal value,
        object controlValue)
    {
        if (!IntegralTypeHelper.TryGetBounds(control.InputDataType, out _, out _))
        {
            result.Add(control.GetFullIdentifier(), controlValue);
            return;
        }

        if (IntegralTypeHelper.TryGetWholeNumber(value, control.InputDataType, out var integralValue))
        {
            result.Add(control.GetFullIdentifier(), integralValue);
        }
    }

    /// <summary>
    /// Adds the value of a date or time picker, converted to the type of the edited property
    /// (see <see cref="ILabelControlBase.InputDataType"/>): <see cref="DateOnly"/>, <see cref="TimeOnly"/>,
    /// <see cref="DateTimeOffset"/> or <see cref="DateTime"/>. An empty picker gives <see langword="null"/> for a
    /// nullable property and no value at all for a non-nullable one, so the model keeps its current value.
    /// </summary>
    private static void AddDateValue(
        Dictionary<string, object> result,
        ILabelControlBase control,
        DateTime? selectedDate)
    {
        var inputDataType = control.InputDataType;
        if (selectedDate is null)
        {
            if (inputDataType is null ||
                !inputDataType.IsValueType ||
                Nullable.GetUnderlyingType(inputDataType) is not null)
            {
                result.Add(control.GetFullIdentifier(), null!);
            }

            return;
        }

        var value = selectedDate.Value;
        var nonNullableType = inputDataType is null
            ? typeof(DateTime)
            : Nullable.GetUnderlyingType(inputDataType) ?? inputDataType;

        object convertedValue;
        if (nonNullableType == typeof(DateOnly))
        {
            convertedValue = DateOnly.FromDateTime(value);
        }
        else if (nonNullableType == typeof(TimeOnly))
        {
            convertedValue = TimeOnly.FromDateTime(value);
        }
        else if (nonNullableType == typeof(DateTimeOffset))
        {
            convertedValue = ToDateTimeOffset(value);
        }
        else
        {
            convertedValue = value;
        }

        result.Add(control.GetFullIdentifier(), convertedValue);
    }

    /// <summary>
    /// Converts a picker's local clock time to a <see cref="DateTimeOffset"/>, clamping at the ends of the range
    /// where the local offset would move the instant outside <see cref="DateTimeOffset.MinValue"/>/<see cref="DateTimeOffset.MaxValue"/>.
    /// </summary>
    private static DateTimeOffset ToDateTimeOffset(DateTime value)
    {
        var localValue = DateTime.SpecifyKind(value, DateTimeKind.Local);
        var offset = TimeZoneInfo.Local.GetUtcOffset(localValue);
        var utcTicks = localValue.Ticks - offset.Ticks;
        if (utcTicks < DateTimeOffset.MinValue.UtcTicks)
        {
            return DateTimeOffset.MinValue;
        }

        if (utcTicks > DateTimeOffset.MaxValue.UtcTicks)
        {
            return DateTimeOffset.MaxValue;
        }

        return new DateTimeOffset(DateTime.SpecifyKind(value, DateTimeKind.Unspecified), offset);
    }

    /// <summary>
    /// Gets the form value of a multi-select control: the combined flags when the control edits a <c>[Flags]</c> enum
    /// (see <see cref="ILabelControlBase.InputDataType"/>), otherwise the list of selected keys.
    /// </summary>
    private static object GetSelectedKeysValue(
        ILabelControlBase control,
        IEnumerable<string>? selectedKeys)
        => MultiSelectPropertyHelper.IsFlagsEnum(control.InputDataType)
            ? MultiSelectPropertyHelper.CombineFlags(control.InputDataType!, selectedKeys)
            : (selectedKeys ?? []).ToList();

    /// <summary>
    /// Calculates the column height when the controls are wrapped in group boxes, using the tallest group.
    /// </summary>
    /// <returns>The calculated height.</returns>
    public int CalculateHeightWithGroupBoxes()
    {
        var maxHeight = 0;
        foreach (var groupIdentifier in GetGroupIdentifiers())
        {
            var labelControls = LabelControls
                .Where(x => x.GroupIdentifier == groupIdentifier)
                .ToList();

            var heightForGroupIdentifiers = 0;
            foreach (var labelControl in labelControls)
            {
                if (labelControl.Orientation == Orientation.Vertical)
                {
                    heightForGroupIdentifiers += LabelControlsHeightForVertical;
                }
                else
                {
                    heightForGroupIdentifiers += LabelControlsHeightForHorizontal;
                }
            }

            if (labelControls.Count > 1)
            {
                maxHeight -= (labelControls.Count - 1) * LabelControlsHeightForSpace;
            }

            if (heightForGroupIdentifiers > maxHeight)
            {
                maxHeight = heightForGroupIdentifiers;
            }
        }

        return maxHeight;
    }

    /// <summary>
    /// Calculates the column height when the controls are not wrapped in group boxes.
    /// </summary>
    /// <returns>The calculated height.</returns>
    public int CalculateHeightWithoutGroupBoxes()
    {
        var maxHeight = 0;
        foreach (var labelControl in LabelControls)
        {
            if (labelControl.Orientation == Orientation.Vertical)
            {
                maxHeight += LabelControlsHeightForVertical;
            }
            else
            {
                maxHeight += LabelControlsHeightForHorizontal;
            }
        }

        if (LabelControls.Count > 1)
        {
            maxHeight -= (LabelControls.Count - 1) * LabelControlsHeightForSpace;
        }

        return maxHeight;
    }

    private Panel GeneratePanelWithGroupBoxes()
    {
        var stackPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
        };

        foreach (var groupIdentifier in GetGroupIdentifiers())
        {
            var stackPanelItems = new StackPanel
            {
                Orientation = Orientation.Vertical,
            };

            var groupBox = new GroupBox
            {
                Header = groupIdentifier,
                Margin = new Thickness(
                    5,
                    5,
                    5,
                    5),
            };

            var labelControls = LabelControls
                .Where(x => x.GroupIdentifier == groupIdentifier)
                .ToList();

            foreach (var labelControl in labelControls)
            {
                labelControl.Orientation = ControlOrientation;
                labelControl.Width = ControlWidth;
                stackPanelItems.Children.Add((UIElement)labelControl);
            }

            groupBox.Content = stackPanelItems;

            stackPanel.Children.Add(groupBox);
        }

        return stackPanel;
    }

    private Panel GeneratePanelWithoutGroupBoxes()
    {
        var stackPanel = new StackPanel
        {
            Orientation = Orientation.Vertical,
        };

        foreach (var labelControl in LabelControls)
        {
            labelControl.Orientation = ControlOrientation;
            labelControl.Width = ControlWidth;
            stackPanel.Children.Add((UIElement)labelControl);
        }

        return stackPanel;
    }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(UseGroupBox)}: {UseGroupBox}, {nameof(ControlOrientation)}: {ControlOrientation}, {nameof(ControlWidth)}: {ControlWidth}, {nameof(LabelControls)}.Count: {LabelControls.Count}";
}