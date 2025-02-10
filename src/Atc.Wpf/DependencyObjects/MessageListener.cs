namespace Atc.Wpf.DependencyObjects;

/// <summary>
/// Message listener, singleton pattern.
/// Inherit from DependencyObject to implement DataBinding.
/// </summary>
public sealed class MessageListener : DependencyObject
{
    /// <summary>
    /// Message Property.
    /// </summary>
    public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
        nameof(Message),
        typeof(string),
        typeof(MessageListener),
        new UIPropertyMetadata(propertyChangedCallback: null));

    private static MessageListener? messageListener;

    /// <summary>
    /// Prevents a default instance of the <see cref="MessageListener" /> class from being created.
    /// </summary>
    private MessageListener()
    {
    }

    /// <summary>
    /// Gets MessageListener instance.
    /// </summary>
    public static MessageListener Instance => messageListener ??= new MessageListener();

    /// <summary>
    /// Gets or sets received message.
    /// </summary>
    public string Message
    {
        get => (string)GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    /// <summary>
    /// Receives the message.
    /// </summary>
    /// <param name="message">The message.</param>
    public void ReceiveMessage(string message)
    {
        SetCurrentValue(MessageProperty, message);
        DispatcherHelper.DoEvents();
    }
}