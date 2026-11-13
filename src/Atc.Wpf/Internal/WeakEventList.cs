namespace Atc.Wpf.Internal;

/// <summary>
/// The subscribers of a static event, held so that the event never keeps a subscriber alive.
/// </summary>
/// <remarks>
/// <para>
/// An instance-method handler is held through a weak reference to its target. A lambda that uses
/// <c>this</c> (and locals) becomes a compiler-generated closure that holds its owner; the closure is
/// attached to that owner, so it lives exactly as long as the owner and the subscription ends when the
/// owner is collected. A lambda without an owner (it captures only locals) has nothing else keeping it
/// alive, so it is held until it is removed with <c>-=</c>.
/// </para>
/// <para>
/// A subscriber that belongs to another UI thread is called on that thread.
/// </para>
/// </remarks>
internal sealed class WeakEventList
{
    private readonly Lock syncLock = new();
    private readonly List<WeakSubscription> subscriptions = [];

    public void Add(Delegate? handler)
    {
        if (handler is null)
        {
            return;
        }

        lock (syncLock)
        {
            subscriptions.RemoveAll(static s => !s.IsAlive);
            subscriptions.Add(new WeakSubscription(handler));
        }
    }

    public void Remove(Delegate? handler)
    {
        if (handler is null)
        {
            return;
        }

        lock (syncLock)
        {
            for (var i = subscriptions.Count - 1; i >= 0; i--)
            {
                if (subscriptions[i].Matches(handler))
                {
                    subscriptions[i].Detach();
                    subscriptions.RemoveAt(i);
                    return;
                }
            }
        }
    }

    public void Raise(
        object? sender,
        EventArgs args)
    {
        WeakSubscription[] snapshot;
        lock (syncLock)
        {
            var live = new List<WeakSubscription>(subscriptions.Count);
            for (var i = subscriptions.Count - 1; i >= 0; i--)
            {
                if (subscriptions[i].IsAlive)
                {
                    live.Add(subscriptions[i]);
                }
                else
                {
                    subscriptions.RemoveAt(i);
                }
            }

            snapshot = [.. live];
        }

        foreach (var subscription in snapshot)
        {
            subscription.Invoke(sender, args);
        }
    }

    private sealed class WeakSubscription
    {
        // Closures attached to their owner: an entry lives exactly as long as the owner.
        private static readonly ConditionalWeakTable<object, List<object>> ClosuresByOwner = [];

        private readonly MethodInfo method;
        private readonly bool isStatic;

        // The object whose lifetime ends the subscription: the handler target, or a closure's owner.
        private readonly WeakReference ownerRef;

        // The closure the method runs on, when it is not the owner itself.
        private readonly WeakReference? closureRef;

        // A closure without an owner has no other root, so it is held until it is removed.
        private readonly object? strongTarget;

        public WeakSubscription(Delegate handler)
        {
            method = handler.Method;
            var target = handler.Target;

            if (target is null)
            {
                isStatic = true;
                ownerRef = new WeakReference(target: null);
                return;
            }

            if (!IsCompilerGeneratedClosure(target.GetType()))
            {
                ownerRef = new WeakReference(target);
                return;
            }

            var owner = FindClosureOwner(target);
            if (owner is null)
            {
                ownerRef = new WeakReference(target);
                strongTarget = target;
                return;
            }

            ownerRef = new WeakReference(owner);
            closureRef = new WeakReference(target);
            lock (ClosuresByOwner)
            {
                ClosuresByOwner.GetOrCreateValue(owner).Add(target);
            }
        }

        public bool IsAlive => isStatic || ownerRef.IsAlive;

        public bool Matches(Delegate handler)
            => method == handler.Method
               && (isStatic
                   ? handler.Target is null
                   : ReferenceEquals(GetTarget(), handler.Target));

        public void Detach()
        {
            if (closureRef?.Target is not { } closure ||
                ownerRef.Target is not { } owner)
            {
                return;
            }

            lock (ClosuresByOwner)
            {
                if (ClosuresByOwner.TryGetValue(owner, out var closures))
                {
                    closures.Remove(closure);
                }
            }
        }

        public void Invoke(
            object? sender,
            EventArgs args)
        {
            var target = isStatic ? null : GetTarget();
            if (!isStatic && target is null)
            {
                return;
            }

            // A control can only be used on the thread that owns it, so a subscriber owned by another
            // UI thread is called on that thread; one whose thread has shut down is skipped.
            var affinity = isStatic ? null : ownerRef.Target ?? target;
            if (affinity is DispatcherObject { Dispatcher: { } dispatcher } &&
                !dispatcher.CheckAccess())
            {
                if (!dispatcher.HasShutdownStarted)
                {
                    _ = dispatcher.BeginInvoke(() => method.Invoke(target, [sender, args]));
                }

                return;
            }

            method.Invoke(target, [sender, args]);
        }

        private object? GetTarget()
            => strongTarget ?? closureRef?.Target ?? (closureRef is null ? ownerRef.Target : null);

        private static bool IsCompilerGeneratedClosure(Type type)
            => type.Name.StartsWith("<>", StringComparison.Ordinal)
               && type.IsDefined(typeof(CompilerGeneratedAttribute), inherit: false);

        // A closure that uses `this` stores it in a field named <>4__this; a closure nested in another
        // closure reaches it through that closure's field. Other shapes are treated as having no owner,
        // which keeps them alive (the behaviour before owners were tracked).
        private static object? FindClosureOwner(object closure)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            var fields = closure.GetType().GetFields(flags);
            var ownerField = Array.Find(fields, static f => f.Name == "<>4__this");
            if (ownerField?.GetValue(closure) is { } owner)
            {
                return owner;
            }

            foreach (var field in fields)
            {
                if (!IsCompilerGeneratedClosure(field.FieldType) ||
                    field.GetValue(closure) is not { } outer)
                {
                    continue;
                }

                var outerOwner = Array.Find(outer.GetType().GetFields(flags), static f => f.Name == "<>4__this");
                if (outerOwner?.GetValue(outer) is { } nestedOwner)
                {
                    return nestedOwner;
                }
            }

            return null;
        }
    }
}