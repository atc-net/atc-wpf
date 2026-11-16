// Drives a chain of four MAX7219 8x8 modules with plain SPI and mirrors every chip-select frame as a Latch.
//
// A MAX7219 takes a 16-bit word (register, data) per device; with a chain the words for all devices are
// shifted in during one chip-select low and latched on the rising edge, the first word ending up in the
// farthest device. latchAll() sends that frame to the chain and the identical bytes to the PC.
//
// Why not MD_MAX72XX? It keeps its register images and SPI buffer private, and getColumn()/getRow() return
// values after its module-type mapping, not the bytes on the wire. With U8g2 (U8G2_MAX7219_32X8_F_4W_HW_SPI)
// use atcMirrorTee() instead: it detects the MAX7219 driver and sends each transfer as a Latch.
//
// Receiver: a MAX7219 target for display id 4, chain length 4 (wiring variant FC16 for the common modules).
#include <AtcDisplayMirror.h>
#include <SPI.h>

static const uint8_t kChipSelect = 5;  // VSPI SS on most ESP32 boards; MOSI 23, SCK 18
static const uint8_t kDevices = 4;
static const uint8_t kDisplayId = 4;

AtcDisplayMirror mirror;

// Sends one chip-select frame to the chain, then mirrors it.
static void latchFrame(const uint8_t* frame, size_t length) {
  SPI.beginTransaction(SPISettings(10000000, MSBFIRST, SPI_MODE0));
  digitalWrite(kChipSelect, LOW);
  for (size_t i = 0; i < length; ++i) {
    SPI.transfer(frame[i]);
  }
  digitalWrite(kChipSelect, HIGH);
  SPI.endTransaction();
  mirror.sendLatch(kDisplayId, frame, length);
}

// The same register in every device; values[0] goes to the farthest device (it is shifted in first).
static void latchAll(uint8_t reg, const uint8_t* values) {
  uint8_t frame[2 * kDevices];
  for (uint8_t d = 0; d < kDevices; ++d) {
    frame[2 * d] = reg;
    frame[2 * d + 1] = values[d];
  }
  latchFrame(frame, sizeof(frame));
}

static void latchSame(uint8_t reg, uint8_t value) {
  uint8_t values[kDevices];
  for (uint8_t d = 0; d < kDevices; ++d) {
    values[d] = value;
  }
  latchAll(reg, values);
}

void setup() {
  Serial.begin(921600);
  mirror.begin(Serial);
  mirror.describe(kDisplayId, atc_mirror::DisplayKind::LedMatrix, atc_mirror::Chip::Max7219, 8 * kDevices, 8,
                  "MAX7219 x4");

  pinMode(kChipSelect, OUTPUT);
  digitalWrite(kChipSelect, HIGH);
  SPI.begin();

  latchSame(0x0F, 0x00);  // display test off
  latchSame(0x09, 0x00);  // decode mode: none (raw segments / matrix rows)
  latchSame(0x0B, 0x07);  // scan limit: digits 0..7
  latchSame(0x0A, 0x04);  // intensity 4 of 15
  latchSame(0x0C, 0x01);  // shutdown off = normal operation
}

void loop() {
  static uint8_t step = 0;

  // A diagonal line that moves through the chain: digit registers 0x01..0x08 are the 8 rows of each module.
  for (uint8_t row = 0; row < 8; ++row) {
    uint8_t values[kDevices];
    for (uint8_t d = 0; d < kDevices; ++d) {
      values[d] = static_cast<uint8_t>(1U << ((row + step + d) & 7));
    }
    latchAll(static_cast<uint8_t>(0x01 + row), values);
  }
  step++;
  delay(100);
}
