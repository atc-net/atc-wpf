namespace Atc.Wpf.Theming.Exceptions;

[Serializable]
public class AtcAppsException : Exception
{
    public AtcAppsException()
    {
    }

    public AtcAppsException(string message)
        : base(message)
    {
    }

    public AtcAppsException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }

    protected AtcAppsException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}