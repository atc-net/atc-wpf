namespace Atc.Wpf.Sample.SamplesWpf.Controls.Layouts;

[SuppressMessage("Security", "CA5394:Do not use insecure randomness", Justification = "Sample app - not security sensitive")]
[SuppressMessage("Security", "SCS0005:Weak random number generator", Justification = "Sample app - not security sensitive")]
public partial class VirtualizingStaggeredPanelView
{
    private static readonly Brush[] Colors =
    [
        new SolidColorBrush(Color.FromRgb(66, 133, 244)),   // Blue
        new SolidColorBrush(Color.FromRgb(219, 68, 55)),    // Red
        new SolidColorBrush(Color.FromRgb(244, 180, 0)),    // Yellow
        new SolidColorBrush(Color.FromRgb(15, 157, 88)),    // Green
        new SolidColorBrush(Color.FromRgb(171, 71, 188)),   // Purple
        new SolidColorBrush(Color.FromRgb(255, 112, 67)),   // Orange
        new SolidColorBrush(Color.FromRgb(0, 172, 193)),    // Cyan
        new SolidColorBrush(Color.FromRgb(124, 179, 66)),   // Light Green
    ];

    private readonly Random random = new();

    public VirtualizingStaggeredPanelView()
    {
        InitializeComponent();

        LsItemCount.ValueChanged += (_, _) => GenerateItems();
        Loaded += (_, _) => GenerateItems();
    }

    private void GenerateItems()
    {
        var count = Convert.ToInt32(LsItemCount.Value);
        var items = new List<StaggeredItem>(count);

        for (var i = 0; i < count; i++)
        {
            items.Add(new StaggeredItem
            {
                Title = $"Item {i + 1}",
                Description = $"Height: {GetRandomHeight()}px",
                Height = GetRandomHeight(),
                Color = Colors[i % Colors.Length],
            });
        }

        ItemsControlPanel.ItemsSource = items;
    }

    private int GetRandomHeight() => random.Next(60, 200);

    private sealed class StaggeredItem
    {
        public required string Title { get; init; }

        public required string Description { get; init; }

        public required int Height { get; init; }

        public required Brush Color { get; init; }
    }
}