namespace Atc.Wpf.Theming.Exceptions;

/// <summary>
/// The exception that is thrown when a required part is missing from a control template.
/// </summary>
[Serializable]
public class MissingRequiredTemplatePartException : AtcAppsException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MissingRequiredTemplatePartException"/> class.
    /// </summary>
    public MissingRequiredTemplatePartException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MissingRequiredTemplatePartException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public MissingRequiredTemplatePartException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MissingRequiredTemplatePartException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public MissingRequiredTemplatePartException(
        string message,
        Exception innerException)
        : base(
            message,
            innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MissingRequiredTemplatePartException"/> class
    /// for the specified element and missing template part.
    /// </summary>
    /// <param name="target">The element whose template is missing the part.</param>
    /// <param name="templatePart">The name of the missing template part.</param>
    [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
    public MissingRequiredTemplatePartException(
        FrameworkElement target,
        string templatePart)
        : base($"Template part \"{templatePart}\" in template for \"{target.GetType().FullName}\" is missing.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MissingRequiredTemplatePartException"/> class with serialized data.
    /// </summary>
    /// <param name="info">The object that holds the serialized object data.</param>
    /// <param name="context">The contextual information about the source or destination.</param>
    protected MissingRequiredTemplatePartException(
        SerializationInfo info,
        StreamingContext context)
        : base(
            info,
            context)
    {
    }
}