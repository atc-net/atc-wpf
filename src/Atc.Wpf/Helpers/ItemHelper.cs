namespace Atc.Wpf.Helpers;

/// <summary>Provides attached properties for styling items in items controls, such as brushes for the alternating, selected, hover, pressed and disabled states.</summary>
[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static partial class ItemHelper
{
    /// <summary>Gets or sets the value of the AlternatingRowBackgroundBrush attached property.</summary>
    [AttachedProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Brush? alternatingRowBackgroundBrush;

    /// <summary>Gets or sets the value of the DisableAlternatingRowColoring attached property.</summary>
    [AttachedProperty(
        DefaultValue = false,
        Flags = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static bool disableAlternatingRowColoring;

    /// <summary>Gets or sets the value of the ActiveSelectionBackgroundBrush attached property.</summary>
    [AttachedProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Brush? activeSelectionBackgroundBrush;

    /// <summary>Gets or sets the value of the ActiveSelectionBorderBrush attached property.</summary>
    [AttachedProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Brush? activeSelectionBorderBrush;

    /// <summary>Gets or sets the value of the ActiveSelectionForegroundBrush attached property.</summary>
    [AttachedProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Brush? activeSelectionForegroundBrush;

    /// <summary>Gets or sets the value of the SelectedBackgroundBrush attached property.</summary>
    [AttachedProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Brush? selectedBackgroundBrush;

    /// <summary>Gets or sets the value of the SelectedBorderBrush attached property.</summary>
    [AttachedProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Brush? selectedBorderBrush;

    /// <summary>Gets or sets the value of the SelectedForegroundBrush attached property.</summary>
    [AttachedProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Brush? selectedForegroundBrush;

    /// <summary>Gets or sets the value of the HoverBackgroundBrush attached property.</summary>
    [AttachedProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Brush? hoverBackgroundBrush;

    /// <summary>Gets or sets the value of the HoverBorderBrush attached property.</summary>
    [AttachedProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Brush? hoverBorderBrush;

    /// <summary>Gets or sets the value of the HoverForegroundBrush attached property.</summary>
    [AttachedProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Brush? hoverForegroundBrush;

    /// <summary>Gets or sets the value of the HoverSelectedBackgroundBrush attached property.</summary>
    [AttachedProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Brush? hoverSelectedBackgroundBrush;

    /// <summary>Gets or sets the value of the HoverSelectedBorderBrush attached property.</summary>
    [AttachedProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Brush? hoverSelectedBorderBrush;

    /// <summary>Gets or sets the value of the HoverSelectedForegroundBrush attached property.</summary>
    [AttachedProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Brush? hoverSelectedForegroundBrush;

    /// <summary>Gets or sets the value of the DisabledSelectedBackgroundBrush attached property.</summary>
    [AttachedProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Brush? disabledSelectedBackgroundBrush;

    /// <summary>Gets or sets the value of the DisabledSelectedBorderBrush attached property.</summary>
    [AttachedProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Brush? disabledSelectedBorderBrush;

    /// <summary>Gets or sets the value of the DisabledSelectedForegroundBrush attached property.</summary>
    [AttachedProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Brush? disabledSelectedForegroundBrush;

    /// <summary>Gets or sets the value of the DisabledBackgroundBrush attached property.</summary>
    [AttachedProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Brush? disabledBackgroundBrush;

    /// <summary>Gets or sets the value of the DisabledBorderBrush attached property.</summary>
    [AttachedProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Brush? disabledBorderBrush;

    /// <summary>Gets or sets the value of the DisabledForegroundBrush attached property.</summary>
    [AttachedProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Brush? disabledForegroundBrush;

    /// <summary>Identifies the IsMouseLeftButtonPressed attached property.</summary>
    [AttachedProperty(DefaultValue = false, IsReadOnly = true, GenerateClrAccessors = false)]
    private static bool isMouseLeftButtonPressed;

    /// <summary>Gets the value of the IsMouseLeftButtonPressed attached property.</summary>
    public static bool GetIsMouseLeftButtonPressed(UIElement element)
        => (bool)element.GetValue(IsMouseLeftButtonPressedProperty);

    /// <summary>Gets or sets the value of the MouseLeftButtonPressedBackgroundBrush attached property.</summary>
    [AttachedProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsRender,
        PropertyChangedCallback = nameof(OnMouseLeftButtonPressedPropertyChanged),
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Brush? mouseLeftButtonPressedBackgroundBrush;

    /// <summary>Gets or sets the value of the MouseLeftButtonPressedBorderBrush attached property.</summary>
    [AttachedProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsRender,
        PropertyChangedCallback = nameof(OnMouseLeftButtonPressedBorderPropertyChanged),
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Brush? mouseLeftButtonPressedBorderBrush;

    /// <summary>Gets or sets the value of the MouseLeftButtonPressedForegroundBrush attached property.</summary>
    [AttachedProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsRender,
        PropertyChangedCallback = nameof(OnMouseLeftButtonPressedForegroundBrushPropertyChanged),
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Brush? mouseLeftButtonPressedForegroundBrush;

    /// <summary>Identifies the IsMouseRightButtonPressed attached property.</summary>
    [AttachedProperty(DefaultValue = false, IsReadOnly = true, GenerateClrAccessors = false)]
    private static bool isMouseRightButtonPressed;

    /// <summary>Gets the value of the IsMouseRightButtonPressed attached property.</summary>
    public static bool GetIsMouseRightButtonPressed(UIElement element)
        => (bool)element.GetValue(IsMouseRightButtonPressedProperty);

    /// <summary>Gets or sets the value of the MouseRightButtonPressedBackgroundBrush attached property.</summary>
    [AttachedProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsRender,
        PropertyChangedCallback = nameof(OnMouseRightButtonPressedPropertyChanged),
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Brush? mouseRightButtonPressedBackgroundBrush;

    /// <summary>Gets or sets the value of the MouseRightButtonPressedBorderBrush attached property.</summary>
    [AttachedProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsRender,
        PropertyChangedCallback = nameof(OnMouseRightButtonPressedBorderPropertyChanged),
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Brush? mouseRightButtonPressedBorderBrush;

    /// <summary>Gets or sets the value of the MouseRightButtonPressedForegroundBrush attached property.</summary>
    [AttachedProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsRender,
        PropertyChangedCallback = nameof(OnMouseRightButtonPressedForegroundBrushPropertyChanged),
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Brush? mouseRightButtonPressedForegroundBrush;

    /// <summary>Gets or sets the value of the GridViewHeaderIndicatorBrush attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Brush? gridViewHeaderIndicatorBrush;

    private static void OnPreviewMouseLeftButtonDown(
        object sender,
        MouseButtonEventArgs e)
    {
        var element = (UIElement)sender;
        if (e.ButtonState == MouseButtonState.Pressed)
        {
            element.SetValue(IsMouseLeftButtonPressedPropertyKey, BooleanBoxes.TrueBox);
        }
    }

    private static void OnPreviewMouseLeftButtonUp(
        object sender,
        MouseButtonEventArgs e)
    {
        var element = (UIElement)sender;
        if (GetIsMouseLeftButtonPressed(element))
        {
            element.SetValue(IsMouseLeftButtonPressedPropertyKey, BooleanBoxes.FalseBox);
        }
    }

    private static void OnPreviewMouseRightButtonDown(
        object sender,
        MouseButtonEventArgs e)
    {
        var element = (UIElement)sender;
        if (e.ButtonState != MouseButtonState.Pressed)
        {
            return;
        }

        if (element is TreeViewItem)
        {
            Mouse.Capture(element, CaptureMode.SubTree);
        }
        else
        {
            Mouse.Capture(element);
        }

        element.SetValue(IsMouseRightButtonPressedPropertyKey, BooleanBoxes.TrueBox);
    }

    private static void OnPreviewMouseRightButtonUp(
        object sender,
        MouseButtonEventArgs e)
    {
        var element = (UIElement)sender;
        if (!GetIsMouseRightButtonPressed(element))
        {
            return;
        }

        Mouse.Capture(element: null);
        element.SetValue(IsMouseRightButtonPressedPropertyKey, BooleanBoxes.FalseBox);
    }

    private static void OnMouseLeftButtonPressedPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not UIElement element || e.OldValue == e.NewValue)
        {
            return;
        }

        element.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
        element.PreviewMouseLeftButtonUp -= OnPreviewMouseLeftButtonUp;
        element.MouseEnter -= OnLeftMouseEnter;
        element.MouseLeave -= OnLeftMouseLeave;

        if (e.NewValue is not Brush &&
            GetMouseLeftButtonPressedForegroundBrush(element) is null &&
            GetMouseLeftButtonPressedBorderBrush(element) is null)
        {
            return;
        }

        element.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
        element.PreviewMouseLeftButtonUp += OnPreviewMouseLeftButtonUp;
        element.MouseEnter += OnLeftMouseEnter;
        element.MouseLeave += OnLeftMouseLeave;
    }

    private static void OnMouseLeftButtonPressedBorderPropertyChanged(
       DependencyObject d,
       DependencyPropertyChangedEventArgs e)
    {
        if (d is not UIElement element || e.OldValue == e.NewValue)
        {
            return;
        }

        element.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
        element.PreviewMouseLeftButtonUp -= OnPreviewMouseLeftButtonUp;
        element.MouseEnter -= OnLeftMouseEnter;
        element.MouseLeave -= OnLeftMouseLeave;

        if (e.NewValue is not Brush &&
            GetMouseLeftButtonPressedForegroundBrush(element) is null &&
            GetMouseLeftButtonPressedBackgroundBrush(element) is null)
        {
            return;
        }

        element.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
        element.PreviewMouseLeftButtonUp += OnPreviewMouseLeftButtonUp;
        element.MouseEnter += OnLeftMouseEnter;
        element.MouseLeave += OnLeftMouseLeave;
    }

    private static void OnMouseLeftButtonPressedForegroundBrushPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not UIElement element ||
            e.OldValue == e.NewValue)
        {
            return;
        }

        element.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
        element.PreviewMouseLeftButtonUp -= OnPreviewMouseLeftButtonUp;
        element.MouseEnter -= OnLeftMouseEnter;
        element.MouseLeave -= OnLeftMouseLeave;

        if (e.NewValue is not Brush &&
            GetMouseLeftButtonPressedBackgroundBrush(element) is null &&
            GetMouseLeftButtonPressedBorderBrush(element) is null)
        {
            return;
        }

        element.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
        element.PreviewMouseLeftButtonUp += OnPreviewMouseLeftButtonUp;
        element.MouseEnter += OnLeftMouseEnter;
        element.MouseLeave += OnLeftMouseLeave;
    }

    private static void OnLeftMouseEnter(
        object sender,
        MouseEventArgs e)
    {
        var element = (UIElement)sender;
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            element.SetValue(IsMouseLeftButtonPressedPropertyKey, BooleanBoxes.TrueBox);
        }
    }

    private static void OnLeftMouseLeave(
        object sender,
        MouseEventArgs e)
    {
        var element = (UIElement)sender;
        if (e.LeftButton == MouseButtonState.Pressed && GetIsMouseLeftButtonPressed(element))
        {
            element.SetValue(IsMouseLeftButtonPressedPropertyKey, BooleanBoxes.FalseBox);
        }
    }

    private static void OnMouseRightButtonPressedPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not UIElement element ||
            e.OldValue == e.NewValue)
        {
            return;
        }

        element.PreviewMouseRightButtonDown -= OnPreviewMouseRightButtonDown;
        element.PreviewMouseRightButtonUp -= OnPreviewMouseRightButtonUp;

        if (e.NewValue is not Brush &&
            GetMouseRightButtonPressedForegroundBrush(element) is null &&
            GetMouseRightButtonPressedBorderBrush(element) is null)
        {
            return;
        }

        element.PreviewMouseRightButtonDown += OnPreviewMouseRightButtonDown;
        element.PreviewMouseRightButtonUp += OnPreviewMouseRightButtonUp;
    }

    private static void OnMouseRightButtonPressedForegroundBrushPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not UIElement element ||
            e.OldValue == e.NewValue)
        {
            return;
        }

        element.PreviewMouseRightButtonDown -= OnPreviewMouseRightButtonDown;
        element.PreviewMouseRightButtonUp -= OnPreviewMouseRightButtonUp;

        if (e.NewValue is not Brush &&
            GetMouseRightButtonPressedBackgroundBrush(element) is null &&
            GetMouseRightButtonPressedBorderBrush(element) is null)
        {
            return;
        }

        element.PreviewMouseRightButtonDown += OnPreviewMouseRightButtonDown;
        element.PreviewMouseRightButtonUp += OnPreviewMouseRightButtonUp;
    }

    private static void OnMouseRightButtonPressedBorderPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not UIElement element ||
            e.OldValue == e.NewValue)
        {
            return;
        }

        element.PreviewMouseRightButtonDown -= OnPreviewMouseRightButtonDown;
        element.PreviewMouseRightButtonUp -= OnPreviewMouseRightButtonUp;

        if (e.NewValue is not Brush &&
            GetMouseRightButtonPressedForegroundBrush(element) is null &&
            GetMouseRightButtonPressedBackgroundBrush(element) is null)
        {
            return;
        }

        element.PreviewMouseRightButtonDown += OnPreviewMouseRightButtonDown;
        element.PreviewMouseRightButtonUp += OnPreviewMouseRightButtonUp;
    }
}