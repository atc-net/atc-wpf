namespace Atc.Wpf.Sample.SamplesWpfComponents.DialogBoxes;

public partial class InWindowDialogsView
{
    public InWindowDialogsView()
    {
        InitializeComponent();

        // In an app the flyout and dialog services are usually registered in DI for the main window.
        var flyoutService = new FlyoutService(DialogHost);
        DataContext = new InWindowDialogsViewModel(new InWindowDialogService(flyoutService));
    }
}