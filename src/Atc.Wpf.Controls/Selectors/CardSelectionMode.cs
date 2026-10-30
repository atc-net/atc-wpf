namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// Specifies how many cards of a <see cref="CardSelector"/> can be selected.
/// </summary>
[SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "OK.")]
public enum CardSelectionMode
{
    /// <summary>Exactly one card once a choice is made (a radio group); clicking the selected card keeps it.</summary>
    Single,

    /// <summary>At most one card; clicking the selected card clears the selection.</summary>
    SingleOrNone,

    /// <summary>Any number of cards (check boxes).</summary>
    Multiple,
}