// AtcU8g2Tee.h - mirrors the raw bus traffic of a U8g2 / U8x8 display.
//
// Include it only in sketches that use U8g2 (it needs <U8x8lib.h>). AtcDisplayMirror.h does not include it.
//
// How it works (verified against U8g2 csrc/u8x8.h, u8x8_byte.c, u8x8_cad.c and cppsrc/U8x8lib.cpp):
//   Every byte U8g2 puts on the bus goes through the byte procedure stored in u8x8->byte_cb (the CAD layer and
//   the u8x8_byte_* helpers all call u8x8->byte_cb). atcMirrorTee() stores the original procedure and puts a
//   wrapper in its place. The wrapper first calls the original procedure (so the real display is never delayed
//   by the mirror), then records the bytes:
//     U8X8_MSG_BYTE_SET_DC          arg_int = D/C level (0 = command, 1 = data)
//     U8X8_MSG_BYTE_START_TRANSFER  chip select on / I2C start
//     U8X8_MSG_BYTE_SEND            arg_int bytes at arg_ptr
//     U8X8_MSG_BYTE_END_TRANSFER    chip select off / I2C stop
//
// Bus modes (AtcTeeBus::Auto picks one when the tee is installed):
//   I2c          each START..END transaction becomes one I2cWrite: address = u8x8->i2c_address >> 1 (U8g2 keeps
//                the 8-bit write address), bytes = what SEND carried, i.e. the SSD13xx control byte (0x00 command
//                stream, 0x40 data) followed by the command or data bytes. Auto: the original byte procedure is
//                u8x8_byte_arduino_hw_i2c, u8x8_byte_arduino_2nd_hw_i2c, u8x8_byte_arduino_sw_i2c or u8x8_byte_sw_i2c.
//                A custom or ESP-IDF I2C byte procedure is not recognised (it would be taken as DcBus): pass
//                AtcTeeBus::I2c explicitly.
//   St7920Serial every SEND byte is sent as Data: the ST7920 serial framing as it is on the wire (0xF8 / 0xFA sync
//                byte, then the high and low nibble bytes), for the St7920SerialDecoder target.
//                Auto: u8x8->cad_cb == u8x8_cad_st7920_spi.
//   Latch        each START..END transfer is one chip-select frame, sent with sendLatch (MAX7219: register and
//                data byte per chained device). Auto: u8x8->display_cb is one of u8x8_d_max7219_*.
//   DcBus        SEND bytes are Commands while the last SET_DC was 0 and Data while it was 1 (4-wire SPI
//                SSD1306/SSD1309/SH1106, ...). Everything else ends up here.
//   Consecutive bytes of the same kind are collected (up to kAtcTeeBufferSize) and sent at END_TRANSFER, on a D/C
//   change or when the buffer is full, so a command sequence is not one 13-byte packet per byte.
//   ⚠️ An I2C transaction or a latch longer than kAtcTeeBufferSize bytes is sent in pieces (each piece is its own
//   I2cWrite / Latch). U8g2's SSD13xx I2C CAD splits data into 24-byte transfers and a MAX7219 latch is 2 bytes
//   per device, so this does not happen with the stock drivers.
//
// Usage: install after the U8G2_* constructor (it calls u8x8_Setup, which sets byte_cb) and before begin(),
// so the init sequence is mirrored too.
//   U8G2_SSD1306_128X64_NONAME_F_HW_I2C u8g2(U8G2_R0);
//   atcMirrorTee(u8g2.getU8x8(), mirror, 0);
//   u8g2.begin();
//
// Up to kAtcTeeMaxDisplays displays can be teed at the same time (U8X8_WITH_USER_PTR is off on the ESP32, so
// the contexts live in a small static table looked up by the u8x8 pointer). Not thread safe: drive each display
// from one task.
#ifndef ATC_U8G2_TEE_H
#define ATC_U8G2_TEE_H

#include <U8x8lib.h>

