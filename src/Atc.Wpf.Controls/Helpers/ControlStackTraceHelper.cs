// ReSharper disable LoopCanBeConvertedToQuery
namespace Atc.Wpf.Controls.Helpers;

public static class ControlStackTraceHelper
{
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