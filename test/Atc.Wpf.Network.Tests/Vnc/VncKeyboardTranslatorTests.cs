namespace Atc.Wpf.Network.Tests.Vnc;

public sealed class VncKeyboardTranslatorTests
{
    private const uint LeftCtrl = 0xFFE3;
    private const uint LeftAlt = 0xFFE9;
    private const uint RightAlt = 0xFFEA;
    private const uint LeftShift = 0xFFE1;

    private readonly VncKeyboardTranslator sut = new();

    [Fact]
    public void PrintableKey_IsSentFromTheTypedCharacter()
    {
        // A Danish keyboard: the key US calls OemSemicolon types 'æ'.
        Assert.Empty(sut.KeyDown(Key.OemSemicolon));
        var events = sut.TextInput("æ");
        Assert.Empty(sut.KeyUp(Key.OemSemicolon));

        Assert.Equal([new VncKeyEvent(0x00E6, true), new VncKeyEvent(0x00E6, false)], events);
    }

    [Fact]
    public void ShiftedLetter_IsSentAsTypedWithShiftHeld()
    {
        var events = new List<VncKeyEvent>();
        events.AddRange(sut.KeyDown(Key.LeftShift));
        events.AddRange(sut.KeyDown(Key.A));
        events.AddRange(sut.TextInput("A"));
        events.AddRange(sut.KeyUp(Key.A));
        events.AddRange(sut.KeyUp(Key.LeftShift));

        Assert.Equal(
            [
                new VncKeyEvent(LeftShift, true),
                new VncKeyEvent(0x0041, true),
                new VncKeyEvent(0x0041, false),
                new VncKeyEvent(LeftShift, false),
            ],
            events);
    }

    [Fact]
    public void AltGr_SendsTheCharacterWithoutCtrlOrAltHeld()
    {
        // Windows reports AltGr as a synthetic LeftCtrl followed by RightAlt.
        var events = new List<VncKeyEvent>();
        events.AddRange(sut.KeyDown(Key.LeftCtrl));
        events.AddRange(sut.KeyDown(Key.RightAlt));
        events.AddRange(sut.KeyDown(Key.D2));
        events.AddRange(sut.TextInput("@"));
        events.AddRange(sut.KeyUp(Key.D2));
        events.AddRange(sut.KeyUp(Key.RightAlt));
        events.AddRange(sut.KeyUp(Key.LeftCtrl));

        Assert.Equal(
            [
                new VncKeyEvent(LeftCtrl, true),
                new VncKeyEvent(LeftCtrl, false),
                new VncKeyEvent(0x0040, true),
                new VncKeyEvent(0x0040, false),
            ],
            events);
        Assert.DoesNotContain(events, e => e.KeySym is RightAlt or LeftAlt);
    }

    [Fact]
    public void AltGr_AutoRepeatOfTheModifiers_SendsNothing()
    {
        sut.KeyDown(Key.LeftCtrl);
        sut.KeyDown(Key.RightAlt);

        Assert.Empty(sut.KeyDown(Key.LeftCtrl));
        Assert.Empty(sut.KeyDown(Key.RightAlt));
    }

    [Fact]
    public void AltGr_AfterRelease_CtrlWorksAgain()
    {
        sut.KeyDown(Key.LeftCtrl);
        sut.KeyDown(Key.RightAlt);
        sut.KeyUp(Key.RightAlt);
        sut.KeyUp(Key.LeftCtrl);

        Assert.Equal([new VncKeyEvent(LeftCtrl, true)], sut.KeyDown(Key.LeftCtrl));
        Assert.Equal([new VncKeyEvent(0x0063, true)], sut.KeyDown(Key.C));
    }

    [Fact]
    public void CtrlLetter_IsSentFromTheKey()
    {
        // With Ctrl held WPF reports control text, not "c", so the key path is used.
        var events = new List<VncKeyEvent>();
        events.AddRange(sut.KeyDown(Key.LeftCtrl));
        events.AddRange(sut.KeyDown(Key.C));
        events.AddRange(sut.TextInput("\u0003"));
        events.AddRange(sut.KeyUp(Key.C));
        events.AddRange(sut.KeyUp(Key.LeftCtrl));

        Assert.Equal(
            [
                new VncKeyEvent(LeftCtrl, true),
                new VncKeyEvent(0x0063, true),
                new VncKeyEvent(0x0063, false),
                new VncKeyEvent(LeftCtrl, false),
            ],
            events);
    }

    [Fact]
    public void AltLetter_IsSentFromTheKey()
    {
        sut.KeyDown(Key.LeftAlt);

        Assert.Equal([new VncKeyEvent(0x0066, true)], sut.KeyDown(Key.F));
        Assert.Empty(sut.TextInput("f"));
    }

    [Fact]
    public void NumPadDigit_IsSentOnceAsKeypadKeySym()
    {
        var events = new List<VncKeyEvent>();
        events.AddRange(sut.KeyDown(Key.NumPad1));
        events.AddRange(sut.TextInput("1"));
        events.AddRange(sut.KeyUp(Key.NumPad1));

        Assert.Equal([new VncKeyEvent(0xFFB1, true), new VncKeyEvent(0xFFB1, false)], events);
    }

    [Fact]
    public void SpecialKey_IsSentFromTheKey_AndItsControlTextIsIgnored()
    {
        var events = new List<VncKeyEvent>();
        events.AddRange(sut.KeyDown(Key.Return));
        events.AddRange(sut.TextInput("\r"));
        events.AddRange(sut.KeyUp(Key.Return));

        Assert.Equal([new VncKeyEvent(0xFF0D, true), new VncKeyEvent(0xFF0D, false)], events);
    }

    [Fact]
    public void DeadKey_SendsTheComposedCharacter()
    {
        var events = new List<VncKeyEvent>();
        events.AddRange(sut.KeyDown(Key.Oem4));
        events.AddRange(sut.KeyUp(Key.Oem4));
        events.AddRange(sut.KeyDown(Key.E));
        events.AddRange(sut.TextInput("é"));
        events.AddRange(sut.KeyUp(Key.E));

        Assert.Equal([new VncKeyEvent(0x00E9, true), new VncKeyEvent(0x00E9, false)], events);
    }

    [Fact]
    public void ReleaseAll_ReleasesEveryKeyStillDown()
    {
        sut.KeyDown(Key.LeftShift);
        sut.KeyDown(Key.F5);

        var events = sut.ReleaseAll();

        Assert.Equal(2, events.Count);
        Assert.Contains(new VncKeyEvent(LeftShift, false), events);
        Assert.Contains(new VncKeyEvent(0xFFC2, false), events);
        Assert.Empty(sut.KeyUp(Key.LeftShift));
    }
}