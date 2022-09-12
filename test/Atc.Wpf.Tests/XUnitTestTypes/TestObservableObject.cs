namespace Atc.Wpf.Tests.XUnitTestTypes;

public class TestObservableObject : ObservableObject
{
    private bool isBoolProperty;
    private bool isBoolPropertyWithExpression;
    private bool isBoolPropertyWithSet;
    private bool isBoolPropertyWithSetAndExpression;

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

    [SuppressMessage("Maintainability", "CA1507:Use nameof to express symbol names", Justification = "OK.")]
    public bool IsBoolPropertyWithSet
    {
        get => isBoolPropertyWithSet;
        set => _ = Set("IsBoolPropertyWithSet", ref isBoolPropertyWithSet, value);
    }

    public bool IsBoolPropertyWithSetAndExpression
    {
        get => isBoolPropertyWithSetAndExpression;
        set => _ = Set(() => IsBoolPropertyWithSetAndExpression, ref isBoolPropertyWithSetAndExpression, value);
    }
}