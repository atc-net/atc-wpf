// Host test of the sinks and the U8g2 tee: AtcDisplayMirror.cpp and AtcU8g2Tee.h built on a PC against the stub
// Arduino and U8x8 headers in stub/. Packets are checked against the golden vectors (vectors.h). See README.md.
#include <cstdio>
#include <vector>

#include "AtcDisplayMirror.h"
#include "AtcU8g2Tee.h"
#include "vectors.h"

static int failures = 0;
static void check(bool ok, const char* what) {
  if (!ok) {
    ++failures;
    std::printf("FAIL %s\n", what);
  }
}

// Fakes for the U8g2 procedures the tee references: the byte procedures log every call.
static std::vector<int> busLog;
extern "C" uint8_t fakeBus(u8x8_t*, uint8_t msg, uint8_t arg_int, void* arg_ptr) {
  busLog.push_back(msg);
  if (msg == U8X8_MSG_BYTE_SEND) {
    for (int i = 0; i < arg_int; ++i) busLog.push_back(1000 + static_cast<const uint8_t*>(arg_ptr)[i]);
  }
  return 1;
}
extern "C" uint8_t u8x8_byte_arduino_hw_i2c(u8x8_t* a, uint8_t b, uint8_t c, void* d) { return fakeBus(a, b, c, d); }
extern "C" uint8_t u8x8_byte_arduino_2nd_hw_i2c(u8x8_t* a, uint8_t b, uint8_t c, void* d) { return fakeBus(a, b, c, d); }
extern "C" uint8_t u8x8_byte_arduino_sw_i2c(u8x8_t* a, uint8_t b, uint8_t c, void* d) { return fakeBus(a, b, c, d); }
extern "C" uint8_t u8x8_byte_arduino_hw_spi(u8x8_t* a, uint8_t b, uint8_t c, void* d) { return fakeBus(a, b, c, d); }
extern "C" uint8_t u8x8_byte_sw_i2c(u8x8_t* a, uint8_t b, uint8_t c, void* d) { return fakeBus(a, b, c, d); }
extern "C" uint8_t u8x8_cad_st7920_spi(u8x8_t*, uint8_t, uint8_t, void*) { return 1; }
extern "C" uint8_t u8x8_cad_001(u8x8_t*, uint8_t, uint8_t, void*) { return 1; }
extern "C" uint8_t u8x8_d_max7219_64x8(u8x8_t*, uint8_t, uint8_t, void*) { return 1; }
extern "C" uint8_t u8x8_d_max7219_32x8(u8x8_t*, uint8_t, uint8_t, void*) { return 1; }
extern "C" uint8_t u8x8_d_max7219_16x16(u8x8_t*, uint8_t, uint8_t, void*) { return 1; }
extern "C" uint8_t u8x8_d_max7219_8x8(u8x8_t*, uint8_t, uint8_t, void*) { return 1; }
extern "C" uint8_t u8x8_d_ssd1306(u8x8_t*, uint8_t, uint8_t, void*) { return 1; }

// The calls U8g2's u8x8_byte_* helpers make (u8x8_byte.c).
static void sendByte(u8x8_t* u, uint8_t b) { u->byte_cb(u, U8X8_MSG_BYTE_SEND, 1, &b); }
static void sendBytes(u8x8_t* u, uint8_t n, const uint8_t* p) { u->byte_cb(u, U8X8_MSG_BYTE_SEND, n, (void*)p); }
static void setDc(u8x8_t* u, uint8_t dc) { u->byte_cb(u, U8X8_MSG_BYTE_SET_DC, dc, nullptr); }
static void start(u8x8_t* u) { u->byte_cb(u, U8X8_MSG_BYTE_START_TRANSFER, 0, nullptr); }
static void stop(u8x8_t* u) { u->byte_cb(u, U8X8_MSG_BYTE_END_TRANSFER, 0, nullptr); }

class FakeStream : public Stream {
 public:
  std::vector<uint8_t> bytes;
  size_t write(uint8_t b) override { bytes.push_back(b); return 1; }
  int available() override { return 0; }
};

class FakeUdp : public UDP {
 public:
  std::vector<std::vector<uint8_t> > datagrams;
  std::vector<uint8_t> current;
  int beginPacket(IPAddress, uint16_t) override { current.clear(); return 1; }
  int endPacket() override { datagrams.push_back(current); return 1; }
  size_t write(uint8_t b) override { current.push_back(b); return 1; }
  size_t write(const uint8_t* b, size_t n) override { current.insert(current.end(), b, b + n); return n; }
  int available() override { return 0; }
};

