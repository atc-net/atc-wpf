namespace Atc.Wpf.Sample.SamplesWpfTheming.Window;

public partial class NiceWindowWithFlyoutView
{
    public NiceWindowWithFlyoutView()
    {
        InitializeComponent();
    }

    private void OnOpenBasicFlyoutWindowClick(
        object sender,
        RoutedEventArgs e)
    {
        var window = CreateNiceWindowWithBasicFlyout();
        window.ShowDialog();
    }

    private void OnOpenFlyoutHostWindowClick(
        object sender,
        RoutedEventArgs e)
    {
        var window = CreateNiceWindowWithFlyoutHost();
        window.ShowDialog();
    }

    private void OnOpenSettingsFlyoutWindowClick(
        object sender,
        RoutedEventArgs e)
    {
        var window = CreateNiceWindowWithSettingsFlyout();
        window.ShowDialog();
    }

    private void OnOpenAllPositionsFlyoutWindowClick(
        object sender,
        RoutedEventArgs e)
    {
        var window = CreateNiceWindowWithAllPositions();
        window.ShowDialog();
    }

    private void OnOpenAdvancedFlyoutWindowClick(
        object sender,
        RoutedEventArgs e)
    {
        var window = CreateNiceWindowWithAdvancedFeatures();
        window.ShowDialog();
    }

    private static NiceWindow CreateNiceWindowWithBasicFlyout()
    {
        var flyout = new Flyout
        {
            Header = "Information Panel",
            FlyoutWidth = 400,
            Position = FlyoutPosition.Right,
            Content = CreateSimpleFlyoutContent("Basic Flyout", "This flyout is displayed inside a NiceWindow."),
        };

        var openButton = new Button
        {
            Content = "Open Flyout",
            Width = 150,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
        };
        openButton.Click += (_, _) => flyout.IsOpen = true;

        var grid = new Grid();
        grid.Children.Add(openButton);
        grid.Children.Add(flyout);

        return new NiceWindow
        {
            Title = "NiceWindow - Basic Flyout Demo",
            Width = 800,
            Height = 600,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            Content = grid,
        };
    }

    private static NiceWindow CreateNiceWindowWithFlyoutHost()
    {
        var flyoutHost = new FlyoutHost { MaxNestingDepth = 5 };

        var flyout1 = CreateResourceGroupFlyout();
        var flyout2 = CreateResourceDetailsFlyout();
        var flyout3 = CreateMetricsFlyout();

        flyoutHost.Items.Add(flyout1);
        flyoutHost.Items.Add(flyout2);
        flyoutHost.Items.Add(flyout3);

        // Wire up nested navigation
        WireUpNestedFlyouts(flyout1, flyout2);
        WireUpNestedFlyouts(flyout2, flyout3);

        var openButton = new Button
        {
            Content = "Open Resource Group",
            Width = 200,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
        };
        openButton.Click += (_, _) => flyout1.IsOpen = true;

        var grid = new Grid();
        grid.Children.Add(openButton);
        grid.Children.Add(flyoutHost);

        return new NiceWindow
        {
            Title = "NiceWindow - FlyoutHost (Azure Portal Style)",
            Width = 1000,
            Height = 700,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            Content = grid,
        };
    }

    private static void WireUpNestedFlyouts(
        Flyout parentFlyout,
        Flyout childFlyout)
    {
        if (parentFlyout.Content is not StackPanel stackPanel)
        {
            return;
        }

        foreach (var child in stackPanel.Children)
        {
            if (child is Border border)
            {
                border.MouseLeftButtonUp += (_, _) => childFlyout.IsOpen = true;
            }
            else if (child is Button button && button.Content?.ToString()?.Contains("View", StringComparison.Ordinal) == true)
            {
                button.Click += (_, _) => childFlyout.IsOpen = true;
            }
        }
    }

