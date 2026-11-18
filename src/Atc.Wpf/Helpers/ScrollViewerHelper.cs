namespace Atc.Wpf.Helpers;

/// <summary>Provides attached properties for a <see cref="ScrollViewer"/>, such as the content presenter margin and placing the vertical scroll bar on the left side.</summary>
[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static partial class ScrollViewerHelper
{
    /// <summary>Gets or sets the value of the ScrollContentPresenterMargin attached property.</summary>
    [AttachedProperty(TargetType = typeof(ScrollViewer), ElementParameterName = "scrollViewer", NullElementHandling = NullElementHandling.None)]
    private static Thickness scrollContentPresenterMargin;

    /// <summary>Gets or sets the value of the VerticalScrollBarOnLeftSide attached property.</summary>
    [AttachedProperty(
        DefaultValue = false,
        Flags = FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.Inherits,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static bool verticalScrollBarOnLeftSide;
}