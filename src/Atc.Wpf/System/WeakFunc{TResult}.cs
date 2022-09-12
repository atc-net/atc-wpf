// ReSharper disable once CheckNamespace
namespace System;

public class WeakFunc<TResult>
{
    private Func<TResult>? staticFunc;

    /// <summary>
    /// Gets a value indicating whether this instance is static.
    /// </summary>
    public bool IsStatic => staticFunc is not null;

    /// <summary>
    /// Gets the name of the method that this WeakFunc represents.
    /// </summary>
    public virtual string MethodName => staticFunc is null
        ? Method!.Name
        : staticFunc.Method.Name;

    /// <summary>
    /// Gets a value indicating whether the Func owner is still alive, or if it was collected
    /// by the Garbage Collector already.
    /// </summary>
    public virtual bool IsAlive
    {
        get
        {
            if (staticFunc is null
                && Reference is null
                && LiveReference is null)
            {
                return false;
            }

            if (staticFunc is not null)
            {
                return Reference is null || Reference.IsAlive;
            }

            // Non static action
            if (LiveReference is not null)
            {
                return true;
            }

            return Reference is not null && Reference.IsAlive;
        }
    }

    /// <summary>
    /// Gets the Func owner. This object is stored as a.
    /// <see cref="WeakReference" />.
    /// </summary>
    public object? Target => Reference?.Target;

    /// <summary>
    /// Gets or sets the <see cref="MethodInfo" /> corresponding to this WeakFunc.
    /// method passed in the constructor.
    /// </summary>
    protected MethodInfo? Method { get; set; }

    /// <summary>
    /// Gets or sets a WeakReference to this WeakFunc action's target.
    /// This is not necessarily the same as
    /// <see cref="Reference" />, for example if the
    /// method is anonymous.
    /// </summary>
    protected WeakReference? FuncReference { get; set; }

    /// <summary>
    /// Gets or sets saves the <see cref="FuncReference"/> as a hard reference. This is
    /// used in relation with this instance's constructor and only if
    /// the constructor's keepTargetAlive parameter is true.
    /// </summary>
    protected object? LiveReference { get; set; }

    /// <summary>
    /// Gets or sets a WeakReference to the target passed when constructing
    /// the WeakFunc. This is not necessarily the same as
    /// <see cref="FuncReference" />, for example if the
    /// method is anonymous.
    /// </summary>
    protected WeakReference? Reference { get; set; }

    /// <summary>
    /// Gets the owner of the Func that was passed as parameter.
    /// This is not necessarily the same as
    /// <see cref="Target" />, for example if the
    /// method is anonymous.
    /// </summary>
    protected object? FuncTarget => LiveReference ?? FuncReference?.Target;

    /// <summary>
    /// Initializes a new instance of the <see cref="WeakFunc{TResult}"/> class.
    /// </summary>
    /// <param name="func">The Func that will be associated to this instance.</param>
    /// <param name="keepTargetAlive">If true, the target of the Action will
    /// be kept as a hard reference, which might cause a memory leak. You should only set this
    /// parameter to true if the action is using closures.</param>
    public WeakFunc(Func<TResult>? func, bool keepTargetAlive = false)
        : this(func?.Target, func, keepTargetAlive)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WeakFunc{TResult}"/> class.
    /// </summary>
    /// <param name="target">The Func owner.</param>
    /// <param name="func">The Func that will be associated to this instance.</param>
    /// <param name="keepTargetAlive">If true, the target of the Action will
    /// be kept as a hard reference, which might cause a memory leak. You should only set this
    /// parameter to true if the action is using closures.</param>
    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Method should fail with an exception if func is null.")]
    public WeakFunc(object? target, Func<TResult>? func, bool keepTargetAlive = false)
    {
        if (func!.Method.IsStatic)
        {
            staticFunc = func;

            if (target is not null)
            {
                // Keep a reference to the target to control the
                // WeakAction's lifetime.
                Reference = new WeakReference(target);
            }

            return;
        }

        Method = func.Method;
        FuncReference = new WeakReference(func.Target);

        LiveReference = keepTargetAlive
            ? func.Target
            : null;
        Reference = new WeakReference(target);

#if DEBUG
        if (this.FuncReference.Target is not null && !keepTargetAlive)
        {
            var type = FuncReference.Target.GetType();

            if (type.Name.StartsWith("<>", StringComparison.Ordinal) && type.Name.Contains("DisplayClass", StringComparison.Ordinal))
            {
                Debug.WriteLine("You are attempting to register a lambda with a closure without using keepTargetAlive. Are you sure?");
            }
        }
#endif
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WeakFunc{TResult}"/> class.
    /// </summary>
    protected WeakFunc()
    {
    }

    /// <summary>
    /// Executes the action. This only happens if the Func owner
    /// is still alive.
    /// </summary>
    /// <returns>The result of the Func stored as reference.</returns>
    public TResult Execute()
    {
        if (staticFunc is not null)
        {
            return staticFunc();
        }

        var funcTarget = FuncTarget;

        if (!IsAlive)
        {
            return default!;
        }

        if (Method is null ||
            (LiveReference is null && FuncReference is null) ||
            funcTarget is null)
        {
            return default!;
        }

        return (TResult)Method.Invoke(funcTarget, parameters: null)!;
    }

    /// <summary>
    /// Sets the reference that this instance stores to null.
    /// </summary>
    protected void MarkForDeletion()
    {
        Reference = null;
        FuncReference = null;
        LiveReference = null;
        Method = null;
        staticFunc = null;
    }
}