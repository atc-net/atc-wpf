namespace Atc.Wpf.Translation;

public sealed class ResourceEventArgs : EventArgs
{
    public ResourceEventArgs(
        string resxName,
        string key,
        CultureInfo uiCulture)
    {
        ResxName = resxName;
        Key = key;
        UiCulture = uiCulture;
    }

    public string ResxName { get; }

    public string Key { get; }

    public CultureInfo UiCulture { get; }
}