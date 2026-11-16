# atc-display-mirror

The Raspberry Pi side of display mirroring. It sends what a program draws on a display (or the picture itself) to the
Atc.Wpf.Displays controls over serial, TCP, UDP or MQTT. Wire format: [DisplayMirrorProtocol](../../docs/Displays/DisplayMirrorProtocol.md).

## Install

```bash
pip install -e "firmware/python"              # no runtime dependencies
pip install -e "firmware/python[serial]"      # pyserial, for SerialTransport
pip install -e "firmware/python[image]"       # Pillow, for send_image
pip install -e "firmware/python[smbus]"       # smbus2, for the I2C tee
pip install -e "firmware/python[test]"        # pytest + Pillow
```

Python 3.9 or newer. pyserial, Pillow, smbus2 and paho-mqtt are imported only where they are used.

## Transports

```python
from atc_display_mirror import Mirror, SerialTransport, TcpTransport, UdpTransport, CallbackTransport

Mirror(SerialTransport("/dev/ttyUSB0", 115200))   # writes whole packets
Mirror(TcpTransport("pc.local", 5555))            # connects lazily, reconnects once on a broken connection
Mirror(UdpTransport("pc.local", 5556))            # one packet per datagram
Mirror(CallbackTransport(lambda p: client.publish("display/0", p)))   # MQTT via paho, see examples/mqtt_paho.py
```

## luma.oled

`MirroredSerialInterface` wraps a luma serial interface. Every `command(...)` and `data(...)` is sent as Commands and Data
for the display id, then passed to the real interface.

```python
serial = MirroredSerialInterface(i2c(port=1, address=0x3C), mirror, display_id=0)
device = ssd1306(serial)
```

See `examples/oled_luma_frame.py`.

## smbus tee

`MirroredSMBus(bus, mirror, {0x3C: 0})` forwards `write_byte`, `write_byte_data`, `write_i2c_block_data` and the write
messages of `i2c_rdwr` for mapped addresses as I2cWrite (address, then the bytes after the address byte), then calls the real
bus. See `examples/smbus_tee.py`.

## Error handling and ordering

The wrappers (`MirroredSMBus`, `MirroredSerialInterface`) call the real bus or interface first and mirror afterwards, so a
dead link never delays or breaks the real display. An exception of the mirror or its transport is caught and passed to the
optional `on_error(exception)` callback; without one, a warning is logged once per exception type (logger
`atc_display_mirror`). An error of the real bus propagates as usual and that write is not mirrored. Calling `Mirror`
directly raises as normal.

```python
bus = MirroredSMBus(SMBus(1), mirror, {0x3C: 0}, on_error=lambda ex: print("mirror:", ex))
```

`UdpTransport` rejects packets over 65,507 bytes (use TCP or serial for large frames); `TcpTransport(host, port, timeout=5.0)`.
`MirroredSMBus` also mirrors `write_word_data` (little-endian) and `write_block_data` (register, count, bytes) and works in a `with` block.

## Pillow frames

```python
mirror.send_image(0, image, DisplayKind.MONO_OLED)      # pages: byte page * width + x, bit y % 8, lit when >= 128
mirror.send_image(1, image, DisplayKind.COLOR_PANEL)    # RGB565 big-endian
```

## E-paper

Planes are in the panel's native (portrait) orientation, rows padded to whole bytes, the most significant bit is the
leftmost pixel, with the SSD1680 RAM polarity: black plane (RAM 0x24) 0 = black, 1 = white; red plane (RAM 0x26) 1 = red,
0 = no red. In `send_image` a pixel with luminance below 128 is black (black image) or red (red image). A GxEPD2_3C
colour buffer is 0 = red: invert the red plane (`bytes(~b & 0xFF for b in red)`) before sending it; Adafruit_EPD buffers
go out unchanged.

The width is the buffer's, not the glass's: the 2.13" panels (122 x 250 visible) are sent **128 x 250** (16 bytes per
row; columns 122 to 127 are padding, as in GxEPD2 and Adafruit_EPD), the 2.9" panels 128 x 296. Show a 2.13" panel with
`PixelColumns = 128` and `VisibleColumns = 122` on the `EPaperDisplay`.

```python
mirror.send_image(2, black_image, DisplayKind.EPAPER, red_image=red_image, partial=False)
mirror.send_epaper_frame(2, 128, 296, black_plane, red_plane, partial=True)   # raw planes
```

## Tests

```bash
python -m pytest firmware/python
```

The golden vectors in `firmware/protocol/test-vectors.json` are checked byte for byte.
