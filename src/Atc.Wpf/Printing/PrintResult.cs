namespace Atc.Wpf.Printing;

/// <summary>
/// Represents the result of a print operation.
/// </summary>
public sealed class PrintResult
{
    private PrintResult()
    {
    }

    /// <summary>
    /// Gets a value indicating whether the print operation completed successfully.
    /// </summary>
    public bool IsSuccess { get; private init; }

    /// <summary>
    /// Gets a value indicating whether the print operation was cancelled by the user.
    /// </summary>
    public bool IsCancelled { get; private init; }

    /// <summary>
    /// Gets the number of pages that were printed.
    /// </summary>
    public int PageCount { get; private init; }

    /// <summary>
    /// Gets the error message if the print operation failed.
    /// </summary>
    public string? ErrorMessage { get; private init; }

    /// <summary>
    /// Creates a successful print result.
    /// </summary>
    /// <param name="pageCount">The number of pages printed.</param>
    /// <returns>A new <see cref="PrintResult"/> indicating success.</returns>
    public static PrintResult Success(int pageCount)
        => new()
        {
            IsSuccess = true,
            PageCount = pageCount,
        };

    /// <summary>
    /// Creates a cancelled print result.
    /// </summary>
    /// <returns>A new <see cref="PrintResult"/> indicating cancellation.</returns>
    public static PrintResult Cancelled()
        => new()
        {
            IsCancelled = true,
        };

    /// <summary>
    /// Creates a failed print result.
    /// </summary>
    /// <param name="errorMessage">The error message describing the failure.</param>
    /// <returns>A new <see cref="PrintResult"/> indicating failure.</returns>
    public static PrintResult Failed(string errorMessage)
        => new()
        {
            ErrorMessage = errorMessage,
        };

    /// <inheritdoc />
    public override string ToString()
    {
        if (IsSuccess)
        {
            return $"Printed {PageCount} page(s)";
        }

        if (IsCancelled)
        {
            return "Cancelled";
        }

        return $"Failed: {ErrorMessage}";
    }
}