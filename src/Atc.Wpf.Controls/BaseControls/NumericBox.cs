// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable InconsistentNaming
// ReSharper disable CommentTypo
namespace Atc.Wpf.Controls.BaseControls;

[SuppressMessage("Performance", "MA0023:Add RegexOptions.ExplicitCapture", Justification = "OK.")]
[TemplatePart(Name = PART_NumericUp, Type = typeof(RepeatButton))]
[TemplatePart(Name = PART_NumericDown, Type = typeof(RepeatButton))]
[TemplatePart(Name = PART_TextBox, Type = typeof(TextBox))]
[StyleTypedProperty(Property = nameof(SpinButtonStyle), StyleTargetType = typeof(ButtonBase))]
public class NumericBox : Control
{
    private const string PART_NumericDown = "PART_NumericDownButton";
    private const string PART_NumericUp = "PART_NumericUpButton";
    private const string PART_TextBox = "PART_TextBox";
    private const string PART_ContentHost = "PART_ContentHost";
    private const double DefaultInterval = 1d;
    private const int DefaultDelay = 500;
    private const string RawRegexNumberString = @"[-+]?(?<![0-9][<DecimalSeparator><GroupSeparator>])[<DecimalSeparator><GroupSeparator>]?[0-9]+(?:[<DecimalSeparator><GroupSeparator>\s][0-9]+)*[<DecimalSeparator><GroupSeparator>]?[0-9]?(?:[eE][-+]?[0-9]+)?(?!\.[0-9])";

    private static readonly Regex RegexStringFormatHexadecimal = new(@"^(?<complexHEX>.*{\d\s*:[Xx]\d*}.*)?(?<simpleHEX>[Xx]\d*)?$", RegexOptions.Compiled, TimeSpan.FromSeconds(1));
    private static readonly Regex RegexHexadecimal = new(@"^([a-fA-F0-9]{1,2}\s?)+$", RegexOptions.Compiled, TimeSpan.FromSeconds(1));
    private static readonly Regex RegexStringFormat = new(@"\{0\s*(:(?<format>.*))?\}", RegexOptions.Compiled, TimeSpan.FromSeconds(1));

    private Regex? regexNumber;
    private Lazy<PropertyInfo?> handlesMouseWheelScrolling = new();
    private double internalIntervalMultiplierForCalculation = DefaultInterval;
    private double internalLargeChange = DefaultInterval * 100;
    private double intervalValueSinceReset;
    private bool manualChange;
    private RepeatButton? repeatDown;
    private RepeatButton? repeatUp;
    private TextBox? valueTextBox;
    private ScrollViewer? scrollViewer;

    public static readonly RoutedEvent ValueIncrementedEvent = EventManager.RegisterRoutedEvent(
        nameof(ValueIncremented),
        RoutingStrategy.Bubble,
        typeof(NumericBoxChangedRoutedEventHandler),
        typeof(NumericBox));

    public event NumericBoxChangedRoutedEventHandler ValueIncremented
    {
        add => AddHandler(ValueIncrementedEvent, value);
        remove => RemoveHandler(ValueIncrementedEvent, value);
    }

    public static readonly RoutedEvent ValueDecrementedEvent = EventManager.RegisterRoutedEvent(
        nameof(ValueDecremented),
        RoutingStrategy.Bubble,
        typeof(NumericBoxChangedRoutedEventHandler),
        typeof(NumericBox));

    public event NumericBoxChangedRoutedEventHandler ValueDecremented
    {
        add => AddHandler(ValueDecrementedEvent, value);
        remove => RemoveHandler(ValueDecrementedEvent, value);
    }

    public static readonly RoutedEvent DelayChangedEvent = EventManager.RegisterRoutedEvent(
        nameof(DelayChanged),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(NumericBox));

    public event RoutedEventHandler DelayChanged
    {
        add => AddHandler(DelayChangedEvent, value);
        remove => RemoveHandler(DelayChangedEvent, value);
    }

    public static readonly RoutedEvent MaximumReachedEvent = EventManager.RegisterRoutedEvent(
        nameof(MaximumReached),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(NumericBox));

    public event RoutedEventHandler MaximumReached
    {
        add => AddHandler(MaximumReachedEvent, value);
        remove => RemoveHandler(MaximumReachedEvent, value);
    }

    public static readonly RoutedEvent MinimumReachedEvent = EventManager.RegisterRoutedEvent(
        nameof(MinimumReached),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(NumericBox));

    public event RoutedEventHandler MinimumReached
    {
        add => AddHandler(MinimumReachedEvent, value);
        remove => RemoveHandler(MinimumReachedEvent, value);
    }

    public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(
        nameof(ValueChanged),
        RoutingStrategy.Bubble,
        typeof(RoutedPropertyChangedEventHandler<double?>),
        typeof(NumericBox));

    public event RoutedPropertyChangedEventHandler<double?> ValueChanged
    {
        add => AddHandler(ValueChangedEvent, value);
        remove => RemoveHandler(ValueChangedEvent, value);
    }

    public static readonly DependencyProperty DelayProperty = DependencyProperty.Register(
        nameof(Delay),
        typeof(int),
        typeof(NumericBox),
        new FrameworkPropertyMetadata(
            DefaultDelay,
            OnDelayPropertyChanged),
        value => Convert.ToInt32(value, GlobalizationConstants.EnglishCultureInfo) >= 0);

    public int Delay
    {
        get => (int)GetValue(DelayProperty);
        set => SetValue(DelayProperty, value);
    }

    public static readonly DependencyProperty TextAlignmentProperty = TextBox.TextAlignmentProperty.AddOwner(typeof(NumericBox));

    public TextAlignment TextAlignment
    {
        get => (TextAlignment)GetValue(TextAlignmentProperty);
        set => SetValue(TextAlignmentProperty, value);
    }

