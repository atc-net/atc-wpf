// AtcDisplayMirror.h - sends what an ESP32 shows on a display to the Atc.Wpf display controls.
//
// Wire format: docs/Displays/DisplayMirrorProtocol.md (protocol version 1). The encoder is AtcMirrorProtocol.h.
//
// Sinks (pick one with begin()):
//   - Stream&  : Serial, WiFiClient (TCP) or any Stream. Packets are streamed; nothing is buffered.
//   - UDP&     : WiFiUDP + IPAddress + port. One packet per datagram. The ESP32 core's WiFiUDP sends a
//                datagram by itself every 1460 bytes, so a packet bigger than that would arrive split and
//                be rejected; such packets are refused (false). Use TCP or serial for colour frames.
//   - callback : gets each whole packet (e.g. MQTT publish), assembled in a caller buffer or in a heap buffer
//                that grows on demand (default limit 65,548 bytes). A callback returning bool reports delivery
//                (send* returns its result); a void callback cannot.
//
// Every send* returns true when the whole packet was handed to the sink. It is not thread safe: call it from
// one task, or guard it with your own mutex.
#ifndef ATC_DISPLAY_MIRROR_H
#define ATC_DISPLAY_MIRROR_H

#include <Arduino.h>
#include <IPAddress.h>
#include <Stream.h>
#include <Udp.h>

#include <functional>
#include <type_traits>
#include <utility>

#include "AtcMirrorProtocol.h"

namespace atc_mirror_detail {

// True when F can be called as f(const uint8_t*, size_t) and returns exactly bool.
template <typename F>
struct ReturnsBool {
  template <typename G>
  static auto test(int)
      -> decltype(std::declval<G&>()(static_cast<const uint8_t*>(nullptr), static_cast<size_t>(0)));
  template <typename G>
  static void test(...);
  static const bool value = std::is_same<decltype(test<F>(0)), bool>::value;
};

}  // namespace atc_mirror_detail

class AtcDisplayMirror {
 public:
  /// Receives one whole packet (callback sink). send* cannot see whether it was delivered.
  typedef std::function<void(const uint8_t*, size_t)> PacketCallback;
  /// Receives one whole packet and returns whether it was delivered (e.g. PubSubClient::publish); false makes
  /// the send* call return false.
  typedef std::function<bool(const uint8_t*, size_t)> CheckedPacketCallback;

  /// WiFiUDP (NetworkUDP) on the ESP32 core buffers 1460 bytes and sends a datagram by itself when the
  /// buffer is full, so this is the largest packet that leaves as one datagram.
  static const size_t kEsp32UdpMaxPacket = 1460;
  /// Largest UDP payload over IPv4.
  static const size_t kUdpMaxDatagram = 65507;
  /// Default limit of the growing heap buffer of the callback sink: the largest Commands/Data/I2cWrite/Latch/
  /// Describe packet (65,536-byte payload + 12). Frames up to that size fit too; larger frames need a larger
  /// heapLimit in begin().
  static const size_t kDefaultCallbackHeapLimit = atc_mirror::kMaxSmallPayload + atc_mirror::kPacketOverhead;

  AtcDisplayMirror();
  ~AtcDisplayMirror();

  /// Streams packets to a Stream (Serial, WiFiClient, ...). The stream must outlive this object.
  void begin(Stream& stream);

  /// Sends each packet as one UDP datagram to ip:port. Packets over maxPacket bytes are refused.
  /// Keep the default (1460) for the ESP32 WiFiUDP; raise it only for a UDP class that sends larger datagrams.
  void begin(UDP& udp, const IPAddress& ip, uint16_t port, size_t maxPacket = kEsp32UdpMaxPacket);

  /// Hands each whole packet to `callback`, assembled in a heap buffer of at most heapLimit bytes.
  void begin(PacketCallback callback, size_t heapLimit = kDefaultCallbackHeapLimit);

  /// Hands each whole packet to `callback`, assembled in the caller's `buffer` of `capacity` bytes.
  /// Larger packets are refused.
  void begin(PacketCallback callback, uint8_t* buffer, size_t capacity);

  /// As begin(PacketCallback, size_t), but send* returns the callback's result.
  void begin(CheckedPacketCallback callback, size_t heapLimit = kDefaultCallbackHeapLimit);

  /// As begin(PacketCallback, uint8_t*, size_t), but send* returns the callback's result.
  void begin(CheckedPacketCallback callback, uint8_t* buffer, size_t capacity);

  /// Any callable returning bool (a lambda, a function): picks the CheckedPacketCallback overloads. Without this,
  /// a bool lambda would match both std::function overloads and be ambiguous.
  template <typename F>
  typename std::enable_if<atc_mirror_detail::ReturnsBool<F>::value>::type begin(
      F callback, size_t heapLimit = kDefaultCallbackHeapLimit) {
    begin(CheckedPacketCallback(callback), heapLimit);
  }

