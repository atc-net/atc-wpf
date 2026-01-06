// ReSharper disable once CheckNamespace
namespace Atc.Wpf;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct PanelUvSize : IEquatable<PanelUvSize>
{
    private readonly Orientation orientation;

    public PanelUvSize(Orientation orientation)
    {
        U = V = 0d;
        this.orientation = orientation;
    }

    public PanelUvSize(
        Orientation orientation,
        Size size)
    {
        U = V = 0d;
        this.orientation = orientation;
        Width = size.Width;
        Height = size.Height;
    }

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

    public Size ScreenSize => new(U, V);

    public double U { get; set; }

    public double V { get; set; }

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

    public static bool operator ==(PanelUvSize left, PanelUvSize right)
        => left.Equals(right);

    public static bool operator !=(PanelUvSize left, PanelUvSize right)
        => !(left == right);

    public readonly bool Equals(PanelUvSize other)
        => U.Equals(other.U) &&
           V.Equals(other.V);

    public override readonly bool Equals(object? obj)
        => obj is PanelUvSize x && Equals(x);

    public override readonly int GetHashCode()
        => HashCode.Combine((int)orientation, U, V);
}