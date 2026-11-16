namespace Atc.Wpf.Displays;

/// <summary>
/// The thread-safe model of a daisy chain of MAX7219 LED display drivers: for every device the eight digit registers (each an 8-bit
/// segment or column pattern) and the control registers (decode mode, intensity, scan limit, shutdown and display test).
/// </summary>
/// <remarks>
/// <para>
/// Device 0 is the first in the chain, the one whose DIN is wired to the microcontroller. A new buffer is in the chip's power-on state
/// (§MAX7219.2): blank digits, no decode, intensity 0, scan limit 0, shut down, display test off.
/// </para>
/// <para>
/// The buffer stores the register values; what is lit (scan limit, display test, shutdown, decode) is decided by the control that draws it
/// (§MAX7219.5). All members are thread-safe; <see cref="Changed"/> is raised on the thread that made the change, never while the buffer
/// is locked, and once when the outermost <see cref="BeginBatch"/> scope ends. A write that does not change a value raises nothing.
/// </para>
/// </remarks>
public sealed class Max7219ChainBuffer
{
    /// <summary>
    /// The largest number of devices in a chain.
    /// </summary>
    public const int MaxDeviceCount = 8;

    /// <summary>
    /// The number of digit registers of a device.
    /// </summary>
    public const int DigitCount = 8;

    /// <summary>
    /// The largest intensity value.
    /// </summary>
    public const int MaxIntensity = 15;

    /// <summary>
    /// The largest scan limit value (the highest scanned digit).
    /// </summary>
    public const int MaxScanLimit = 7;

    private readonly Lock sync = new();
    private readonly DisplayChangeNotifier notifier;
    private int deviceCount;
    private byte[] digits;
    private byte[] decodeModes;
    private byte[] intensities;
    private byte[] scanLimits;
    private bool[] shutdowns;
    private bool[] displayTests;

    /// <summary>
    /// Initializes a new instance of the <see cref="Max7219ChainBuffer"/> class in the power-on state.
    /// </summary>
    /// <param name="deviceCount">The number of devices in the chain, 1 to <see cref="MaxDeviceCount"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="deviceCount"/> is out of range.</exception>
    public Max7219ChainBuffer(int deviceCount = 4)
    {
        ValidateDeviceCount(deviceCount);

        this.deviceCount = deviceCount;
        digits = new byte[deviceCount * DigitCount];
        decodeModes = new byte[deviceCount];
        intensities = new byte[deviceCount];
        scanLimits = new byte[deviceCount];
        shutdowns = CreateShutdowns(deviceCount);
        displayTests = new bool[deviceCount];
        notifier = new DisplayChangeNotifier(sync, RaiseChanged);
    }

    /// <summary>
    /// Occurs when a register value or the number of devices has changed; once per batch, or once per change outside a batch.
    /// </summary>
    public event EventHandler? Changed;

    /// <summary>
    /// Gets the number of devices in the chain.
    /// </summary>
    public int DeviceCount
    {
        get
        {
            lock (sync)
            {
                return deviceCount;
            }
        }
    }

    /// <summary>
    /// Changes the number of devices; the registers of the devices that exist before and after are kept, new devices are in the power-on
    /// state.
    /// </summary>
    /// <param name="deviceCount">The number of devices in the chain, 1 to <see cref="MaxDeviceCount"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="deviceCount"/> is out of range.</exception>
    public void Resize(int deviceCount)
    {
        ValidateDeviceCount(deviceCount);

        lock (sync)
        {
            if (this.deviceCount == deviceCount)
            {
                return;
            }

            var kept = System.Math.Min(this.deviceCount, deviceCount);
            var newDigits = new byte[deviceCount * DigitCount];
            Array.Copy(digits, newDigits, kept * DigitCount);
            var newDecodeModes = new byte[deviceCount];
            Array.Copy(decodeModes, newDecodeModes, kept);
            var newIntensities = new byte[deviceCount];
            Array.Copy(intensities, newIntensities, kept);
            var newScanLimits = new byte[deviceCount];
            Array.Copy(scanLimits, newScanLimits, kept);
            var newShutdowns = CreateShutdowns(deviceCount);
            Array.Copy(shutdowns, newShutdowns, kept);
            var newDisplayTests = new bool[deviceCount];
            Array.Copy(displayTests, newDisplayTests, kept);

            digits = newDigits;
            decodeModes = newDecodeModes;
            intensities = newIntensities;
            scanLimits = newScanLimits;
            shutdowns = newShutdowns;
            displayTests = newDisplayTests;
            this.deviceCount = deviceCount;
        }

        notifier.Notify();
    }

    /// <summary>
    /// Starts a batch: <see cref="Changed"/> is raised once, if anything changed, when the last open batch is disposed.
    /// </summary>
    /// <returns>The scope to dispose to end the batch.</returns>
    public IDisposable BeginBatch()
        => notifier.BeginBatch();

