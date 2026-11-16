# AtcDisplayMirror

The ESP32 side of display mirroring. It sends what a sketch shows on a display, either the picture or the raw bus
traffic, to the Atc.Wpf.Displays controls (`DisplayMirrorReceiver`) over serial, TCP, UDP or MQTT.
The wire format is described in [DisplayMirrorProtocol](../../../docs/Displays/DisplayMirrorProtocol.md).

## Install

Copy this folder to your Arduino `libraries` folder, or zip it and use **Sketch > Include Library > Add .ZIP Library**.
The library has no required dependencies. The examples use U8g2, Adafruit SSD1306/GFX, TFT_eSPI, PubSubClient or GxEPD2.
Install the ones you need.

`architectures=esp32`. The protocol encoder `src/AtcMirrorProtocol.h` has no Arduino dependency: it is plain C++ with
only `<cstdint>` and `<cstddef>`. You can copy it to any C++11 (or later) project.

## Sinks

```cpp
#include <AtcDisplayMirror.h>
AtcDisplayMirror mirror;

mirror.begin(Serial);                                  // any Stream: Serial, WiFiClient (TCP)
mirror.begin(udp, IPAddress(192, 168, 1, 10), 5556);   // WiFiUDP: one packet per datagram
mirror.begin([](const uint8_t* p, size_t n) {          // callback returning bool: send* reports it
  return mqtt.publish("atc/display/0", p, n);
});
mirror.begin([](const uint8_t* p, size_t n) {          // void callback: send* cannot see delivery
  udpLikeThing.send(p, n);
});
```

| Sink | Behaviour |
|---|---|
| `Stream&` | Each packet is streamed with `write()`. Frames are not copied, so a 320×240 colour frame (153,612 bytes) needs no buffer. |
| `UDP&`, `IPAddress`, port | One packet per datagram. The ESP32 core's `WiFiUDP` (`NetworkUDP`) has a 1,460-byte transmit buffer and sends a datagram by itself when that buffer fills. A larger packet would arrive as several datagrams, and the receiver would reject each one. So packets over 1,460 bytes are refused: the `send*` call returns `false`. A 128×64 MonoFrame (1,040 bytes) fits, and so does all U8g2 tee traffic. Use TCP or serial for colour and e-paper frames. The optional fourth argument raises the limit (at most 65,507) for a UDP class that sends larger datagrams. |
| callback | Each whole packet is assembled in a heap buffer (`begin(cb, heapLimit)`, grown on demand) or in your own buffer (`begin(cb, buffer, capacity)`). It is then handed to the callback. A packet larger than the buffer or the limit is refused. The default `heapLimit` is 65,548 bytes: the largest Commands, Data, I2cWrite, Latch or Describe packet (65,536-byte payload + 12). Frames up to that size fit too. Larger frames need an explicit, larger `heapLimit`. A callback that returns `bool` (a lambda, a function or `AtcDisplayMirror::CheckedPacketCallback`) reports delivery: `false` makes the `send*` call return `false`. A `void` callback (`AtcDisplayMirror::PacketCallback`) cannot, so `send*` only fails on a buffer problem. |

Every `send*` returns `true` when the whole packet was handed to the sink. With a `Stream`, a failed write can leave
part of a packet on the wire. The receiver drops it on the next magic or after its partial-packet timeout (1 s).
`failedPackets()` counts the failures since `begin()`. The class is not thread safe: call it from one task.

### MQTT (PubSubClient)

PubSubClient refuses a `publish()` unless its buffer holds 5 + 2 + topic length + payload bytes, and the default buffer is
256 bytes. Raise it with `mqtt.setBufferSize(n)` before you send frames. `n` is a `uint16_t`, so at most 65,535.
For bigger packets, use PubSubClient's `beginPublish(topic, length, retained)`, `write()` and `endPublish()`. It is a
`Print`, but `beginPublish` needs the packet length first: 12 + the payload length. See `examples/MqttPubSubClient`. Its callback returns the result of `publish()`, so a refused publish (for example, a
buffer that is too small) makes `sendMonoFrame` return `false`.

## Messages

```cpp
using namespace atc_mirror;
mirror.describe(0, DisplayKind::MonoOled, Chip::Ssd1306, 128, 64, "OLED");   // advisory
mirror.sendMonoFrame(0, display.getBuffer(), 128, 64);           // pages, bit 0 = top row of the page
mirror.sendColorFrame(1, (const uint16_t*)sprite.getPointer(), 160, 128, true);
mirror.sendEPaperFrame(2, blackPlane, redPlaneOrNull, 128, 296, /*partial*/ false);
mirror.sendCommands(0, bytes, n);    // D/C low
mirror.sendData(0, bytes, n);        // D/C high, or the serial stream of an ST7920
mirror.sendI2cWrite(0, 0x3C, bytes, n);
mirror.sendReset(0);
mirror.sendLatch(4, frame, n);       // one MAX7219 chip-select frame
```

- **Colour byte order.** RGB565 goes out big-endian. Set `pixelsAreBigEndian = true` when the bytes in memory are
  already high byte first; TFT_eSPI sprites store their 16-bit pixels byte-swapped. Set it to `false` for ordinary
  `uint16_t` values, such as `GFXcanvas16::getBuffer()`. The conversion runs 64 pixels at a time on the stack.
  It never allocates.
