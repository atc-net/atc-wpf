namespace Atc.Wpf.Controls.Inputs;

/// <summary>
/// A rating control that displays a row of stars (or custom icons) for user rating input.
/// </summary>
/// <remarks>
/// Features:
/// <list type="bullet">
///   <item>Configurable number of items (1-10)</item>
///   <item>Half-star precision option</item>
///   <item>Customizable filled/empty/half icons</item>
///   <item>Read-only mode</item>
///   <item>Hover preview</item>
///   <item>Keyboard accessibility</item>
/// </list>
/// </remarks>
[TemplatePart(Name = nameof(ItemsHost), Type = typeof(StackPanel))]
public partial class Rating : Control
{
    private StackPanel? ItemsHost { get; set; }

    private readonly List<ContentControl> ratingItems = [];
    private double previewValue = -1;

    /// <summary>
    /// Gets or sets the current rating value.
    /// </summary>
    [DependencyProperty(
        DefaultValue = 0.0,
        PropertyChangedCallback = nameof(OnValueChanged),
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)]
    private double value;

    /// <summary>
    /// Gets or sets the maximum number of rating items.
    /// </summary>
    [DependencyProperty(
        DefaultValue = 5,
        PropertyChangedCallback = nameof(OnMaximumChanged))]
    private int maximum;

    /// <summary>
    /// Gets or sets a value indicating whether half-star precision is enabled.
    /// </summary>
    [DependencyProperty(DefaultValue = false)]
    private bool allowHalfStars;

    /// <summary>
    /// Gets or sets a value indicating whether the control is read-only.
    /// </summary>
    [DependencyProperty(
        DefaultValue = false,
        PropertyChangedCallback = nameof(OnIsReadOnlyChanged))]
    private bool isReadOnly;

    /// <summary>
    /// Gets or sets a value indicating whether hover preview is enabled.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool showPreviewOnHover;

    /// <summary>
    /// Gets or sets the content displayed for a filled rating item.
    /// </summary>
    [DependencyProperty(
        DefaultValue = "\"★\"",
        PropertyChangedCallback = nameof(OnIconChanged))]
    private object filledIcon;

    /// <summary>
    /// Gets or sets the content displayed for an empty rating item.
    /// </summary>
    [DependencyProperty(
        DefaultValue = "\"☆\"",
        PropertyChangedCallback = nameof(OnIconChanged))]
    private object emptyIcon;

    /// <summary>
    /// Gets or sets the content displayed for a half-filled rating item.
    /// </summary>
    [DependencyProperty(
        DefaultValue = "\"★\"",
        PropertyChangedCallback = nameof(OnIconChanged))]
    private object halfFilledIcon;

    /// <summary>
    /// Gets or sets the size of each rating item.
    /// </summary>
    [DependencyProperty(
        DefaultValue = 24.0,
        PropertyChangedCallback = nameof(OnItemSizeChanged))]
    private double itemSize;

    /// <summary>
    /// Gets or sets the spacing between rating items.
    /// </summary>
    [DependencyProperty(
        DefaultValue = 4.0,
        PropertyChangedCallback = nameof(OnItemSpacingChanged))]
    private double itemSpacing;

    /// <summary>
    /// Gets or sets the brush for filled rating items.
    /// </summary>
    [DependencyProperty]
    private Brush? filledBrush;

    /// <summary>
    /// Gets or sets the brush for empty rating items.
    /// </summary>
    [DependencyProperty]
    private Brush? emptyBrush;

    /// <summary>
    /// Gets or sets the brush for rating items during hover preview.
    /// </summary>
    [DependencyProperty]
    private Brush? previewBrush;

    /// <summary>
    /// Occurs when the rating value changes.
    /// </summary>
    public event RoutedPropertyChangedEventHandler<double>? ValueChanged;

