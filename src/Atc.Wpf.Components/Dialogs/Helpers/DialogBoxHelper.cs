namespace Atc.Wpf.Components.Dialogs.Helpers;

internal static class DialogBoxHelper
{
    /// <summary>
    /// Makes <paramref name="owningWindow"/> the owner of <paramref name="dialog"/> and centers the dialog on it,
    /// when the owner has already been shown. A window that has no handle yet (or has been closed) cannot own
    /// another window, so in that case the dialog keeps its current startup location.
    /// </summary>
    public static void ApplyOwner(
        Window dialog,
        Window? owningWindow)
    {
        ArgumentNullException.ThrowIfNull(dialog);

        if (owningWindow is null ||
            ReferenceEquals(owningWindow, dialog) ||
            !HasWindowHandle(owningWindow))
        {
            return;
        }

        dialog.Owner = owningWindow;
        dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
    }

    /// <summary>
    /// Whether the window has been shown and not yet closed. <see cref="FrameworkElement.IsLoaded"/> is not
    /// used because it stays <see langword="true"/> until the deferred Unloaded broadcast after <see cref="Window.Close"/>.
    /// </summary>
    private static bool HasWindowHandle(Window window)
        => new WindowInteropHelper(window).Handle != IntPtr.Zero &&
           PresentationSource.FromVisual(window) is not null;

    /// <summary>
    /// The largest share of the work area that a dialog grows to before its content scrolls.
    /// </summary>
    internal const double MaxWorkAreaHeightRatio = 0.8;

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
    public static ContentControl CreateContentControl(
        string contentText,
        FrameworkElement? contentImage)
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

        Grid.SetColumn(textBlock, 1);
        grid.Children.Add(textBlock);

        return new ContentControl
        {
            Content = grid,
        };
    }

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

        var workArea = GetWorkArea(owningWindow);
        dialog.ClearValue(FrameworkElement.HeightProperty);
        dialog.MinHeight = minimumHeight;
        dialog.MaxHeight = System.Math.Max(minimumHeight, workArea.Height * MaxWorkAreaHeightRatio);
        dialog.SizeToContent = SizeToContent.Height;
        dialog.SizeChanged -= OnAutoHeightDialogSizeChanged;
        dialog.SizeChanged += OnAutoHeightDialogSizeChanged;
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
    /// owner (or the screen) stays centred when it grows after it was placed. A user resize ends size-to-content.
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
    }

    private static Rect GetWorkArea(Window? owningWindow)
        => owningWindow is not null && HasWindowHandle(owningWindow)
            ? Atc.Wpf.Controls.Layouts.DockScreenHelper.GetWorkAreaAt(
                owningWindow,
                new Point(owningWindow.ActualWidth / 2, owningWindow.ActualHeight / 2))
            : SystemParameters.WorkArea;
}