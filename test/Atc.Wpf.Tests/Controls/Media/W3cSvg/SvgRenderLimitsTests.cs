namespace Atc.Wpf.Tests.Controls.Media.W3cSvg;

public sealed class SvgRenderLimitsTests
{
    private const string SvgOpen = """<svg viewBox="0 0 10 10" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink">""";

    // A runaway render takes minutes; CI runs with code coverage on slower machines, so it gets more room.
    private static readonly TimeSpan MaxDuration = TimeSpan.FromSeconds(
        string.Equals(Environment.GetEnvironmentVariable("CI"), "true", StringComparison.OrdinalIgnoreCase) ? 20 : 5);

    [StaFact]
    public void UseFanOut_TenLevelsOfTen_StopsAtTheDrawingBudgetQuickly()
    {
        var sb = new StringBuilder(SvgOpen);
        sb.Append("<defs>");
        sb.Append("""<rect id="l0" width="1" height="1" />""");
        for (var level = 1; level <= 10; level++)
        {
            sb.Append(CultureInfo.InvariantCulture, $"""<g id="l{level}">""");
            for (var i = 0; i < 10; i++)
            {
                sb.Append(CultureInfo.InvariantCulture, $"""<use href="#l{level - 1}" />""");
            }

            sb.Append("</g>");
        }

        sb.Append("</defs>");
        sb.Append("""<use href="#l10" />""");
        sb.Append("</svg>");

        var (drawing, elapsed) = RenderTimed(sb.ToString());

        Assert.True(elapsed < MaxDuration, $"Rendering took {elapsed}.");
        Assert.InRange(CountDrawings(drawing), 1, SvgLimits.MaxDrawings);
    }

    [StaFact]
    public void DeeplyNestedGroups_DoNotOverflowTheStack()
    {
        const int depth = 20_000;
        var sb = new StringBuilder(SvgOpen);
        for (var i = 0; i < depth; i++)
        {
            sb.Append("<g>");
        }

        sb.Append("""<rect width="1" height="1" />""");
        for (var i = 0; i < depth; i++)
        {
            sb.Append("</g>");
        }

        sb.Append("""<rect width="2" height="2" />""");
        sb.Append("</svg>");

        var (drawing, elapsed) = RenderTimed(sb.ToString());

        Assert.True(elapsed < MaxDuration, $"Rendering took {elapsed}.");
        Assert.NotEmpty(GeometryDrawings(drawing));
    }

    [StaTheory]
    [InlineData("")]
    [InlineData("fill=\"red\"")]
    [InlineData("stroke=\"red\"")]
    public void ShapeUnderManyPlainGroups_InheritsItsPaintQuickly(
        string rectAttributes)
    {
        const int depth = 250;
        var sb = new StringBuilder(SvgOpen);
        for (var i = 0; i < depth; i++)
        {
            sb.Append("<g>");
        }

        sb.Append(CultureInfo.InvariantCulture, $"""<rect width="1" height="1" {rectAttributes} />""");
        for (var i = 0; i < depth; i++)
        {
            sb.Append("</g>");
        }

        sb.Append("</svg>");

        var (drawing, elapsed) = RenderTimed(sb.ToString());

        Assert.True(elapsed < MaxDuration, $"Rendering took {elapsed}.");
        Assert.Single(GeometryDrawings(drawing));
    }

    [StaTheory]
    [InlineData("""<rect id="l0" width="1" height="1" fill="red" />""", "")]
    [InlineData("""<linearGradient id="gr"><stop offset="0" stop-color="red" /><stop offset="1" stop-color="blue" /></linearGradient><rect id="l0" width="1" height="1" fill="url(#gr)" />""", "")]
    [InlineData("""<rect id="l0" width="1" height="1" />""", "fill=\"red\"")]
    [InlineData("""<rect id="l0" width="1" height="1" stroke="red" />""", "")]
    public void UseFanOut_WithSharedPaint_StopsAtTheDrawingBudgetQuickly(
        string leaf,
        string rootAttributes)
    {
        var sb = new StringBuilder();
        sb.Append(CultureInfo.InvariantCulture, $"""<svg viewBox="0 0 10 10" xmlns="http://www.w3.org/2000/svg" {rootAttributes}>""");
        sb.Append("<defs>");
        sb.Append(leaf);
        for (var level = 1; level <= 6; level++)
        {
            sb.Append(CultureInfo.InvariantCulture, $"""<g id="l{level}">""");
            for (var i = 0; i < 10; i++)
            {
                sb.Append(CultureInfo.InvariantCulture, $"""<use href="#l{level - 1}" />""");
            }

            sb.Append("</g>");
        }

        sb.Append("""</defs><use href="#l6" /></svg>""");

        var (drawing, elapsed) = RenderTimed(sb.ToString());

        Assert.True(elapsed < MaxDuration, $"Rendering took {elapsed}.");
        Assert.InRange(CountDrawings(drawing), 1, SvgLimits.MaxDrawings);
    }

