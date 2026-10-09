namespace Atc.Wpf.Theming.Controls.Windows;

public partial class NiceWindow
{
    /// <summary>
    /// Identifies the <see cref="IsBackdropActive"/> dependency property key.
    /// </summary>
    private static readonly DependencyPropertyKey IsBackdropActivePropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(IsBackdropActive),
        typeof(bool),
        typeof(NiceWindow),
        new PropertyMetadata(defaultValue: false));

    /// <summary>
    /// Identifies the <see cref="IsBackdropActive"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsBackdropActiveProperty = IsBackdropActivePropertyKey.DependencyProperty;

    /// <summary>
    /// The Windows 11 system backdrop (Mica, Acrylic or Tabbed) drawn behind the window.
    /// While it is applied, the window and title bar backgrounds are transparent so the backdrop shows through.
    /// Windows that do not support it (Windows 10, <see cref="Window.AllowsTransparency"/>) keep their theme background.
    /// </summary>
    [DependencyProperty(
        DefaultValue = WindowBackdropType.None,
        PropertyChangedCallback = nameof(OnBackdropTypePropertyChanged))]
    private WindowBackdropType backdropType;

    /// <summary>
    /// Gets a value indicating whether Windows applied the <see cref="BackdropType"/> to this window.
    /// </summary>
    public bool IsBackdropActive => (bool)GetValue(IsBackdropActiveProperty);

    /// <inheritdoc />
    protected override void OnPropertyChanged(
        DependencyPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (ReferenceEquals(e.Property, WindowBackdropManager.CurrentBackdropTypeProperty))
        {
            SetValue(IsBackdropActivePropertyKey, e.NewValue is not WindowBackdropType.None);
        }
        else if (ReferenceEquals(e.Property, StyleProperty))
        {
            RemoveFluentWindowStyle();
        }
        else if (ReferenceEquals(e.Property, OverrideDefaultWindowCommandsBrushProperty))
        {
            // The window commands take this brush when they are themed, so re-theme them on a change
            // (for example when a backdrop turns on and the title bar is no longer accent coloured).
            this.ResetAllWindowCommandsBrush();
        }
    }

    private static void OnBackdropTypePropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => WindowBackdropManager.SetBackdropType((Window)d, (WindowBackdropType)e.NewValue);
}