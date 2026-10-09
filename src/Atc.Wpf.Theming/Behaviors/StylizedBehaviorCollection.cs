namespace Atc.Wpf.Theming.Behaviors;

/// <summary>
/// A collection of behaviors that can be assigned from a style through <see cref="StylizedBehaviors"/>.
/// </summary>
public class StylizedBehaviorCollection : FreezableCollection<Behavior>
{
    /// <inheritdoc />
    protected override Freezable CreateInstanceCore()
        => new StylizedBehaviorCollection();
}