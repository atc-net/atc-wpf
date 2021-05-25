using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Atc.Wpf.Mvvm;

namespace Atc.Wpf.Sample.Samples.Threading
{
    public class DebounceViewModel : ViewModelBase
    {
        private readonly Collection<string> totalItems;
        private string status;
        private string filter;

        public DebounceViewModel()
        {
            this.totalItems = new Collection<string>
            {
                "John Doe",
                "Jane Doe",
                "Len Kaden",
                "Travis Echo",
                "Malik Lenore",
                "Jasper Blair",
                "Harrison Leilani",
                "Cruz Melodie",
                "Hakeem Rose",
                "Rafael Wanda",
            };
            this.FoundItems = new ObservableCollection<string>();
            this.status = string.Empty;
            this.filter = string.Empty;
        }

        public string Status
        {
            get => this.status;
            set
            {
                this.status = value;
                this.RaisePropertyChanged();
            }
        }

        public string Filter
        {
            get => this.filter;
            set
            {
                this.filter = value;
                this.RaisePropertyChanged();
            }
        }

        public ObservableCollection<string> FoundItems { get; }

        public async Task Search(string searchQuery)
        {
            this.Filter = searchQuery;
            this.FoundItems.Clear();

            if (string.IsNullOrEmpty(searchQuery))
            {
                this.Status = "Clear result";
                return;
            }

            this.Status = "Searching...";

            await Task.Delay(1000).ConfigureAwait(true);

            foreach (string item in this.totalItems
                .Where(item => item.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)))
            {
                this.FoundItems.Add(item);
            }

            this.Status = $"Done - found {this.FoundItems.Count}";
        }
    }
}