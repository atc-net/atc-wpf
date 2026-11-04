namespace Atc.Wpf.Sample.SamplesWpfComponents.DialogBoxes;

/// <summary>
/// Opens every dialog box with short and long texts (one long line, several long lines, a long path with no
/// spaces, text long enough to scroll), in a default or narrow dialog and left-to-right or right-to-left, so the
/// text can be checked for wrapping in both themes.
/// </summary>
public sealed partial class LongTextDialogBoxViewModel : ViewModelBase
{
    private const string Title = "Long text";

    private const string HeaderText = "A header that is long enough to wrap onto a second line";

    [ObservableProperty]
    private string selectedTextKind = "OneLongLine";

    [ObservableProperty]
    private bool useNarrowDialog;

    [ObservableProperty]
    private bool isRightToLeft;

    [ObservableProperty]
    private bool autoHeight = true;

    [ObservableProperty]
    private string result = string.Empty;

    /// <summary>
    /// Gets the text kinds that the dialogs can show.
    /// </summary>
    public IReadOnlyList<string> TextKinds { get; } =
    [
        "Short",
        "OneLongLine",
        "SeveralLongLines",
        "LongPath",
        "VeryLong",
    ];

    /// <summary>
    /// Gets the icons an information dialog can show: none, or one per log category.
    /// </summary>
    public IReadOnlyList<string> InfoIcons { get; } =
    [
        "None",
        .. Enum.GetNames<LogCategoryType>(),
    ];

    private string Text
        => SelectedTextKind switch
        {
            "Short" => "Are you sure to delete item?",
            "OneLongLine" => "4 of 4 cameras record or wait for motion to record (3 recording now). Closing CameraWall stops this. Close anyway?",
            "SeveralLongLines" =>
                "The first line is long enough to wrap at the default dialog width of the information dialog.\n" +
                "The second line is also long enough to wrap at the default dialog width of the information dialog.\n" +
                "The third line ends the message.",
            "LongPath" => @"C:\ProgramData\AtcSoft\Video\Surveillance\Recordings\Camera-0001\2026\11\04\19\recording-0001-segment-000123456789.mp4",
            "VeryLong" => string.Join(
                "\n",
                Enumerable.Range(1, 60).Select(i => $"Line {i}: this text is long enough to scroll once the dialog has grown to its maximum height.")),
            _ => string.Empty,
        };

    [RelayCommand]
    private void ShowInfo(string icon)
    {
        var settings = icon == "None"
            ? new DialogBoxSettings(DialogBoxType.Ok) { ContentSvgImage = null }
            : new DialogBoxSettings(DialogBoxType.Ok, Enum.Parse<LogCategoryType>(icon));
        settings.TitleBarText = $"{Title} - {icon}";

        Show(new InfoDialogBox(Application.Current.MainWindow!, ApplySettings(settings), Text));
    }

    [RelayCommand]
    private void ShowQuestion(string constructor)
    {
        var owner = Application.Current.MainWindow!;
        var dialogBox = constructor switch
        {
            "Text" => new QuestionDialogBox(owner, Text),
            "Title" => new QuestionDialogBox(owner, Title, Text),
            "Header" => new QuestionDialogBox(owner, Title, HeaderText, Text),
            _ => new QuestionDialogBox(owner, ApplySettings(new DialogBoxSettings(DialogBoxType.YesNo) { TitleBarText = Title }), Text),
        };

        if (constructor != "Settings")
        {
            // These constructors create their own settings; the width and height apply before the dialog is shown.
            dialogBox.Width = UseNarrowDialog ? 350 : dialogBox.Width;
        }

        Show(dialogBox);
    }

    [RelayCommand]
    private void ShowInfoWithHeader()
        => Show(new InfoDialogBox(Application.Current.MainWindow!, Title, HeaderText, Text));

    [RelayCommand]
    private void ShowInput()
    {
        var dialogBox = new InputDialogBox(
            Application.Current.MainWindow!,
            ApplySettings(new DialogBoxSettings(DialogBoxType.OkCancel) { TitleBarText = Title }),
            new LabelTextBox
            {
                LabelText = "The name shown for this camera on the camera wall and in recordings",
                Text = Text,
            })
        {
            HeaderControl = new ContentControl
            {
                Margin = new Thickness(10, 5, 10, 5),
                Content = new TextBlock { Text = HeaderText, TextWrapping = TextWrapping.Wrap, FontSize = 18 },
            },
        };

        Show(dialogBox);
    }

