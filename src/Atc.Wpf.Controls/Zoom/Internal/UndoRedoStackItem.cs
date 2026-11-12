namespace Atc.Wpf.Controls.Zoom.Internal;

internal sealed class UndoRedoStackItem(
    double offsetX,
    double offsetY,
    double width,
    double height,
    double zoom) : IEquatable<UndoRedoStackItem>
{
    public Rect Rect { get; } = new(offsetX, offsetY, width, height);

    public double Zoom { get; } = zoom;

    public override string ToString()
        => $"Rectangle ({Rect.X},{Rect.Y}), Zoom {Zoom}";

    public override bool Equals(object? obj)
        => obj is UndoRedoStackItem item &&
           Equals(item);

    public bool Equals(UndoRedoStackItem? other)
    {
        if (other is null)
        {
            return false;
        }

        return Zoom.IsWithinOnePercent(other.Zoom) &&
               Rect.Equals(other.Rect);
    }

    public override int GetHashCode()
        => Rect.GetHashCode() + Zoom.GetHashCode();
}