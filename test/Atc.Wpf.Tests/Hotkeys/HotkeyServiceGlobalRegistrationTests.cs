namespace Atc.Wpf.Tests.Hotkeys;

/// <summary>
/// Exercises the real Win32 RegisterHotKey: a global hotkey can only be owned by one window at a time,
/// so a second service claiming the same combination must report the failure instead of returning a
/// registration that never fires.
/// </summary>
public sealed class HotkeyServiceGlobalRegistrationTests
{
    private const ModifierKeys UnlikelyModifiers = ModifierKeys.Control | ModifierKeys.Alt | ModifierKeys.Shift;
    private const Key UnlikelyKey = Key.F23;

    [StaFact]
    public void Register_GlobalHotkeyAlreadyOwnedElsewhere_RaisesRegistrationFailed()
    {
        using var owner = new HotkeyService();
        using var contender = new HotkeyService();
        owner.StartListening(new Window());
        contender.StartListening(new Window());

        var ownerFailures = new List<HotkeyRegistrationFailedEventArgs>();
        var contenderFailures = new List<HotkeyRegistrationFailedEventArgs>();
        owner.RegistrationFailed += (_, e) => ownerFailures.Add(e);
        contender.RegistrationFailed += (_, e) => contenderFailures.Add(e);

        owner.Register(UnlikelyModifiers, UnlikelyKey, _ => { }, "owner", HotkeyScope.Global);
        SkipIfOwnerCouldNotRegister(ownerFailures);
        var contended = contender.Register(UnlikelyModifiers, UnlikelyKey, _ => { }, "contender", HotkeyScope.Global);

        contenderFailures.Should().ContainSingle();
        contenderFailures[0].Registration.Should().BeSameAs(contended);
        contenderFailures[0].ErrorCode.Should().Be(1409); // ERROR_HOTKEY_ALREADY_REGISTERED
    }

    [StaFact]
    public void StartListening_GlobalHotkeyAlreadyOwnedElsewhere_RaisesRegistrationFailed()
    {
        using var owner = new HotkeyService();
        using var contender = new HotkeyService();
        owner.StartListening(new Window());
        var ownerFailures = new List<HotkeyRegistrationFailedEventArgs>();
        owner.RegistrationFailed += (_, e) => ownerFailures.Add(e);
        owner.Register(UnlikelyModifiers, UnlikelyKey, _ => { }, "owner", HotkeyScope.Global);
        SkipIfOwnerCouldNotRegister(ownerFailures);

        var contended = contender.Register(UnlikelyModifiers, UnlikelyKey, _ => { }, "contender", HotkeyScope.Global);
        var contenderFailures = new List<HotkeyRegistrationFailedEventArgs>();
        contender.RegistrationFailed += (_, e) => contenderFailures.Add(e);

        contender.StartListening(new Window());

        contenderFailures.Should().ContainSingle();
        contenderFailures[0].Registration.Should().BeSameAs(contended);
    }

    private static void SkipIfOwnerCouldNotRegister(
        List<HotkeyRegistrationFailedEventArgs> ownerFailures)
    {
        // Another application on this machine already owns the combination, or the session does not allow
        // global hotkeys at all; the contention case cannot be set up here.
        if (ownerFailures.Count > 0)
        {
            Assert.Skip($"Could not claim the global hotkey for the owner (Win32 error {ownerFailures[0].ErrorCode}).");
        }
    }
}