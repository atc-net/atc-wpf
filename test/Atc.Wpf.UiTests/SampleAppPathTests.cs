namespace Atc.Wpf.UiTests;

/// <summary>
/// Pure-logic tests for the <see cref="SampleAppPath"/> locator helper.
/// These deliberately do <b>not</b> launch the sample app — they only validate
/// the locator's resolution logic. They exist primarily so the
/// <c>Atc.Wpf.UiTests</c> project has at least one non-skipped test, which
/// keeps Microsoft Testing Platform from exiting with code 8 ("Zero tests ran")
/// in CI when the FlaUI tests are gated behind <c>[Fact(Skip = "...")]</c>.
/// </summary>
public sealed class SampleAppPathTests
{
    [Fact]
    public void Resolve_returns_a_path_that_exists()
    {
        // The sample app is added as a ProjectReference, so MSBuild copies
        // Atc.Wpf.Sample.exe into the test bin directory at build time.
        var path = SampleAppPath.Resolve();

        Assert.True(File.Exists(path), $"Resolved path should exist on disk: {path}");
    }

    [Fact]
    public void Resolve_returns_path_ending_with_sample_executable_name()
    {
        var path = SampleAppPath.Resolve();

        Assert.EndsWith("Atc.Wpf.Sample.exe", path, StringComparison.OrdinalIgnoreCase);
    }
}