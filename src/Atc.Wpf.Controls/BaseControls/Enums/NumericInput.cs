// ReSharper disable CheckNamespace
namespace Atc.Wpf.Controls.BaseControls;

[SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "OK.")]
[Flags]
public enum NumericInput
{
    /// <summary>
    /// Only numbers are allowed
    /// </summary>
    Numbers = 1 << 1, // Only Numbers

    /// <summary>
    /// Numbers with decimal point and allowed scientific input
    /// </summary>
    Decimal = 2 << 1,

    /// <summary>
    /// All is allowed
    /// </summary>
    All = Numbers | Decimal,
}