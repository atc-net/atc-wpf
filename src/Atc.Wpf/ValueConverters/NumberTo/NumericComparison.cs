// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// Shared parser/evaluator for the numeric comparison mini-DSL used by the
/// <c>NumericComparisonToVisibility*ValueConverter</c> pair.
/// </summary>
internal static class NumericComparison
{
    /// <summary>
    /// Evaluates the comparison expression in <paramref name="parameter"/> against
    /// <paramref name="value"/>. Returns <see langword="false"/> when value is not numeric.
    /// Throws <see cref="FormatException"/> on a malformed expression.
    /// </summary>
    public static bool Evaluate(
        object? value,
        object? parameter)
    {
        if (!TryToDouble(value, out var n))
        {
            return false;
        }

        var expression = parameter?.ToString()?.Trim();
        if (string.IsNullOrEmpty(expression))
        {
            return false;
        }

        if (expression.StartsWith("between:", StringComparison.OrdinalIgnoreCase))
        {
            return EvaluateBetween(n, expression["between:".Length..]);
        }

        return EvaluateOperator(n, expression);
    }

    private static bool EvaluateBetween(
        double value,
        string rangeExpression)
    {
        var parts = rangeExpression.Split(',');
        if (parts.Length != 2 ||
            !double.TryParse(parts[0].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var min) ||
            !double.TryParse(parts[1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var max))
        {
            throw new FormatException(
                $"ComparisonToVisibility: malformed 'between:' parameter — expected 'between:min,max', got 'between:{rangeExpression}'.");
        }

        return value >= min && value <= max;
    }

    private static bool EvaluateOperator(
        double value,
        string expression)
    {
        string? op = null;
        string? operandText = null;

        // Longer-prefix operators first so '>=' matches before '>'.
        foreach (var candidate in Operators)
        {
            if (expression.StartsWith(candidate, StringComparison.Ordinal))
            {
                op = candidate;
                operandText = expression[candidate.Length..].Trim();
                break;
            }
        }

        if (op is null ||
            !double.TryParse(operandText, NumberStyles.Float, CultureInfo.InvariantCulture, out var rhs))
        {
            throw new FormatException(
                $"ComparisonToVisibility: malformed parameter '{expression}'. Expected '>5', '>=5', '<5', '<=5', '=5', '<>5', or 'between:min,max'.");
        }

        return op switch
        {
            ">=" => value >= rhs,
            "<=" => value <= rhs,
            "<>" => System.Math.Abs(value - rhs) > double.Epsilon,
            ">" => value > rhs,
            "<" => value < rhs,
            "=" => System.Math.Abs(value - rhs) <= double.Epsilon,
            _ => false,
        };
    }

    private static readonly string[] Operators = [">=", "<=", "<>", ">", "<", "="];

    private static bool TryToDouble(
        object? value,
        out double result)
    {
        switch (value)
        {
            case int i:
                result = i;
                return true;
            case long l:
                result = l;
                return true;
            case double d when !double.IsNaN(d) && !double.IsInfinity(d):
                result = d;
                return true;
            case decimal dec:
                result = (double)dec;
                return true;
            case float f when !float.IsNaN(f) && !float.IsInfinity(f):
                result = f;
                return true;
            case short s:
                result = s;
                return true;
            case byte b:
                result = b;
                return true;
            case uint u:
                result = u;
                return true;
            case ulong ul:
                result = ul;
                return true;
            default:
                result = 0;
                return false;
        }
    }
}