namespace Atc.Wpf.Controls.Tests.DataDisplay;

public sealed class StepperStepChangedEventArgsTests
{
    [Fact]
    public void Constructor_exposes_old_and_new_index_as_provided()
    {
        var args = new StepperStepChangedEventArgs(oldIndex: 0, newIndex: 3);

        Assert.Equal(0, args.OldIndex);
        Assert.Equal(3, args.NewIndex);
    }
}