static bool checkedFn(const uint8_t*, size_t) { return false; }
static std::vector<std::vector<uint8_t> > packets;
static void collect(const uint8_t* p, size_t n) { packets.push_back(std::vector<uint8_t>(p, p + n)); }

static bool samePacket(const std::vector<uint8_t>& got, uint8_t id, uint8_t type, const uint8_t* payload, size_t n) {
  uint8_t expected[2048];
  size_t len = atc_mirror::encode(id, type, payload, n, expected, sizeof(expected));
  return len == got.size() && std::memcmp(expected, got.data(), len) == 0;
}

static const GoldenVector& vec(const char* name) {
  for (size_t i = 0; i < kVectorCount; ++i) if (std::strcmp(kVectors[i].name, name) == 0) return kVectors[i];
  return kVectors[0];
}

static bool streamEquals(FakeStream& s, const GoldenVector& v) {
  bool ok = s.bytes.size() == v.packetLength && std::memcmp(s.bytes.data(), v.packet, v.packetLength) == 0;
  s.bytes.clear();
  return ok;
}

int main() {
  AtcDisplayMirror mirror;
  FakeStream stream;
  mirror.begin(stream);

  check(mirror.describe(0, atc_mirror::DisplayKind::MonoOled, atc_mirror::Chip::Ssd1306, 128, 64, "OLED") &&
            streamEquals(stream, vec("describe-oled-128x64-ssd1306")), "describe");
  check(mirror.describe(3, 7, 0x50, 128, 64, "\xC3\x86\xC3\x98\xC3\x85") &&
            streamEquals(stream, vec("describe-non-ascii-name")), "describe non-ascii");
  const GoldenVector& mono = vec("mono-frame-8x8");
  check(mirror.sendMonoFrame(0, mono.payload + 4, 8, 8) && streamEquals(stream, mono), "mono");
  const uint16_t px[4] = {0xF800, 0x07E0, 0x001F, 0xFFFF};
  check(mirror.sendColorFrame(1, px, 2, 2, false) && streamEquals(stream, vec("color-frame-2x2")), "color native");
  uint16_t pxBe[4];
  std::memcpy(pxBe, vec("color-frame-2x2").payload + 4, 8);
  check(mirror.sendColorFrame(1, pxBe, 2, 2, true) && streamEquals(stream, vec("color-frame-2x2")), "color BE");
  const GoldenVector& ep2 = vec("epaper-frame-8x2-two-planes-partial");
  check(mirror.sendEPaperFrame(2, ep2.payload + 5, ep2.payload + 7, 8, 2, true) && streamEquals(stream, ep2), "epaper 2");
  const GoldenVector& ep1 = vec("epaper-frame-8x2-one-plane-full");
  check(mirror.sendEPaperFrame(2, ep1.payload + 5, nullptr, 8, 2, false) && streamEquals(stream, ep1), "epaper 1");
  const GoldenVector& cmd = vec("commands");
  check(mirror.sendCommands(0, cmd.payload, cmd.payloadLength) && streamEquals(stream, cmd), "commands");
  const GoldenVector& dat = vec("data");
  check(mirror.sendData(0, dat.payload, dat.payloadLength) && streamEquals(stream, dat), "data");
  const GoldenVector& i2c = vec("i2c-write-0x3c");
  check(mirror.sendI2cWrite(0, 0x3C, i2c.payload + 1, i2c.payloadLength - 1) && streamEquals(stream, i2c), "i2c");
  check(mirror.sendReset(0) && streamEquals(stream, vec("reset-empty")), "reset");
  const GoldenVector& latch = vec("latch-4-bytes");
  check(mirror.sendLatch(4, latch.payload, latch.payloadLength) && streamEquals(stream, latch), "latch");
  mirror.setFlags(0x80);
  check(mirror.sendCommands(255, vec("commands-display-255-flags-0x80").payload, 1) &&
            streamEquals(stream, vec("commands-display-255-flags-0x80")), "flags");
  mirror.setFlags(0);
  // Colour frame bigger than one 64-pixel chunk: 10 x 10 native values.
  {
    uint16_t big[100];
    uint8_t payload[4 + 200];
    atc_mirror::encodeFrameSize(10, 10, payload);
    for (int i = 0; i < 100; ++i) {
      big[i] = static_cast<uint16_t>(i * 653);
      payload[4 + 2 * i] = static_cast<uint8_t>(big[i] >> 8);
      payload[5 + 2 * i] = static_cast<uint8_t>(big[i] & 0xFF);
    }
    check(mirror.sendColorFrame(9, big, 10, 10, false), "color 10x10 sent");
    std::vector<uint8_t> got = stream.bytes;
    stream.bytes.clear();
    check(samePacket(got, 9, atc_mirror::MessageType::ColorFrame, payload, sizeof(payload)), "color 10x10 bytes");
  }
  check(!mirror.sendMonoFrame(0, nullptr, 8, 8) && !mirror.sendMonoFrame(0, mono.payload, 0, 8), "bad args refused");
  check(stream.bytes.empty(), "nothing written for refused frames");

  // UDP: one datagram per packet, more than 1460 bytes refused.
  FakeUdp udp;
  mirror.begin(udp, IPAddress(192, 168, 1, 2), 5556);
  static uint8_t zeros[2000];
  check(mirror.sendData(0, zeros, 1448), "1460-byte packet fits");
  check(!mirror.sendData(0, zeros, 1449), "1461-byte packet refused");
  check(udp.datagrams.size() == 1 && udp.datagrams[0].size() == 1460, "one datagram of 1460 bytes");

  // Callback sink with heap buffer and with a caller buffer.
  mirror.begin(collect);
  check(mirror.sendLatch(4, latch.payload, latch.payloadLength), "callback latch");
  check(packets.size() == 1 && samePacket(packets[0], 4, 0x24, latch.payload, 4), "callback bytes");
  packets.clear();
  uint8_t small[20];
  mirror.begin(collect, small, sizeof(small));
  check(mirror.sendLatch(4, latch.payload, 4) && packets.size() == 1, "caller buffer fits 16 bytes");
  check(!mirror.sendData(0, zeros, 9) && packets.size() == 1, "caller buffer refuses 21 bytes");
  check(mirror.failedPackets() == 1, "failedPackets counts");
  packets.clear();

  // The tee: I2C, D/C SPI, ST7920 serial and MAX7219 latch buses.
  mirror.begin(collect);
  u8x8_t oled = {};
  oled.byte_cb = u8x8_byte_arduino_hw_i2c;
  oled.cad_cb = u8x8_cad_001;
  oled.display_cb = u8x8_d_ssd1306;
  oled.i2c_address = 255;
  check(atcMirrorTee(&oled, mirror, 0), "tee installed");
  check(oled.byte_cb == atcMirrorTeeByteCb, "byte_cb replaced");
  check(atcMirrorTee(&oled, mirror, 0), "tee installed twice");
  oled.i2c_address = 0x78;  // set by CAD INIT
  start(&oled); sendByte(&oled, 0x00); sendByte(&oled, 0x8D); sendByte(&oled, 0x14); stop(&oled);
  const uint8_t i2cExpected[4] = {0x3C, 0x00, 0x8D, 0x14};
  check(packets.size() == 1 && samePacket(packets[0], 0, 0x22, i2cExpected, 4), "I2C transaction -> I2cWrite 3C 00 8D 14");
  check(busLog.size() == 1 + 3 * 2 + 1 && busLog[1] == U8X8_MSG_BYTE_SEND && busLog[2] == 1000, "inner got every call");
  packets.clear();

  u8x8_t spi = {};
  spi.byte_cb = u8x8_byte_arduino_hw_spi;
  spi.cad_cb = u8x8_cad_001;
  spi.display_cb = u8x8_d_ssd1306;
  check(atcMirrorTee(&spi, mirror, 1), "spi tee");
  start(&spi);
  setDc(&spi, 0); sendByte(&spi, 0xAE); setDc(&spi, 0); sendByte(&spi, 0xD5); setDc(&spi, 0); sendByte(&spi, 0x80);
  const uint8_t d[4] = {0x00, 0xFF, 0x01, 0x7F};
  setDc(&spi, 1); sendBytes(&spi, 4, d);
  stop(&spi);
  const uint8_t cmds[3] = {0xAE, 0xD5, 0x80};
  check(packets.size() == 2 && samePacket(packets[0], 1, 0x20, cmds, 3) && samePacket(packets[1], 1, 0x21, d, 4),
        "SPI: Commands AE D5 80 then Data");
  packets.clear();

  u8x8_t lcd = {};
  lcd.byte_cb = u8x8_byte_arduino_hw_spi;
  lcd.cad_cb = u8x8_cad_st7920_spi;
  check(atcMirrorTee(&lcd, mirror, 2), "st7920 tee");
  start(&lcd); sendByte(&lcd, 0xF8); sendByte(&lcd, 0x30); sendByte(&lcd, 0x00); stop(&lcd);
  const uint8_t st[3] = {0xF8, 0x30, 0x00};
  check(packets.size() == 1 && samePacket(packets[0], 2, 0x21, st, 3), "ST7920 -> Data F8 30 00");
  packets.clear();

  u8x8_t mx = {};
  mx.byte_cb = u8x8_byte_arduino_hw_spi;
  mx.display_cb = u8x8_d_max7219_32x8;
  check(atcMirrorTee(&mx, mirror, 3), "max tee");
  start(&mx);
  for (int i = 0; i < 4; ++i) { sendByte(&mx, 0x0C); sendByte(&mx, 0x01); }
  stop(&mx);
  const uint8_t lx[8] = {0x0C, 0x01, 0x0C, 0x01, 0x0C, 0x01, 0x0C, 0x01};
  check(packets.size() == 1 && samePacket(packets[0], 3, 0x24, lx, 8), "MAX7219 -> Latch");
  packets.clear();

  u8x8_t extra = {};
  extra.byte_cb = u8x8_byte_arduino_hw_spi;
  check(!atcMirrorTee(&extra, mirror, 5), "fifth display refused");
  atcMirrorUntee(&mx);
  check(mx.byte_cb == u8x8_byte_arduino_hw_spi, "untee restores");
  check(atcMirrorTee(&extra, mirror, 5), "slot freed by untee");

  // Checked (bool) callbacks, the default heap limit and a re-tee that flushes pending bytes.
  {
    AtcDisplayMirror m;
    static int calls = 0;
    m.begin([](const uint8_t*, size_t) { ++calls; return false; });
    check(!m.sendReset(0) && calls == 1, "bool lambda returning false -> send false");
    m.begin([](const uint8_t*, size_t) { ++calls; return true; });
    check(m.sendReset(0) && calls == 2, "bool lambda returning true -> send true");
    m.begin(checkedFn);
    check(!m.sendReset(0), "bool function pointer");
    m.begin(AtcDisplayMirror::CheckedPacketCallback(checkedFn));
    check(!m.sendReset(0), "explicit CheckedPacketCallback");
    m.begin([](const uint8_t*, size_t) { ++calls; });
    check(m.sendReset(0) && calls == 3, "void lambda still works");
    uint8_t buf[16];
    m.begin([](const uint8_t*, size_t) { return false; }, buf, sizeof(buf));
    check(!m.sendReset(0), "bool lambda with caller buffer");
    FakeStream fs;
    m.begin(fs);
    check(m.sendReset(0) && fs.bytes.size() == 12, "Stream overload still chosen");
    static uint8_t big[65536];
    m.begin(collect);
    check(m.sendData(0, big, 65536), "default heap limit fits a 65,536-byte Data payload");
    check(AtcDisplayMirror::kDefaultCallbackHeapLimit == 65548, "default heap limit 65,548");
    packets.clear();

    mirror.begin(collect);
    u8x8_t re = {};
    re.byte_cb = u8x8_byte_arduino_hw_spi;
    atcMirrorUntee(&extra);
    check(atcMirrorTee(&re, mirror, 6), "re tee first");
    start(&re); setDc(&re, 0); sendByte(&re, 0xAE);
    check(packets.empty(), "pending bytes not sent yet");
    check(atcMirrorTee(&re, mirror, 7), "re tee second");
    const uint8_t ae[1] = {0xAE};
    check(packets.size() == 1 && samePacket(packets[0], 6, 0x20, ae, 1), "re-tee flushes pending to old id");
    packets.clear();
  }

  std::printf("%s: %d failures\n", failures == 0 ? "OK" : "FAILED", failures);
  return failures == 0 ? 0 : 1;
}
