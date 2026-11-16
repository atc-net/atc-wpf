namespace Atc.Wpf.Displays;

/// <summary>
/// Creates the built-in <see cref="IDisplayMirrorTarget"/> adapters for the frame buffers, chip emulators and bus decoders of this namespace.
/// </summary>
/// <remarks>
/// Every adapter returns <see cref="DisplayMirrorRejectReason.Unsupported"/> for the message types it does not take (Describe included).
/// The adapters add no locking; a <see cref="DisplayMirrorReceiver"/> applies one packet at a time.
/// </remarks>
public static class DisplayMirrorTargets
{
    private const int DimensionsLength = 4;
    private const int PageHeight = 8;

    /// <summary>
    /// Creates a target that loads MonoFrame packets into a monochrome frame buffer.
    /// </summary>
    /// <param name="buffer">The buffer.</param>
    /// <returns>The target. A frame of another size than the buffer is rejected with <see cref="DisplayMirrorRejectReason.SizeMismatch"/>.</returns>
    public static IDisplayMirrorTarget For(MonochromeFrameBuffer buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        return new MonochromeFrameTarget(buffer);
    }

    /// <summary>
    /// Creates a target that loads ColorFrame packets (big-endian RGB565) into a colour frame buffer.
    /// </summary>
    /// <param name="buffer">The buffer.</param>
    /// <returns>The target. A frame of another size than the buffer is rejected with <see cref="DisplayMirrorRejectReason.SizeMismatch"/>.</returns>
    public static IDisplayMirrorTarget For(ColorFrameBuffer buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        return new ColorFrameTarget(buffer);
    }

    /// <summary>
    /// Creates a target that loads EPaperFrame packets into an e-paper frame buffer: the planes go into the RAM
    /// (<see cref="EPaperFrameBuffer.LoadRam"/>), then the buffer is refreshed with the kind of the packet.
    /// </summary>
    /// <param name="buffer">The buffer.</param>
    /// <returns>
    /// The target. A frame of another size than the buffer is rejected with <see cref="DisplayMirrorRejectReason.SizeMismatch"/>;
    /// a frame with one plane leaves the red RAM as it is.
    /// </returns>
    public static IDisplayMirrorTarget For(EPaperFrameBuffer buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        return new EPaperFrameTarget(buffer);
    }

    /// <summary>
    /// Creates a target that feeds a graphic display chip emulator: Commands to <see cref="IGraphicDisplayController.WriteCommands"/>,
    /// Data and Latch to <see cref="IGraphicDisplayController.WriteData"/> and Reset to <see cref="IGraphicDisplayController.Reset"/>.
    /// </summary>
    /// <param name="controller">The emulator.</param>
    /// <returns>The target.</returns>
    public static IDisplayMirrorTarget For(IGraphicDisplayController controller)
    {
        ArgumentNullException.ThrowIfNull(controller);

        return new GraphicControllerTarget(controller);
    }

    /// <summary>
    /// Creates a target that passes I2cWrite packets (without their address byte) to an SSD1306 I2C decoder.
    /// </summary>
    /// <param name="decoder">The decoder.</param>
    /// <param name="address">The 7-bit I2C address to take; <see langword="null"/> takes every address. Writes to other addresses are <see cref="DisplayMirrorRejectReason.Unsupported"/>.</param>
    /// <returns>The target.</returns>
    public static IDisplayMirrorTarget For(
        Ssd1306I2cDecoder decoder,
        byte? address = null)
    {
        ArgumentNullException.ThrowIfNull(decoder);

        return new I2cTarget(decoder.Write, address);
    }

    /// <summary>
    /// Creates a target that passes I2cWrite packets (without their address byte) to a PCF8574 HD44780 backpack decoder.
    /// </summary>
    /// <param name="decoder">The decoder.</param>
    /// <param name="address">The 7-bit I2C address to take; <see langword="null"/> takes every address. Writes to other addresses are <see cref="DisplayMirrorRejectReason.Unsupported"/>.</param>
    /// <returns>The target.</returns>
    public static IDisplayMirrorTarget For(
        Pcf8574Hd44780Decoder decoder,
        byte? address = null)
    {
        ArgumentNullException.ThrowIfNull(decoder);

        return new I2cTarget(decoder.Write, address);
    }

