// ReSharper disable LoopCanBeConvertedToQuery
namespace Atc.Wpf.Controls.Helpers;

/// <summary>Helper methods that inspect the current call stack.</summary>
public static class ControlStackTraceHelper
{
    /// <summary>Determines whether the current call originates from the <c>ClearControl</c> command.</summary>
    public static bool IsCalledFromClearCommand()
    {
        var stackTrace = new StackTrace();

        foreach (var frame in stackTrace.GetFrames())
        {
            var methodBase = frame.GetMethod();
            if (methodBase is not null &&
                nameof(AtcAppsCommands.ClearControl).Equals(methodBase.Name, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }
}