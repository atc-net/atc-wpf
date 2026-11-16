# Display Mirror Protocol (version 1)

The display mirroring protocol carries what a microcontroller (ESP32, Arduino) or a Raspberry Pi sends to a display
over a serial link or a socket, so a WPF display control can show the same picture. The C# codec is
`DisplayMirrorProtocol` in the `Atc.Wpf.Displays` package (namespace `Atc.Wpf.Displays`). The Python module ([firmware/python](../../firmware/python/README.md)) and
the ESP32 Arduino library ([firmware/arduino/AtcDisplayMirror](../../firmware/arduino/AtcDisplayMirror/README.md)) implement the same bytes.

The golden vectors in `firmware/protocol/test-vectors.json` are the single source of truth. Every implementation
must encode each vector to exactly its `packetHex`, decode it back, and reject every entry of `rejects` with the stated reason.

## Regenerating / verifying the vectors

`firmware/protocol/gen_vectors.py` (Python 3.9+, standard library only) holds the hand-written payloads and builds the packets and CRCs with `binascii.crc_hqx`.

```bash
# rewrite firmware/protocol/test-vectors.json
python firmware/protocol/gen_vectors.py

# verify the existing file (CRC, magic, lengths, and that each reject shows its defect); exits 1 on a mismatch
python firmware/protocol/gen_vectors.py --check
```

## Packet layout

All multi-byte fields are little-endian.

| Offset | Size | Field | Value |
|---|---|---|---|
| 0 | 1 | Magic 0 | `0xA7` |
| 1 | 1 | Magic 1 | `0xC0` |
| 2 | 1 | Version | `1` |
| 3 | 1 | Display id | `0..255` (selects the target; one link can carry several displays) |
| 4 | 1 | Type | see below |
| 5 | 1 | Flags | receivers ignore unknown bits; version 1 defines none |
| 6 | 4 | Payload length | u32 |
| 10 | n | Payload | per type |
| 10 + n | 2 | CRC | u16 |

A packet is `12 + n` bytes. The header is 10 bytes and the CRC 2 bytes.

## CRC

CRC-16/CCITT-FALSE: polynomial `0x1021`, initial value `0xFFFF`, no input or output reflection, no final xor.
It is computed over header bytes 2..9 (version, display id, type, flags, length) followed by the payload, and written little-endian.
The magic is not covered. The check value of the ASCII text `123456789` is `0x29B1`.

In Python this is `binascii.crc_hqx(data, 0xFFFF)`.

## Message types

| Value | Type | Payload |
|---|---|---|
| `0x01` | Describe | kind (u8), chip (u8), u16 width, u16 height, UTF-8 name (the rest of the payload) |
| `0x10` | MonoFrame | u16 width, u16 height, controller pages |
| `0x11` | ColorFrame | u16 width, u16 height, RGB565 big-endian pixels |
| `0x12` | EPaperFrame | u16 width, u16 height, plane count (1 or 2), black plane, [red plane], refresh kind |
| `0x20` | Commands | command bytes as the chip receives them on a bus with a D/C line |
| `0x21` | Data | data bytes as the chip receives them on a bus with a D/C line |
| `0x22` | I2cWrite | 7-bit address, then the bytes of one write transaction |
| `0x23` | Reset | empty |
| `0x24` | Latch | the bytes of one chip-select frame (for example one MAX7219 latch) |

### Describe

