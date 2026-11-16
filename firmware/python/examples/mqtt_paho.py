"""Publish mirror packets to an MQTT topic with paho-mqtt (pip install paho-mqtt)."""
import paho.mqtt.client as mqtt

from atc_display_mirror import CallbackTransport, Chip, DisplayKind, Mirror

client = mqtt.Client()
client.connect("broker.local", 1883)
client.loop_start()

mirror = Mirror(CallbackTransport(lambda packet: client.publish("display/0", packet)))
mirror.describe(0, DisplayKind.MONO_OLED, Chip.SSD1306, 128, 64, "MQTT OLED")
mirror.send_commands(0, bytes([0xAE, 0xD5, 0x80, 0xAF]))
