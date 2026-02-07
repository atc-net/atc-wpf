// ReSharper disable AsyncVoidEventHandlerMethod
namespace Atc.Wpf.Sample.SamplesWpfControls.Progressing;

public partial class SkeletonView
{
    public SkeletonView()
    {
        InitializeComponent();
    }

    private async void OnLoadDataClick(
        object sender,
        RoutedEventArgs e)
    {
        BtnLoadData.IsEnabled = false;
        TbStatus.Text = "Loading...";
        SkeletonContainer.IsLoading = true;

        await Task
            .Delay(2000)
            .ConfigureAwait(true);

        SkeletonContainer.IsLoading = false;
        TbStatus.Text = "Loaded";
        BtnLoadData.IsEnabled = true;
    }
}