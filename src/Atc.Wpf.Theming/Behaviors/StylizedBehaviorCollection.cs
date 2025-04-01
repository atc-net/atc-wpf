namespace Atc.Wpf.Theming.Behaviors;

public class StylizedBehaviorCollection : FreezableCollection<Behavior>
{
    protected override Freezable CreateInstanceCore()
        => new StylizedBehaviorCollection();
}