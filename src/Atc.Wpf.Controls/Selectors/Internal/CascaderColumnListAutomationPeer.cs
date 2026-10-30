#pragma warning disable IDE0130
namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// The automation peer of a <see cref="CascaderComboBox"/> column (or its list of filter results): a list whose items
/// are check boxes with a three-state toggle.
/// </summary>
internal sealed class CascaderColumnListAutomationPeer : ItemsControlAutomationPeer
{
    public CascaderColumnListAutomationPeer(CascaderColumnList owner)
        : base(owner)
    {
    }

    /// <inheritdoc />
    protected override ItemAutomationPeer CreateItemAutomationPeer(object item)
        => new CascaderRowAutomationPeer(item, this);

    /// <inheritdoc />
    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.List;

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => "CascaderColumn";

    /// <inheritdoc />
    protected override string GetAutomationIdCore()
    {
        var id = base.GetAutomationIdCore();
        if (!string.IsNullOrEmpty(id))
        {
            return id;
        }

        var list = (CascaderColumnList)Owner;
        return list.IsFilterResults
            ? "FilterResults"
            : "Column" + list.ColumnIndex.ToString(CultureInfo.InvariantCulture);
    }
}