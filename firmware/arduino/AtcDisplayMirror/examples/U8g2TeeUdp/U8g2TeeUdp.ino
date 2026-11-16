// Mirrors the raw I2C traffic of a U8g2 SSD1306 128x64 OLED over UDP.
//
// atcMirrorTee() wraps the U8g2 byte procedure: every I2C transaction U8g2 sends to the OLED (init sequence,
// commands, page data) is also sent as an I2cWrite packet (address 0x3C, control byte, bytes), one packet per
// datagram. On the PC, register DisplayMirrorTargets.For(ssd1306I2cDecoder) for display id 0; it replays it on an
// Ssd1306 emulator. Each I2C transaction is small (U8g2 sends page data in 24-byte transfers), so every packet
// fits a single 1,460-byte WiFiUDP datagram.
//
// Other buses work the same way: 4-wire SPI displays become Commands/Data by the D/C line, an ST7920 over SPI
// becomes Data (its serial framing, for St7920SerialDecoder), and a MAX7219 driven by U8g2 becomes Latch packets.
//
// Libraries: U8g2. Receiver: DisplayMirrorReceiver on UDP port 5556.
#include <AtcDisplayMirror.h>
#include <AtcU8g2Tee.h>
#include <U8g2lib.h>
#include <WiFi.h>
#include <WiFiUdp.h>
#include <Wire.h>

static const char* kSsid = "your-ssid";
static const char* kPassword = "your-password";
static const IPAddress kPcAddress(192, 168, 1, 10);
static const uint16_t kPcPort = 5556;
static const uint8_t kDisplayId = 0;

U8G2_SSD1306_128X64_NONAME_F_HW_I2C u8g2(U8G2_R0, U8X8_PIN_NONE);
WiFiUDP udp;
AtcDisplayMirror mirror;

void setup() {
  WiFi.begin(kSsid, kPassword);
  while (WiFi.status() != WL_CONNECTED) {
    delay(200);
  }
  mirror.begin(udp, kPcAddress, kPcPort);
  mirror.describe(kDisplayId, atc_mirror::DisplayKind::MonoOled, atc_mirror::Chip::Ssd1306, 128, 64, "U8g2 OLED");
  mirror.sendReset(kDisplayId);

  // After the constructor (it sets the byte procedure) and before begin() (so the init sequence is mirrored).
  atcMirrorTee(u8g2.getU8x8(), mirror, kDisplayId);
  u8g2.begin();
}

void loop() {
  static uint32_t counter = 0;

  u8g2.clearBuffer();
  u8g2.setFont(u8g2_font_ncenB14_tr);
  u8g2.drawStr(0, 20, "U8g2 tee");
  u8g2.setCursor(0, 50);
  u8g2.print(counter++);
  u8g2.sendBuffer();  // each I2C transaction goes to the OLED and, as I2cWrite, to the PC
  delay(200);
}
