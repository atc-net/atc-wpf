namespace Atc.Wpf.Controls.Viewers;

/// <summary>
/// Interaction logic for JsonViewer.xaml
/// </summary>
public partial class JsonViewer
{
    public JsonViewer()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
        nameof(Data),
        typeof(string),
        typeof(JsonViewer),
        new PropertyMetadata(OnDataChanged));

    public string Data
    {
        get => (string)GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK.")]
    public void Load(string json)
    {
        JsonTreeView.ItemsSource = null;
        JsonTreeView.Items.Clear();

        var children = new List<JToken>();

        try
        {
            var token = JToken.Parse(json);

            if (token != null)
            {
                children.Add(token);
            }

            JsonTreeView.ItemsSource = children;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Could not open the JSON string:\r\n" + ex.Message);
        }
    }

    private static void OnDataChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs args)
    {
        if (args.NewValue is null)
        {
            return;
        }

        var jsonData = args.NewValue.ToString();
        if (string.IsNullOrEmpty(jsonData))
        {
            return;
        }

        var control = (JsonViewer)d;
        control.Load(jsonData);
        control.ToggleItems(isExpanded: true);
    }

    private void ToggleItems(
        bool isExpanded)
    {
        if (JsonTreeView.Items.IsEmpty)
        {
            return;
        }

        var prevCursor = Cursor;
        Cursor = Cursors.Wait;
        ToggleItems(JsonTreeView, JsonTreeView.Items, isExpanded);
        Cursor = prevCursor;
    }

    private void ToggleItems(
        ItemsControl parentContainer,
        IEnumerable items,
        bool isExpanded)
    {
        var itemGen = parentContainer.ItemContainerGenerator;
        if (itemGen.Status == GeneratorStatus.ContainersGenerated)
        {
            Recurse(items, isExpanded, itemGen);
        }
        else
        {
            itemGen.StatusChanged += (_, _) => Recurse(items, isExpanded, itemGen);
        }
    }

    private void Recurse(
        IEnumerable items,
        bool isExpanded,
        ItemContainerGenerator itemContainerGenerator)
    {
        if (itemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
        {
            return;
        }

        foreach (var item in items)
        {
            if (itemContainerGenerator.ContainerFromItem(item) is not TreeViewItem tvi)
            {
                continue;
            }

            tvi.IsExpanded = isExpanded;
            ToggleItems(tvi, tvi.Items, isExpanded);
        }
    }
}