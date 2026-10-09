namespace Atc.Wpf.Controls.Factories;

/// <summary>Factory methods for creating <see cref="MenuItem"/> instances.</summary>
public static class MenuItemFactory
{
    /// <summary>Creates a menu item with the specified header text.</summary>
    public static MenuItem Create(string labelText)
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

    /// <summary>Creates a menu item with the specified header text and icon.</summary>
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

    /// <summary>Creates a menu item with the specified header text and command.</summary>
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

    /// <summary>Creates a menu item with the specified header text, icon and command.</summary>
    public static MenuItem Create(
        string labelText,
        ImageSource icon,
        ICommand command)
    {
        ArgumentException.ThrowIfNullOrEmpty(labelText);
        ArgumentNullException.ThrowIfNull(icon);
        ArgumentNullException.ThrowIfNull(command);

        var menuItem = Create(
            labelText,
            icon);
        menuItem.Command = command;
        return menuItem;
    }

    /// <summary>Creates a menu item with the specified header text, icon, command and command parameter.</summary>
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

        var menuItem = Create(
            labelText,
            icon,
            command);
        menuItem.CommandParameter = commandParameter;
        return menuItem;
    }

    /// <summary>Creates a menu item with the specified header text, icon, command and input gesture text.</summary>
    public static MenuItem Create(
        string labelText,
        ImageSource icon,
        ICommand command,
        string inputGestureText)
    {
        ArgumentException.ThrowIfNullOrEmpty(labelText);
        ArgumentNullException.ThrowIfNull(icon);
        ArgumentNullException.ThrowIfNull(command);
        ArgumentException.ThrowIfNullOrEmpty(inputGestureText);

        var menuItem = Create(
            labelText,
            icon,
            command);
        menuItem.InputGestureText = inputGestureText;
        return menuItem;
    }

    /// <summary>Creates a menu item with the specified header text, icon, command, command parameter and input gesture text.</summary>
    public static MenuItem Create(
        string labelText,
        ImageSource icon,
        ICommand command,
        object commandParameter,
        string inputGestureText)
    {
        ArgumentException.ThrowIfNullOrEmpty(labelText);
        ArgumentNullException.ThrowIfNull(icon);
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(commandParameter);
        ArgumentException.ThrowIfNullOrEmpty(inputGestureText);

        var menuItem = Create(
            labelText,
            icon,
            command,
            commandParameter);
        menuItem.InputGestureText = inputGestureText;
        return menuItem;
    }
}