namespace Atc.Wpf.Internal;

/// <summary>
/// Writes a configuration warning to <see cref="Trace"/> the first time it happens, so a missing resource or a
/// broken converter is visible in a Release build without flooding the output on every binding update.
/// </summary>
internal static class TraceOnce
{
    private static readonly ConcurrentDictionary<string, byte> Reported = new(StringComparer.Ordinal);

    public static void Warn(
        string key,
        string message)
    {
        if (Reported.TryAdd(key, 0))
        {
            Trace.TraceWarning(message);
        }
    }
}