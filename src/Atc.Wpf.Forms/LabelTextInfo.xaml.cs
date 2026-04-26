namespace Atc.Wpf.Forms;

public partial class LabelTextInfo : ILabelTextInfo
{
    private ContextMenu? cachedCopyContextMenu;

    [DependencyProperty(DefaultValue = false, PropertyChangedCallback = nameof(OnEnableCopyToClipboardChanged))]
    private bool enableCopyToClipboard;

    [DependencyProperty(DefaultValue = "")]
    private string text;

    public LabelTextInfo()
    {
        InitializeComponent();
    }

    private static void OnEnableCopyToClipboardChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelTextInfo)d;
        if (control.EnableCopyToClipboard)
        {
            control.ContextMenu = control.cachedCopyContextMenu ??= control.BuildCopyContextMenu();
        }
        else
        {
            control.ContextMenu = null;
        }
    }

    private ContextMenu BuildCopyContextMenu()
    {
        var copyMenuItem = new MenuItem
        {
            Header = Miscellaneous.CopyToClipboard,
            Icon = new SvgImage
            {
                Width = 16,
                Height = 16,
                Source = "/Atc.Wpf.Controls;component/Resources/Icons/clipboard.svg",
            },
        };

        copyMenuItem.SetBinding(
            MenuItem.IsEnabledProperty,
            new Binding(nameof(Text))
            {
                Source = this,
                Converter = StringNullOrEmptyToInverseBoolValueConverter.Instance,
            });

        copyMenuItem.Click += OnCopyToClipboardClick;

        var contextMenu = new ContextMenu();
        contextMenu.Items.Add(copyMenuItem);
        return contextMenu;
    }

    private void OnCopyToClipboardClick(
        object sender,
        RoutedEventArgs e)
        => System.Windows.Clipboard.SetText(Text);
}