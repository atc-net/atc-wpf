class Recorder:
    """A transport that records the packets it was given."""

    def __init__(self):
        self.packets = []

    def send(self, packet):
        self.packets.append(packet)

    def close(self):
        pass


class FailingTransport:
    """A transport whose send always raises."""

    def __init__(self, exc=None):
        self.exc = exc or OSError("link down")
        self.attempts = 0

    def send(self, packet):
        self.attempts += 1
        raise self.exc

    def close(self):
        pass
