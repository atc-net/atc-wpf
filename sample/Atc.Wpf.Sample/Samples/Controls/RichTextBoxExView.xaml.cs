namespace Atc.Wpf.Sample.Samples.Controls
{
    /// <summary>
    /// Interaction logic for RichTextBoxExView.
    /// </summary>
    public partial class RichTextBoxExView
    {
        public RichTextBoxExView()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        public static string XamlCode =>
@"<UserControl x:Class=""Atc.Wpf.Sample.Samples.Controls.GridExView""
             xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
             xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
             xmlns:atc=""https://github.com/atc-net/atc-wpf""
             xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
             xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
             d:DesignHeight=""450""
             d:DesignWidth=""800""
             mc:Ignorable=""d"">

    <atc:GridEx Margin=""5""
                Columns=""*,*""
                Rows=""*,*"">

        <Rectangle Grid.Row=""0""
                   Grid.Column=""0""
                   Fill=""Blue"" />
        <Rectangle Grid.Row=""0""
                   Grid.Column=""1""
                   Fill=""Red"" />
        <Rectangle Grid.Row=""1""
                   Grid.Column=""0""
                   Fill=""Green"" />
        <Rectangle Grid.Row=""1""
                   Grid.Column=""2""
                   Fill=""Yellow"" />
    </atc:GridEx>

</UserControl>
";

        public static string CSharpCode =>
@"namespace Atc.Wpf.Sample.Samples.Layouts
{
    /// <summary>
    /// Interaction logic for GridExView.
    /// </summary>
    public partial class GridExView
    {
        public GridExView()
        {
            this.InitializeComponent();
        }
    }
}";
}
}