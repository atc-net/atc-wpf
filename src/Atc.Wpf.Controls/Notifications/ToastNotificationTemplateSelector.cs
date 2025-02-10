namespace Atc.Wpf.Controls.Notifications;

public sealed class ToastNotificationTemplateSelector : DataTemplateSelector
{
    private DataTemplate? defaultStringTemplate;
    private DataTemplate? defaultToastNotificationTemplate;

    private void GetTemplatesFromResources(
        FrameworkElement? container)
    {
        defaultStringTemplate = container?.FindResource("DefaultStringTemplate") as DataTemplate;
        defaultToastNotificationTemplate = container?.FindResource("DefaultToastNotificationTemplate") as DataTemplate;
    }

    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        if (defaultStringTemplate == null && defaultToastNotificationTemplate == null)
        {
            GetTemplatesFromResources((FrameworkElement)container);
        }

        return item switch
        {
            string => defaultStringTemplate,
            ToastNotificationContent => defaultToastNotificationTemplate,
            _ => base.SelectTemplate(item, container),
        };
    }
}