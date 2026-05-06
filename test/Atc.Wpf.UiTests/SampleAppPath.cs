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

        // PREFER the sample's own bin/ folder over the copy MSBuild dropped
        // alongside the test exe. The sample's view-loader walks up from the
        // running exe until it finds a "bin" folder, then takes its parent as
        // the project root and searches there for sample XAML files. From the
        // test's bin/ that root would be the test project (which has no
        // sample sources) — every leaf would fail with "Can't find sample by
        // invalid location". Running the source-tree exe puts that root at
        // the sample project, where the XAMLs live.
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
            var sampleBinRoot = Path.Combine(dir.FullName, "sample", "Atc.Wpf.Sample", "bin", configuration);

            if (Directory.Exists(sampleBinRoot))
            {
                var sourceExe = Directory
                    .EnumerateFiles(sampleBinRoot, SampleExeName, SearchOption.AllDirectories)
                    .FirstOrDefault();
                if (sourceExe is not null)
                {
                    return sourceExe;
                }
            }
        }

        // Fallback: the copy alongside the test exe. Sample loading from this
        // path can fail (see comment above), but it's better than nothing for
        // smoke tests that don't navigate into samples.
        var candidate = Path.Combine(testDir, SampleExeName);
        if (File.Exists(candidate))
        {
            return candidate;
        }

        throw new FileNotFoundException(
            $"Could not locate {SampleExeName}. Build the sample app first " +
            $"(`dotnet build sample/Atc.Wpf.Sample`).",
            SampleExeName);
    }
}