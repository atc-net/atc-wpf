namespace Atc.Wpf.Sample.SamplesWpfTheming.Window;

public partial class NiceDialogBoxView
{
    public NiceDialogBoxView()
    {
        InitializeComponent();
    }

    private void OnOpenBasicDialogClick(
        object sender,
        RoutedEventArgs e)
    {
        var dialog = CreateBasicDialog("Basic Dialog", "This is a basic NiceDialogBox with default settings.");
        dialog.Show();
    }

    private void OnOpenDialogWithCloseButtonClick(
        object sender,
        RoutedEventArgs e)
    {
        var dialog = CreateBasicDialog("Dialog with Close Button", "This dialog has a close button in the title bar.");
        dialog.ShowCloseButton = true;
        dialog.Show();
    }

    private void OnOpenModalDialogClick(
        object sender,
        RoutedEventArgs e)
    {
        var dialog = CreateBasicDialog("Modal Dialog", "This is a modal dialog. Close it to continue.");
        dialog.ShowCloseButton = true;
        dialog.ShowDialog();
    }

    private void OnOpenConfirmationDialogClick(
        object sender,
        RoutedEventArgs e)
    {
        var dialog = CreateConfirmationDialog();
        var result = dialog.ShowDialog();

        var resultMessage = result == true ? "Yes" : "No";
        var resultDialog = CreateBasicDialog("Result", $"You clicked: {resultMessage}");
        resultDialog.ShowCloseButton = true;
        resultDialog.Show();
    }

    private void OnOpenCustomSizedDialogClick(
        object sender,
        RoutedEventArgs e)
    {
        var dialog = new NiceDialogBox
        {
            Title = "Custom Sized Dialog",
            Width = 500,
            Height = 350,
            ShowCloseButton = true,
            Content = CreateCustomSizedContent(),
        };
        dialog.Show();
    }

    private static NiceDialogBox CreateBasicDialog(
        string title,
        string message)
        => new()
        {
            Title = title,
            Content = new TextBlock
            {
                Text = message,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(20),
            },
        };

    private static NiceDialogBox CreateConfirmationDialog()
    {
        var dialog = new NiceDialogBox
        {
            Title = "Confirmation",
            Width = 400,
            Height = 180,
        };

        var stackPanel = new StackPanel
        {
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(20),
        };

        stackPanel.Children.Add(new TextBlock
        {
            Text = "Do you want to proceed with this action?",
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 0, 0, 20),
            HorizontalAlignment = HorizontalAlignment.Center,
        });

        var buttonPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
        };

        var yesButton = new Button
        {
            Content = "Yes",
            Width = 80,
            Margin = new Thickness(0, 0, 10, 0),
        };
        yesButton.Click += (_, _) =>
        {
            dialog.DialogResult = true;
            dialog.Close();
        };
        buttonPanel.Children.Add(yesButton);

        var noButton = new Button
        {
            Content = "No",
            Width = 80,
        };
        noButton.Click += (_, _) =>
        {
            dialog.DialogResult = false;
            dialog.Close();
        };
        buttonPanel.Children.Add(noButton);

        stackPanel.Children.Add(buttonPanel);
        dialog.Content = stackPanel;

        return dialog;
    }

    private static StackPanel CreateCustomSizedContent()
    {
        var stackPanel = new StackPanel
        {
            Margin = new Thickness(20),
        };

        stackPanel.Children.Add(new TextBlock
        {
            Text = "Custom Sized Dialog",
            FontWeight = FontWeights.Bold,
            FontSize = 18,
            Margin = new Thickness(0, 0, 0, 15),
        });

        stackPanel.Children.Add(new TextBlock
        {
            Text = "This dialog has custom dimensions (500x350) to accommodate more content. " +
                   "You can use larger dialogs for forms, settings panels, or detailed information display.",
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 0, 0, 15),
        });

        stackPanel.Children.Add(new TextBox
        {
            Text = "Sample input field",
            Margin = new Thickness(0, 0, 0, 10),
        });

        stackPanel.Children.Add(new CheckBox
        {
            Content = "Sample checkbox option",
            Margin = new Thickness(0, 0, 0, 10),
        });

        return stackPanel;
    }
}