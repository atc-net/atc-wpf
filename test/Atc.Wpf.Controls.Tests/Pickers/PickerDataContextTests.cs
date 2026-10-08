namespace Atc.Wpf.Controls.Tests.Pickers;

public sealed class PickerDataContextTests : IDisposable
{
    public PickerDataContextTests()
        => RuntimeHelpers.RunClassConstructor(typeof(Application).TypeHandle);

    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void FilePicker_BindingOnThePicker_UsesTheInheritedDataContext()
    {
        var sut = new FilePicker();
        sut.SetBinding(FilePicker.WatermarkTextProperty, new Binding(nameof(PickerPageViewModel.Watermark)));

        _ = new ContentControl { DataContext = new PickerPageViewModel(), Content = sut };
        ProcessBindings();

        Assert.Equal(PickerPageViewModel.WatermarkValue, sut.WatermarkText);
    }

    [StaFact]
    public void FilePicker_InnerTextBox_ShowsThePickerWatermark()
    {
        var sut = new FilePicker { WatermarkText = "Pick a file" };
        _ = new ContentControl { DataContext = new PickerPageViewModel(), Content = sut };
        ProcessBindings();

        Assert.Equal("Pick a file", TextBoxHelper.GetWatermark(InnerTextBox(sut)));
    }

    [StaFact]
    public void DirectoryPicker_BindingOnThePicker_UsesTheInheritedDataContext()
    {
        var sut = new DirectoryPicker();
        sut.SetBinding(DirectoryPicker.WatermarkTextProperty, new Binding(nameof(PickerPageViewModel.Watermark)));

        _ = new ContentControl { DataContext = new PickerPageViewModel(), Content = sut };
        ProcessBindings();

        Assert.Equal(PickerPageViewModel.WatermarkValue, sut.WatermarkText);
    }

    [StaFact]
    public void DirectoryPicker_InnerTextBox_ShowsThePickerWatermark()
    {
        var sut = new DirectoryPicker { WatermarkText = "Pick a folder" };
        _ = new ContentControl { DataContext = new PickerPageViewModel(), Content = sut };
        ProcessBindings();

        Assert.Equal("Pick a folder", TextBoxHelper.GetWatermark(InnerTextBox(sut)));
    }

    private static void ProcessBindings()
        => Dispatcher.CurrentDispatcher.Invoke(() => { }, DispatcherPriority.ContextIdle);

    private static TextBox InnerTextBox(UserControl picker)
        => LogicalTreeHelper.GetChildren((DependencyObject)picker.Content)
            .OfType<TextBox>()
            .Single();
}