    [StaFact]
    public void LongChainOfUseElements_DoesNotOverflowTheStack()
    {
        const int length = 20_000;
        var sb = new StringBuilder(SvgOpen);
        sb.Append("<defs>");
        for (var i = 0; i < length; i++)
        {
            sb.Append(CultureInfo.InvariantCulture, $"""<g id="a{i}"><use href="#a{i + 1}" /></g>""");
        }

        sb.Append(CultureInfo.InvariantCulture, $"""<rect id="a{length}" width="1" height="1" />""");
        sb.Append("</defs>");
        sb.Append("""<use href="#a0" /><rect width="2" height="2" />""");
        sb.Append("</svg>");

        var (drawing, elapsed) = RenderTimed(sb.ToString());

        Assert.True(elapsed < MaxDuration, $"Rendering took {elapsed}.");
        Assert.NotEmpty(GeometryDrawings(drawing));
    }

    [StaFact]
    public void DeeplyNestedTSpans_DoNotOverflowTheStack()
    {
        const int depth = 20_000;
        var sb = new StringBuilder(SvgOpen);
        sb.Append("""<text x="1" y="5">""");
        for (var i = 0; i < depth; i++)
        {
            sb.Append("<tspan>");
        }

        sb.Append('x');
        for (var i = 0; i < depth; i++)
        {
            sb.Append("</tspan>");
        }

        sb.Append("</text>");
        sb.Append("""<rect width="2" height="2" />""");
        sb.Append("</svg>");

        var (drawing, elapsed) = RenderTimed(sb.ToString());

        Assert.True(elapsed < MaxDuration, $"Rendering took {elapsed}.");
        Assert.NotEmpty(GeometryDrawings(drawing));
    }

    [StaFact]
    public void EntityExpansion_IsNotPerformed()
    {
        var sb = new StringBuilder();
        sb.Append("""<?xml version="1.0"?><!DOCTYPE svg [<!ENTITY lol0 "lollollollollollollollollollol">""");
        for (var i = 1; i < 10; i++)
        {
            sb.Append(CultureInfo.InvariantCulture, $"<!ENTITY lol{i} \"");
            for (var j = 0; j < 10; j++)
            {
                sb.Append(CultureInfo.InvariantCulture, $"&lol{i - 1};");
            }

            sb.Append("\">");
        }

        sb.Append("]>");
        sb.Append(SvgOpen);
        sb.Append("<text>&lol9;</text></svg>");

        var stopwatch = Stopwatch.StartNew();
        Assert.ThrowsAny<XmlException>(() => new SvgRender().LoadXmlDrawing(sb.ToString()));
        Assert.True(stopwatch.Elapsed < MaxDuration, $"Loading took {stopwatch.Elapsed}.");
    }

    [StaFact]
    public void DocumentType_WithoutEntities_StillLoads()
    {
        var drawing = new SvgRender().LoadXmlDrawing(
            $"""<?xml version="1.0"?><!DOCTYPE svg PUBLIC "-//W3C//DTD SVG 1.1//EN" "http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd">{SvgOpen}<rect width="2" height="2" /></svg>""");

        Assert.Single(GeometryDrawings(drawing));
    }

    [StaFact]
    public void CompressedStream_LargerThanTheLimit_IsRejected()
    {
        using var compressed = new MemoryStream();
        using (var zip = new GZipStream(compressed, CompressionLevel.Fastest, leaveOpen: true))
        {
            var block = new byte[1024 * 1024];
            for (long written = 0; written <= SvgLimits.MaxDecompressedBytes; written += block.Length)
            {
                zip.Write(block, 0, block.Length);
            }
        }

        compressed.Position = 0;
        using var limited = SvgLimits.Decompress(compressed);
        var buffer = new byte[81920];

        Assert.Throws<InvalidDataException>(() =>
        {
            var total = 0L;
            int read;
            while ((read = limited.Read(buffer, 0, buffer.Length)) > 0)
            {
                total += read;
            }

            return total;
        });
    }

    private static (DrawingGroup Drawing, TimeSpan Elapsed) RenderTimed(
        string svg)
    {
        var stopwatch = Stopwatch.StartNew();
        var drawing = new SvgRender().LoadXmlDrawing(svg);
        return (drawing, stopwatch.Elapsed);
    }

    private static int CountDrawings(DrawingGroup group)
    {
        var count = 0;
        var pending = new Stack<DrawingGroup>();
        pending.Push(group);
        while (pending.Count > 0)
        {
            foreach (var child in pending.Pop().Children)
            {
                count++;
                if (child is DrawingGroup childGroup)
                {
                    pending.Push(childGroup);
                }
            }
        }

        return count;
    }

    private static List<GeometryDrawing> GeometryDrawings(DrawingGroup group)
    {
        var result = new List<GeometryDrawing>();
        var pending = new Stack<DrawingGroup>();
        pending.Push(group);
        while (pending.Count > 0)
        {
            foreach (var child in pending.Pop().Children)
            {
                switch (child)
                {
                    case GeometryDrawing geometryDrawing:
                        result.Add(geometryDrawing);
                        break;
                    case DrawingGroup childGroup:
                        pending.Push(childGroup);
                        break;
                }
            }
        }

        return result;
    }
}