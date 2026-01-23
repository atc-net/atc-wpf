namespace Atc.Wpf.Controls.Flyouts;

/// <summary>
/// Non-generic factory methods for <see cref="FlyoutFormResult{TModel}"/>.
/// </summary>
public static class FlyoutFormResultFactory
{
    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <typeparam name="TModel">The type of the form model.</typeparam>
    /// <param name="model">The validated model.</param>
    /// <returns>A successful form result.</returns>
    public static FlyoutFormResult<TModel> Success<TModel>(TModel model)
        where TModel : class
        => new(model, isSubmitted: true, isValid: true);

    /// <summary>
    /// Creates a cancelled result.
    /// </summary>
    /// <typeparam name="TModel">The type of the form model.</typeparam>
    /// <param name="model">The model at the time of cancellation.</param>
    /// <returns>A cancelled form result.</returns>
    public static FlyoutFormResult<TModel> Cancelled<TModel>(TModel model)
        where TModel : class
        => new(model, isSubmitted: false, isValid: false);

    /// <summary>
    /// Creates a validation failure result.
    /// </summary>
    /// <typeparam name="TModel">The type of the form model.</typeparam>
    /// <param name="model">The model that failed validation.</param>
    /// <param name="errors">The validation errors.</param>
    /// <returns>A validation failure form result.</returns>
    public static FlyoutFormResult<TModel> ValidationFailed<TModel>(
        TModel model,
        IReadOnlyList<string> errors)
        where TModel : class
        => new(model, isSubmitted: true, isValid: false, errors);
}