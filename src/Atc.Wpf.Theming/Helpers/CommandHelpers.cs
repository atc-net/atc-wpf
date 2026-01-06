namespace Atc.Wpf.Theming.Helpers;

public static class CommandHelpers
{
    public static bool CanExecuteCommandSource(ICommandSource commandSource)
    {
        ArgumentNullException.ThrowIfNull(commandSource);

        var command = commandSource.Command;
        if (command == null)
        {
            return false;
        }

        var commandParameter = commandSource.CommandParameter ?? commandSource;
        if (command is RoutedCommand routedCommand)
        {
            var target = commandSource.CommandTarget ?? commandSource as IInputElement;
            return routedCommand.CanExecute(
                commandParameter,
                target);
        }

        return command.CanExecute(commandParameter);
    }

    public static bool CanExecuteCommandSource(
        ICommandSource commandSource,
        ICommand? command)
    {
        ArgumentNullException.ThrowIfNull(commandSource);

        if (command is null)
        {
            return false;
        }

        var commandParameter = commandSource.CommandParameter ?? commandSource;
        if (command is RoutedCommand routedCommand)
        {
            var target = commandSource.CommandTarget ?? commandSource as IInputElement;
            return routedCommand.CanExecute(
                commandParameter,
                target);
        }

        return command.CanExecute(commandParameter);
    }

    [SecurityCritical]
    [SecuritySafeCritical]
    public static void ExecuteCommandSource(ICommandSource commandSource)
        => CriticalExecuteCommandSource(commandSource);

    [SecurityCritical]
    [SecuritySafeCritical]
    public static void ExecuteCommandSource(
        ICommandSource commandSource,
        ICommand? command)
        => CriticalExecuteCommandSource(
            commandSource,
            command);

    [SecurityCritical]
    public static void CriticalExecuteCommandSource(
        ICommandSource commandSource)
    {
        ArgumentNullException.ThrowIfNull(commandSource);

        var command = commandSource.Command;
        if (command == null)
        {
            return;
        }

        var commandParameter = commandSource.CommandParameter ?? commandSource;
        if (command is RoutedCommand routedCommand)
        {
            var target = commandSource.CommandTarget ?? commandSource as IInputElement;
            if (routedCommand.CanExecute(
                commandParameter,
                target))
            {
                routedCommand.Execute(
                    commandParameter,
                    target);
            }
        }
        else
        {
            if (command.CanExecute(commandParameter))
            {
                command.Execute(commandParameter);
            }
        }
    }

    [SecurityCritical]
    public static void CriticalExecuteCommandSource(
        ICommandSource commandSource,
        ICommand? command)
    {
        ArgumentNullException.ThrowIfNull(commandSource);

        if (command is null)
        {
            return;
        }

        var commandParameter = commandSource.CommandParameter ?? commandSource;
        if (command is RoutedCommand routedCommand)
        {
            var target = commandSource.CommandTarget ?? commandSource as IInputElement;
            if (routedCommand.CanExecute(
                commandParameter,
                target))
            {
                routedCommand.Execute(
                    commandParameter,
                    target);
            }
        }
        else
        {
            if (command.CanExecute(commandParameter))
            {
                command.Execute(commandParameter);
            }
        }
    }
}