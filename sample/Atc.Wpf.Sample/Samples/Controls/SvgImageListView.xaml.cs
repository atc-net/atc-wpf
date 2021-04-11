using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Atc.Wpf.Controls;
using Microsoft.Win32;

namespace Atc.Wpf.Sample.Samples.Controls
{
    /// <summary>
    /// Interaction logic for SvgImageListView.
    /// </summary>
    public partial class SvgImageListView
    {
        public SvgImageListView()
        {
            this.InitializeComponent();

            this.DataContext = this;

            this.Loaded += this.OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var currentDomainBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var indexOfBin = currentDomainBaseDirectory.IndexOf("bin", StringComparison.Ordinal);
            var basePath = currentDomainBaseDirectory.Substring(0, indexOfBin);
            var assetPath = Path.Combine(basePath, "Assets");
            if (!Directory.Exists(assetPath))
            {
                return;
            }

            var svgFiles = new DirectoryInfo(assetPath).GetFiles("*.svg");
            this.LoadFiles(svgFiles);
        }

        private void BtnBrowseOnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Svg files|*.svg",
            };

            if (openFileDialog.ShowDialog() != true)
            {
                return;
            }

            var fileNames = openFileDialog.FileNames;
            if (fileNames.Length <= 0)
            {
                return;
            }

            var svgFiles = fileNames
                .Select(fileName => new FileInfo(fileName))
                .ToArray();

            this.LoadFiles(svgFiles);
        }

        private void LoadFiles(IEnumerable<FileInfo> svgFiles)
        {
            this.UsagePanel.Children.Clear();
            foreach (var svgFile in svgFiles)
            {
                var svgImage = new SvgImage();
                svgImage.SetImage(svgFile.FullName);
                svgImage.Margin = new Thickness(10);
                svgImage.Width = 80;
                svgImage.Height = 80;
                this.UsagePanel.Children.Add(svgImage);
            }
        }
    }
}