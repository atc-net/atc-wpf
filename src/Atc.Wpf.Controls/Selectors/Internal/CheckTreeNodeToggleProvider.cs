#pragma warning disable IDE0130
namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// The toggle pattern of a <see cref="CheckTreeViewItem"/> container (for clients that talk to the container peer).
/// </summary>
internal sealed class CheckTreeNodeToggleProvider(CheckTreeViewItem container) : IToggleProvider
{
    /// <inheritdoc />
    public ToggleState ToggleState
        => container.Node?.IsChecked switch
        {
            true => ToggleState.On,
            false => ToggleState.Off,
            _ => ToggleState.Indeterminate,
        };

    /// <inheritdoc />
    public void Toggle()
    {
        if (container.Node is { } node)
        {
            if (!node.IsEnabled || !container.OwnerView.IsEnabled)
            {
                throw new ElementNotEnabledException();
            }

            container.OwnerView.ToggleNode(node);
        }
    }
}