namespace Atc.Wpf.Controls.Inputs;

/// <summary>A numeric input box restricted to whole numbers.</summary>
public sealed class IntegerBox : NumericBox
{
    /// <summary>Initializes a new instance of the <see cref="IntegerBox"/> class.</summary>
    public IntegerBox()
    {
        UseUiCultureUntilSet();
    }
}