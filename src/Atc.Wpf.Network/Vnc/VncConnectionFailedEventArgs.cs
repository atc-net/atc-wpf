namespace Atc.Wpf.Network.Vnc;

public sealed class VncConnectionFailedEventArgs : EventArgs
{
    public VncConnectionFailedEventArgs(string message)
    {
        Message = message;
    }

    public string Message { get; }
}