namespace Atc.Wpf.Controls.Sample;

public partial class SampleSidePanel
{
    [DependencyProperty(PropertyChangedCallback = nameof(OnSourceObjectChanged))]
    private object? sourceObject;

    public SampleSidePanel()
    {
        InitializeComponent();
    }

    private static void OnSourceObjectChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (SampleSidePanel)d;
        control.PropertyController.SourceObject = e.NewValue;
        control.DataController.SourceObject = e.NewValue;

        var hasDataGenerator = e.NewValue?
            .GetType()
            .GetCustomAttribute<SampleDataGeneratorAttribute>() is not null;

        control.DataGroupBox.Visibility = hasDataGenerator
            ? Visibility.Visible
            : Visibility.Collapsed;
    }
}