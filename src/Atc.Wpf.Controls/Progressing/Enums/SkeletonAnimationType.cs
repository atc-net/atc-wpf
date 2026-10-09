// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Controls.Progressing;

/// <summary>Specifies the loading animation of a <see cref="SkeletonElement"/>.</summary>
public enum SkeletonAnimationType
{
    /// <summary>A moving highlight gradient.</summary>
    Shimmer,

    /// <summary>A fading in and out effect.</summary>
    Pulse,

    /// <summary>No animation; a static placeholder.</summary>
    None,
}