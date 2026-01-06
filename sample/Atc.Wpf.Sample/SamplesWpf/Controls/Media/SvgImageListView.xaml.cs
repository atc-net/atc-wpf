namespace Atc.Wpf.Sample.SamplesWpf.Controls.Media;

public partial class SvgImageListView
{
    public SvgImageListView()
    {
        InitializeComponent();

        DataContext = this;

        Loaded += OnLoaded;
    }

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        var currentDomainBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var indexOfBin = currentDomainBaseDirectory.IndexOf("bin", StringComparison.Ordinal);
        var basePath = currentDomainBaseDirectory.Substring(0, indexOfBin);
        var assetPath = Path.Combine(basePath, "Assets");
        if (!Directory.Exists(assetPath))
        {
            return;
        }

        var svgFiles = new DirectoryInfo(assetPath).GetFiles("*.svg");
        LoadFiles(svgFiles);
    }

    private void BtnBrowseOnClick(
        object sender,
        RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog
        {
            Multiselect = true,
            Filter = "Svg files|*.svg",
        };

        if (openFileDialog.ShowDialog() != true)
        {
            return;
        }

        var fileNames = openFileDialog.FileNames;
        if (fileNames.Length <= 0)
        {
            return;
        }

        var svgFiles = fileNames
            .Select(fileName => new FileInfo(fileName))
            .ToArray();

        LoadFiles(svgFiles);
    }

    private void LoadFiles(IEnumerable<FileInfo> svgFiles)
    {
        UsagePanel.Children.Clear();
        foreach (var svgFile in svgFiles)
        {
            var svgImage = new SvgImage();
            svgImage.SetImage(svgFile.FullName);
            svgImage.Margin = new Thickness(10);
            svgImage.Width = 80;
            svgImage.Height = 80;
            UsagePanel.Children.Add(svgImage);
        }
    }
}