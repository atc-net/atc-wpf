import json
import pathlib
import struct

import pytest

from atc_display_mirror import protocol
from atc_display_mirror.mirror import Mirror
from atc_display_mirror.protocol import Chip, DisplayKind, MessageType, crc16, encode_packet
from tests.conftest import Recorder

VECTORS = json.loads(
    (pathlib.Path(__file__).resolve().parents[2] / "protocol" / "test-vectors.json").read_text(encoding="utf-8"))


def test_crc_check_value():
    assert crc16(b"123456789") == 0x29B1
    assert VECTORS["crcCheck"]["crc"] == "0x29B1"


@pytest.mark.parametrize("v", VECTORS["vectors"], ids=lambda v: v["name"])
def test_golden_vectors_encode(v):
    packet = encode_packet(v["displayId"], v["type"], bytes.fromhex(v["payloadHex"]), v["flags"])
    assert packet.hex() == v["packetHex"]


def test_enums_match_protocol():
    assert MessageType.DESCRIBE == 1 and MessageType.LATCH == 0x24 and MessageType.EPAPER_FRAME == 0x12
    assert DisplayKind.MONO_OLED == 2 and DisplayKind.GRAPHIC_LCD == 7
    assert Chip.SSD1306 == 0x10 and Chip.ST7920 == 0x50 and Chip.SSD1680 == 0x30


def test_payload_builders_match_vectors():
    by_name = {v["name"]: v for v in VECTORS["vectors"]}
    assert protocol.describe_payload(DisplayKind.MONO_OLED, Chip.SSD1306, 128, 64, "OLED").hex() == \
        by_name["describe-oled-128x64-ssd1306"]["payloadHex"]
    assert protocol.describe_payload(7, Chip.SSD1680 + 0x20, 128, 64, "ÆØÅ").hex() == \
        by_name["describe-non-ascii-name"]["payloadHex"]
    assert protocol.mono_frame_payload(8, 8, bytes.fromhex("ff818181818181ff")).hex() == \
        by_name["mono-frame-8x8"]["payloadHex"]
    assert protocol.color_frame_payload(2, 2, bytes.fromhex("f80007e0001fffff")).hex() == \
        by_name["color-frame-2x2"]["payloadHex"]
    assert protocol.epaper_frame_payload(8, 2, bytes.fromhex("f00f"), bytes.fromhex("0180"), partial=True).hex() == \
        by_name["epaper-frame-8x2-two-planes-partial"]["payloadHex"]
    assert protocol.epaper_frame_payload(8, 2, bytes.fromhex("f00f")).hex() == \
        by_name["epaper-frame-8x2-one-plane-full"]["payloadHex"]


def test_rejects_are_not_valid_packets():
    for r in VECTORS["rejects"]:
        p = bytes.fromhex(r["packetHex"])
        valid = (len(p) >= 12 and p[:2] == b"\xa7\xc0" and p[2] == 1
                 and struct.unpack_from("<I", p, 6)[0] == len(p) - 12
                 and struct.unpack_from("<H", p, len(p) - 2)[0] == crc16(p[2:-2])
                 and p[4] in (1, 0x10, 0x11, 0x12, 0x20, 0x21, 0x22, 0x23, 0x24))
        assert not valid, r["name"]


def test_encode_validation():
    with pytest.raises(ValueError):
        encode_packet(256, MessageType.RESET, b"")
    with pytest.raises(ValueError):
        encode_packet(-1, MessageType.RESET, b"")
    with pytest.raises(ValueError):
        encode_packet(0, MessageType.DATA, bytes(65537))
    encode_packet(0, MessageType.DATA, bytes(65536))
    with pytest.raises(ValueError):
        protocol.mono_frame_payload(8, 8, b"\x00")
    with pytest.raises(ValueError):
        protocol.color_frame_payload(2, 2, b"\x00")
    with pytest.raises(ValueError):
        protocol.epaper_frame_payload(8, 2, b"\x00")
    with pytest.raises(ValueError):
        protocol.epaper_frame_payload(8, 2, b"\x00\x00", b"\x00")


def test_mirror_sends_vectors():
    rec = Recorder()
    m = Mirror(rec)
    m.describe(0, DisplayKind.MONO_OLED, Chip.SSD1306, 128, 64, "OLED")
    m.send_commands(0, bytes.fromhex("aed580a83f"))
    m.send_data(0, bytes.fromhex("00ff017f"))
    m.send_i2c_write(0, 0x3C, bytes.fromhex("008d14"))
    m.send_reset(0)
    m.send_latch(4, bytes.fromhex("0c010900"))
    m.send_mono_frame(0, 8, 8, bytes.fromhex("ff818181818181ff"))
    m.send_color_frame(1, 2, 2, bytes.fromhex("f80007e0001fffff"))
    m.send_epaper_frame(2, 8, 2, bytes.fromhex("f00f"), bytes.fromhex("0180"), partial=True)
    by_name = {v["name"]: v["packetHex"] for v in VECTORS["vectors"]}
    expected = [by_name[n] for n in (
        "describe-oled-128x64-ssd1306", "commands", "data", "i2c-write-0x3c", "reset-empty", "latch-4-bytes",
        "mono-frame-8x8", "color-frame-2x2", "epaper-frame-8x2-two-planes-partial")]
    assert [p.hex() for p in rec.packets] == expected


def test_mirror_validates_ids_and_sizes():
    m = Mirror(Recorder())
    with pytest.raises(ValueError):
        m.send_reset(256)
    with pytest.raises(ValueError):
        m.send_data(0, bytes(65537))
    with pytest.raises(ValueError):
        m.send_mono_frame(0, 8, 8, b"")
    with pytest.raises(ValueError):
        m.send_i2c_write(0, 0x80, b"")
