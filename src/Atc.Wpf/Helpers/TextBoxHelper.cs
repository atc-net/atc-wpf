namespace Atc.Wpf.Helpers;

[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static class TextBoxHelper
{
    public static readonly DependencyProperty ButtonHeightProperty = DependencyProperty.RegisterAttached(
        "ButtonHeight",
        typeof(double),
        typeof(TextBoxHelper),
        new FrameworkPropertyMetadata(
            22d,
            FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits));

    public static double GetButtonHeight(
        DependencyObject d)
        => (double)d.GetValue(ButtonHeightProperty);

    public static void SetButtonHeight(
        DependencyObject d,
        double value)
        => d.SetValue(ButtonHeightProperty, value);

    public static readonly DependencyProperty ButtonWidthProperty = DependencyProperty.RegisterAttached(
        "ButtonWidth",
        typeof(double),
        typeof(TextBoxHelper),
        new FrameworkPropertyMetadata(
            22d,
            FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits));

    public static double GetButtonWidth(
        DependencyObject d)
        => (double)d.GetValue(ButtonWidthProperty);

    public static void SetButtonWidth(
        DependencyObject d,
        double value)
        => d.SetValue(ButtonWidthProperty, value);

    public static readonly DependencyProperty ButtonsAlignmentProperty = DependencyProperty.RegisterAttached(
        "ButtonsAlignment",
        typeof(ButtonsAlignmentType),
        typeof(TextBoxHelper),
        new FrameworkPropertyMetadata(
            ButtonsAlignmentType.Right,
            FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static ButtonsAlignmentType GetButtonsAlignment(
        DependencyObject d)
        => (ButtonsAlignmentType)d.GetValue(ButtonsAlignmentProperty);

    public static void SetButtonsAlignment(
        DependencyObject d,
        ButtonsAlignmentType value)
        => d.SetValue(ButtonsAlignmentProperty, value);

    public static readonly DependencyProperty ButtonCommandProperty = DependencyProperty.RegisterAttached(
        "ButtonCommand",
        typeof(ICommand),
        typeof(TextBoxHelper),
        new FrameworkPropertyMetadata(
            defaultValue: null,
            ButtonCommandOrClearTextChanged));

    public static ICommand? GetButtonCommand(
        DependencyObject d)
        => (ICommand?)d.GetValue(ButtonCommandProperty);

    public static void SetButtonCommand(
        DependencyObject d,
        ICommand? value)
        => d.SetValue(ButtonCommandProperty, value);

    public static readonly DependencyProperty ButtonCommandParameterProperty = DependencyProperty.RegisterAttached(
        "ButtonCommandParameter",
        typeof(object),
        typeof(TextBoxHelper),
        new FrameworkPropertyMetadata(propertyChangedCallback: null));

    public static object? GetButtonCommandParameter(DependencyObject d)
        => d.GetValue(ButtonCommandParameterProperty);

    public static void SetButtonCommandParameter(
        DependencyObject d,
        object? value)
        => d.SetValue(ButtonCommandParameterProperty, value);

    public static readonly DependencyProperty ButtonCommandTargetProperty = DependencyProperty.RegisterAttached(
        "ButtonCommandTarget",
        typeof(IInputElement),
        typeof(TextBoxHelper),
        new FrameworkPropertyMetadata(propertyChangedCallback: null));

    public static IInputElement? GetButtonCommandTarget(
        DependencyObject d)
        => (IInputElement?)d.GetValue(ButtonCommandTargetProperty);

    public static void SetButtonCommandTarget(
        DependencyObject d,
        IInputElement? value)
        => d.SetValue(ButtonCommandTargetProperty, value);

    public static readonly DependencyProperty ButtonContentProperty = DependencyProperty.RegisterAttached(
        "ButtonContent",
        typeof(object),
        typeof(TextBoxHelper),
        new FrameworkPropertyMetadata("#"));

    public static object GetButtonContent(
        DependencyObject d)
        => d.GetValue(ButtonContentProperty);

    public static void SetButtonContent(
        DependencyObject d,
        object value)
        => d.SetValue(ButtonContentProperty, value);

    public static readonly DependencyProperty ButtonContentTemplateProperty = DependencyProperty.RegisterAttached(
        "ButtonContentTemplate",
        typeof(DataTemplate),
        typeof(TextBoxHelper),
        new FrameworkPropertyMetadata(propertyChangedCallback: null));

    public static DataTemplate? GetButtonContentTemplate(
        DependencyObject d)
        => (DataTemplate?)d.GetValue(ButtonContentTemplateProperty);

    public static void SetButtonContentTemplate(
        DependencyObject d,
        DataTemplate? value)
        => d.SetValue(ButtonContentTemplateProperty, value);

    public static readonly DependencyProperty ButtonTemplateProperty = DependencyProperty.RegisterAttached(
        "ButtonTemplate",
        typeof(ControlTemplate),
        typeof(TextBoxHelper),
        new FrameworkPropertyMetadata(propertyChangedCallback: null));

    public static ControlTemplate? GetButtonTemplate(
        DependencyObject d)
        => (ControlTemplate?)d.GetValue(ButtonTemplateProperty);

    public static void SetButtonTemplate(
        DependencyObject d,
        ControlTemplate? value)
        => d.SetValue(ButtonTemplateProperty, value);

    public static readonly DependencyProperty ButtonFontFamilyProperty = DependencyProperty.RegisterAttached(
        "ButtonFontFamily",
        typeof(FontFamily),
        typeof(TextBoxHelper),
        new FrameworkPropertyMetadata(new FontFamilyConverter().ConvertFromString("Segoe UI")));

    public static FontFamily GetButtonFontFamily(
        DependencyObject d)
        => (FontFamily)d.GetValue(ButtonFontFamilyProperty);

    public static void SetButtonFontFamily(
        DependencyObject d,
        FontFamily value)
        => d.SetValue(ButtonFontFamilyProperty, value);

    public static readonly DependencyProperty ButtonFontSizeProperty = DependencyProperty.RegisterAttached(
        "ButtonFontSize",
        typeof(double),
        typeof(TextBoxHelper),
        new FrameworkPropertyMetadata(SystemFonts.MessageFontSize));

    public static double GetButtonFontSize(
        DependencyObject d)
        => (double)d.GetValue(ButtonFontSizeProperty);

    public static void SetButtonFontSize(
        DependencyObject d,
        double value)
        => d.SetValue(ButtonFontSizeProperty, value);

    public static readonly DependencyProperty HasTextProperty = DependencyProperty.RegisterAttached(
        "HasText",
        typeof(bool),
        typeof(TextBoxHelper),
        new FrameworkPropertyMetadata(
            BooleanBoxes.FalseBox,
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));

    public static bool GetHasText(
        DependencyObject d)
        => (bool)d.GetValue(HasTextProperty);

    public static void SetHasText(
        DependencyObject d,
        bool value)
        => d.SetValue(HasTextProperty, BooleanBoxes.Box(value));

    public static readonly DependencyProperty ClearTextButtonProperty = DependencyProperty.RegisterAttached(
        "ClearTextButton",
        typeof(bool),
        typeof(TextBoxHelper),
        new FrameworkPropertyMetadata(
            BooleanBoxes.FalseBox,
            ButtonCommandOrClearTextChanged));

    public static bool GetClearTextButton(
        DependencyObject d)
        => (bool)d.GetValue(ClearTextButtonProperty);

    public static void SetClearTextButton(
        DependencyObject d,
        bool value)
        => d.SetValue(ClearTextButtonProperty, BooleanBoxes.Box(value));

    public static readonly DependencyProperty UseFloatingWatermarkProperty = DependencyProperty.RegisterAttached(
        "UseFloatingWatermark",
        typeof(bool),
        typeof(TextBoxHelper),
        new FrameworkPropertyMetadata(
            BooleanBoxes.FalseBox,
            ButtonCommandOrClearTextChanged));

    public static bool GetUseFloatingWatermark(
        DependencyObject d)
        => (bool)d.GetValue(UseFloatingWatermarkProperty);

    public static void SetUseFloatingWatermark(
        DependencyObject d,
        bool value)
        => d.SetValue(UseFloatingWatermarkProperty, BooleanBoxes.Box(value));

    public static readonly DependencyProperty HasValidationErrorProperty = DependencyProperty.RegisterAttached(
        "HasValidationError",
        typeof(bool),
        typeof(TextBoxHelper),
        new FrameworkPropertyMetadata(
            BooleanBoxes.FalseBox,
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));

    public static bool GetHasValidationError(
        DependencyObject d)
        => (bool)d.GetValue(HasValidationErrorProperty);

    public static void SetHasValidationError(
        DependencyObject d,
        bool value)
        => d.SetValue(HasValidationErrorProperty, BooleanBoxes.Box(value));

    public static readonly DependencyProperty ShowToolTipProperty = DependencyProperty.RegisterAttached(
        "ShowToolTip",
        typeof(bool),
        typeof(TextBoxHelper),
        new FrameworkPropertyMetadata(
            BooleanBoxes.FalseBox,
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));

    public static bool GetShowToolTip(
        DependencyObject d)
        => (bool)d.GetValue(ShowToolTipProperty);

    public static void SetShowToolTip(
        DependencyObject d,
        bool value)
        => d.SetValue(ShowToolTipProperty, BooleanBoxes.Box(value));

    public static readonly DependencyProperty ToolTipTextProperty = DependencyProperty.RegisterAttached(
        "ToolTipText",
        typeof(string),
        typeof(TextBoxHelper),
        new UIPropertyMetadata(string.Empty));

    public static string GetToolTipText(
        DependencyObject d)
        => (string)d.GetValue(ToolTipTextProperty);

    public static void SetToolTipText(
        DependencyObject d,
        string value)
        => d.SetValue(ToolTipTextProperty, value);

    public static readonly DependencyProperty WatermarkProperty = DependencyProperty.RegisterAttached(
        "Watermark",
        typeof(string),
        typeof(TextBoxHelper),
        new UIPropertyMetadata(string.Empty));

    public static string GetWatermark(
        DependencyObject d)
        => (string)d.GetValue(WatermarkProperty);

    public static void SetWatermark(
        DependencyObject d,
        string value)
        => d.SetValue(WatermarkProperty, value);

    public static readonly DependencyProperty WatermarkAlignmentProperty = DependencyProperty.RegisterAttached(
        "WatermarkAlignment",
        typeof(TextAlignment),
        typeof(TextBoxHelper),
        new FrameworkPropertyMetadata(
            TextAlignment.Left,
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static TextAlignment GetWatermarkAlignment(
        DependencyObject d)
        => (TextAlignment)d.GetValue(WatermarkAlignmentProperty);

    public static void SetWatermarkAlignment(
        DependencyObject d,
        TextAlignment value)
        => d.SetValue(WatermarkAlignmentProperty, value);

    public static readonly DependencyProperty WatermarkTrimmingProperty = DependencyProperty.RegisterAttached(
        "WatermarkTrimming",
        typeof(TextTrimming),
        typeof(TextBoxHelper),
        new FrameworkPropertyMetadata(
            TextTrimming.None,
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

    public static TextTrimming GetWatermarkTrimming(
        DependencyObject d)
        => (TextTrimming)d.GetValue(WatermarkTrimmingProperty);

    public static void SetWatermarkTrimming(
        DependencyObject d,
        TextTrimming value)
        => d.SetValue(WatermarkTrimmingProperty, value);

    public static readonly DependencyProperty WatermarkWrappingProperty = DependencyProperty.RegisterAttached(
        "WatermarkWrapping",
        typeof(TextWrapping),
        typeof(TextBoxHelper),
        new FrameworkPropertyMetadata(
            TextWrapping.NoWrap,
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

    public static TextWrapping GetWatermarkWrapping(
        DependencyObject d)
        => (TextWrapping)d.GetValue(WatermarkWrappingProperty);

    public static void SetWatermarkWrapping(
        DependencyObject d,
        TextWrapping value)
        => d.SetValue(WatermarkWrappingProperty, value);

    public static readonly DependencyProperty SelectAllOnFocusProperty
        = DependencyProperty.RegisterAttached(
            "SelectAllOnFocus",
            typeof(bool),
            typeof(TextBoxHelper),
            new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

    public static bool GetSelectAllOnFocus(DependencyObject obj)
        => (bool)obj.GetValue(SelectAllOnFocusProperty);

    public static void SetSelectAllOnFocus(DependencyObject obj, bool value)
        => obj.SetValue(SelectAllOnFocusProperty, BooleanBoxes.Box(value));

    public static readonly DependencyProperty IsMonitoringProperty
        = DependencyProperty.RegisterAttached(
            "IsMonitoring",
            typeof(bool),
            typeof(TextBoxHelper),
            new UIPropertyMetadata(
                BooleanBoxes.FalseBox,
                OnIsMonitoringChanged));

    public static bool GetIsMonitoring(UIElement element)
        => (bool)element.GetValue(IsMonitoringProperty);

    public static void SetIsMonitoring(DependencyObject obj, bool value)
        => obj.SetValue(IsMonitoringProperty, BooleanBoxes.Box(value));

    public static readonly DependencyPropertyKey TextLengthPropertyKey = DependencyProperty.RegisterAttachedReadOnly(
        "TextLength",
        typeof(int),
        typeof(TextBoxHelper),
        new PropertyMetadata(0));

    public static readonly DependencyProperty TextLengthProperty = TextLengthPropertyKey.DependencyProperty;

    private static void TextChanged(
        object sender,
        RoutedEventArgs e)
        => SetTextLength(sender as TextBox, textBox => textBox.Text.Length);

    private static void PasswordChanged(
        object sender,
        RoutedEventArgs e)
        => SetTextLength(sender as PasswordBox, passwordBox => passwordBox.Password.Length);

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

    private static void SetRichTextBoxTextLength(
        RichTextBox richTextBox)
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
            comboBox.SetCurrentValue(HasTextProperty, BooleanBoxes.Box(!string.IsNullOrWhiteSpace(comboBox.Text) || comboBox.SelectedItem != null));
        }
    }

    private static void OnIsMonitoringChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not PasswordBox passBox)
        {
            return;
        }

        if ((bool)e.NewValue)
        {
            _ = passBox.BeginInvoke(() =>
            {
                PasswordChanged(passBox, new RoutedEventArgs(PasswordBox.PasswordChangedEvent, passBox));
            });

            passBox.PasswordChanged += PasswordChanged;
            passBox.GotFocus += PasswordGotFocus;
            passBox.PreviewMouseLeftButtonDown += UiElementPreviewMouseLeftButtonDown;
        }
        else
        {
            passBox.PasswordChanged -= PasswordChanged;
            passBox.GotFocus -= PasswordGotFocus;
            passBox.PreviewMouseLeftButtonDown -= UiElementPreviewMouseLeftButtonDown;
        }
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
        => ControlGotFocus(sender as PasswordBox, passwordBox => passwordBox.SelectAll());

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