    [RelayCommand]
    private void ShowInputForm()
    {
        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(
            new List<ILabelControlBase>
            {
                new LabelTextBox { LabelText = "Name", Text = Text },
                new LabelTextBox { LabelText = "Description" },
            });

        Show(new InputFormDialogBox(Application.Current.MainWindow!, Title, HeaderText, labelControlsForm));
    }

    [RelayCommand]
    private void ShowColorPicker()
        => Show(new ColorPickerDialogBox(Application.Current.MainWindow!, Colors.DodgerBlue)
        {
            HeaderControl = CreatePickerHeader(),
        });

    [RelayCommand]
    private void ShowFontPicker()
        => Show(new FontPickerDialogBox(Application.Current.MainWindow!, new FontDescription())
        {
            HeaderControl = CreatePickerHeader(),
        });

    private static ContentControl CreatePickerHeader()
        => new()
        {
            Content = new TextBlock { Text = HeaderText, TextWrapping = TextWrapping.Wrap, FontSize = 18 },
        };

    private DialogBoxSettings ApplySettings(DialogBoxSettings settings)
    {
        if (UseNarrowDialog)
        {
            settings.Width = 350;
        }

        settings.AutoHeight = AutoHeight;
        return settings;
    }

    private void Show(Window dialogBox)
    {
        if (IsRightToLeft)
        {
            dialogBox.FlowDirection = FlowDirection.RightToLeft;
        }

        var dialogResult = dialogBox.ShowDialog();
        Result = dialogBox switch
        {
            QuestionDialogBox question => $"{nameof(QuestionDialogBox)}: {dialogResult}, clicked {question.ClickedButton?.ToString() ?? "none"}, don't ask again {question.IsDontAskAgainChecked}",
            InfoDialogBox info => $"{nameof(InfoDialogBox)}: {dialogResult}, clicked {info.ClickedButton?.ToString() ?? "none"}",
            _ => $"{dialogBox.GetType().Name}: {dialogResult}",
        };
    }

    [RelayCommand]
    private void ShowYesNoCancel()
        => Show(new QuestionDialogBox(
            Application.Current.MainWindow!,
            ApplySettings(new DialogBoxSettings(DialogBoxType.YesNoCancel)
            {
                TitleBarText = "Unsaved changes",
                AffirmativeButtonText = "Save",
                NegativeButtonText = "Don't save",
            }),
            "Save the changes to Camera layout 3 before closing?"));

    [RelayCommand]
    private void ShowDestructiveQuestion()
        => Show(new QuestionDialogBox(
            Application.Current.MainWindow!,
            ApplySettings(new DialogBoxSettings(DialogBoxType.YesNo)
            {
                TitleBarText = "Delete recordings",
                DefaultButton = DialogBoxButton.Negative,
            }),
            "Delete 40 recordings? This cannot be undone. Enter answers No."));

    [RelayCommand]
    private void ShowErrorWithDetails()
        => Show(new InfoDialogBox(
            Application.Current.MainWindow!,
            ApplySettings(new DialogBoxSettings(DialogBoxType.Ok, LogCategoryType.Error)
            {
                TitleBarText = "Error",
                DetailsText = string.Join(
                    "\n",
                    "System.InvalidOperationException: The camera stream could not be opened because the RTSP server at rtsp://192.168.1.20:554/stream1 did not answer within 10 seconds.",
                    "   at AtcSoft.Video.Streams.RtspClient.OpenAsync(Uri uri, CancellationToken cancellationToken)",
                    "   at AtcSoft.Video.Surveillance.CameraWall.StartAsync()"),
            }),
            "The camera stream could not be opened. Press Ctrl+C to copy this message."));

    [RelayCommand]
    private void ShowExceptionDialog()
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
            ExceptionDialog.Show(ex, "Dispatcher Unhandled Exception");
            Result = $"{nameof(ExceptionDialog)}: closed";
        }
    }

    [RelayCommand]
    private void ShowQuestionWithDontAskAgain()
        => Show(new QuestionDialogBox(
            Application.Current.MainWindow!,
            ApplySettings(new DialogBoxSettings(DialogBoxType.YesNo)
            {
                TitleBarText = "Close camera wall",
                ShowDontAskAgain = true,
            }),
            "Close the camera wall?"));
}