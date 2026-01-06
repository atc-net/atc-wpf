namespace Atc.Wpf.Theming.Controls.Windows.Internal;

internal static class CommandHelpers
{
    public static bool CanExecuteCommandSource(ICommandSource commandSource)
    {
        var command = commandSource.Command;
        if (command is null)
        {
            return false;
        }

        var commandParameter = commandSource.CommandParameter ?? commandSource;
        if (command is not RoutedCommand routedCommand)
        {
            return command.CanExecute(commandParameter);
        }

        var target = commandSource.CommandTarget ?? commandSource as IInputElement;
        return routedCommand.CanExecute(
            commandParameter,
            target);
    }

    public static bool CanExecuteCommandSource(
        ICommandSource commandSource,
        ICommand? theCommand)
    {
        var command = theCommand;
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
    {
        CriticalExecuteCommandSource(commandSource);
    }

    [SecurityCritical]
    [SecuritySafeCritical]
    public static void ExecuteCommandSource(
        ICommandSource commandSource,
        ICommand? theCommand)
    {
        CriticalExecuteCommandSource(
            commandSource,
            theCommand);
    }

    [SecurityCritical]
    public static void CriticalExecuteCommandSource(
        ICommandSource commandSource)
    {
        var command = commandSource.Command;
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

    [SecurityCritical]
    public static void CriticalExecuteCommandSource(
        ICommandSource commandSource,
        ICommand? theCommand)
    {
        var command = theCommand;
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