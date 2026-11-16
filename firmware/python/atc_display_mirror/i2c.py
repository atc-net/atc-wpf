"""Tee for an smbus/smbus2 bus object: calls the real bus first, then mirrors writes to mapped addresses.

Mirroring never breaks the hardware: an exception of the mirror or its transport is caught and handed to on_error
(default: one logged warning per exception type); an exception of the real bus propagates and is not mirrored."""
from __future__ import annotations

from typing import Callable, Dict, Optional

from ._errors import ErrorReporter


class MirroredSMBus:
    """Wraps a real bus. address_map maps a 7-bit I2C address to a display id; other addresses are not mirrored."""

    def __init__(self, bus, mirror, address_map: Dict[int, int],
                 on_error: Optional[Callable[[BaseException], None]] = None) -> None:
        self._bus = bus
        self._mirror = mirror
        self._map = dict(address_map)
        self._errors = ErrorReporter(on_error)

    def _tee(self, address: int, data) -> None:
        try:
            display_id = self._map.get(address)
            if display_id is not None:
                self._mirror.send_i2c_write(display_id, address, bytes(data))
        except Exception as ex:
            self._errors.report(ex)

    def write_byte(self, i2c_addr, value, *args, **kwargs):
        result = self._bus.write_byte(i2c_addr, value, *args, **kwargs)
        self._tee(i2c_addr, [value])
        return result

    def write_byte_data(self, i2c_addr, register, value, *args, **kwargs):
        result = self._bus.write_byte_data(i2c_addr, register, value, *args, **kwargs)
        self._tee(i2c_addr, [register, value])
        return result

    def write_word_data(self, i2c_addr, register, value, *args, **kwargs):
        result = self._bus.write_word_data(i2c_addr, register, value, *args, **kwargs)
        self._tee(i2c_addr, [register, value & 0xFF, (value >> 8) & 0xFF])  # SMBus words are little-endian
        return result

    def write_block_data(self, i2c_addr, register, data, *args, **kwargs):
        data = list(data)
        result = self._bus.write_block_data(i2c_addr, register, data, *args, **kwargs)
        self._tee(i2c_addr, [register, len(data)] + data)  # SMBus block write: register, count, bytes
        return result

    def write_i2c_block_data(self, i2c_addr, register, data, *args, **kwargs):
        data = list(data)
        result = self._bus.write_i2c_block_data(i2c_addr, register, data, *args, **kwargs)
        self._tee(i2c_addr, [register] + data)
        return result

    def i2c_rdwr(self, *i2c_msgs):
        result = self._bus.i2c_rdwr(*i2c_msgs)
        for msg in i2c_msgs:
            # smbus2.i2c_msg: a write message has the read flag (bit 0 of flags) clear
            try:
                if not (getattr(msg, "flags", 0) & 1):
                    self._tee(msg.addr, bytes(list(msg)))
            except Exception as ex:
                self._errors.report(ex)
        return result

    def __enter__(self):
        self._bus.__enter__()
        return self

    def __exit__(self, *exc_info):
        return self._bus.__exit__(*exc_info)

    def __getattr__(self, name):
        return getattr(self._bus, name)
