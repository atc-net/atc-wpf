# 📡 DisplayMirrorReceiver

Receives display mirroring packets from an ESP32, Arduino or Raspberry Pi over a serial port, TCP, UDP or MQTT, and applies them to the display controls, so a screen shows what the device's display shows.

## 🔍 Overview

A device sends what it puts on its display (whole frames, or the bytes it sends to the display chip) as small packets. `DisplayMirrorReceiver` finds the packets in a byte stream (`Feed`, `RunAsync`) or takes one packet per message (`ProcessMessage`, `RunUdpAsync`), checks them and applies each one to the `IDisplayMirrorTarget` registered for its display id: a frame buffer, a chip emulator or a bus decoder. One link can carry up to 256 displays.

- **Robust on a noisy link.** A damaged packet costs only that packet: the CRC fails, the receiver skips one byte and looks for the next packet. A header that declares an impossible size is noise and never makes the receiver wait or allocate. A packet that stops in the middle (the device was reset) is dropped after `PartialPacketTimeout`.
- **Statistics.** Packets, bytes, rejected and unsupported packets per display id, plus CRC errors and resyncs of the stream.
- **Describe.** A device can describe its displays (kind, chip, size, name). The receiver only reports it (`DescribeReceived`); the application decides what to create.

## 📍 Namespace

```csharp
using Atc.Wpf.Displays;
```

## 📦 Protocol in short

Every packet is a 10-byte header, the payload and a CRC. All multi-byte fields are little-endian.

| Bytes | Field |
|---|---|
| 2 | Magic `0xA7 0xC0` |
| 1 | Version `1` |
| 1 | Display id `0..255` |
| 1 | Type (below) |
| 1 | Flags (none in version 1) |
| 4 | Payload length |
| n | Payload |
| 2 | CRC-16/CCITT-FALSE over the header from the version byte, and the payload |

| Type | Payload | Typical target |
|---|---|---|
| `0x01` Describe | kind, chip, u16 width, u16 height, UTF-8 name | the application (`DescribeReceived`) |
| `0x10` MonoFrame | u16 width, u16 height, controller pages (bit 0 = top row) | `MonochromeFrameBuffer` |
| `0x11` ColorFrame | u16 width, u16 height, RGB565 pixels, high byte first | `ColorFrameBuffer` |
| `0x12` EPaperFrame | u16 width, u16 height, plane count, planes, refresh kind | e-paper buffer |
| `0x20` Commands / `0x21` Data | the bytes the chip receives with D/C low / high | `Ssd1306Controller`, `TftController`, `Ssd1331Controller`, `Ssd1351Controller`, `Hd44780Controller` |
| `0x22` I2cWrite | 7-bit address, then the bytes of one write | `Ssd1306I2cDecoder`, `Pcf8574Hd44780Decoder` |
| `0x23` Reset | empty | a chip emulator |
| `0x24` Latch | the bytes of one chip-select frame | a chip emulator |

The full specification, with the validation order, the reject reasons and a worked example, is in [DisplayMirrorProtocol.md](../../docs/Displays/DisplayMirrorProtocol.md). The golden vectors in `firmware/protocol/test-vectors.json` show the exact bytes. `DisplayMirrorProtocol.Encode` and the `Create*Payload` helpers build packets in C#.

## 🚀 Usage

### Targets

Register one target per display id. `DisplayMirrorTargets` creates the built-in targets; `Combine` tries several in order, so one display can take frames and chip traffic.

