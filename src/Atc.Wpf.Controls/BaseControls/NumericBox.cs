// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable InconsistentNaming
// ReSharper disable CommentTypo
// ReSharper disable ConvertIfStatementToReturnStatement
namespace Atc.Wpf.Controls.BaseControls;

[SuppressMessage("Performance", "MA0023:Add RegexOptions.ExplicitCapture", Justification = "OK.")]
[TemplatePart(Name = PART_NumericUp, Type = typeof(RepeatButton))]
[TemplatePart(Name = PART_NumericDown, Type = typeof(RepeatButton))]
[TemplatePart(Name = PART_TextBox, Type = typeof(TextBox))]
[StyleTypedProperty(Property = nameof(SpinButtonStyle), StyleTargetType = typeof(ButtonBase))]
public partial class NumericBox : Control
{
    private const string PART_NumericDown = "PART_NumericDownButton";
    private const string PART_NumericUp = "PART_NumericUpButton";
    private const string PART_TextBox = "PART_TextBox";
    private const string PART_ContentHost = "PART_ContentHost";
    private const double DefaultInterval = 1d;
    private const int DefaultDelay = 500;
    private const string RawRegexNumberString = @"[-+]?(?<![0-9][<DecimalSeparator><GroupSeparator>])[<DecimalSeparator><GroupSeparator>]?[0-9]+(?:[<DecimalSeparator><GroupSeparator>\s][0-9]+)*[<DecimalSeparator><GroupSeparator>]?[0-9]?(?:[eE][-+]?[0-9]+)?(?!\.[0-9])";

    private static readonly Regex RegexStringFormatHexadecimal = new(
        @"^(?<complexHEX>.*{\d\s*:[Xx]\d*}.*)?(?<simpleHEX>[Xx]\d*)?$",
        RegexOptions.Compiled,
        TimeSpan.FromSeconds(1));

    private static readonly Regex RegexHexadecimal = new(
        @"^([a-fA-F0-9]{1,2}\s?)+$",
        RegexOptions.Compiled,
        TimeSpan.FromSeconds(1));

    private static readonly Regex RegexStringFormat = new(
        @"\{0\s*(:(?<format>.*))?\}",
        RegexOptions.Compiled,
        TimeSpan.FromSeconds(1));

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

    [RoutedEvent(HandlerType = typeof(NumericBoxChangedRoutedEventHandler))]
    private static readonly RoutedEvent valueIncremented;

    [RoutedEvent(HandlerType = typeof(NumericBoxChangedRoutedEventHandler))]
    private static readonly RoutedEvent valueDecremented;

    [RoutedEvent]
    private static readonly RoutedEvent delayChanged;

    [RoutedEvent]
    private static readonly RoutedEvent maximumReached;

    [RoutedEvent]
    private static readonly RoutedEvent minimumReached;

    [RoutedEvent(HandlerType = typeof(RoutedPropertyChangedEventHandler<double?>))]
    private static readonly RoutedEvent valueChanged;

    [DependencyProperty(
        DefaultValue = DefaultDelay,
        PropertyChangedCallback = nameof(OnDelayPropertyChanged),
        ValidateValueCallback = nameof(ValidateDefaultDelay))]
    private int delay;

    public static readonly DependencyProperty TextAlignmentProperty = TextBox.TextAlignmentProperty.AddOwner(typeof(NumericBox));

    public TextAlignment TextAlignment
    {
        get => (TextAlignment)GetValue(TextAlignmentProperty);
        set => SetValue(
            TextAlignmentProperty,
            value);
    }

    [DependencyProperty(
        DefaultValue = true,
        PropertyChangedCallback = nameof(OnSpeedupPropertyChanged))]
    private bool speedup;

    public static readonly DependencyProperty IsReadOnlyProperty = TextBoxBase.IsReadOnlyProperty.AddOwner(
        typeof(NumericBox),
        new FrameworkPropertyMetadata(
            BooleanBoxes.FalseBox,
            FrameworkPropertyMetadataOptions.Inherits,
            OnIsReadOnlyPropertyChanged));

    public bool IsReadOnly
    {
        get => (bool)GetValue(IsReadOnlyProperty);
        set => SetValue(
            IsReadOnlyProperty,
            BooleanBoxes.Box(value));
    }

    [DependencyProperty(
        DefaultValue = "",
        PropertyChangedCallback = nameof(OnStringFormatPropertyChanged),
        CoerceValueCallback = nameof(CoerceStringFormat))]
    private string stringFormat;

