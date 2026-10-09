namespace Atc.Wpf.Components.Notifications;

/// <summary>
/// Selects the data template for toast notification content: one for plain strings and one for <see cref="ToastNotificationContent"/>.
/// </summary>
public sealed class ToastNotificationTemplateSelector : DataTemplateSelector
{
    private DataTemplate? defaultStringTemplate;
    private DataTemplate? defaultToastNotificationTemplate;

    private void GetTemplatesFromResources(FrameworkElement? container)
    {
        defaultStringTemplate = container?.FindResource("DefaultStringTemplate") as DataTemplate;
        defaultToastNotificationTemplate = container?.FindResource("DefaultToastNotificationTemplate") as DataTemplate;
    }

    /// <inheritdoc />
    public override DataTemplate? SelectTemplate(
        object? item,
        DependencyObject container)
    {
        if (defaultStringTemplate is null &&
            defaultToastNotificationTemplate is null)
        {
            GetTemplatesFromResources((FrameworkElement)container);
        }

        return item switch
        {
            string => defaultStringTemplate,
            ToastNotificationContent => defaultToastNotificationTemplate,
            _ => base.SelectTemplate(
                item,
                container),
        };
    }
}