namespace Atc.Wpf.Tests.XUnitTestTypes;

/// <summary>
/// A managed markup extension (like ResxExtension) whose value can change, for tests that update its targets.
/// </summary>
public sealed class TestTextExtension : ManagedMarkupExtension
{
    public TestTextExtension()
        : base(Manager)
    {
    }

    public static MarkupExtensionManager Manager { get; } = new(cleanupInterval: 1000);

    public static string Text { get; set; } = "Before";

    protected override object GetValue()
        => Text;
}