// Mirrors an Adafruit_SSD1306 128x64 OLED over the USB serial port as MonoFrame packets.
// The Adafruit_SSD1306 buffer (getBuffer()) is already in the MonoFrame layout: 128 bytes per page,
// 8 pages, bit 0 = top row of the page. It stays in the panel's own orientation whatever setRotation() says.
//
// Libraries: Adafruit SSD1306, Adafruit GFX. Receiver: DisplayMirrorReceiver over a serial port, display id 0.
// Do not print text on Serial: this sketch uses it for the mirror packets only.
#include <Adafruit_GFX.h>
#include <Adafruit_SSD1306.h>
#include <AtcDisplayMirror.h>
#include <Wire.h>

static const uint8_t kWidth = 128;
static const uint8_t kHeight = 64;
static const uint8_t kDisplayId = 0;

Adafruit_SSD1306 display(kWidth, kHeight, &Wire, -1);
AtcDisplayMirror mirror;

void setup() {
  Serial.begin(921600);
  mirror.begin(Serial);

  if (!display.begin(SSD1306_SWITCHCAPVCC, 0x3C)) {
    for (;;) {
      delay(1000);
    }
  }
  mirror.describe(kDisplayId, atc_mirror::DisplayKind::MonoOled, atc_mirror::Chip::Ssd1306, kWidth, kHeight, "OLED");
}

void loop() {
  static uint32_t counter = 0;

  display.clearDisplay();
  display.setTextSize(2);
  display.setTextColor(SSD1306_WHITE);
  display.setCursor(0, 0);
  display.println(F("Mirror"));
  display.print(counter++);
  display.drawRect(0, 40, kWidth, 24, SSD1306_WHITE);
  display.fillRect(2, 42, (counter * 4) % (kWidth - 4), 20, SSD1306_WHITE);
  display.display();

  // 1,040-byte packet: about 11 ms at 921,600 baud.
  mirror.sendMonoFrame(kDisplayId, display.getBuffer(), kWidth, kHeight);
  delay(100);
}
