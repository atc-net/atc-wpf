namespace Atc.Wpf.Helpers;

/// <summary>Provides attached properties for a <see cref="PasswordBox"/>, such as a bindable password, a caps lock warning and a reveal-text button.</summary>
[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static partial class PasswordBoxHelper
{
    /// <summary>Gets or sets the value of the BoundPassword attached property.</summary>
    [AttachedProperty(
        DefaultValue = "",
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        PropertyChangedCallback = nameof(OnBoundPasswordChanged),
        CoerceValueCallback = nameof(CoerceBoundPassword),
        ElementParameterName = "obj",
        NullElementHandling = NullElementHandling.None)]
    private static string boundPassword;

    /// <summary>Gets or sets the value of the CapsLockIcon attached property.</summary>
    [AttachedProperty(
        DefaultValueExpression = "\"!\"",
        PropertyChangedCallback = nameof(OnCapsLockIconPropertyChanged),
        TargetType = typeof(PasswordBox),
        NullElementHandling = NullElementHandling.None)]
    private static object capsLockIcon;

    /// <summary>Gets or sets the value of the CapsLockWarningToolTip attached property.</summary>
    [AttachedProperty(DefaultValueExpression = "\"Caps lock is on\"", TargetType = typeof(PasswordBox), NullElementHandling = NullElementHandling.None)]
    private static object capsLockWarningToolTip;

    /// <summary>Identifies the RevealTextButtonContent attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        GenerateClrAccessors = false)]
    private static object? revealTextButtonContent;

    /// <summary>Gets the value of the RevealTextButtonContent attached property.</summary>
    public static object? GetRevealTextButtonContent(DependencyObject d)
        => (object?)d.GetValue(RevealTextButtonContentProperty);

    /// <summary>Sets the value of the RevealTextButtonContent attached property.</summary>
    public static void SetRevealTextButtonContent(
        DependencyObject obj,
        object? value)
        => obj.SetValue(
            RevealTextButtonContentProperty,
            value);

    /// <summary>Identifies the RevealTextButtonContentTemplate attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        GenerateClrAccessors = false)]
    private static DataTemplate? revealTextButtonContentTemplate;

    /// <summary>Gets the value of the RevealTextButtonContentTemplate attached property.</summary>
    public static DataTemplate? GetRevealTextButtonContentTemplate(
        DependencyObject d)
        => (DataTemplate?)d.GetValue(RevealTextButtonContentTemplateProperty);

    /// <summary>Sets the value of the RevealTextButtonContentTemplate attached property.</summary>
    public static void SetRevealTextButtonContentTemplate(
        DependencyObject obj,
        DataTemplate? value)
        => obj.SetValue(
            RevealTextButtonContentTemplateProperty,
            value);

    private static object CoerceBoundPassword(
        DependencyObject d,
        object baseValue)
    {
        // Coercion runs for every value the property receives, including one that equals the
        // current value (for which no change callback fires), so this is where the
        // PasswordChanged subscription is guaranteed to exist as soon as BoundPassword is in use.
        if (d is PasswordBox pb)
        {
            pb.PasswordChanged -= PasswordChanged;
            pb.PasswordChanged += PasswordChanged;
        }

        return baseValue;
    }

    private static void OnBoundPasswordChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not PasswordBox pb)
        {
            return;
        }

        pb.PasswordChanged -= PasswordChanged;
        if (pb.Password != (string)e.NewValue)
        {
            pb.Password = (string)e.NewValue;
        }

        pb.PasswordChanged += PasswordChanged;
    }

    private static void PasswordChanged(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is PasswordBox pb)
        {
            // SetCurrentValue keeps a one-way binding alive; SetValue would replace it.
            pb.SetCurrentValue(
                BoundPasswordProperty,
                pb.Password);
        }
    }

    private static void OnCapsLockIconPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue == e.OldValue)
        {
            return;
        }

        var pb = (PasswordBox)d;

        pb.KeyDown -= RefreshCapsLockStatus;
        pb.GotFocus -= RefreshCapsLockStatus;
        pb.PreviewGotKeyboardFocus -= RefreshCapsLockStatus;
        pb.LostFocus -= HandlePasswordBoxLostFocus;

        if (e.NewValue is null)
        {
            return;
        }

        pb.KeyDown += RefreshCapsLockStatus;
        pb.GotFocus += RefreshCapsLockStatus;
        pb.PreviewGotKeyboardFocus += RefreshCapsLockStatus;
        pb.LostFocus += HandlePasswordBoxLostFocus;
    }

    private static void RefreshCapsLockStatus(
        object sender,
        RoutedEventArgs e)
    {
        var fe = FindCapsLockIndicator(sender as Control);
        if (fe != null)
        {
            fe.Visibility = Keyboard.IsKeyToggled(Key.CapsLock) ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    private static void HandlePasswordBoxLostFocus(
        object sender,
        RoutedEventArgs e)
    {
        var fe = FindCapsLockIndicator(sender as Control);
        if (fe != null)
        {
            fe.Visibility = Visibility.Collapsed;
        }
    }

    private static FrameworkElement? FindCapsLockIndicator(Control? pb)
        => pb?
            .Template?
            .FindName(
                "PART_CapsLockIndicator",
                pb) as FrameworkElement;
}