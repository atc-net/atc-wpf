namespace Atc.Wpf.Theming.Controls.Selectors;

/// <summary>
/// Interaction logic for ThemeAndAccentColorSelectors.xaml
/// </summary>
public partial class ThemeAndAccentColorSelectors
{
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
        nameof(Orientation),
        typeof(Orientation),
        typeof(ThemeAndAccentColorSelectors),
        new PropertyMetadata(default(Orientation)));

    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public static readonly DependencyProperty LabelControlOrientationProperty = DependencyProperty.Register(
        nameof(LabelControlOrientation),
        typeof(Orientation),
        typeof(ThemeAndAccentColorSelectors),
        new PropertyMetadata(default(Orientation)));

    public Orientation LabelControlOrientation
    {
        get => (Orientation)GetValue(LabelControlOrientationProperty);
        set => SetValue(LabelControlOrientationProperty, value);
    }

    public ThemeAndAccentColorSelectors()
    {
        InitializeComponent();

        DataContext = this;
    }
}