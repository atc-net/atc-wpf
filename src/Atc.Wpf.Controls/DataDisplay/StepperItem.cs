namespace Atc.Wpf.Controls.DataDisplay;

/// <summary>
/// Represents a single step in a <see cref="Stepper"/> control.
/// </summary>
[ContentProperty(nameof(Content))]
public sealed partial class StepperItem : ContentControl
{
    /// <summary>
    /// The status of this step (Pending, Active, Completed, Error).
    /// </summary>
    [DependencyProperty(DefaultValue = StepperStepStatus.Pending)]
    private StepperStepStatus status;

    /// <summary>
    /// The header text for this step.
    /// </summary>
    [DependencyProperty]
    private string? title;

    /// <summary>
    /// An optional description displayed below the title.
    /// </summary>
    [DependencyProperty]
    private string? subtitle;

    /// <summary>
    /// Custom content displayed inside the step indicator (e.g., an icon).
    /// When set, overrides the default number or status symbol.
    /// </summary>
    [DependencyProperty]
    private object? iconContent;

    /// <summary>
    /// An optional template for rendering the <see cref="IconContent"/>.
    /// </summary>
    [DependencyProperty]
    private DataTemplate? iconTemplate;

    /// <summary>
    /// The zero-based index of this step.
    /// Set automatically by the parent <see cref="Stepper"/> container.
    /// </summary>
    [DependencyProperty(DefaultValue = 0)]
    private int stepIndex;

    /// <summary>
    /// Indicates whether this is the last step in the stepper.
    /// When true, the connecting line after this step is hidden.
    /// Set automatically by the parent <see cref="Stepper"/> container.
    /// </summary>
    [DependencyProperty(DefaultValue = false)]
    private bool isLast;

    /// <summary>
    /// The display text inside the indicator circle (e.g., step number, checkmark, or error symbol).
    /// Set automatically by the parent <see cref="Stepper"/> container.
    /// </summary>
    [DependencyProperty(DefaultValue = "")]
    private string indicatorText;

    /// <summary>
    /// The resolved brush for the step indicator background.
    /// Set automatically by the parent <see cref="Stepper"/> based on the step's status.
    /// </summary>
    [DependencyProperty]
    private Brush? indicatorBrush;

    /// <summary>
    /// The resolved brush for the connector line after this step.
    /// Set automatically by the parent <see cref="Stepper"/> based on the step's status.
    /// </summary>
    [DependencyProperty]
    private Brush? connectorBrush;

    /// <summary>
    /// The diameter of the indicator circle.
    /// Pushed from the parent <see cref="Stepper.IndicatorSize"/>.
    /// </summary>
    [DependencyProperty(DefaultValue = 32d)]
    private double resolvedIndicatorSize;

    /// <summary>
    /// The thickness of the connector line.
    /// Pushed from the parent <see cref="Stepper.LineThickness"/>.
    /// </summary>
    [DependencyProperty(DefaultValue = 2d)]
    private double resolvedLineThickness;

    /// <summary>
    /// Whether clicking the step indicator is enabled.
    /// Pushed from the parent <see cref="Stepper.IsClickable"/>.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool resolvedIsClickable;

    /// <summary>
    /// The computed margin for the connector line, incorporating base gap and step spacing.
    /// Pushed from the parent <see cref="Stepper"/>.
    /// </summary>
    [DependencyProperty]
    private Thickness connectorMargin;

    static StepperItem()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(StepperItem),
            new FrameworkPropertyMetadata(typeof(StepperItem)));
    }
}