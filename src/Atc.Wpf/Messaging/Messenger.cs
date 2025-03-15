namespace Atc.Wpf.Messaging;

/// <summary>
/// The Messenger is a class allowing objects to exchange messages.
/// </summary>
public class Messenger : IMessenger
{
    private static readonly object CreationLock = new();
    private static IMessenger? defaultInstance;
    private readonly object registerLock = new();
    private readonly Dictionary<Type, List<WeakActionAndToken>> recipientsOfSubclassesAction = new();
    private readonly Dictionary<Type, List<WeakActionAndToken>> recipientsStrictAction = new();
    private readonly SynchronizationContext? context = SynchronizationContext.Current;
    private bool isCleanupRegistered;

    /// <summary>
    /// Gets the Messenger's default instance, allowing
    /// to register and send messages in a static manner.
    /// </summary>
    public static IMessenger Default
    {
        get
        {
            if (defaultInstance is not null)
            {
                return defaultInstance;
            }

            lock (CreationLock)
            {
                defaultInstance = new Messenger();
            }

            return defaultInstance;
        }
    }

    /// <summary>
    /// Provides a way to override the Messenger.Default instance with
    /// a custom instance, for example for unit testing purposes.
    /// </summary>
    /// <param name="newMessenger">The instance that will be used as Messenger.Default.</param>
    public static void OverrideDefault(IMessenger newMessenger)
    {
        defaultInstance = newMessenger;
    }

    /// <summary>
    /// Sets the Messenger's default (static) instance to null.
    /// </summary>
    public static void Reset()
    {
        defaultInstance = null;
    }

    /// <inheritdoc />
    public virtual void Register<TMessage>(object recipient, Action<TMessage> action, bool keepTargetAlive = false)
    {
        Register(recipient, token: null, receiveDerivedMessagesToo: false, action, keepTargetAlive);
    }

    /// <inheritdoc />
    public virtual void Register<TMessage>(object recipient, object? token, Action<TMessage> action, bool keepTargetAlive = false)
    {
        Register(recipient, token, receiveDerivedMessagesToo: false, action, keepTargetAlive);
    }

    /// <inheritdoc />
    public virtual void Register<TMessage>(object recipient, object? token, bool receiveDerivedMessagesToo, Action<TMessage> action, bool keepTargetAlive = false)
    {
        lock (registerLock)
        {
            var messageType = typeof(TMessage);

            var recipients = receiveDerivedMessagesToo
                ? recipientsOfSubclassesAction
                : recipientsStrictAction;

            lock (recipients)
            {
                List<WeakActionAndToken> list;

                if (!recipients.TryGetValue(messageType, out var result))
                {
                    list = [];
                    recipients.Add(messageType, list);
                }
                else
                {
                    list = result;
                }

                var weakAction = new WeakAction<TMessage>(recipient, action, keepTargetAlive);

                var item = new WeakActionAndToken
                {
                    Action = weakAction,
                    Token = token,
                };

                list.Add(item);
            }
        }

        RequestCleanup();
    }

    /// <inheritdoc />
    public virtual void Register<TMessage>(object recipient, bool receiveDerivedMessagesToo, Action<TMessage> action, bool keepTargetAlive = false)
    {
        Register(recipient, token: null, receiveDerivedMessagesToo, action, keepTargetAlive);
    }

    /// <inheritdoc />
    public virtual void Send<TMessage>(TMessage message)
    {
        SendToTargetOrType(message, messageTargetType: null, token: null);
    }

    /// <inheritdoc />
    public virtual void Send<TMessage, TTarget>(TMessage message)
    {
        SendToTargetOrType(message, typeof(TTarget), token: null);
    }

    /// <inheritdoc />
    public virtual void Send<TMessage>(TMessage message, object token)
    {
        SendToTargetOrType(message, messageTargetType: null, token);
    }

    /// <inheritdoc />
    public virtual void UnRegister(object recipient)
    {
        UnRegisterFromLists(recipient, recipientsOfSubclassesAction);
        UnRegisterFromLists(recipient, recipientsStrictAction);
    }

    /// <inheritdoc />
    public virtual void UnRegister<TMessage>(object recipient)
    {
        UnRegister<TMessage>(recipient, token: null, action: null);
    }

    /// <inheritdoc />
    public virtual void UnRegister<TMessage>(object recipient, object token)
    {
        UnRegister<TMessage>(recipient, token, action: null);
    }

    /// <inheritdoc />
    public virtual void UnRegister<TMessage>(object recipient, Action<TMessage> action)
    {
        UnRegister(recipient, token: null, action);
    }

    /// <inheritdoc />
    public virtual void UnRegister<TMessage>(object recipient, object? token, Action<TMessage>? action)
    {
        UnRegisterFromLists(recipient, token, action, recipientsStrictAction);
        UnRegisterFromLists(recipient, token, action, recipientsOfSubclassesAction);
        RequestCleanup();
    }

