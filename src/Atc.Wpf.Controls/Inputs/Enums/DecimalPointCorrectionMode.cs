// ReSharper disable CheckNamespace
namespace Atc.Wpf.Controls;

/// <summary>Specifies how the decimal-point key is corrected while typing in a numeric input.</summary>
public enum DecimalPointCorrectionMode
{
    /// <summary>
    /// (Default) No correction is applied, and any style
    /// inherited setting may influence the correction behavior.
    /// </summary>
    Inherits,

    /// <summary>
    /// Enable the decimal-point correction for generic numbers.
    /// </summary>
    Number,

    /// <summary>
    /// Enable the decimal-point correction for currency numbers.
    /// </summary>
    Currency,

    /// <summary>
    /// Enable the decimal-point correction for percent-numbers.
    /// </summary>
    Percent,
}