namespace Atc.Wpf.Sample.SamplesWpfSourceGenerators;

[SuppressMessage("", "CS0169:The field is never used", Justification = "OK - SourceGenerator declaration field")]
[SuppressMessage("", "CS8618:Non-nullable variable must contain a non-null value when exiting constructor. Consider declaring it as nullable.", Justification = "OK - SourceGenerator declaration field")]
[SuppressMessage("", "CA1823:Avoid unused private fields", Justification = "OK - SourceGenerator declaration field")]
public partial class TestFieldsDependencyPropertiesView
{
    public TestFieldsDependencyPropertiesView()
    {
        InitializeComponent();

        IsRunning = false;
    }

    [DependencyProperty(DefaultValue = true)]
    private bool isRunning;

    [DependencyProperty(DefaultValue = 1.1)]
    private decimal decimalValue;

    [DependencyProperty(DefaultValue = 1.1)]
    private float floatValue;

    [DependencyProperty(DefaultValue = 1)]
    private int intValue;

    [DependencyProperty]
    private LogItem logItem;

    [DependencyProperty(DefaultValue = LogCategoryType.Debug)]
    private LogCategoryType logCategory;

    [DependencyProperty(DefaultValue = "Hello world")]
    private string stringValue;

    [DependencyProperty(DefaultValue = "error;err:")]
    private IList<string> errorTerms;

    [DependencyProperty(DefaultValue = "Red")]
    private Color errorTextColor;

    [DependencyProperty(DefaultValue = "Red")]
    private Brush errorTextBrush;

    [DependencyProperty(DefaultValue = "Consolas")]
    private FontFamily myFontFamily;

    [DependencyProperty(DefaultValue = 12.2)]
    private double myFontSize;
}