    static Rating()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(Rating),
            new FrameworkPropertyMetadata(typeof(Rating)));

        FocusableProperty.OverrideMetadata(
            typeof(Rating),
            new FrameworkPropertyMetadata(BooleanBoxes.TrueBox));
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        ItemsHost = GetTemplateChild(nameof(ItemsHost)) as StackPanel;

        BuildRatingItems();
        UpdateVisualState();
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnMouseMove(e);

        if (IsReadOnly || !ShowPreviewOnHover || ItemsHost is null)
        {
            return;
        }

        var position = e.GetPosition(ItemsHost);
        var newPreviewValue = CalculateValueFromPosition(position.X);

        if (System.Math.Abs(previewValue - newPreviewValue) > double.Epsilon)
        {
            previewValue = newPreviewValue;
            UpdateVisualState(isPreview: true);
        }
    }

    protected override void OnMouseLeave(MouseEventArgs e)
    {
        base.OnMouseLeave(e);

        if (previewValue >= 0)
        {
            previewValue = -1;
            UpdateVisualState();
        }
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnMouseLeftButtonDown(e);

        if (IsReadOnly || ItemsHost is null)
        {
            return;
        }

        Focus();

        var position = e.GetPosition(ItemsHost);
        var newValue = CalculateValueFromPosition(position.X);
        SetCurrentValue(ValueProperty, newValue);
        previewValue = -1;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnKeyDown(e);

        if (IsReadOnly)
        {
            return;
        }

        var increment = AllowHalfStars ? 0.5 : 1.0;
        var handled = true;

        switch (e.Key)
        {
            case Key.Left:
            case Key.Down:
                SetCurrentValue(ValueProperty, System.Math.Max(0, Value - increment));
                break;
            case Key.Right:
            case Key.Up:
                SetCurrentValue(ValueProperty, System.Math.Min(Maximum, Value + increment));
                break;
            case Key.Home:
                SetCurrentValue(ValueProperty, 0.0);
                break;
            case Key.End:
                SetCurrentValue(ValueProperty, (double)Maximum);
                break;
            default:
                handled = false;
                break;
        }

        e.Handled = handled;
    }

    protected override AutomationPeer OnCreateAutomationPeer()
        => new RatingAutomationPeer(this);

    protected virtual void OnValueChanged(
        double oldValue,
        double newValue)
        => ValueChanged?.Invoke(this, new RoutedPropertyChangedEventArgs<double>(oldValue, newValue));

    private static void OnValueChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not Rating control)
        {
            return;
        }

        var newValue = (double)e.NewValue;
        var coercedValue = System.Math.Max(0, System.Math.Min(control.Maximum, newValue));

        if (System.Math.Abs(coercedValue - newValue) > double.Epsilon)
        {
            control.SetCurrentValue(ValueProperty, coercedValue);
            return;
        }

        control.UpdateVisualState();
        control.OnValueChanged((double)e.OldValue, newValue);
    }

    private static void OnMaximumChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not Rating control)
        {
            return;
        }

        var newMax = (int)e.NewValue;
        var coercedMax = System.Math.Max(1, System.Math.Min(10, newMax));

        if (coercedMax != newMax)
        {
            control.SetCurrentValue(MaximumProperty, coercedMax);
            return;
        }

        if (control.Value > newMax)
        {
            control.SetCurrentValue(ValueProperty, (double)newMax);
        }

        control.BuildRatingItems();
        control.UpdateVisualState();
    }

    private static void OnIsReadOnlyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is Rating control)
        {
            control.UpdateVisualState();
        }
    }

    private static void OnIconChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is Rating control)
        {
            control.UpdateVisualState();
        }
    }

    private static void OnItemSizeChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is Rating control)
        {
            control.UpdateItemSizes();
        }
    }

    private static void OnItemSpacingChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is Rating control)
        {
            control.UpdateItemSpacing();
        }
    }

    private void BuildRatingItems()
    {
        if (ItemsHost is null)
        {
            return;
        }

        ItemsHost.Children.Clear();
        ratingItems.Clear();

        for (var i = 0; i < Maximum; i++)
        {
            var item = new ContentControl
            {
                Width = ItemSize,
                Height = ItemSize,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Margin = new Thickness(
                    i > 0 ? ItemSpacing / 2 : 0,
                    0,
                    i < Maximum - 1 ? ItemSpacing / 2 : 0,
                    0),
                IsTabStop = false,
            };

            ratingItems.Add(item);
            ItemsHost.Children.Add(item);
        }

        UpdateVisualState();
    }

    private void UpdateVisualState(bool isPreview = false)
    {
        if (ItemsHost is null)
        {
            return;
        }

        var displayValue = isPreview && previewValue >= 0 ? previewValue : Value;
        var isInPreviewMode = isPreview && previewValue >= 0;

        for (var i = 0; i < ratingItems.Count; i++)
        {
            var item = ratingItems[i];
            var itemIndex = i + 1;
            var fillLevel = GetFillLevel(displayValue, itemIndex);

            var icon = fillLevel switch
            {
                FillLevel.Full => FilledIcon,
                FillLevel.Half => HalfFilledIcon,
                _ => EmptyIcon,
            };
            var brush = GetItemBrush(fillLevel, isInPreviewMode);
            item.Content = CreateIconContent(icon, brush, ItemSize);

            item.Opacity = IsReadOnly ? 0.6 : 1.0;
            item.Cursor = IsReadOnly ? Cursors.Arrow : Cursors.Hand;
        }
    }

    private FillLevel GetFillLevel(
        double currentValue,
        int itemIndex)
    {
        if (currentValue >= itemIndex)
        {
            return FillLevel.Full;
        }

        if (AllowHalfStars && currentValue >= itemIndex - 0.5)
        {
            return FillLevel.Half;
        }

        return FillLevel.Empty;
    }

    private Brush GetItemBrush(
        FillLevel fillLevel,
        bool isPreview)
    {
        if (isPreview && PreviewBrush is not null)
        {
            return fillLevel == FillLevel.Empty
                ? (EmptyBrush ?? Brushes.Gray)
                : PreviewBrush;
        }

        return fillLevel switch
        {
            FillLevel.Full or FillLevel.Half => FilledBrush ?? Brushes.Gold,
            _ => EmptyBrush ?? Brushes.Gray,
        };
    }

    private double CalculateValueFromPosition(double x)
    {
        if (ratingItems.Count == 0)
        {
            return 0;
        }

        var totalWidth = ratingItems.Sum(item => item.ActualWidth + item.Margin.Left + item.Margin.Right);
        var itemWidth = totalWidth / Maximum;

        var rawValue = x / itemWidth;

        if (AllowHalfStars)
        {
            rawValue = System.Math.Round(rawValue * 2) / 2;
        }
        else
        {
            rawValue = System.Math.Ceiling(rawValue);
        }

        return System.Math.Max(0, System.Math.Min(Maximum, rawValue));
    }

    private void UpdateItemSizes()
    {
        foreach (var item in ratingItems)
        {
            item.Width = ItemSize;
            item.Height = ItemSize;
        }
    }

    private void UpdateItemSpacing()
    {
        for (var i = 0; i < ratingItems.Count; i++)
        {
            ratingItems[i].Margin = new Thickness(
                i > 0 ? ItemSpacing / 2 : 0,
                0,
                i < ratingItems.Count - 1 ? ItemSpacing / 2 : 0,
                0);
        }
    }

    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "Switch expression with many icon type cases.")]
    private FrameworkElement CreateIconContent(
        object icon,
        Brush brush,
        double size)
        => icon switch
        {
            // SVG path string - check first (before general string)
            string path when path.EndsWith(".svg", StringComparison.OrdinalIgnoreCase) => new SvgImage
            {
                Source = path,
                Width = size,
                Height = size,
                OverrideColor = (brush as SolidColorBrush)?.Color,
            },

            // String content - use TextBlock
            string text => new TextBlock
            {
                Text = text,
                Foreground = brush,
                FontSize = size * 0.8,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            },

            // Pre-rendered ImageSource - use Image
            ImageSource imageSource => new Image
            {
                Source = imageSource,
                Width = size,
                Height = size,
                Stretch = Stretch.Uniform,
            },

            // FontAwesome Solid
            FontAwesomeSolidType solidType => new Image
            {
                Source = ImageAwesomeSolid.CreateDrawingImage(solidType, brush),
                Width = size,
                Height = size,
                Stretch = Stretch.Uniform,
            },

            // FontAwesome Regular
            FontAwesomeRegularType regularType => new Image
            {
                Source = ImageAwesomeRegular.CreateDrawingImage(regularType, brush),
                Width = size,
                Height = size,
                Stretch = Stretch.Uniform,
            },

            // FontAwesome Brand
            FontAwesomeBrandType brandType => new Image
            {
                Source = ImageAwesomeBrand.CreateDrawingImage(brandType, brush),
                Width = size,
                Height = size,
                Stretch = Stretch.Uniform,
            },

            // Material Design
            FontMaterialDesignType materialType => new Image
            {
                Source = ImageMaterialDesign.CreateDrawingImage(materialType, brush),
                Width = size,
                Height = size,
                Stretch = Stretch.Uniform,
            },

            // IcoFont
            IcoFontType icoFontType => new Image
            {
                Source = ImageIcoFont.CreateDrawingImage(icoFontType, brush),
                Width = size,
                Height = size,
                Stretch = Stretch.Uniform,
            },

            // Bootstrap
            FontBootstrapType bootstrapType => new Image
            {
                Source = ImageBootstrap.CreateDrawingImage(bootstrapType, brush),
                Width = size,
                Height = size,
                Stretch = Stretch.Uniform,
            },

            // Weather
            FontWeatherType weatherType => new Image
            {
                Source = ImageWeather.CreateDrawingImage(weatherType, brush),
                Width = size,
                Height = size,
                Stretch = Stretch.Uniform,
            },

            // Fallback - wrap in ContentControl
            _ => new ContentControl
            {
                Content = icon,
                Width = size,
                Height = size,
            },
        };

    private enum FillLevel
    {
        Empty,
        Half,
        Full,
    }
}