namespace Atc.Wpf.Controls.DataGrids;

/// <summary>
/// Represents the method that handles the <see cref="DataGridFilter"/> <c>FilterChanged</c> attached event.
/// </summary>
/// <param name="sender">The object where the handler is attached.</param>
/// <param name="e">The event data.</param>
[SuppressMessage("Design", "CA1003:Use generic event handler instances", Justification = "Routed event handler.")]
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "Routed event handler.")]
public delegate void DataGridFilterChangedEventHandler(
    object sender,
    DataGridFilterChangedEventArgs e);