namespace Atc.Wpf.Sample.SamplesWpfDisplays.Displays;

/// <summary>
/// Creates the card (control, buffer and emulator target) that matches a Describe. The receiver never does this itself:
/// a Describe is advisory, and the application decides what to show.
/// </summary>
public static class DisplayMirrorCardFactory
{
    /// <summary>
    /// Creates the card for a display id from its description; a kind, chip or size the sample has no control for gives a placeholder card.
    /// Call it on the UI thread.
    /// </summary>
    /// <param name="displayId">The display id.</param>
    /// <param name="describe">The description.</param>
    /// <returns>The card.</returns>
    [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "The card is returned; the caller owns and disposes it.")]
    public static DisplayMirrorCard Create(
        byte displayId,
        DisplayMirrorDescribe describe)
    {
        ArgumentNullException.ThrowIfNull(describe);

        return describe.Kind switch
        {
            DisplayMirrorDisplayKind.CharacterLcd => CreateCharacterCard(displayId, describe),
            DisplayMirrorDisplayKind.MonoOled => CreateOledCard(displayId, describe),
            DisplayMirrorDisplayKind.ColorPanel => CreateColorCard(displayId, describe),
            DisplayMirrorDisplayKind.EPaper => CreateEPaperCard(displayId, describe),
            DisplayMirrorDisplayKind.LedMatrix => CreateLedMatrixCard(displayId, describe),
            DisplayMirrorDisplayKind.SevenSegment => CreateSevenSegmentCard(displayId, describe),
            DisplayMirrorDisplayKind.GraphicLcd => CreateGraphicLcdCard(displayId, describe),
            _ => new DisplayMirrorPlaceholderCard(displayId, describe, $"The sample has no control for the kind {describe.Kind}."),
        };
    }

    /// <summary>
    /// Maps an OLED emulator chip to its protocol chip value.
    /// </summary>
    /// <param name="chip">The emulator chip.</param>
    /// <returns>The protocol chip.</returns>
    public static DisplayMirrorChip ToMirrorChip(OledChip chip)
        => chip switch
        {
            OledChip.Ssd1309 => DisplayMirrorChip.Ssd1309,
            OledChip.Sh1106 => DisplayMirrorChip.Sh1106,
            _ => DisplayMirrorChip.Ssd1306,
        };

    /// <summary>
    /// Gets the protocol chip of a colour module preset.
    /// </summary>
    /// <param name="preset">The preset.</param>
    /// <returns>The protocol chip.</returns>
    public static DisplayMirrorChip ChipOf(ColorGraphicDisplayPreset preset)
    {
        ArgumentNullException.ThrowIfNull(preset);

        return preset.Name switch
        {
            ColorGraphicDisplayPreset.St7735Mini or
            ColorGraphicDisplayPreset.St7735Green144 or
            ColorGraphicDisplayPreset.St7735Green => DisplayMirrorChip.St7735,
            ColorGraphicDisplayPreset.St7789Small or
            ColorGraphicDisplayPreset.St7789Square or
            ColorGraphicDisplayPreset.St7789Large => DisplayMirrorChip.St7789,
            ColorGraphicDisplayPreset.Ili9341 => DisplayMirrorChip.Ili9341,
            ColorGraphicDisplayPreset.Ssd1331 => DisplayMirrorChip.Ssd1331,
            _ => DisplayMirrorChip.Ssd1351,
        };
    }

    private static DisplayMirrorCard CreateCharacterCard(
        byte displayId,
        DisplayMirrorDescribe describe)
    {
        if (describe.Chip is not (DisplayMirrorChip.Hd44780 or DisplayMirrorChip.Unknown))
        {
            return Unsupported(displayId, describe, "chip");
        }

        if (describe.Width is < 1 or > 40 ||
            describe.Height is < 1 or > 4)
        {
            return Unsupported(displayId, describe, "size (1 to 40 columns, 1 to 4 rows)");
        }

        return new DisplayMirrorCharacterCard(displayId, describe);
    }

    private static DisplayMirrorCard CreateOledCard(
        byte displayId,
        DisplayMirrorDescribe describe)
    {
        OledChip? chip = describe.Chip switch
        {
            DisplayMirrorChip.Ssd1306 => OledChip.Ssd1306,
            DisplayMirrorChip.Ssd1309 => OledChip.Ssd1309,
            DisplayMirrorChip.Sh1106 => OledChip.Sh1106,
            _ => null,
        };

        if (chip is null &&
            describe.Chip != DisplayMirrorChip.Unknown)
        {
            return Unsupported(displayId, describe, "chip");
        }

        if (describe.Width is < 1 or > MonochromeFrameBuffer.MaxWidth ||
            describe.Height is < 1 or > MonochromeFrameBuffer.MaxHeight)
        {
            return Unsupported(displayId, describe, "size (up to 132 x 64)");
        }

        // The colour of the module the Describe names (a preset of the OledDisplay sample), else of a module with the same chip and size.
        var preset = OledDisplayPreset.All.FirstOrDefault(p => string.Equals(p.Name, describe.Name, StringComparison.Ordinal))
                     ?? OledDisplayPreset.All.FirstOrDefault(p => chip == p.Chip && p.Columns == describe.Width && p.Rows == describe.Height);

        // An unknown chip takes whole frames only.
        return new DisplayMirrorOledCard(displayId, describe, chip, preset?.Color ?? OledDisplayColor.White);
    }

    private static DisplayMirrorCard CreateColorCard(
        byte displayId,
        DisplayMirrorDescribe describe)
    {
        if (describe.Width is < 1 or > ColorFrameBuffer.MaxSize ||
            describe.Height is < 1 or > ColorFrameBuffer.MaxSize)
        {
            return Unsupported(displayId, describe, "size (up to 480 x 480)");
        }

        // The module of the ColorGraphicDisplay sample with this chip and size gives the panel window, RAM size and colour order.
        var preset = ColorGraphicDisplayPreset.All.FirstOrDefault(p =>
            ChipOf(p) == describe.Chip &&
            p.Columns == describe.Width &&
            p.Rows == describe.Height);

        Func<ColorFrameBuffer, IGraphicDisplayController>? createController = describe.Chip switch
        {
            _ when preset is not null => preset.CreateController,
            DisplayMirrorChip.St7735 => buffer => new TftController(buffer, TftChip.St7735),
            DisplayMirrorChip.St7789 => buffer => new TftController(buffer, TftChip.St7789),
            DisplayMirrorChip.Ili9341 => buffer => new TftController(buffer, TftChip.Ili9341),
            DisplayMirrorChip.Ssd1331 => buffer => new Ssd1331Controller(buffer),
            DisplayMirrorChip.Ssd1351 => buffer => new Ssd1351Controller(buffer),
            _ => null,
        };

        if (createController is null &&
            describe.Chip != DisplayMirrorChip.Unknown)
        {
            return Unsupported(displayId, describe, "chip");
        }

        var panelKind = describe.Chip is DisplayMirrorChip.Ssd1331 or DisplayMirrorChip.Ssd1351
            ? ColorPanelKind.Oled
            : ColorPanelKind.Tft;

        // An unknown chip takes whole frames only.
        return new DisplayMirrorColorCard(displayId, describe, panelKind, createController);
    }

    private static DisplayMirrorCard CreateEPaperCard(
        byte displayId,
        DisplayMirrorDescribe describe)
    {
        if (describe.Chip is not (DisplayMirrorChip.Ssd1680 or DisplayMirrorChip.Unknown))
        {
            return Unsupported(displayId, describe, "chip");
        }

        if (describe.Width is < 1 or > EPaperFrameBuffer.MaxWidth ||
            describe.Height is < 1 or > EPaperFrameBuffer.MaxHeight)
        {
            return Unsupported(displayId, describe, "size (up to 400 x 400, native portrait)");
        }

        // A Describe does not say whether the panel has red: the preset the name matches does, else black/white. A black/white panel
        // ignores the red RAM, which may hold the previous picture (§SSD1680.4 item 3), so assuming red would tint it.
        // A 2.13" panel is described (and its frames sent) 128 pixels wide, as GxEPD2 and Adafruit_EPD keep it; the glass shows 122.
        var preset = EPaperDisplayPreset.All.FirstOrDefault(p => string.Equals(p.Name, describe.Name, StringComparison.Ordinal));
        var isCropped = preset is not null &&
                        preset.Columns == describe.Width &&
                        preset.VisibleColumns != preset.Columns;
        int? visibleColumns = isCropped ? preset!.VisibleColumns : null;
        return new DisplayMirrorEPaperCard(
            displayId,
            describe,
            preset?.HasRed ?? false,
            visibleColumns,
            isSsd1680: describe.Chip == DisplayMirrorChip.Ssd1680);
    }

    // A matrix is described in pixels: width = 8 per module (1 to 8 modules), height 8.
    private static DisplayMirrorCard CreateLedMatrixCard(
        byte displayId,
        DisplayMirrorDescribe describe)
    {
        if (describe.Chip is not (DisplayMirrorChip.Max7219 or DisplayMirrorChip.Unknown))
        {
            return Unsupported(displayId, describe, "chip");
        }

        if (describe.Width % 8 != 0 ||
            describe.Width / 8 is < 1 or > Max7219ChainBuffer.MaxDeviceCount ||
            describe.Height != 8)
        {
            return Unsupported(displayId, describe, "size (8 to 64 x 8, 8 pixels per module)");
        }

        var preset = LedMatrixDisplayPreset.All.FirstOrDefault(p => string.Equals(p.Name, describe.Name, StringComparison.Ordinal));
        return new DisplayMirrorMax7219Card(
            displayId,
            describe,
            isMatrix: true,
            describe.Width / 8,
            digitCount: 8,
            preset?.Wiring ?? Max7219MatrixWiring.Fc16,
            preset?.LedColor ?? LedColor.Red);
    }

    // A seven-segment module is described in digits: width = the digit count (1 to 8), height 1.
    private static DisplayMirrorCard CreateSevenSegmentCard(
        byte displayId,
        DisplayMirrorDescribe describe)
    {
        if (describe.Chip is not (DisplayMirrorChip.Max7219 or DisplayMirrorChip.Unknown))
        {
            return Unsupported(displayId, describe, "chip");
        }

        if (describe.Width is < 1 or > Max7219ChainBuffer.DigitCount ||
            describe.Height != 1)
        {
            return Unsupported(displayId, describe, "size (1 to 8 digits x 1)");
        }

        return new DisplayMirrorMax7219Card(
            displayId,
            describe,
            isMatrix: false,
            deviceCount: 1,
            describe.Width,
            Max7219MatrixWiring.Fc16,
            LedColor.Red);
    }

    private static DisplayMirrorCard CreateGraphicLcdCard(
        byte displayId,
        DisplayMirrorDescribe describe)
    {
        if (describe.Chip is not (DisplayMirrorChip.St7920 or DisplayMirrorChip.Unknown))
        {
            return Unsupported(displayId, describe, "chip");
        }

        if (describe.Width != GraphicLcdDisplay.PixelColumns ||
            describe.Height != GraphicLcdDisplay.PixelRows)
        {
            return Unsupported(displayId, describe, "size (128 x 64)");
        }

        var preset = GraphicLcdDisplayPreset.All.FirstOrDefault(p => string.Equals(p.Name, describe.Name, StringComparison.Ordinal));
        return new DisplayMirrorGraphicLcdCard(
            displayId,
            describe,
            preset?.Color ?? CharacterDisplayColor.Blue,
            isSt7920: describe.Chip == DisplayMirrorChip.St7920);
    }

    private static DisplayMirrorPlaceholderCard Unsupported(
        byte displayId,
        DisplayMirrorDescribe describe,
        string what)
        => new(displayId, describe, $"The sample cannot show this {what}.");
}