    [DependencyProperty(DefaultValue = true)]
    private bool interceptArrowKeys;

    [DependencyProperty(DefaultValue = true)]
    private bool interceptMouseWheel;

    [DependencyProperty(
        DefaultValue = true,
        PropertyChangedCallback = nameof(OnInterceptManualEnterPropertyChanged))]
    private bool interceptManualEnter;

    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        PropertyChangedCallback = nameof(OnValuePropertyChanged),
        CoerceValueCallback = nameof(CoerceValueItem1))]
    private double? value;

    [DependencyProperty(
        PropertyChangedCallback = nameof(OnDefaultValuePropertyChanged),
        CoerceValueCallback = nameof(CoerceDefaultValue))]
    private double? defaultValue;

    [DependencyProperty(
        DefaultValue = PropertyDefaultValueConstants.MinValue,
        PropertyChangedCallback = nameof(OnMinimumPropertyChanged))]
    private double minimum;

    [DependencyProperty(
        DefaultValue = PropertyDefaultValueConstants.MaxValue,
        PropertyChangedCallback = nameof(OnMaximumPropertyChanged),
        CoerceValueCallback = nameof(CoerceMaximum))]
    private double maximum;

    [DependencyProperty(
        DefaultValue = DefaultInterval,
        PropertyChangedCallback = nameof(OnIntervalPropertyChanged))]
    private double interval;

    [DependencyProperty(DefaultValue = true)]
    private bool trackMouseWheelWhenMouseOver;

    [DependencyProperty]
    private Style? spinButtonStyle;

    [DependencyProperty(
        DefaultValue = ButtonsAlignmentType.Right,
        Flags = FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure)]
    private ButtonsAlignmentType buttonsAlignment;

    [DependencyProperty(DefaultValue = false)]
    private bool hideUpDownButtons;

    [DependencyProperty(DefaultValue = "new Thickness(0, -0.5, -0.5, 0)")]
    private Thickness upDownButtonsMargin;

    [DependencyProperty(DefaultValue = 20)]
    private double upDownButtonsWidth;

    [DependencyProperty(DefaultValue = true)]
    private bool upDownButtonsFocusable;

    [DependencyProperty(DefaultValue = false)]
    private bool switchUpDownButtons;

    [DependencyProperty]
    private object? buttonUpContent;

    [DependencyProperty]
    private DataTemplate? buttonUpContentTemplate;

    [DependencyProperty]
    private string? buttonUpContentStringFormat;

    [DependencyProperty]
    private object? buttonDownContent;

    [DependencyProperty]
    private DataTemplate? buttonDownContentTemplate;

    [DependencyProperty]
    private string? buttonDownContentStringFormat;

    [DependencyProperty(PropertyChangedCallback = nameof(OnCulturePropertyChanged))]
    private CultureInfo? culture;

    [DependencyProperty(
        DefaultValue = NumericInput.All,
        PropertyChangedCallback = nameof(OnNumericInputModePropertyChanged))]
    private NumericInput numericInputMode;

    [DependencyProperty(DefaultValue = DecimalPointCorrectionMode.Inherits)]
    private DecimalPointCorrectionMode decimalPointCorrection;

    [DependencyProperty(
        DefaultValue = false,
        PropertyChangedCallback = nameof(OnSnapToMultipleOfIntervalPropertyChanged))]
    private bool snapToMultipleOfInterval;

    [DependencyProperty(DefaultValue = NumberStyles.Any)]
    private NumberStyles parsingNumberStyle;

    [DependencyProperty(
        DefaultValue = "",
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal)]
    private string prefixText;

    [DependencyProperty(
        DefaultValue = "",
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal)]
    private string suffixText;

    private CultureInfo SpecificCultureInfo
        => Culture ?? Language.GetSpecificCulture();

    static NumericBox()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(NumericBox),
            new FrameworkPropertyMetadata(typeof(NumericBox)));

        VerticalContentAlignmentProperty.OverrideMetadata(
            typeof(NumericBox),
            new FrameworkPropertyMetadata(VerticalAlignment.Center));

        HorizontalContentAlignmentProperty.OverrideMetadata(
            typeof(NumericBox),
            new FrameworkPropertyMetadata(HorizontalAlignment.Right));

        EventManager.RegisterClassHandler(
            typeof(NumericBox),
            GotFocusEvent,
            new RoutedEventHandler(OnGotFocus));
    }

    public NumericBox()
    {
        CultureManager.UiCultureChanged += OnUiCultureChanged;
    }

    protected override AutomationPeer OnCreateAutomationPeer()
        => new NumericBoxAutomationPeer(this);

    private void OnUiCultureChanged(
        object? sender,
        UiCultureEventArgs e)
        => Culture = Thread.CurrentThread.CurrentUICulture;

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
        numericBox.OnDelayChanged(
            oldDelay,
            newDelay);
    }

    private static void OnSpeedupPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue == e.NewValue)
        {
            return;
        }

        (d as NumericBox)?.OnSpeedupChanged(
            (bool)e.OldValue,
            (bool)e.NewValue);
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

        numericBox.SetCurrentValue(
            ParsingNumberStyleProperty,
            NumberStyles.HexNumber);
        numericBox.SetCurrentValue(
            NumericInputModeProperty,
            numericBox.NumericInputMode | NumericInput.Decimal);
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

        (d as NumericBox)?.OnValueChanged(
            (double?)e.OldValue,
            (double?)e.NewValue);
    }

    private static Tuple<double?, bool> CoerceValue(
        DependencyObject d,
        object? baseValue)
    {
        var numericBox = (NumericBox)d;
        if (baseValue is null)
        {
            return new Tuple<double?, bool>(
                numericBox.DefaultValue,
                item2: false);
        }

        var val = ((double?)baseValue).Value;

        if (!numericBox.NumericInputMode.HasFlag(NumericInput.Decimal))
        {
            val = System.Math.Truncate(val);
        }

        if (val < numericBox.Minimum)
        {
            return new Tuple<double?, bool>(
                numericBox.Minimum,
                item2: true);
        }

        if (val > numericBox.Maximum)
        {
            return new Tuple<double?, bool>(
                numericBox.Maximum,
                item2: true);
        }

        return new Tuple<double?, bool>(
            val,
            item2: false);
    }

    private static object? CoerceValueItem1(
        DependencyObject d,
        object? baseValue)
        => CoerceValue(
            d,
            baseValue).Item1;

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

        var minimumValue = ((NumericBox)d).Minimum;
        var maximumValue = ((NumericBox)d).Maximum;

        if (val < minimumValue)
        {
            return minimumValue;
        }

        if (val > maximumValue)
        {
            return maximumValue;
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
        numericBox.OnMinimumChanged(
            (double)e.OldValue,
            (double)e.NewValue);
        numericBox.EnableDisableUpDown();
    }

    private static void OnMaximumPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var numericBox = (NumericBox)d;

        numericBox.CoerceValue(ValueProperty);
        numericBox.CoerceValue(DefaultValueProperty);
        numericBox.OnMaximumChanged(
            (double)e.OldValue,
            (double)e.NewValue);
        numericBox.EnableDisableUpDown();
    }

    private static object CoerceMaximum(
        DependencyObject d,
        object value)
    {
        var minimumValue = ((NumericBox)d).Minimum;
        var val = (double)value;
        return val < minimumValue
            ? minimumValue
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
        numericBox.OnValueChanged(
            numericBox.Value,
            numericBox.Value);
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

        var val = numericBox.Value.GetValueOrDefault();
        numericBox.Value = System.Math.Round(val / numericBox.Interval) * numericBox.Interval;
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
            !Equals(
                e.OriginalSource,
                numericBox))
        {
            return;
        }

        var request = new TraversalRequest(
            (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift
                ? FocusNavigationDirection.Previous
                : FocusNavigationDirection.Next);

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

        OnValueChanged(
            Value,
            Value);

        scrollViewer = null;
    }

    private void ToggleReadOnlyMode(bool isReadOnly)
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
            DataObject.RemovePastingHandler(
                valueTextBox,
                OnValueTextBoxPaste);
        }
        else
        {
            valueTextBox.LostFocus += OnTextBoxLostFocus;
            valueTextBox.PreviewTextInput += OnPreviewTextInput;
            valueTextBox.PreviewKeyDown += OnTextBoxKeyDown;
            valueTextBox.TextChanged += OnTextChanged;
            DataObject.AddPastingHandler(
                valueTextBox,
                OnValueTextBoxPaste);
        }
    }

    public void SelectAll()
        => valueTextBox?.SelectAll();

    private void RaiseChangeDelay()
        => RaiseEvent(new RoutedEventArgs(DelayChangedEvent));

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

    protected override void OnPreviewKeyDown(KeyEventArgs e)
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

    protected override void OnPreviewKeyUp(KeyEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnPreviewKeyUp(e);

        if (e.Key is Key.Down or Key.Up)
        {
            ResetInternal();
        }
    }

    protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
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
                handlesMouseWheelScrolling.Value.SetValue(
                    sv,
                    value: true,
                    index: null);
            }
            else if (InterceptMouseWheel)
            {
                handlesMouseWheelScrolling.Value.SetValue(
                    sv,
                    valueTextBox?.IsFocused == true,
                    index: null);
            }
            else
            {
                handlesMouseWheelScrolling.Value.SetValue(
                    sv,
                    value: true,
                    index: null);
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
            .Remove(
                textBox.SelectionStart,
                textBox.SelectionLength)
            .Insert(
                textBox.CaretIndex,
                e.Text);

        var isInvalid = !ValidateText(
            fullText,
            out var convertedValue);

        if (isInvalid)
        {
            e.Handled = true;
            manualChange = false;
            return;
        }

        var isValueInvalid = CoerceValue(
            this,
            convertedValue as double?).Item2;
        if (isValueInvalid &&
            convertedValue >= Minimum &&
            convertedValue <= Maximum)
        {
            e.Handled = true;
            manualChange = false;
            return;
        }

        e.Handled = false;
        manualChange = true;
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

                if (!Equals(
                    oldValue,
                    newValue))
                {
                    RaiseEvent(
                        new RoutedPropertyChangedEventArgs<double?>(
                            oldValue,
                            newValue,
                            ValueChangedEvent));
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

        if (!Equals(
            oldValue,
            newValue))
        {
            RaiseEvent(
                new RoutedPropertyChangedEventArgs<double?>(
                    oldValue,
                    newValue,
                    ValueChangedEvent));
        }
    }

    private static bool ValidateDefaultDelay(object value)
        => value is int intValue
           && Convert.ToInt32(
               intValue,
               GlobalizationConstants.EnglishCultureInfo) >= 0;

    private void InternalSetText(double? newValue)
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
            valueTextBox.Text = FormattedValueString(
                newValue.Value,
                StringFormat,
                SpecificCultureInfo);
        }
    }

    private static string FormattedValueString(
        double newValue,
        string format,
        CultureInfo culture)
    {
        format = format.Replace(
            "{}",
            string.Empty,
            StringComparison.Ordinal);
        if (string.IsNullOrWhiteSpace(format))
        {
            return newValue.ToString(culture);
        }

        if (TryFormatHexadecimal(
            newValue,
            format,
            culture,
            out var hexValue))
        {
            return hexValue;
        }

        var match = RegexStringFormat.Match(format);
        return match.Success
            ? string.Format(
                culture,
                format,
                newValue)
            : newValue.ToString(
                format,
                culture);
    }

    private static double FormattedValue(
        double newValue,
        string format,
        CultureInfo culture)
    {
        format = format.Replace(
            "{}",
            string.Empty,
            StringComparison.Ordinal);
        if (string.IsNullOrWhiteSpace(format))
        {
            return newValue;
        }

        if (TryFormatHexadecimal(
            newValue,
            format,
            culture,
            out _))
        {
            return newValue;
        }

        var match = RegexStringFormat.Match(format);
        return ConvertStringFormatValue(
            newValue,
            match.Success ? match.Groups["format"].Value : format);
    }

    private static double ConvertStringFormatValue(
        double value,
        string format)
    {
        if (format.Contains(
            'P',
            StringComparison.Ordinal) ||
            format.Contains(
                '%',
                StringComparison.Ordinal))
        {
            value /= 100d;
        }
        else if (format.Contains(
            '‰',
            StringComparison.Ordinal))
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
                output = ((int)newValue).ToString(
                    match.Groups["simpleHEX"].Value,
                    culture);
                return true;
            }

            if (match.Groups["complexHEX"].Success)
            {
                output = string.Format(
                    culture,
                    match.Groups["complexHEX"].Value,
                    (int)newValue);
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

        scrollViewer = valueTextBox?.Template.FindName(
            PART_ContentHost,
            valueTextBox) as ScrollViewer;
        if (scrollViewer is not null)
        {
            handlesMouseWheelScrolling = new Lazy<PropertyInfo?>(
                () => scrollViewer
                    .GetType()
                    .GetProperties(BindingFlags.NonPublic | BindingFlags.Instance)
                    .SingleOrDefault(i => i.Name == "HandlesMouseWheelScrolling"));
        }

        return scrollViewer;
    }

    private void ChangeValueWithSpeedUp(bool toPositive)
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

    private void ChangeValueInternal(double interVal)
    {
        if (IsReadOnly)
        {
            return;
        }

        manualChange = false;

        var routedEvent = interVal > 0
            ? new NumericBoxChangedRoutedEventArgs(
                ValueIncrementedEvent,
                interVal)
            : new NumericBoxChangedRoutedEventArgs(
                ValueDecrementedEvent,
                interVal);

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

    private void ChangeValueBy(double difference)
        => SetValueTo(Value.GetValueOrDefault() + difference);

    private void SetValueTo(double newValue)
    {
        var val = newValue;

        if (SnapToMultipleOfInterval && System.Math.Abs(Interval) > 0)
        {
            val = System.Math.Round(newValue / Interval) * Interval;
        }

        if (val > Maximum)
        {
            val = Maximum;
        }
        else if (val < Minimum)
        {
            val = Minimum;
        }

        SetCurrentValue(
            ValueProperty,
            CoerceValue(this, val).Item1);
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
        var cultureInfo = SpecificCultureInfo;

        // Surrogate the blocked key pressed
        SimulateDecimalPointKeyPress(
            textBox,
            correctionMode,
            cultureInfo);
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

        var tc = new TextComposition(
            InputManager.Current,
            textBox,
            replace);

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
        => ChangeValueFromTextInput(((TextBox)sender).Text);

    private void ChangeValueFromTextInput(string text)
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
                SetCurrentValue(
                    ValueProperty,
                    value: null);
            }
        }
        else if (manualChange)
        {
            if (ValidateText(
                text,
                out var convertedValue))
            {
                convertedValue = FormattedValue(
                    convertedValue,
                    StringFormat,
                    SpecificCultureInfo);
                SetValueTo(convertedValue);
            }
            else if (DefaultValue.HasValue)
            {
                SetValueTo(DefaultValue.Value);
                InternalSetText(Value);
            }
            else
            {
                SetCurrentValue(
                    ValueProperty,
                    value: null);
            }
        }

        OnValueChanged(
            Value,
            Value);

        manualChange = false;
    }

    private void OnValueTextBoxPaste(
        object sender,
        DataObjectPastingEventArgs e)
    {
        var textBox = (TextBox)sender;
        var textPresent = textBox.Text;

        var isText = e.SourceDataObject.GetDataPresent(
            DataFormats.Text,
            autoConvert: true);
        if (!isText)
        {
            e.CancelCommand();
            return;
        }

        var text = e.SourceDataObject.GetData(DataFormats.Text) as string;

        var newText = string.Concat(
            textPresent.Substring(
                0,
                textBox.SelectionStart),
            text,
            textPresent.Substring(textBox.SelectionStart + textBox.SelectionLength));
        if (!ValidateText(
            newText,
            out _))
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

    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
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

        var positiveSignCount = text
            .Where(c => c == SpecificCultureInfo.NumberFormat.PositiveSign[0])
            .Skip(2)
            .Any();

        var negativeSignCount = text
            .Where(c => c == SpecificCultureInfo.NumberFormat.NegativeSign[0])
            .Skip(2)
            .Any();

        if (positiveSignCount || negativeSignCount)
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

        var number = TryGetNumberFromText(
            text,
            isHex);

        if (isNumeric)
        {
            return ConvertNumber(
                number,
                out convertedValue);
        }

        if (number == SpecificCultureInfo.NumberFormat.NumberDecimalSeparator ||
            number == SpecificCultureInfo.NumberFormat.CurrencyDecimalSeparator ||
            number == SpecificCultureInfo.NumberFormat.PercentDecimalSeparator ||
            number == (SpecificCultureInfo.NumberFormat.NegativeSign + SpecificCultureInfo.NumberFormat.NumberDecimalSeparator) ||
            number == (SpecificCultureInfo.NumberFormat.PositiveSign + SpecificCultureInfo.NumberFormat.NumberDecimalSeparator))
        {
            return true;
        }

        return double.TryParse(
            number,
            ParsingNumberStyle,
            SpecificCultureInfo,
            out convertedValue);
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

        if (!long.TryParse(
            text,
            ParsingNumberStyle,
            SpecificCultureInfo,
            out var convertedInt))
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
                .Replace(
                    "<DecimalSeparator>",
                    SpecificCultureInfo.NumberFormat.NumberDecimalSeparator,
                    StringComparison.Ordinal)
                .Replace(
                    "<GroupSeparator>",
                    SpecificCultureInfo.NumberFormat.NumberGroupSeparator,
                    StringComparison.Ordinal),
            RegexOptions.Compiled,
            TimeSpan.FromSeconds(1));

        var matches = regexNumber.Matches(text);
        return matches.Count > 0
            ? matches[0].Value
            : text;
    }
}