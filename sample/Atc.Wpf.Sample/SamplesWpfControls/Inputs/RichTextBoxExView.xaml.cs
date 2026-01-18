namespace Atc.Wpf.Sample.SamplesWpfControls.Inputs;

public partial class RichTextBoxExView
{
    public RichTextBoxExView()
    {
        InitializeComponent();
        DataContext = this;
    }

    public static string XamlCode =>
        """
        <UserControl x:Class="Atc.Wpf.Sample.SamplesWpf.Controls.GridExView"
                     xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                     xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                     xmlns:atc="https://github.com/atc-net/atc-wpf"
                     xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
                     xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
                     d:DesignHeight="450"
                     d:DesignWidth="800"
                     mc:Ignorable="d">
        
            <atc:GridEx Margin="5"
                        Columns="*,*"
                        Rows="*,*">
        
                <Rectangle Grid.Row="0"
                           Grid.Column="0"
                           Fill="Blue" />
                <Rectangle Grid.Row="0"
                           Grid.Column="1"
                           Fill="Red" />
                <Rectangle Grid.Row="1"
                           Grid.Column="0"
                           Fill="Green" />
                <Rectangle Grid.Row="1"
                           Grid.Column="2"
                           Fill="Yellow" />
            </atc:GridEx>

        </UserControl>

        """;

    public static string CSharpCode =>
        """
        namespace Atc.Wpf.Sample.SamplesWpf.Layouts
        {
            public partial class GridExView
            {
                public GridExView()
                {
                    this.InitializeComponent();
                }
            }
        }
        """;

    public static string JsonText =>
        """
        {
          "Application": {
            "String": "Hello world",
            "Boolean": true,
            "Integer": 27,
            "Float": 5.5,
            "Guid": "56EEDF56-3633-4BCF-9BF0-3AD69C9AE769",
            "Uri": "https://google.com/",
            "Date": "2000-10-31T01:30:00.000-05:00",
            "Data": [
                { "Item": "Value1" },
                { "Item": "Value2" }
            ]
          }
        }
        """;
}