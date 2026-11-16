# Generates (default) or verifies (--check) firmware/protocol/test-vectors.json. Python 3.9+, standard library only.
#   python firmware/protocol/gen_vectors.py            write test-vectors.json next to this script
#   python firmware/protocol/gen_vectors.py --check    verify the existing file, exit 1 on any mismatch
# Payloads are written by hand (hex); packets are assembled with struct
# and the CRC comes from binascii.crc_hqx (CRC-16/CCITT-FALSE), independent of the C# implementation.
import argparse, binascii, json, os, struct, sys

def crc(data): return binascii.crc_hqx(data, 0xFFFF)

def packet(display_id, ptype, flags, payload, version=1, length=None, magic=b"\xa7\xc0"):
    n = len(payload) if length is None else length
    body = struct.pack("<BBBBI", version, display_id, ptype, flags, n)  # header bytes 2..9
    return magic + body + payload + struct.pack("<H", crc(body + payload))

assert crc(b"123456789") == 0x29B1

# (name, displayId, type, flags, payloadHex)
V = [
 ("describe-oled-128x64-ssd1306", 0, 0x01, 0, "0210" "8000" "4000" "4f4c4544"),
 ("describe-non-ascii-name", 3, 0x01, 0, "0750" "8000" "4000" "c386c398c385"),
 ("mono-frame-8x8", 0, 0x10, 0, "0800" "0800" "ff818181818181ff"),
 ("color-frame-2x2", 1, 0x11, 0, "0200" "0200" "f800" "07e0" "001f" "ffff"),
 ("epaper-frame-8x2-two-planes-partial", 2, 0x12, 0, "0800" "0200" "02" "f00f" "0180" "01"),
 ("epaper-frame-8x2-one-plane-full", 2, 0x12, 0, "0800" "0200" "01" "f00f" "00"),
 ("commands", 0, 0x20, 0, "aed580a83f"),
 ("data", 0, 0x21, 0, "00ff017f"),
 ("i2c-write-0x3c", 0, 0x22, 0, "3c" "008d14"),
 ("reset-empty", 0, 0x23, 0, ""),
 ("latch-4-bytes", 4, 0x24, 0, "0c010900"),
 ("data-300-bytes-counting-pattern", 0, 0x21, 0, bytes(i & 0xFF for i in range(300)).hex()),
 ("commands-display-255-flags-0x80", 255, 0x20, 0x80, "ae"),
]
vectors = []
for name, did, t, fl, ph in V:
    p = bytes.fromhex(ph)
    vectors.append({"name": name, "displayId": did, "type": t, "flags": fl,
                    "payloadHex": ph, "packetHex": packet(did, t, fl, p).hex()})

good = packet(0, 0x23, 0, b"")
bad_crc = bytearray(good); bad_crc[-1] ^= 0xFF
cmd = packet(0, 0x20, 0, bytes.fromhex("aed580"))
R = [
 ("bad-crc", bytes(bad_crc).hex(), "BadCrc"),
 ("version-2", packet(0, 0x23, 0, b"", version=2).hex(), "UnsupportedVersion"),
 ("unknown-type-0x7f", packet(0, 0x7F, 0, b"").hex(), "UnknownType"),
 ("truncated-missing-crc-byte", cmd[:-1].hex(), "Truncated"),
 ("truncated-shorter-than-header", cmd[:5].hex(), "Truncated"),
 ("trailing-byte-after-crc", (cmd + b"\x00").hex(), "Malformed"),
 ("bad-magic", (b"\xa7\xc1" + good[2:]).hex(), "Malformed"),
 ("data-over-64kib-cap", packet(0, 0x21, 0, b"\x00\x00", length=65537).hex(), "TooLarge"),
 ("data-length-0xffffffff", packet(0, 0x21, 0, b"\x00\x00", length=0xFFFFFFFF).hex(), "TooLarge"),
 ("color-frame-over-4mib-cap", packet(0, 0x11, 0, b"\x00\x00", length=4194305).hex(), "TooLarge"),
]
rejects = [{"name": n, "packetHex": h, "reason": r} for n, h, r in R]
doc = {"crcCheck": {"inputAscii": "123456789", "crc": "0x29B1"},
       "vectors": vectors, "rejects": rejects}

MAGIC = bytes((0xA7, 0xC0))
HERE = os.path.dirname(os.path.abspath(__file__))
PATH = os.path.join(HERE, "test-vectors.json")


def check(path):
    errors = []
    with open(path, encoding="utf-8") as f:
        d = json.load(f)
    if d.get("crcCheck", {}).get("crc") != "0x29B1":
        errors.append("crcCheck")
    for v in d["vectors"]:
        p, pl, n = bytes.fromhex(v["packetHex"]), bytes.fromhex(v["payloadHex"]), v["name"]
        if p[:2] != MAGIC: errors.append(n + ": magic")
        if len(p) != 12 + len(pl): errors.append(n + ": packet length")
        elif struct.unpack_from("<I", p, 6)[0] != len(pl): errors.append(n + ": length field")
        elif p[2] != 1 or p[3] != v["displayId"] or p[4] != v["type"] or p[5] != v["flags"]: errors.append(n + ": header")
        elif p[10:-2] != pl: errors.append(n + ": payload")
        if len(p) >= 12 and struct.unpack_from("<H", p, len(p) - 2)[0] != crc(p[2:-2]): errors.append(n + ": crc")
    for r in d["rejects"]:
        p, n, why = bytes.fromhex(r["packetHex"]), r["name"], r["reason"]
        if len(p) < 12:
            ok = why == "Truncated"
        else:
            ln = struct.unpack_from("<I", p, 6)[0]
            crc_ok = struct.unpack_from("<H", p, len(p) - 2)[0] == crc(p[2:-2]) if ln == len(p) - 12 else None
            ok = {"BadCrc": p[:2] == MAGIC and ln == len(p) - 12 and crc_ok is False,
                  "UnsupportedVersion": p[2] != 1 and crc_ok,
                  "UnknownType": p[4] not in (1, 0x10, 0x11, 0x12, 0x20, 0x21, 0x22, 0x23, 0x24) and crc_ok,
                  "Truncated": ln > len(p) - 12,
                  "Malformed": p[:2] != MAGIC or ln < len(p) - 12,
                  "TooLarge": ln > (4194304 if p[4] in (0x10, 0x11, 0x12) else 65536)}.get(why, False)
        if not ok: errors.append(n + ": does not exhibit " + why)
    return errors


ap = argparse.ArgumentParser()
ap.add_argument("--check", action="store_true", help="verify the existing test-vectors.json instead of writing it")
args = ap.parse_args()
if args.check:
    errs = check(PATH)
    for e in errs: print("MISMATCH", e, file=sys.stderr)
    print("OK" if not errs else "FAILED", len(vectors), "vectors,", len(rejects), "rejects")
    sys.exit(1 if errs else 0)
with open(PATH, "w", newline=chr(10)) as f:
    f.write(json.dumps(doc, indent=2))
