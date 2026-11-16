namespace Atc.Wpf.Displays.Tests;

public sealed class Max7219ChainBufferTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(9)]
    public void Constructor_DeviceCountOutOfRange_Throws(int count)
        => Assert.Throws<ArgumentOutOfRangeException>(() => new Max7219ChainBuffer(count));

    [Fact]
    public void Constructor_DefaultsToFourDevicesInThePowerOnState()
    {
        // ref: §MAX7219.2.
        var buffer = new Max7219ChainBuffer();

        Assert.Equal(4, buffer.DeviceCount);
        for (var device = 0; device < 4; device++)
        {
            Assert.True(buffer.IsShutdown(device));
            Assert.False(buffer.IsDisplayTest(device));
            Assert.Equal(0, buffer.GetIntensity(device));
            Assert.Equal(0, buffer.GetScanLimit(device));
            Assert.Equal(0, buffer.GetDecodeMode(device));
            Assert.Equal(0, buffer.GetDigit(device, 7));
        }
    }

    [Fact]
    public void Accessors_OutOfRange_Throw()
    {
        var buffer = new Max7219ChainBuffer(2);

        Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetDigit(-1, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetDigit(2, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetDigit(0, 8));
        Assert.Throws<ArgumentOutOfRangeException>(() => buffer.SetDigit(0, -1, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => buffer.SetIntensity(0, 16));
        Assert.Throws<ArgumentOutOfRangeException>(() => buffer.SetScanLimit(0, 8));
        Assert.Throws<ArgumentOutOfRangeException>(() => buffer.SetShutdown(2, false));
        Assert.Throws<ArgumentOutOfRangeException>(() => buffer.IsDisplayTest(5));
    }

    [Fact]
    public void Setters_RoundTrip_AndRaiseChangedOnlyOnAChange()
    {
        var buffer = new Max7219ChainBuffer(2);
        var changes = 0;
        buffer.Changed += (_, _) => changes++;

        buffer.SetDigit(1, 3, 0x42);
        buffer.SetDigit(1, 3, 0x42);
        buffer.SetDecodeMode(0, 0xFF);
        buffer.SetIntensity(0, 15);
        buffer.SetScanLimit(0, 7);
        buffer.SetShutdown(0, false);
        buffer.SetDisplayTest(1, true);
        buffer.SetShutdown(0, false);

        Assert.Equal(6, changes);
        Assert.Equal(0x42, buffer.GetDigit(1, 3));
        Assert.Equal(0xFF, buffer.GetDecodeMode(0));
        Assert.Equal(15, buffer.GetIntensity(0));
        Assert.Equal(7, buffer.GetScanLimit(0));
        Assert.False(buffer.IsShutdown(0));
        Assert.True(buffer.IsDisplayTest(1));
    }

    [Fact]
    public void BeginBatch_RaisesChangedOnceWhenTheOutermostScopeEnds()
    {
        var buffer = new Max7219ChainBuffer(2);
        var changes = 0;
        buffer.Changed += (_, _) => changes++;

        using (buffer.BeginBatch())
        {
            using (buffer.BeginBatch())
            {
                buffer.SetDigit(0, 0, 1);
                buffer.SetDigit(1, 0, 2);
            }

            Assert.Equal(0, changes);
        }

        Assert.Equal(1, changes);

        using (buffer.BeginBatch())
        {
            buffer.SetDigit(0, 0, 1);
        }

        Assert.Equal(1, changes);
    }

    [Fact]
    public void Resize_KeepsTheRemainingDevicesAndAddsPowerOnDevices()
    {
        var buffer = new Max7219ChainBuffer(2);
        buffer.SetDigit(1, 2, 0x77);
        buffer.SetShutdown(1, false);
        var changes = 0;
        buffer.Changed += (_, _) => changes++;

        buffer.Resize(4);
        buffer.Resize(4);

        Assert.Equal(1, changes);
        Assert.Equal(4, buffer.DeviceCount);
        Assert.Equal(0x77, buffer.GetDigit(1, 2));
        Assert.False(buffer.IsShutdown(1));
        Assert.True(buffer.IsShutdown(3));

        buffer.Resize(1);

        Assert.Equal(1, buffer.DeviceCount);
        Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetDigit(1, 2));
        Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Resize(9));
    }

    [Fact]
    public void ChangedHandler_MayReadTheBuffer_WithoutDeadlock()
    {
        var buffer = new Max7219ChainBuffer(1);
        byte seen = 0;
        buffer.Changed += (_, _) => seen = buffer.GetDigit(0, 0);

        buffer.SetDigit(0, 0, 9);

        Assert.Equal(9, seen);
    }

    [Fact]
    public void ConcurrentWrites_AreSafe()
    {
        var buffer = new Max7219ChainBuffer(8);

        Parallel.For(
            0,
            8,
            device =>
            {
                for (var i = 0; i < 500; i++)
                {
                    buffer.SetDigit(device, i % 8, (byte)i);
                    _ = buffer.GetDigit(device, i % 8);
                }
            });

        for (var device = 0; device < 8; device++)
        {
            Assert.Equal((byte)(499 & 0xFF), buffer.GetDigit(device, 499 % 8));
        }
    }

    [Fact]
    public void CopyState_ReturnsAnIndependentSnapshot()
    {
        var buffer = new Max7219ChainBuffer(2);
        buffer.SetDigit(1, 1, 0x11);
        buffer.SetIntensity(1, 5);

        var snapshot = buffer.CopyState();
        buffer.SetDigit(1, 1, 0x22);

        Assert.Equal(2, snapshot.DeviceCount);
        Assert.Equal(0x11, snapshot.Digits[(1 * 8) + 1]);
        Assert.Equal(5, snapshot.Intensities[1]);
        Assert.True(snapshot.Shutdowns[0]);
    }
}