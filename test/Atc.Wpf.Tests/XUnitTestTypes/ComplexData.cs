namespace Atc.Wpf.Tests.XUnitTestTypes;

public class ComplexData
{
    public Color? MyColor { get; set; }

    public SolidColorBrush? MyBrush { get; set; }

    public override string ToString()
        => $"{nameof(MyColor)}: {MyColor}, {nameof(MyBrush)}: {MyBrush}";
}