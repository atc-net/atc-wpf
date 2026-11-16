"""Tee an smbus2 bus: writes to 0x3C are mirrored to display 0 as I2cWrite, then sent to the real device."""
from smbus2 import SMBus

from atc_display_mirror import Mirror, MirroredSMBus, TcpTransport

mirror = Mirror(TcpTransport("192.168.1.50", 5555))
bus = MirroredSMBus(SMBus(1), mirror, {0x3C: 0})

bus.write_i2c_block_data(0x3C, 0x00, [0xAE, 0xD5, 0x80])  # control byte 0x00 + commands
bus.write_byte_data(0x3C, 0x00, 0xAF)