```csharp
public sealed class MirrorViewModel : ViewModelBase, IDisposable
{
    private readonly Ssd1306Controller controller;

    public MirrorViewModel()
    {
        // Display 0: a 128 x 64 OLED that takes whole frames.
        Receiver.Register(0, DisplayMirrorTargets.For(Oled));

        // Display 1: a 16 x 2 LCD behind a PCF8574 I2C backpack (I2cWrite), or driven with Commands / Data.
        var lcdController = new Hd44780Controller(Lcd);
        Receiver.Register(1, DisplayMirrorTargets.Combine(
            DisplayMirrorTargets.For(new Pcf8574Hd44780Decoder(lcdController)),
            DisplayMirrorTargets.For(lcdController)));

        // Display 2: an SSD1306 driven by the device's library (I2C traffic or Commands / Data).
        controller = new Ssd1306Controller(Traffic);
        Receiver.Register(2, DisplayMirrorTargets.Combine(
            DisplayMirrorTargets.For(controller),
            DisplayMirrorTargets.For(new Ssd1306I2cDecoder(controller))));
    }

    public DisplayMirrorReceiver Receiver { get; } = new();

    public MonochromeFrameBuffer Oled { get; } = new(128, 64);

    public MonochromeFrameBuffer Traffic { get; } = new(128, 64);

    public CharacterDisplayBuffer Lcd { get; } = new(16, 2);

    public void Dispose()
    {
        Receiver.Dispose();
        controller.Dispose();
    }
}
```

```xml
<atc:OledDisplay PixelColumns="128" PixelRows="64" Buffer="{Binding Oled}" />
<atc:CharacterDisplay Columns="16" Rows="2" Buffer="{Binding Lcd}" />
<atc:OledDisplay PixelColumns="128" PixelRows="64" Buffer="{Binding Traffic}" />
```

A chip emulator applies the chip's power-on state to its buffer when it is created: display off and the RAM cleared, as on a module that was just powered. Whole frames loaded into that buffer stay invisible until the device's init switches the display on. So give a display that sends only frames a plain buffer target, or create the emulator when the first chip traffic arrives (as the sample does).

### Serial port

```csharp
using var port = new SerialPort("COM5", 921600);
port.Open();

// Returns when the token is cancelled or the stream ends; IOException (the device was unplugged) propagates.
await receiver.RunAsync(port.BaseStream, cancellationToken);
```

After a cancellation a read may still be pending on the stream, so close (dispose) the port and open a new one rather than calling `RunAsync` on it again.

### TCP

```csharp
using var listener = new TcpListener(IPAddress.Any, 5555);
listener.Start();
while (!cancellationToken.IsCancellationRequested)
{
    // One device at a time: accept the next when this one disconnects.
    using var client = await listener.AcceptTcpClientAsync(cancellationToken);
    receiver.ResetPartial(); // a new connection starts a new stream
    await receiver.RunAsync(client.GetStream(), cancellationToken);
}
```

The stream parser keeps one partial packet, so feed one stream at a time into a receiver. For several devices at once, use one receiver per connection.

### UDP

Every datagram holds exactly one packet. Frames larger than a datagram (about 64 KiB, less on most networks) need a stream transport.

```csharp
await receiver.RunUdpAsync(5556, cancellationToken);
```

The overload taking a `UdpClient` uses a socket you created (for example on a port picked with 0, or joined to a multicast group).

### MQTT

Publish one packet per message; the receiver takes it with `ProcessMessage`. With MQTTnet 5 (not referenced by this library):

```csharp
var client = new MqttClientFactory().CreateMqttClient();
client.ApplicationMessageReceivedAsync += e =>
{
    // One message = one packet. The return value tells why a packet was rejected (None when applied).
    receiver.ProcessMessage(e.ApplicationMessage.Payload.ToArray());
    return Task.CompletedTask;
};

await client.ConnectAsync(new MqttClientOptionsBuilder().WithTcpServer("broker.local").Build());
await client.SubscribeAsync("devices/esp32-kitchen/display");
```

### Describe

A device that describes its displays (once at start and then every few seconds, so a receiver started later learns them too) lets the application create the matching control. The event is raised on the thread that fed the packet: dispatch it to the UI thread without waiting.

```csharp
receiver.DescribeReceived += (_, e) =>
    Application.Current.Dispatcher.BeginInvoke(() => CreateCard(e.DisplayId, e.Describe));
```

