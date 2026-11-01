#pragma warning disable IDE0130
namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// A check box of the <see cref="MultiColumnComboBox"/> pop-up grid (a row's check box or the "select all" header).
/// It never changes its own state: a click, <c>Space</c> or the UI Automation toggle asks the owner, which updates the
/// selection and pushes the new state back.
/// </summary>
internal sealed class MultiColumnCheckBox : CheckBox
{
    private readonly Action<MultiColumnCheckBox> toggle;

    public MultiColumnCheckBox(Action<MultiColumnCheckBox> toggle)
    {
        this.toggle = toggle;
    }

    /// <summary>
    /// Gets a value indicating whether this is a row's check box (not the "select all" header).
    /// </summary>
    public bool IsRowCheckBox { get; init; }

    /// <inheritdoc />
    protected override void OnToggle()
        => toggle(this);
}