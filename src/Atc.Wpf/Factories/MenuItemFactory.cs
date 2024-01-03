namespace Atc.Wpf.Factories;

public static class MenuItemFactory
{
    public static MenuItem Create(
        string labelText)
    {
        ArgumentException.ThrowIfNullOrEmpty(labelText);

        var menuItem = new MenuItem
        {
            Header = new TextBlock
            {
                Text = labelText,
            },
        };

        return menuItem;
    }

    public static MenuItem Create(
        string labelText,
        ImageSource icon)
    {
        ArgumentException.ThrowIfNullOrEmpty(labelText);
        ArgumentNullException.ThrowIfNull(icon);

        var menuItem = Create(labelText);
        menuItem.Icon = new AutoGreyableImage
        {
            Source = icon,
            Width = 16,
            Height = 16,
        };

        return menuItem;
    }

    public static MenuItem Create(
        string labelText,
        ICommand command)
    {
        ArgumentException.ThrowIfNullOrEmpty(labelText);
        ArgumentNullException.ThrowIfNull(command);

        var menuItem = Create(labelText);
        menuItem.Command = command;
        return menuItem;
    }

    public static MenuItem Create(
        string labelText,
        ImageSource icon,
        ICommand command)
    {
        ArgumentException.ThrowIfNullOrEmpty(labelText);
        ArgumentNullException.ThrowIfNull(icon);
        ArgumentNullException.ThrowIfNull(command);

        var menuItem = Create(labelText, icon);
        menuItem.Command = command;
        return menuItem;
    }

    public static MenuItem Create(
        string labelText,
        ImageSource icon,
        ICommand command,
        object commandParameter)
    {
        ArgumentException.ThrowIfNullOrEmpty(labelText);
        ArgumentNullException.ThrowIfNull(icon);
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(commandParameter);

        var menuItem = Create(labelText, icon, command);
        menuItem.CommandParameter = commandParameter;
        return menuItem;
    }
}