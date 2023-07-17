// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable LoopCanBeConvertedToQuery
namespace Atc.Wpf.Controls.LabelControls;

public class LabelControlsFormColumn : ILabelControlsFormColumn
{
    private const int GroupBoxMarginHeight = 20;
    private const int LabelControlsHeightForVertical = 103;
    private const int LabelControlsHeightForHorizontal = 77;

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
            : CalculateHeightWithOutGroupBoxes();

    public Panel GeneratePanel()
        => UseGroupBox
            ? GeneratePanelWithGroupBoxes()
            : GeneratePanelWithOutGroupBoxes();

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
                    result.Add(control.Identifier, labelCheckBox.IsChecked);
                    break;
                case LabelComboBox labelComboBox:
                    result.Add(control.Identifier, labelComboBox.SelectedKey);
                    break;
                case LabelDecimalBox labelDecimalBox:
                    result.Add(control.Identifier, labelDecimalBox.Value);
                    break;
                case LabelDecimalXyBox labelDecimalXyBox:
                    if (labelDecimalXyBox.InputDataType == typeof(Point2D) ||
                        labelDecimalXyBox.InputDataType == typeof(Point2D?))
                    {
                        result.Add(control.Identifier, new Point2D(
                            (double)labelDecimalXyBox.ValueX,
                            (double)labelDecimalXyBox.ValueY));
                    }

                    break;
                case LabelIntegerBox labelIntegerBox:
                    result.Add(control.Identifier, labelIntegerBox.Value);
                    break;
                case LabelPixelSizeBox labelPixelSizeBox:
                    result.Add(control.Identifier, new Size(labelPixelSizeBox.ValueWidth, labelPixelSizeBox.ValueHeight));
                    break;
                case LabelSlider labelSlider:
                    result.Add(control.Identifier, labelSlider.Value);
                    break;
                case LabelTextBox labelTextBox:
                    result.Add(control.Identifier, labelTextBox.Text);
                    break;
                case LabelCountrySelector labelCountrySelector:
                    if (NumberHelper.IsInt(labelCountrySelector.SelectedKey))
                    {
                        result.Add(control.Identifier, new CultureInfo(NumberHelper.ParseToInt(labelCountrySelector.SelectedKey)));
                    }

                    break;
                case LabelLanguageSelector labelLanguageSelector:
                    if (NumberHelper.IsInt(labelLanguageSelector.SelectedKey))
                    {
                        result.Add(control.Identifier, new CultureInfo(NumberHelper.ParseToInt(labelLanguageSelector.SelectedKey)));
                    }

                    break;
                case LabelWellKnownColorSelector labelWellKnownColorSelector:
                    var color = ColorUtil.GetColorFromName(labelWellKnownColorSelector.SelectedKey);
                    if (color is not null)
                    {
                        result.Add(control.Identifier, (Color)color);
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

            var heightForGroupIdentifiers = ControlOrientation == Orientation.Vertical
                ? labelControls.Sum(_ => LabelControlsHeightForVertical)
                : labelControls.Sum(_ => LabelControlsHeightForHorizontal);

            heightForGroupIdentifiers += GroupBoxMarginHeight;

            if (heightForGroupIdentifiers > maxHeight)
            {
                maxHeight = heightForGroupIdentifiers;
            }
        }

        return maxHeight;
    }

    public int CalculateHeightWithOutGroupBoxes()
    {
        return ControlOrientation == Orientation.Vertical
            ? LabelControls.Sum(_ => LabelControlsHeightForVertical)
            : LabelControls.Sum(_ => LabelControlsHeightForHorizontal);
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

    private Panel GeneratePanelWithOutGroupBoxes()
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
}