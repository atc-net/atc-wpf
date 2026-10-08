namespace Atc.Wpf.Controls.Navigation;

/// <summary>
/// An entry in the pane of a <see cref="NavigationView"/>.
/// </summary>
/// <remarks>
/// Set <see cref="TargetViewModelType"/> to navigate through the view's <see cref="INavigationService"/>
/// when the item is invoked, or handle <see cref="NavigationView.ItemInvoked"/> or <see cref="ButtonBase.Command"/>.
/// </remarks>
public partial class NavigationViewItem : ButtonBase
{
    /// <summary>
    /// The icon shown in the compact part of the pane.
    /// </summary>
    [DependencyProperty]
    private object? icon;

    /// <summary>
    /// The ViewModel type to navigate to when the item is invoked.
    /// </summary>
    [DependencyProperty]
    private Type? targetViewModelType;

    /// <summary>
    /// The parameters passed with the navigation to <see cref="TargetViewModelType"/>.
    /// </summary>
    [DependencyProperty]
    private NavigationParameters? navigationParameters;

    /// <summary>
    /// Whether invoking the item selects it. Set to false for items that only run a command,
    /// such as opening a dialog.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool selectsOnInvoked;

    /// <summary>
    /// Whether the item is the selected item of its <see cref="NavigationView"/>. Set by the view.
    /// </summary>
    [DependencyProperty]
    private bool isSelected;

    static NavigationViewItem()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(NavigationViewItem),
            new FrameworkPropertyMetadata(typeof(NavigationViewItem)));
    }

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer()
        => new NavigationViewItemAutomationPeer(this);

    internal void AutomationInvoke()
        => OnClick();
}