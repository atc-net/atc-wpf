namespace Atc.Wpf.Components.Dialogs.Helpers;

internal static class DialogBoxHelper
{
    /// <summary>
    /// The largest share of the work area that a dialog grows to before its content scrolls.
    /// </summary>
    internal const double MaxWorkAreaHeightRatio = 0.8;

    private const string DontAskAgainCheckBoxName = "DontAskAgainCheckBox";

    public static ContentControl CreateHeaderControl(string headerText)
        => new()
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(10, 5, 10, 5),
            Content = new TextBlock
            {
                Text = TextHelper.NormalizeLineBreaks(headerText),
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center,
                FontSize = 24,
            },
        };

    /// <summary>
    /// Creates the content of a text dialog: the optional icon in an auto-sized column and the text in the rest of
    /// the width, where it wraps (also inside long words or paths with no spaces). The block is vertically centred in
    /// the dialog; text shorter than the icon is centred on it, and taller text starts level with the icon, so the
    /// icon stays in view at the top of a long, scrolling message.
    /// </summary>
    /// <param name="contentText">The message.</param>
    /// <param name="contentImage">The icon, if any; the dialog shows a copy.</param>
    /// <param name="settings">
    /// The settings; when they have <see cref="DialogBoxSettings.DetailsText"/> or
    /// <see cref="DialogBoxSettings.ShowDontAskAgain"/>, a collapsed details area and a "Don't ask again" check box
    /// follow the message.
    /// </param>
    public static ContentControl CreateContentControl(
        string contentText,
        FrameworkElement? contentImage,
        DialogBoxSettings? settings = null)
    {
        // Centred as a block, so a short message stays in the middle of a dialog taller than the text.
        var grid = new Grid
        {
            VerticalAlignment = VerticalAlignment.Center,
        };
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        if (contentImage is not null)
        {
            // The icon belongs to the settings, which can serve several dialogs, so each dialog gets its own copy.
            var image = CopyElement(contentImage);
            image.VerticalAlignment = VerticalAlignment.Top;
            Grid.SetColumn(image, 0);
            grid.Children.Add(image);
        }

        var textBlock = new TextBlock
        {
            Text = TextHelper.NormalizeLineBreaks(contentText),
            TextWrapping = TextWrapping.Wrap,
            VerticalAlignment = VerticalAlignment.Center,
        };

        var extras = CreateExtras(settings);
        if (extras.Count == 0)
        {
            Grid.SetColumn(textBlock, 1);
            grid.Children.Add(textBlock);
        }
        else
        {
            var textPanel = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
            };

            textPanel.Children.Add(textBlock);
            foreach (var extra in extras)
            {
                textPanel.Children.Add(extra);
            }

            Grid.SetColumn(textPanel, 1);
            grid.Children.Add(textPanel);
        }

        return new ContentControl
        {
            Content = grid,
        };
    }

    /// <summary>
    /// Gets the "Don't ask again" check box of content created by <see cref="CreateContentControl"/>, if any.
    /// </summary>
    public static CheckBox? FindDontAskAgainCheckBox(
        ContentControl? contentControl)
        => (contentControl?.Content as Grid)?.Children
            .OfType<StackPanel>()
            .SelectMany(x => x.Children.OfType<CheckBox>())
            .FirstOrDefault(x => x.Name == DontAskAgainCheckBoxName);

    /// <summary>
    /// Creates the text a message dialog puts on the clipboard for Ctrl+C, laid out like a Windows message box:
    /// the title, the header (if any), the message, the details (if any) and the button texts, separated by dashed
    /// lines.
    /// </summary>
    public static string CreateClipboardText(
        string title,
        string? header,
        string message,
        DialogBoxSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        const string separator = "---------------------------";
        var buttons = new List<string> { settings.AffirmativeButtonText };
        if (settings.ShowNegativeButton)
        {
            buttons.Add(settings.NegativeButtonText);
        }

        if (settings.ShowCancelButton)
        {
            buttons.Add(settings.CancelButtonText);
        }

        var lines = new List<string> { separator, title, separator };
        if (!string.IsNullOrEmpty(header))
        {
            lines.Add(header);
            lines.Add(separator);
        }

        lines.Add(ToWindowsLineBreaks(message));
        lines.Add(separator);
        if (!string.IsNullOrWhiteSpace(settings.DetailsText))
        {
            lines.Add(ToWindowsLineBreaks(settings.DetailsText));
            lines.Add(separator);
        }

        lines.Add(string.Join("   ", buttons));
        lines.Add(separator);
        return string.Join(Environment.NewLine, lines);
    }

    /// <summary>
    /// Copies the dialog text when Ctrl+C is pressed and no text box has focus (it copies its own selection).
    /// </summary>
    /// <returns><see langword="true"/> when the key press was handled.</returns>
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "The clipboard can be locked by another process; copying is best effort.")]
    public static bool TryCopyOnCtrlC(
        KeyEventArgs e,
        Func<string> createText)
    {
        ArgumentNullException.ThrowIfNull(e);
        ArgumentNullException.ThrowIfNull(createText);

        if (e.Handled ||
            e.Key != Key.C ||
            Keyboard.Modifiers != ModifierKeys.Control ||
            Keyboard.FocusedElement is TextBoxBase)
        {
            return false;
        }

        try
        {
            System.Windows.Clipboard.SetText(createText());
        }
        catch (Exception)
        {
            // Another process holds the clipboard.
        }

        return true;
    }

    /// <summary>
    /// Gets the text of the header created by <see cref="CreateHeaderControl"/>, if any.
    /// </summary>
    public static string? GetHeaderText(ContentControl? headerControl)
        => headerControl?.Content switch
        {
            TextBlock textBlock => textBlock.Text,
            string text => text,
            _ => null,
        };

    /// <summary>
    /// Creates a new element of the same type with the same locally set property values (bindings included), so one
    /// icon description can be shown in several dialogs. An element that cannot be created again (no parameterless
    /// constructor) is moved out of its previous panel and used itself.
    /// </summary>
    internal static FrameworkElement CopyElement(FrameworkElement element)
    {
        ArgumentNullException.ThrowIfNull(element);

        if (element.GetType().GetConstructor(Type.EmptyTypes) is null ||
            Activator.CreateInstance(element.GetType()) is not FrameworkElement copy)
        {
            if (element.Parent is Panel previousPanel)
            {
                previousPanel.Children.Remove(element);
            }

            return element;
        }

        var localValues = element.GetLocalValueEnumerator();
        while (localValues.MoveNext())
        {
            var entry = localValues.Current;
            if (entry.Property.ReadOnly ||
                entry.Property == FrameworkElement.NameProperty)
            {
                continue;
            }

            if (BindingOperations.GetBindingBase(element, entry.Property) is { } binding)
            {
                BindingOperations.SetBinding(copy, entry.Property, binding);
            }
            else
            {
                copy.SetValue(entry.Property, entry.Value);
            }
        }

        return copy;
    }

    /// <summary>
    /// Lets a text dialog grow taller than <paramref name="minimumHeight"/> to fit its content, up to
    /// <see cref="MaxWorkAreaHeightRatio"/> of the work area of the owner's monitor (the primary monitor
    /// without an owner); beyond that the content scrolls. The width stays as set.
    /// </summary>
    public static void ApplyAutoHeight(
        Window dialog,
        Window? owningWindow,
        double minimumHeight)
    {
        ArgumentNullException.ThrowIfNull(dialog);

        var workArea = DialogBoxWindowHelper.GetWorkArea(owningWindow);
        dialog.ClearValue(FrameworkElement.HeightProperty);
        dialog.MinHeight = minimumHeight;
        dialog.MaxHeight = System.Math.Max(minimumHeight, workArea.Height * MaxWorkAreaHeightRatio);
        dialog.SizeToContent = SizeToContent.Height;
        dialog.SizeChanged -= OnAutoHeightDialogSizeChanged;
        dialog.SizeChanged += OnAutoHeightDialogSizeChanged;
        dialog.ContentRendered -= OnAutoHeightDialogContentRendered;
        dialog.ContentRendered += OnAutoHeightDialogContentRendered;
    }

    /// <summary>
    /// Ends auto height when the caller sets an explicit height on a dialog that has not been shown yet, so the
    /// dialog keeps that height as before auto height existed. Once the window exists, WPF sets the height itself
    /// from the window size, so those changes are ignored. Call it from the dialog's
    /// <see cref="FrameworkElement.OnPropertyChanged"/>.
    /// </summary>
    public static void OnDialogPropertyChanged(
        Window dialog,
        DependencyPropertyChangedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(dialog);

        if (e.Property != FrameworkElement.HeightProperty ||
            e.NewValue is not double height ||
            double.IsNaN(height) ||
            dialog.SizeToContent != SizeToContent.Height ||
            new WindowInteropHelper(dialog).Handle != IntPtr.Zero)
        {
            return;
        }

        dialog.SizeToContent = SizeToContent.Manual;
        dialog.ClearValue(FrameworkElement.MinHeightProperty);
        dialog.ClearValue(FrameworkElement.MaxHeightProperty);
        dialog.SizeChanged -= OnAutoHeightDialogSizeChanged;
    }

    /// <summary>
    /// Keeps the vertical centre of a placed dialog when its height follows its content, so a dialog centred on its
    /// owner (or the screen) stays centred when it grows after it was placed, and keeps it inside the work area.
    /// A user resize ends size-to-content.
    /// </summary>
    private static void OnAutoHeightDialogSizeChanged(
        object sender,
        SizeChangedEventArgs e)
    {
        if (sender is not Window window ||
            !e.HeightChanged ||
            e.PreviousSize.Height <= 0 ||
            window.SizeToContent != SizeToContent.Height ||
            window.WindowState != WindowState.Normal ||
            double.IsNaN(window.Top))
        {
            return;
        }

        window.Top -= (e.NewSize.Height - e.PreviousSize.Height) / 2;
        DialogBoxWindowHelper.KeepInWorkArea(window);
    }

    private static void OnAutoHeightDialogContentRendered(
        object? sender,
        EventArgs e)
    {
        if (sender is Window window)
        {
            window.ContentRendered -= OnAutoHeightDialogContentRendered;
            DialogBoxWindowHelper.KeepInWorkArea(window);
        }
    }

    private static List<UIElement> CreateExtras(DialogBoxSettings? settings)
    {
        var extras = new List<UIElement>();
        if (settings is null)
        {
            return extras;
        }

        if (!string.IsNullOrWhiteSpace(settings.DetailsText))
        {
            extras.Add(new Expander
            {
                Margin = new Thickness(0, 10, 0, 0),
                Header = Atc.Wpf.Components.Resources.Miscellaneous.Details,
                IsExpanded = false,
                Content = new TextBox
                {
                    Text = settings.DetailsText,
                    IsReadOnly = true,
                    IsReadOnlyCaretVisible = true,
                    TextWrapping = TextWrapping.NoWrap,
                    FontFamily = new FontFamily("Consolas"),
                    MaxHeight = 200,
                    Margin = new Thickness(0, 5, 0, 0),
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                },
            });
        }

        if (settings.ShowDontAskAgain)
        {
            extras.Add(new CheckBox
            {
                Name = DontAskAgainCheckBoxName,
                Margin = new Thickness(0, 10, 0, 0),
                Content = settings.DontAskAgainText ?? Atc.Wpf.Components.Resources.Miscellaneous.DontAskAgain,
            });
        }

        return extras;
    }

    private static string ToWindowsLineBreaks(string text)
        => TextHelper.NormalizeLineBreaks(text)
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace("\n", Environment.NewLine, StringComparison.Ordinal);
}