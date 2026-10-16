namespace Atc.Wpf.Tests.Hotkeys;

/// <summary>
/// Drives the chord state machine of <see cref="HotkeyService"/> directly with key strokes.
/// </summary>
public sealed class HotkeyServiceChordTests : IDisposable
{
    private readonly HotkeyService sut = new();

    public void Dispose()
        => sut.Dispose();

    [StaFact]
    public void Chord_fires_when_the_second_stroke_follows_the_first()
    {
        var fired = 0;
        sut.RegisterChord(ModifierKeys.Control, Key.K, ModifierKeys.Control, Key.S, _ => fired++);

        var firstHandled = sut.ProcessPreviewKeyDown(Key.K, ModifierKeys.Control);
        var secondHandled = sut.ProcessPreviewKeyDown(Key.S, ModifierKeys.Control);

        Assert.True(firstHandled);
        Assert.True(secondHandled);
        Assert.Equal(1, fired);
    }

    [StaTheory]
    [InlineData(Key.LeftCtrl)]
    [InlineData(Key.RightCtrl)]
    [InlineData(Key.LeftShift)]
    [InlineData(Key.RightShift)]
    [InlineData(Key.LeftAlt)]
    [InlineData(Key.RightAlt)]
    [InlineData(Key.LWin)]
    [InlineData(Key.RWin)]
    public void Modifier_key_pressed_between_the_strokes_does_not_cancel_the_chord(
        Key modifierKey)
    {
        var fired = 0;
        sut.RegisterChord(ModifierKeys.Control, Key.K, ModifierKeys.Control, Key.S, _ => fired++);

        // Ctrl+K, release Ctrl, press Ctrl again (arrives as a bare modifier key-down), then S.
        sut.ProcessPreviewKeyDown(Key.K, ModifierKeys.Control);
        var modifierHandled = sut.ProcessPreviewKeyDown(modifierKey, ModifierKeys.None);
        var secondHandled = sut.ProcessPreviewKeyDown(Key.S, ModifierKeys.Control);

        Assert.False(modifierHandled);
        Assert.True(secondHandled);
        Assert.Equal(1, fired);
    }

    [StaFact]
    public void Non_matching_second_stroke_cancels_the_chord()
    {
        var fired = 0;
        sut.RegisterChord(ModifierKeys.Control, Key.K, ModifierKeys.Control, Key.S, _ => fired++);

        sut.ProcessPreviewKeyDown(Key.K, ModifierKeys.Control);
        var wrongHandled = sut.ProcessPreviewKeyDown(Key.X, ModifierKeys.Control);
        var lateHandled = sut.ProcessPreviewKeyDown(Key.S, ModifierKeys.Control);

        Assert.False(wrongHandled);
        Assert.False(lateHandled);
        Assert.Equal(0, fired);
    }

    [StaFact]
    public void Key_that_starts_no_chord_is_not_handled()
    {
        sut.RegisterChord(ModifierKeys.Control, Key.K, ModifierKeys.Control, Key.S, _ => { });

        var handled = sut.ProcessPreviewKeyDown(Key.A, ModifierKeys.Control);

        Assert.False(handled);
    }

    [StaFact]
    public void Chord_raises_HotkeyPressed_with_its_registration()
    {
        IHotkeyRegistration? pressed = null;
        sut.HotkeyPressed += (_, e) => pressed = e.Registration;
        var registration = sut.RegisterChord(ModifierKeys.Control, Key.K, ModifierKeys.Control, Key.S, _ => { });

        sut.ProcessPreviewKeyDown(Key.K, ModifierKeys.Control);
        sut.ProcessPreviewKeyDown(Key.S, ModifierKeys.Control);

        Assert.Same(registration, pressed);
    }
}