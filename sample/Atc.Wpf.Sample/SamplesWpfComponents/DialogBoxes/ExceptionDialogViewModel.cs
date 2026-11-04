namespace Atc.Wpf.Sample.SamplesWpfComponents.DialogBoxes;

/// <summary>
/// Shows an exception in the themed <see cref="ExceptionDialog"/> and in the native last-resort message box
/// (<see cref="ExceptionReport.ShowMessageBox"/>), and throws exceptions nobody catches, so the sample app's own
/// handlers in App.xaml.cs run (they close the app).
/// </summary>
public sealed partial class ExceptionDialogViewModel : ViewModelBase
{
    private const string Title = "Exception dialog sample";

    [ObservableProperty]
    private string result = string.Empty;

    [RelayCommand]
    private void ShowExceptionDialog()
    {
        ExceptionDialog.Show(CreateThrownException(), Title);
        Result = $"{nameof(ExceptionDialog)} closed. The report was saved in {ExceptionReport.GetDefaultFolder()}";
    }

    [RelayCommand]
    private void ShowNativeMessageBox()
    {
        var file = ExceptionReport.ShowMessageBox(CreateThrownException(), Title);
        Result = file is null
            ? "Native message box closed. The report could not be saved."
            : $"Native message box closed. The report was saved to {file} and copied to the clipboard.";
    }

    [RelayCommand]
    private void ThrowOnUiThread()
    {
        if (!Confirm("The exception goes to the app's dispatcher handler: it shows the exception dialog and then closes the sample app. Throw it?"))
        {
            return;
        }

        // Thrown from a later dispatcher call, outside the command, so nothing on the way catches it.
        _ = Application.Current.Dispatcher.BeginInvoke(() => throw CreateThrownException());
    }

    [RelayCommand]
    private void ThrowOnBackgroundThread()
    {
        if (!Confirm("The exception goes to AppDomain.UnhandledException: the native message box shows the report and then the process ends. Throw it?"))
        {
            return;
        }

        var thread = new Thread(() => throw CreateThrownException())
        {
            IsBackground = true,
            Name = "Exception dialog sample",
        };

        thread.Start();
    }

    private static bool Confirm(string message)
        => new QuestionDialogBox(
            Application.Current.MainWindow!,
            new DialogBoxSettings(DialogBoxType.YesNo, LogCategoryType.Warning)
            {
                TitleBarText = "This closes the sample app",
                DefaultButton = DialogBoxButton.Negative,
            },
            message).ShowDialog() == true;

    /// <summary>
    /// Creates an exception with an inner exception and real stack traces, as an app would see one.
    /// </summary>
    private static InvalidOperationException CreateThrownException()
    {
        try
        {
            try
            {
                throw new TimeoutException("The RTSP server at rtsp://192.168.1.20:554/stream1 did not answer within 10 seconds.");
            }
            catch (TimeoutException ex)
            {
                throw new InvalidOperationException("The camera stream could not be opened.", ex);
            }
        }
        catch (InvalidOperationException ex)
        {
            return ex;
        }
    }
}