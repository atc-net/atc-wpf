namespace Atc.Wpf.Translation;

/// <summary>
/// Stores a single subscriber to <see cref="CultureManager.UiCultureChanged"/> as a
/// weak reference, so a long-lived static subscription does not root the subscriber.
/// </summary>
/// <remarks>
/// For instance-method subscriptions the delegate target is held weakly. For
/// compiler-generated closures (lambda captures) the closure is held strongly,
/// because the delegate's only strong root would otherwise be lost the moment
/// the caller releases its local reference.
/// </remarks>
internal sealed class WeakSubscription
{
    private readonly WeakReference targetRef;
    private readonly MethodInfo method;
    private readonly bool isStatic;

    // For compiler-generated closures (lambda captures) the delegate's only
    // strong root is normally the closure itself, so we have to hold it strongly
    // or the subscription dies on the next GC. Detected via [CompilerGenerated].
    private readonly object? strongTarget;

    public WeakSubscription(EventHandler<UiCultureEventArgs> handler)
    {
        method = handler.Method;
        var target = handler.Target;

        if (target is null)
        {
            isStatic = true;
            targetRef = new WeakReference(target: null);
            strongTarget = null;
            return;
        }

        isStatic = false;
        targetRef = new WeakReference(target);
        strongTarget = IsCompilerGeneratedClosure(target.GetType())
            ? target
            : null;
    }

    public bool IsAlive => isStatic || targetRef.IsAlive;

    public bool Matches(EventHandler<UiCultureEventArgs> handler)
        => method == handler.Method
            && (isStatic
                ? handler.Target is null
                : ReferenceEquals(targetRef.Target, handler.Target));

    public void Invoke(
        object? sender,
        UiCultureEventArgs args)
    {
        var target = isStatic ? null : (strongTarget ?? targetRef.Target);
        if (!isStatic && target is null)
        {
            return;
        }

        method.Invoke(target, [sender, args]);
    }

    private static bool IsCompilerGeneratedClosure(Type type)
        => type.Name.StartsWith("<>", StringComparison.Ordinal)
            && type.IsDefined(typeof(CompilerGeneratedAttribute), inherit: false);
}