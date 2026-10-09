namespace Atc.Wpf.Theming.Exceptions;

/// <summary>
/// The base exception for errors raised by the theming library.
/// </summary>
[Serializable]
public class AtcAppsException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AtcAppsException"/> class.
    /// </summary>
    public AtcAppsException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AtcAppsException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public AtcAppsException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AtcAppsException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public AtcAppsException(
        string message,
        Exception? innerException)
        : base(
            message,
            innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AtcAppsException"/> class with serialized data.
    /// </summary>
    /// <param name="info">The object that holds the serialized object data.</param>
    /// <param name="context">The contextual information about the source or destination.</param>
    protected AtcAppsException(
        SerializationInfo info,
        StreamingContext context)
        : base(
            info,
            context)
    {
    }
}