// ReSharper disable once CheckNamespace
namespace Atc.Wpf;

/// <summary>
/// Flippable Interface
/// </summary>
public interface IFlippable
{
    /// <summary>
    /// Gets or sets the current orientation (horizontal, vertical).
    /// </summary>
    FlipOrientationType FlipOrientation { get; set; }
}