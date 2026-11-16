// AtcDisplayMirror.cpp - see AtcDisplayMirror.h.
#include "AtcDisplayMirror.h"

#include <stdlib.h>
#include <string.h>

using atc_mirror::MessageType;

const size_t AtcDisplayMirror::kEsp32UdpMaxPacket;
const size_t AtcDisplayMirror::kUdpMaxDatagram;
const size_t AtcDisplayMirror::kDefaultCallbackHeapLimit;

namespace {

// Pixels converted per step of sendColorFrame (128 bytes on the stack).
const size_t kPixelChunk = 64;

}  // namespace

AtcDisplayMirror::AtcDisplayMirror()
    : _sink(SinkNone),
      _stream(nullptr),
      _udp(nullptr),
      _ip(),
      _port(0),
      _udpMaxPacket(kEsp32UdpMaxPacket),
      _callback(),
      _checkedCallback(),
      _buffer(nullptr),
      _capacity(0),
      _ownsBuffer(false),
      _heapLimit(0),
      _used(0),
      _flags(0),
      _failed(0) {}

AtcDisplayMirror::~AtcDisplayMirror() {
  releaseHeap();
}

void AtcDisplayMirror::begin(Stream& stream) {
  end();
  _stream = &stream;
  _sink = SinkStream;
}

void AtcDisplayMirror::begin(UDP& udp, const IPAddress& ip, uint16_t port, size_t maxPacket) {
  end();
  _udp = &udp;
  _ip = ip;
  _port = port;
  _udpMaxPacket = maxPacket > kUdpMaxDatagram ? kUdpMaxDatagram : maxPacket;
  _sink = SinkUdp;
}

void AtcDisplayMirror::begin(PacketCallback callback, size_t heapLimit) {
  end();
  _callback = callback;
  _heapLimit = heapLimit;
  _ownsBuffer = true;
  _sink = SinkCallback;
}

void AtcDisplayMirror::begin(PacketCallback callback, uint8_t* buffer, size_t capacity) {
  end();
  _callback = callback;
  _buffer = buffer;
  _capacity = buffer != nullptr ? capacity : 0;
  _ownsBuffer = false;
  _sink = SinkCallback;
}

void AtcDisplayMirror::begin(CheckedPacketCallback callback, size_t heapLimit) {
  begin(PacketCallback(), heapLimit);
  _checkedCallback = callback;
}

void AtcDisplayMirror::begin(CheckedPacketCallback callback, uint8_t* buffer, size_t capacity) {
  begin(PacketCallback(), buffer, capacity);
  _checkedCallback = callback;
}

void AtcDisplayMirror::end() {
  releaseHeap();
  _sink = SinkNone;
  _stream = nullptr;
  _udp = nullptr;
  _callback = PacketCallback();
  _checkedCallback = CheckedPacketCallback();
  _buffer = nullptr;
  _capacity = 0;
  _ownsBuffer = false;
  _failed = 0;
}

void AtcDisplayMirror::releaseHeap() {
  if (_ownsBuffer && _buffer != nullptr) {
    free(_buffer);
  }
  if (_ownsBuffer) {
    _buffer = nullptr;
    _capacity = 0;
  }
}

bool AtcDisplayMirror::fail() {
  ++_failed;
  return false;
}

bool AtcDisplayMirror::describe(uint8_t displayId, uint8_t kind, uint8_t chip, uint16_t width, uint16_t height,
                                const char* name) {
  uint8_t head[6];
  head[0] = kind;
  head[1] = chip;
  atc_mirror::encodeFrameSize(width, height, head + 2);
  size_t nameLength = name != nullptr ? strlen(name) : 0;
  Part parts[2] = {{head, sizeof(head), nullptr, false},
                   {reinterpret_cast<const uint8_t*>(name), nameLength, nullptr, false}};
  return sendParts(displayId, MessageType::Describe, parts, 2);
}

