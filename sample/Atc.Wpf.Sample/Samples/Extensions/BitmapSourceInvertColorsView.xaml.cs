using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Atc.Wpf.Sample.Samples.Extensions
{
    /// <summary>
    /// Interaction logic for BitmapSourceInvertColorsView.
    /// </summary>
    public partial class BitmapSourceInvertColorsView
    {
        public BitmapSourceInvertColorsView()
        {
            this.InitializeComponent();

            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var bitmap = new BitmapImage(
                new Uri(@"pack://application:,,,/Atc.Wpf.Sample;component/Assets/the-road_640.jpg", UriKind.Absolute));

            var bitmapSource = bitmap.InvertColors();

            this.TestImage.Source = bitmapSource;
        }
    }
}