namespace Atc.Wpf.Tests.Hotkeys;

// Apps register their hotkeys in code and then load the user's saved bindings: loading must rebind or skip, never duplicate.
public sealed class HotkeyServiceLoadBindingsTests : IDisposable
{
    private readonly HotkeyService sut = new();
    private readonly string filePath = Path.Combine(Path.GetTempPath(), $"hotkey-load-{Guid.NewGuid():N}.json");

    public void Dispose()
    {
        sut.Dispose();
        File.Delete(filePath);
    }

    [StaFact]
    public void Saved_bindings_loaded_after_registering_in_code_do_not_duplicate()
    {
        sut.Register(ModifierKeys.Control, Key.S, _ => { }, "Save");
        sut.SaveBindings(filePath);

        sut.LoadBindings(filePath);

        var registration = Assert.Single(sut.Registrations);
        Assert.Equal("Save", registration.Description);
    }

    [StaFact]
    public void A_saved_binding_with_new_keys_rebinds_the_registration_with_that_description()
    {
        var pressed = 0;
        sut.Register(ModifierKeys.Control, Key.S, _ => pressed++, "Save");
        Write("""[ { "Modifiers": "Control, Shift", "Key": "S", "Description": "Save", "Scope": "Local" } ]""");

        sut.LoadBindings(filePath);

        var registration = Assert.Single(sut.Registrations);
        Assert.Equal(ModifierKeys.Control | ModifierKeys.Shift, registration.Modifiers);
        Assert.Equal(Key.S, registration.Key);
        ((HotkeyRegistration)registration).Callback(new HotkeyPressedEventArgs(registration));
        Assert.Equal(1, pressed);
    }

    [StaFact]
    public void A_saved_binding_for_keys_already_registered_is_skipped_without_a_conflict()
    {
        var conflicts = 0;
        sut.ConflictDetected += (_, _) => conflicts++;
        sut.Register(ModifierKeys.Control, Key.O, _ => { }, "Open");
        Write("""[ { "Modifiers": "Control", "Key": "O", "Description": "Open recent", "Scope": "Local" } ]""");

        sut.LoadBindings(filePath);

        Assert.Equal("Open", Assert.Single(sut.Registrations).Description);
        Assert.Equal(0, conflicts);
    }

    [StaFact]
    public void A_saved_binding_with_an_unknown_description_and_free_keys_is_added()
    {
        sut.Register(ModifierKeys.Control, Key.S, _ => { }, "Save");
        Write("""[ { "Modifiers": "Control", "Key": "P", "Description": "Print", "Scope": "Local" } ]""");

        sut.LoadBindings(filePath);

        Assert.Equal(["Save", "Print"], sut.Registrations.Select(x => x.Description), StringComparer.Ordinal);
    }

    [StaFact]
    public void A_saved_chord_with_new_keys_rebinds_the_chord_with_that_description()
    {
        sut.RegisterChord(ModifierKeys.Control, Key.K, ModifierKeys.Control, Key.C, _ => { }, "Comment");
        Write("""[ { "Modifiers": "Control", "Key": "K", "SecondModifiers": "Control", "SecondKey": "D", "Description": "Comment", "Scope": "Local" } ]""");

        sut.LoadBindings(filePath);

        var registration = Assert.Single(sut.Registrations);
        Assert.NotNull(registration.Chord);
        Assert.Equal(Key.D, registration.Chord!.SecondKey);
    }

    private void Write(string json)
        => File.WriteAllText(filePath, json);
}