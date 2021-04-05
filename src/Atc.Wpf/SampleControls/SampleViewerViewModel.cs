using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Atc.Wpf.Messaging;
using Atc.Wpf.Mvvm;

namespace Atc.Wpf.SampleControls
{
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "OK.")]
    public class SampleViewerViewModel : ViewModelBase
    {
        public SampleViewerViewModel()
        {
            Messenger.Default.Register<SampleItemMessage>(this, SampleItemMessageHandler);
        }

        private string? header;
        private UserControl? sampleContent;
        private string? xamlCode;
        private string? codeBehindCode;
        private string? viewModelCode;

        public bool HasSampleContent => this.SampleContent != null;

        public bool HasXamlCode => this.XamlCode != null;

        public bool HasCodeBehindCode => this.CodeBehindCode != null;

        public bool HasViewModelCode => this.ViewModelCode != null;

        public string? Header
        {
            get => this.header;
            set
            {
                this.header = value;
                this.RaisePropertyChanged();
            }
        }

        public UserControl? SampleContent
        {
            get => this.sampleContent;
            set
            {
                this.sampleContent = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(HasSampleContent));
            }
        }

        public string? XamlCode
        {
            get => this.xamlCode;
            set
            {
                this.xamlCode = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(HasXamlCode));
            }
        }

        public string? CodeBehindCode
        {
            get => this.codeBehindCode;
            set
            {
                this.codeBehindCode = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(HasCodeBehindCode));
            }
        }

        public string? ViewModelCode
        {
            get => this.viewModelCode;
            set
            {
                this.viewModelCode = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(HasViewModelCode));
            }
        }

        private string ExtractClassName(string classFullName)
        {
            return classFullName.Split('.').Last();
        }

        private DirectoryInfo? ExtractBasePath(DirectoryInfo path)
        {
            if ("bin".Equals(path.Name, StringComparison.Ordinal))
            {
                return path.Parent;
            }

            return path.Parent == null
                ? null
                : this.ExtractBasePath(path.Parent);
        }

        private DirectoryInfo? ExtractSamplePath(FileSystemInfo baseLocation, string classViewName)
        {
            var files = Directory.GetFiles(baseLocation.FullName, $"{classViewName}.xaml", SearchOption.AllDirectories);
            return files.Length == 1
                ? new DirectoryInfo(files[0]).Parent
                : null;
        }

        private string? ReadFileText(string filePath)
        {
            return File.Exists(filePath)
                ? File.ReadAllText(filePath)
                : null;
        }

        private void SampleItemMessageHandler(SampleItemMessage obj)
        {
            if (string.IsNullOrEmpty(obj.SampleItemPath))
            {
                this.ClearSelectedViewData();
            }
            else
            {
                this.SetSelectedViewData(obj.Header, obj.SampleItemPath);
            }
        }

        private void ClearSelectedViewData()
        {
            this.Header = null;
            this.SampleContent = null;
            this.XamlCode = null;
            this.CodeBehindCode = null;
            this.ViewModelCode = null;
        }

        private void SetSelectedViewData(string sampleHeader, string samplePath)
        {
            var entryAssembly = Assembly.GetEntryAssembly();

            var sampleType = entryAssembly!
                .GetExportedTypes()
                .FirstOrDefault(x => x.FullName != null && x.FullName.EndsWith(samplePath, StringComparison.Ordinal));

            if (sampleType is null)
            {
                _ = MessageBox.Show($"Can't find sample by path '{samplePath}'", "Error", MessageBoxButton.OK);
                return;
            }

            if (!(Activator.CreateInstance(sampleType) is UserControl instance))
            {
                _ = MessageBox.Show($"Can't create instance of sample by path '{samplePath}'", "Error", MessageBoxButton.OK);
                return;
            }

            var entryAssemblyLocation = new DirectoryInfo(Path.GetDirectoryName(entryAssembly.Location)!);
            var baseLocation = this.ExtractBasePath(entryAssemblyLocation);
            if (baseLocation is null)
            {
                _ = MessageBox.Show($"Can't find sample by invalid base location", "Error", MessageBoxButton.OK);
                return;
            }

            var classViewName = this.ExtractClassName(instance.ToString()!);
            var sampleLocation = this.ExtractSamplePath(baseLocation, classViewName);

            this.Header = sampleHeader;
            this.SampleContent = instance;
            this.XamlCode = this.ReadFileText(Path.Combine(sampleLocation!.FullName, classViewName + ".xaml"));
            this.CodeBehindCode = this.ReadFileText(Path.Combine(sampleLocation!.FullName, classViewName + ".xaml.cs"));

            if (instance.DataContext == null ||
                nameof(SampleViewerViewModel).Equals(this.ExtractClassName(instance.DataContext.ToString()!), StringComparison.Ordinal))
            {
                this.ViewModelCode = null;
            }
            else
            {
                var classViewModelName = this.ExtractClassName(instance.DataContext.ToString()!);
                this.ViewModelCode = this.ReadFileText(Path.Combine(sampleLocation!.FullName, classViewModelName + ".cs"));
            }
        }
    }
}