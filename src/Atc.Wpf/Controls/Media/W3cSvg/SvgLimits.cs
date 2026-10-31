namespace Atc.Wpf.Controls.Media.W3cSvg;

/// <summary>
/// Limits that keep a hostile or broken SVG from hanging the application, exhausting its memory
/// or overflowing the stack. When a limit is hit, the SVG engine stops and keeps what it has so far,
/// or fails the load with an exception that the caller reports.
/// </summary>
internal static class SvgLimits
{
    /// <summary>
    /// The deepest element nesting that is read. Deeper elements are dropped, and so are
    /// <c>use</c> references that would render deeper than this.
    /// </summary>
    public const int MaxNestingDepth = 256;

    /// <summary>
    /// The most elements rendered for one drawing, counting every element that a <c>use</c> expands to.
    /// Rendering stops at this budget.
    /// </summary>
    public const int MaxDrawings = 100_000;

    /// <summary>
    /// The most characters read from one SVG document.
    /// </summary>
    public const long MaxCharactersInDocument = 64L * 1024 * 1024;

    /// <summary>
    /// The most bytes a compressed SVG (svgz) may decompress to.
    /// </summary>
    public const long MaxDecompressedBytes = 64L * 1024 * 1024;

    /// <summary>
    /// Creates the settings used to read SVG documents: no DTD processing (entities are not expanded),
    /// no external resolution and a cap on the document size.
    /// </summary>
    public static XmlReaderSettings CreateReaderSettings()
        => new()
        {
            DtdProcessing = DtdProcessing.Ignore,
            XmlResolver = null,
            MaxCharactersInDocument = MaxCharactersInDocument,
            MaxCharactersFromEntities = 1024,
        };

    /// <summary>
    /// Loads an XML document with the safe <see cref="CreateReaderSettings"/>.
    /// </summary>
    public static XmlDocument LoadDocument(
        Func<XmlReaderSettings, XmlReader> createReader)
    {
        ArgumentNullException.ThrowIfNull(createReader);

        var doc = new XmlDocument { XmlResolver = null };
        using (var reader = createReader(CreateReaderSettings()))
        {
            doc.Load(reader);
        }

        TrimDeepNodes(doc, MaxNestingDepth);
        return doc;
    }

    /// <summary>
    /// Opens a GZip-compressed stream for reading, failing with <see cref="InvalidDataException"/>
    /// when it decompresses to more than <see cref="MaxDecompressedBytes"/>.
    /// </summary>
    public static Stream Decompress(Stream compressed)
        => new SizeLimitedReadStream(
            new GZipStream(
                compressed,
                CompressionMode.Decompress),
            MaxDecompressedBytes);

    /// <summary>
    /// Removes the children of every node at <paramref name="maxDepth"/>, without recursion,
    /// so later recursive walks of the document stay shallow.
    /// </summary>
    internal static void TrimDeepNodes(
        XmlNode root,
        int maxDepth)
    {
        ArgumentNullException.ThrowIfNull(root);

        var current = root;
        var depth = 0;
        while (true)
        {
            if (depth >= maxDepth)
            {
                while (current.LastChild is not null)
                {
                    current.RemoveChild(current.LastChild);
                }
            }

            if (current.FirstChild is not null)
            {
                current = current.FirstChild;
                depth++;
                continue;
            }

            while (!ReferenceEquals(current, root) && current.NextSibling is null)
            {
                current = current.ParentNode!;
                depth--;
            }

            if (ReferenceEquals(current, root))
            {
                return;
            }

            current = current.NextSibling!;
        }
    }

    private sealed class SizeLimitedReadStream : Stream
    {
        private readonly Stream inner;
        private readonly long maxBytes;
        private long totalRead;

        public SizeLimitedReadStream(
            Stream inner,
            long maxBytes)
        {
            this.inner = inner;
            this.maxBytes = maxBytes;
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => totalRead;
            set => throw new NotSupportedException();
        }

        public override int Read(
            byte[] buffer,
            int offset,
            int count)
            => Read(buffer.AsSpan(offset, count));

        public override int Read(Span<byte> buffer)
        {
            var read = inner.Read(buffer);
            totalRead += read;
            if (totalRead > maxBytes)
            {
                throw new InvalidDataException($"The compressed SVG expands to more than {maxBytes} bytes.");
            }

            return read;
        }

        public override void Flush()
        {
        }

        public override long Seek(
            long offset,
            SeekOrigin origin)
            => throw new NotSupportedException();

        public override void SetLength(long value)
            => throw new NotSupportedException();

        public override void Write(
            byte[] buffer,
            int offset,
            int count)
            => throw new NotSupportedException();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                inner.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}