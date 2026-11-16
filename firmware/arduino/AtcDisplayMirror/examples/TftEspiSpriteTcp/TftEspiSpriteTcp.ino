// Draws into a TFT_eSPI sprite, pushes it to the panel and mirrors it over TCP as a ColorFrame.
//
// TFT_eSprite stores 16-bit pixels byte-swapped (high byte first in memory), so getPointer() is sent with
// pixelsAreBigEndian = true and the bytes go out unchanged. A 160 x 128 sprite is 40,960 bytes; a full
// 320 x 240 sprite (153,600 bytes) needs an ESP32 with PSRAM. The frame is streamed: no extra buffer.
//
// Libraries: TFT_eSPI (configure User_Setup.h for your panel). Receiver: DisplayMirrorReceiver listening on
// TCP port 5555 (the default of the WPF display mirror sample), display id 1. The frame is the sprite as drawn
// in rotation 1 (landscape), so size the receiver's target 160 x 128, not the panel's native 128 x 160.
#include <AtcDisplayMirror.h>
#include <TFT_eSPI.h>
#include <WiFi.h>

static const char* kSsid = "your-ssid";
static const char* kPassword = "your-password";
static const char* kPcHost = "192.168.1.10";
static const uint16_t kPcPort = 5555;
static const uint8_t kDisplayId = 1;
static const int16_t kWidth = 160;
static const int16_t kHeight = 128;

TFT_eSPI tft;
TFT_eSprite sprite(&tft);
WiFiClient client;
AtcDisplayMirror mirror;

static bool ensureConnected() {
  if (client.connected()) {
    return true;
  }
  if (!client.connect(kPcHost, kPcPort)) {
    return false;
  }
  client.setNoDelay(true);
  mirror.describe(kDisplayId, atc_mirror::DisplayKind::ColorPanel, atc_mirror::Chip::St7735, kWidth, kHeight,
                  "TFT");
  return true;
}

void setup() {
  tft.init();
  tft.setRotation(1);
  sprite.setColorDepth(16);
  sprite.createSprite(kWidth, kHeight);

  WiFi.begin(kSsid, kPassword);
  while (WiFi.status() != WL_CONNECTED) {
    delay(200);
  }
  mirror.begin(client);
}

void loop() {
  static uint16_t frame = 0;

  sprite.fillSprite(TFT_NAVY);
  sprite.fillCircle((frame * 3) % kWidth, kHeight / 2, 20, TFT_ORANGE);
  sprite.setTextColor(TFT_WHITE);
  sprite.drawString(String("Frame ") + String(frame++), 4, 4, 2);
  sprite.pushSprite(0, 0);

  if (ensureConnected()) {
    const uint16_t* pixels = static_cast<const uint16_t*>(sprite.getPointer());
    if (!mirror.sendColorFrame(kDisplayId, pixels, kWidth, kHeight, true)) {
      client.stop();  // reconnect on the next frame
    }
  }
  delay(50);
}
