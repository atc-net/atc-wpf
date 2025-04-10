namespace Atc.Wpf.Sample.SamplesWpfSourceGenerators;

[DependencyProperty<bool>("IsRunning", DefaultValue = true)]
[DependencyProperty<decimal>("DecimalValue", DefaultValue = 1.1)]
[DependencyProperty<float>("FloatValue", DefaultValue = 1.1)]
[DependencyProperty<int>("IntValue", DefaultValue = 1)]
[DependencyProperty<LogItem>("LogItem")]
[DependencyProperty<LogCategoryType>("LogCategory", DefaultValue = LogCategoryType.Debug)]
[DependencyProperty<string>("StringValue", DefaultValue = "Hello world")]
[DependencyProperty<IList<string>>("ErrorTerms", DefaultValue = "error;err:")]
[DependencyProperty<Color>("ErrorTextColor", DefaultValue = "Red")]
[DependencyProperty<Brush>("ErrorTextBrush", DefaultValue = "Red")]
[DependencyProperty<FontFamily>("MyFontFamily", DefaultValue = "Consolas")]
[DependencyProperty<double>("MyFontSize", DefaultValue = 12.2)]
public partial class TestClassDependencyPropertiesView
{
    public TestClassDependencyPropertiesView()
    {
        InitializeComponent();

        IsRunning = false;
    }
}