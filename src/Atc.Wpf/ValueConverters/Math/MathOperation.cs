// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// The math operations which can be used at the <see cref="MathValueConverter"/>
/// </summary>
public enum MathOperation
{
    /// <summary>
    /// Adds the second operand to the first.
    /// </summary>
    Add,

    /// <summary>
    /// Subtracts the second operand from the first.
    /// </summary>
    Subtract,

    /// <summary>
    /// Multiplies the two operands.
    /// </summary>
    Multiply,

    /// <summary>
    /// Divides the first operand by the second (only when the second operand is greater than zero).
    /// </summary>
    Divide,
}