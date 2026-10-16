namespace Atc.Wpf.Tests.XUnitTestTypes;

/// <summary>
/// A managed markup extension whose value lookup registers another extension with the same manager,
/// like a Resx value whose target re-applies a template that contains further Resx extensions.
/// </summary>
public sealed class SpawningTextExtension : ManagedMarkupExtension
{
    public SpawningTextExtension()
        : base(Manager)
    {
    }

    public static MarkupExtensionManager Manager { get; } = new(cleanupInterval: 1000);

    protected override object GetValue()
    {
        _ = new SpawningTextExtension();
        return "spawned";
    }
}