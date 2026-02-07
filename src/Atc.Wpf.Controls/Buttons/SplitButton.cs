namespace Atc.Wpf.Controls.Buttons;

[TemplatePart(Name = "PART_ActionButton", Type = typeof(Button))]
[TemplatePart(Name = "PART_DropdownButton", Type = typeof(Button))]
[TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
public partial class SplitButton : ContentControl
{
    public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
        nameof(Click),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(SplitButton));

    [DependencyProperty]
    private ICommand? command;

    [DependencyProperty]
    private object? commandParameter;

    [DependencyProperty]
    private object? dropdownContent;

    [DependencyProperty]
    private DataTemplate? dropdownContentTemplate;

    [DependencyProperty(DefaultValue = false, Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PropertyChangedCallback = nameof(OnIsDropdownOpenChanged))]
    private bool isDropdownOpen;

    [DependencyProperty]
    private CornerRadius cornerRadius;

    private Button? actionButton;
    private Button? dropdownButton;
    private Popup? popup;

    static SplitButton()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(SplitButton),
            new FrameworkPropertyMetadata(typeof(SplitButton)));
    }

    public event RoutedEventHandler Click
    {
        add => AddHandler(ClickEvent, value);
        remove => RemoveHandler(ClickEvent, value);
    }

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

        actionButton = GetTemplateChild("PART_ActionButton") as Button;
        dropdownButton = GetTemplateChild("PART_DropdownButton") as Button;
        popup = GetTemplateChild("PART_Popup") as Popup;

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

    private static void OnIsDropdownOpenChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var splitButton = (SplitButton)d;

        if (splitButton.popup is not null)
        {
            splitButton.popup.IsOpen = (bool)e.NewValue;
        }
    }
}