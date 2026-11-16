// WPF's XAML/BAML loading is not thread-safe across STA threads; constructing pickers in parallel
// test classes intermittently fails inside Application.LoadComponent, so these tests run sequentially.
[assembly: Xunit.v3.Parallelization(Mode = Xunit.Sdk.ParallelMode.None)]

[assembly: ShutDownDispatcherAfterTest]