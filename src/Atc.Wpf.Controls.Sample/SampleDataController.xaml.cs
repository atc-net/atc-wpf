namespace Atc.Wpf.Controls.Sample;

public partial class SampleDataController
{
    private ISampleDataGenerator? generator;

    [DependencyProperty(PropertyChangedCallback = nameof(OnSourceObjectChanged))]
    private object? sourceObject;

    public SampleDataController()
    {
        InitializeComponent();
    }

    private static void OnSourceObjectChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (SampleDataController)d;
        control.RebuildPanel();
    }

    private void RebuildPanel()
    {
        ContentPanel.Children.Clear();
        generator = null;

        if (SourceObject is null)
        {
            Visibility = Visibility.Collapsed;
            return;
        }

        var attr = SourceObject
            .GetType()
            .GetCustomAttribute<SampleDataGeneratorAttribute>();

        if (attr is null)
        {
            Visibility = Visibility.Collapsed;
            return;
        }

        generator = (ISampleDataGenerator)Activator.CreateInstance(attr.GeneratorType)!;
        Visibility = Visibility.Visible;

        if (generator.CanPopulateSampleData)
        {
            AddButton("📦 Populate Sample Data", OnPopulateSampleData);
        }

        if (generator.CanAddItems)
        {
            AddButton("➕ Add Item", OnAddItem);
        }

        if (generator.CanRemoveItems)
        {
            AddButton("➖ Remove Item", OnRemoveItem);
        }

        if (generator.CanReset)
        {
            AddButton("🔄 Reset", OnReset);
        }
    }

    private void AddButton(
        string text,
        RoutedEventHandler handler)
    {
        var button = new Button { Content = text };
        button.Click += handler;
        ContentPanel.Children.Add(button);
    }

    private void OnAddItem(
        object sender,
        RoutedEventArgs e)
    {
        if (generator is not null && SourceObject is not null)
        {
            generator.AddItem(SourceObject);
        }
    }

    private void OnRemoveItem(
        object sender,
        RoutedEventArgs e)
    {
        if (generator is not null && SourceObject is not null)
        {
            generator.RemoveItem(SourceObject);
        }
    }

    private void OnReset(
        object sender,
        RoutedEventArgs e)
    {
        if (generator is not null && SourceObject is not null)
        {
            generator.Reset(SourceObject);
        }
    }

    private void OnPopulateSampleData(
        object sender,
        RoutedEventArgs e)
    {
        if (generator is not null && SourceObject is not null)
        {
            generator.PopulateSampleData(SourceObject);
        }
    }
}