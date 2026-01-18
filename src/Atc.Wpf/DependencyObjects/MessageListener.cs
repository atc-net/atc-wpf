namespace Atc.Wpf.DependencyObjects;

/// <summary>
/// Message listener, singleton pattern.
/// Inherit from DependencyObject to implement DataBinding.
/// </summary>
public sealed class MessageListener : DependencyObject
{
    public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
        nameof(Message),
        typeof(string),
        typeof(MessageListener),
        new PropertyMetadata(default(string)));

    public string Message
    {
        get => (string)GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

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
    public static MessageListener Instance
        => messageListener ??= new MessageListener();

    /// <summary>
    /// Receives the message.
    /// </summary>
    /// <param name="messageValue">The message value.</param>
    public void ReceiveMessage(string messageValue)
    {
        SetCurrentValue(MessageProperty, messageValue);
        DispatcherHelper.DoEvents();
    }
}