    private static Flyout CreateResourceGroupFlyout()
    {
        var flyout = new Flyout
        {
            Name = "ResourceGroupFlyout",
            Header = "Resource Group",
            FlyoutWidth = 500,
            Position = FlyoutPosition.Right,
        };

        var stackPanel = new StackPanel { Margin = new Thickness(16) };
        AddTextBlock(stackPanel, "Resource Group Details", FontWeights.SemiBold, 16);
        AddTextBlock(stackPanel, "Click a resource below to see details.", FontWeights.Normal, 16);

        stackPanel.Children.Add(CreateResourceItem("Virtual Machine 1", "Standard_DS2_v2 - Running"));
        stackPanel.Children.Add(CreateResourceItem("Storage Account", "Standard_LRS - Active"));
        stackPanel.Children.Add(CreateResourceItem("SQL Database", "Basic - Online"));

        AddCloseButton(stackPanel, flyout);

        flyout.Content = stackPanel;
        return flyout;
    }

    private static Flyout CreateResourceDetailsFlyout()
    {
        var flyout = new Flyout
        {
            Name = "ResourceDetailsFlyout",
            Header = "Resource Details",
            FlyoutWidth = 450,
            Position = FlyoutPosition.Right,
        };

        var stackPanel = new StackPanel { Margin = new Thickness(16) };
        AddTextBlock(stackPanel, "Virtual Machine Details", FontWeights.SemiBold, 16);
        AddDetailItem(stackPanel, "Status", "Running");
        AddDetailItem(stackPanel, "Size", "Standard_DS2_v2");
        AddDetailItem(stackPanel, "Location", "West Europe");

        var metricsButton = new Button
        {
            Content = "View Metrics",
            Width = 150,
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(0, 16, 0, 0),
        };
        stackPanel.Children.Add(metricsButton);

        AddCloseButton(stackPanel, flyout);

        flyout.Content = stackPanel;
        return flyout;
    }

    private static Flyout CreateMetricsFlyout()
    {
        var flyout = new Flyout
        {
            Name = "MetricsFlyout",
            Header = "Metrics",
            FlyoutWidth = 400,
            Position = FlyoutPosition.Right,
        };

        var stackPanel = new StackPanel { Margin = new Thickness(16) };
        AddTextBlock(stackPanel, "Performance Metrics", FontWeights.SemiBold, 16);
        AddDetailItem(stackPanel, "CPU Usage", "23%");
        AddDetailItem(stackPanel, "Memory Usage", "4.2 GB / 8 GB");
        AddDetailItem(stackPanel, "Network In", "1.2 MB/s");
        AddDetailItem(stackPanel, "Network Out", "0.8 MB/s");
        AddCloseButton(stackPanel, flyout);

        flyout.Content = stackPanel;
        return flyout;
    }

    private static NiceWindow CreateNiceWindowWithSettingsFlyout()
    {
        var flyout = CreateSettingsFlyout();

        var openButton = new Button
        {
            Content = "Open Settings",
            Width = 150,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
        };
        openButton.Click += (_, _) => flyout.IsOpen = true;

        var mainGrid = new Grid();
        mainGrid.Children.Add(openButton);
        mainGrid.Children.Add(flyout);

        return new NiceWindow
        {
            Title = "NiceWindow - Settings Flyout Demo",
            Width = 800,
            Height = 600,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            Content = mainGrid,
        };
    }

    private static Flyout CreateSettingsFlyout()
    {
        var flyout = new Flyout
        {
            Header = "Settings",
            FlyoutWidth = 450,
            Position = FlyoutPosition.Right,
            ShowPinButton = true,
        };

        var stackPanel = new StackPanel { Margin = new Thickness(16) };
        AddTextBlock(stackPanel, "General Settings", FontWeights.SemiBold, 16);
        AddFormField(stackPanel, "Display Name", "John Doe");
        AddFormField(stackPanel, "Email Address", "john@example.com");
        AddCheckBox(stackPanel, "Enable notifications", true);
        AddCheckBox(stackPanel, "Auto-save documents", true);
        AddCheckBox(stackPanel, "Show welcome screen", false);
        AddFormButtons(stackPanel, flyout);

        flyout.Content = stackPanel;
        return flyout;
    }

