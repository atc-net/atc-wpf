// ReSharper disable once CheckNamespace
namespace System;

/// <summary>
/// Stores an Action without causing a hard reference
/// to be created to the Action's owner. The owner can be garbage collected at any time.
/// </summary>
/// <typeparam name="T">The type of the Action's parameter.</typeparam>
public class WeakAction<T> : WeakAction, IExecuteWithObject
{
    private Action<T>? staticAction;

    /// <summary>
    /// Initializes a new instance of the <see cref="WeakAction{T}"/> class.
    /// </summary>
    /// <param name="action">The action that will be associated to this instance.</param>
    /// <param name="keepTargetAlive">If true, the target of the Action will
    /// be kept as a hard reference, which might cause a memory leak.</param>
    public WeakAction(Action<T>? action, bool keepTargetAlive = false)
        : this(action?.Target, action, keepTargetAlive)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WeakAction{T}"/> class.
    /// </summary>
    /// <param name="target">The action's owner.</param>
    /// <param name="action">The action that will be associated to this instance.</param>
    /// <param name="keepTargetAlive">If true, the target of the Action will
    /// be kept as a hard reference, which might cause a memory leak.</param>
    public WeakAction(object? target, Action<T>? action, bool keepTargetAlive = false)
    {
        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        if (action.Method.IsStatic)
        {
            this.staticAction = action;

            if (target is not null)
            {
                // Keep a reference to the target to control the
                // WeakAction's lifetime.
                this.Reference = new WeakReference(target);
            }

            return;
        }

        this.Method = action.Method;
        this.ActionReference = new WeakReference(action.Target);

        this.LiveReference = keepTargetAlive
            ? action.Target
            : null;
        this.Reference = new WeakReference(target);

#if DEBUG
        if (this.ActionReference.Target is not null && !keepTargetAlive)
        {
            var type = this.ActionReference.Target.GetType();

            if (type.Name.StartsWith("<>", StringComparison.Ordinal) && type.Name.Contains("DisplayClass", StringComparison.Ordinal))
            {
                Debug.WriteLine("You are attempting to register a lambda with a closure without using keepTargetAlive. Are you sure?");
            }
        }
#endif
    }

    /// <summary>
    /// Gets the name of the method that this WeakAction represents.
    /// </summary>
    public override string MethodName => this.staticAction is null
        ? this.Method!.Name
        : this.staticAction.Method.Name;

    /// <summary>
    /// Gets a value indicating whether the Action's owner is still alive, or if it was collected
    /// by the Garbage Collector already.
    /// </summary>
    public override bool IsAlive
    {
        get
        {
            if (this.staticAction is null && this.Reference is null)
            {
                return false;
            }

            return this.staticAction is null
                ? this.Reference is not null && this.Reference.IsAlive
                : this.Reference is null || this.Reference.IsAlive;
        }
    }

    /// <summary>
    /// Executes the action. This only happens if the action's owner
    /// is still alive.
    /// </summary>
    /// <param name="parameter">A parameter to be passed to the action.</param>
    public void Execute(T parameter = default)
    {
        if (this.staticAction is not null)
        {
            this.staticAction(parameter!);
            return;
        }

        var actionTarget = this.ActionTarget;

        if (!this.IsAlive)
        {
            return;
        }

        if (this.Method is not null
            && (this.LiveReference is not null || this.ActionReference is not null)
            && actionTarget is not null)
        {
            _ = this.Method.Invoke(
                actionTarget,
                new object[] { parameter! });
        }
    }

    /// <summary>
    /// Executes the action with a parameter of type object. This parameter
    /// will be casted to T. This method implements <see cref="IExecuteWithObject.ExecuteWithObject" />
    /// and can be useful if you store multiple WeakAction{T} instances but don't know in advance
    /// what type T represents.
    /// </summary>
    /// <param name="parameter">The parameter that will be passed to the action after
    ///     being casted to T.</param>
    public void ExecuteWithObject(object? parameter)
    {
        if (parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        var parameterCasted = (T)parameter;
        this.Execute(parameterCasted);
    }

    /// <summary>
    /// Sets all the actions that this WeakAction contains to null,
    /// which is a signal for containing objects that this WeakAction
    /// should be deleted.
    /// </summary>
    public new void MarkForDeletion()
    {
        this.staticAction = null;
        base.MarkForDeletion();
    }
}