    public static readonly DependencyProperty SpeedupProperty = DependencyProperty.Register(
        nameof(Speedup),
        typeof(bool),
        typeof(NumericBox),
        new FrameworkPropertyMetadata(
            BooleanBoxes.TrueBox,
            OnSpeedupPropertyChanged));

    public bool Speedup
    {
        get => (bool)GetValue(SpeedupProperty);
        set => SetValue(SpeedupProperty, BooleanBoxes.Box(value));
    }

    public static readonly DependencyProperty IsReadOnlyProperty = TextBoxBase.IsReadOnlyProperty.AddOwner(
        typeof(NumericBox),
        new FrameworkPropertyMetadata(
            BooleanBoxes.FalseBox,
            FrameworkPropertyMetadataOptions.Inherits,
            OnIsReadOnlyPropertyChanged));

    public bool IsReadOnly
    {
        get => (bool)GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, BooleanBoxes.Box(value));
    }

    public static readonly DependencyProperty StringFormatProperty = DependencyProperty.Register(
        nameof(StringFormat),
        typeof(string),
        typeof(NumericBox),
        new FrameworkPropertyMetadata(
            string.Empty,
            OnStringFormatPropertyChanged,
            CoerceStringFormat));

    public string StringFormat
    {
        get => (string)GetValue(StringFormatProperty);
        set => SetValue(StringFormatProperty, value);
    }

    public static readonly DependencyProperty InterceptArrowKeysProperty = DependencyProperty.Register(
        nameof(InterceptArrowKeys),
        typeof(bool),
        typeof(NumericBox),
        new FrameworkPropertyMetadata(BooleanBoxes.TrueBox));

    public bool InterceptArrowKeys
    {
        get => (bool)GetValue(InterceptArrowKeysProperty);
        set => SetValue(InterceptArrowKeysProperty, BooleanBoxes.Box(value));
    }

    public static readonly DependencyProperty InterceptMouseWheelProperty = DependencyProperty.Register(
        nameof(InterceptMouseWheel),
        typeof(bool),
        typeof(NumericBox),
        new FrameworkPropertyMetadata(BooleanBoxes.TrueBox));

    public bool InterceptMouseWheel
    {
        get => (bool)GetValue(InterceptMouseWheelProperty);
        set => SetValue(InterceptMouseWheelProperty, BooleanBoxes.Box(value));
    }

    public static readonly DependencyProperty InterceptManualEnterProperty = DependencyProperty.Register(
        nameof(InterceptManualEnter),
        typeof(bool),
        typeof(NumericBox),
        new PropertyMetadata(
            BooleanBoxes.TrueBox,
            OnInterceptManualEnterPropertyChanged));

    public bool InterceptManualEnter
    {
        get => (bool)GetValue(InterceptManualEnterProperty);
        set => SetValue(InterceptManualEnterProperty, BooleanBoxes.Box(value));
    }

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value),
        typeof(double?),
        typeof(NumericBox),
        new FrameworkPropertyMetadata(
            default(double?),
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            OnValuePropertyChanged,
            (o, value) => CoerceValue(o, value).Item1));

    [SuppressMessage("Naming", "CA1721:Property names should not match get methods", Justification = "OK.")]
    public double? Value
    {
        get => (double?)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public static readonly DependencyProperty DefaultValueProperty = DependencyProperty.Register(
        nameof(DefaultValue),
        typeof(double?),
        typeof(NumericBox),
        new PropertyMetadata(
            defaultValue: null,
            OnDefaultValuePropertyChanged,
            CoerceDefaultValue));

    public double? DefaultValue
    {
        get => (double?)GetValue(DefaultValueProperty);
        set => SetValue(DefaultValueProperty, value);
    }

    public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
        nameof(Minimum),
        typeof(double),
        typeof(NumericBox),
        new FrameworkPropertyMetadata(
            double.MinValue,
            OnMinimumPropertyChanged));

    public double Minimum
    {
        get => (double)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
        nameof(Maximum),
        typeof(double),
        typeof(NumericBox),
        new FrameworkPropertyMetadata(
            double.MaxValue,
            OnMaximumPropertyChanged,
            CoerceMaximum));

    public double Maximum
    {
        get => (double)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public static readonly DependencyProperty IntervalProperty = DependencyProperty.Register(
        nameof(Interval),
        typeof(double),
        typeof(NumericBox),
        new FrameworkPropertyMetadata(
            DefaultInterval,
            OnIntervalPropertyChanged));

    public double Interval
    {
        get => (double)GetValue(IntervalProperty);
        set => SetValue(IntervalProperty, value);
    }

    public static readonly DependencyProperty TrackMouseWheelWhenMouseOverProperty = DependencyProperty.Register(
        nameof(TrackMouseWheelWhenMouseOver),
        typeof(bool),
        typeof(NumericBox),
        new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

    public bool TrackMouseWheelWhenMouseOver
    {
        get => (bool)GetValue(TrackMouseWheelWhenMouseOverProperty);
        set => SetValue(TrackMouseWheelWhenMouseOverProperty, BooleanBoxes.Box(value));
    }

    public static readonly DependencyProperty SpinButtonStyleProperty = DependencyProperty.Register(
        nameof(SpinButtonStyle),
        typeof(Style),
        typeof(NumericBox),
        new PropertyMetadata(propertyChangedCallback: null));

    public Style? SpinButtonStyle
    {
        get => (Style?)GetValue(SpinButtonStyleProperty);
        set => SetValue(SpinButtonStyleProperty, value);
    }

    public static readonly DependencyProperty ButtonsAlignmentProperty = DependencyProperty.Register(
        nameof(ButtonsAlignment),
        typeof(ButtonsAlignment),
        typeof(NumericBox),
        new FrameworkPropertyMetadata(
            ButtonsAlignment.Right,
            FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

    public ButtonsAlignment ButtonsAlignment
    {
        get => (ButtonsAlignment)GetValue(ButtonsAlignmentProperty);
        set => SetValue(ButtonsAlignmentProperty, value);
    }

    public static readonly DependencyProperty HideUpDownButtonsProperty = DependencyProperty.Register(
        nameof(HideUpDownButtons),
        typeof(bool),
        typeof(NumericBox),
        new PropertyMetadata(BooleanBoxes.FalseBox));

    public bool HideUpDownButtons
    {
        get => (bool)GetValue(HideUpDownButtonsProperty);
        set => SetValue(HideUpDownButtonsProperty, BooleanBoxes.Box(value));
    }

    public static readonly DependencyProperty UpDownButtonsMarginProperty = DependencyProperty.Register(
        nameof(UpDownButtonsMargin),
        typeof(Thickness),
        typeof(NumericBox),
        new PropertyMetadata(new Thickness(0, -0.5, -0.5, 0)));

    public Thickness UpDownButtonsMargin
    {
        get => (Thickness)GetValue(UpDownButtonsMarginProperty);
        set => SetValue(UpDownButtonsMarginProperty, value);
    }

    public static readonly DependencyProperty UpDownButtonsWidthProperty = DependencyProperty.Register(
        nameof(UpDownButtonsWidth),
        typeof(double),
        typeof(NumericBox),
        new PropertyMetadata(20d));

    public double UpDownButtonsWidth
    {
        get => (double)GetValue(UpDownButtonsWidthProperty);
        set => SetValue(UpDownButtonsWidthProperty, value);
    }

    public static readonly DependencyProperty UpDownButtonsFocusableProperty = DependencyProperty.Register(
        nameof(UpDownButtonsFocusable),
        typeof(bool),
        typeof(NumericBox),
        new PropertyMetadata(BooleanBoxes.TrueBox));

    public bool UpDownButtonsFocusable
    {
        get => (bool)GetValue(UpDownButtonsFocusableProperty);
        set => SetValue(UpDownButtonsFocusableProperty, BooleanBoxes.Box(value));
    }

    public static readonly DependencyProperty SwitchUpDownButtonsProperty = DependencyProperty.Register(
        nameof(SwitchUpDownButtons),
        typeof(bool),
        typeof(NumericBox),
        new PropertyMetadata(BooleanBoxes.FalseBox));

    public bool SwitchUpDownButtons
    {
        get => (bool)GetValue(SwitchUpDownButtonsProperty);
        set => SetValue(SwitchUpDownButtonsProperty, BooleanBoxes.Box(value));
    }

    public static readonly DependencyProperty ButtonUpContentProperty = DependencyProperty.Register(
        nameof(ButtonUpContent),
        typeof(object),
        typeof(NumericBox),
        new FrameworkPropertyMetadata(propertyChangedCallback: null));

    public object? ButtonUpContent
    {
        get => GetValue(ButtonUpContentProperty);
        set => SetValue(ButtonUpContentProperty, value);
    }

    public static readonly DependencyProperty ButtonUpContentTemplateProperty = DependencyProperty.Register(
        nameof(ButtonUpContentTemplate),
        typeof(DataTemplate),
        typeof(NumericBox));

    public DataTemplate? ButtonUpContentTemplate
    {
        get => (DataTemplate?)GetValue(ButtonUpContentTemplateProperty);
        set => SetValue(ButtonUpContentTemplateProperty, value);
    }

    public static readonly DependencyProperty ButtonUpContentStringFormatProperty = DependencyProperty.Register(
        nameof(ButtonUpContentStringFormat),
        typeof(string),
        typeof(NumericBox),
        new FrameworkPropertyMetadata(propertyChangedCallback: null));

    public string? ButtonUpContentStringFormat
    {
        get => (string?)GetValue(ButtonUpContentStringFormatProperty);
        set => SetValue(ButtonUpContentStringFormatProperty, value);
    }

    public static readonly DependencyProperty ButtonDownContentProperty = DependencyProperty.Register(
        nameof(ButtonDownContent),
        typeof(object),
        typeof(NumericBox),
        new FrameworkPropertyMetadata(propertyChangedCallback: null));

    public object? ButtonDownContent
    {
        get => GetValue(ButtonDownContentProperty);
        set => SetValue(ButtonDownContentProperty, value);
    }

    public static readonly DependencyProperty ButtonDownContentTemplateProperty = DependencyProperty.Register(
        nameof(ButtonDownContentTemplate),
        typeof(DataTemplate),
        typeof(NumericBox));

    public DataTemplate? ButtonDownContentTemplate
    {
        get => (DataTemplate?)GetValue(ButtonDownContentTemplateProperty);
        set => SetValue(ButtonDownContentTemplateProperty, value);
    }

    public static readonly DependencyProperty ButtonDownContentStringFormatProperty = DependencyProperty.Register(
        nameof(ButtonDownContentStringFormat),
        typeof(string),
        typeof(NumericBox),
        new FrameworkPropertyMetadata(propertyChangedCallback: null));

    public string? ButtonDownContentStringFormat
    {
        get => (string?)GetValue(ButtonDownContentStringFormatProperty);
        set => SetValue(ButtonDownContentStringFormatProperty, value);
    }

    public static readonly DependencyProperty CultureProperty = DependencyProperty.Register(
        nameof(Culture),
        typeof(CultureInfo),
        typeof(NumericBox),
        new PropertyMetadata(
            defaultValue: null,
            OnCulturePropertyChanged));

    public CultureInfo? Culture
    {
        get => (CultureInfo?)GetValue(CultureProperty);
        set => SetValue(CultureProperty, value);
    }

    public static readonly DependencyProperty NumericInputModeProperty = DependencyProperty.Register(
        nameof(NumericInputMode),
        typeof(NumericInput),
        typeof(NumericBox),
        new FrameworkPropertyMetadata(
            NumericInput.All, OnNumericInputModePropertyChanged));

    public NumericInput NumericInputMode
    {
        get => (NumericInput)GetValue(NumericInputModeProperty);
        set => SetValue(NumericInputModeProperty, value);
    }

    public static readonly DependencyProperty DecimalPointCorrectionProperty = DependencyProperty.Register(
        nameof(DecimalPointCorrection),
        typeof(DecimalPointCorrectionMode),
        typeof(NumericBox),
        new PropertyMetadata(default(DecimalPointCorrectionMode)));

    public DecimalPointCorrectionMode DecimalPointCorrection
    {
        get => (DecimalPointCorrectionMode)GetValue(DecimalPointCorrectionProperty);
        set => SetValue(DecimalPointCorrectionProperty, value);
    }

    public static readonly DependencyProperty SnapToMultipleOfIntervalProperty = DependencyProperty.Register(
        nameof(SnapToMultipleOfInterval),
        typeof(bool),
        typeof(NumericBox),
        new PropertyMetadata(
            BooleanBoxes.FalseBox,
            OnSnapToMultipleOfIntervalPropertyChanged));

    public bool SnapToMultipleOfInterval
    {
        get => (bool)GetValue(SnapToMultipleOfIntervalProperty);
        set => SetValue(SnapToMultipleOfIntervalProperty, BooleanBoxes.Box(value));
    }

    public static readonly DependencyProperty ParsingNumberStyleProperty = DependencyProperty.Register(
        nameof(ParsingNumberStyle),
        typeof(NumberStyles),
        typeof(NumericBox),
        new PropertyMetadata(NumberStyles.Any));

    public NumberStyles ParsingNumberStyle
    {
        get => (NumberStyles)GetValue(ParsingNumberStyleProperty);
        set => SetValue(ParsingNumberStyleProperty, value);
    }

    public static readonly DependencyProperty PrefixTextProperty = DependencyProperty.Register(
        nameof(PrefixText),
        typeof(string),
        typeof(NumericBox),
        new FrameworkPropertyMetadata(
            string.Empty,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));

    public string PrefixText
    {
        get => (string)GetValue(PrefixTextProperty);
        set => SetValue(PrefixTextProperty, value);
    }

    public static readonly DependencyProperty SuffixTextProperty = DependencyProperty.Register(
        nameof(SuffixText),
        typeof(string),
        typeof(NumericBox),
        new FrameworkPropertyMetadata(
            string.Empty,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));

    public string SuffixText
    {
        get => (string)GetValue(SuffixTextProperty);
        set => SetValue(SuffixTextProperty, value);
    }

    private CultureInfo SpecificCultureInfo => Culture ?? Language.GetSpecificCulture();

    static NumericBox()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericBox), new FrameworkPropertyMetadata(typeof(NumericBox)));

        VerticalContentAlignmentProperty.OverrideMetadata(typeof(NumericBox), new FrameworkPropertyMetadata(VerticalAlignment.Center));
        HorizontalContentAlignmentProperty.OverrideMetadata(typeof(NumericBox), new FrameworkPropertyMetadata(HorizontalAlignment.Right));

        EventManager.RegisterClassHandler(typeof(NumericBox), GotFocusEvent, new RoutedEventHandler(OnGotFocus));
    }

    public NumericBox()
    {
        CultureManager.UiCultureChanged += OnUiCultureChanged;
    }

    private void OnUiCultureChanged(
        object? sender,
        UiCultureEventArgs e)
    {
        Culture = Thread.CurrentThread.CurrentUICulture;
    }

    private static void OnDelayPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue == e.NewValue ||
            e.OldValue is not int oldDelay ||
            e.NewValue is not int newDelay ||
            d is not NumericBox numericBox)
        {
            return;
        }

        numericBox.RaiseChangeDelay();
        numericBox.OnDelayChanged(oldDelay, newDelay);
    }

    private static void OnSpeedupPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue == e.NewValue)
        {
            return;
        }

        (d as NumericBox)?.OnSpeedupChanged((bool)e.OldValue, (bool)e.NewValue);
    }

    private static void OnIsReadOnlyPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue == e.NewValue ||
            e.NewValue is not bool isReadOnly)
        {
            return;
        }

        if (d is not NumericBox numericBox)
        {
            return;
        }

        numericBox.ToggleReadOnlyMode(isReadOnly);
    }

    private static void OnStringFormatPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue == e.NewValue ||
            d is not NumericBox numericBox)
        {
            return;
        }

        if (numericBox.valueTextBox is not null &&
            numericBox.Value.HasValue)
        {
            numericBox.InternalSetText(numericBox.Value);
        }

        if (e.NewValue is not string format ||
            string.IsNullOrEmpty(format) ||
            !RegexStringFormatHexadecimal.IsMatch(format))
        {
            return;
        }

        numericBox.SetCurrentValue(ParsingNumberStyleProperty, NumberStyles.HexNumber);
        numericBox.SetCurrentValue(NumericInputModeProperty, numericBox.NumericInputMode | NumericInput.Decimal);
    }

    private static object CoerceStringFormat(
        DependencyObject d,
        object? baseValue)
        => baseValue ?? string.Empty;

    private static void OnInterceptManualEnterPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue == e.NewValue)
        {
            return;
        }

        if (d is not NumericBox numericBox)
        {
            return;
        }

        numericBox.ToggleReadOnlyMode(numericBox.IsReadOnly);
    }

    private static void OnValuePropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue == e.NewValue)
        {
            return;
        }

        (d as NumericBox)?.OnValueChanged((double?)e.OldValue, (double?)e.NewValue);
    }

    private static Tuple<double?, bool> CoerceValue(
        DependencyObject d,
        object? baseValue)
    {
        var numericBox = (NumericBox)d;
        if (baseValue is null)
        {
            return new Tuple<double?, bool>(numericBox.DefaultValue, item2: false);
        }

        var value = ((double?)baseValue).Value;

        if (!numericBox.NumericInputMode.HasFlag(NumericInput.Decimal))
        {
            value = System.Math.Truncate(value);
        }

        if (value < numericBox.Minimum)
        {
            return new Tuple<double?, bool>(numericBox.Minimum, item2: true);
        }

        if (value > numericBox.Maximum)
        {
            return new Tuple<double?, bool>(numericBox.Maximum, item2: true);
        }

        return new Tuple<double?, bool>(value, item2: false);
    }

    private static void OnDefaultValuePropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var numericBox = (NumericBox)d;

        if (!numericBox.Value.HasValue && numericBox.DefaultValue.HasValue)
        {
            numericBox.SetValueTo(numericBox.DefaultValue.Value);
        }
    }

    private static object? CoerceDefaultValue(
        DependencyObject d,
        object? baseValue)
    {
        if (baseValue is not double val)
        {
            return baseValue;
        }

        var minimum = ((NumericBox)d).Minimum;
        var maximum = ((NumericBox)d).Maximum;

        if (val < minimum)
        {
            return minimum;
        }

        if (val > maximum)
        {
            return maximum;
        }

        return baseValue;
    }

    private static void OnMinimumPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var numericBox = (NumericBox)d;

        numericBox.CoerceValue(MaximumProperty);
        numericBox.CoerceValue(ValueProperty);
        numericBox.CoerceValue(DefaultValueProperty);
        numericBox.OnMinimumChanged((double)e.OldValue, (double)e.NewValue);
        numericBox.EnableDisableUpDown();
    }

    private static void OnMaximumPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var numericBox = (NumericBox)d;

        numericBox.CoerceValue(ValueProperty);
        numericBox.CoerceValue(DefaultValueProperty);
        numericBox.OnMaximumChanged((double)e.OldValue, (double)e.NewValue);
        numericBox.EnableDisableUpDown();
    }

    private static object CoerceMaximum(
        DependencyObject d,
        object value)
    {
        var minimum = ((NumericBox)d).Minimum;
        var val = (double)value;
        return val < minimum
            ? minimum
            : val;
    }

    private static void OnIntervalPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => (d as NumericBox)?.ResetInternal();

    private static void OnCulturePropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue == e.OldValue ||
            d is not NumericBox numericBox)
        {
            return;
        }

        numericBox.regexNumber = null;
        numericBox.OnValueChanged(numericBox.Value, numericBox.Value);
    }

    private static void OnNumericInputModePropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue == e.NewValue ||
            e.NewValue is not NumericInput numericInput ||
            d is not NumericBox numericBox ||
            numericBox.Value is null)
        {
            return;
        }

        if (!numericInput.HasFlag(NumericInput.Decimal))
        {
            numericBox.Value = System.Math.Truncate(numericBox.Value.GetValueOrDefault());
        }
    }

    private static void OnSnapToMultipleOfIntervalPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue == e.NewValue ||
            e.NewValue is not bool snap ||
            d is not NumericBox numericBox)
        {
            return;
        }

        if (!snap)
        {
            return;
        }

        if (System.Math.Abs(numericBox.Interval) <= 0)
        {
            return;
        }

        var value = numericBox.Value.GetValueOrDefault();
        numericBox.Value = System.Math.Round(value / numericBox.Interval) * numericBox.Interval;
    }

    private static void OnGotFocus(
        object sender,
        RoutedEventArgs e)
    {
        if (e.Handled)
        {
            return;
        }

        var numericBox = (NumericBox)sender;
        if (numericBox is { InterceptManualEnter: false, IsReadOnly: false } ||
            !numericBox.Focusable ||
            !Equals(e.OriginalSource, numericBox))
        {
            return;
        }

        var request = new TraversalRequest((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift ? FocusNavigationDirection.Previous : FocusNavigationDirection.Next);

        if (Keyboard.FocusedElement is UIElement elementWithFocus)
        {
            elementWithFocus.MoveFocus(request);
        }
        else
        {
            numericBox.Focus();
        }

        e.Handled = true;
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        repeatUp = GetTemplateChild(PART_NumericUp) as RepeatButton;
        repeatDown = GetTemplateChild(PART_NumericDown) as RepeatButton;
        valueTextBox = GetTemplateChild(PART_TextBox) as TextBox;

        if (repeatUp is null || repeatDown is null || valueTextBox is null)
        {
            throw new InvalidOperationException($"You have missed to specify {PART_NumericUp}, {PART_NumericDown} or {PART_TextBox} in your template!");
        }

        ToggleReadOnlyMode(IsReadOnly);

        repeatUp.Click += (_, _) => { ChangeValueWithSpeedUp(toPositive: true); };
        repeatDown.Click += (_, _) => { ChangeValueWithSpeedUp(toPositive: false); };

        repeatUp.PreviewMouseUp += (_, _) => ResetInternal();
        repeatDown.PreviewMouseUp += (_, _) => ResetInternal();

        OnValueChanged(Value, Value);

        scrollViewer = null;
    }

    private void ToggleReadOnlyMode(
        bool isReadOnly)
    {
        if (repeatUp is null || repeatDown is null || valueTextBox is null)
        {
            return;
        }

        if (isReadOnly)
        {
            valueTextBox.LostFocus -= OnTextBoxLostFocus;
            valueTextBox.PreviewTextInput -= OnPreviewTextInput;
            valueTextBox.PreviewKeyDown -= OnTextBoxKeyDown;
            valueTextBox.TextChanged -= OnTextChanged;
            DataObject.RemovePastingHandler(valueTextBox, OnValueTextBoxPaste);
        }
        else
        {
            valueTextBox.LostFocus += OnTextBoxLostFocus;
            valueTextBox.PreviewTextInput += OnPreviewTextInput;
            valueTextBox.PreviewKeyDown += OnTextBoxKeyDown;
            valueTextBox.TextChanged += OnTextChanged;
            DataObject.AddPastingHandler(valueTextBox, OnValueTextBoxPaste);
        }
    }

    public void SelectAll()
    {
        valueTextBox?.SelectAll();
    }

    private void RaiseChangeDelay()
    {
        RaiseEvent(new RoutedEventArgs(DelayChangedEvent));
    }

    protected virtual void OnDelayChanged(
        int oldDelay,
        int newDelay)
    {
        // Skip
    }

    protected virtual void OnSpeedupChanged(
        bool oldSpeedup,
        bool newSpeedup)
    {
        // Skip
    }

    protected virtual void OnMaximumChanged(
        double oldMaximum,
        double newMaximum)
    {
        // Skip
    }

    protected virtual void OnMinimumChanged(
        double oldMinimum,
        double newMinimum)
    {
        // Skip
    }

    protected override void OnPreviewKeyDown(
        KeyEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnPreviewKeyDown(e);

        if (!InterceptArrowKeys)
        {
            return;
        }

        switch (e.Key)
        {
            case Key.Up:
                ChangeValueWithSpeedUp(toPositive: true);
                e.Handled = true;
                break;
            case Key.Down:
                ChangeValueWithSpeedUp(toPositive: false);
                e.Handled = true;
                break;
        }
    }

    protected override void OnPreviewKeyUp(
        KeyEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnPreviewKeyUp(e);

        if (e.Key is Key.Down or Key.Up)
        {
            ResetInternal();
        }
    }

    protected override void OnPreviewMouseWheel(
        MouseWheelEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnPreviewMouseWheel(e);

        if (InterceptMouseWheel && (IsFocused || valueTextBox?.IsFocused == true || TrackMouseWheelWhenMouseOver))
        {
            var increment = e.Delta > 0;
            ChangeValueWithSpeedUp(increment);
        }

        var sv = TryFindScrollViewer();

        if (sv is not null && handlesMouseWheelScrolling.Value is not null)
        {
            if (TrackMouseWheelWhenMouseOver)
            {
                handlesMouseWheelScrolling.Value.SetValue(sv, value: true, index: null);
            }
            else if (InterceptMouseWheel)
            {
                handlesMouseWheelScrolling.Value.SetValue(sv, valueTextBox?.IsFocused == true, index: null);
            }
            else
            {
                handlesMouseWheelScrolling.Value.SetValue(sv, value: true, index: null);
            }
        }
    }

    [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers", Justification = "OK.")]
    protected void OnPreviewTextInput(
        object sender,
        TextCompositionEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(sender);
        ArgumentNullException.ThrowIfNull(e);

        var textBox = (TextBox)sender;
        var fullText = textBox.Text
            .Remove(textBox.SelectionStart, textBox.SelectionLength)
            .Insert(textBox.CaretIndex, e.Text);
        var textIsValid = ValidateText(fullText, out var convertedValue);

        e.Handled = !textIsValid || CoerceValue(this, convertedValue as double?).Item2;
        manualChange = !e.Handled;
    }

    [SuppressMessage("Major Code Smell", "S2589:Boolean expressions should not be gratuitous", Justification = "OK.")]
    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
    protected virtual void OnValueChanged(
        double? oldValue,
        double? newValue)
    {
        if (!manualChange)
        {
            if (!newValue.HasValue)
            {
                if (valueTextBox is not null)
                {
                    valueTextBox.Text = string.Empty;
                }

                if (!Equals(oldValue, newValue))
                {
                    RaiseEvent(new RoutedPropertyChangedEventArgs<double?>(oldValue, newValue, ValueChangedEvent));
                }

                return;
            }

            if (repeatUp is { IsEnabled: false })
            {
                repeatUp.IsEnabled = true;
            }

            if (repeatDown is { IsEnabled: false })
            {
                repeatDown.IsEnabled = true;
            }

            if (newValue <= Minimum)
            {
                if (repeatDown is not null)
                {
                    repeatDown.IsEnabled = false;
                }

                ResetInternal();

                if (IsLoaded)
                {
                    RaiseEvent(new RoutedEventArgs(MinimumReachedEvent));
                }
            }

            if (newValue >= Maximum)
            {
                if (repeatUp is not null)
                {
                    repeatUp.IsEnabled = false;
                }

                ResetInternal();
                if (IsLoaded)
                {
                    RaiseEvent(new RoutedEventArgs(MaximumReachedEvent));
                }
            }

            if (valueTextBox is not null)
            {
                InternalSetText(newValue);
            }
        }

        if (!Equals(oldValue, newValue))
        {
            RaiseEvent(new RoutedPropertyChangedEventArgs<double?>(oldValue, newValue, ValueChangedEvent));
        }
    }

    private void InternalSetText(
        double? newValue)
    {
        if (!newValue.HasValue)
        {
            if (valueTextBox is not null)
            {
                valueTextBox.Text = string.Empty;
            }

            return;
        }

        if (valueTextBox is not null)
        {
            valueTextBox.Text = FormattedValueString(newValue.Value, StringFormat, SpecificCultureInfo);
        }
    }

    private static string FormattedValueString(
        double newValue,
        string format,
        CultureInfo culture)
    {
        format = format.Replace("{}", string.Empty, StringComparison.Ordinal);
        if (string.IsNullOrWhiteSpace(format))
        {
            return newValue.ToString(culture);
        }

        if (TryFormatHexadecimal(newValue, format, culture, out var hexValue))
        {
            return hexValue;
        }

        var match = RegexStringFormat.Match(format);
        return match.Success
            ? string.Format(culture, format, newValue)
            : newValue.ToString(format, culture);
    }

    private static double FormattedValue(
        double newValue,
        string format,
        CultureInfo culture)
    {
        format = format.Replace("{}", string.Empty, StringComparison.Ordinal);
        if (string.IsNullOrWhiteSpace(format))
        {
            return newValue;
        }

        if (TryFormatHexadecimal(newValue, format, culture, out _))
        {
            return newValue;
        }

        var match = RegexStringFormat.Match(format);
        return ConvertStringFormatValue(newValue, match.Success ? match.Groups["format"].Value : format);
    }

    private static double ConvertStringFormatValue(
        double value,
        string format)
    {
        if (format.Contains('P', StringComparison.Ordinal) ||
            format.Contains('%', StringComparison.Ordinal))
        {
            value /= 100d;
        }
        else if (format.Contains('‰', StringComparison.Ordinal))
        {
            value /= 1000d;
        }

        return value;
    }

    private static bool TryFormatHexadecimal(
        double newValue,
        string format,
        CultureInfo culture,
        [NotNullWhen(true)] out string? output)
    {
        var match = RegexStringFormatHexadecimal.Match(format);
        if (match.Success)
        {
            if (match.Groups["simpleHEX"].Success)
            {
                // HEX DOES SUPPORT INT ONLY.
                output = ((int)newValue).ToString(match.Groups["simpleHEX"].Value, culture);
                return true;
            }

            if (match.Groups["complexHEX"].Success)
            {
                output = string.Format(culture, match.Groups["complexHEX"].Value, (int)newValue);
                return true;
            }
        }

        output = null;
        return false;
    }

    private ScrollViewer? TryFindScrollViewer()
    {
        if (scrollViewer is not null)
        {
            return scrollViewer;
        }

        valueTextBox?.ApplyTemplate();

        scrollViewer = valueTextBox?.Template.FindName(PART_ContentHost, valueTextBox) as ScrollViewer;
        if (scrollViewer is not null)
        {
            handlesMouseWheelScrolling = new Lazy<PropertyInfo?>(() => scrollViewer.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance).SingleOrDefault(i => i.Name == "HandlesMouseWheelScrolling"));
        }

        return scrollViewer;
    }

    private void ChangeValueWithSpeedUp(
        bool toPositive)
    {
        if (IsReadOnly)
        {
            return;
        }

        double direction = toPositive ? 1 : -1;

        if (Speedup)
        {
            var d = Interval * internalLargeChange;
            if ((intervalValueSinceReset += Interval * internalIntervalMultiplierForCalculation) > d)
            {
                internalLargeChange *= 10;
                internalIntervalMultiplierForCalculation *= 10;
            }

            ChangeValueInternal(direction * internalIntervalMultiplierForCalculation);
        }
        else
        {
            ChangeValueInternal(direction * Interval);
        }
    }

    private void ChangeValueInternal(
        double interval)
    {
        if (IsReadOnly)
        {
            return;
        }

        manualChange = false;

        var routedEvent = interval > 0 ? new NumericBoxChangedRoutedEventArgs(ValueIncrementedEvent, interval) : new NumericBoxChangedRoutedEventArgs(ValueDecrementedEvent, interval);

        RaiseEvent(routedEvent);

        if (routedEvent.Handled)
        {
            return;
        }

        ChangeValueBy(routedEvent.Interval);

        InternalSetText(Value);

        if (valueTextBox is not null)
        {
            valueTextBox.CaretIndex = valueTextBox.Text.Length;
        }
    }

    private void ChangeValueBy(
        double difference)
    {
        var newValue = Value.GetValueOrDefault() + difference;
        SetValueTo(newValue);
    }

    private void SetValueTo(
        double newValue)
    {
        var value = newValue;

        if (SnapToMultipleOfInterval && System.Math.Abs(Interval) > 0)
        {
            value = System.Math.Round(newValue / Interval) * Interval;
        }

        if (value > Maximum)
        {
            value = Maximum;
        }
        else if (value < Minimum)
        {
            value = Minimum;
        }

        SetCurrentValue(ValueProperty, CoerceValue(this, value).Item1);
    }

    private void EnableDisableDown()
    {
        if (repeatDown is not null)
        {
            repeatDown.IsEnabled = Value is null || Value > Minimum;
        }
    }

    private void EnableDisableUp()
    {
        if (repeatUp is not null)
        {
            repeatUp.IsEnabled = Value is null || Value < Maximum;
        }
    }

    private void EnableDisableUpDown()
    {
        EnableDisableUp();
        EnableDisableDown();
    }

    private void OnTextBoxKeyDown(
        object sender,
        KeyEventArgs e)
    {
        manualChange = manualChange || e.Key is
            Key.Back or
            Key.Delete or
            Key.Decimal or
            Key.OemComma or
            Key.OemPeriod;

        // Filter the Numpad's decimal-point key only
        if (e.Key != Key.Decimal ||
            DecimalPointCorrection == DecimalPointCorrectionMode.Inherits)
        {
            return;
        }

        // Mark the event as handled, so no further action will take place
        e.Handled = true;

        // Grab the originating TextBox control...
        var textBox = (TextBoxBase)sender;

        // The current correction mode...
        var correctionMode = DecimalPointCorrection;

        // And the culture of the NUD
        var culture = SpecificCultureInfo;

        // Surrogate the blocked key pressed
        SimulateDecimalPointKeyPress(textBox, correctionMode, culture);
    }

    private static void SimulateDecimalPointKeyPress(
        TextBoxBase textBox,
        DecimalPointCorrectionMode mode,
        CultureInfo culture)
    {
        var replace = mode switch
        {
            DecimalPointCorrectionMode.Number => culture.NumberFormat.NumberDecimalSeparator,
            DecimalPointCorrectionMode.Currency => culture.NumberFormat.CurrencyDecimalSeparator,
            DecimalPointCorrectionMode.Percent => culture.NumberFormat.PercentDecimalSeparator,
            _ => null,
        };

        if (string.IsNullOrEmpty(replace))
        {
            return;
        }

        var tc = new TextComposition(InputManager.Current, textBox, replace);

        TextCompositionManager.StartComposition(tc);
    }

    private void OnTextBoxLostFocus(
        object? sender,
        RoutedEventArgs e)
    {
        if (valueTextBox is null)
        {
            return;
        }

        manualChange = false;
        valueTextBox.TextChanged -= OnTextChanged;
        InternalSetText(Value);
        valueTextBox.TextChanged += OnTextChanged;
    }

    private void OnTextChanged(
        object sender,
        TextChangedEventArgs e)
    {
        var text = ((TextBox)sender).Text;
        ChangeValueFromTextInput(text);
    }

    private void ChangeValueFromTextInput(
        string text)
    {
        if (!InterceptManualEnter)
        {
            return;
        }

        if (string.IsNullOrEmpty(text))
        {
            if (DefaultValue.HasValue)
            {
                SetValueTo(DefaultValue.Value);
                if (!manualChange)
                {
                    InternalSetText(DefaultValue.Value);
                }
            }
            else
            {
                SetCurrentValue(ValueProperty, value: null);
            }
        }
        else if (manualChange)
        {
            if (ValidateText(text, out var convertedValue))
            {
                convertedValue = FormattedValue(convertedValue, StringFormat, SpecificCultureInfo);
                SetValueTo(convertedValue);
            }
            else if (DefaultValue.HasValue)
            {
                SetValueTo(DefaultValue.Value);
                InternalSetText(Value);
            }
            else
            {
                SetCurrentValue(ValueProperty, value: null);
            }
        }

        OnValueChanged(Value, Value);

        manualChange = false;
    }

    private void OnValueTextBoxPaste(
        object sender,
        DataObjectPastingEventArgs e)
    {
        var textBox = (TextBox)sender;
        var textPresent = textBox.Text;

        var isText = e.SourceDataObject.GetDataPresent(DataFormats.Text, autoConvert: true);
        if (!isText)
        {
            e.CancelCommand();
            return;
        }

        var text = e.SourceDataObject.GetData(DataFormats.Text) as string;

        var newText = string.Concat(textPresent.Substring(0, textBox.SelectionStart), text, textPresent.Substring(textBox.SelectionStart + textBox.SelectionLength));
        if (!ValidateText(newText, out _))
        {
            e.CancelCommand();
        }
        else
        {
            manualChange = true;
        }
    }

    private void ResetInternal()
    {
        if (IsReadOnly)
        {
            return;
        }

        internalLargeChange = 100 * Interval;
        internalIntervalMultiplierForCalculation = Interval;
        intervalValueSinceReset = 0;
    }

    private bool ValidateText(
        string text,
        out double convertedValue)
    {
        convertedValue = 0d;

        if (text == SpecificCultureInfo.NumberFormat.PositiveSign ||
            text == SpecificCultureInfo.NumberFormat.NegativeSign)
        {
            return true;
        }

        if (text.Where(c => c == SpecificCultureInfo.NumberFormat.PositiveSign[0]).Skip(2).Any() ||
            text.Where(c => c == SpecificCultureInfo.NumberFormat.NegativeSign[0]).Skip(2).Any())
        {
            return false;
        }

        if (text.Any(char.IsLetter))
        {
            return false;
        }

        var isNumeric = NumericInputMode == NumericInput.Numbers ||
                        ParsingNumberStyle.HasFlag(NumberStyles.AllowHexSpecifier) ||
                        ParsingNumberStyle is NumberStyles.HexNumber or NumberStyles.Integer or NumberStyles.Number;

        var isHex = NumericInputMode == NumericInput.Numbers ||
                    ParsingNumberStyle.HasFlag(NumberStyles.AllowHexSpecifier) ||
                    ParsingNumberStyle == NumberStyles.HexNumber;

        var number = TryGetNumberFromText(text, isHex);

        if (isNumeric)
        {
            return ConvertNumber(number, out convertedValue);
        }

        if (number == SpecificCultureInfo.NumberFormat.NumberDecimalSeparator ||
            number == SpecificCultureInfo.NumberFormat.CurrencyDecimalSeparator ||
            number == SpecificCultureInfo.NumberFormat.PercentDecimalSeparator ||
            number == (SpecificCultureInfo.NumberFormat.NegativeSign + SpecificCultureInfo.NumberFormat.NumberDecimalSeparator) ||
            number == (SpecificCultureInfo.NumberFormat.PositiveSign + SpecificCultureInfo.NumberFormat.NumberDecimalSeparator))
        {
            return true;
        }

        return double.TryParse(number, ParsingNumberStyle, SpecificCultureInfo, out convertedValue);
    }

    private bool ConvertNumber(
        string text,
        out double convertedValue)
    {
        if (text.Any(c => c == SpecificCultureInfo.NumberFormat.NumberDecimalSeparator[0] ||
                          c == SpecificCultureInfo.NumberFormat.PercentDecimalSeparator[0] ||
                          c == SpecificCultureInfo.NumberFormat.CurrencyDecimalSeparator[0]))
        {
            convertedValue = 0d;
            return false;
        }

        if (!long.TryParse(text, ParsingNumberStyle, SpecificCultureInfo, out var convertedInt))
        {
            convertedValue = convertedInt;
            return false;
        }

        convertedValue = convertedInt;
        return true;
    }

    private string TryGetNumberFromText(
        string text,
        bool isHex)
    {
        if (isHex)
        {
            var hexMatches = RegexHexadecimal.Matches(text);
            return hexMatches.Count > 0 ? hexMatches[0].Value : text;
        }

        regexNumber ??= new Regex(
            RawRegexNumberString
                .Replace("<DecimalSeparator>", SpecificCultureInfo.NumberFormat.NumberDecimalSeparator, StringComparison.Ordinal)
                .Replace("<GroupSeparator>", SpecificCultureInfo.NumberFormat.NumberGroupSeparator, StringComparison.Ordinal),
            RegexOptions.Compiled,
            TimeSpan.FromSeconds(1));

        var matches = regexNumber.Matches(text);
        return matches.Count > 0
            ? matches[0].Value
            : text;
    }
}