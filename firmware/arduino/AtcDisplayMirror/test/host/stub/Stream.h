// Host stub of <Stream.h> (with Print).
#pragma once
#include "Arduino.h"

class Print {
 public:
  virtual ~Print() {}
  virtual size_t write(uint8_t b) = 0;
  virtual size_t write(const uint8_t* bytes, size_t length) {
    size_t i = 0;
    for (; i < length; ++i) {
      if (!write(bytes[i])) {
        break;
      }
    }
    return i;
  }
};

class Stream : public Print {
 public:
  virtual int available() = 0;
};