- **E-paper.** Planes are in the panel's native (portrait) orientation, rows are padded to whole bytes, and the MSB is
  the leftmost pixel. The bit polarity is the SSD1680 RAM's. Black plane: 0 = black, 1 = white. Red plane: 1 = red,
  0 = no red. GxEPD2's black buffer and Adafruit_EPD's buffers go out unchanged. ⚠️ A GxEPD2_3C colour buffer is
  0 = red: invert the red plane (`~byte`) before sending it.
- **2.13" width.** Send the buffer's width, not the glass's: the 2.13" panels (122 x 250 visible) go out **128 x 250**
  (16 bytes per row, columns 122 to 127 padding, as GxEPD2 and Adafruit_EPD keep them); the 2.9" panels 128 x 296.
  The `EPaperDisplay` shows a 2.13" panel with `PixelColumns = 128` and `VisibleColumns = 122`.
- **Sizes.** Width and height must be 1 to 65,535. A frame payload is at most 4 MiB. Other payloads are at most 64 KiB.

## Raw traffic: U8g2 tee

```cpp
#include <AtcU8g2Tee.h>
U8G2_SSD1306_128X64_NONAME_F_HW_I2C u8g2(U8G2_R0);
atcMirrorTee(u8g2.getU8x8(), mirror, 0);   // after the constructor, before begin()
u8g2.begin();
```

Every U8g2 bus byte goes through `u8x8->byte_cb`. The tee stores the original byte procedure and installs a wrapper
in its place. The wrapper calls the original first, so the real display is never delayed, and then forwards the bytes.
The bus mode is detected when the tee is installed. You can also pass it as the last argument:

| Mode | Detected when | Sent as |
|---|---|---|
| `I2c` | The byte procedure is `u8x8_byte_arduino_hw_i2c`, `_2nd_hw_i2c`, `_sw_i2c` or `u8x8_byte_sw_i2c`. | One `I2cWrite` per START..END transaction. The address is `u8x8->i2c_address >> 1`. The bytes include the SSD13xx control byte (0x00 or 0x40), ready for `Ssd1306I2cDecoder`. |
| `St7920Serial` | `cad_cb == u8x8_cad_st7920_spi` | `Data`: the serial framing as it is on the wire (0xF8/0xFA sync byte, then the nibble bytes), for the ST7920 serial decoder. |
| `Latch` | `display_cb` is a `u8x8_d_max7219_*` driver | One `Latch` per chip-select transfer. U8g2 drives the MAX7219: it sends a (register, data) pair per device. |
| `DcBus` | Anything else (4-wire SPI) | `Commands` while the last `SET_DC` was 0, `Data` while it was 1. |

Bytes are collected in a buffer of up to 256 bytes per display. They are sent at the end of a transfer, on a D/C
change, or when the buffer is full. Up to four displays can be teed at once. U8g2 has no user pointer on the ESP32,
so the tee keeps a small static table. `atcMirrorUntee()` restores the original procedure.

A custom or ESP-IDF I2C byte procedure is not one of the four above, so `Auto` takes it as `DcBus`. In that case, pass the mode
explicitly: `atcMirrorTee(u8x8, mirror, 0, nullptr, AtcTeeBus::I2c)`.

Mirroring is synchronous: the packets are sent from inside U8g2's byte procedure, right after the real transfer. A
slow sink (a congested TCP connection, a low serial baud rate) therefore adds latency to every display update. Over
UDP, each I2C transaction becomes its own datagram: one per command (with its arguments) and one per 24-byte data
chunk, which comes to dozens of small datagrams for a full SSD1306 128x64 frame. For SSD1306 over I2C, prefer TCP or
serial, or send whole frames with `sendMonoFrame`.

⚠️ An I2C transaction or a latch longer than 256 bytes is split into several packets. The stock U8g2 drivers never
send one that long: SSD13xx data goes in 24-byte transfers, and a MAX7219 latch is 2 bytes per device.

Arduino `TwoWire` and `SPIClass` are not wrapped. On the ESP32 core 3.x, `TwoWire::beginTransmission`, `write` and
`endTransmission` are virtual overrides of `HardwareI2C`. A `TwoWire` subclass could tee them for libraries that
accept a `TwoWire*`, such as Adafruit_SSD1306. This library does not provide one. For libraries without hooks
(Adafruit_SSD1306, TFT_eSPI, LiquidCrystal_I2C), send frames or make explicit calls.

## Examples

| Example | Shows |
|---|---|
| `Ssd1306FrameSerial` | Adafruit_SSD1306 `getBuffer()` → `sendMonoFrame` over Serial |
| `TftEspiSpriteTcp` | TFT_eSPI sprite → `sendColorFrame(..., true)` over TCP (`WiFiClient`) |
| `U8g2TeeUdp` | U8g2 SSD1306 I2C traffic → `I2cWrite` over UDP |
| `MqttPubSubClient` | Callback sink → PubSubClient `publish`, with `setBufferSize` |
| `Max7219Latch` | Explicit MAX7219 SPI frames → `sendLatch`. MD_MAX72XX keeps its register images private, so use your own frames or U8g2 with the tee. |
| `EPaperFrame` | GxEPD2 (SSD1680 2.9") drawn from a `GFXcanvas1` → `sendEPaperFrame` |

The receiver defaults in the WPF sample are TCP port 5555, UDP port 5556 and serial at 921,600 baud.

## Host test

`test/host` checks the encoder against the golden vectors on a PC. See [test/host/README.md](test/host/README.md).
