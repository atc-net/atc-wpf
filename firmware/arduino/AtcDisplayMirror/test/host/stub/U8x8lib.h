// Host stub of U8g2's <U8x8lib.h>: the minimal u8x8 C API the tee touches (written for this test, not copied from U8g2).
// Message numbers and the struct members used match U8g2's csrc/u8x8.h; the procedures are defined by the test as fakes.
#pragma once
#include "Arduino.h"

typedef struct u8x8_struct u8x8_t;
typedef uint8_t (*u8x8_msg_cb)(u8x8_t* u8x8, uint8_t msg, uint8_t arg_int, void* arg_ptr);

struct u8x8_struct {
  u8x8_msg_cb display_cb;
  u8x8_msg_cb cad_cb;
  u8x8_msg_cb byte_cb;
  uint8_t i2c_address;
};

#define U8X8_MSG_BYTE_SET_DC 32
#define U8X8_MSG_BYTE_SEND 23
#define U8X8_MSG_BYTE_START_TRANSFER 24
#define U8X8_MSG_BYTE_END_TRANSFER 25

extern "C" {
uint8_t u8x8_byte_arduino_hw_i2c(u8x8_t* u8x8, uint8_t msg, uint8_t arg_int, void* arg_ptr);
uint8_t u8x8_byte_arduino_2nd_hw_i2c(u8x8_t* u8x8, uint8_t msg, uint8_t arg_int, void* arg_ptr);
uint8_t u8x8_byte_arduino_sw_i2c(u8x8_t* u8x8, uint8_t msg, uint8_t arg_int, void* arg_ptr);
uint8_t u8x8_byte_arduino_hw_spi(u8x8_t* u8x8, uint8_t msg, uint8_t arg_int, void* arg_ptr);
uint8_t u8x8_byte_sw_i2c(u8x8_t* u8x8, uint8_t msg, uint8_t arg_int, void* arg_ptr);
uint8_t u8x8_cad_st7920_spi(u8x8_t* u8x8, uint8_t msg, uint8_t arg_int, void* arg_ptr);
uint8_t u8x8_cad_001(u8x8_t* u8x8, uint8_t msg, uint8_t arg_int, void* arg_ptr);
uint8_t u8x8_d_max7219_64x8(u8x8_t* u8x8, uint8_t msg, uint8_t arg_int, void* arg_ptr);
uint8_t u8x8_d_max7219_32x8(u8x8_t* u8x8, uint8_t msg, uint8_t arg_int, void* arg_ptr);
uint8_t u8x8_d_max7219_16x16(u8x8_t* u8x8, uint8_t msg, uint8_t arg_int, void* arg_ptr);
uint8_t u8x8_d_max7219_8x8(u8x8_t* u8x8, uint8_t msg, uint8_t arg_int, void* arg_ptr);
uint8_t u8x8_d_ssd1306(u8x8_t* u8x8, uint8_t msg, uint8_t arg_int, void* arg_ptr);
}
