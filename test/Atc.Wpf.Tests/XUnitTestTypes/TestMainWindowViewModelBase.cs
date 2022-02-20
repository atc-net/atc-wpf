namespace Atc.Wpf.Tests.XUnitTestTypes;

public class TestMainWindowViewModelBase : MainWindowViewModelBase
{
    private bool isBoolProperty;
    private bool isBoolPropertyWithExpression;

    public bool IsBoolProperty
    {
        get => this.isBoolProperty;
        set
        {
            this.isBoolProperty = value;
            this.RaisePropertyChanged();
        }
    }

    public bool IsBoolPropertyWithExpression
    {
        get => this.isBoolPropertyWithExpression;
        set
        {
            this.isBoolPropertyWithExpression = value;
            this.RaisePropertyChanged(() => this.IsBoolPropertyWithExpression);
        }
    }
}