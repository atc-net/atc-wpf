namespace Atc.Wpf.Theming.Controls.Windows;

/// <summary>
/// Provides data for the <see cref="WindowButtonCommands.ClosingWindow"/> event.
/// </summary>
public sealed class ClosingWindowEventArgs : EventArgs
{
    /// <summary>
    /// Gets or sets a value indicating whether closing the window should be cancelled.
    /// </summary>
    public bool Cancelled { get; set; }
}