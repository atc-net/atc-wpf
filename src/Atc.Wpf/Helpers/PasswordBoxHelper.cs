namespace Atc.Wpf.Helpers;

[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static class PasswordBoxHelper
{
    public static readonly DependencyProperty BoundPasswordProperty = DependencyProperty.RegisterAttached(
        "BoundPassword",
        typeof(string),
        typeof(PasswordBoxHelper),
        new FrameworkPropertyMetadata(
            string.Empty,
            OnBoundPasswordChanged));

    public static string GetBoundPassword(
        DependencyObject obj)
        => (string)obj.GetValue(BoundPasswordProperty);

    public static void SetBoundPassword(
        DependencyObject obj,
        string value)
        => obj.SetValue(BoundPasswordProperty, value);

    public static readonly DependencyProperty CapsLockIconProperty = DependencyProperty.RegisterAttached(
        "CapsLockIcon",
        typeof(object),
        typeof(PasswordBoxHelper),
        new PropertyMetadata(
            "!",
            OnCapsLockIconPropertyChanged));

    public static object GetCapsLockIcon(
        PasswordBox element)
        => element.GetValue(CapsLockIconProperty);

    public static void SetCapsLockIcon(
        PasswordBox element,
        object value)
        => element.SetValue(CapsLockIconProperty, value);

    public static readonly DependencyProperty CapsLockWarningToolTipProperty = DependencyProperty.RegisterAttached(
        "CapsLockWarningToolTip",
        typeof(object),
        typeof(PasswordBoxHelper),
        new PropertyMetadata("Caps lock is on"));

    public static object GetCapsLockWarningToolTip(
        PasswordBox element)
        => element.GetValue(CapsLockWarningToolTipProperty);

    public static void SetCapsLockWarningToolTip(
        PasswordBox element,
        object value)
        => element.SetValue(CapsLockWarningToolTipProperty, value);

    public static readonly DependencyProperty RevealTextButtonContentProperty = DependencyProperty.RegisterAttached(
        "RevealTextButtonContent",
        typeof(object),
        typeof(PasswordBoxHelper),
        new FrameworkPropertyMetadata(propertyChangedCallback: null));

    public static object? GetRevealTextButtonContent(
        DependencyObject d)
        => (object?)d.GetValue(RevealTextButtonContentProperty);

    public static void SetRevealTextButtonContent(
        DependencyObject obj,
        object? value)
        => obj.SetValue(RevealTextButtonContentProperty, value);

    public static readonly DependencyProperty RevealTextButtonContentTemplateProperty = DependencyProperty.RegisterAttached(
            "RevealTextButtonContentTemplate",
            typeof(DataTemplate),
            typeof(PasswordBoxHelper),
            new FrameworkPropertyMetadata(propertyChangedCallback: null));

    public static DataTemplate? GetRevealTextButtonContentTemplate(
        DependencyObject d)
        => (DataTemplate?)d.GetValue(RevealTextButtonContentTemplateProperty);

    public static void SetRevealTextButtonContentTemplate(
        DependencyObject obj,
        DataTemplate? value)
        => obj.SetValue(RevealTextButtonContentTemplateProperty, value);

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
            SetBoundPassword(pb, pb.Password);
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

    private static FrameworkElement? FindCapsLockIndicator(
        Control? pb)
    {
        return pb?.Template?.FindName("PART_CapsLockIndicator", pb) as FrameworkElement;
    }
}