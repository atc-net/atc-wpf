// Host stub of <Udp.h>.
#pragma once
#include "IPAddress.h"
#include "Stream.h"

class UDP : public Stream {
 public:
  virtual int beginPacket(IPAddress ip, uint16_t port) = 0;
  virtual int endPacket() = 0;
  size_t write(uint8_t b) override = 0;
  size_t write(const uint8_t* bytes, size_t length) override = 0;
};
