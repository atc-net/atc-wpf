// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable LoopCanBeConvertedToQuery
namespace Atc.Wpf.Controls.LabelControls;

public class LabelControlsFormColumn : ILabelControlsFormColumn
{
    private const int LabelControlsHeightForSpace = 20;
    private const int LabelControlsHeightForVertical = 98;
    private const int LabelControlsHeightForHorizontal = 72;

    public LabelControlsFormColumn(
        IList<ILabelControlBase> labelControls)
    {
        ArgumentNullException.ThrowIfNull(labelControls);

        this.LabelControls = labelControls;
    }

    public bool UseGroupBox { get; set; }

    public Orientation ControlOrientation { get; set; }

    public int ControlWidth { get; set; }

    public IList<ILabelControlBase> LabelControls { get; }

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

    public bool HasMultiGroupIdentifiers()
        => GetGroupIdentifiers()
            .Skip(1)
            .Any();

    public IList<string?> GetGroupIdentifiers()
        => LabelControls
            .Select(x => x.GroupIdentifier)
            .Distinct(StringComparer.Ordinal)
            .ToList();

    public IList<ILabelControlBase> GetLabelControlsByGroupIdentifier(
        string? groupIdentifier)
        => LabelControls
            .Where(x => x.GroupIdentifier == groupIdentifier)
            .ToList();

    public int CalculateHeight()
        => UseGroupBox
            ? CalculateHeightWithGroupBoxes()
            : CalculateHeightWithoutGroupBoxes();

    public Panel GeneratePanel()
        => UseGroupBox
            ? GeneratePanelWithGroupBoxes()
            : GeneratePanelWithoutGroupBoxes();

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
                    result.Add(control.GetFullIdentifier(), labelComboBox.SelectedKey);
                    break;
                case LabelDecimalBox labelDecimalBox:
                    result.Add(control.GetFullIdentifier(), labelDecimalBox.Value);
                    break;
                case LabelDecimalXyBox labelDecimalXyBox:
                    if (labelDecimalXyBox.InputDataType == typeof(Point2D) ||
                        labelDecimalXyBox.InputDataType == typeof(Point2D?))
                    {
                        result.Add(control.GetFullIdentifier(), new Point2D(
                            (double)labelDecimalXyBox.ValueX,
                            (double)labelDecimalXyBox.ValueY));
                    }

                    break;
                case LabelIntegerBox labelIntegerBox:
                    result.Add(control.GetFullIdentifier(), labelIntegerBox.Value);
                    break;
                case LabelPixelSizeBox labelPixelSizeBox:
                    result.Add(control.GetFullIdentifier(), new Size(labelPixelSizeBox.ValueWidth, labelPixelSizeBox.ValueHeight));
                    break;
                case LabelSlider labelSlider:
                    result.Add(control.GetFullIdentifier(), labelSlider.Value);
                    break;
                case LabelTextBox labelTextBox:
                    result.Add(control.GetFullIdentifier(), labelTextBox.Text);
                    break;
                case LabelDateTimePicker labelDateTimePicker:
                    result.Add(control.GetFullIdentifier(), labelDateTimePicker.SelectedDate ?? DateTime.MinValue);
                    break;
                case LabelDatePicker labelDatePicker:
                    result.Add(control.GetFullIdentifier(), labelDatePicker.SelectedDate ?? DateTime.MinValue);
                    break;
                case LabelTimePicker labelTimePicker:
                    result.Add(control.GetFullIdentifier(), labelTimePicker.SelectedTime ?? DateTime.MinValue);
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
                    var color = ColorHelper.GetColorFromName(labelWellKnownColorSelector.SelectedKey, GlobalizationConstants.EnglishCultureInfo);
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
                Margin = new Thickness(5, 5, 5, 5),
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

    public override string ToString()
        => $"{nameof(UseGroupBox)}: {UseGroupBox}, {nameof(ControlOrientation)}: {ControlOrientation}, {nameof(ControlWidth)}: {ControlWidth}, {nameof(LabelControls)}.Count: {LabelControls.Count}";
}