    private static NiceWindow CreateNiceWindowWithAllPositions()
    {
        var mainGrid = new Grid();
        var buttonPanel = CreateCenteredButtonPanel();

        var positions = new[] { FlyoutPosition.Right, FlyoutPosition.Left, FlyoutPosition.Top, FlyoutPosition.Bottom, FlyoutPosition.Center };

        foreach (var position in positions)
        {
            var flyout = CreatePositionFlyout(position);
            var button = CreateOpenButton($"Open {position}", flyout);
            buttonPanel.Children.Add(button);
            mainGrid.Children.Add(flyout);
        }

        mainGrid.Children.Add(buttonPanel);

        return new NiceWindow
        {
            Title = "NiceWindow - All Flyout Positions",
            Width = 900,
            Height = 700,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            Content = mainGrid,
        };
    }

    private static Flyout CreatePositionFlyout(FlyoutPosition position)
    {
        var flyout = new Flyout
        {
            Header = $"{position} Flyout",
            Position = position,
            FlyoutWidth = position == FlyoutPosition.Center ? 400 : 350,
            FlyoutHeight = position is FlyoutPosition.Top or FlyoutPosition.Bottom ? 200 : 300,
            CornerRadius = position == FlyoutPosition.Center ? new CornerRadius(8) : new CornerRadius(0),
            Content = CreateSimpleFlyoutContent($"{position} Position", $"Slides in from {position.ToString().ToLowerInvariant()}."),
        };

        return flyout;
    }

    private static NiceWindow CreateNiceWindowWithAdvancedFeatures()
    {
        var mainGrid = new Grid();
        var buttonPanel = CreateCenteredButtonPanel();

        // Pinnable flyout
        var pinnableFlyout = CreatePinnableFlyout();
        buttonPanel.Children.Add(CreateOpenButton("Open Pinnable Flyout", pinnableFlyout));
        mainGrid.Children.Add(pinnableFlyout);

        // Resizable flyout
        var resizableFlyout = CreateResizableFlyout();
        buttonPanel.Children.Add(CreateOpenButton("Open Resizable Flyout", resizableFlyout));
        mainGrid.Children.Add(resizableFlyout);

        // Bouncy animation flyout
        var bouncyFlyout = CreateBouncyFlyout();
        buttonPanel.Children.Add(CreateOpenButton("Open Bouncy Flyout", bouncyFlyout));
        mainGrid.Children.Add(bouncyFlyout);

        mainGrid.Children.Add(buttonPanel);

        return new NiceWindow
        {
            Title = "NiceWindow - Advanced Flyout Features",
            Width = 900,
            Height = 700,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            Content = mainGrid,
        };
    }

    private static Flyout CreatePinnableFlyout()
        => new()
        {
            Header = "Pinnable Panel",
            FlyoutWidth = 400,
            Position = FlyoutPosition.Right,
            ShowPinButton = true,
            Content = CreateSimpleFlyoutContent("Pin/Unpin Feature", "Click the pin icon to keep this flyout open."),
        };

    private static Flyout CreateResizableFlyout()
        => new()
        {
            Header = "Resizable Panel",
            FlyoutWidth = 400,
            Position = FlyoutPosition.Right,
            IsResizable = true,
            MinFlyoutWidth = 250,
            MaxFlyoutWidth = 700,
            Content = CreateSimpleFlyoutContent("Resizable Flyout", "Drag the left edge to resize (250-700px)."),
        };

    private static Flyout CreateBouncyFlyout()
        => new()
        {
            Header = "Custom Animation",
            FlyoutWidth = 400,
            Position = FlyoutPosition.Right,
            AnimationDuration = 600,
            EasingFunction = new System.Windows.Media.Animation.ElasticEase
            {
                EasingMode = System.Windows.Media.Animation.EasingMode.EaseOut,
                Oscillations = 2,
                Springiness = 5,
            },
            Content = CreateSimpleFlyoutContent("Bouncy Animation", "Uses ElasticEase for a bouncy effect."),
        };

