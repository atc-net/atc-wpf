namespace Atc.Wpf.Theming.Helpers;

public static class AtcAppsCommands
{
    private static RoutedUICommand? clearControlCommand;
    private static RoutedUICommand? searchCommand;

    static AtcAppsCommands()
    {
        CommandManager.RegisterClassCommandBinding(
            typeof(Window),
            new CommandBinding(
                ClearControlCommand,
                (_, args) => ClearControl(args),
                (_, args) => CanClearControl(args)));
    }

    public static ICommand ClearControlCommand
        => clearControlCommand ??= new RoutedUICommand(
            "Clear",
            nameof(ClearControlCommand),
            typeof(AtcAppsCommands));

    public static ICommand SearchCommand
        => searchCommand ??= new RoutedUICommand(
            "Search",
            nameof(SearchCommand),
            typeof(AtcAppsCommands));

    private static void CanClearControl(CanExecuteRoutedEventArgs args)
    {
        ArgumentNullException.ThrowIfNull(args);

        if (args.Handled)
        {
            return;
        }

        if (args.OriginalSource is not DependencyObject control ||
            !TextBoxHelper.GetClearTextButton(control))
        {
            return;
        }

        args.CanExecute = control switch
        {
            DatePicker datePicker => !ControlsHelper.GetIsReadOnly(datePicker),
            TextBoxBase textBox => !textBox.IsReadOnly,
            ComboBox comboBox => !comboBox.IsReadOnly,
            _ => true,
        };
    }

    public static void ClearControl(ExecutedRoutedEventArgs args)
    {
        ArgumentNullException.ThrowIfNull(args);

        if (args.Handled)
        {
            return;
        }

        if (args.OriginalSource is not DependencyObject control ||
            !TextBoxHelper.GetClearTextButton(control))
        {
            return;
        }

        switch (control)
        {
            case RichTextBox richTextBox:
                richTextBox.Document?.Blocks.Clear();
                richTextBox.Selection
                    .Select(
                        richTextBox.CaretPosition,
                        richTextBox.CaretPosition);
                break;
            case DatePicker datePicker:
                datePicker.SetCurrentValue(
                    DatePicker.SelectedDateProperty,
                    value: null);
                datePicker.SetCurrentValue(
                    DatePicker.TextProperty,
                    string.Empty);
                datePicker.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
                break;
            case TextBox textBox:
                textBox.Clear();
                textBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
                break;
            case PasswordBox passwordBox:
                passwordBox.Clear();
                passwordBox.GetBindingExpression(PasswordBoxBindingBehavior.PasswordProperty)?.UpdateSource();
                break;
            case ComboBox comboBox:
            {
                if (comboBox.IsEditable)
                {
                    comboBox.SetCurrentValue(
                        ComboBox.TextProperty,
                        string.Empty);
                    comboBox.GetBindingExpression(ComboBox.TextProperty)?.UpdateSource();
                }

                comboBox.SetCurrentValue(
                    Selector.SelectedItemProperty,
                    value: null);
                comboBox.GetBindingExpression(Selector.SelectedItemProperty)?.UpdateSource();

                break;
            }
        }
    }
}