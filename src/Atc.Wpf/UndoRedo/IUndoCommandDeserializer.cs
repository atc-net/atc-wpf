namespace Atc.Wpf.UndoRedo;

/// <summary>
/// Provides deserialization of undo commands from their binary representation.
/// Implementations should map type identifier values to concrete
/// command types and reconstruct them from the serialized data.
/// </summary>
public interface IUndoCommandDeserializer
{
    /// <summary>
    /// Deserializes a command from its type identifier and binary data.
    /// </summary>
    /// <param name="typeId">The stable type identifier written during serialization.</param>
    /// <param name="data">The serialized command data.</param>
    /// <returns>The deserialized command, or <see langword="null"/> if the type is not recognized.</returns>
    IUndoCommand? Deserialize(
        string typeId,
        byte[] data);
}