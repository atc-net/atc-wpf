namespace Atc.Wpf.Sample.SamplesWpfTheming.Window;

public partial class NiceDialogBoxWithFlyoutView
{
    public NiceDialogBoxWithFlyoutView()
    {
        InitializeComponent();
    }

    private void OnOpenDialogWithSidePanelClick(
        object sender,
        RoutedEventArgs e)
    {
        var dialog = CreateDialogWithSidePanel();
        dialog.Show();
    }

    private void OnOpenSettingsDialogClick(
        object sender,
        RoutedEventArgs e)
    {
        var dialog = CreateSettingsDialog();
        dialog.Show();
    }

    private void OnOpenWizardDialogClick(
        object sender,
        RoutedEventArgs e)
    {
        var dialog = CreateWizardDialog();
        dialog.Show();
    }

    private void OnOpenDialogWithCenterFlyoutClick(
        object sender,
        RoutedEventArgs e)
    {
        var dialog = CreateDialogWithCenterFlyout();
        dialog.Show();
    }

    private static NiceDialogBox CreateDialogWithSidePanel()
    {
        var flyout = new Flyout
        {
            Header = "Help & Information",
            FlyoutWidth = 300,
            Position = FlyoutPosition.Right,
            Content = CreateHelpContent(),
        };

        var openButton = new Button
        {
            Content = "Show Help Panel",
            Width = 150,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
        };
        openButton.Click += (_, _) => flyout.IsOpen = true;

        var mainContent = new StackPanel
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
        };

        mainContent.Children.Add(new TextBlock
        {
            Text = "Main Dialog Content",
            FontWeight = FontWeights.Bold,
            FontSize = 16,
            Margin = new Thickness(0, 0, 0, 15),
            HorizontalAlignment = HorizontalAlignment.Center,
        });

        mainContent.Children.Add(new TextBlock
        {
            Text = "Click the button to open a help panel.",
            Margin = new Thickness(0, 0, 0, 15),
            HorizontalAlignment = HorizontalAlignment.Center,
        });

        mainContent.Children.Add(openButton);

        var grid = new Grid();
        grid.Children.Add(mainContent);
        grid.Children.Add(flyout);

