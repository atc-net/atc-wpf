// Publishes an Adafruit_SSD1306 128x64 frame to an MQTT broker with PubSubClient, one packet per message.
//
// The callback sink assembles each packet in a heap buffer and hands it to publish(). PubSubClient refuses a
// publish unless its buffer holds 5 + 2 + topic length + payload bytes (PubSubClient.cpp), and its default
// buffer is 256 bytes, so raise it with setBufferSize() (a uint16_t, at most 65,535). A 128x64 MonoFrame
// packet is 1,040 bytes. For packets near or over 64 KB use beginPublish()/write()/endPublish() instead.
//
// On the PC, subscribe to the topic and pass each message payload to DisplayMirrorReceiver.ProcessMessage.
// Libraries: PubSubClient, Adafruit SSD1306, Adafruit GFX.
#include <Adafruit_GFX.h>
#include <Adafruit_SSD1306.h>
#include <AtcDisplayMirror.h>
#include <PubSubClient.h>
#include <WiFi.h>
#include <Wire.h>

static const char* kSsid = "your-ssid";
static const char* kPassword = "your-password";
static const char* kBroker = "192.168.1.10";
static const uint16_t kBrokerPort = 1883;
static const char* kTopic = "atc/display/0";
static const uint8_t kDisplayId = 0;
static const uint8_t kWidth = 128;
static const uint8_t kHeight = 64;

Adafruit_SSD1306 display(kWidth, kHeight, &Wire, -1);
WiFiClient network;
PubSubClient mqtt(network);
AtcDisplayMirror mirror;

// Returning the publish result makes sendMonoFrame() return false when PubSubClient refuses the message.
static bool publishPacket(const uint8_t* packet, size_t length) {
  return mqtt.publish(kTopic, packet, static_cast<unsigned int>(length));
}

static bool ensureConnected() {
  if (mqtt.connected()) {
    return true;
  }
  if (!mqtt.connect("atc-display-mirror")) {
    return false;
  }
  mirror.describe(kDisplayId, atc_mirror::DisplayKind::MonoOled, atc_mirror::Chip::Ssd1306, kWidth, kHeight, "OLED");
  return true;
}

void setup() {
  display.begin(SSD1306_SWITCHCAPVCC, 0x3C);

  WiFi.begin(kSsid, kPassword);
  while (WiFi.status() != WL_CONNECTED) {
    delay(200);
  }
  mqtt.setServer(kBroker, kBrokerPort);
  mqtt.setBufferSize(1200);  // 1,040-byte packet + 7 + topic, with room to spare
  mirror.begin(publishPacket, 1200);
}

void loop() {
  static uint32_t counter = 0;

  display.clearDisplay();
  display.setTextSize(2);
  display.setTextColor(SSD1306_WHITE);
  display.setCursor(0, 0);
  display.println(F("MQTT"));
  display.print(counter++);
  display.display();

  if (ensureConnected()) {
    if (!mirror.sendMonoFrame(kDisplayId, display.getBuffer(), kWidth, kHeight)) {
      // Not published: the connection dropped, or the buffer set with setBufferSize() is too small.
    }
  }
  mqtt.loop();
  delay(500);
}
