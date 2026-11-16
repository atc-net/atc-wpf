"""Transports: each has send(packet) and close()."""
from __future__ import annotations

import socket
from typing import Callable


class CallbackTransport:
    """Hands every packet to a function, for example an MQTT client's publish."""

    def __init__(self, fn: Callable[[bytes], None]) -> None:
        self._fn = fn

    def send(self, packet: bytes) -> None:
        self._fn(bytes(packet))

    def close(self) -> None:
        pass


class SerialTransport:
    """Writes whole packets to a serial port (needs the 'serial' extra: pyserial)."""

    def __init__(self, port: str, baudrate: int = 115200) -> None:
        try:
            import serial  # type: ignore
        except ImportError as ex:
            raise ImportError("SerialTransport needs pyserial: pip install atc-display-mirror[serial]") from ex
        self._serial = serial.Serial(port, baudrate)

    def send(self, packet: bytes) -> None:
        self._serial.write(bytes(packet))

    def close(self) -> None:
        self._serial.close()


class TcpTransport:
    """Connects lazily (with a connect timeout); reconnects once when the connection was broken."""

    def __init__(self, host: str, port: int, timeout: float = 5.0) -> None:
        self._address = (host, port)
        self._timeout = timeout
        self._sock = None

    def _connect(self) -> None:
        self._sock = socket.create_connection(self._address, timeout=self._timeout)

    def send(self, packet: bytes) -> None:
        data = bytes(packet)
        if self._sock is None:
            self._connect()
        try:
            self._sock.sendall(data)
        except (BrokenPipeError, ConnectionResetError, ConnectionAbortedError):
            self._drop()
            self._connect()
            self._sock.sendall(data)

    def _drop(self) -> None:
        if self._sock is not None:
            try:
                self._sock.close()
            except OSError:
                pass
        self._sock = None

    def close(self) -> None:
        self._drop()


class UdpTransport:
    """One packet per datagram. The host is resolved with getaddrinfo (IPv4 or IPv6)."""

    MAX_DATAGRAM = 65507

    def __init__(self, host: str, port: int) -> None:
        family, socktype, proto, _, address = socket.getaddrinfo(host, port, type=socket.SOCK_DGRAM)[0]
        self._address = address
        self._sock = socket.socket(family, socktype, proto)

    def send(self, packet: bytes) -> None:
        data = bytes(packet)
        if len(data) > self.MAX_DATAGRAM:
            raise ValueError("a packet of %d bytes does not fit in one UDP datagram (max %d); "
                             "use TCP (TcpTransport) or serial (SerialTransport) for large frames" % (len(data), self.MAX_DATAGRAM))
        self._sock.sendto(data, self._address)

    def close(self) -> None:
        self._sock.close()
