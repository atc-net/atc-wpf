namespace Atc.Wpf.UiTests;

/// <summary>
/// Locates the built `Atc.Wpf.Sample.exe` so UI tests can launch it.
/// The sample is added as a ProjectReference, so MSBuild copies it into the
/// test bin directory; it can also be found at the well-known sample bin path
/// when running locally.
/// </summary>
internal static class SampleAppPath
{
    private const string SampleExeName = "Atc.Wpf.Sample.exe";

    public static string Resolve()
    {
        var testDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

        var candidate = Path.Combine(testDir, SampleExeName);
        if (File.Exists(candidate))
        {
            return candidate;
        }

        // Walk up to the repo root (looking for the .slnx) and probe the sample bin folder.
        var dir = new DirectoryInfo(testDir);
        while (dir is not null && dir.GetFiles("Atc.Wpf.slnx").Length == 0)
        {
            dir = dir.Parent;
        }

        if (dir is not null)
        {
            var configuration = testDir.Contains("Release", StringComparison.OrdinalIgnoreCase)
                ? "Release"
                : "Debug";
            var sampleBin = Path.Combine(
                dir.FullName,
                "sample",
                "Atc.Wpf.Sample",
                "bin",
                configuration,
                "net10.0-windows",
                SampleExeName);

            if (File.Exists(sampleBin))
            {
                return sampleBin;
            }
        }

        throw new FileNotFoundException(
            $"Could not locate {SampleExeName}. Build the sample app first " +
            $"(`dotnet build sample/Atc.Wpf.Sample`).",
            SampleExeName);
    }
}