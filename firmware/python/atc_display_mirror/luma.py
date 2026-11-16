"""Tee for luma.core serial interfaces (i2c, spi): the same duck-typed API, no luma import needed."""
from __future__ import annotations

from typing import Callable, Optional

from ._errors import ErrorReporter


class MirroredSerialInterface:
    """Calls the inner interface first, then forwards command(*cmd) as Commands and data(data) as Data.

    An exception of the mirror or its transport is caught and handed to on_error (default: one logged warning per
    exception type); it never reaches the caller. An exception of the inner interface propagates and is not mirrored."""

    def __init__(self, inner, mirror, display_id: int,
                 on_error: Optional[Callable[[BaseException], None]] = None) -> None:
        self._inner = inner
        self._mirror = mirror
        self._display_id = display_id
        self._errors = ErrorReporter(on_error)

    def command(self, *cmd):
        result = self._inner.command(*cmd)
        self._errors.run(lambda: self._mirror.send_commands(self._display_id, bytes(cmd)))
        return result

    def data(self, data):
        data = list(data)
        result = self._inner.data(data)
        self._errors.run(lambda: self._mirror.send_data(self._display_id, bytes(data)))
        return result

    def cleanup(self):
        return self._inner.cleanup()

    def __getattr__(self, name):
        return getattr(self._inner, name)
