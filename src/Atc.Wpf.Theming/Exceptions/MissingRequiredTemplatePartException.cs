namespace Atc.Wpf.Theming.Exceptions;

[Serializable]
public class MissingRequiredTemplatePartException : AtcAppsException
{
    public MissingRequiredTemplatePartException()
    {
    }

    public MissingRequiredTemplatePartException(string message)
        : base(message)
    {
    }

    public MissingRequiredTemplatePartException(
        string message,
        Exception innerException)
        : base(
            message,
            innerException)
    {
    }

    [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
    public MissingRequiredTemplatePartException(
        FrameworkElement target,
        string templatePart)
        : base($"Template part \"{templatePart}\" in template for \"{target.GetType().FullName}\" is missing.")
    {
    }

    protected MissingRequiredTemplatePartException(
        SerializationInfo info,
        StreamingContext context)
        : base(
            info,
            context)
    {
    }
}