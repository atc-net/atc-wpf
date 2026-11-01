namespace Atc.Wpf.Tests.Hotkeys;

/// <summary>
/// Local hotkeys that WPF's <see cref="KeyGesture"/> refuses (letters and digits with no modifier
/// or Shift only), and registrations made from a background thread.
/// </summary>
public sealed class HotkeyServiceLocalGestureTests : IDisposable
{
    private readonly HotkeyService sut = new();
    private readonly Window window = new();

    public void Dispose()
    {
        sut.Dispose();
        window.Close();
    }

    [StaFact]
    public void Register_letter_without_modifier_while_listening_does_not_throw_and_fires_on_key_down()
    {
        var fired = 0;
        sut.StartListening(window);

        var exception = Record.Exception(() => sut.Register(ModifierKeys.None, Key.A, _ => fired++));

        Assert.Null(exception);
        Assert.True(sut.IsListening);
        Assert.Single(sut.Registrations);
        Assert.True(sut.ProcessPreviewKeyDown(Key.A, ModifierKeys.None));
        Assert.Equal(1, fired);
        Assert.False(sut.ProcessPreviewKeyDown(Key.B, ModifierKeys.None));
        Assert.Equal(1, fired);
    }

    [StaFact]
    public void StartListening_with_a_shift_digit_registration_sets_up_the_other_hotkeys()
    {
        var fired = 0;
        sut.Register(ModifierKeys.Shift, Key.D1, _ => fired++);
        sut.Register(ModifierKeys.Control, Key.B, _ => { });

        var exception = Record.Exception(() => sut.StartListening(window));

        Assert.Null(exception);
        Assert.True(sut.IsListening);
        Assert.Single(window.InputBindings);
        Assert.True(sut.ProcessPreviewKeyDown(Key.D1, ModifierKeys.Shift));
        Assert.Equal(1, fired);
    }

    [StaFact]
    public void Letter_without_modifier_does_not_fire_while_typing_in_a_text_input()
    {
        var fired = 0;
        sut.StartListening(window);
        sut.Register(ModifierKeys.None, Key.A, _ => fired++);

        Assert.False(sut.ProcessPreviewKeyDown(Key.A, ModifierKeys.None, isTextInputFocused: true));
        Assert.Equal(0, fired);
    }

    [StaFact]
    public void Unregister_of_a_letter_without_modifier_stops_it_firing()
    {
        var fired = 0;
        sut.StartListening(window);
        var registration = sut.Register(ModifierKeys.None, Key.A, _ => fired++);

        sut.Unregister(registration);

        Assert.False(sut.ProcessPreviewKeyDown(Key.A, ModifierKeys.None));
        Assert.Equal(0, fired);
    }

    [StaFact]
    public void Register_and_Unregister_from_a_background_thread_update_the_window_on_its_own_thread()
    {
        sut.StartListening(window);

        var registerTask = Task.Run(() => sut.Register(ModifierKeys.Control, Key.B, _ => { }));
        Assert.True(DispatcherPump.Until(() => registerTask.IsCompleted, TimeSpan.FromSeconds(10)));
        Assert.Null(registerTask.Exception);
        Assert.Single(window.InputBindings);

        var registration = registerTask.Result;
        var unregisterTask = Task.Run(() => sut.Unregister(registration));
        Assert.True(DispatcherPump.Until(() => unregisterTask.IsCompleted, TimeSpan.FromSeconds(10)));
        Assert.Null(unregisterTask.Exception);
        Assert.Empty(window.InputBindings);
    }
}