    /// <summary>
    /// Gets the value of a digit register.
    /// </summary>
    /// <param name="device">The device, 0 (nearest the microcontroller) to <see cref="DeviceCount"/> - 1.</param>
    /// <param name="digit">The digit, 0 to 7 (register address 1 to 8).</param>
    /// <returns>The register value: in no-decode mode bit 7 = DP, bit 6 = A ... bit 0 = G (§MAX7219.3), in decode mode the Code B code.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The device or digit is out of range.</exception>
    public byte GetDigit(
        int device,
        int digit)
    {
        lock (sync)
        {
            ThrowIfOutside(device, digit);

            return digits[(device * DigitCount) + digit];
        }
    }

    /// <summary>
    /// Sets the value of a digit register.
    /// </summary>
    /// <param name="device">The device, 0 to <see cref="DeviceCount"/> - 1.</param>
    /// <param name="digit">The digit, 0 to 7.</param>
    /// <param name="value">The register value.</param>
    /// <exception cref="ArgumentOutOfRangeException">The device or digit is out of range.</exception>
    public void SetDigit(
        int device,
        int digit,
        byte value)
    {
        lock (sync)
        {
            ThrowIfOutside(device, digit);

            ref var slot = ref digits[(device * DigitCount) + digit];
            if (slot == value)
            {
                return;
            }

            slot = value;
        }

        notifier.Notify();
    }

    /// <summary>
    /// Gets the decode mode register of a device: bit n set = digit n uses Code B decoding (§MAX7219.4).
    /// </summary>
    /// <param name="device">The device, 0 to <see cref="DeviceCount"/> - 1.</param>
    /// <returns>The register value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The device is out of range.</exception>
    public byte GetDecodeMode(int device)
    {
        lock (sync)
        {
            ThrowIfOutside(device);

            return decodeModes[device];
        }
    }

    /// <summary>
    /// Sets the decode mode register of a device.
    /// </summary>
    /// <param name="device">The device, 0 to <see cref="DeviceCount"/> - 1.</param>
    /// <param name="value">The register value, one bit per digit.</param>
    /// <exception cref="ArgumentOutOfRangeException">The device is out of range.</exception>
    public void SetDecodeMode(
        int device,
        byte value)
    {
        lock (sync)
        {
            ThrowIfOutside(device);

            if (decodeModes[device] == value)
            {
                return;
            }

            decodeModes[device] = value;
        }

        notifier.Notify();
    }

    /// <summary>
    /// Gets the intensity register of a device, 0 to <see cref="MaxIntensity"/> (duty cycle (2n + 1) / 32, §MAX7219.5).
    /// </summary>
    /// <param name="device">The device, 0 to <see cref="DeviceCount"/> - 1.</param>
    /// <returns>The intensity.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The device is out of range.</exception>
    public byte GetIntensity(int device)
    {
        lock (sync)
        {
            ThrowIfOutside(device);

            return intensities[device];
        }
    }

    /// <summary>
    /// Sets the intensity register of a device.
    /// </summary>
    /// <param name="device">The device, 0 to <see cref="DeviceCount"/> - 1.</param>
    /// <param name="value">The intensity, 0 to <see cref="MaxIntensity"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException">The device or value is out of range.</exception>
    public void SetIntensity(
        int device,
        byte value)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(value, MaxIntensity);

        lock (sync)
        {
            ThrowIfOutside(device);

            if (intensities[device] == value)
            {
                return;
            }

            intensities[device] = value;
        }