    /// <summary>
    /// Provides a non-static access to the static <see cref="Reset"/> method.
    /// Sets the Messenger's default (static) instance to null.
    /// </summary>
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "OK - By design.")]
    [SuppressMessage("Performance", "S2325:Mark members as static", Justification = "OK - By design.")]
    public void ResetAll()
    {
        Reset();
    }

    /// <summary>
    /// Notifies the Messenger that the lists of recipients should
    /// be scanned and cleaned up.
    /// Since recipients are stored as <see cref="WeakReference"/>,
    /// recipients can be garbage collected even though the Messenger keeps
    /// them in a list. During the cleanup operation, all "dead"
    /// recipients are removed from the lists. Since this operation
    /// can take a moment, it is only executed when the application is
    /// idle. For this reason, a user of the Messenger class should use
    /// <see cref="RequestCleanup"/> instead of forcing one with the
    /// <see cref="Cleanup" /> method.
    /// </summary>
    public void RequestCleanup()
    {
        if (isCleanupRegistered)
        {
            return;
        }

        var cleanupAction = Cleanup;

        if (context is not null)
        {
            context.Post(_ => cleanupAction(), state: null);
        }
        else
        {
            cleanupAction(); // run inline w/o a context
        }

        isCleanupRegistered = true;
    }

    /// <summary>
    /// Scans the recipients' lists for "dead" instances and removes them.
    /// Since recipients are stored as <see cref="WeakReference"/>,
    /// recipients can be garbage collected even though the Messenger keeps
    /// them in a list. During the cleanup operation, all "dead"
    /// recipients are removed from the lists. Since this operation
    /// can take a moment, it is only executed when the application is
    /// idle. For this reason, a user of the Messenger class should use
    /// <see cref="RequestCleanup"/> instead of forcing one with the
    /// <see cref="Cleanup" /> method.
    /// </summary>
    public void Cleanup()
    {
        CleanupList(recipientsOfSubclassesAction);
        CleanupList(recipientsStrictAction);
        isCleanupRegistered = false;
    }

    private static void CleanupList(IDictionary<Type, List<WeakActionAndToken>>? lists)
    {
        if (lists is null)
        {
            return;
        }

        lock (lists)
        {
            var listsToRemove = new List<Type>();
            foreach (var (key, value) in lists)
            {
                var recipientsToRemove = value
                    .Where(item => item.Action is null || !item.Action.IsAlive)
                    .ToList();

                foreach (var recipient in recipientsToRemove)
                {
                    _ = value.Remove(recipient);
                }

                if (value.Count == 0)
                {
                    listsToRemove.Add(key);
                }
            }

            foreach (var key in listsToRemove)
            {
                _ = lists.Remove(key);
            }
        }
    }

    private static void SendToList<TMessage>(
        TMessage message,
        IEnumerable<WeakActionAndToken>? weakActionsAndTokens,
        Type? messageTargetType,
        object? token)
    {
        if (weakActionsAndTokens is null)
        {
            return;
        }

        // Clone to protect from people registering in a "receive message" method
        // Correction Messaging BL0004.007
        var list = weakActionsAndTokens.ToList();
        var listClone = list.Take(list.Count).ToList();

        foreach (var item in listClone)
        {
            if (item.Action is IExecuteWithObject executeAction
                && item.Action.IsAlive
                && item.Action.Target is not null
                && (messageTargetType is null
                    || item.Action.Target.GetType() == messageTargetType
                    || messageTargetType.IsInstanceOfType(item.Action.Target))
                && ((item.Token is null && token is null)
                    || (token is not null && item.Token is not null && item.Token.Equals(token))))
            {
                executeAction.ExecuteWithObject(message);
            }
        }
    }

    [SuppressMessage("Major Code Smell", "S3267:Loops should be simplified with \"LINQ\" expressions", Justification = "OK.")]
    private static void UnRegisterFromLists(object? recipient, Dictionary<Type, List<WeakActionAndToken>>? lists)
    {
        if (recipient is null
            || lists is null
            || lists.Count == 0)
        {
            return;
        }

        lock (lists)
        {
            foreach (var messageType in lists.Keys)
            {
                foreach (var item in lists[messageType])
                {
                    if (item.Action is null)
                    {
                        continue;
                    }

                    var weakAction = (IExecuteWithObject)item.Action;

                    if (recipient == weakAction.Target)
                    {
                        weakAction.MarkForDeletion();
                    }
                }
            }
        }
    }

    private static void UnRegisterFromLists<TMessage>(
        object? recipient,
        object? token,
        Action<TMessage>? action,
        Dictionary<Type, List<WeakActionAndToken>>? lists)
    {
        var messageType = typeof(TMessage);

        if (recipient is null
            || lists is null
            || lists.Count == 0
            || !lists.TryGetValue(messageType, out var list))
        {
            return;
        }

        lock (lists)
        {
            foreach (var item in list)
            {
                if (item.Action is WeakAction<TMessage> weakActionCasted
                    && recipient == weakActionCasted.Target
                    && (action is null || action.Method.Name == weakActionCasted.MethodName)
                    && (token is null || token.Equals(item.Token)))
                {
                    item.Action!.MarkForDeletion();
                }
            }
        }
    }

    private void SendToTargetOrType<TMessage>(TMessage message, Type? messageTargetType, object? token)
    {
        var messageType = typeof(TMessage);

        List<WeakActionAndToken>? list = null;
        var listClone = recipientsOfSubclassesAction.Keys.Take(recipientsOfSubclassesAction.Count).ToList();
        foreach (var type in listClone)
        {
            if (messageType == type
                || messageType.IsSubclassOf(type)
                || type.IsAssignableFrom(messageType))
            {
                lock (recipientsOfSubclassesAction)
                {
                    list = recipientsOfSubclassesAction[type].Take(recipientsOfSubclassesAction[type].Count).ToList();
                }
            }

            SendToList(message, list, messageTargetType, token);
        }

        list = null;
        lock (recipientsStrictAction)
        {
            if (recipientsStrictAction.ContainsKey(messageType))
            {
                list = recipientsStrictAction[messageType]
                    .Take(recipientsStrictAction[messageType].Count)
                    .ToList();
            }
        }

        if (list is not null)
        {
            SendToList(message, list, messageTargetType, token);
        }

        RequestCleanup();
    }
}