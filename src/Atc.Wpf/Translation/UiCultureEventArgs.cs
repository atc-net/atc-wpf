namespace Atc.Wpf.Translation;

public class UiCultureEventArgs : EventArgs
{
    public UiCultureEventArgs(
        CultureInfo oldCulture,
        CultureInfo newCulture)
    {
        OldCulture = oldCulture;
        NewCulture = newCulture;
    }

    public CultureInfo OldCulture { get; }

    public CultureInfo NewCulture { get; }

    public override string ToString()
        => $"{nameof(OldCulture)}: {OldCulture}, {nameof(NewCulture)}: {NewCulture}";
}