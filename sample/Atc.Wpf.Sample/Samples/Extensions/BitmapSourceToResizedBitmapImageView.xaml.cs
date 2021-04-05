using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Atc.Wpf.Sample.Samples.Extensions
{
    /// <summary>
    /// Interaction logic for BitmapSourceToResizedBitmapImageView.
    /// </summary>
    public partial class BitmapSourceToResizedBitmapImageView
    {
        public BitmapSourceToResizedBitmapImageView()
        {
            this.InitializeComponent();

            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var bitmap = new BitmapImage(
                new Uri(@"pack://application:,,,/Atc.Wpf.Sample;component/Assets/the-road_640.jpg", UriKind.Absolute));

            int newWidth = (int)bitmap.Width / 2;
            var bitmapSource = bitmap.ToResizedBitmapImage(newWidth);

            this.TestImage.Source = bitmapSource;
        }
    }
}