Describe is advisory: a receiver never creates, resizes or reconfigures a target from it. It is parsed with at least 6
bytes. Unknown kind and chip byte values are still accepted (the C# enums keep them as they are). A name that is not valid UTF-8
is decoded with replacement characters; it does not make the message fail.

### MonoFrame

`width` bytes per page, one page per 8 rows, pages top to bottom. Bit 0 of a byte is the top row of the page. The payload
after the dimensions is exactly `width x ceil(height / 8)` bytes.

### ColorFrame

Rows from top to bottom, pixels left to right, each pixel two bytes (high byte first). The payload after the dimensions is exactly `width x height x 2` bytes.

### EPaperFrame

The frame is in the native (portrait) orientation of the panel, with the SSD1680 RAM layout: rows are padded to whole
bytes, the most significant bit is the leftmost pixel, padding bits are ignored. Each plane is `ceil(width / 8) x height`
bytes. Bit polarity follows the SSD1680 RAM: black plane (RAM 0x24) bit 0 = black, 1 = white; red plane (RAM 0x26) bit 1 = red, 0 = no red. The plane count is 1 (black only) or 2 (black, then red). The last byte is the refresh kind: `0` full, `1` partial.

GxEPD2's `_black_buffer` and both Adafruit_EPD buffers are already in this polarity and go out unchanged. ⚠️ A **GxEPD2_3C colour buffer is 0 = red**: invert the red plane (`~byte`) before sending it, as GxEPD2 itself does on its way to RAM 0x26.

The width is the width of the library's buffer, not of the glass. The 2.13" panels (122 x 250 visible) are sent **128 pixels wide**
(16 bytes per row, 4000 bytes per plane): GxEPD2 (`WIDTH = 128`) and Adafruit_EPD (the 122 short side rounded up to 128) both keep the
picture in columns 0 to 121 and bits 122 to 127 as padding. Describe them 128 x 250 and show them with an `EPaperDisplay` of
`PixelColumns = 128` and `VisibleColumns = 122`; a 122-wide frame sent to a 128-wide buffer is rejected as `SizeMismatch`. The 2.9"
panels are 128 x 296.

### Display kinds

| Value | Kind |
|---|---|
| `1` | CharacterLcd |
| `2` | MonoOled |
| `3` | ColorPanel |
| `4` | EPaper |
| `5` | LedMatrix |
| `6` | SevenSegment |
| `7` | GraphicLcd |

### Chips

| Value | Chip |
|---|---|
| `0x00` | Unknown |
| `0x01` | Hd44780 |
| `0x10` | Ssd1306 |
| `0x11` | Ssd1309 |
| `0x12` | Sh1106 |
| `0x20` | St7735 |
| `0x21` | St7789 |
| `0x22` | Ili9341 |
| `0x23` | Ssd1331 |
| `0x24` | Ssd1351 |
| `0x30` | Ssd1680 |
| `0x40` | Max7219 |
| `0x50` | St7920 |

## Payload caps

| Types | Default cap |
|---|---|
| Describe, Commands, Data, I2cWrite, Reset, Latch | 65,536 bytes |
| MonoFrame, ColorFrame, EPaperFrame | 4,194,304 bytes |

Both caps are configurable in a receiver. A header whose declared payload is over the cap of its type is noise, not a packet.

## Validation order and reject reasons

A decoder given exactly one packet checks in this order and stops at the first failure:

| Step | Check | Reason |
|---|---|---|
| 1 | At least 12 bytes | `Truncated` |
| 2 | Magic `0xA7 0xC0` | `Malformed` |
| 3 | Version is `1` | `UnsupportedVersion` |
| 4 | Type is one of the nine known types | `UnknownType` |
| 5 | Declared length is within the cap of the type | `TooLarge` |
| 6 | Declared length against the bytes present | `Truncated` when fewer bytes than declared, `Malformed` when more |
| 7 | CRC matches | `BadCrc` |

The cap check (5) comes before the length check (6) on purpose: a header that declares an oversized payload is rejected
as `TooLarge` without waiting for a body. The decoded payload is a copy that does not depend on the caller's buffer.

## Resync and timeouts

A stream receiver looks for the magic. When a header fails steps 2 to 5, or a complete packet fails the CRC, it is noise:
the receiver drops one byte and scans again for the magic, so a damaged packet costs at most that packet.
A frame header is also checked against its first payload bytes as soon as they arrive, before the receiver waits for the
rest of the body: width and height must be 1 to 65535, an EPaperFrame plane count must be 1 or 2, and the declared payload
length must be exactly what they need (MonoFrame `4 + w × ceil(h / 8)`, ColorFrame `4 + w × h × 2`, EPaperFrame
`5 + planes × ceil(w / 8) × h + 1`). A mismatch is noise (`Malformed`): one byte is dropped and the scan goes on, so a false
frame header in line noise does not hold up the good packets behind it until megabytes have arrived.
A packet that stops in the middle is dropped after `PartialPacketTimeout` (default 1 second) without new bytes, so a device that
reset mid-packet does not stall the link. The CRC is the only integrity check; there are no acknowledgements and no retransmissions.

Receiver behaviour that is not part of the wire format:

- A frame whose size differs from the size of its target is rejected as `SizeMismatch`.
- A target that throws is counted as `TargetFailed` and the receiver keeps running.
- A packet for a display id without a target is counted as `NoTarget`.

## Worked example

A Describe message for display id 0, an OLED named "OLED", 128 x 64, chip SSD1306:

| Bytes | Meaning |
|---|---|
| `A7 C0` | magic |
| `01` | version |
| `00` | display id |
| `01` | type Describe |
| `00` | flags |
| `0A 00 00 00` | payload length 10 |
| `02` | kind MonoOled |
| `10` | chip Ssd1306 |
| `80 00` | width 128 |
| `40 00` | height 64 |
| `4F 4C 45 44` | name "OLED" |
| `35 32` | CRC `0x3235` |

The whole packet is `A7C0010001000A0000000210800040004F4C45443532`. The CRC covers `01 00 01 00 0A 00 00 00 02 10 80 00 40 00 4F 4C 45 44`.
This is the `describe-oled-128x64-ssd1306` vector.