#include "AtcDisplayMirror.h"

/// How the tee turns the bus traffic into messages.
enum class AtcTeeBus : uint8_t {
  Auto,
  DcBus,
  I2c,
  St7920Serial,
  Latch
};

/// Displays that can be teed at once.
static const uint8_t kAtcTeeMaxDisplays = 4;
/// Bytes collected before a message is sent.
static const uint16_t kAtcTeeBufferSize = 256;

namespace atc_mirror_tee {

struct Context {
  u8x8_t* u8x8;
  AtcDisplayMirror* mirror;
  u8x8_msg_cb inner;
  uint8_t displayId;
  AtcTeeBus bus;
  uint8_t dc;         // last SET_DC level
  uint8_t pendingDc;  // D/C level of the collected bytes (DcBus)
  uint8_t address;    // 7-bit I2C address of the current transaction
  uint16_t length;
  uint8_t buffer[kAtcTeeBufferSize];
};

// Function-local static in an inline function: one table for the whole program, whatever the number of
// translation units that include this header.
inline Context* contexts() {
  static Context table[kAtcTeeMaxDisplays];
  return table;
}

inline Context* find(u8x8_t* u8x8) {
  Context* table = contexts();
  for (uint8_t i = 0; i < kAtcTeeMaxDisplays; ++i) {
    if (table[i].u8x8 == u8x8) {
      return &table[i];
    }
  }
  return nullptr;
}

inline void flush(Context& c) {
  if (c.length == 0) {
    return;
  }
  switch (c.bus) {
    case AtcTeeBus::I2c:
      c.mirror->sendI2cWrite(c.displayId, c.address, c.buffer, c.length);
      break;
    case AtcTeeBus::Latch:
      c.mirror->sendLatch(c.displayId, c.buffer, c.length);
      break;
    case AtcTeeBus::St7920Serial:
      c.mirror->sendData(c.displayId, c.buffer, c.length);
      break;
    default:
      if (c.pendingDc == 0) {
        c.mirror->sendCommands(c.displayId, c.buffer, c.length);
      } else {
        c.mirror->sendData(c.displayId, c.buffer, c.length);
      }
      break;
  }
  c.length = 0;
}

inline void append(Context& c, const uint8_t* bytes, uint8_t count) {
  if (c.bus == AtcTeeBus::DcBus && c.length != 0 && c.pendingDc != c.dc) {
    flush(c);
  }
  c.pendingDc = c.dc;
  for (uint8_t i = 0; i < count; ++i) {
    if (c.length == kAtcTeeBufferSize) {
      flush(c);
    }
    c.buffer[c.length++] = bytes[i];
  }
}

inline bool isI2cProcedure(u8x8_msg_cb procedure) {
  return procedure == u8x8_byte_arduino_hw_i2c || procedure == u8x8_byte_arduino_2nd_hw_i2c ||
         procedure == u8x8_byte_arduino_sw_i2c || procedure == u8x8_byte_sw_i2c;
}

inline bool isMax7219(u8x8_msg_cb display) {
  return display == u8x8_d_max7219_64x8 || display == u8x8_d_max7219_32x8 || display == u8x8_d_max7219_16x16 ||
         display == u8x8_d_max7219_8x8;
}

inline AtcTeeBus detect(u8x8_t* u8x8, u8x8_msg_cb inner) {
  if (u8x8->cad_cb == u8x8_cad_st7920_spi) {
    return AtcTeeBus::St7920Serial;
  }
  if (isMax7219(u8x8->display_cb)) {
    return AtcTeeBus::Latch;
  }
  if (isI2cProcedure(inner)) {
    return AtcTeeBus::I2c;
  }
  return AtcTeeBus::DcBus;
}

}  // namespace atc_mirror_tee