bool AtcDisplayMirror::sendMonoFrame(uint8_t displayId, const uint8_t* pages, uint16_t width, uint16_t height) {
  uint64_t bytes = atc_mirror::monoFrameBytes(width, height);
  if (pages == nullptr || width == 0 || height == 0 || bytes + 4 > atc_mirror::kMaxFramePayload) {
    return fail();
  }
  uint8_t size[4];
  atc_mirror::encodeFrameSize(width, height, size);
  Part parts[2] = {{size, sizeof(size), nullptr, false}, {pages, static_cast<size_t>(bytes), nullptr, false}};
  return sendParts(displayId, MessageType::MonoFrame, parts, 2);
}

bool AtcDisplayMirror::sendColorFrame(uint8_t displayId, const uint16_t* pixels, uint16_t width, uint16_t height,
                                      bool pixelsAreBigEndian) {
  uint64_t bytes = atc_mirror::colorFrameBytes(width, height);
  if (pixels == nullptr || width == 0 || height == 0 || bytes + 4 > atc_mirror::kMaxFramePayload) {
    return fail();
  }
  uint8_t size[4];
  atc_mirror::encodeFrameSize(width, height, size);
  Part parts[2] = {{size, sizeof(size), nullptr, false},
                   {nullptr, static_cast<size_t>(bytes / 2), pixels, pixelsAreBigEndian}};
  return sendParts(displayId, MessageType::ColorFrame, parts, 2);
}

bool AtcDisplayMirror::sendEPaperFrame(uint8_t displayId, const uint8_t* blackPlane, const uint8_t* redPlane,
                                       uint16_t width, uint16_t height, bool partial) {
  uint64_t plane = atc_mirror::epaperPlaneBytes(width, height);
  uint64_t planes = redPlane != nullptr ? 2 : 1;
  if (blackPlane == nullptr || width == 0 || height == 0 || 6 + planes * plane > atc_mirror::kMaxFramePayload) {
    return fail();
  }
  uint8_t head[5];
  atc_mirror::encodeFrameSize(width, height, head);
  head[4] = static_cast<uint8_t>(planes);
  uint8_t refresh = partial ? atc_mirror::RefreshKind::Partial : atc_mirror::RefreshKind::Full;
  Part parts[4] = {{head, sizeof(head), nullptr, false},
                   {blackPlane, static_cast<size_t>(plane), nullptr, false},
                   {redPlane, redPlane != nullptr ? static_cast<size_t>(plane) : 0, nullptr, false},
                   {&refresh, 1, nullptr, false}};
  return sendParts(displayId, MessageType::EPaperFrame, parts, 4);
}

bool AtcDisplayMirror::sendCommands(uint8_t displayId, const uint8_t* bytes, size_t length) {
  return sendRaw(displayId, MessageType::Commands, bytes, length);
}

bool AtcDisplayMirror::sendData(uint8_t displayId, const uint8_t* bytes, size_t length) {
  return sendRaw(displayId, MessageType::Data, bytes, length);
}

bool AtcDisplayMirror::sendI2cWrite(uint8_t displayId, uint8_t address7, const uint8_t* bytes, size_t length) {
  if (bytes == nullptr && length != 0) {
    return fail();
  }
  uint8_t address = static_cast<uint8_t>(address7 & 0x7F);
  Part parts[2] = {{&address, 1, nullptr, false}, {bytes, length, nullptr, false}};
  return sendParts(displayId, MessageType::I2cWrite, parts, 2);
}

bool AtcDisplayMirror::sendReset(uint8_t displayId) {
  return sendRaw(displayId, MessageType::Reset, nullptr, 0);
}

bool AtcDisplayMirror::sendLatch(uint8_t displayId, const uint8_t* bytes, size_t length) {
  return sendRaw(displayId, MessageType::Latch, bytes, length);
}

bool AtcDisplayMirror::sendRaw(uint8_t displayId, uint8_t type, const uint8_t* payload, size_t length) {
  if (payload == nullptr && length != 0) {
    return fail();
  }
  Part part = {payload, length, nullptr, false};
  return sendParts(displayId, type, &part, 1);
}

