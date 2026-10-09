namespace Atc.Wpf.Theming.Controls;

/// <summary>
/// A <see cref="ContentPresenter"/> that keeps input content hit-testable inside a window chrome.
/// </summary>
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "OK.")]
public sealed class ContentPresenterEx : ContentPresenter
{
    static ContentPresenterEx()
    {
        ContentProperty.OverrideMetadata(
            typeof(ContentPresenterEx),
            new FrameworkPropertyMetadata(OnContentPropertyChanged));
    }

    private static void OnContentPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is IInputElement and DependencyObject oldInputElement)
        {
            BindingOperations.ClearBinding(
                oldInputElement,
                WindowChrome.IsHitTestVisibleInChromeProperty);
        }

        if (e.NewValue is IInputElement and DependencyObject newInputElement)
        {
            BindingOperations.SetBinding(
                newInputElement,
                WindowChrome.IsHitTestVisibleInChromeProperty,
                new Binding
                {
                    Path = new PropertyPath(WindowChrome.IsHitTestVisibleInChromeProperty),
                    Source = d,
                });
        }
    }
}