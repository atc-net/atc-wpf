namespace Atc.Wpf.Controls.Tests.DataDisplay;

public sealed class StepperStepChangingEventArgsTests
{
    [Fact]
    public void Constructor_exposes_indices_and_defaults_Cancel_to_false()
    {
        var args = new StepperStepChangingEventArgs(currentIndex: 2, targetIndex: 5);

        Assert.Equal(2, args.CurrentIndex);
        Assert.Equal(5, args.TargetIndex);
        Assert.False(args.Cancel);
    }

    [Fact]
    public void Cancel_is_settable()
    {
        var args = new StepperStepChangingEventArgs(0, 1);

        args.Cancel = true;

        Assert.True(args.Cancel);
    }
}