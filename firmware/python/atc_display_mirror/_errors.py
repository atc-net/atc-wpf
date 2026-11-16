"""Error reporting for the wrappers: mirroring must never break the real hardware."""
from __future__ import annotations

import logging
from typing import Callable, Optional

_log = logging.getLogger("atc_display_mirror")


class ErrorReporter:
    """Runs mirror calls, swallows their exceptions and reports them.

    With no on_error callback, a warning is logged the first time each exception type is seen."""

    def __init__(self, on_error: Optional[Callable[[BaseException], None]] = None) -> None:
        self._on_error = on_error
        self._seen = set()

    def report(self, ex: BaseException) -> None:
        try:
            if self._on_error is not None:
                self._on_error(ex)
            elif type(ex) not in self._seen:
                self._seen.add(type(ex))
                _log.warning("display mirroring failed (%s: %s); further errors of this type are not logged",
                             type(ex).__name__, ex)
        except Exception:  # a failing error handler must not break the hardware path either
            pass

    def run(self, fn, *args) -> None:
        try:
            fn(*args)
        except Exception as ex:
            self.report(ex)
