namespace Atc.Wpf.Helpers;

/// <summary>Provides attached properties for text input controls, such as watermarks, a clear-text button, select-all on focus and text length monitoring.</summary>
[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static class TextBoxHelper
{
    /// <summary>Identifies the ButtonHeight attached property.</summary>
    public static readonly DependencyProperty ButtonHeightProperty = DependencyProperty.RegisterAttached(
        "ButtonHeight",
        typeof(double),
        typeof(TextBoxHelper),
        new FrameworkPropertyMetadata(
            22d,
            FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>Gets the value of the ButtonHeight attached property.</summary>
    public static double GetButtonHeight(DependencyObject d)
        => (double)d.GetValue(ButtonHeightProperty);

    /// <summary>Sets the value of the ButtonHeight attached property.</summary>
    public static void SetButtonHeight(
        DependencyObject d,
        double value)
        => d.SetValue(ButtonHeightProperty, value);

    /// <summary>Identifies the ButtonWidth attached property.</summary>
    public static readonly DependencyProperty ButtonWidthProperty = DependencyProperty.RegisterAttached(
        "ButtonWidth",
        typeof(double),
        typeof(TextBoxHelper),
        new FrameworkPropertyMetadata(
            22d,
            FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>Gets the value of the ButtonWidth attached property.</summary>
    public static double GetButtonWidth(DependencyObject d)
        => (double)d.GetValue(ButtonWidthProperty);

    /// <summary>Sets the value of the ButtonWidth attached property.</summary>
    public static void SetButtonWidth(
        DependencyObject d,
        double value)
        => d.SetValue(ButtonWidthProperty, value);

    /// <summary>Identifies the ButtonsAlignment attached property.</summary>
    public static readonly DependencyProperty ButtonsAlignmentProperty = DependencyProperty.RegisterAttached(
        "ButtonsAlignment",
        typeof(ButtonsAlignmentType),
        typeof(TextBoxHelper),
        new FrameworkPropertyMetadata(
            ButtonsAlignmentType.Right,
            FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

    /// <summary>Gets the value of the ButtonsAlignment attached property.</summary>
    public static ButtonsAlignmentType GetButtonsAlignment(DependencyObject d)
        => (ButtonsAlignmentType)d.GetValue(ButtonsAlignmentProperty);

    /// <summary>Sets the value of the ButtonsAlignment attached property.</summary>
    public static void SetButtonsAlignment(
        DependencyObject d,
        ButtonsAlignmentType value)
        => d.SetValue(ButtonsAlignmentProperty, value);

    /// <summary>Identifies the ButtonCommand attached property.</summary>
    public static readonly DependencyProperty ButtonCommandProperty = DependencyProperty.RegisterAttached(
        "ButtonCommand",
        typeof(ICommand),
        typeof(TextBoxHelper),
        new FrameworkPropertyMetadata(
            defaultValue: null,
            ButtonCommandOrClearTextChanged));

    /// <summary>Gets the value of the ButtonCommand attached property.</summary>
    public static ICommand? GetButtonCommand(DependencyObject d)
        => (ICommand?)d.GetValue(ButtonCommandProperty);

    /// <summary>Sets the value of the ButtonCommand attached property.</summary>
    public static void SetButtonCommand(
        DependencyObject d,
        ICommand? value)
        => d.SetValue(ButtonCommandProperty, value);

    /// <summary>Identifies the ButtonCommandParameter attached property.</summary>
    public static readonly DependencyProperty ButtonCommandParameterProperty = DependencyProperty.RegisterAttached(
        "ButtonCommandParameter",
        typeof(object),
        typeof(TextBoxHelper),
        new FrameworkPropertyMetadata(propertyChangedCallback: null));

    /// <summary>Gets the value of the ButtonCommandParameter attached property.</summary>
    public static object? GetButtonCommandParameter(DependencyObject d)
        => d.GetValue(ButtonCommandParameterProperty);

    /// <summary>Sets the value of the ButtonCommandParameter attached property.</summary>
    public static void SetButtonCommandParameter(
        DependencyObject d,
        object? value)
        => d.SetValue(ButtonCommandParameterProperty, value);

    /// <summary>Identifies the ButtonCommandTarget attached property.</summary>
    public static readonly DependencyProperty ButtonCommandTargetProperty = DependencyProperty.RegisterAttached(
        "ButtonCommandTarget",
        typeof(IInputElement),
        typeof(TextBoxHelper),
        new FrameworkPropertyMetadata(propertyChangedCallback: null));

    /// <summary>Gets the value of the ButtonCommandTarget attached property.</summary>
    public static IInputElement? GetButtonCommandTarget(DependencyObject d)
        => (IInputElement?)d.GetValue(ButtonCommandTargetProperty);

    /// <summary>Sets the value of the ButtonCommandTarget attached property.</summary>
    public static void SetButtonCommandTarget(
        DependencyObject d,
        IInputElement? value)
        => d.SetValue(ButtonCommandTargetProperty, value);

    /// <summary>Identifies the ButtonContent attached property.</summary>
    public static readonly DependencyProperty ButtonContentProperty = DependencyProperty.RegisterAttached(
        "ButtonContent",
        typeof(object),
        typeof(TextBoxHelper),
        new FrameworkPropertyMetadata("#"));

    /// <summary>Gets the value of the ButtonContent attached property.</summary>
    public static object GetButtonContent(DependencyObject d)
        => d.GetValue(ButtonContentProperty);

    /// <summary>Sets the value of the ButtonContent attached property.</summary>
    public static void SetButtonContent(
        DependencyObject d,
        object value)
        => d.SetValue(ButtonContentProperty, value);

    /// <summary>Identifies the ButtonContentTemplate attached property.</summary>
    public static readonly DependencyProperty ButtonContentTemplateProperty = DependencyProperty.RegisterAttached(
        "ButtonContentTemplate",
        typeof(DataTemplate),
        typeof(TextBoxHelper),
        new FrameworkPropertyMetadata(propertyChangedCallback: null));

    /// <summary>Gets the value of the ButtonContentTemplate attached property.</summary>
    public static DataTemplate? GetButtonContentTemplate(DependencyObject d)
        => (DataTemplate?)d.GetValue(ButtonContentTemplateProperty);

    /// <summary>Sets the value of the ButtonContentTemplate attached property.</summary>
    public static void SetButtonContentTemplate(
        DependencyObject d,
        DataTemplate? value)
        => d.SetValue(ButtonContentTemplateProperty, value);

    /// <summary>Identifies the ButtonTemplate attached property.</summary>
    public static readonly DependencyProperty ButtonTemplateProperty = DependencyProperty.RegisterAttached(
        "ButtonTemplate",
        typeof(ControlTemplate),
        typeof(TextBoxHelper),
        new FrameworkPropertyMetadata(propertyChangedCallback: null));

    /// <summary>Gets the value of the ButtonTemplate attached property.</summary>
    public static ControlTemplate? GetButtonTemplate(DependencyObject d)
        => (ControlTemplate?)d.GetValue(ButtonTemplateProperty);

    /// <summary>Sets the value of the ButtonTemplate attached property.</summary>
    public static void SetButtonTemplate(
        DependencyObject d,
        ControlTemplate? value)
        => d.SetValue(ButtonTemplateProperty, value);

    /// <summary>Identifies the ButtonFontFamily attached property.</summary>
    public static readonly DependencyProperty ButtonFontFamilyProperty = DependencyProperty.RegisterAttached(
        "ButtonFontFamily",
        typeof(FontFamily),
        typeof(TextBoxHelper),
        new FrameworkPropertyMetadata(new FontFamilyConverter().ConvertFromString("Segoe UI")));

    /// <summary>Gets the value of the ButtonFontFamily attached property.</summary>
    public static FontFamily GetButtonFontFamily(DependencyObject d)
        => (FontFamily)d.GetValue(ButtonFontFamilyProperty);

    /// <summary>Sets the value of the ButtonFontFamily attached property.</summary>
    public static void SetButtonFontFamily(
        DependencyObject d,
        FontFamily value)
        => d.SetValue(ButtonFontFamilyProperty, value);

    /// <summary>Identifies the ButtonFontSize attached property.</summary>
    public static readonly DependencyProperty ButtonFontSizeProperty = DependencyProperty.RegisterAttached(
        "ButtonFontSize",
        typeof(double),
        typeof(TextBoxHelper),
        new FrameworkPropertyMetadata(SystemFonts.MessageFontSize));

    /// <summary>Gets the value of the ButtonFontSize attached property.</summary>
    public static double GetButtonFontSize(DependencyObject d)
        => (double)d.GetValue(ButtonFontSizeProperty);

    /// <summary>Sets the value of the ButtonFontSize attached property.</summary>
    public static void SetButtonFontSize(
        DependencyObject d,
        double value)
        => d.SetValue(ButtonFontSizeProperty, value);

    /// <summary>Identifies the HasText attached property.</summary>
    public static readonly DependencyProperty HasTextProperty = DependencyProperty.RegisterAttached(
        "HasText",
        typeof(bool),
        typeof(TextBoxHelper),
        new FrameworkPropertyMetadata(
            BooleanBoxes.FalseBox,
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Gets the value of the HasText attached property.</summary>
    public static bool GetHasText(DependencyObject d)
        => (bool)d.GetValue(HasTextProperty);

    /// <summary>Sets the value of the HasText attached property.</summary>
    public static void SetHasText(
        DependencyObject d,
        bool value)
        => d.SetValue(HasTextProperty, BooleanBoxes.Box(value));

    /// <summary>Identifies the ClearTextButton attached property.</summary>
    public static readonly DependencyProperty ClearTextButtonProperty = DependencyProperty.RegisterAttached(
        "ClearTextButton",
        typeof(bool),
        typeof(TextBoxHelper),
        new FrameworkPropertyMetadata(
            BooleanBoxes.FalseBox,
            ButtonCommandOrClearTextChanged));

    /// <summary>Gets the value of the ClearTextButton attached property.</summary>
    public static bool GetClearTextButton(DependencyObject d)
        => (bool)d.GetValue(ClearTextButtonProperty);

    /// <summary>Sets the value of the ClearTextButton attached property.</summary>
    public static void SetClearTextButton(
        DependencyObject d,
        bool value)
        => d.SetValue(ClearTextButtonProperty, BooleanBoxes.Box(value));

    /// <summary>Identifies the UseFloatingWatermark attached property.</summary>
    public static readonly DependencyProperty UseFloatingWatermarkProperty = DependencyProperty.RegisterAttached(
        "UseFloatingWatermark",
        typeof(bool),
        typeof(TextBoxHelper),
        new FrameworkPropertyMetadata(
            BooleanBoxes.FalseBox,
            ButtonCommandOrClearTextChanged));

    /// <summary>Gets the value of the UseFloatingWatermark attached property.</summary>
    public static bool GetUseFloatingWatermark(DependencyObject d)
        => (bool)d.GetValue(UseFloatingWatermarkProperty);

    /// <summary>Sets the value of the UseFloatingWatermark attached property.</summary>
    public static void SetUseFloatingWatermark(
        DependencyObject d,
        bool value)
        => d.SetValue(UseFloatingWatermarkProperty, BooleanBoxes.Box(value));

    /// <summary>Identifies the HasValidationError attached property.</summary>
    public static readonly DependencyProperty HasValidationErrorProperty = DependencyProperty.RegisterAttached(
        "HasValidationError",
        typeof(bool),
        typeof(TextBoxHelper),
        new FrameworkPropertyMetadata(
            BooleanBoxes.FalseBox,
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Gets the value of the HasValidationError attached property.</summary>
    public static bool GetHasValidationError(DependencyObject d)
        => (bool)d.GetValue(HasValidationErrorProperty);

    /// <summary>Sets the value of the HasValidationError attached property.</summary>
    public static void SetHasValidationError(
        DependencyObject d,
        bool value)
        => d.SetValue(HasValidationErrorProperty, BooleanBoxes.Box(value));

    /// <summary>Identifies the ShowToolTip attached property.</summary>
    public static readonly DependencyProperty ShowToolTipProperty = DependencyProperty.RegisterAttached(
        "ShowToolTip",
        typeof(bool),
        typeof(TextBoxHelper),
        new FrameworkPropertyMetadata(
            BooleanBoxes.FalseBox,
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Gets the value of the ShowToolTip attached property.</summary>
    public static bool GetShowToolTip(DependencyObject d)
        => (bool)d.GetValue(ShowToolTipProperty);

    /// <summary>Sets the value of the ShowToolTip attached property.</summary>
    public static void SetShowToolTip(
        DependencyObject d,
        bool value)
        => d.SetValue(ShowToolTipProperty, BooleanBoxes.Box(value));

    /// <summary>Identifies the ToolTipText attached property.</summary>
    public static readonly DependencyProperty ToolTipTextProperty = DependencyProperty.RegisterAttached(
        "ToolTipText",
        typeof(string),
        typeof(TextBoxHelper),
        new UIPropertyMetadata(string.Empty));

    /// <summary>Gets the value of the ToolTipText attached property.</summary>
    public static string GetToolTipText(DependencyObject d)
        => (string)d.GetValue(ToolTipTextProperty);

    /// <summary>Sets the value of the ToolTipText attached property.</summary>
    public static void SetToolTipText(
        DependencyObject d,
        string value)
        => d.SetValue(ToolTipTextProperty, value);

    /// <summary>Identifies the Watermark attached property.</summary>
    public static readonly DependencyProperty WatermarkProperty = DependencyProperty.RegisterAttached(
        "Watermark",
        typeof(string),
        typeof(TextBoxHelper),
        new UIPropertyMetadata(string.Empty));

    /// <summary>Gets the value of the Watermark attached property.</summary>
    public static string GetWatermark(DependencyObject d)
        => (string)d.GetValue(WatermarkProperty);

    /// <summary>Sets the value of the Watermark attached property.</summary>
    public static void SetWatermark(
        DependencyObject d,
        string value)
        => d.SetValue(WatermarkProperty, value);

    /// <summary>Identifies the WatermarkAlignment attached property.</summary>
    public static readonly DependencyProperty WatermarkAlignmentProperty = DependencyProperty.RegisterAttached(
        "WatermarkAlignment",
        typeof(TextAlignment),
        typeof(TextBoxHelper),
        new FrameworkPropertyMetadata(
            TextAlignment.Left,
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>Gets the value of the WatermarkAlignment attached property.</summary>
    public static TextAlignment GetWatermarkAlignment(DependencyObject d)
        => (TextAlignment)d.GetValue(WatermarkAlignmentProperty);

    /// <summary>Sets the value of the WatermarkAlignment attached property.</summary>
    public static void SetWatermarkAlignment(
        DependencyObject d,
        TextAlignment value)
        => d.SetValue(WatermarkAlignmentProperty, value);

    /// <summary>Identifies the WatermarkTrimming attached property.</summary>
    public static readonly DependencyProperty WatermarkTrimmingProperty = DependencyProperty.RegisterAttached(
        "WatermarkTrimming",
        typeof(TextTrimming),
        typeof(TextBoxHelper),
        new FrameworkPropertyMetadata(
            TextTrimming.None,
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Gets the value of the WatermarkTrimming attached property.</summary>
    public static TextTrimming GetWatermarkTrimming(DependencyObject d)
        => (TextTrimming)d.GetValue(WatermarkTrimmingProperty);

    /// <summary>Sets the value of the WatermarkTrimming attached property.</summary>
    public static void SetWatermarkTrimming(
        DependencyObject d,
        TextTrimming value)
        => d.SetValue(WatermarkTrimmingProperty, value);

    /// <summary>Identifies the WatermarkWrapping attached property.</summary>
    public static readonly DependencyProperty WatermarkWrappingProperty = DependencyProperty.RegisterAttached(
        "WatermarkWrapping",
        typeof(TextWrapping),
        typeof(TextBoxHelper),
        new FrameworkPropertyMetadata(
            TextWrapping.NoWrap,
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Gets the value of the WatermarkWrapping attached property.</summary>
    public static TextWrapping GetWatermarkWrapping(DependencyObject d)
        => (TextWrapping)d.GetValue(WatermarkWrappingProperty);

    /// <summary>Sets the value of the WatermarkWrapping attached property.</summary>
    public static void SetWatermarkWrapping(
        DependencyObject d,
        TextWrapping value)
        => d.SetValue(WatermarkWrappingProperty, value);

    /// <summary>Identifies the SelectAllOnFocus attached property.</summary>
    public static readonly DependencyProperty SelectAllOnFocusProperty
        = DependencyProperty.RegisterAttached(
            "SelectAllOnFocus",
            typeof(bool),
            typeof(TextBoxHelper),
            new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>Gets the value of the SelectAllOnFocus attached property.</summary>
    public static bool GetSelectAllOnFocus(DependencyObject obj)
        => (bool)obj.GetValue(SelectAllOnFocusProperty);

    /// <summary>Sets the value of the SelectAllOnFocus attached property.</summary>
    public static void SetSelectAllOnFocus(
        DependencyObject obj,
        bool value)
        => obj.SetValue(SelectAllOnFocusProperty, BooleanBoxes.Box(value));

    /// <summary>Identifies the IsMonitoring attached property.</summary>
    public static readonly DependencyProperty IsMonitoringProperty
        = DependencyProperty.RegisterAttached(
            "IsMonitoring",
            typeof(bool),
            typeof(TextBoxHelper),
            new UIPropertyMetadata(
                BooleanBoxes.FalseBox,
                OnIsMonitoringChanged));

    /// <summary>Gets the value of the IsMonitoring attached property.</summary>
    public static bool GetIsMonitoring(UIElement element)
        => (bool)element.GetValue(IsMonitoringProperty);

    /// <summary>Sets the value of the IsMonitoring attached property.</summary>
    public static void SetIsMonitoring(
        DependencyObject obj,
        bool value)
        => obj.SetValue(IsMonitoringProperty, BooleanBoxes.Box(value));

    /// <summary>Identifies the key for the read-only TextLength attached property.</summary>
    public static readonly DependencyPropertyKey TextLengthPropertyKey = DependencyProperty.RegisterAttachedReadOnly(
        "TextLength",
        typeof(int),
        typeof(TextBoxHelper),
        new PropertyMetadata(0));

    /// <summary>Identifies the TextLength attached property.</summary>
    public static readonly DependencyProperty TextLengthProperty = TextLengthPropertyKey.DependencyProperty;

    /// <summary>
    /// Mirrors <see cref="ComboBox.Text"/> of a monitored ComboBox through a one-way self binding, so HasText follows
    /// the text update that the ComboBox performs after it has raised SelectionChanged.
    /// </summary>
    private static readonly DependencyProperty ComboBoxTextProxyProperty = DependencyProperty.RegisterAttached(
        "ComboBoxTextProxy",
        typeof(string),
        typeof(TextBoxHelper),
        new PropertyMetadata(
            default(string),
            OnComboBoxTextProxyChanged));

    private static void TextChanged(
        object sender,
        RoutedEventArgs e)
        => SetTextLength(sender as TextBox, textBox => textBox.Text.Length);

    private static void PasswordChanged(
        object sender,
        RoutedEventArgs e)
        => SetTextLength(
            sender as PasswordBox,
            passwordBox => passwordBox.Password.Length);

    private static void ButtonCommandOrClearTextChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        switch (d)
        {
            case RichTextBox richTextBox:
            {
                richTextBox.Loaded -= RichTextBoxLoaded;
                richTextBox.Loaded += RichTextBoxLoaded;
                if (richTextBox.IsLoaded)
                {
                    RichTextBoxLoaded(richTextBox, new RoutedEventArgs());
                }

                break;
            }

            case TextBox textBox:
            {
                textBox.Loaded -= TextChanged;
                textBox.Loaded += TextChanged;
                if (textBox.IsLoaded)
                {
                    TextChanged(textBox, new RoutedEventArgs());
                }

                break;
            }

            case PasswordBox passwordBox:
            {
                passwordBox.Loaded -= PasswordChanged;
                passwordBox.Loaded += PasswordChanged;
                if (passwordBox.IsLoaded)
                {
                    PasswordChanged(passwordBox, new RoutedEventArgs());
                }

                break;
            }

            case ComboBox comboBox:
            {
                comboBox.Loaded -= ComboBoxLoaded;
                comboBox.Loaded += ComboBoxLoaded;
                if (comboBox.IsLoaded)
                {
                    ComboBoxLoaded(comboBox, new RoutedEventArgs());
                }

                break;
            }
        }
    }

    private static void SetTextLength<TDependencyObject>(
        TDependencyObject? sender,
        Func<TDependencyObject, int> funcTextLength)
        where TDependencyObject : DependencyObject
    {
        if (sender is null)
        {
            return;
        }

        var value = funcTextLength(sender);
        sender.SetValue(TextLengthPropertyKey, value);
        sender.SetCurrentValue(HasTextProperty, BooleanBoxes.Box(value > 0));
    }

    private static void SetRichTextBoxTextLength(RichTextBox richTextBox)
    {
        SetTextLength(richTextBox, rtb =>
        {
            var textRange = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
            var text = textRange.Text;
            var lastIndexOfNewLine = text.LastIndexOf(Environment.NewLine, StringComparison.InvariantCulture);
            if (lastIndexOfNewLine >= 0)
            {
                text = text.Remove(lastIndexOfNewLine);
            }

            return text.Length;
        });
    }

    private static void RichTextBoxLoaded(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is RichTextBox richTextBox)
        {
            SetRichTextBoxTextLength(richTextBox);
        }
    }

    private static void ComboBoxLoaded(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is ComboBox comboBox)
        {
            UpdateComboBoxText(comboBox);
        }
    }

    private static void UpdateComboBoxText(ComboBox comboBox)
    {
        comboBox.SetValue(TextLengthPropertyKey, comboBox.Text?.Length ?? 0);
        comboBox.SetCurrentValue(HasTextProperty, BooleanBoxes.Box(!string.IsNullOrWhiteSpace(comboBox.Text) || comboBox.SelectedItem != null));
    }

    private static void TextBoxTextChanged(
        object sender,
        TextChangedEventArgs e)
        => SetTextLength(sender as TextBox, textBox => textBox.Text.Length);

    private static void RichTextBoxTextChanged(
        object sender,
        TextChangedEventArgs e)
    {
        if (sender is RichTextBox richTextBox)
        {
            SetRichTextBoxTextLength(richTextBox);
        }
    }

    private static void ComboBoxSelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (sender is ComboBox comboBox)
        {
            UpdateComboBoxText(comboBox);
        }
    }

    private static void OnComboBoxTextProxyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is ComboBox comboBox && GetIsMonitoring(comboBox))
        {
            UpdateComboBoxText(comboBox);
        }
    }

    /// <summary>Handles the TextChanged event that bubbles up from the editable text box of a monitored ComboBox.</summary>
    private static void ComboBoxTextChanged(
        object sender,
        TextChangedEventArgs e)
    {
        if (sender is ComboBox comboBox)
        {
            UpdateComboBoxText(comboBox);
        }
    }

    private static void OnIsMonitoringChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var isMonitoring = (bool)e.NewValue;
        if (d is PasswordBox passwordBox)
        {
            MonitorPasswordBox(passwordBox, isMonitoring);
            return;
        }

        if (d is not FrameworkElement element)
        {
            return;
        }

        // The themed styles turn monitoring on while the element is still being built. Hooking into other
        // controls' statics at that point, inside the BAML load, can deadlock against parallel UI threads
        // (seen in the test host), so wait for Loaded unless the element is already in a live tree.
        element.Loaded -= MonitorWhenLoaded;
        if (isMonitoring && !element.IsLoaded)
        {
            element.Loaded += MonitorWhenLoaded;
            return;
        }

        Monitor(element, isMonitoring);
    }

    private static void MonitorWhenLoaded(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is not FrameworkElement element)
        {
            return;
        }

        element.Loaded -= MonitorWhenLoaded;
        if (GetIsMonitoring(element))
        {
            Monitor(element, isMonitoring: true);
        }
    }

    private static void Monitor(
        FrameworkElement element,
        bool isMonitoring)
    {
        switch (element)
        {
            case RichTextBox richTextBox:
                MonitorRichTextBox(richTextBox, isMonitoring);
                break;
            case TextBox textBox:
                MonitorTextBox(textBox, isMonitoring);
                break;
            case ComboBox comboBox:
                MonitorComboBox(comboBox, isMonitoring);
                break;
        }
    }

    private static void MonitorPasswordBox(
        PasswordBox passwordBox,
        bool isMonitoring)
    {
        if (isMonitoring)
        {
            _ = passwordBox.BeginInvoke(() =>
            {
                PasswordChanged(passwordBox, new RoutedEventArgs(PasswordBox.PasswordChangedEvent, passwordBox));
            });

            passwordBox.PasswordChanged += PasswordChanged;
            passwordBox.GotFocus += PasswordGotFocus;
            passwordBox.PreviewMouseLeftButtonDown += UiElementPreviewMouseLeftButtonDown;
        }
        else
        {
            passwordBox.PasswordChanged -= PasswordChanged;
            passwordBox.GotFocus -= PasswordGotFocus;
            passwordBox.PreviewMouseLeftButtonDown -= UiElementPreviewMouseLeftButtonDown;
        }
    }

    private static void MonitorTextBox(
        TextBox textBox,
        bool isMonitoring)
    {
        textBox.TextChanged -= TextBoxTextChanged;
        textBox.GotFocus -= TextBoxGotFocus;
        textBox.PreviewMouseLeftButtonDown -= UiElementPreviewMouseLeftButtonDown;
        if (!isMonitoring)
        {
            return;
        }

        textBox.TextChanged += TextBoxTextChanged;
        textBox.GotFocus += TextBoxGotFocus;
        textBox.PreviewMouseLeftButtonDown += UiElementPreviewMouseLeftButtonDown;
        SetTextLength(textBox, x => x.Text.Length);
    }

    private static void MonitorRichTextBox(
        RichTextBox richTextBox,
        bool isMonitoring)
    {
        richTextBox.TextChanged -= RichTextBoxTextChanged;
        richTextBox.GotFocus -= RichTextBoxGotFocus;
        richTextBox.PreviewMouseLeftButtonDown -= UiElementPreviewMouseLeftButtonDown;
        if (!isMonitoring)
        {
            return;
        }

        richTextBox.TextChanged += RichTextBoxTextChanged;
        richTextBox.GotFocus += RichTextBoxGotFocus;
        richTextBox.PreviewMouseLeftButtonDown += UiElementPreviewMouseLeftButtonDown;
        SetRichTextBoxTextLength(richTextBox);
    }

    private static void MonitorComboBox(
        ComboBox comboBox,
        bool isMonitoring)
    {
        comboBox.SelectionChanged -= ComboBoxSelectionChanged;
        comboBox.RemoveHandler(TextBoxBase.TextChangedEvent, new TextChangedEventHandler(ComboBoxTextChanged));
        comboBox.GotFocus -= ComboBoxGotFocus;
        comboBox.PreviewMouseLeftButtonDown -= UiElementPreviewMouseLeftButtonDown;
        BindingOperations.ClearBinding(comboBox, ComboBoxTextProxyProperty);
        if (!isMonitoring)
        {
            return;
        }

        comboBox.SelectionChanged += ComboBoxSelectionChanged;
        comboBox.AddHandler(TextBoxBase.TextChangedEvent, new TextChangedEventHandler(ComboBoxTextChanged));
        comboBox.GotFocus += ComboBoxGotFocus;
        comboBox.PreviewMouseLeftButtonDown += UiElementPreviewMouseLeftButtonDown;
        BindingOperations.SetBinding(
            comboBox,
            ComboBoxTextProxyProperty,
            new Binding(nameof(ComboBox.Text))
            {
                RelativeSource = RelativeSource.Self,
                Mode = BindingMode.OneWay,
            });
        UpdateComboBoxText(comboBox);
    }

    private static void UiElementPreviewMouseLeftButtonDown(
        object? sender,
        MouseButtonEventArgs e)
    {
        if (sender is not UIElement { IsKeyboardFocusWithin: false } uiElement || !GetSelectAllOnFocus(uiElement))
        {
            return;
        }

        uiElement.Focus();
        e.Handled = true;
    }

    private static void PasswordGotFocus(
        object sender,
        RoutedEventArgs e)
        => ControlGotFocus(
            sender as PasswordBox,
            passwordBox => passwordBox.SelectAll());

    private static void TextBoxGotFocus(
        object sender,
        RoutedEventArgs e)
        => ControlGotFocus(
            sender as TextBox,
            textBox => textBox.SelectAll());

    private static void RichTextBoxGotFocus(
        object sender,
        RoutedEventArgs e)
        => ControlGotFocus(
            sender as RichTextBox,
            richTextBox => richTextBox.SelectAll());

    private static void ComboBoxGotFocus(
        object sender,
        RoutedEventArgs e)
        => ControlGotFocus(
            sender as ComboBox,
            comboBox =>
            {
                if (comboBox.IsEditable &&
                    comboBox.Template?.FindName("PART_EditableTextBox", comboBox) is TextBox editableTextBox)
                {
                    editableTextBox.SelectAll();
                }
            });

    private static void ControlGotFocus<TDependencyObject>(
        TDependencyObject? sender,
        Action<TDependencyObject> action)
        where TDependencyObject : DependencyObject
    {
        if (sender is null)
        {
            return;
        }

        if (GetSelectAllOnFocus(sender))
        {
            _ = sender.Dispatcher?.BeginInvoke(action, sender);
        }
    }
}