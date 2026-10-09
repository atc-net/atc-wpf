namespace Atc.Wpf.Theming.Helpers;

/// <summary>
/// Helpers for evaluating and executing the command of an <see cref="ICommandSource"/>,
/// honoring routed command targets.
/// </summary>
public static class CommandHelpers
{
    /// <summary>
    /// Determines whether the command of the specified command source can execute.
    /// </summary>
    /// <param name="commandSource">The command source whose command, parameter and target are used.</param>
    /// <returns><see langword="true"/> if the command exists and can execute; otherwise <see langword="false"/>.</returns>
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

    /// <summary>
    /// Determines whether the specified command can execute, using the parameter and target of the command source.
    /// </summary>
    /// <param name="commandSource">The command source that supplies the parameter and target.</param>
    /// <param name="command">The command to evaluate.</param>
    /// <returns><see langword="true"/> if the command exists and can execute; otherwise <see langword="false"/>.</returns>
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

    /// <summary>
    /// Executes the command of the specified command source if it can execute.
    /// </summary>
    /// <param name="commandSource">The command source whose command, parameter and target are used.</param>
    [SecurityCritical]
    [SecuritySafeCritical]
    public static void ExecuteCommandSource(ICommandSource commandSource)
        => CriticalExecuteCommandSource(commandSource);

    /// <summary>
    /// Executes the specified command if it can execute, using the parameter and target of the command source.
    /// </summary>
    /// <param name="commandSource">The command source that supplies the parameter and target.</param>
    /// <param name="command">The command to execute.</param>
    [SecurityCritical]
    [SecuritySafeCritical]
    public static void ExecuteCommandSource(
        ICommandSource commandSource,
        ICommand? command)
        => CriticalExecuteCommandSource(
            commandSource,
            command);

    /// <summary>
    /// Executes the command of the specified command source if it can execute.
    /// </summary>
    /// <param name="commandSource">The command source whose command, parameter and target are used.</param>
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

    /// <summary>
    /// Executes the specified command if it can execute, using the parameter and target of the command source.
    /// </summary>
    /// <param name="commandSource">The command source that supplies the parameter and target.</param>
    /// <param name="command">The command to execute.</param>
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