namespace Atc.Wpf.Controls.LabelControls;

public class LabelControlsFormColumn : ILabelControlsFormColumn
{
    private const int LabelControlsHeightForVertical = 103;
    private const int LabelControlsHeightForHorizontal = 77;

    public LabelControlsFormColumn(
        IList<ILabelControlBase> labelControls)
    {
        ArgumentNullException.ThrowIfNull(labelControls);

        this.LabelControls = labelControls;
    }

    public Orientation ControlOrientation { get; set; } = Orientation.Vertical;

    public int ControlWidth { get; set; } = 300;

    public IList<ILabelControlBase> LabelControls { get; }

    public int CalculateHeight()
        => ControlOrientation == Orientation.Vertical
            ? LabelControls.Sum(_ => LabelControlsHeightForVertical)
            : LabelControls.Sum(_ => LabelControlsHeightForHorizontal);

    public Panel GeneratePanel()
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
}