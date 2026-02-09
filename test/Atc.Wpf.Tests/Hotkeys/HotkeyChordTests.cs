namespace Atc.Wpf.Tests.Hotkeys;

public class HotkeyChordTests
{
    [StaFact]
    public void Constructor_SetsProperties()
    {
        // Arrange & Act
        var chord = new HotkeyChord(
            ModifierKeys.Control,
            Key.K,
            ModifierKeys.Control,
            Key.C);

        // Assert
        chord.FirstModifiers.Should().Be(ModifierKeys.Control);
        chord.FirstKey.Should().Be(Key.K);
        chord.SecondModifiers.Should().Be(ModifierKeys.Control);
        chord.SecondKey.Should().Be(Key.C);
    }

    [StaFact]
    public void Equals_SameChord_ReturnsTrue()
    {
        // Arrange
        var chord1 = new HotkeyChord(ModifierKeys.Control, Key.K, ModifierKeys.Control, Key.C);
        var chord2 = new HotkeyChord(ModifierKeys.Control, Key.K, ModifierKeys.Control, Key.C);

        // Act & Assert
        chord1.Equals(chord2).Should().BeTrue();
        chord1.GetHashCode().Should().Be(chord2.GetHashCode());
    }

    [StaFact]
    public void Equals_DifferentChord_ReturnsFalse()
    {
        // Arrange
        var chord1 = new HotkeyChord(ModifierKeys.Control, Key.K, ModifierKeys.Control, Key.C);
        var chord2 = new HotkeyChord(ModifierKeys.Control, Key.K, ModifierKeys.Control, Key.T);

        // Act & Assert
        chord1.Equals(chord2).Should().BeFalse();
    }

    [StaFact]
    public void ToString_ReturnsReadableFormat()
    {
        // Arrange
        var chord = new HotkeyChord(ModifierKeys.Control, Key.K, ModifierKeys.Control, Key.C);

        // Act
        var result = chord.ToString();

        // Assert
        result.Should().Be("Ctrl+K, Ctrl+C");
    }

    [StaFact]
    public void ToString_WithMultipleModifiers_ReturnsReadableFormat()
    {
        // Arrange
        var chord = new HotkeyChord(
            ModifierKeys.Control | ModifierKeys.Shift,
            Key.K,
            ModifierKeys.Alt,
            Key.C);

        // Act
        var result = chord.ToString();

        // Assert
        result.Should().Be("Ctrl+Shift+K, Alt+C");
    }
}