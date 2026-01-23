namespace Atc.Wpf.Controls.Flyouts;

/// <summary>
/// Represents the result of a form flyout.
/// </summary>
/// <typeparam name="TModel">The type of the form model.</typeparam>
#pragma warning disable CA1000 // Do not declare static members on generic types
public sealed class FlyoutFormResult<TModel>
    where TModel : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FlyoutFormResult{TModel}"/> class.
    /// </summary>
    /// <param name="model">The form model.</param>
    /// <param name="isSubmitted">Whether the form was submitted.</param>
    /// <param name="isValid">Whether the form is valid.</param>
    /// <param name="validationErrors">The validation errors, if any.</param>
    public FlyoutFormResult(
        TModel model,
        bool isSubmitted,
        bool isValid,
        IReadOnlyList<string>? validationErrors = null)
    {
        Model = model;
        IsSubmitted = isSubmitted;
        IsValid = isValid;
        ValidationErrors = validationErrors ?? [];
    }

    /// <summary>
    /// Gets the form model.
    /// </summary>
    public TModel Model { get; }

    /// <summary>
    /// Gets whether the form was submitted (user clicked Save/Submit).
    /// </summary>
    public bool IsSubmitted { get; }

    /// <summary>
    /// Gets whether the form passed validation.
    /// </summary>
    public bool IsValid { get; }

    /// <summary>
    /// Gets the validation errors, if any.
    /// </summary>
    public IReadOnlyList<string> ValidationErrors { get; }

    /// <summary>
    /// Gets whether the form was cancelled (user clicked Cancel or dismissed).
    /// </summary>
    public bool IsCancelled => !IsSubmitted;

    /// <summary>
    /// Gets whether the form was successfully submitted with valid data.
    /// </summary>
    public bool IsSuccess => IsSubmitted && IsValid;

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <param name="model">The validated model.</param>
    /// <returns>A successful form result.</returns>
    public static FlyoutFormResult<TModel> Success(TModel model)
        => new(model, isSubmitted: true, isValid: true);

    /// <summary>
    /// Creates a cancelled result.
    /// </summary>
    /// <param name="model">The model at the time of cancellation.</param>
    /// <returns>A cancelled form result.</returns>
    public static FlyoutFormResult<TModel> Cancelled(TModel model)
        => new(model, isSubmitted: false, isValid: false);

    /// <summary>
    /// Creates a validation failure result.
    /// </summary>
    /// <param name="model">The model that failed validation.</param>
    /// <param name="errors">The validation errors.</param>
    /// <returns>A validation failure form result.</returns>
    public static FlyoutFormResult<TModel> ValidationFailed(
        TModel model,
        IReadOnlyList<string> errors)
        => new(model, isSubmitted: true, isValid: false, errors);
}
#pragma warning restore CA1000 // Do not declare static members on generic types