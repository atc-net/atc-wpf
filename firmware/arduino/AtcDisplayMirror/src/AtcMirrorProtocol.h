// AtcMirrorProtocol.h - encoder for the display mirroring protocol, version 1.
//
// Wire format: docs/Displays/DisplayMirrorProtocol.md in the atc-wpf repository.
// Golden vectors: firmware/protocol/test-vectors.json (checked by test/host/protocol_test.cpp).
//
// Header-only and portable: it includes only <cstdint> and <cstddef>, uses no Arduino types,
// no heap and no STL containers. It is written in the C++11 subset (the ESP32 Arduino core 2.x
// compiles sketches with -std=gnu++11) and is tested with -std=c++17.
//
// Packet: A7 C0 | version 1 | display id | type | flags | u32 payload length (LE) | payload | u16 CRC (LE)
// CRC-16/CCITT-FALSE (poly 0x1021, init 0xFFFF, no reflection, no xor-out) over header bytes 2..9
// followed by the payload. The magic is not covered.
//
// Two ways to build a packet:
//   1. encode(...) writes a whole packet into a caller buffer.
//   2. The streaming form sends a large payload without copying it:
//        uint8_t header[kHeaderSize];
//        encodeHeader(displayId, type, payloadLength, header);
//        uint16_t crc = headerCrc(header);            // = crcUpdate(kCrcInit, header + 2, 8)
//        write(header, kHeaderSize);
//        for each payload chunk: crc = crcUpdate(crc, chunk, n); write(chunk, n);
//        uint8_t tail[kCrcSize]; encodeCrc(crc, tail); write(tail, kCrcSize);
#ifndef ATC_MIRROR_PROTOCOL_H
#define ATC_MIRROR_PROTOCOL_H

#include <cstddef>
#include <cstdint>

