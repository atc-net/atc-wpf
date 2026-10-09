namespace Atc.Wpf.Helpers;

/// <summary>Provides attached properties for a <see cref="PasswordBox"/>, such as a bindable password, a caps lock warning and a reveal-text button.</summary>
[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static class PasswordBoxHelper
{
    /// <summary>Identifies the BoundPassword attached property.</summary>
    public static readonly DependencyProperty BoundPasswordProperty = DependencyProperty.RegisterAttached(
        "BoundPassword",
        typeof(string),
        typeof(PasswordBoxHelper),
        new FrameworkPropertyMetadata(
            string.Empty,
            OnBoundPasswordChanged));

    /// <summary>Gets the value of the BoundPassword attached property.</summary>
    public static string GetBoundPassword(DependencyObject obj)
        => (string)obj.GetValue(BoundPasswordProperty);

    /// <summary>Sets the value of the BoundPassword attached property.</summary>
    public static void SetBoundPassword(
        DependencyObject obj,
        string value)
        => obj.SetValue(
            BoundPasswordProperty,
            value);

    /// <summary>Identifies the CapsLockIcon attached property.</summary>
    public static readonly DependencyProperty CapsLockIconProperty = DependencyProperty.RegisterAttached(
        "CapsLockIcon",
        typeof(object),
        typeof(PasswordBoxHelper),
        new PropertyMetadata(
            "!",
            OnCapsLockIconPropertyChanged));

    /// <summary>Gets the value of the CapsLockIcon attached property.</summary>
    public static object GetCapsLockIcon(PasswordBox element)
        => element.GetValue(CapsLockIconProperty);

    /// <summary>Sets the value of the CapsLockIcon attached property.</summary>
    public static void SetCapsLockIcon(
        PasswordBox element,
        object value)
        => element.SetValue(
            CapsLockIconProperty,
            value);

    /// <summary>Identifies the CapsLockWarningToolTip attached property.</summary>
    public static readonly DependencyProperty CapsLockWarningToolTipProperty = DependencyProperty.RegisterAttached(
        "CapsLockWarningToolTip",
        typeof(object),
        typeof(PasswordBoxHelper),
        new PropertyMetadata("Caps lock is on"));

    /// <summary>Gets the value of the CapsLockWarningToolTip attached property.</summary>
    public static object GetCapsLockWarningToolTip(PasswordBox element)
        => element.GetValue(CapsLockWarningToolTipProperty);

    /// <summary>Sets the value of the CapsLockWarningToolTip attached property.</summary>
    public static void SetCapsLockWarningToolTip(
        PasswordBox element,
        object value)
        => element.SetValue(
            CapsLockWarningToolTipProperty,
            value);

    /// <summary>Identifies the RevealTextButtonContent attached property.</summary>
    public static readonly DependencyProperty RevealTextButtonContentProperty = DependencyProperty.RegisterAttached(
        "RevealTextButtonContent",
        typeof(object),
        typeof(PasswordBoxHelper),
        new FrameworkPropertyMetadata(propertyChangedCallback: null));

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
    public static readonly DependencyProperty RevealTextButtonContentTemplateProperty = DependencyProperty.RegisterAttached(
            "RevealTextButtonContentTemplate",
            typeof(DataTemplate),
            typeof(PasswordBoxHelper),
            new FrameworkPropertyMetadata(propertyChangedCallback: null));

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
            SetBoundPassword(
                pb,
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