        notifier.Notify();
    }

    /// <summary>
    /// Gets the scan limit register of a device, 0 to <see cref="MaxScanLimit"/>: the highest digit that is scanned.
    /// </summary>
    /// <param name="device">The device, 0 to <see cref="DeviceCount"/> - 1.</param>
    /// <returns>The scan limit.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The device is out of range.</exception>
    public byte GetScanLimit(int device)
    {
        lock (sync)
        {
            ThrowIfOutside(device);

            return scanLimits[device];
        }
    }

    /// <summary>
    /// Sets the scan limit register of a device.
    /// </summary>
    /// <param name="device">The device, 0 to <see cref="DeviceCount"/> - 1.</param>
    /// <param name="value">The scan limit, 0 to <see cref="MaxScanLimit"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException">The device or value is out of range.</exception>
    public void SetScanLimit(
        int device,
        byte value)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(value, MaxScanLimit);

        lock (sync)
        {
            ThrowIfOutside(device);

            if (scanLimits[device] == value)
            {
                return;
            }

            scanLimits[device] = value;
        }

        notifier.Notify();
    }

    /// <summary>
    /// Gets a value indicating whether a device is in shutdown mode (the display is blank, the registers are kept).
    /// </summary>
    /// <param name="device">The device, 0 to <see cref="DeviceCount"/> - 1.</param>
    /// <returns><see langword="true"/> when the device is shut down.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The device is out of range.</exception>
    public bool IsShutdown(int device)
    {
        lock (sync)
        {
            ThrowIfOutside(device);

            return shutdowns[device];
        }
    }

    /// <summary>
    /// Sets whether a device is in shutdown mode.
    /// </summary>
    /// <param name="device">The device, 0 to <see cref="DeviceCount"/> - 1.</param>
    /// <param name="isShutdown"><see langword="true"/> for shutdown mode, <see langword="false"/> for normal operation.</param>
    /// <exception cref="ArgumentOutOfRangeException">The device is out of range.</exception>
    public void SetShutdown(
        int device,
        bool isShutdown)
    {
        lock (sync)
        {
            ThrowIfOutside(device);

            if (shutdowns[device] == isShutdown)
            {
                return;
            }

            shutdowns[device] = isShutdown;
        }

        notifier.Notify();
    }

    /// <summary>
    /// Gets a value indicating whether the display test of a device is on (every LED lit, overriding shutdown).
    /// </summary>
    /// <param name="device">The device, 0 to <see cref="DeviceCount"/> - 1.</param>
    /// <returns><see langword="true"/> when the display test is on.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The device is out of range.</exception>
    public bool IsDisplayTest(int device)
    {
        lock (sync)
        {
            ThrowIfOutside(device);

            return displayTests[device];
        }
    }

    /// <summary>
    /// Sets whether the display test of a device is on.
    /// </summary>
    /// <param name="device">The device, 0 to <see cref="DeviceCount"/> - 1.</param>
    /// <param name="isDisplayTest"><see langword="true"/> to light every LED.</param>
    /// <exception cref="ArgumentOutOfRangeException">The device is out of range.</exception>
    public void SetDisplayTest(
        int device,
        bool isDisplayTest)
    {
        lock (sync)
        {
            ThrowIfOutside(device);

            if (displayTests[device] == isDisplayTest)
            {
                return;
            }

            displayTests[device] = isDisplayTest;
        }

        notifier.Notify();
    }

    /// <summary>
    /// Copies the whole state under one lock, for a renderer.
    /// </summary>
    /// <returns>The snapshot.</returns>
    internal Max7219ChainSnapshot CopyState()
    {
        lock (sync)
        {
            return new Max7219ChainSnapshot(
                deviceCount,
                (byte[])digits.Clone(),
                (byte[])decodeModes.Clone(),
                (byte[])intensities.Clone(),
                (byte[])scanLimits.Clone(),
                (bool[])shutdowns.Clone(),
                (bool[])displayTests.Clone());
        }
    }

    // The power-on state of every device (§MAX7219.2); the digit registers are cleared (⚠️ their power-on content is not stated).
    internal void ResetToPowerOn()
    {
        lock (sync)
        {
            var isChanged = digits.Any(static d => d != 0) ||
                            decodeModes.Any(static d => d != 0) ||
                            intensities.Any(static d => d != 0) ||
                            scanLimits.Any(static d => d != 0) ||
                            shutdowns.Any(static d => !d) ||
                            displayTests.Any(static d => d);
            if (!isChanged)
            {
                return;
            }

            Array.Clear(digits);
            Array.Clear(decodeModes);
            Array.Clear(intensities);
            Array.Clear(scanLimits);
            Array.Fill(shutdowns, true);
            Array.Clear(displayTests);
        }

        notifier.Notify();
    }

    private static bool[] CreateShutdowns(int deviceCount)
    {
        var flags = new bool[deviceCount];
        Array.Fill(flags, true);
        return flags;
    }

    private static void ValidateDeviceCount(int deviceCount)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(deviceCount, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(deviceCount, MaxDeviceCount);
    }

    // Must be called with the lock held.
    private void ThrowIfOutside(int device)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(device);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(device, deviceCount);
    }

    // Must be called with the lock held.
    private void ThrowIfOutside(
        int device,
        int digit)
    {
        ThrowIfOutside(device);
        ArgumentOutOfRangeException.ThrowIfNegative(digit);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(digit, DigitCount);
    }

    private void RaiseChanged()
        => Changed?.Invoke(this, EventArgs.Empty);
}

/// <summary>
/// A copy of the state of a <see cref="Max7219ChainBuffer"/>, taken under one lock; the digit array holds 8 values per device.
/// </summary>
internal readonly record struct Max7219ChainSnapshot(
    int DeviceCount,
    byte[] Digits,
    byte[] DecodeModes,
    byte[] Intensities,
    byte[] ScanLimits,
    bool[] Shutdowns,
    bool[] DisplayTests);