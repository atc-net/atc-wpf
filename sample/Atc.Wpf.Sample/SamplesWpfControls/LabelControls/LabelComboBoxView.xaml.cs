namespace Atc.Wpf.Sample.SamplesWpfControls.LabelControls;

/// <summary>
/// Interaction logic for LabelComboBoxView.
/// </summary>
public partial class LabelComboBoxView : INotifyPropertyChanged
{
    private IDictionary<string, string> items = new Dictionary<string, string>(StringComparer.Ordinal);
    private string myLabel3HorizontalSelectedKey = string.Empty;
    private string myLabel3VerticalSelectedKey = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    public LabelComboBoxView()
    {
        InitializeComponent();
        DataContext = this;

        Items = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { "Item1", "Item 1" },
                { "Item2", "Item 2" },
                { "Item3", "Item 3" },
            };

        MyLabel3HorizontalSelectedKey = Items.First().Key;
        MyLabel3VerticalSelectedKey = Items.Last().Key;
    }

    public IDictionary<string, string> Items
    {
        get => items;
        set
        {
            items = value;
            OnPropertyChanged();
        }
    }

    public string MyLabel3HorizontalSelectedKey
    {
        get => myLabel3HorizontalSelectedKey;
        set
        {
            myLabel3HorizontalSelectedKey = value;
            OnPropertyChanged();
        }
    }

    public string MyLabel3VerticalSelectedKey
    {
        get => myLabel3VerticalSelectedKey;
        set
        {
            myLabel3VerticalSelectedKey = value;
            OnPropertyChanged();
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void MyLabel3HorizontalOnSelectedKeyChanged(
        object? sender,
        ChangedStringEventArgs e)
    {
        if (string.IsNullOrEmpty(e.OldValue))
        {
            return;
        }

        MessageBox.Show(
            $"{e.Identifier}-Horizontal # From: {e.OldValue}, To: {e.NewValue}",
            "Event: MyLabel3HorizontalOnSelectedKeyChanged");
    }

    private void MyLabel3VerticalOnSelectedKeyChanged(
        object? sender,
        ChangedStringEventArgs e)
    {
        if (string.IsNullOrEmpty(e.OldValue))
        {
            return;
        }

        MessageBox.Show(
            $"{e.Identifier}-Vertical # From: {e.OldValue}, To: {e.NewValue}",
            "Event: MyLabel3VerticalOnSelectedKeyChanged");
    }
}