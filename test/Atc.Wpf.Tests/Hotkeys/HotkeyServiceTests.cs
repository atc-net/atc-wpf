namespace Atc.Wpf.Tests.Hotkeys;

public sealed class HotkeyServiceTests : IDisposable
{
    private readonly HotkeyService sut = new();

    [StaFact]
    public void Register_AddsToRegistrations()
    {
        // Arrange & Act
        sut.Register(
            ModifierKeys.Control,
            Key.A,
            _ => { },
            "Test hotkey");

        // Assert
        sut.Registrations.Should().HaveCount(1);
        sut.Registrations[0].Description.Should().Be("Test hotkey");
        sut.Registrations[0].Modifiers.Should().Be(ModifierKeys.Control);
        sut.Registrations[0].Key.Should().Be(Key.A);
        sut.Registrations[0].Scope.Should().Be(HotkeyScope.Local);
    }

    [StaFact]
    public void Register_ReturnsDisposableRegistration()
    {
        // Arrange
        var registration = sut.Register(
            ModifierKeys.Control,
            Key.B,
            _ => { },
            "Disposable test");

        // Act
        registration.Dispose();

        // Assert
        sut.Registrations.Should().BeEmpty();
    }

    [StaFact]
    public void Unregister_RemovesFromRegistrations()
    {
        // Arrange
        var registration = sut.Register(
            ModifierKeys.Control,
            Key.C,
            _ => { },
            "To remove");

        // Act
        sut.Unregister(registration);

        // Assert
        sut.Registrations.Should().BeEmpty();
    }

    [StaFact]
    public void Register_DuplicateKey_RaisesConflictDetected()
    {
        // Arrange
        HotkeyConflictEventArgs? conflictArgs = null;
        sut.ConflictDetected += (_, args) => conflictArgs = args;

        sut.Register(
            ModifierKeys.Control,
            Key.D,
            _ => { },
            "First");

        // Act
        sut.Register(
            ModifierKeys.Control,
            Key.D,
            _ => { },
            "Second");

        // Assert
        conflictArgs.Should().NotBeNull();
        conflictArgs!.Existing.Description.Should().Be("First");
        conflictArgs.Requested.Description.Should().Be("Second");
    }

    [StaFact]
    public void RegisterChord_AddsToRegistrations()
    {
        // Arrange & Act
        var registration = sut.RegisterChord(
            ModifierKeys.Control,
            Key.K,
            ModifierKeys.Control,
            Key.C,
            _ => { },
            "Chord test");

        // Assert
        sut.Registrations.Should().HaveCount(1);
        registration.Chord.Should().NotBeNull();
        registration.Chord!.FirstKey.Should().Be(Key.K);
        registration.Chord.SecondKey.Should().Be(Key.C);
        registration.Description.Should().Be("Chord test");
    }

    [StaFact]
    public void Registrations_ReturnsReadOnlyCopy()
    {
        // Arrange
        sut.Register(ModifierKeys.Control, Key.E, _ => { });
        var snapshot = sut.Registrations;

        // Act — register another
        sut.Register(ModifierKeys.Control, Key.F, _ => { });

        // Assert — the snapshot should not have changed
        snapshot.Should().HaveCount(1);
        sut.Registrations.Should().HaveCount(2);
    }

    [StaFact]
    public void Dispose_ClearsAllRegistrations()
    {
        // Arrange
        sut.Register(ModifierKeys.Control, Key.G, _ => { });
        sut.Register(ModifierKeys.Alt, Key.H, _ => { });

        // Act
        sut.Dispose();

        // Assert
        sut.Registrations.Should().BeEmpty();
    }

    [StaFact]
    public void Register_GlobalScope_SetsScope()
    {
        // Act
        var registration = sut.Register(
            ModifierKeys.Control | ModifierKeys.Shift,
            Key.H,
            _ => { },
            "Global test",
            HotkeyScope.Global);

        // Assert
        registration.Scope.Should().Be(HotkeyScope.Global);
    }

    [StaFact]
    public void Register_DifferentScopes_DoesNotConflict()
    {
        // Arrange
        HotkeyConflictEventArgs? conflictArgs = null;
        sut.ConflictDetected += (_, args) => conflictArgs = args;

        sut.Register(ModifierKeys.Control, Key.I, _ => { }, "Local", HotkeyScope.Local);

        // Act
        sut.Register(ModifierKeys.Control, Key.I, _ => { }, "Global", HotkeyScope.Global);

        // Assert — different scopes should not conflict
        conflictArgs.Should().BeNull();
    }

