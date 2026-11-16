import pytest

pytest.importorskip("PIL")
from PIL import Image  # noqa: E402

from atc_display_mirror import images  # noqa: E402
from atc_display_mirror.mirror import Mirror  # noqa: E402
from atc_display_mirror.protocol import DisplayKind, MessageType  # noqa: E402
from tests.conftest import Recorder  # noqa: E402


def make_16x8():
    img = Image.new("L", (16, 8), 0)
    img.putpixel((0, 0), 255)    # page 0, column 0, bit 0
    img.putpixel((1, 3), 128)    # threshold: >= 128 is lit -> bit 3
    img.putpixel((2, 7), 127)    # below threshold, stays off
    img.putpixel((15, 7), 255)   # column 15, bit 7
    return img


def test_mono_pages_layout():
    w, h, pages = images.mono_pages(make_16x8())
    assert (w, h) == (16, 8)
    assert len(pages) == 16
    assert pages[0] == 0x01 and pages[1] == 0x08 and pages[2] == 0x00 and pages[15] == 0x80


def test_mono_pages_two_pages_and_mode_1():
    img = Image.new("1", (4, 16), 0)
    img.putpixel((3, 9), 1)  # page 1, bit 1, byte 1*4+3
    w, h, pages = images.mono_pages(img)
    assert len(pages) == 8
    assert pages[7] == 0x02 and sum(pages) == 2


def test_rgb565_big_endian():
    img = Image.new("RGB", (16, 8), (0, 0, 0))
    img.putpixel((0, 0), (255, 0, 0))
    img.putpixel((1, 0), (0, 255, 0))
    img.putpixel((2, 0), (0, 0, 255))
    img.putpixel((3, 0), (255, 255, 255))
    w, h, px = images.rgb565_be(img)
    assert (w, h) == (16, 8) and len(px) == 16 * 8 * 2
    assert px[0:8].hex() == "f80007e0001fffff"
    assert px[8:10] == b"\x00\x00"


def test_epaper_plane_msb_first_padded():
    img = Image.new("L", (10, 2), 255)
    img.putpixel((0, 0), 0)
    img.putpixel((7, 0), 0)
    img.putpixel((9, 1), 0)
    w, h, plane = images.epaper_plane(img)
    assert (w, h) == (10, 2) and len(plane) == 4
    assert plane.hex() == "7effffbf"  # dark = 0, light and padding = 1


def test_epaper_red_plane_polarity():
    img = Image.new("L", (10, 2), 255)
    img.putpixel((0, 0), 0)
    img.putpixel((9, 1), 0)
    w, h, plane = images.epaper_red_plane(img)
    assert plane.hex() == "80000040"  # red = 1, no red and padding = 0


def test_send_image_kinds():
    rec = Recorder()
    m = Mirror(rec)
    m.send_image(0, make_16x8(), DisplayKind.MONO_OLED)
    m.send_image(1, Image.new("RGB", (2, 2), (255, 0, 0)), DisplayKind.COLOR_PANEL)
    black = Image.new("L", (8, 2), 255)
    red = Image.new("L", (8, 2), 255)
    red.putpixel((0, 0), 0)
    m.send_image(2, black, DisplayKind.EPAPER, red_image=red, partial=True)
    assert [p[4] for p in rec.packets] == [MessageType.MONO_FRAME, MessageType.COLOR_FRAME, MessageType.EPAPER_FRAME]
    ep = rec.packets[2]
    assert ep[10 + 4] == 2 and ep[-3] == 1  # two planes, partial
    assert ep[15 + 2] == 0x80 and ep[18] == 0x00  # red plane: red pixel = 1; black plane first byte 0xFF
    assert ep[15] == 0xFF
    with pytest.raises(ValueError):
        m.send_image(0, black, DisplayKind.CHARACTER_LCD)
