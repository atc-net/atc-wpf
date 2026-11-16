import socket
import threading

from atc_display_mirror.i2c import MirroredSMBus
from atc_display_mirror.luma import MirroredSerialInterface
from atc_display_mirror.mirror import Mirror
from atc_display_mirror.protocol import MessageType
from atc_display_mirror.transports import CallbackTransport, TcpTransport, UdpTransport
import pytest

from tests.conftest import FailingTransport, Recorder


def payloads(rec):
    return [(p[3], p[4], p[10:-2]) for p in rec.packets]


class FakeBus:
    def __init__(self):
        self.calls = []

    def write_byte(self, a, v):
        self.calls.append(("wb", a, v))

    def write_byte_data(self, a, r, v):
        self.calls.append(("wbd", a, r, v))

    def write_i2c_block_data(self, a, r, d):
        self.calls.append(("wibd", a, r, list(d)))

    def write_word_data(self, a, r, v):
        self.calls.append(("wwd", a, r, v))

    def write_block_data(self, a, r, d):
        self.calls.append(("wbl", a, r, list(d)))

    def __enter__(self):
        return self

    def __exit__(self, *exc):
        self.calls.append(("exit",))

    def i2c_rdwr(self, *msgs):
        self.calls.append(("rdwr", len(msgs)))

    def read_byte(self, a):
        return 42


class Msg:
    def __init__(self, addr, data, flags=0):
        self.addr, self.data, self.flags = addr, data, flags

    def __iter__(self):
        return iter(self.data)


def test_smbus_tee_forwards_mapped_addresses_only():
    rec, bus = Recorder(), FakeBus()
    b = MirroredSMBus(bus, Mirror(rec), {0x3C: 5})
    b.write_byte_data(0x3C, 0x00, 0xAF)
    b.write_i2c_block_data(0x3C, 0x40, [1, 2, 3])
    b.write_byte(0x3C, 0x7F)
    b.write_byte_data(0x50, 1, 2)  # unmapped
    I2C = int(MessageType.I2C_WRITE)
    assert payloads(rec) == [(5, I2C, bytes([0x3C, 0x00, 0xAF])),
                             (5, I2C, bytes([0x3C, 0x40, 1, 2, 3])),
                             (5, I2C, bytes([0x3C, 0x7F]))]
    assert [c[0] for c in bus.calls] == ["wbd", "wibd", "wb", "wbd"]
    assert b.read_byte(1) == 42


def test_smbus_rdwr_write_messages():
    rec, bus = Recorder(), FakeBus()
    b = MirroredSMBus(bus, Mirror(rec), {0x3C: 0})
    b.i2c_rdwr(Msg(0x3C, [0x40, 9, 8]), Msg(0x3C, [1], flags=1), Msg(0x10, [1]))
    assert payloads(rec) == [(0, int(MessageType.I2C_WRITE), bytes([0x3C, 0x40, 9, 8]))]
    assert bus.calls == [("rdwr", 3)]


class FakeInterface:
    def __init__(self):
        self.calls = []
        self.mode = "i2c"

    def command(self, *cmd):
        self.calls.append(("c", cmd))

    def data(self, data):
        self.calls.append(("d", list(data)))

    def cleanup(self):
        self.calls.append(("x",))


def test_luma_interface():
    rec, inner = Recorder(), FakeInterface()
    i = MirroredSerialInterface(inner, Mirror(rec), 7)
    i.command(0xAE, 0xD5, 0x80)
    i.data([0, 255, 1])
    i.cleanup()
    assert payloads(rec) == [(7, int(MessageType.COMMANDS), bytes([0xAE, 0xD5, 0x80])),
                             (7, int(MessageType.DATA), bytes([0, 255, 1]))]
    assert inner.calls == [("c", (0xAE, 0xD5, 0x80)), ("d", [0, 255, 1]), ("x",)]
    assert i.mode == "i2c"


def test_callback_transport():
    got = []
    t = CallbackTransport(got.append)
    t.send(b"abc")
    t.close()
    assert got == [b"abc"]


def test_udp_one_packet_per_datagram():
    srv = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    srv.bind(("127.0.0.1", 0))
    srv.settimeout(2)
    t = UdpTransport("127.0.0.1", srv.getsockname()[1])
    t.send(b"one")
    t.send(b"two!")
    assert srv.recv(100) == b"one" and srv.recv(100) == b"two!"
    t.close()
    srv.close()


