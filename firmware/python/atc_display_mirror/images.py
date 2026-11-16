"""Pillow image conversions (needs the 'image' extra: Pillow). Only methods of the image object are used."""
from __future__ import annotations

from typing import Tuple


def mono_pages(image) -> Tuple[int, int, bytes]:
    """Image to controller pages: byte page * width + x, bit y % 8, lit when luminance >= 128."""
    g = image.convert("L")
    w, h = g.size
    px = g.load()
    pages = bytearray(w * ((h + 7) // 8))
    for y in range(h):
        mask = 1 << (y % 8)
        base = (y // 8) * w
        for x in range(w):
            if px[x, y] >= 128:
                pages[base + x] |= mask
    return w, h, bytes(pages)


def rgb565_be(image) -> Tuple[int, int, bytes]:
    """Image to big-endian RGB565 (r>>3, g>>2, b>>3), rows top to bottom."""
    rgb = image.convert("RGB")
    w, h = rgb.size
    px = rgb.load()
    out = bytearray(w * h * 2)
    i = 0
    for y in range(h):
        for x in range(w):
            r, g, b = px[x, y][:3]
            v = ((r >> 3) << 11) | ((g >> 2) << 5) | (b >> 3)
            out[i] = v >> 8
            out[i + 1] = v & 0xFF
            i += 2
    return w, h, bytes(out)


def _epaper_bits(image, dark_bit: int):
    g = image.convert("L")
    w, h = g.size
    px = g.load()
    stride = (w + 7) // 8
    fill = 0x00 if dark_bit else 0xFF
    plane = bytearray([fill]) * (stride * h)
    for y in range(h):
        for x in range(w):
            if px[x, y] < 128:
                if dark_bit:
                    plane[y * stride + (x >> 3)] |= 0x80 >> (x & 7)
                else:
                    plane[y * stride + (x >> 3)] &= ~(0x80 >> (x & 7)) & 0xFF
    return w, h, bytes(plane)


def epaper_plane(image) -> Tuple[int, int, bytes]:
    """Black plane (SSD1680 RAM 0x24 polarity): native orientation, rows padded to bytes, MSB = leftmost pixel.

    A dark pixel (luminance < 128) is bit 0 (black), a light pixel is bit 1 (white); padding bits are 1."""
    return _epaper_bits(image, 0)


def epaper_red_plane(image) -> Tuple[int, int, bytes]:
    """Red plane (SSD1680 RAM 0x26 polarity): a dark pixel (luminance < 128) of the red image is bit 1 (red),
    a light pixel is bit 0 (no red); padding bits are 0."""
    return _epaper_bits(image, 1)
