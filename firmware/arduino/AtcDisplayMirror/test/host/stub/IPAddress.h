// Host stub of <IPAddress.h>.
#pragma once
#include "Arduino.h"

class IPAddress {
 public:
  IPAddress() : value(0) {}
  IPAddress(uint8_t a, uint8_t b, uint8_t c, uint8_t d)
      : value((static_cast<uint32_t>(a) << 24) | (static_cast<uint32_t>(b) << 16) | (static_cast<uint32_t>(c) << 8) | d) {}
  uint32_t value;
};
