"""Mirror: builds packets for one link and sends them through a transport."""
from __future__ import annotations

from . import protocol
from .protocol import DisplayKind, MessageType


class Mirror:
    """Sends display traffic as protocol packets through a transport (anything with send(packet) and close())."""

    def __init__(self, transport) -> None:
        self.transport = transport

    def _send(self, display_id: int, msg_type: int, payload: bytes = b"") -> None:
        self.transport.send(protocol.encode_packet(display_id, msg_type, payload))

    def close(self) -> None:
        self.transport.close()

    def describe(self, display_id: int, kind: int, chip: int, width: int, height: int, name: str = "") -> None:
        self._send(display_id, MessageType.DESCRIBE, protocol.describe_payload(kind, chip, width, height, name))

    def send_mono_frame(self, display_id: int, width: int, height: int, pages: bytes) -> None:
        self._send(display_id, MessageType.MONO_FRAME, protocol.mono_frame_payload(width, height, pages))

    def send_color_frame(self, display_id: int, width: int, height: int, rgb565_be: bytes) -> None:
        self._send(display_id, MessageType.COLOR_FRAME, protocol.color_frame_payload(width, height, rgb565_be))

    def send_epaper_frame(self, display_id: int, width: int, height: int, black: bytes,
                          red: bytes = b"", partial: bool = False) -> None:
        self._send(display_id, MessageType.EPAPER_FRAME,
                   protocol.epaper_frame_payload(width, height, black, red, partial))

    def send_commands(self, display_id: int, data: bytes) -> None:
        self._send(display_id, MessageType.COMMANDS, bytes(data))

    def send_data(self, display_id: int, data: bytes) -> None:
        self._send(display_id, MessageType.DATA, bytes(data))

    def send_i2c_write(self, display_id: int, address: int, data: bytes) -> None:
        """I2cWrite: the 7-bit address, then the bytes of one write transaction after the address byte."""
        if not 0 <= address <= 0x7F:
            raise ValueError("address must be a 7-bit address")
        self._send(display_id, MessageType.I2C_WRITE, bytes((address,)) + bytes(data))

    def send_reset(self, display_id: int) -> None:
        self._send(display_id, MessageType.RESET)

    def send_latch(self, display_id: int, data: bytes) -> None:
        self._send(display_id, MessageType.LATCH, bytes(data))

    def send_image(self, display_id: int, image, kind: int, *, red_image=None, partial: bool = False) -> None:
        """Converts a Pillow image for the display kind and sends it as a frame."""
        from . import images  # Pillow is only needed for image input
        if kind == DisplayKind.MONO_OLED:
            w, h, pages = images.mono_pages(image)
            self.send_mono_frame(display_id, w, h, pages)
        elif kind == DisplayKind.COLOR_PANEL:
            w, h, px = images.rgb565_be(image)
            self.send_color_frame(display_id, w, h, px)
        elif kind == DisplayKind.EPAPER:
            w, h, black = images.epaper_plane(image)
            red = b""
            if red_image is not None:
                rw, rh, red = images.epaper_red_plane(red_image)
                if (rw, rh) != (w, h):
                    raise ValueError("red_image must have the size of image")
            self.send_epaper_frame(display_id, w, h, black, red, partial)
        else:
            raise ValueError("send_image supports MONO_OLED, COLOR_PANEL and EPAPER, not %r" % (kind,))
