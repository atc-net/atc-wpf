// ReSharper disable EmptyConstructor
namespace Atc.Wpf.Theming.Controls.Icons;

/// <summary>
/// Represents the base class for an icon UI element.
/// </summary>
[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public abstract class IconElement : Control
{
    private bool isForegroundPropertyDefaultOrInherited = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="IconElement"/> class.
    /// </summary>
    protected IconElement()
    {
        // Empty
    }

    static IconElement()
    {
        ForegroundProperty.OverrideMetadata(
            typeof(IconElement),
            new FrameworkPropertyMetadata(
                SystemColors.ControlTextBrush,
                FrameworkPropertyMetadataOptions.Inherits,
                (sender, e) => ((IconElement)sender).OnForegroundPropertyChanged(e)));
    }

    /// <summary>
    /// Called when the <see cref="Control.Foreground"/> property changes; tracks whether the value is
    /// default or inherited and updates <see cref="InheritsForegroundFromVisualParent"/>.
    /// </summary>
    /// <param name="e">The event data describing the property change.</param>
    protected void OnForegroundPropertyChanged(
        DependencyPropertyChangedEventArgs e)
    {
        var baseValueSource = DependencyPropertyHelper
            .GetValueSource(
                this,
                e.Property)
            .BaseValueSource;
        isForegroundPropertyDefaultOrInherited = baseValueSource <= BaseValueSource.Inherited;
        UpdateInheritsForegroundFromVisualParent();
    }

    /// <inheritdoc />
    protected override void OnVisualParentChanged(DependencyObject oldParent)
    {
        base.OnVisualParentChanged(oldParent);
        UpdateInheritsForegroundFromVisualParent();
    }

    private void UpdateInheritsForegroundFromVisualParent()
    {
        InheritsForegroundFromVisualParent
            = isForegroundPropertyDefaultOrInherited
              && Parent is not null
              && VisualParent is not null
              && !Parent.Equals(VisualParent);
    }

    internal static readonly DependencyPropertyKey InheritsForegroundFromVisualParentPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(InheritsForegroundFromVisualParent),
            typeof(bool),
            typeof(IconElement),
            new PropertyMetadata(
                BooleanBoxes.FalseBox,
                (sender, e) => ((IconElement)sender).OnInheritsForegroundFromVisualParentPropertyChanged(e)));

    /// <summary>Identifies the <see cref="InheritsForegroundFromVisualParent"/> dependency property.</summary>
    public static readonly DependencyProperty InheritsForegroundFromVisualParentProperty = InheritsForegroundFromVisualParentPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets whether that this element inherits the <see cref="Control.Foreground"/> form the <see cref="Visual.VisualParent"/>.
    /// </summary>
    public bool InheritsForegroundFromVisualParent
    {
        get => (bool)GetValue(InheritsForegroundFromVisualParentProperty);
        protected set => SetValue(
            InheritsForegroundFromVisualParentPropertyKey,
            BooleanBoxes.Box(value));
    }

    /// <summary>
    /// Called when <see cref="InheritsForegroundFromVisualParent"/> changes; binds or clears
    /// <see cref="VisualParentForeground"/> to the visual parent's foreground.
    /// </summary>
    /// <param name="e">The event data describing the property change.</param>
    [SuppressMessage("Minor", "S1125:Boolean literals should not be redundant", Justification = "OK.")]
    protected virtual void OnInheritsForegroundFromVisualParentPropertyChanged(
        DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue == e.NewValue)
        {
            return;
        }

        if (e.NewValue is true)
        {
            SetBinding(
                VisualParentForegroundProperty,
                new Binding
                {
                    Path = new PropertyPath(TextElement.ForegroundProperty),
                    Source = VisualParent,
                });
        }
        else
        {
            ClearValue(VisualParentForegroundProperty);
        }
    }

    private static readonly DependencyProperty VisualParentForegroundProperty =
        DependencyProperty.Register(
            nameof(VisualParentForeground),
            typeof(Brush),
            typeof(IconElement),
            new PropertyMetadata(
                default(Brush),
                (sender, e) => ((IconElement)sender).OnVisualParentForegroundPropertyChanged(e)));

    /// <summary>
    /// Gets or sets the foreground brush of the visual parent, bound while
    /// <see cref="InheritsForegroundFromVisualParent"/> is <see langword="true"/>.
    /// </summary>
    protected Brush? VisualParentForeground
    {
        get => (Brush?)GetValue(VisualParentForegroundProperty);
        set => SetValue(VisualParentForegroundProperty, value);
    }

    /// <summary>
    /// Called when <see cref="VisualParentForeground"/> changes.
    /// </summary>
    /// <param name="e">The event data describing the property change.</param>
    protected virtual void OnVisualParentForegroundPropertyChanged(
        DependencyPropertyChangedEventArgs e)
    {
    }

    /// <summary>Identifies the <c>Geometry</c> attached property.</summary>
    public static readonly DependencyProperty GeometryProperty =
        DependencyProperty.RegisterAttached(
            "Geometry",
            typeof(Geometry),
            typeof(IconElement),
            new PropertyMetadata(default(Geometry)));

    /// <summary>
    /// Sets the <c>Geometry</c> attached property on the specified element.
    /// </summary>
    /// <param name="element">The element to set the value on.</param>
    /// <param name="value">The geometry to assign.</param>
    public static void SetGeometry(
        DependencyObject element,
        Geometry value)
        => element.SetValue(
            GeometryProperty,
            value);

    /// <summary>
    /// Gets the <c>Geometry</c> attached property from the specified element.
    /// </summary>
    /// <param name="element">The element to read the value from.</param>
    /// <returns>The assigned geometry.</returns>
    public static Geometry GetGeometry(DependencyObject element)
        => (Geometry)element.GetValue(GeometryProperty);
}