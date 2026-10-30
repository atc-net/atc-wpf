[assembly: AssemblyFixture(typeof(Atc.Wpf.Controls.Tests.TestSupport.WpfStaticInitialization))]

namespace Atc.Wpf.Controls.Tests.TestSupport;

/// <summary>
/// Runs the static constructors of the WPF framework elements once, on one thread, before any test starts.
/// </summary>
/// <remarks>
/// WPF can deadlock when several UI threads initialize it at the same time: a thread loading XAML holds
/// the shared BAML schema lock and waits for a type initializer (e.g. <see cref="ContentPresenter" />),
/// while the thread running that initializer builds a default template and waits for the schema lock.
/// The tests run on many STA threads in parallel, so the first ones would hit this race.
/// </remarks>
[SuppressMessage("Major Code Smell", "S1118:Utility classes should not have public constructors", Justification = "xUnit creates the assembly fixture.")]
public sealed class WpfStaticInitialization
{
    public WpfStaticInitialization()
    {
        var thread = new Thread(RunFrameworkElementTypeInitializers);
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
    }

    private static void RunFrameworkElementTypeInitializers()
    {
        var frameworkElementTypes = typeof(FrameworkElement).Assembly
            .GetTypes()
            .Where(t => typeof(DependencyObject).IsAssignableFrom(t) && !t.ContainsGenericParameters);

        foreach (var type in frameworkElementTypes)
        {
            RuntimeHelpers.RunClassConstructor(type.TypeHandle);
        }

        Dispatcher.CurrentDispatcher.InvokeShutdown();
    }
}