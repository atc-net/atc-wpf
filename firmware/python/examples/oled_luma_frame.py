"""Mirror a luma.oled SSD1306 (I2C) to the WPF OledDisplay, plus a Describe and one Pillow frame."""
from luma.core.interface.serial import i2c
from luma.oled.device import ssd1306

from atc_display_mirror import (Chip, DisplayKind, Mirror, MirroredSerialInterface, UdpTransport)
from PIL import Image, ImageDraw

mirror = Mirror(UdpTransport("192.168.1.50", 5556))  # the PC running DisplayMirrorReceiver
mirror.describe(0, DisplayKind.MONO_OLED, Chip.SSD1306, 128, 64, "Pi OLED")

serial = MirroredSerialInterface(i2c(port=1, address=0x3C), mirror, display_id=0)
device = ssd1306(serial)  # init commands and every refresh are mirrored (Commands / Data)

img = Image.new("1", (128, 64))
ImageDraw.Draw(img).text((8, 24), "Hello from Pi", fill=1)
device.display(img)

# Alternative without luma: send the picture as one frame
mirror.send_image(0, img, DisplayKind.MONO_OLED)