    private static StackPanel CreateSimpleFlyoutContent(
        string title,
        string description)
    {
        var stackPanel = new StackPanel { Margin = new Thickness(16) };
        AddTextBlock(stackPanel, title, FontWeights.SemiBold, 16);
        AddTextBlock(stackPanel, description, FontWeights.Normal, 16);
        return stackPanel;
    }

    private static StackPanel CreateCenteredButtonPanel()
        => new()
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Orientation = Orientation.Vertical,
        };

    private static Button CreateOpenButton(
        string content,
        Flyout flyout)
    {
        var button = new Button
        {
            Content = content,
            Width = 200,
            Margin = new Thickness(0, 5, 0, 5),
        };
        button.Click += (_, _) => flyout.IsOpen = true;
        return button;
    }

    private static void AddTextBlock(
        StackPanel parent,
        string text,
        FontWeight fontWeight,
        double bottomMargin)
    {
        parent.Children.Add(new TextBlock
        {
            Text = text,
            FontWeight = fontWeight,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 0, 0, bottomMargin),
        });
    }

    private static void AddDetailItem(
        StackPanel parent,
        string label,
        string value)
    {
        var stackPanel = new StackPanel { Margin = new Thickness(0, 0, 0, 8) };

        var labelText = new TextBlock { Text = label };
        labelText.SetResourceReference(TextBlock.ForegroundProperty, "AtcApps.Brushes.Gray3");
        stackPanel.Children.Add(labelText);

        stackPanel.Children.Add(new TextBlock { Text = value, FontWeight = FontWeights.SemiBold });
        parent.Children.Add(stackPanel);
    }

    private static void AddFormField(
        StackPanel parent,
        string label,
        string defaultValue)
    {
        var stackPanel = new StackPanel { Margin = new Thickness(0, 0, 0, 12) };
        stackPanel.Children.Add(new TextBlock { Text = label, Margin = new Thickness(0, 0, 0, 4) });
        stackPanel.Children.Add(new TextBox { Text = defaultValue });
        parent.Children.Add(stackPanel);
    }

    private static void AddCheckBox(
        StackPanel parent,
        string content,
        bool isChecked)
    {
        parent.Children.Add(new CheckBox
        {
            Content = content,
            IsChecked = isChecked,
            Margin = new Thickness(0, 0, 0, 8),
        });
    }

    private static void AddFormButtons(
        StackPanel parent,
        Flyout flyout)
    {
        var buttonPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(0, 16, 0, 0),
        };

        var cancelButton = new Button { Content = "Cancel", Width = 80, Margin = new Thickness(0, 0, 8, 0) };
        cancelButton.Click += (_, _) => flyout.IsOpen = false;
        buttonPanel.Children.Add(cancelButton);

        var saveButton = new Button { Content = "Save", Width = 80 };
        saveButton.Click += (_, _) => flyout.IsOpen = false;
        buttonPanel.Children.Add(saveButton);

        parent.Children.Add(buttonPanel);
    }

    private static void AddCloseButton(
        StackPanel parent,
        Flyout flyout)
    {
        var closeButton = new Button
        {
            Content = "Close",
            Width = 100,
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(0, 16, 0, 0),
        };
        closeButton.Click += (_, _) => flyout.IsOpen = false;
        parent.Children.Add(closeButton);
    }

    private static Border CreateResourceItem(
        string name,
        string status)
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
        stackPanel.Children.Add(new TextBlock { Text = name, FontWeight = FontWeights.SemiBold });

        var statusText = new TextBlock { Text = status };
        statusText.SetResourceReference(TextBlock.ForegroundProperty, "AtcApps.Brushes.Gray3");
        stackPanel.Children.Add(statusText);

        border.Child = stackPanel;
        return border;
    }
}