  /// Any callable returning bool, with a caller buffer.
  template <typename F>
  typename std::enable_if<atc_mirror_detail::ReturnsBool<F>::value>::type begin(F callback, uint8_t* buffer,
                                                                               size_t capacity) {
    begin(CheckedPacketCallback(callback), buffer, capacity);
  }

  /// Detaches the sink and frees the heap buffer. send* returns false until the next begin().
  void end();

  /// Flags byte written in every header (version 1 defines none; receivers ignore unknown bits).
  void setFlags(uint8_t flags) { _flags = flags; }

  /// Describe (advisory): kind (atc_mirror::DisplayKind), chip (atc_mirror::Chip), size and a UTF-8 name.
  bool describe(uint8_t displayId, uint8_t kind, uint8_t chip, uint16_t width, uint16_t height,
                const char* name = nullptr);

  /// MonoFrame: `pages` holds width x ceil(height / 8) bytes, one page per 8 rows, bit 0 = top row of the page
  /// (the Adafruit_SSD1306 getBuffer() and U8g2 getBufferPtr() layout).
  bool sendMonoFrame(uint8_t displayId, const uint8_t* pages, uint16_t width, uint16_t height);

  /// ColorFrame: width x height RGB565 pixels, rows top to bottom. They are sent big-endian, converted on the
  /// fly in small chunks on the stack (no heap).
  /// pixelsAreBigEndian = true when the bytes in memory are already high byte first (TFT_eSprite::getPointer());
  /// false for ordinary uint16_t values (GFXcanvas16::getBuffer()).
  bool sendColorFrame(uint8_t displayId, const uint16_t* pixels, uint16_t width, uint16_t height,
                      bool pixelsAreBigEndian);

  /// EPaperFrame in the panel's native (portrait) orientation: each plane ceil(width / 8) x height bytes,
  /// MSB = leftmost pixel. Black plane: 0 = black, 1 = white (SSD1680 RAM 0x24). Red plane (or nullptr):
  /// 1 = red, 0 = no red (SSD1680 RAM 0x26). A GxEPD2_3C colour buffer is 0 = red: invert it before sending;
  /// Adafruit_EPD buffers go out unchanged. width is the buffer's: 128 for the 2.13" (122 visible) and 2.9" panels.
  /// partial = partial refresh.
  bool sendEPaperFrame(uint8_t displayId, const uint8_t* blackPlane, const uint8_t* redPlane, uint16_t width,
                       uint16_t height, bool partial = false);

  /// Commands: bytes the chip receives with D/C low.
  bool sendCommands(uint8_t displayId, const uint8_t* bytes, size_t length);

  /// Data: bytes the chip receives with D/C high (or the raw serial stream of a chip with an in-band protocol,
  /// such as ST7920 SPI).
  bool sendData(uint8_t displayId, const uint8_t* bytes, size_t length);

  /// I2cWrite: the 7-bit address, then the bytes of one write transaction (control byte included).
  bool sendI2cWrite(uint8_t displayId, uint8_t address7, const uint8_t* bytes, size_t length);

  /// Reset: the chip was reset (empty payload).
  bool sendReset(uint8_t displayId);

  /// Latch: the bytes of one chip-select frame (one MAX7219 latch: 2 bytes per chained device, farthest first).
  bool sendLatch(uint8_t displayId, const uint8_t* bytes, size_t length);

  /// Any message type with a ready payload.
  bool sendRaw(uint8_t displayId, uint8_t type, const uint8_t* payload, size_t length);

  /// Number of send* calls that returned false since begin().
  uint32_t failedPackets() const { return _failed; }

 private:
  enum SinkKind { SinkNone, SinkStream, SinkUdp, SinkCallback };

  struct Part {
    const uint8_t* data;
    size_t length;
    const uint16_t* pixels;  // when set, `length` is a pixel count converted with rgb565ToWire
    bool pixelsAreBigEndian;
  };

  AtcDisplayMirror(const AtcDisplayMirror&);
  AtcDisplayMirror& operator=(const AtcDisplayMirror&);

  bool sendParts(uint8_t displayId, uint8_t type, const Part* parts, size_t count);
  bool openPacket(size_t packetSize);
  bool put(const uint8_t* bytes, size_t length);
  bool closePacket();
  void releaseHeap();
  bool fail();

  SinkKind _sink;
  Stream* _stream;
  UDP* _udp;
  IPAddress _ip;
  uint16_t _port;
  size_t _udpMaxPacket;
  PacketCallback _callback;
  CheckedPacketCallback _checkedCallback;
  uint8_t* _buffer;
  size_t _capacity;
  bool _ownsBuffer;
  size_t _heapLimit;
  size_t _used;
  uint8_t _flags;
  uint32_t _failed;
};

#endif  // ATC_DISPLAY_MIRROR_H
