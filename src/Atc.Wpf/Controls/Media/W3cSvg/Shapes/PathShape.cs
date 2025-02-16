// ReSharper disable once StringLiteralTypo
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Atc.Wpf.Controls.Media.W3cSvg.Shapes;

[SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "OK.")]
[SuppressMessage("Maintainability", "S1144:Unused private types or members should be removed", Justification = "OK.")]
internal sealed class PathShape : Shape
{
    internal sealed class CommandSplitter
    {
        // Command is from one non numeric character to the next (-.,space   is part of the numeric value since it defines a point)
        private readonly char[] commands = { 'm', 'M', 'z', 'Z', 'A', 'a', 'L', 'l', 'h', 'H', 'v', 'V', 'c', 'C', 's', 'S', 'q', 'Q' };
        private readonly string val;
        private readonly StringSplitter splitter = new(string.Empty);
        private int curPos = -1;

        public CommandSplitter(string value)
        {
            val = value;
        }

        public string ReadNext()
        {
            var startPos = curPos;
            if (startPos < 0)
            {
                startPos = 0;
            }

            if (startPos >= val.Length)
            {
                return string.Empty;
            }

            var cmdStart = val.IndexOfAny(commands, startPos);
            var cmdEnd = cmdStart;
            if (cmdStart >= 0)
            {
                cmdEnd = val.IndexOfAny(commands, cmdStart + 1);
            }

            if (cmdEnd < 0)
            {
                var len = val.Length - startPos;
                curPos = val.Length;
                return val.Substring(startPos, len).Trim();
            }
            else
            {
                var len = cmdEnd - startPos;
                curPos = cmdEnd;
                return val.Substring(startPos, len).Trim();
            }
        }

        public StringSplitter SplitCommand(string command, out char cmd)
        {
            ArgumentNullException.ThrowIfNull(command);

            cmd = command[0];
            splitter.SetString(command, 1);
            return splitter;
        }
    }

    internal class PathElement
    {
        protected PathElement(char command)
        {
            Command = command;
        }

        public char Command { get; }

        public bool IsRelative => char.IsLower(Command);
    }

    internal sealed class MoveTo : PathElement
    {
        public MoveTo(char command, StringSplitter value)
            : base(command)
        {
            ArgumentNullException.ThrowIfNull(value);

            Point = value.ReadNextPoint();
        }

        public Point Point { get; }
    }

    internal sealed class LineTo : PathElement
    {
        public LineTo(char command, StringSplitter value)
            : base(command)
        {
            ArgumentNullException.ThrowIfNull(value);

            if (char.ToLower(command, GlobalizationConstants.EnglishCultureInfo) == 'h')
            {
                PositionType = PathShapeLineToType.Horizontal;
                var v = value.ReadNextValue();
                Points = new[]
                {
                    new Point(v, 0),
                };

                return;
            }

            if (char.ToLower(command, GlobalizationConstants.EnglishCultureInfo) == 'v')
            {
                PositionType = PathShapeLineToType.Vertical;
                var v = value.ReadNextValue();
                Points = new[]
                {
                    new Point(0, v),
                };

                return;
            }

            PositionType = PathShapeLineToType.Point;
            var list = new List<Point>();
            while (value.More)
            {
                var p = value.ReadNextPoint();
                list.Add(p);
            }

            Points = list.ToArray();
        }

        public PathShapeLineToType PositionType { get; }

        public Point[] Points { get; }
    }

    internal sealed class CurveTo : PathElement
    {
        public CurveTo(char command, StringSplitter value)
            : base(command)
        {
            ArgumentNullException.ThrowIfNull(value);

            CtrlPoint1 = value.ReadNextPoint();
            CtrlPoint2 = value.ReadNextPoint();
            Point = value.ReadNextPoint();
        }

        public CurveTo(char command, StringSplitter value, Point ctrlPoint)
            : base(command)
        {
            ArgumentNullException.ThrowIfNull(value);

            CtrlPoint1 = ctrlPoint;
            CtrlPoint2 = value.ReadNextPoint();
            Point = value.ReadNextPoint();
        }

        public Point CtrlPoint1 { get; }

        public Point CtrlPoint2 { get; }

        public Point Point { get; }
    }

    internal sealed class QuadraticCurveTo : PathElement
    {
        public QuadraticCurveTo(char command, StringSplitter value)
            : base(command)
        {
            ArgumentNullException.ThrowIfNull(value);

            CtrlPoint = value.ReadNextPoint();
            Point = value.ReadNextPoint();
        }

        public QuadraticCurveTo(char command, StringSplitter value, Point ctrlPoint)
            : base(command)
        {
            ArgumentNullException.ThrowIfNull(value);

            CtrlPoint = ctrlPoint;
            Point = value.ReadNextPoint();
        }

        public Point CtrlPoint { get; }

        public Point Point { get; }
    }

    internal sealed class EllipticalArcTo : PathElement
    {
        public EllipticalArcTo(char command, StringSplitter value)
            : base(command)
        {
            ArgumentNullException.ThrowIfNull(value);

            Rx = value.ReadNextValue();
            Ry = value.ReadNextValue();
            AxisRotation = value.ReadNextValue();
            var arcFlag = value.ReadNextValue();
            LargeArc = arcFlag > 0;
            var sweepFlag = value.ReadNextValue();
            Clockwise = sweepFlag > 0;
            X = value.ReadNextValue();
            Y = value.ReadNextValue();
        }

        public double Rx { get; }

        public double Ry { get; }

        public double AxisRotation { get; }

        public double X { get; }

        public double Y { get; }

        public bool Clockwise { get; }

        public bool LargeArc { get; }
    }

    private readonly List<PathElement> elements = new();

    private static Fill? defaultFill;

    public override Fill? Fill => base.Fill ?? defaultFill;

    public IList<PathElement> Elements => elements.AsReadOnly();

    public bool ClosePath { get; }

    public string? Data { get; }

    [SuppressMessage("Major Code Smell", "S3010:Static fields should not be updated in constructors", Justification = "OK.")]
    internal PathShape(Svg svg, XmlNode node, Shape parent)
        : base(svg, node, parent)
    {
        ArgumentNullException.ThrowIfNull(svg);

        defaultFill ??= new Fill
        {
            PaintServerKey = svg.PaintServers.Parse("black"),
        };

        ClosePath = false;
        var path = SvgXmlUtil.AttrValue(node, "d", string.Empty);
        Data = path;
    }
}