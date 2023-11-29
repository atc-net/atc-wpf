namespace Atc.Wpf.Controls.BaseControls;

/// <summary>
/// Interaction logic for FilePicker.
/// </summary>
public partial class FilePicker
{
    public FilePicker()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value),
        typeof(FileInfo),
        typeof(FilePicker),
        new PropertyMetadata(default(FileInfo?)));

    [SuppressMessage("Naming", "CA1721:Property names should not match get methods", Justification = "OK.")]
    public FileInfo? Value
    {
        get => (FileInfo?)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    private void OnClick(object sender, RoutedEventArgs e)
    {
        var fileDialog = new OpenFileDialog
        {
            Title = "Select File",
            Multiselect = false,
        };

        var dialogResult = fileDialog.ShowDialog();
        if (dialogResult.HasValue && dialogResult.Value)
        {
            Value = new FileInfo(fileDialog.FileName);
        }
    }
}