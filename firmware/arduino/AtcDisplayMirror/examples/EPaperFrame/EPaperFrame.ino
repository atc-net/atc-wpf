// Draws a 2.9" SSD1680 e-paper picture (128 x 296, native portrait) into a GFXcanvas1, shows it with GxEPD2
// and mirrors it over serial as an EPaperFrame.
//
// GxEPD2 keeps its page buffer private (and it may hold only part of the screen), so the picture is drawn into
// a full-screen GFXcanvas1 instead: ceil(128 / 8) x 296 = 4,736 bytes, rows padded to whole bytes, MSB = leftmost
// pixel - the SSD1680 RAM layout the EPaperFrame uses. Polarity: canvas bit 1 = white, 0 = black, the same as the
// SSD1680 black RAM (GxEPD2 fills 0xFF for GxEPD_WHITE), so the buffer goes to the panel and the PC unchanged.
// GxEPD2's writeImageForFullRefresh() writes the bitmap to RAM 0x26 and 0x24 as it is (invert = false).
//
// Libraries: GxEPD2, Adafruit GFX. Receiver: an e-paper target for display id 2 on a serial port.
#include <Adafruit_GFX.h>
#include <AtcDisplayMirror.h>
#include <GxEPD2_BW.h>

static const uint16_t kWidth = GxEPD2_290_T94::WIDTH;   // 128
static const uint16_t kHeight = GxEPD2_290_T94::HEIGHT;  // 296
static const uint8_t kDisplayId = 2;

// CS 5, DC 17, RST 16, BUSY 4: adjust to your wiring.
GxEPD2_BW<GxEPD2_290_T94, GxEPD2_290_T94::HEIGHT> display(GxEPD2_290_T94(5, 17, 16, 4));
GFXcanvas1 canvas(kWidth, kHeight);
AtcDisplayMirror mirror;

static void drawPicture(uint32_t counter) {
  canvas.fillScreen(GxEPD_WHITE);  // bits 1
  canvas.setTextColor(GxEPD_BLACK);  // bits 0
  canvas.setTextSize(2);
  canvas.setCursor(8, 20);
  canvas.print("E-paper");
  canvas.setCursor(8, 60);
  canvas.print(counter);
  canvas.drawRect(4, 100, kWidth - 8, 180, GxEPD_BLACK);
  canvas.fillCircle(kWidth / 2, 190, 30 + (counter % 4) * 5, GxEPD_BLACK);
}

void setup() {
  Serial.begin(921600);
  mirror.begin(Serial);
  mirror.describe(kDisplayId, atc_mirror::DisplayKind::EPaper, atc_mirror::Chip::Ssd1680, kWidth, kHeight,
                  "2.9in e-paper");
  display.init();
}

void loop() {
  static uint32_t counter = 0;

  drawPicture(counter++);
  display.epd2.writeImageForFullRefresh(canvas.getBuffer(), 0, 0, kWidth, kHeight);
  display.epd2.refresh(false);  // full refresh
  display.epd2.powerOff();

  // One plane (black/white), full refresh. A red plane follows the SSD1680 RAM 0x26 polarity (1 = red): a GxEPD2_3C
  // colour buffer is 0 = red, so invert it (~byte) before sending; Adafruit_EPD buffers go out unchanged.
  mirror.sendEPaperFrame(kDisplayId, canvas.getBuffer(), nullptr, kWidth, kHeight, false);
  delay(30000);
}
