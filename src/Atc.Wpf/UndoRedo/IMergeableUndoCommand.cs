namespace Atc.Wpf.UndoRedo;

/// <summary>
/// Represents an undo command that can be merged with a consecutive similar command,
/// allowing multiple related operations (e.g., slider drags, character typing) to
/// coalesce into a single undo step.
/// </summary>
public interface IMergeableUndoCommand : IUndoCommand
{
    /// <summary>
    /// Gets an identifier used to determine whether two commands are candidates for merging.
    /// Commands with the same <see cref="MergeId"/> may be merged if <see cref="TryMergeWith"/>
    /// returns <see langword="true"/>.
    /// </summary>
    int MergeId { get; }

    /// <summary>
    /// Attempts to absorb <paramref name="other"/> into this command instance.
    /// If the merge succeeds, this command's state is updated to reflect the combined
    /// effect and the caller discards <paramref name="other"/>.
    /// </summary>
    /// <param name="other">The newer command to merge into this instance.</param>
    /// <returns><see langword="true"/> if the merge was successful; otherwise <see langword="false"/>.</returns>
    bool TryMergeWith(IUndoCommand other);
}