    [StaFact]
    public void SaveBindings_WritesJsonFile()
    {
        // Arrange
        var filePath = Path.Combine(Path.GetTempPath(), $"hotkey-test-{Guid.NewGuid()}.json");
        try
        {
            sut.Register(ModifierKeys.Control, Key.S, _ => { }, "Save", HotkeyScope.Local);

            // Act
            sut.SaveBindings(filePath);

            // Assert
            File.Exists(filePath).Should().BeTrue();
            var json = File.ReadAllText(filePath);
            json.Should().Contain("\"Key\": \"S\"");
            json.Should().Contain("\"Description\": \"Save\"");
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [StaFact]
    public void LoadBindings_RestoresRegistrations()
    {
        // Arrange
        var filePath = Path.Combine(Path.GetTempPath(), $"hotkey-test-{Guid.NewGuid()}.json");
        try
        {
            var json = """
                [
                  {
                    "Modifiers": "Control",
                    "Key": "N",
                    "Description": "New file",
                    "Scope": "Local"
                  }
                ]
                """;
            File.WriteAllText(filePath, json);

            // Act
            sut.LoadBindings(filePath);

            // Assert
            sut.Registrations.Should().HaveCount(1);
            sut.Registrations[0].Modifiers.Should().Be(ModifierKeys.Control);
            sut.Registrations[0].Key.Should().Be(Key.N);
            sut.Registrations[0].Description.Should().Be("New file");
            sut.Registrations[0].Scope.Should().Be(HotkeyScope.Local);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [StaFact]
    public void SaveAndLoad_RoundTrip_PreservesBindings()
    {
        // Arrange
        var filePath = Path.Combine(Path.GetTempPath(), $"hotkey-test-{Guid.NewGuid()}.json");
        try
        {
            sut.Register(
                ModifierKeys.Control | ModifierKeys.Shift,
                Key.H,
                _ => { },
                "Show app",
                HotkeyScope.Global);

            sut.Register(
                ModifierKeys.Alt,
                Key.F4,
                _ => { },
                "Close window",
                HotkeyScope.Local);

            // Act
            sut.SaveBindings(filePath);

            using var loadService = new HotkeyService();
            loadService.LoadBindings(filePath);

            // Assert
            loadService.Registrations.Should().HaveCount(2);

            loadService.Registrations[0].Description.Should().Be("Show app");
            loadService.Registrations[0].Modifiers.Should().Be(ModifierKeys.Control | ModifierKeys.Shift);
            loadService.Registrations[0].Key.Should().Be(Key.H);
            loadService.Registrations[0].Scope.Should().Be(HotkeyScope.Global);

            loadService.Registrations[1].Description.Should().Be("Close window");
            loadService.Registrations[1].Modifiers.Should().Be(ModifierKeys.Alt);
            loadService.Registrations[1].Key.Should().Be(Key.F4);
            loadService.Registrations[1].Scope.Should().Be(HotkeyScope.Local);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [StaFact]
    public void LoadBindings_WithChord_RestoresChord()
    {
        // Arrange
        var filePath = Path.Combine(Path.GetTempPath(), $"hotkey-test-{Guid.NewGuid()}.json");
        try
        {
            var json = """
                [
                  {
                    "Modifiers": "Control",
                    "Key": "K",
                    "Description": "Comment selection",
                    "Scope": "Local",
                    "SecondModifiers": "Control",
                    "SecondKey": "C"
                  }
                ]
                """;
            File.WriteAllText(filePath, json);

            // Act
            sut.LoadBindings(filePath);

            // Assert
            sut.Registrations.Should().HaveCount(1);
            sut.Registrations[0].Description.Should().Be("Comment selection");
            sut.Registrations[0].Chord.Should().NotBeNull();
            sut.Registrations[0].Chord!.FirstModifiers.Should().Be(ModifierKeys.Control);
            sut.Registrations[0].Chord!.FirstKey.Should().Be(Key.K);
            sut.Registrations[0].Chord!.SecondModifiers.Should().Be(ModifierKeys.Control);
            sut.Registrations[0].Chord!.SecondKey.Should().Be(Key.C);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    public void Dispose()
    {
        sut.Dispose();
    }
}