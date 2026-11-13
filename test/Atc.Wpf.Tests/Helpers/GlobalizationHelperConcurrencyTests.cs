namespace Atc.Wpf.Tests.Helpers;

// Culture changes are allowed from any thread, so the resource manager cache is used from several threads at once.
public sealed class GlobalizationHelperConcurrencyTests
{
    private const string ResxName = "Atc.Wpf.Resources.Miscellaneous";

    [Fact]
    public void GetResourceManager_from_many_threads_does_not_throw()
    {
        var options = new ParallelOptions { MaxDegreeOfParallelism = 16 };

        var exception = Record.Exception(() =>
        {
            for (var round = 0; round < 20; round++)
            {
                Parallel.For(0, 2000, options, _ => GlobalizationHelper.GetResourceManager(ResxName));
            }
        });

        Assert.Null(exception);
    }
}