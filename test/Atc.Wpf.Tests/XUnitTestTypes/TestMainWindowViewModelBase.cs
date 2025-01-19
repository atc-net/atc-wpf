namespace Atc.Wpf.Tests.XUnitTestTypes;

internal sealed class TestMainWindowViewModelBase : MainWindowViewModelBase
{
    private bool isBoolProperty;
    private bool isBoolPropertyWithExpression;

    public bool IsBoolProperty
    {
        get => isBoolProperty;
        set
        {
            isBoolProperty = value;
            RaisePropertyChanged();
        }
    }

    public bool IsBoolPropertyWithExpression
    {
        get => isBoolPropertyWithExpression;
        set
        {
            isBoolPropertyWithExpression = value;
            RaisePropertyChanged(() => IsBoolPropertyWithExpression);
        }
    }
}