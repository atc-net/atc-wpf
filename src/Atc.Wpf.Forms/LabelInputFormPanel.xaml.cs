namespace Atc.Wpf.Forms;

public partial class LabelInputFormPanel
{
    public LabelInputFormPanel()
    {
        InitializeComponent();
        DataContext = this;
    }

    public LabelInputFormPanel(ILabelControlsForm labelInputFormPanel)
        : this()
    {
        Render(
            new LabelInputFormPanelSettings(),
            labelInputFormPanel);
    }

    public LabelInputFormPanel(
        LabelInputFormPanelSettings settings,
        ILabelControlsForm labelInputFormPanel)
        : this()
    {
        Render(
            settings,
            labelInputFormPanel);
    }

    public LabelInputFormPanelSettings Settings { get; private set; } = new();

    public ILabelControlsForm Data { get; private set; } = new LabelControlsForm();

    public ContentControl ContentControl { get; private set; } = new();

    public void Render(
        LabelInputFormPanelSettings settings,
        ILabelControlsForm labelInputFormPanel)
    {
        this.Settings = settings;
        this.Data = labelInputFormPanel;

        SetContentControlSettings();
        PopulateContentControl();
    }

    public void ReRender()
    {
        SetContentControlSettings();

        if (ContentControl.Content is Panel panel)
        {
            UpdateSettingContentControls(panel);
        }
    }

    private void SetContentControlSettings()
    {
        if (Data.Rows is null)
        {
            return;
        }

        foreach (var row in Data.Rows)
        {
            if (row.Columns is null)
            {
                continue;
            }

            foreach (var column in row.Columns)
            {
                column.SetSettings(
                    Settings.UseGroupBox,
                    Settings.ControlOrientation,
                    Settings.ControlWidth);
            }
        }
    }

    private void PopulateContentControl()
    {
        ContentControl = new ContentControl
        {
            Content = Data.GeneratePanel(),
        };
    }

    private void UpdateSettingContentControls(Panel panel)
    {
        foreach (UIElement uiElement in panel.Children)
        {
            switch (uiElement)
            {
                case Panel subPanel:
                    UpdateSettingContentControls(subPanel);
                    break;
                case GroupBox groupBox:
                {
                    if (groupBox.Content is Panel groupBoxSubPanel)
                    {
                        UpdateSettingContentControls(groupBoxSubPanel);
                    }

                    break;
                }

                case ILabelControlBase labelControl:
                    labelControl.Orientation = Settings.ControlOrientation;
                    break;
            }
        }
    }
}