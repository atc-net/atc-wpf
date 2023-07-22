namespace Atc.Wpf.Helpers;

[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static class ItemHelper
{
    public static readonly DependencyProperty AlternatingRowBackgroundBrushProperty = DependencyProperty.RegisterAttached(
        "AlternatingRowBackgroundBrush",
        typeof(Brush),
        typeof(ItemHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Brush? GetAlternatingRowBackgroundBrush(
        UIElement element)
        => (Brush?)element.GetValue(AlternatingRowBackgroundBrushProperty);

    public static void SetAlternatingRowBackgroundBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(AlternatingRowBackgroundBrushProperty, value);

    public static readonly DependencyProperty DisableAlternatingRowColoringProperty = DependencyProperty.RegisterAttached(
        "DisableAlternatingRowColoring",
        typeof(bool),
        typeof(ItemHelper),
        new FrameworkPropertyMetadata(
            defaultValue: BooleanBoxes.FalseBox,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static bool GetDisableAlternatingRowColoring(
        UIElement element)
        => (bool)element.GetValue(DisableAlternatingRowColoringProperty);

    public static void SetDisableAlternatingRowColoring(
        UIElement element,
        bool value)
        => element.SetValue(DisableAlternatingRowColoringProperty, value);

    public static readonly DependencyProperty ActiveSelectionBackgroundBrushProperty = DependencyProperty.RegisterAttached(
        "ActiveSelectionBackgroundBrush",
        typeof(Brush),
        typeof(ItemHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Brush? GetActiveSelectionBackgroundBrush(
        UIElement element)
        => (Brush?)element.GetValue(ActiveSelectionBackgroundBrushProperty);

    public static void SetActiveSelectionBackgroundBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(ActiveSelectionBackgroundBrushProperty, value);

    public static readonly DependencyProperty ActiveSelectionBorderBrushProperty = DependencyProperty.RegisterAttached(
        "ActiveSelectionBorderBrush",
        typeof(Brush),
        typeof(ItemHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Brush? GetActiveSelectionBorderBrush(
        UIElement element)
        => (Brush?)element.GetValue(ActiveSelectionBorderBrushProperty);

    public static void SetActiveSelectionBorderBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(ActiveSelectionBorderBrushProperty, value);

    public static readonly DependencyProperty ActiveSelectionForegroundBrushProperty = DependencyProperty.RegisterAttached(
        "ActiveSelectionForegroundBrush",
        typeof(Brush),
        typeof(ItemHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Brush? GetActiveSelectionForegroundBrush(
        UIElement element)
        => (Brush?)element.GetValue(ActiveSelectionForegroundBrushProperty);

    public static void SetActiveSelectionForegroundBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(ActiveSelectionForegroundBrushProperty, value);

    public static readonly DependencyProperty SelectedBackgroundBrushProperty = DependencyProperty.RegisterAttached(
        "SelectedBackgroundBrush",
        typeof(Brush),
        typeof(ItemHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Brush? GetSelectedBackgroundBrush(
        UIElement element)
        => (Brush?)element.GetValue(SelectedBackgroundBrushProperty);

    public static void SetSelectedBackgroundBrush(
        UIElement element,
        Brush? value) =>
        element.SetValue(SelectedBackgroundBrushProperty, value);

    public static readonly DependencyProperty SelectedBorderBrushProperty = DependencyProperty.RegisterAttached(
        "SelectedBorderBrush",
        typeof(Brush),
        typeof(ItemHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Brush? GetSelectedBorderBrush(
        UIElement element)
        => (Brush?)element.GetValue(SelectedBorderBrushProperty);

    public static void SetSelectedBorderBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(SelectedBorderBrushProperty, value);

    public static readonly DependencyProperty SelectedForegroundBrushProperty = DependencyProperty.RegisterAttached(
        "SelectedForegroundBrush",
        typeof(Brush),
        typeof(ItemHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Brush? GetSelectedForegroundBrush(
        UIElement element)
        => (Brush?)element.GetValue(SelectedForegroundBrushProperty);

    public static void SetSelectedForegroundBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(SelectedForegroundBrushProperty, value);

    public static readonly DependencyProperty HoverBackgroundBrushProperty = DependencyProperty.RegisterAttached(
        "HoverBackgroundBrush",
        typeof(Brush),
        typeof(ItemHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Brush? GetHoverBackgroundBrush(
        UIElement element)
        => (Brush?)element.GetValue(HoverBackgroundBrushProperty);

    public static void SetHoverBackgroundBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(HoverBackgroundBrushProperty, value);

    public static readonly DependencyProperty HoverBorderBrushProperty = DependencyProperty.RegisterAttached(
        "HoverBorderBrush",
        typeof(Brush),
        typeof(ItemHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Brush? GetHoverBorderBrush(
        UIElement element)
        => (Brush?)element.GetValue(HoverBorderBrushProperty);

    public static void SetHoverBorderBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(HoverBorderBrushProperty, value);

    public static readonly DependencyProperty HoverForegroundBrushProperty = DependencyProperty.RegisterAttached(
        "HoverForegroundBrush",
        typeof(Brush),
        typeof(ItemHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Brush? GetHoverForegroundBrush(
        UIElement element)
        => (Brush?)element.GetValue(HoverForegroundBrushProperty);

    public static void SetHoverForegroundBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(HoverForegroundBrushProperty, value);

    public static readonly DependencyProperty HoverSelectedBackgroundBrushProperty = DependencyProperty.RegisterAttached(
        "HoverSelectedBackgroundBrush",
        typeof(Brush),
        typeof(ItemHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Brush? GetHoverSelectedBackgroundBrush(
        UIElement element)
        => (Brush?)element.GetValue(HoverSelectedBackgroundBrushProperty);

    public static void SetHoverSelectedBackgroundBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(HoverSelectedBackgroundBrushProperty, value);

    public static readonly DependencyProperty HoverSelectedBorderBrushProperty = DependencyProperty.RegisterAttached(
        "HoverSelectedBorderBrush",
        typeof(Brush),
        typeof(ItemHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Brush? GetHoverSelectedBorderBrush(
        UIElement element)
        => (Brush?)element.GetValue(HoverSelectedBorderBrushProperty);

    public static void SetHoverSelectedBorderBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(HoverSelectedBorderBrushProperty, value);

    public static readonly DependencyProperty HoverSelectedForegroundBrushProperty = DependencyProperty.RegisterAttached(
        "HoverSelectedForegroundBrush",
        typeof(Brush),
        typeof(ItemHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Brush? GetHoverSelectedForegroundBrush(
        UIElement element)
        => (Brush?)element.GetValue(HoverSelectedForegroundBrushProperty);

    public static void SetHoverSelectedForegroundBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(HoverSelectedForegroundBrushProperty, value);

    public static readonly DependencyProperty DisabledSelectedBackgroundBrushProperty = DependencyProperty.RegisterAttached(
        "DisabledSelectedBackgroundBrush",
        typeof(Brush),
        typeof(ItemHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Brush? GetDisabledSelectedBackgroundBrush(
        UIElement element)
        => (Brush?)element.GetValue(DisabledSelectedBackgroundBrushProperty);

    public static void SetDisabledSelectedBackgroundBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(DisabledSelectedBackgroundBrushProperty, value);

    public static readonly DependencyProperty DisabledSelectedBorderBrushProperty = DependencyProperty.RegisterAttached(
        "DisabledSelectedBorderBrush",
        typeof(Brush),
        typeof(ItemHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Brush? GetDisabledSelectedBorderBrush(
        UIElement element)
        => (Brush?)element.GetValue(DisabledSelectedBorderBrushProperty);

    public static void SetDisabledSelectedBorderBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(DisabledSelectedBorderBrushProperty, value);

    public static readonly DependencyProperty DisabledSelectedForegroundBrushProperty
        = DependencyProperty.RegisterAttached(
            "DisabledSelectedForegroundBrush",
            typeof(Brush),
            typeof(ItemHelper),
            new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Brush? GetDisabledSelectedForegroundBrush(
        UIElement element)
        => (Brush?)element.GetValue(DisabledSelectedForegroundBrushProperty);

    public static void SetDisabledSelectedForegroundBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(DisabledSelectedForegroundBrushProperty, value);

    public static readonly DependencyProperty DisabledBackgroundBrushProperty = DependencyProperty.RegisterAttached(
        "DisabledBackgroundBrush",
        typeof(Brush),
        typeof(ItemHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Brush? GetDisabledBackgroundBrush(
        UIElement element)
        => (Brush?)element.GetValue(DisabledBackgroundBrushProperty);

    public static void SetDisabledBackgroundBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(DisabledBackgroundBrushProperty, value);

    public static readonly DependencyProperty DisabledBorderBrushProperty = DependencyProperty.RegisterAttached(
        "DisabledBorderBrush",
        typeof(Brush),
        typeof(ItemHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Brush? GetDisabledBorderBrush(
        UIElement element)
        => (Brush?)element.GetValue(DisabledBorderBrushProperty);

    public static void SetDisabledBorderBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(DisabledBorderBrushProperty, value);

    public static readonly DependencyProperty DisabledForegroundBrushProperty = DependencyProperty.RegisterAttached(
        "DisabledForegroundBrush",
        typeof(Brush),
        typeof(ItemHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Brush? GetDisabledForegroundBrush(
        UIElement element)
        => (Brush?)element.GetValue(DisabledForegroundBrushProperty);

    public static void SetDisabledForegroundBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(DisabledForegroundBrushProperty, value);

    private static readonly DependencyPropertyKey IsMouseLeftButtonPressedPropertyKey = DependencyProperty.RegisterAttachedReadOnly(
        "IsMouseLeftButtonPressed",
        typeof(bool),
        typeof(ItemHelper),
        new PropertyMetadata(BooleanBoxes.FalseBox));

    public static readonly DependencyProperty IsMouseLeftButtonPressedProperty = IsMouseLeftButtonPressedPropertyKey.DependencyProperty;

    public static bool GetIsMouseLeftButtonPressed(
        UIElement element)
        => (bool)element.GetValue(IsMouseLeftButtonPressedProperty);

    public static readonly DependencyProperty MouseLeftButtonPressedBackgroundBrushProperty = DependencyProperty.RegisterAttached(
        "MouseLeftButtonPressedBackgroundBrush",
        typeof(Brush),
        typeof(ItemHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender,
            OnMouseLeftButtonPressedPropertyChanged));

    public static Brush? GetMouseLeftButtonPressedBackgroundBrush(
        UIElement element)
        => (Brush?)element.GetValue(MouseLeftButtonPressedBackgroundBrushProperty);

    public static void SetMouseLeftButtonPressedBackgroundBrush(
        UIElement element, Brush? value)
        => element.SetValue(MouseLeftButtonPressedBackgroundBrushProperty, value);

    public static readonly DependencyProperty MouseLeftButtonPressedBorderBrushProperty = DependencyProperty.RegisterAttached(
        "MouseLeftButtonPressedBorderBrush",
        typeof(Brush),
        typeof(ItemHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender,
            OnMouseLeftButtonPressedBorderPropertyChanged));

    public static Brush? GetMouseLeftButtonPressedBorderBrush(
        UIElement element)
        => (Brush?)element.GetValue(MouseLeftButtonPressedBorderBrushProperty);

    public static void SetMouseLeftButtonPressedBorderBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(MouseLeftButtonPressedBorderBrushProperty, value);

    public static readonly DependencyProperty MouseLeftButtonPressedForegroundBrushProperty = DependencyProperty.RegisterAttached(
        "MouseLeftButtonPressedForegroundBrush",
        typeof(Brush),
        typeof(ItemHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender,
            OnMouseLeftButtonPressedForegroundBrushPropertyChanged));

    public static Brush? GetMouseLeftButtonPressedForegroundBrush(
        UIElement element)
        => (Brush?)element.GetValue(MouseLeftButtonPressedForegroundBrushProperty);

    public static void SetMouseLeftButtonPressedForegroundBrush(
        UIElement element,
        Brush? value)
    {
        element.SetValue(MouseLeftButtonPressedForegroundBrushProperty, value);
    }

    private static readonly DependencyPropertyKey IsMouseRightButtonPressedPropertyKey = DependencyProperty.RegisterAttachedReadOnly(
        "IsMouseRightButtonPressed",
        typeof(bool),
        typeof(ItemHelper),
        new PropertyMetadata(BooleanBoxes.FalseBox));

    public static readonly DependencyProperty IsMouseRightButtonPressedProperty = IsMouseRightButtonPressedPropertyKey.DependencyProperty;

    public static bool GetIsMouseRightButtonPressed(
        UIElement element)
        => (bool)element.GetValue(IsMouseRightButtonPressedProperty);

    public static readonly DependencyProperty MouseRightButtonPressedBackgroundBrushProperty = DependencyProperty.RegisterAttached(
        "MouseRightButtonPressedBackgroundBrush",
        typeof(Brush),
        typeof(ItemHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender,
            OnMouseRightButtonPressedPropertyChanged));

    public static Brush? GetMouseRightButtonPressedBackgroundBrush(
        UIElement element)
        => (Brush?)element.GetValue(MouseRightButtonPressedBackgroundBrushProperty);

    public static void SetMouseRightButtonPressedBackgroundBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(MouseRightButtonPressedBackgroundBrushProperty, value);

    public static readonly DependencyProperty MouseRightButtonPressedBorderBrushProperty = DependencyProperty.RegisterAttached(
        "MouseRightButtonPressedBorderBrush",
        typeof(Brush),
        typeof(ItemHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender,
            OnMouseRightButtonPressedBorderPropertyChanged));

    public static Brush? GetMouseRightButtonPressedBorderBrush(
        UIElement element)
        => (Brush?)element.GetValue(MouseRightButtonPressedBorderBrushProperty);

    public static void SetMouseRightButtonPressedBorderBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(MouseRightButtonPressedBorderBrushProperty, value);

    public static readonly DependencyProperty MouseRightButtonPressedForegroundBrushProperty = DependencyProperty.RegisterAttached(
        "MouseRightButtonPressedForegroundBrush",
        typeof(Brush),
        typeof(ItemHelper),
        new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.AffectsRender,
            OnMouseRightButtonPressedForegroundBrushPropertyChanged));

    public static Brush? GetMouseRightButtonPressedForegroundBrush(
        UIElement element)
        => (Brush?)element.GetValue(MouseRightButtonPressedForegroundBrushProperty);

    public static void SetMouseRightButtonPressedForegroundBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(MouseRightButtonPressedForegroundBrushProperty, value);

    public static readonly DependencyProperty GridViewHeaderIndicatorBrushProperty = DependencyProperty.RegisterAttached(
        "GridViewHeaderIndicatorBrush",
        typeof(Brush),
        typeof(ItemHelper),
        new FrameworkPropertyMetadata(propertyChangedCallback: null));

    public static Brush? GetGridViewHeaderIndicatorBrush(
        UIElement element)
        => (Brush?)element.GetValue(GridViewHeaderIndicatorBrushProperty);

    public static void SetGridViewHeaderIndicatorBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(GridViewHeaderIndicatorBrushProperty, value);

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