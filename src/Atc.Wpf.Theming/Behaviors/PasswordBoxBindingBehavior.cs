namespace Atc.Wpf.Theming.Behaviors;

/// <summary>
/// Provides a bindable <c>Password</c> attached property for a <see cref="PasswordBox"/>.
/// </summary>
[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public partial class PasswordBoxBindingBehavior : Behavior<PasswordBox>
{
    /// <summary>
    /// Gets or sets the Password property on the PasswordBox control. This is a dependency property.
    /// </summary>
    [AttachedProperty(
        DefaultValue = "",
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        PropertyChangedCallback = nameof(OnPasswordPropertyChanged),
        GenerateClrAccessors = false)]
    private static string password;

    [AttachedProperty(
        DefaultValue = false,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None,
        MetadataType = DependencyPropertyMetadataType.UIPropertyMetadata,
        AccessModifier = PropertyAccessModifier.Private,
        GenerateDocumentation = false)]
    private static bool isChanging;

    [AttachedProperty(
        NullElementHandling = NullElementHandling.None,
        MetadataType = DependencyPropertyMetadataType.UIPropertyMetadata,
        AccessModifier = PropertyAccessModifier.Private,
        GenerateDocumentation = false)]
    private static TextSelection? selection;

    [AttachedProperty(
        NullElementHandling = NullElementHandling.None,
        MetadataType = DependencyPropertyMetadataType.UIPropertyMetadata,
        AccessModifier = PropertyAccessModifier.Private,
        GenerateDocumentation = false)]
    private static TextBox? revealedPasswordTextBox;

    /// <summary>Helper for getting <see cref="PasswordProperty"/> from <paramref name="d"/>.</summary>
    /// <param name="d"><see cref="DependencyObject"/> to read <see cref="PasswordProperty"/> from.</param>
    /// <returns>Password property value.</returns>
    public static string GetPassword(DependencyObject d)
        => (string)d.GetValue(PasswordProperty);

    /// <summary>Helper for setting <see cref="PasswordProperty"/> on <paramref name="d"/>.</summary>
    /// <param name="d"><see cref="DependencyObject"/> to set <see cref="PasswordProperty"/> on.</param>
    /// <param name="value">Password property value.</param>
    public static void SetPassword(
        DependencyObject d,
        string value)
        => d.SetValue(PasswordProperty, value);

    /// <summary>
    /// Handles changes to the 'Password' attached property.
    /// </summary>
    private static void OnPasswordPropertyChanged(
        DependencyObject sender,
        DependencyPropertyChangedEventArgs e)
    {
        if (sender is not PasswordBox targetPasswordBox)
        {
            return;
        }

        targetPasswordBox.PasswordChanged -= PasswordBoxPasswordChanged;
        if (!GetIsChanging(targetPasswordBox))
        {
            targetPasswordBox.Password = (string)e.NewValue;
        }

        targetPasswordBox.PasswordChanged += PasswordBoxPasswordChanged;
    }

    /// <summary>
    /// Handle the 'PasswordChanged'-event on the PasswordBox
    /// </summary>
    private static void PasswordBoxPasswordChanged(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is not PasswordBox passwordBox)
        {
            return;
        }

        SetIsChanging(passwordBox, value: true);
        SetPassword(passwordBox, passwordBox.Password);
        SetIsChanging(passwordBox, value: false);
    }

    private static void SetRevealedPasswordCaretIndex(PasswordBox passwordBox)
    {
        var textBox = GetRevealedPasswordTextBox(passwordBox);
        if (textBox is null)
        {
            return;
        }

        var caretPos = GetPasswordBoxCaretPosition(passwordBox);
        textBox.CaretIndex = caretPos;
    }

    private static int GetPasswordBoxCaretPosition(PasswordBox passwordBox)
    {
        var textSelection = GetSelection(passwordBox);
        var tTextRange = textSelection?
            .GetType()
            .GetInterfaces()
            .FirstOrDefault(i => i.Name == "ITextRange");
        var oStart = tTextRange?
            .GetProperty("Start")?
            .GetGetMethod()?
            .Invoke(textSelection, parameters: null);
        var value = oStart?
            .GetType()
            .GetProperty("Offset", BindingFlags.NonPublic | BindingFlags.Instance)?
            .GetValue(oStart, index: null) as int?;
        var caretPosition = value.GetValueOrDefault(0);
        return caretPosition;
    }

    /// <summary>
    /// Called after the behavior is attached to an AssociatedObject.
    /// </summary>
    /// <remarks>
    /// Override this to hook up functionality to the AssociatedObject.
    /// </remarks>
    protected override void OnAttached()
    {
        base.OnAttached();

        AssociatedObject.PasswordChanged += PasswordBoxPasswordChanged;
        AssociatedObject.Loaded += PasswordBoxLoaded;
        var textSelection = GetSelection(AssociatedObject);
        if (textSelection is not null)
        {
            textSelection.Changed += PasswordBoxSelectionChanged;
        }
    }

    private void PasswordBoxLoaded(
        object sender,
        RoutedEventArgs e)
    {
        SetPassword(AssociatedObject, AssociatedObject.Password);

        var textBox = AssociatedObject.FindChild<TextBox>("RevealedPassword");
        if (textBox is null)
        {
            return;
        }

        var textSelection = GetSelection(AssociatedObject);
        if (textSelection is not null)
        {
            return;
        }

        var infos = AssociatedObject
            .GetType()
            .GetProperty("Selection", BindingFlags.NonPublic | BindingFlags.Instance);
        textSelection = infos?.GetValue(AssociatedObject, null) as TextSelection;
        SetSelection(AssociatedObject, textSelection);
        if (textSelection is null)
        {
            return;
        }

        SetRevealedPasswordTextBox(AssociatedObject, textBox);
        SetRevealedPasswordCaretIndex(AssociatedObject);

        textSelection.Changed += PasswordBoxSelectionChanged;
    }

    private void PasswordBoxSelectionChanged(
        object? sender,
        EventArgs e)
        => SetRevealedPasswordCaretIndex(AssociatedObject);

    /// <summary>
    /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
    /// </summary>
    /// <remarks>
    /// Override this to unhook functionality from the AssociatedObject.
    /// </remarks>
    protected override void OnDetaching()
    {
        // it seems, it was already detached, or never attached
        if (AssociatedObject != null)
        {
            var textSelection = GetSelection(AssociatedObject);
            if (textSelection != null)
            {
                textSelection.Changed -= PasswordBoxSelectionChanged;
            }

            AssociatedObject.Loaded -= PasswordBoxLoaded;
            AssociatedObject.PasswordChanged -= PasswordBoxPasswordChanged;
        }

        base.OnDetaching();
    }
}