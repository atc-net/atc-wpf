// ReSharper disable once StringLiteralTypo
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Atc.Wpf.Controls.W3cSvg.Shapes;

[SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "OK.")]
internal class PathShape : Shape
{
    internal class CommandSplitter
    {
        // Command is from one non numeric character to the next (-.,space   is part of the numeric value since it defines a point)
        private readonly char[] commands = { 'm', 'M', 'z', 'Z', 'A', 'a', 'L', 'l', 'h', 'H', 'v', 'V', 'c', 'C', 's', 'S', 'q', 'Q' };
        private readonly string val;
        private readonly StringSplitter splitter = new StringSplitter(string.Empty);
        private int curPos = -1;

        public CommandSplitter(string value)
        {
            this.val = value;
        }

        public string ReadNext()
        {
            int startPos = this.curPos;
            if (startPos < 0)
            {
                startPos = 0;
            }

            if (startPos >= this.val.Length)
            {
                return string.Empty;
            }

            int cmdStart = this.val.IndexOfAny(this.commands, startPos);
            int cmdEnd = cmdStart;
            if (cmdStart >= 0)
            {
                cmdEnd = this.val.IndexOfAny(this.commands, cmdStart + 1);
            }

            if (cmdEnd < 0)
            {
                int len = this.val.Length - startPos;
                this.curPos = this.val.Length;
                return this.val.Substring(startPos, len).Trim();
            }
            else
            {
                int len = cmdEnd - startPos;
                this.curPos = cmdEnd;
                return this.val.Substring(startPos, len).Trim();
            }
        }

        public StringSplitter SplitCommand(string command, out char cmd)
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            cmd = command[0];
            this.splitter.SetString(command, 1);
            return this.splitter;
        }
    }

    internal class PathElement
    {
        protected PathElement(char command)
        {
            this.Command = command;
        }

        public char Command { get; }

        public bool IsRelative => char.IsLower(this.Command);
    }

    internal class MoveTo : PathElement
    {
        public MoveTo(char command, StringSplitter value)
            : base(command)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            this.Point = value.ReadNextPoint();
        }

        public Point Point { get; }
    }

    internal class LineTo : PathElement
    {
        public LineTo(char command, StringSplitter value)
            : base(command)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (char.ToLower(command, GlobalizationConstants.EnglishCultureInfo) == 'h')
            {
                this.PositionType = PathShapeLineToType.Horizontal;
                double v = value.ReadNextValue();
                this.Points = new[]
                {
                    new Point(v, 0),
                };

                return;
            }

            if (char.ToLower(command, GlobalizationConstants.EnglishCultureInfo) == 'v')
            {
                this.PositionType = PathShapeLineToType.Vertical;
                double v = value.ReadNextValue();
                this.Points = new[]
                {
                    new Point(0, v),
                };

                return;
            }

            this.PositionType = PathShapeLineToType.Point;
            List<Point> list = new List<Point>();
            while (value.More)
            {
                Point p = value.ReadNextPoint();
                list.Add(p);
            }

            this.Points = list.ToArray();
        }

        public PathShapeLineToType PositionType { get; }

        public Point[] Points { get; }
    }

    internal class CurveTo : PathElement
    {
        public CurveTo(char command, StringSplitter value)
            : base(command)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            this.CtrlPoint1 = value.ReadNextPoint();
            this.CtrlPoint2 = value.ReadNextPoint();
            this.Point = value.ReadNextPoint();
        }

        public CurveTo(char command, StringSplitter value, Point ctrlPoint)
            : base(command)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            this.CtrlPoint1 = ctrlPoint;
            this.CtrlPoint2 = value.ReadNextPoint();
            this.Point = value.ReadNextPoint();
        }

        public Point CtrlPoint1 { get; }

        public Point CtrlPoint2 { get; }

        public Point Point { get; }
    }

    internal class QuadraticCurveTo : PathElement
    {
        public QuadraticCurveTo(char command, StringSplitter value)
            : base(command)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            this.CtrlPoint = value.ReadNextPoint();
            this.Point = value.ReadNextPoint();
        }

        public QuadraticCurveTo(char command, StringSplitter value, Point ctrlPoint)
            : base(command)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            this.CtrlPoint = ctrlPoint;
            this.Point = value.ReadNextPoint();
        }

        public Point CtrlPoint { get; }

        public Point Point { get; }
    }

    internal class EllipticalArcTo : PathElement
    {
        public EllipticalArcTo(char command, StringSplitter value)
            : base(command)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            this.Rx = value.ReadNextValue();
            this.Ry = value.ReadNextValue();
            this.AxisRotation = value.ReadNextValue();
            double arcFlag = value.ReadNextValue();
            this.LargeArc = arcFlag > 0;
            double sweepFlag = value.ReadNextValue();
            this.Clockwise = sweepFlag > 0;
            this.X = value.ReadNextValue();
            this.Y = value.ReadNextValue();
        }

        public double Rx { get; }

        public double Ry { get; }

        public double AxisRotation { get; }

        public double X { get; }

        public double Y { get; }

        public bool Clockwise { get; }

        public bool LargeArc { get; }
    }

    private readonly List<PathElement> elements = new List<PathElement>();

    private static Fill? defaultFill;

    public override Fill? Fill => base.Fill ?? defaultFill;

    public IList<PathElement> Elements => this.elements.AsReadOnly();

    public bool ClosePath { get; }

    public string? Data { get; }

    [SuppressMessage("Major Code Smell", "S3010:Static fields should not be updated in constructors", Justification = "OK.")]
    internal PathShape(Svg svg, XmlNode node, Shape parent)
        : base(svg, node, parent)
    {
        if (svg is null)
        {
            throw new ArgumentNullException(nameof(svg));
        }

        defaultFill ??= new Fill
        {
            PaintServerKey = svg.PaintServers.Parse("black"),
        };

        this.ClosePath = false;
        var path = SvgXmlUtil.AttrValue(node, "d", string.Empty);
        this.Data = path;
    }
}