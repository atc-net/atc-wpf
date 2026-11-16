# Host test of the protocol encoder

`protocol_test.cpp` checks `src/AtcMirrorProtocol.h` against the golden vectors in
`firmware/protocol/test-vectors.json` on a PC (no Arduino needed). It covers:

- `encode()`: byte-for-byte equal to every `packetHex`, and it refuses a buffer one byte short;
- the streaming API: `encodeHeader`, `crcUpdate` over chunks of 1, 7 and 64 bytes, then `encodeCrc`;
- the CRC check value;
- `rgb565ToWire` with both byte orders;
- the frame size helpers and the payload caps;
- what each reject vector shows (bad CRC, version, type, length, magic).

`vectors.h` is generated from the JSON. Regenerate it whenever the vectors change:

```bash
python firmware/arduino/AtcDisplayMirror/test/host/gen_vectors.py
```

Build and run it with any C++11 or later compiler:

```bash
cd firmware/arduino/AtcDisplayMirror/test/host
g++ -std=c++17 -Wall -Wextra -I../../src protocol_test.cpp -o protocol_test && ./protocol_test
```

With MSVC, from a Developer Command Prompt:

```bat
cl /nologo /std:c++17 /W4 /EHsc /I ..\..\src protocol_test.cpp && protocol_test.exe
```

It prints `OK: <n> checks, 0 failures` and exits with 0, or prints each failure and exits with 1.

## Sinks and the U8g2 tee

`sink_tee_test.cpp` builds `src/AtcDisplayMirror.cpp` and `src/AtcU8g2Tee.h` against the small stub headers in `stub/`
(`Arduino.h`, `IPAddress.h`, `Stream.h`, `Udp.h` and a minimal `U8x8lib.h` written for this test, not copied from U8g2).
It checks:

- every `send*` method on a `Stream` sink, byte for byte against the golden vectors (`vectors.h`), including colour
  frames larger than one 64-pixel conversion chunk and the arguments it refuses;
- the UDP sink: one datagram per packet, a 1,460-byte packet fits and a 1,461-byte one is refused;
- the callback sinks: heap buffer, caller buffer (and `failedPackets()`), `bool` callbacks that report failure, and the
  default 65,548-byte heap limit;
- the tee: an I2C transaction becomes one I2cWrite, D/C SPI becomes Commands then Data, the ST7920 serial stream becomes
  Data, a MAX7219 chip-select frame becomes Latch, the inner procedure still gets every call, the slot limit,
  `atcMirrorUntee`, and a second tee of the same display flushes its pending bytes to the old display id.

```bash
cd firmware/arduino/AtcDisplayMirror/test/host
g++ -std=c++17 -Wall -Wextra -Istub -I../../src sink_tee_test.cpp ../../src/AtcDisplayMirror.cpp -o sink_tee_test && ./sink_tee_test
```

With MSVC, from a Developer Command Prompt:

```bat
cl /nologo /std:c++17 /W4 /EHsc /I stub /I ..\..\src sink_tee_test.cpp ..\..\src\AtcDisplayMirror.cpp && sink_tee_test.exe
```

It prints `OK: 0 failures` and exits with 0, or prints each failure and exits with 1.

The Arduino IDE does not compile this folder: it only builds `src/` and the sketch.
