#pragma warning disable IDE0130
namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// Picks the row template of a <see cref="CheckListBox"/> row from the owner's item style.
/// </summary>
internal sealed class CheckListRowTemplateSelector : DataTemplateSelector
{
    private readonly CheckListBox owner;

    public CheckListRowTemplateSelector(CheckListBox owner)
        => this.owner = owner;

    public override DataTemplate? SelectTemplate(
        object item,
        DependencyObject container)
        => item is CheckListItem row
            ? owner.SelectRowTemplate(row)
            : base.SelectTemplate(item, container);
}