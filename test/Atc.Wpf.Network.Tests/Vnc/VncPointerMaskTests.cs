namespace Atc.Wpf.Network.Tests.Vnc;

public sealed class VncPointerMaskTests
{
    [Theory]
    [InlineData(false, false, false, 0)]
    [InlineData(true, false, false, 1)]
    [InlineData(false, true, false, 2)]
    [InlineData(false, false, true, 4)]
    [InlineData(true, false, true, 5)]
    [InlineData(true, true, true, 7)]
    public void FromButtons_SetsOneBitPerHeldButton(
        bool left,
        bool middle,
        bool right,
        byte expected)
        => Assert.Equal(expected, VncPointerMask.FromButtons(left, middle, right));

    [Fact]
    public void ForButtonChange_RightPressedWhileLeftHeld_KeepsLeft()
        => Assert.Equal(
            5,
            VncPointerMask.ForButtonChange(left: true, middle: false, right: false, MouseButton.Right, pressed: true));

    [Fact]
    public void ForButtonChange_RightReleasedWhileLeftHeld_KeepsLeft()
        => Assert.Equal(
            1,
            VncPointerMask.ForButtonChange(left: true, middle: false, right: true, MouseButton.Right, pressed: false));

    [Fact]
    public void ForButtonChange_LastButtonReleased_IsZero()
        => Assert.Equal(
            0,
            VncPointerMask.ForButtonChange(left: true, middle: false, right: false, MouseButton.Left, pressed: false));

    [Theory]
    [InlineData(120, 0, 8, 1)]
    [InlineData(-120, 0, 16, 1)]
    [InlineData(360, 1, 8, 3)]
    [InlineData(-240, 4, 16, 2)]
    [InlineData(60, 0, 8, 1)]
    public void GetWheel_MapsNotchesToButtonsFourAndFive(
        int delta,
        byte heldMask,
        byte expectedBit,
        int expectedClicks)
    {
        var (wheelMask, clicks) = VncPointerMask.GetWheel(delta, heldMask);

        Assert.Equal((byte)(heldMask | expectedBit), wheelMask);
        Assert.Equal(expectedClicks, clicks);
    }

    [Fact]
    public void GetWheel_ZeroDelta_HasNoClicks()
        => Assert.Equal(0, VncPointerMask.GetWheel(0, 0).Clicks);
}