    /// <summary>
    /// Creates a target that passes the serial (SPI) byte stream of an ST7920 module to its decoder: Data packets carry the bytes as
    /// they went over the wire (start bytes <c>0xF8</c> / <c>0xFA</c> and nibble pairs, as U8g2 and ESPHome send them) and go to
    /// <see cref="St7920SerialDecoder.Write"/>; a Reset packet resets the chip (<see cref="St7920Controller.Reset"/>) and the decoder's
    /// framing (<see cref="St7920SerialDecoder.ResetFraming"/>).
    /// </summary>
    /// <param name="decoder">The decoder.</param>
    /// <returns>
    /// The target. Commands and Latch packets are <see cref="DisplayMirrorRejectReason.Unsupported"/>: logical ST7920 instructions go to
    /// <see cref="For(IGraphicDisplayController)"/> of the controller.
    /// </returns>
    public static IDisplayMirrorTarget For(St7920SerialDecoder decoder)
    {
        ArgumentNullException.ThrowIfNull(decoder);

        return new St7920SerialTarget(decoder);
    }

    /// <summary>
    /// Creates a target that feeds an HD44780 emulator: the bytes of a Commands packet are instructions (RS low),
    /// the bytes of a Data packet are data (RS high).
    /// </summary>
    /// <param name="controller">The emulator.</param>
    /// <returns>The target.</returns>
    public static IDisplayMirrorTarget For(Hd44780Controller controller)
    {
        ArgumentNullException.ThrowIfNull(controller);

        return new Hd44780Target(controller);
    }

    /// <summary>
    /// Combines targets: a packet goes to each target in order until one does not return <see cref="DisplayMirrorRejectReason.Unsupported"/>.
    /// </summary>
    /// <param name="targets">The targets, in order of priority.</param>
    /// <returns>The combined target; it returns <see cref="DisplayMirrorRejectReason.Unsupported"/> when no target takes the packet.</returns>
    /// <exception cref="ArgumentException">A target is <see langword="null"/>.</exception>
    public static IDisplayMirrorTarget Combine(
        params IDisplayMirrorTarget[] targets)
    {
        ArgumentNullException.ThrowIfNull(targets);

        var copy = (IDisplayMirrorTarget[])targets.Clone();
        if (Array.IndexOf(copy, null) >= 0)
        {
            throw new ArgumentException("A target is null.", nameof(targets));
        }

        return new CombinedTarget(copy);
    }

    private static bool TryReadDimensions(
        ReadOnlySpan<byte> payload,
        out int width,
        out int height)
    {
        if (payload.Length < DimensionsLength)
        {
            width = 0;
            height = 0;
            return false;
        }

        width = BinaryPrimitives.ReadUInt16LittleEndian(payload);
        height = BinaryPrimitives.ReadUInt16LittleEndian(payload[2..]);
        return true;
    }

    private sealed class MonochromeFrameTarget(MonochromeFrameBuffer buffer) : IDisplayMirrorTarget
    {
        public DisplayMirrorRejectReason Apply(DisplayMirrorPacket packet)
        {
            if (packet.Type != DisplayMirrorMessageType.MonoFrame)
            {
                return DisplayMirrorRejectReason.Unsupported;
            }

            var payload = packet.Payload.Span;
            if (!TryReadDimensions(payload, out var width, out var height) ||
                payload.Length - DimensionsLength != (long)width * ((height + PageHeight - 1) / PageHeight))
            {
                return DisplayMirrorRejectReason.Malformed;
            }

            if (width != buffer.Width ||
                height != buffer.Height)
            {
                return DisplayMirrorRejectReason.SizeMismatch;
            }

            buffer.LoadFrame(payload[DimensionsLength..]);
            return DisplayMirrorRejectReason.None;
        }
    }

    private sealed class ColorFrameTarget(ColorFrameBuffer buffer) : IDisplayMirrorTarget
    {
        public DisplayMirrorRejectReason Apply(DisplayMirrorPacket packet)
        {
            if (packet.Type != DisplayMirrorMessageType.ColorFrame)
            {
                return DisplayMirrorRejectReason.Unsupported;
            }

            var payload = packet.Payload.Span;
            if (!TryReadDimensions(payload, out var width, out var height) ||
                payload.Length - DimensionsLength != (long)width * height * 2)
            {
                return DisplayMirrorRejectReason.Malformed;
            }

            if (width != buffer.Width ||
                height != buffer.Height)
            {
                return DisplayMirrorRejectReason.SizeMismatch;
            }

            buffer.LoadFrameRgb565(payload[DimensionsLength..], Rgb565ByteOrder.BigEndian);
            return DisplayMirrorRejectReason.None;
        }
    }

    private sealed class EPaperFrameTarget(EPaperFrameBuffer buffer) : IDisplayMirrorTarget
    {
        private const int PlanesOffset = DimensionsLength;
        private const int PlanesStart = DimensionsLength + 1;

