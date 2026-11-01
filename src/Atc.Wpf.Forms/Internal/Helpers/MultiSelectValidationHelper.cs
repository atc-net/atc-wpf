namespace Atc.Wpf.Forms.Internal.Helpers;

/// <summary>
/// The validation shared by the Label* multi-select wrappers: mandatory and <c>MaxSelectionCount</c>, checked
/// against one count of the selected keys.
/// </summary>
internal static class MultiSelectValidationHelper
{
    /// <summary>
    /// Counts the distinct selected keys; with <paramref name="items"/>, only keys that are present in it (a stale key
    /// neither satisfies "mandatory" nor counts towards the limit).
    /// </summary>
    public static int CountKeys(
        IEnumerable<string>? selectedKeys,
        IReadOnlyDictionary<string, string>? items = null)
    {
        if (selectedKeys is null)
        {
            return 0;
        }

        var keys = selectedKeys.Distinct(StringComparer.Ordinal);
        return items is null
            ? keys.Count()
            : keys.Count(items.ContainsKey);
    }

    /// <summary>
    /// Returns the validation text for <paramref name="keyCount"/> selected keys: "Field is required" when mandatory
    /// and none, "Select at most {0}" beyond the limit, otherwise an empty string.
    /// </summary>
    public static string GetValidationText(
        bool isMandatory,
        int? maxSelectionCount,
        int keyCount)
    {
        if (isMandatory && keyCount == 0)
        {
            return Validations.FieldIsRequired;
        }

        if (maxSelectionCount is { } max && keyCount > max)
        {
            return string.Format(CultureInfo.CurrentCulture, Validations.SelectAtMost1, max);
        }

        return string.Empty;
    }
}