namespace atc_mirror {

/// First magic byte.
static const uint8_t kMagic0 = 0xA7;
/// Second magic byte.
static const uint8_t kMagic1 = 0xC0;
/// Protocol version written in every header.
static const uint8_t kVersion = 1;
/// Header size in bytes (magic, version, display id, type, flags, u32 length).
static const size_t kHeaderSize = 10;
/// CRC size in bytes.
static const size_t kCrcSize = 2;
/// Bytes a packet adds to its payload (header + CRC).
static const size_t kPacketOverhead = kHeaderSize + kCrcSize;
/// Initial CRC value.
static const uint16_t kCrcInit = 0xFFFF;
/// Default payload cap of Describe, Commands, Data, I2cWrite, Reset and Latch.
static const uint32_t kMaxSmallPayload = 65536UL;
/// Default payload cap of MonoFrame, ColorFrame and EPaperFrame.
static const uint32_t kMaxFramePayload = 4194304UL;

/// Message types (byte 4 of the header). Unscoped inside a struct, so the values convert to uint8_t.
struct MessageType {
  enum Value : uint8_t {
    Describe = 0x01,
    MonoFrame = 0x10,
    ColorFrame = 0x11,
    EPaperFrame = 0x12,
    Commands = 0x20,
    Data = 0x21,
    I2cWrite = 0x22,
    Reset = 0x23,
    Latch = 0x24
  };
};

/// Display kinds (first byte of a Describe payload).
struct DisplayKind {
  enum Value : uint8_t {
    CharacterLcd = 1,
    MonoOled = 2,
    ColorPanel = 3,
    EPaper = 4,
    LedMatrix = 5,
    SevenSegment = 6,
    GraphicLcd = 7
  };
};

/// Chips (second byte of a Describe payload). Advisory; receivers accept unknown values.
struct Chip {
  enum Value : uint8_t {
    Unknown = 0x00,
    Hd44780 = 0x01,
    Ssd1306 = 0x10,
    Ssd1309 = 0x11,
    Sh1106 = 0x12,
    St7735 = 0x20,
    St7789 = 0x21,
    Ili9341 = 0x22,
    Ssd1331 = 0x23,
    Ssd1351 = 0x24,
    Ssd1680 = 0x30,
    Max7219 = 0x40,
    St7920 = 0x50
  };
};

/// EPaperFrame refresh kind (last payload byte).
struct RefreshKind {
  enum Value : uint8_t {
    Full = 0,
    Partial = 1
  };
};

/// True for the nine message types of version 1.
inline bool isKnownType(uint8_t type) {
  return type == MessageType::Describe || type == MessageType::MonoFrame || type == MessageType::ColorFrame ||
         type == MessageType::EPaperFrame || type == MessageType::Commands || type == MessageType::Data ||
         type == MessageType::I2cWrite || type == MessageType::Reset || type == MessageType::Latch;
}

/// True for MonoFrame, ColorFrame and EPaperFrame.
inline bool isFrameType(uint8_t type) {
  return type == MessageType::MonoFrame || type == MessageType::ColorFrame || type == MessageType::EPaperFrame;
}

/// Default payload cap of a message type (a receiver treats a larger header as noise).
inline uint32_t maxPayload(uint8_t type) {
  return isFrameType(type) ? kMaxFramePayload : kMaxSmallPayload;
}

/// Continues a CRC-16/CCITT-FALSE over `length` bytes. Start with kCrcInit.
inline uint16_t crcUpdate(uint16_t crc, const uint8_t* data, size_t length) {
  for (size_t i = 0; i < length; ++i) {
    crc = static_cast<uint16_t>(crc ^ (static_cast<uint16_t>(data[i]) << 8));
    for (int bit = 0; bit < 8; ++bit) {
      crc = (crc & 0x8000U) != 0 ? static_cast<uint16_t>((crc << 1) ^ 0x1021U) : static_cast<uint16_t>(crc << 1);
    }
  }
  return crc;
}

/// CRC-16/CCITT-FALSE of a whole buffer. crc16("123456789") == 0x29B1.
inline uint16_t crc16(const uint8_t* data, size_t length) {
  return crcUpdate(kCrcInit, data, length);
}

/// Writes a little-endian u16.
inline void writeU16le(uint16_t value, uint8_t* out) {
  out[0] = static_cast<uint8_t>(value & 0xFF);
  out[1] = static_cast<uint8_t>(value >> 8);
}

/// Writes a little-endian u32.
inline void writeU32le(uint32_t value, uint8_t* out) {
  out[0] = static_cast<uint8_t>(value & 0xFF);
  out[1] = static_cast<uint8_t>((value >> 8) & 0xFF);
  out[2] = static_cast<uint8_t>((value >> 16) & 0xFF);
  out[3] = static_cast<uint8_t>((value >> 24) & 0xFF);
}

/// Writes the 10-byte header into `out` (which must hold kHeaderSize bytes).
inline void encodeHeader(uint8_t displayId, uint8_t type, uint32_t payloadLength, uint8_t* out, uint8_t flags = 0) {
  out[0] = kMagic0;
  out[1] = kMagic1;
  out[2] = kVersion;
  out[3] = displayId;
  out[4] = type;
  out[5] = flags;
  writeU32le(payloadLength, out + 6);
}

/// CRC over header bytes 2..9; continue it with crcUpdate over the payload.
inline uint16_t headerCrc(const uint8_t* header) {
  return crcUpdate(kCrcInit, header + 2, kHeaderSize - 2);
}

/// Writes the final CRC little-endian into `out` (kCrcSize bytes).
inline void encodeCrc(uint16_t crc, uint8_t* out) {
  writeU16le(crc, out);
}

/// Size of a whole packet for a payload of `payloadLength` bytes.
inline size_t packetSize(size_t payloadLength) {
  return payloadLength + kPacketOverhead;
}

/// Encodes one packet into `out`. Returns the packet size, or 0 when it does not fit in `outCapacity`,
/// when the payload is over the cap of its type, or when `payload` is null with a non-zero length.
inline size_t encode(uint8_t displayId, uint8_t type, const uint8_t* payload, size_t length, uint8_t* out,
                     size_t outCapacity, uint8_t flags = 0) {
  if (out == 0 || (payload == 0 && length != 0) || length > maxPayload(type)) {
    return 0;
  }
  if (outCapacity < kPacketOverhead || length > outCapacity - kPacketOverhead) {
    return 0;
  }
  encodeHeader(displayId, type, static_cast<uint32_t>(length), out, flags);
  uint16_t crc = headerCrc(out);
  for (size_t i = 0; i < length; ++i) {
    out[kHeaderSize + i] = payload[i];
  }
  crc = crcUpdate(crc, payload, length);
  encodeCrc(crc, out + kHeaderSize + length);
  return length + kPacketOverhead;
}

/// Bytes of the pages of a MonoFrame: width x ceil(height / 8). Computed in 64 bits (no overflow).
inline uint64_t monoFrameBytes(uint16_t width, uint16_t height) {
  return static_cast<uint64_t>(width) * ((static_cast<uint64_t>(height) + 7U) / 8U);
}

/// Bytes of the pixels of a ColorFrame: width x height x 2.
inline uint64_t colorFrameBytes(uint16_t width, uint16_t height) {
  return static_cast<uint64_t>(width) * static_cast<uint64_t>(height) * 2U;
}

/// Bytes of one EPaperFrame plane: ceil(width / 8) x height.
inline uint64_t epaperPlaneBytes(uint16_t width, uint16_t height) {
  return ((static_cast<uint64_t>(width) + 7U) / 8U) * static_cast<uint64_t>(height);
}

/// Writes the u16 width and u16 height that start every frame payload (4 bytes).
inline void encodeFrameSize(uint16_t width, uint16_t height, uint8_t* out) {
  writeU16le(width, out);
  writeU16le(height, out + 2);
}

/// Converts RGB565 pixels to the big-endian wire order (2 bytes per pixel into `out`).
/// pixelsAreBigEndian = true: the bytes in memory are already high byte first (TFT_eSPI sprites store
///   byte-swapped values), so the memory bytes are copied unchanged.
/// pixelsAreBigEndian = false: the values are ordinary uint16_t (Adafruit GFXcanvas16, LVGL without swap),
///   so each value is written high byte, then low byte; this is right on any host byte order.
inline void rgb565ToWire(const uint16_t* pixels, size_t count, bool pixelsAreBigEndian, uint8_t* out) {
  if (pixelsAreBigEndian) {
    const uint8_t* bytes = reinterpret_cast<const uint8_t*>(pixels);
    for (size_t i = 0; i < count * 2; ++i) {
      out[i] = bytes[i];
    }
    return;
  }
  for (size_t i = 0; i < count; ++i) {
    out[2 * i] = static_cast<uint8_t>(pixels[i] >> 8);
    out[2 * i + 1] = static_cast<uint8_t>(pixels[i] & 0xFF);
  }
}

}  // namespace atc_mirror

#endif  // ATC_MIRROR_PROTOCOL_H