`e.Describe` has `Kind` (`CharacterLcd`, `MonoOled`, `ColorPanel`, ...), `Chip`, `Width`, `Height` (columns and rows for a character display) and `Name`. Unknown kind and chip values are kept as they are. The receiver never creates or resizes a target itself, and a frame of another size than its target is rejected as `SizeMismatch`.

The sizes of the newer kinds: `EPaper` gives the native (portrait) width and height of the frames: 128 x 250 for the 2.13" (122 columns visible, shown with `VisibleColumns = 122`) and 128 x 296 for the 2.9"; `LedMatrix` gives pixels (8 per module, height 8); `SevenSegment` gives the digit count as the width and 1 as the height; `GraphicLcd` is 128 x 64. A Describe does not say whether an e-paper has red: the sample takes it from the module name and otherwise assumes black/white.

For an ST7920 the bytes the U8g2 tee sends are serial-framed (start bytes `0xF8` / `0xFA` and nibble pairs): register `DisplayMirrorTargets.For(new St7920SerialDecoder(controller))` and send them as Data. A device that sends logical ST7920 instruction and data bytes uses `DisplayMirrorTargets.For(controller)` instead; one target cannot tell the two Data forms apart. The sample's GraphicLcd card combines them so Data is serial-framed and Commands are logical instructions.

### Statistics

```csharp
var display = receiver.GetStatistics(0);     // Packets, Bytes, Unsupported, Rejected, LastPacket
var stream = receiver.GetStreamStatistics(); // CrcErrors, Resyncs, Rejected (before the display id could be trusted)
var all = receiver.GetAllStatistics();       // every display id that has seen a packet

receiver.PacketRejected += (_, e) => lastReason = e.Reason; // DisplayId is null for stream-level rejects
```

Poll the statistics on a timer (a few times a second) rather than updating the UI from `PacketRejected`, which can fire for every packet of a bad stream.

### The device side

