namespace Atc.Wpf.Controls.Buttons;

/// <summary>A button with a primary action and a dropdown arrow that opens a popup with secondary content.</summary>
[DefaultStyleKey]
public partial class SplitButton : ContentControl
{
    /// <summary>Occurs when the primary action part of the button is clicked.</summary>
    [RoutedEvent(RoutingStrategy.Bubble)]
    private static readonly RoutedEvent click;

    [DependencyProperty]
    private ICommand? command;

    [DependencyProperty]
    private object? commandParameter;

    [DependencyProperty]
    private object? dropdownContent;

    [DependencyProperty]
    private DataTemplate? dropdownContentTemplate;

    [DependencyProperty(DefaultValue = false, Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, GeneratePartialHooks = true)]
    private bool isDropdownOpen;

    [DependencyProperty]
    private CornerRadius cornerRadius;

    [TemplatePartField("PART_ActionButton")]
    private Button? actionButton;

    [TemplatePartField("PART_DropdownButton")]
    private Button? dropdownButton;

    [TemplatePartField("PART_Popup")]
    private Popup? popup;

    static SplitButton()
    {
        OverrideDefaultStyleKey();
        StyleCheck.Register(typeof(SplitButton));
    }

    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (actionButton is not null)
        {
            actionButton.Click -= OnActionButtonClick;
        }

        if (dropdownButton is not null)
        {
            dropdownButton.Click -= OnDropdownButtonClick;
        }

        if (popup is not null)
        {
            popup.Closed -= OnPopupClosed;
        }

        ResolveTemplateParts();

        if (actionButton is not null)
        {
            actionButton.Click += OnActionButtonClick;
        }

        if (dropdownButton is not null)
        {
            dropdownButton.Click += OnDropdownButtonClick;
        }

        if (popup is not null)
        {
            popup.Closed += OnPopupClosed;
        }
    }

    /// <inheritdoc />
    protected override void OnKeyDown(KeyEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnKeyDown(e);

        switch (e.Key)
        {
            case Key.Enter or Key.Space:
                ExecutePrimaryAction();
                e.Handled = true;
                break;
            case Key.Down when Keyboard.Modifiers == ModifierKeys.Alt:
            case Key.F4:
                SetCurrentValue(IsDropdownOpenProperty, true);
                e.Handled = true;
                break;
            case Key.Escape when IsDropdownOpen:
                SetCurrentValue(IsDropdownOpenProperty, false);
                e.Handled = true;
                break;
        }
    }

    private void ExecutePrimaryAction()
    {
        RaiseEvent(new RoutedEventArgs(ClickEvent, this));

        if (Command is not null && Command.CanExecute(CommandParameter))
        {
            Command.Execute(CommandParameter);
        }
    }

    private void OnActionButtonClick(
        object sender,
        RoutedEventArgs e)
    {
        ExecutePrimaryAction();
    }

    private void OnDropdownButtonClick(
        object sender,
        RoutedEventArgs e)
    {
        SetCurrentValue(IsDropdownOpenProperty, !IsDropdownOpen);
    }

    private void OnPopupClosed(
        object? sender,
        EventArgs e)
    {
        SetCurrentValue(IsDropdownOpenProperty, false);
    }

    partial void OnIsDropdownOpenChanged(
        bool oldValue,
        bool newValue)
    {
        if (popup is not null)
        {
            popup.IsOpen = newValue;
        }
    }
}