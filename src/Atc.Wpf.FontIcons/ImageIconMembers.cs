namespace Atc.Wpf.FontIcons;

// The dependency property that every image icon control declares in addition to IconTransformMembers.
// The Icon property is not here: its default value is the None member of the control's own icon enumeration.
[DependencyPropertyTemplate]
internal abstract class ImageIconMembers
{
    /// <summary>
    /// Gets or sets the foreground brush of the icon. Changing this property will cause the icon to be redrawn.
    /// </summary>
    [DependencyProperty(DefaultValueExpression = "Brushes.Black", PropertyChangedCallback = "OnIconPropertyChanged")]
    private Brush foreground;
}