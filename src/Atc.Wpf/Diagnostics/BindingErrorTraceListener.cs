namespace Atc.Wpf.Diagnostics;

/// <summary>
/// A class that listens to binding errors and forwards them as message boxes.
/// </summary>
public class BindingErrorTraceListener : DefaultTraceListener
{
    private static BindingErrorTraceListener? listener;
    private readonly StringBuilder errorMessage = new StringBuilder();
    private readonly List<string> ignoreErrorMessages = new List<string>();

    /// <summary>
    /// Prevents a default instance of the <see cref="BindingErrorTraceListener" /> class from being created.
    /// </summary>
    private BindingErrorTraceListener()
    {
        ignoreErrorMessages.Add("Cannot find governing FrameworkElement or FrameworkContentElement for target element. BindingExpression:Path=Fill; DataItem=null; target element is 'GeometryDrawing'");
    }

    /// <summary>
    /// Starts the trace.
    /// </summary>
    /// <param name="level">The level.</param>
    /// <param name="options">The options.</param>
    public static void StartTrace(SourceLevels level = SourceLevels.Error, TraceOptions options = TraceOptions.None)
    {
        if (listener is null)
        {
            listener = new BindingErrorTraceListener();
            _ = PresentationTraceSources.DataBindingSource.Listeners.Add(listener);
        }

        listener.TraceOutputOptions = options;
        PresentationTraceSources.DataBindingSource.Switch.Level = level;
    }

    /// <summary>
    /// Closes the trace.
    /// </summary>
    public static void CloseTrace()
    {
        if (listener is null)
        {
            return;
        }

        listener.Flush();
        listener.Close();
        PresentationTraceSources.DataBindingSource.Listeners.Remove(listener);
        listener = null;
    }

    /// <summary>
    /// Writes the output to the <see langword="OutputDebugString" /> function and to the <see ref="M:System.Diagnostics.Debugger.Log(System.Int32,System.String,System.String)" /> method.
    /// </summary>
    /// <param name="message">The message to write to <see langword="OutputDebugString" /> and <see ref="M:System.Diagnostics.Debugger.Log(System.Int32,System.String,System.String)" />.</param>
    /// <PermissionSet>
    /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="True" />
    /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="ControlEvidence" />
    /// </PermissionSet>
    public override void Write(string? message)
    {
        _ = errorMessage.Append(message);
    }

    /// <summary>
    /// Writes the output to the <see langword="OutputDebugString" /> function and to the <see ref="M:System.Diagnostics.Debugger.Log(System.Int32,System.String,System.String)" /> method,
    /// followed by a carriage return and line feed (\r\n).
    /// </summary>
    /// <param name="message">The message to write to <see langword="OutputDebugString" /> and <see ref="M:System.Diagnostics.Debugger.Log(System.Int32,System.String,System.String)" />.</param>
    /// <exception cref="ArgumentException">BindingError: " + error.</exception>
    /// <PermissionSet>
    /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="True" />
    /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="True" />
    /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="ControlEvidence" />
    /// </PermissionSet>
    public override void WriteLine(string? message)
    {
        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        if (ignoreErrorMessages.Exists(x => message.Contains(x, StringComparison.Ordinal)))
        {
            return;
        }

        _ = errorMessage.Append(message);
        var error = errorMessage.ToString();
        errorMessage.Length = 0;

        _ = Application.Current.Dispatcher.BeginInvoke(
            DispatcherPriority.Normal,
            new DispatcherOperationCallback(
                delegate
                {
                    _ = MessageBox.Show(error, "Binding Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }),
            arg: null);
    }
}