namespace Atc.Wpf.Helpers;

/// <summary>Provides attached properties for text input controls, such as watermarks, a clear-text button, select-all on focus and text length monitoring.</summary>
[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static partial class TextBoxHelper
{
    /// <summary>Gets or sets the value of the ButtonHeight attached property.</summary>
    [AttachedProperty(
        DefaultValue = 22d,
        Flags = FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static double buttonHeight;

    /// <summary>Gets or sets the value of the ButtonWidth attached property.</summary>
    [AttachedProperty(
        DefaultValue = 22d,
        Flags = FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static double buttonWidth;

    /// <summary>Gets or sets the value of the ButtonsAlignment attached property.</summary>
    [AttachedProperty(
        DefaultValue = ButtonsAlignmentType.Right,
        Flags = FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static ButtonsAlignmentType buttonsAlignment;

    /// <summary>Gets or sets the value of the ButtonCommand attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        PropertyChangedCallback = nameof(ButtonCommandOrClearTextChanged),
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static ICommand? buttonCommand;

    /// <summary>Gets or sets the value of the ButtonCommandParameter attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static object? buttonCommandParameter;

    /// <summary>Gets or sets the value of the ButtonCommandTarget attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static IInputElement? buttonCommandTarget;

    /// <summary>Gets or sets the value of the ButtonContent attached property.</summary>
    [AttachedProperty(
        DefaultValueExpression = "\"#\"",
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static object buttonContent;

    /// <summary>Gets or sets the value of the ButtonContentTemplate attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static DataTemplate? buttonContentTemplate;

    /// <summary>Gets or sets the value of the ButtonTemplate attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static ControlTemplate? buttonTemplate;

    /// <summary>Gets or sets the value of the ButtonFontFamily attached property.</summary>
    [AttachedProperty(
        DefaultValueExpression = "new FontFamilyConverter().ConvertFromString(\"Segoe UI\")",
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static FontFamily buttonFontFamily;

    /// <summary>Gets or sets the value of the ButtonFontSize attached property.</summary>
    [AttachedProperty(
        DefaultValueExpression = "SystemFonts.MessageFontSize",
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static double buttonFontSize;

    /// <summary>Gets or sets the value of the HasText attached property.</summary>
    [AttachedProperty(
        DefaultValue = false,
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static bool hasText;

    /// <summary>Gets or sets the value of the ClearTextButton attached property.</summary>
    [AttachedProperty(
        DefaultValue = false,
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        PropertyChangedCallback = nameof(ButtonCommandOrClearTextChanged),
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static bool clearTextButton;

    /// <summary>Gets or sets the value of the UseFloatingWatermark attached property.</summary>
    [AttachedProperty(
        DefaultValue = false,
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        PropertyChangedCallback = nameof(ButtonCommandOrClearTextChanged),
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static bool useFloatingWatermark;

    /// <summary>Gets or sets the value of the HasValidationError attached property.</summary>
    [AttachedProperty(
        DefaultValue = false,
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static bool hasValidationError;

    /// <summary>Gets or sets the value of the ShowToolTip attached property.</summary>
    [AttachedProperty(
        DefaultValue = false,
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static bool showToolTip;

    /// <summary>Gets or sets the value of the ToolTipText attached property.</summary>
    [AttachedProperty(
        DefaultValue = "",
        MetadataType = DependencyPropertyMetadataType.UIPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static string toolTipText;

    /// <summary>Gets or sets the value of the Watermark attached property.</summary>
    [AttachedProperty(
        DefaultValue = "",
        MetadataType = DependencyPropertyMetadataType.UIPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static string watermark;

    /// <summary>Gets or sets the value of the WatermarkAlignment attached property.</summary>
    [AttachedProperty(
        DefaultValue = TextAlignment.Left,
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static TextAlignment watermarkAlignment;

    /// <summary>Gets or sets the value of the WatermarkTrimming attached property.</summary>
    [AttachedProperty(
        DefaultValue = TextTrimming.None,
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static TextTrimming watermarkTrimming;

    /// <summary>Gets or sets the value of the WatermarkWrapping attached property.</summary>
    [AttachedProperty(
        DefaultValue = TextWrapping.NoWrap,
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static TextWrapping watermarkWrapping;

    /// <summary>Gets or sets the value of the SelectAllOnFocus attached property.</summary>
    [AttachedProperty(
        DefaultValue = false,
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "obj",
        NullElementHandling = NullElementHandling.None)]
    private static bool selectAllOnFocus;

    /// <summary>Identifies the IsMonitoring attached property.</summary>
    [AttachedProperty(
        DefaultValue = false,
        MetadataType = DependencyPropertyMetadataType.UIPropertyMetadata,
        PropertyChangedCallback = nameof(OnIsMonitoringChanged),
        GenerateClrAccessors = false)]
    private static bool isMonitoring;

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
    [AttachedProperty(
        PropertyChangedCallback = nameof(OnComboBoxTextProxyChanged),
        AccessModifier = PropertyAccessModifier.Private,
        GenerateClrAccessors = false)]
    private static string comboBoxTextProxy;

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
        var monitoring = (bool)e.NewValue;
        if (d is PasswordBox passwordBox)
        {
            MonitorPasswordBox(passwordBox, monitoring);
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
        if (monitoring && !element.IsLoaded)
        {
            element.Loaded += MonitorWhenLoaded;
            return;
        }

        Monitor(element, monitoring);
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