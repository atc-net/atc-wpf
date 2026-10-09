namespace Atc.Wpf.Forms;

public partial class LabelInputFormPanel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LabelInputFormPanel"/> class.
    /// </summary>
    public LabelInputFormPanel()
    {
        InitializeComponent();
        DataContext = this;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LabelInputFormPanel"/> class and renders the form with default settings.
    /// </summary>
    /// <param name="labelInputFormPanel">The form to render.</param>
    public LabelInputFormPanel(ILabelControlsForm labelInputFormPanel)
        : this()
    {
        Render(
            new LabelInputFormPanelSettings(),
            labelInputFormPanel);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LabelInputFormPanel"/> class and renders the form with the given settings.
    /// </summary>
    /// <param name="settings">The layout settings.</param>
    /// <param name="labelInputFormPanel">The form to render.</param>
    public LabelInputFormPanel(
        LabelInputFormPanelSettings settings,
        ILabelControlsForm labelInputFormPanel)
        : this()
    {
        Render(
            settings,
            labelInputFormPanel);
    }

    /// <summary>
    /// Gets the layout settings used for rendering.
    /// </summary>
    public LabelInputFormPanelSettings Settings { get; private set; } = new();

    /// <summary>
    /// Gets the rendered form.
    /// </summary>
    public ILabelControlsForm Data { get; private set; } = new LabelControlsForm();

    /// <summary>
    /// Gets the content control that hosts the generated form panel.
    /// </summary>
    public ContentControl ContentControl { get; private set; } = new();

    /// <summary>
    /// Renders the form with the given settings.
    /// </summary>
    /// <param name="settings">The layout settings.</param>
    /// <param name="labelInputFormPanel">The form to render.</param>
    public void Render(
        LabelInputFormPanelSettings settings,
        ILabelControlsForm labelInputFormPanel)
    {
        this.Settings = settings;
        this.Data = labelInputFormPanel;

        SetContentControlSettings();
        PopulateContentControl();
    }

    /// <summary>
    /// Re-applies the current settings to the already rendered form.
    /// </summary>
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