The packet is simple enough to build by hand. In C on an ESP32 or Arduino (the CRC is CRC-16/CCITT-FALSE, the same as Python's `binascii.crc_hqx(data, 0xFFFF)`):

```cpp
uint16_t crc16(uint16_t crc, const uint8_t* data, size_t length) {
  while (length--) {
    crc ^= (uint16_t)(*data++) << 8;
    for (int i = 0; i < 8; i++) crc = (crc & 0x8000) ? (crc << 1) ^ 0x1021 : crc << 1;
  }
  return crc;
}

void sendPacket(Stream& out, uint8_t displayId, uint8_t type, const uint8_t* payload, uint32_t length) {
  uint8_t header[10] = { 0xA7, 0xC0, 1, displayId, type, 0,
                         (uint8_t)length, (uint8_t)(length >> 8), (uint8_t)(length >> 16), (uint8_t)(length >> 24) };
  uint16_t crc = crc16(0xFFFF, header + 2, 8);
  crc = crc16(crc, payload, length);
  out.write(header, 10);
  out.write(payload, length);
  out.write((uint8_t)crc);
  out.write((uint8_t)(crc >> 8));
}
```

A MonoFrame payload is the width and height (u16 little-endian) followed by `display.getBuffer()` (Adafruit_SSD1306) or `u8g2.getBufferPtr()` (U8g2, full-buffer constructor).

## ⚙️ Members

| Member | Description |
|--------|-------------|
| `Register(id, target)` / `Unregister(id)` | Sets or removes the target of a display id |
| `Feed(chunk)` | Feeds a chunk of a byte stream of any size; complete packets are applied before it returns |
| `ProcessMessage(packet)` | Processes a message that holds exactly one packet; returns the reject reason (`None` when applied) |
| `RunAsync(stream, token)` | Reads a stream and feeds it until it ends or the token is cancelled; the caller owns and closes the stream |
| `RunUdpAsync(port, token)` / `RunUdpAsync(client, token)` | Processes every datagram as one packet until cancelled |
| `ResetPartial()` | Discards a partial packet, for example when the stream was reconnected |
| `DescribeReceived` | A valid Describe arrived (whether or not a target is registered) |
| `PacketRejected` | A packet or header was rejected, with the reason and (when it can be trusted) the display id |
| `GetStatistics(id)` / `GetAllStatistics()` / `GetStreamStatistics()` | Counter snapshots |
| `MaxFramePayload` | Payload cap of the frame types; default 4 MiB |
| `MaxTrafficPayload` | Payload cap of the other types; default 64 KiB |
| `PartialPacketTimeout` | How long a partial packet is kept without new bytes; default 1 second |

| Target factory | Takes |
|---|---|
| `DisplayMirrorTargets.For(MonochromeFrameBuffer)` | MonoFrame |
| `DisplayMirrorTargets.For(ColorFrameBuffer)` | ColorFrame (RGB565, high byte first) |
| `DisplayMirrorTargets.For(EPaperFrameBuffer)` | EPaperFrame (loads the RAM, then refreshes full or partial) |
| `DisplayMirrorTargets.For(IGraphicDisplayController)` | Commands, Data, Latch, Reset (a `Max7219Controller` takes one CS frame per Latch; an `St7920Controller` takes logical instruction and data bytes) |
| `DisplayMirrorTargets.For(St7920SerialDecoder)` | Data (the serial-framed SPI bytes, `0xF8` / `0xFA` start bytes and nibble pairs, as the U8g2 tee sends them), Reset (chip and framing) |
| `DisplayMirrorTargets.For(Ssd1306I2cDecoder, address?)` | I2cWrite (all addresses, or one) |
| `DisplayMirrorTargets.For(Pcf8574Hd44780Decoder, address?)` | I2cWrite (all addresses, or one) |
| `DisplayMirrorTargets.For(Hd44780Controller)` | Commands (instructions), Data |
| `DisplayMirrorTargets.Combine(targets)` | The first target that does not answer `Unsupported` |

## 📝 Notes

- **Threading.** `Feed` and `ProcessMessage` may be called from any thread; one lock applies one packet at a time. The targets run on the feeding thread, and the buffers and emulators are thread-safe; the controls redraw on the UI thread. The events are raised on the feeding thread after the lock is released: use `Dispatcher.BeginInvoke`, never `Dispatcher.Invoke` from a `Changed` handler of a buffer the receiver feeds.
- **No re-entry.** A target, or a handler it raises, must not call `Feed`, `ProcessMessage` or `ResetPartial` on the receiver that is applying to it; that throws `InvalidOperationException`.
- **Payload lifetime.** The payload passed to `IDisplayMirrorTarget.Apply` is a pooled buffer that is reused when `Apply` returns. A custom target copies what it keeps (`packet.Payload.ToArray()`).
- **Target failures.** A target that throws is counted as `TargetFailed` and the receiver keeps running.
- **No acknowledgements.** The CRC is the only integrity check; a lost packet is not sent again. A frame stream recovers with the next frame; chip traffic can leave the emulator out of step until the device sends its init again.
- **Dispose.** `Dispose` unregisters every target; later packets are ignored.

## 🔗 Related Controls

- **OledDisplay** - monochrome OLED with a frame buffer, an SSD1306 / SSD1309 / SH1106 emulator and an I2C decoder
- **ColorGraphicDisplay** - colour TFT / OLED with a frame buffer and ST7735, ST7789, ILI9341, SSD1331 and SSD1351 emulators
- **CharacterDisplay** - HD44780 character LCD with an emulator and a PCF8574 I2C decoder

## 🎮 Sample Application

See the DisplayMirror sample in the Atc.Wpf.Sample application under **Wpf.Displays > Displays > DisplayMirror**: a TCP, UDP or serial (`SerialPortPicker`, baud rate) receiver with start and stop, a card per display id created from its Describe (or added by hand), live statistics per display and for the stream, and a **Loopback demo** that feeds an OLED frame stream, a colour frame stream and LCD I2C traffic through the same receiver without hardware, with some noise and a damaged packet to move the resync and CRC counters. A 40 x 4 character LCD (two HD44780 controllers) mirrors only its first controller (rows 1-2) in the sample.