        return new NiceDialogBox
        {
            Title = "Dialog with Side Panel",
            Width = 500,
            Height = 350,
            ShowCloseButton = true,
            Content = grid,
        };
    }

    private static NiceDialogBox CreateSettingsDialog()
    {
        var detailsFlyout = new Flyout
        {
            Header = "Setting Details",
            FlyoutWidth = 280,
            Position = FlyoutPosition.Right,
        };

        var settingsPanel = new StackPanel { Margin = new Thickness(20) };

        settingsPanel.Children.Add(new TextBlock
        {
            Text = "Settings",
            FontWeight = FontWeights.Bold,
            FontSize = 16,
            Margin = new Thickness(0, 0, 0, 15),
        });

        settingsPanel.Children.Add(CreateSettingItem("Display", "Configure display options", detailsFlyout));
        settingsPanel.Children.Add(CreateSettingItem("Network", "Network and proxy settings", detailsFlyout));
        settingsPanel.Children.Add(CreateSettingItem("Privacy", "Privacy and security options", detailsFlyout));
        settingsPanel.Children.Add(CreateSettingItem("Advanced", "Advanced configuration", detailsFlyout));

        var grid = new Grid();
        grid.Children.Add(settingsPanel);
        grid.Children.Add(detailsFlyout);

        return new NiceDialogBox
        {
            Title = "Settings",
            Width = 550,
            Height = 400,
            ShowCloseButton = true,
            Content = grid,
        };
    }

    private static NiceDialogBox CreateWizardDialog()
    {
        var step2Flyout = CreateWizardStepFlyout("Step 2: Configuration");
        var step3Flyout = CreateWizardStepFlyout("Step 3: Confirmation");

        step2Flyout.Content = CreateWizardStep2Content(step2Flyout, step3Flyout);
        step3Flyout.Content = CreateWizardStep3Content(step2Flyout, step3Flyout);

        var mainContent = CreateWizardMainContent(step2Flyout);

        var grid = new Grid();
        grid.Children.Add(mainContent);
        grid.Children.Add(step2Flyout);
        grid.Children.Add(step3Flyout);

        return new NiceDialogBox
        {
            Title = "Setup Wizard",
            Width = 500,
            Height = 350,
            ShowCloseButton = true,
            Content = grid,
        };
    }

    private static Flyout CreateWizardStepFlyout(string header)
        => new()
        {
            Header = header,
            FlyoutWidth = 300,
            Position = FlyoutPosition.Right,
        };

    private static StackPanel CreateWizardStep2Content(
        Flyout step2Flyout,
        Flyout step3Flyout)
    {
        var content = new StackPanel { Margin = new Thickness(16) };
        content.Children.Add(new TextBlock
        {
            Text = "Configure your options:",
            FontWeight = FontWeights.SemiBold,
            Margin = new Thickness(0, 0, 0, 10),
        });
        content.Children.Add(new CheckBox { Content = "Enable feature A", Margin = new Thickness(0, 0, 0, 5) });
        content.Children.Add(new CheckBox { Content = "Enable feature B", Margin = new Thickness(0, 0, 0, 5) });
        content.Children.Add(new CheckBox { Content = "Enable feature C", Margin = new Thickness(0, 0, 0, 15) });

        var nextButton = new Button { Content = "Next", Width = 80 };
        nextButton.Click += (_, _) =>
        {
            step2Flyout.IsOpen = false;
            step3Flyout.IsOpen = true;
        };
        content.Children.Add(nextButton);

        return content;
    }

    private static StackPanel CreateWizardStep3Content(
        Flyout step2Flyout,
        Flyout step3Flyout)
    {
        var content = new StackPanel { Margin = new Thickness(16) };
        content.Children.Add(new TextBlock
        {
            Text = "Review and confirm:",
            FontWeight = FontWeights.SemiBold,
            Margin = new Thickness(0, 0, 0, 10),
        });
        content.Children.Add(new TextBlock
        {
            Text = "Your settings have been configured.",
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 0, 0, 15),
        });

        var backButton = new Button { Content = "Back", Width = 80, Margin = new Thickness(0, 0, 10, 0) };
        backButton.Click += (_, _) =>
        {
            step3Flyout.IsOpen = false;
            step2Flyout.IsOpen = true;
        };

        var finishButton = new Button { Content = "Finish", Width = 80 };
        finishButton.Click += (_, _) => step3Flyout.IsOpen = false;

        var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal };
        buttonPanel.Children.Add(backButton);
        buttonPanel.Children.Add(finishButton);
        content.Children.Add(buttonPanel);

        return content;
    }

    private static StackPanel CreateWizardMainContent(Flyout step2Flyout)
    {
        var mainContent = new StackPanel
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
        };

        mainContent.Children.Add(new TextBlock
        {
            Text = "Step 1: Welcome",
            FontWeight = FontWeights.Bold,
            FontSize = 16,
            Margin = new Thickness(0, 0, 0, 15),
            HorizontalAlignment = HorizontalAlignment.Center,
        });

        mainContent.Children.Add(new TextBlock
        {
            Text = "Welcome to the setup wizard.\nClick Next to begin configuration.",
            TextWrapping = TextWrapping.Wrap,
            TextAlignment = TextAlignment.Center,
            Margin = new Thickness(0, 0, 0, 15),
        });

        var nextButton = new Button
        {
            Content = "Next",
            Width = 80,
            HorizontalAlignment = HorizontalAlignment.Center,
        };
        nextButton.Click += (_, _) => step2Flyout.IsOpen = true;
        mainContent.Children.Add(nextButton);

        return mainContent;
    }

    private static NiceDialogBox CreateDialogWithCenterFlyout()
    {
        var confirmFlyout = CreateCenterConfirmFlyout();
        NiceDialogBox? dialog = null;

        confirmFlyout.Content = CreateCenterConfirmContent(confirmFlyout, () => dialog?.Close());
        var mainContent = CreateCenterFlyoutMainContent(confirmFlyout);

        var grid = new Grid();
        grid.Children.Add(mainContent);
        grid.Children.Add(confirmFlyout);

        dialog = new NiceDialogBox
        {
            Title = "Dialog with Center Flyout",
            Width = 450,
            Height = 300,
            ShowCloseButton = true,
            Content = grid,
        };

        return dialog;
    }

    private static Flyout CreateCenterConfirmFlyout()
        => new()
        {
            Header = "Confirm Action",
            FlyoutWidth = 300,
            FlyoutHeight = 150,
            Position = FlyoutPosition.Center,
            CornerRadius = new CornerRadius(8),
        };

    private static StackPanel CreateCenterConfirmContent(
        Flyout confirmFlyout,
        Action onConfirm)
    {
        var content = new StackPanel
        {
            Margin = new Thickness(16),
            VerticalAlignment = VerticalAlignment.Center,
        };

        content.Children.Add(new TextBlock
        {
            Text = "Are you sure you want to proceed?",
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 0, 0, 15),
            HorizontalAlignment = HorizontalAlignment.Center,
        });

        var buttonPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
        };

        var yesButton = new Button { Content = "Yes", Width = 70, Margin = new Thickness(0, 0, 10, 0) };
        yesButton.Click += (_, _) => onConfirm();

        var noButton = new Button { Content = "No", Width = 70 };
        noButton.Click += (_, _) => confirmFlyout.IsOpen = false;

        buttonPanel.Children.Add(yesButton);
        buttonPanel.Children.Add(noButton);
        content.Children.Add(buttonPanel);

        return content;
    }

    private static StackPanel CreateCenterFlyoutMainContent(
        Flyout confirmFlyout)
    {
        var mainContent = new StackPanel
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
        };

        mainContent.Children.Add(new TextBlock
        {
            Text = "Main Dialog Content",
            FontWeight = FontWeights.Bold,
            FontSize = 16,
            Margin = new Thickness(0, 0, 0, 15),
            HorizontalAlignment = HorizontalAlignment.Center,
        });

        mainContent.Children.Add(new TextBlock
        {
            Text = "Click the button to show a center confirmation flyout.",
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 0, 0, 15),
            HorizontalAlignment = HorizontalAlignment.Center,
        });

        var actionButton = new Button
        {
            Content = "Perform Action",
            Width = 120,
            HorizontalAlignment = HorizontalAlignment.Center,
        };
        actionButton.Click += (_, _) => confirmFlyout.IsOpen = true;
        mainContent.Children.Add(actionButton);

        return mainContent;
    }

    private static StackPanel CreateHelpContent()
    {
        var stackPanel = new StackPanel { Margin = new Thickness(16) };

        stackPanel.Children.Add(new TextBlock
        {
            Text = "Help Information",
            FontWeight = FontWeights.SemiBold,
            Margin = new Thickness(0, 0, 0, 10),
        });

        stackPanel.Children.Add(new TextBlock
        {
            Text = "This is a help panel that provides additional context and guidance for the dialog content.",
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 0, 0, 10),
        });

        stackPanel.Children.Add(new TextBlock
        {
            Text = "Tips:",
            FontWeight = FontWeights.SemiBold,
            Margin = new Thickness(0, 10, 0, 5),
        });

        stackPanel.Children.Add(new TextBlock
        {
            Text = "- Use flyouts for contextual help\n- Keep content focused\n- Provide clear actions",
            TextWrapping = TextWrapping.Wrap,
        });

        return stackPanel;
    }

    private static Border CreateSettingItem(
        string name,
        string description,
        Flyout detailsFlyout)
    {
        var border = new Border
        {
            Margin = new Thickness(0, 0, 0, 8),
            Padding = new Thickness(12),
            CornerRadius = new CornerRadius(4),
            Cursor = Cursors.Hand,
        };
        border.SetResourceReference(Border.BackgroundProperty, "AtcApps.Brushes.Gray9");

        var stackPanel = new StackPanel();

        stackPanel.Children.Add(new TextBlock
        {
            Text = name,
            FontWeight = FontWeights.SemiBold,
        });

        var descText = new TextBlock { Text = description };
        descText.SetResourceReference(TextBlock.ForegroundProperty, "AtcApps.Brushes.Gray3");
        stackPanel.Children.Add(descText);

        border.Child = stackPanel;

        border.MouseLeftButtonUp += (_, _) =>
        {
            UpdateDetailsFlyout(detailsFlyout, name, description);
            detailsFlyout.IsOpen = true;
        };

        return border;
    }

    private static void UpdateDetailsFlyout(
        Flyout flyout,
        string settingName,
        string description)
    {
        var content = new StackPanel { Margin = new Thickness(16) };

        content.Children.Add(new TextBlock
        {
            Text = settingName,
            FontWeight = FontWeights.Bold,
            FontSize = 14,
            Margin = new Thickness(0, 0, 0, 10),
        });

        content.Children.Add(new TextBlock
        {
            Text = description,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 0, 0, 15),
        });

        content.Children.Add(new TextBlock
        {
            Text = "Options:",
            FontWeight = FontWeights.SemiBold,
            Margin = new Thickness(0, 0, 0, 5),
        });

        content.Children.Add(new CheckBox
        {
            Content = $"Enable {settingName.ToLowerInvariant()}",
            IsChecked = true,
            Margin = new Thickness(0, 0, 0, 5),
        });

        content.Children.Add(new CheckBox
        {
            Content = "Show notifications",
            Margin = new Thickness(0, 0, 0, 5),
        });

        var closeButton = new Button
        {
            Content = "Close",
            Width = 80,
            Margin = new Thickness(0, 15, 0, 0),
        };
        closeButton.Click += (_, _) => flyout.IsOpen = false;
        content.Children.Add(closeButton);

        flyout.Header = $"{settingName} Details";
        flyout.Content = content;
    }
}