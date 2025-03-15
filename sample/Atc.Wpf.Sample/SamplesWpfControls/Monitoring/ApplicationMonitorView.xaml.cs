namespace Atc.Wpf.Sample.SamplesWpfControls.Monitoring
{
    public partial class ApplicationMonitorView
    {
        public ApplicationMonitorView()
        {
            InitializeComponent();

            DataContext = this;

            ApplicationMonitorViewModel = new ApplicationMonitorViewModel();
        }

        public ApplicationMonitorViewModel ApplicationMonitorViewModel { get; set; }

        public IRelayCommand AddOneCommand => new RelayCommand(AddOneCommandHandler);

        public IRelayCommand AddManyCommand => new RelayCommand(AddManyCommandHandler);

        private static void AddOneCommandHandler()
            => Messenger.Default.Send(
                new ApplicationEventEntry(LogCategoryType.Information, "Area1", $"Hello world - {Guid.NewGuid()}"));

        private static void AddManyCommandHandler()
        {
            foreach (var enumItem in Enum.GetValues<LogCategoryType>())
            {
                Messenger.Default.Send(new ApplicationEventEntry(enumItem, "Area2", $"Hello world - {Guid.NewGuid()}"));
            }
        }
    }
}