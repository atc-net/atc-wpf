namespace Atc.Wpf.Tests.DragDrop;

/// <summary>
/// Shows content in an off-screen window so item containers are generated and laid out.
/// </summary>
internal sealed class DragDropTestWindow : IDisposable
{
    private readonly Window window;

    public DragDropTestWindow(
        FrameworkElement content,
        double width = 300,
        double height = 300)
    {
        window = new Window
        {
            Content = content,
            Width = width,
            Height = height,
            Left = -10000,
            Top = -10000,
            ShowActivated = false,
            ShowInTaskbar = false,
            WindowStyle = WindowStyle.None,
        };
        window.Show();
        window.UpdateLayout();
    }

    public static T FindDescendant<T>(DependencyObject root)
        where T : DependencyObject
        => TryFindDescendant<T>(root)
           ?? throw new InvalidOperationException($"No {typeof(T).Name} found.");

    public static T? TryFindDescendant<T>(DependencyObject root)
        where T : DependencyObject
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
        {
            var child = VisualTreeHelper.GetChild(root, i);
            if (child is T match)
            {
                return match;
            }

            var found = TryFindDescendant<T>(child);
            if (found is not null)
            {
                return found;
            }
        }

        return null;
    }

    public void Dispose()
        => window.Close();
}