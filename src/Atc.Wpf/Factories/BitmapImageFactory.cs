using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Media.Imaging;

namespace Atc.Wpf.Factories
{
    public static class BitmapImageFactory
    {
        [SuppressMessage("Design", "CA1054:URI-like parameters should not be strings", Justification = "OK.")]
        public static BitmapImage Create(string uriLocation, UriKind uriKind = UriKind.RelativeOrAbsolute)
        {
            if (uriLocation is null)
            {
                throw new ArgumentNullException(nameof(uriLocation));
            }

            var bitmapImage = new BitmapImage(
                new Uri(
                    uriLocation,
                    uriKind));

            return uriKind is UriKind.Absolute
                ? bitmapImage
                : bitmapImage
                    .EnsureRelativeUriLocation()
                    .ToBitmapImage();
        }
    }
}