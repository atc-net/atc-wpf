namespace Atc.Wpf.Components.Tests.Selectors;

/// <summary>
/// Hosts a <see cref="DualListSelector"/> with a minimal template (only the named parts) in an off-screen
/// window, so the lists are measured with a bounded height and can virtualize.
/// </summary>
internal sealed class DualListSelectorTestHost : IDisposable
{
    private const string TemplateXaml = """
        <ControlTemplate
            xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
            xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
            TargetType="Control">
            <UniformGrid Columns="2">
                <DockPanel>
                    <CheckBox x:Name="PART_AvailableSelectAll" DockPanel.Dock="Top" />
                    <TextBlock x:Name="PART_AvailableHeaderCount" DockPanel.Dock="Top" />
                    <TextBox x:Name="PART_AvailableFilterTextBox" DockPanel.Dock="Top" />
                    <TextBlock x:Name="PART_AvailableItemCount" DockPanel.Dock="Bottom" />
                    <TextBlock x:Name="PART_AvailableEmptyText" DockPanel.Dock="Bottom" />
                    <ListBox x:Name="PART_AvailableListBox" Height="200" SelectionMode="Extended" ItemsSource="{Binding AvailableItems, RelativeSource={RelativeSource TemplatedParent}}" />
                </DockPanel>
                <DockPanel>
                    <CheckBox x:Name="PART_SelectedSelectAll" DockPanel.Dock="Top" />
                    <TextBlock x:Name="PART_SelectedHeaderCount" DockPanel.Dock="Top" />
                    <TextBox x:Name="PART_SelectedFilterTextBox" DockPanel.Dock="Top" />
                    <TextBlock x:Name="PART_SelectedItemCount" DockPanel.Dock="Bottom" />
                    <TextBlock x:Name="PART_SelectedEmptyText" DockPanel.Dock="Bottom" />
                    <ListBox x:Name="PART_SelectedListBox" Height="200" SelectionMode="Extended" ItemsSource="{Binding SelectedItems, RelativeSource={RelativeSource TemplatedParent}}" />
                </DockPanel>
            </UniformGrid>
        </ControlTemplate>
        """;

    private readonly Window window;

    public DualListSelectorTestHost(Action<DualListSelector>? configure = null)
    {
        Selector = new DualListSelector
        {
            Template = (ControlTemplate)XamlReader.Parse(TemplateXaml),
        };
        configure?.Invoke(Selector);

        window = new Window
        {
            Content = Selector,
            Width = 600,
            Height = 400,
            Left = -10000,
            Top = -10000,
            ShowActivated = false,
            ShowInTaskbar = false,
            WindowStyle = WindowStyle.None,
        };
        window.Show();
        window.UpdateLayout();
    }

    public DualListSelector Selector { get; }

    public ListBox AvailableList
        => Part<ListBox>("PART_AvailableListBox");

    public ListBox SelectedList
        => Part<ListBox>("PART_SelectedListBox");

    public CheckBox AvailableSelectAll
        => Part<CheckBox>("PART_AvailableSelectAll");

    public CheckBox SelectedSelectAll
        => Part<CheckBox>("PART_SelectedSelectAll");

    public TextBlock AvailableHeaderCount
        => Part<TextBlock>("PART_AvailableHeaderCount");

    public TextBlock SelectedHeaderCount
        => Part<TextBlock>("PART_SelectedHeaderCount");

    public TextBox AvailableFilter
        => Part<TextBox>("PART_AvailableFilterTextBox");

    public TextBlock SelectedItemCount
        => Part<TextBlock>("PART_SelectedItemCount");

    public static ObservableCollection<DualListSelectorItem> CreateItems(
        string prefix,
        int count)
        => new(Enumerable
            .Range(0, count)
            .Select(i => new DualListSelectorItem
            {
                Identifier = $"{prefix}{i}",
                Name = $"{prefix} {i}",
            }));

    public static void ToggleViaAutomation(CheckBox checkBox)
    {
        var peer = (IToggleProvider)new CheckBoxAutomationPeer(checkBox).GetPattern(PatternInterface.Toggle);
        peer.Toggle();
    }

    public void Flush()
    {
        window.UpdateLayout();
        Dispatcher.CurrentDispatcher.Invoke(() => { }, DispatcherPriority.ContextIdle);
    }

    public void RaiseKey(
        UIElement target,
        Key key)
    {
        var source = PresentationSource.FromVisual(window)!;
        target.RaiseEvent(new KeyEventArgs(Keyboard.PrimaryDevice, source, 0, key)
        {
            RoutedEvent = Keyboard.PreviewKeyDownEvent,
        });
    }

    public void Dispose()
        => window.Close();

    private T Part<T>(string name)
        where T : class
        => (T)Selector.Template.FindName(name, Selector);
}