/// The byte procedure installed by atcMirrorTee. C linkage to match the u8x8_msg_cb type of the C library.
extern "C" inline uint8_t atcMirrorTeeByteCb(u8x8_t* u8x8, uint8_t msg, uint8_t arg_int, void* arg_ptr) {
  atc_mirror_tee::Context* c = atc_mirror_tee::find(u8x8);
  if (c == nullptr || c->inner == nullptr) {
    return 0;
  }
  // Read the address before the inner procedure runs, as it uses it for this START_TRANSFER.
  if (msg == U8X8_MSG_BYTE_START_TRANSFER) {
    atc_mirror_tee::flush(*c);
    c->address = static_cast<uint8_t>(u8x8->i2c_address >> 1);
  }
  uint8_t result = c->inner(u8x8, msg, arg_int, arg_ptr);
  switch (msg) {
    case U8X8_MSG_BYTE_SET_DC:
      c->dc = arg_int;
      break;
    case U8X8_MSG_BYTE_SEND:
      if (arg_ptr != nullptr) {
        atc_mirror_tee::append(*c, static_cast<const uint8_t*>(arg_ptr), arg_int);
      }
      break;
    case U8X8_MSG_BYTE_END_TRANSFER:
      atc_mirror_tee::flush(*c);
      break;
    default:
      break;
  }
  return result;
}

/// Mirrors the bus traffic of `u8x8` to `mirror` as display `displayId`.
/// inner: the original byte procedure; nullptr takes the one installed now (u8x8->byte_cb).
/// bus: how bytes become messages; Auto detects I2C, ST7920 serial, MAX7219 latch, else Commands/Data by D/C.
/// Call it after the U8G2_* constructor and before begin(). Calling it again for the same u8x8 first sends the
/// bytes collected so far (to the old mirror and display id), then updates the mirror, display id and mode. Returns false when u8x8 is null, the table is full or no procedure is known.
inline bool atcMirrorTee(u8x8_t* u8x8, AtcDisplayMirror& mirror, uint8_t displayId, u8x8_msg_cb inner = nullptr,
                         AtcTeeBus bus = AtcTeeBus::Auto) {
  if (u8x8 == nullptr) {
    return false;
  }
  atc_mirror_tee::Context* c = atc_mirror_tee::find(u8x8);
  if (c != nullptr) {
    // Re-tee: send what was collected for the old mirror / display id / mode before it is reset.
    atc_mirror_tee::flush(*c);
  } else {
    c = atc_mirror_tee::find(nullptr);
    if (c == nullptr) {
      return false;
    }
    c->inner = nullptr;
  }
  if (inner == nullptr || inner == atcMirrorTeeByteCb) {
    // Already teed: keep the stored original; never chain the wrapper to itself.
    inner = u8x8->byte_cb == atcMirrorTeeByteCb ? c->inner : u8x8->byte_cb;
  }
  if (inner == nullptr) {
    return false;
  }
  c->u8x8 = u8x8;
  c->mirror = &mirror;
  c->inner = inner;
  c->displayId = displayId;
  c->bus = bus == AtcTeeBus::Auto ? atc_mirror_tee::detect(u8x8, inner) : bus;
  c->dc = 0;
  c->pendingDc = 0;
  c->address = static_cast<uint8_t>(u8x8->i2c_address >> 1);
  c->length = 0;
  u8x8->byte_cb = atcMirrorTeeByteCb;
  return true;
}

/// Removes the tee from `u8x8` and restores its original byte procedure (collected bytes are sent first).
inline void atcMirrorUntee(u8x8_t* u8x8) {
  atc_mirror_tee::Context* c = atc_mirror_tee::find(u8x8);
  if (u8x8 == nullptr || c == nullptr) {
    return;
  }
  atc_mirror_tee::flush(*c);
  if (u8x8->byte_cb == atcMirrorTeeByteCb) {
    u8x8->byte_cb = c->inner;
  }
  c->u8x8 = nullptr;
  c->inner = nullptr;
}

#endif  // ATC_U8G2_TEE_H
