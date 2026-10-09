// ReSharper disable once CheckNamespace
namespace Atc.Wpf;

/// <summary>
/// Represents a size in orientation-relative coordinates, where <see cref="U"/> is the extent along
/// the stacking direction and <see cref="V"/> the extent across it.
/// </summary>
[Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct PanelUvSize : IEquatable<PanelUvSize>
{
    private readonly Orientation orientation;

    /// <summary>
    /// Initializes a new instance of the <see cref="PanelUvSize"/> struct with a zero size.
    /// </summary>
    /// <param name="orientation">The orientation that maps width and height to U and V.</param>
    public PanelUvSize(Orientation orientation)
    {
        U = V = 0d;
        this.orientation = orientation;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PanelUvSize"/> struct from a <see cref="Size"/>.
    /// </summary>
    /// <param name="orientation">The orientation that maps width and height to U and V.</param>
    /// <param name="size">The size in screen coordinates.</param>
    public PanelUvSize(
        Orientation orientation,
        Size size)
    {
        U = V = 0d;
        this.orientation = orientation;
        Width = size.Width;
        Height = size.Height;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PanelUvSize"/> struct from a width and height.
    /// </summary>
    /// <param name="orientation">The orientation that maps width and height to U and V.</param>
    /// <param name="width">The width in screen coordinates.</param>
    /// <param name="height">The height in screen coordinates.</param>
    public PanelUvSize(
        Orientation orientation,
        double width,
        double height)
    {
        U = V = 0d;
        this.orientation = orientation;
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Gets a <see cref="Size"/> built from <see cref="U"/> (width) and <see cref="V"/> (height).
    /// </summary>
    public Size ScreenSize => new(U, V);

    /// <summary>
    /// Gets or sets the extent along the orientation (width when horizontal, height when vertical).
    /// </summary>
    public double U { get; set; }

    /// <summary>
    /// Gets or sets the extent across the orientation (height when horizontal, width when vertical).
    /// </summary>
    public double V { get; set; }

    /// <summary>
    /// Gets the width in screen coordinates.
    /// </summary>
    public double Width
    {
        readonly get => orientation == Orientation.Horizontal ? U : V;
        private set
        {
            if (orientation == Orientation.Horizontal)
            {
                U = value;
            }
            else
            {
                V = value;
            }
        }
    }

    /// <summary>
    /// Gets the height in screen coordinates.
    /// </summary>
    public double Height
    {
        readonly get => orientation == Orientation.Horizontal ? V : U;
        private set
        {
            if (orientation == Orientation.Horizontal)
            {
                V = value;
            }
            else
            {
                U = value;
            }
        }
    }

    /// <summary>
    /// Determines whether two <see cref="PanelUvSize"/> values are equal.
    /// </summary>
    public static bool operator ==(PanelUvSize left, PanelUvSize right)
        => left.Equals(right);

    /// <summary>
    /// Determines whether two <see cref="PanelUvSize"/> values are not equal.
    /// </summary>
    public static bool operator !=(PanelUvSize left, PanelUvSize right)
        => !(left == right);

    /// <inheritdoc />
    public readonly bool Equals(PanelUvSize other)
        => U.IsEqual(other.U) &&
           V.IsEqual(other.V);

    /// <inheritdoc />
    public override readonly bool Equals(object? obj)
        => obj is PanelUvSize x && Equals(x);

    /// <inheritdoc />
    public override readonly int GetHashCode()
        => HashCode.Combine((int)orientation, U, V);
}