        public DisplayMirrorRejectReason Apply(DisplayMirrorPacket packet)
        {
            if (packet.Type != DisplayMirrorMessageType.EPaperFrame)
            {
                return DisplayMirrorRejectReason.Unsupported;
            }

            // Protocol EPaperFrame: u16 width, u16 height, plane count (1 or 2), the planes (ceil(width / 8) x height bytes each),
            // the refresh kind (0 full, 1 partial).
            var payload = packet.Payload.Span;
            if (!TryReadDimensions(payload, out var width, out var height) ||
                payload.Length <= PlanesStart)
            {
                return DisplayMirrorRejectReason.Malformed;
            }

            var planes = payload[PlanesOffset];
            var planeLength = (long)((width + 7) / 8) * height;
            if (planes is not (1 or 2) ||
                payload.Length != PlanesStart + (planes * planeLength) + 1 ||
                payload[^1] > 1)
            {
                return DisplayMirrorRejectReason.Malformed;
            }

            if (width != buffer.Width ||
                height != buffer.Height)
            {
                return DisplayMirrorRejectReason.SizeMismatch;
            }

            var length = (int)planeLength;
            var black = payload.Slice(PlanesStart, length);
            var red = planes == 2
                ? payload.Slice(PlanesStart + length, length)
                : [];
            var kind = payload[^1] == 0
                ? EPaperRefreshKind.Full
                : EPaperRefreshKind.Partial;

            using (buffer.BeginBatch())
            {
                buffer.LoadRam(black, red);
                buffer.Refresh(kind);
            }

            return DisplayMirrorRejectReason.None;
        }
    }

    private sealed class GraphicControllerTarget(IGraphicDisplayController controller) : IDisplayMirrorTarget
    {
        public DisplayMirrorRejectReason Apply(DisplayMirrorPacket packet)
        {
            switch (packet.Type)
            {
                case DisplayMirrorMessageType.Commands:
                    controller.WriteCommands(packet.Payload.Span);
                    return DisplayMirrorRejectReason.None;
                case DisplayMirrorMessageType.Data:
                case DisplayMirrorMessageType.Latch:
                    controller.WriteData(packet.Payload.Span);
                    return DisplayMirrorRejectReason.None;
                case DisplayMirrorMessageType.Reset:
                    controller.Reset();
                    return DisplayMirrorRejectReason.None;
                default:
                    return DisplayMirrorRejectReason.Unsupported;
            }
        }
    }

    private sealed class I2cTarget(
        I2cWriter write,
        byte? address) : IDisplayMirrorTarget
    {
        public DisplayMirrorRejectReason Apply(DisplayMirrorPacket packet)
        {
            if (packet.Type != DisplayMirrorMessageType.I2cWrite)
            {
                return DisplayMirrorRejectReason.Unsupported;
            }

            var payload = packet.Payload.Span;
            if (payload.IsEmpty)
            {
                return DisplayMirrorRejectReason.Malformed;
            }

            if (address is { } filter &&
                payload[0] != filter)
            {
                return DisplayMirrorRejectReason.Unsupported;
            }

            write(payload[1..]);
            return DisplayMirrorRejectReason.None;
        }
    }

    private delegate void I2cWriter(ReadOnlySpan<byte> transfer);

    private sealed class Hd44780Target(Hd44780Controller controller) : IDisplayMirrorTarget
    {
        public DisplayMirrorRejectReason Apply(DisplayMirrorPacket packet)
        {
            switch (packet.Type)
            {
                case DisplayMirrorMessageType.Commands:
                    controller.Write(packet.Payload.Span, isData: false);
                    return DisplayMirrorRejectReason.None;
                case DisplayMirrorMessageType.Data:
                    controller.Write(packet.Payload.Span, isData: true);
                    return DisplayMirrorRejectReason.None;
                default:
                    return DisplayMirrorRejectReason.Unsupported;
            }
        }
    }

    private sealed class St7920SerialTarget(St7920SerialDecoder decoder) : IDisplayMirrorTarget
    {
        public DisplayMirrorRejectReason Apply(DisplayMirrorPacket packet)
        {
            switch (packet.Type)
            {
                case DisplayMirrorMessageType.Data:
                    decoder.Write(packet.Payload.Span);
                    return DisplayMirrorRejectReason.None;
                case DisplayMirrorMessageType.Reset:
                    // A pending half pair of the old stream must not combine with the first byte after the reset.
                    decoder.Controller.Reset();
                    decoder.ResetFraming();
                    return DisplayMirrorRejectReason.None;
                default:
                    return DisplayMirrorRejectReason.Unsupported;
            }
        }
    }

    private sealed class CombinedTarget(IDisplayMirrorTarget[] targets) : IDisplayMirrorTarget
    {
        public DisplayMirrorRejectReason Apply(DisplayMirrorPacket packet)
        {
            foreach (var target in targets)
            {
                var result = target.Apply(packet);
                if (result != DisplayMirrorRejectReason.Unsupported)
                {
                    return result;
                }
            }

            return DisplayMirrorRejectReason.Unsupported;
        }
    }
}