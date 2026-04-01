namespace Atc.Wpf.Forms;

public partial class LabelTextInfo : ILabelTextInfo
{
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
                    Source = control,
                    Converter = StringNullOrEmptyToInverseBoolValueConverter.Instance,
                });

            copyMenuItem.Click += control.OnCopyToClipboardClick;

            var contextMenu = new ContextMenu();
            contextMenu.Items.Add(copyMenuItem);
            control.ContextMenu = contextMenu;
        }
        else
        {
            control.ContextMenu = null;
        }
    }

    private void OnCopyToClipboardClick(
        object sender,
        RoutedEventArgs e)
        => System.Windows.Clipboard.SetText(Text);
}