bool AtcDisplayMirror::sendParts(uint8_t displayId, uint8_t type, const Part* parts, size_t count) {
  if (_sink == SinkNone) {
    return fail();
  }
  uint64_t payloadLength = 0;
  for (size_t i = 0; i < count; ++i) {
    payloadLength += parts[i].pixels != nullptr ? 2ULL * parts[i].length : parts[i].length;
  }
  if (payloadLength > atc_mirror::maxPayload(type)) {
    return fail();
  }
  size_t length = static_cast<size_t>(payloadLength);
  if (!openPacket(atc_mirror::packetSize(length))) {
    return fail();
  }

  uint8_t header[atc_mirror::kHeaderSize];
  atc_mirror::encodeHeader(displayId, type, static_cast<uint32_t>(length), header, _flags);
  uint16_t crc = atc_mirror::headerCrc(header);
  bool ok = put(header, sizeof(header));

  for (size_t i = 0; ok && i < count; ++i) {
    const Part& part = parts[i];
    if (part.pixels == nullptr) {
      if (part.length == 0) {
        continue;
      }
      crc = atc_mirror::crcUpdate(crc, part.data, part.length);
      ok = put(part.data, part.length);
      continue;
    }
    uint8_t chunk[2 * kPixelChunk];
    for (size_t done = 0; ok && done < part.length; done += kPixelChunk) {
      size_t n = part.length - done < kPixelChunk ? part.length - done : kPixelChunk;
      atc_mirror::rgb565ToWire(part.pixels + done, n, part.pixelsAreBigEndian, chunk);
      crc = atc_mirror::crcUpdate(crc, chunk, 2 * n);
      ok = put(chunk, 2 * n);
    }
  }

  if (ok) {
    uint8_t tail[atc_mirror::kCrcSize];
    atc_mirror::encodeCrc(crc, tail);
    ok = put(tail, sizeof(tail));
  }
  // A failed packet is not closed: no UDP datagram and no callback for a partial packet. A Stream may already
  // carry part of it; the receiver drops it after its partial-packet timeout or on the next magic.
  if (!ok || !closePacket()) {
    return fail();
  }
  return true;
}

bool AtcDisplayMirror::openPacket(size_t packetSize) {
  switch (_sink) {
    case SinkStream:
      return _stream != nullptr;
    case SinkUdp:
      if (_udp == nullptr || packetSize > _udpMaxPacket) {
        return false;
      }
      return _udp->beginPacket(_ip, _port) == 1;
    case SinkCallback:
      if (!_callback && !_checkedCallback) {
        return false;
      }
      if (packetSize > _capacity) {
        if (!_ownsBuffer || packetSize > _heapLimit) {
          return false;
        }
        free(_buffer);
        _buffer = static_cast<uint8_t*>(malloc(packetSize));
        _capacity = _buffer != nullptr ? packetSize : 0;
        if (_buffer == nullptr) {
          return false;
        }
      }
      _used = 0;
      return true;
    default:
      return false;
  }
}

bool AtcDisplayMirror::put(const uint8_t* bytes, size_t length) {
  switch (_sink) {
    case SinkStream:
      return _stream->write(bytes, length) == length;
    case SinkUdp:
      return _udp->write(bytes, length) == length;
    case SinkCallback:
      if (length > _capacity - _used) {
        return false;
      }
      memcpy(_buffer + _used, bytes, length);
      _used += length;
      return true;
    default:
      return false;
  }
}

bool AtcDisplayMirror::closePacket() {
  switch (_sink) {
    case SinkStream:
      return true;
    case SinkUdp:
      return _udp->endPacket() == 1;
    case SinkCallback:
      if (_checkedCallback) {
        return _checkedCallback(_buffer, _used);
      }
      _callback(_buffer, _used);
      return true;
    default:
      return false;
  }
}