def test_tcp_lazy_connect_and_reconnect():
    srv = socket.socket()
    srv.bind(("127.0.0.1", 0))
    srv.listen(2)
    srv.settimeout(2)
    port = srv.getsockname()[1]
    t = TcpTransport("127.0.0.1", port)  # nothing connected yet
    conns = []

    def accept():
        c, _ = srv.accept()
        c.settimeout(2)
        conns.append(c)

    th = threading.Thread(target=accept)
    th.start()
    t.send(b"hello")
    th.join()
    assert conns[0].recv(10) == b"hello"
    class Broken:
        def sendall(self, data):
            raise BrokenPipeError()

        def close(self):
            pass

    t._sock = Broken()  # the peer dropped the connection
    th = threading.Thread(target=accept)
    th.start()
    t.send(b"again")  # reconnects once and resends
    th.join()
    assert len(conns) == 2 and conns[1].recv(10) == b"again"
    t.close()
    srv.close()


def test_tcp_retries_on_connection_aborted():
    class Aborting:
        def sendall(self, data):
            raise ConnectionAbortedError()

        def close(self):
            pass

    srv = socket.socket()
    srv.bind(("127.0.0.1", 0))
    srv.listen(1)
    srv.settimeout(2)
    t = TcpTransport("127.0.0.1", srv.getsockname()[1], timeout=2)
    t._sock = Aborting()
    t.send(b"x")
    c, _ = srv.accept()
    c.settimeout(2)
    assert c.recv(5) == b"x"
    t.close()
    c.close()
    srv.close()


def test_udp_rejects_oversized_packet():
    t = UdpTransport("127.0.0.1", 9)
    with pytest.raises(ValueError, match="TCP"):
        t.send(bytes(65508))
    t.close()


def test_smbus_word_and_block_data():
    rec, bus = Recorder(), FakeBus()
    b = MirroredSMBus(bus, Mirror(rec), {0x3C: 1})
    b.write_word_data(0x3C, 0x10, 0x1234)
    b.write_block_data(0x3C, 0x40, [7, 8])
    I2C = int(MessageType.I2C_WRITE)
    assert payloads(rec) == [(1, I2C, bytes([0x3C, 0x10, 0x34, 0x12])),
                             (1, I2C, bytes([0x3C, 0x40, 2, 7, 8]))]
    assert [c[0] for c in bus.calls] == ["wwd", "wbl"]


def test_smbus_context_manager():
    bus = FakeBus()
    with MirroredSMBus(bus, Mirror(Recorder()), {}) as b:
        assert isinstance(b, MirroredSMBus)
    assert bus.calls[-1] == ("exit",)


def test_smbus_calls_bus_first_and_survives_transport_failure():
    errors, bus, t = [], FakeBus(), FailingTransport()
    b = MirroredSMBus(bus, Mirror(t), {0x3C: 0}, on_error=errors.append)
    b.write_byte_data(0x3C, 0, 0xAF)
    b.write_i2c_block_data(0x3C, 0, [1])
    b.i2c_rdwr(Msg(0x3C, [1]))
    assert [c[0] for c in bus.calls] == ["wbd", "wibd", "rdwr"]
    assert t.attempts == 3 and len(errors) >= 1 and isinstance(errors[0], OSError)


def test_smbus_bus_failure_propagates_and_is_not_mirrored():
    class Bad(FakeBus):
        def write_byte_data(self, a, r, v):
            raise OSError("nack")

    rec = Recorder()
    b = MirroredSMBus(Bad(), Mirror(rec), {0x3C: 0})
    with pytest.raises(OSError):
        b.write_byte_data(0x3C, 0, 1)
    assert rec.packets == []


def test_luma_survives_transport_failure_and_reports():
    errors, inner, t = [], FakeInterface(), FailingTransport()
    i = MirroredSerialInterface(inner, Mirror(t), 0, on_error=errors.append)
    i.command(0xAE)
    i.data([1, 2])
    assert inner.calls == [("c", (0xAE,)), ("d", [1, 2])]
    assert t.attempts == 2 and errors and isinstance(errors[0], OSError)


def test_default_on_error_logs_warning_once_per_type(caplog):
    inner, t = FakeInterface(), FailingTransport()
    i = MirroredSerialInterface(inner, Mirror(t), 0)
    with caplog.at_level("WARNING"):
        i.command(1)
        i.command(2)
    assert len(inner.calls) == 2
    assert len([r for r in caplog.records if r.levelname == "WARNING"]) == 1
