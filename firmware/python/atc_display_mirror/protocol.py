"""Wire codec of the display mirroring protocol (version 1). See docs/Displays/DisplayMirrorProtocol.md."""
from __future__ import annotations

import binascii
import struct
from enum import IntEnum

MAGIC = b"\xa7\xc0"
VERSION = 1
MAX_SMALL_PAYLOAD = 65536
MAX_FRAME_PAYLOAD = 4194304


class MessageType(IntEnum):
    DESCRIBE = 0x01
    MONO_FRAME = 0x10
    COLOR_FRAME = 0x11
    EPAPER_FRAME = 0x12
    COMMANDS = 0x20
    DATA = 0x21
    I2C_WRITE = 0x22
    RESET = 0x23
    LATCH = 0x24


class DisplayKind(IntEnum):
    CHARACTER_LCD = 1
    MONO_OLED = 2
    COLOR_PANEL = 3
    EPAPER = 4
    LED_MATRIX = 5
    SEVEN_SEGMENT = 6
    GRAPHIC_LCD = 7


class Chip(IntEnum):
    UNKNOWN = 0x00
    HD44780 = 0x01
    SSD1306 = 0x10
    SSD1309 = 0x11
    SH1106 = 0x12
    ST7735 = 0x20
    ST7789 = 0x21
    ILI9341 = 0x22
    SSD1331 = 0x23
    SSD1351 = 0x24
    SSD1680 = 0x30
    MAX7219 = 0x40
    ST7920 = 0x50


def crc16(data: bytes) -> int:
    """CRC-16/CCITT-FALSE (poly 0x1021, init 0xFFFF)."""
    return binascii.crc_hqx(bytes(data), 0xFFFF)


def max_payload(msg_type: int) -> int:
    """Default payload cap of a message type."""
    if msg_type in (MessageType.MONO_FRAME, MessageType.COLOR_FRAME, MessageType.EPAPER_FRAME):
        return MAX_FRAME_PAYLOAD
    return MAX_SMALL_PAYLOAD


def encode_packet(display_id: int, msg_type: int, payload: bytes, flags: int = 0) -> bytes:
    """Builds one packet: magic, header, payload, little-endian CRC over header bytes 2..9 and the payload."""
    if not 0 <= display_id <= 255:
        raise ValueError("display_id must be 0..255")
    if not 0 <= flags <= 255:
        raise ValueError("flags must be 0..255")
    if not 0 <= int(msg_type) <= 255:
        raise ValueError("msg_type must be 0..255")
    payload = bytes(payload)
    cap = max_payload(msg_type)
    if len(payload) > cap:
        raise ValueError("payload of %d bytes is over the cap of %d" % (len(payload), cap))
    body = struct.pack("<BBBBI", VERSION, display_id, int(msg_type), flags, len(payload))
    return MAGIC + body + payload + struct.pack("<H", crc16(body + payload))


def _check_dim(value: int, name: str) -> None:
    if not 1 <= value <= 65535:
        raise ValueError("%s must be 1..65535" % name)


def mono_frame_payload(width: int, height: int, pages: bytes) -> bytes:
    """u16 width, u16 height, then width x ceil(height / 8) controller page bytes."""
    _check_dim(width, "width")
    _check_dim(height, "height")
    pages = bytes(pages)
    expected = width * ((height + 7) // 8)
    if len(pages) != expected:
        raise ValueError("pages must be %d bytes, got %d" % (expected, len(pages)))
    return struct.pack("<HH", width, height) + pages


def color_frame_payload(width: int, height: int, rgb565_be: bytes) -> bytes:
    """u16 width, u16 height, then width x height x 2 bytes of big-endian RGB565."""
    _check_dim(width, "width")
    _check_dim(height, "height")
    pixels = bytes(rgb565_be)
    expected = width * height * 2
    if len(pixels) != expected:
        raise ValueError("pixels must be %d bytes, got %d" % (expected, len(pixels)))
    return struct.pack("<HH", width, height) + pixels


def epaper_frame_payload(width: int, height: int, black: bytes, red: bytes = b"", partial: bool = False) -> bytes:
    """u16 width, u16 height, plane count, black plane, [red plane], refresh kind (0 full, 1 partial).

    Each plane is ceil(width / 8) x height bytes, MSB = leftmost pixel; SSD1680 RAM polarity
    (black plane: 0 = black, 1 = white; red plane: 1 = red, 0 = no red)."""
    _check_dim(width, "width")
    _check_dim(height, "height")
    black = bytes(black)
    red = bytes(red) if red else b""
    expected = ((width + 7) // 8) * height
    if len(black) != expected:
        raise ValueError("black plane must be %d bytes, got %d" % (expected, len(black)))
    if red and len(red) != expected:
        raise ValueError("red plane must be %d bytes, got %d" % (expected, len(red)))
    return (struct.pack("<HHB", width, height, 2 if red else 1) + black + red
            + bytes((1 if partial else 0,)))


def describe_payload(kind: int, chip: int, width: int, height: int, name: str = "") -> bytes:
    """kind, chip, u16 width, u16 height, UTF-8 name."""
    for v, n in ((kind, "kind"), (chip, "chip")):
        if not 0 <= int(v) <= 255:
            raise ValueError("%s must be 0..255" % n)
    for v, n in ((width, "width"), (height, "height")):
        if not 0 <= v <= 65535:
            raise ValueError("%s must be 0..65535" % n)
    return struct.pack("<BBHH", int(kind), int(chip), width, height) + name.encode("utf-8")
