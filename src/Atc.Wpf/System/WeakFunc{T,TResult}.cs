// ReSharper disable once CheckNamespace
namespace System;

/// <summary>
/// Stores an Func without causing a hard reference
/// to be created to the Func owner. The owner can be garbage collected at any time.
/// </summary>
/// <typeparam name="T">The type of the Func parameter.</typeparam>
/// <typeparam name="TResult">The type of the Func return value.</typeparam>
public class WeakFunc<T, TResult> : WeakFunc<TResult>, IExecuteWithObjectAndResult
{
    private Func<T, TResult>? staticFunc;

    /// <summary>
    /// Initializes a new instance of the <see cref="WeakFunc{T, TResult}"/> class.
    /// </summary>
    /// <param name="func">The Func that will be associated to this instance.</param>
    /// <param name="keepTargetAlive">If true, the target of the Action will
    /// be kept as a hard reference, which might cause a memory leak. You should only set this
    /// parameter to true if the action is using closures.</param>
    public WeakFunc(Func<T, TResult>? func, bool keepTargetAlive = false)
        : this(func?.Target, func, keepTargetAlive)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WeakFunc{T, TResult}"/> class.
    /// </summary>
    /// <param name="target">The Func owner.</param>
    /// <param name="func">The Func that will be associated to this instance.</param>
    /// <param name="keepTargetAlive">If true, the target of the Action will
    /// be kept as a hard reference, which might cause a memory leak. You should only set this
    /// parameter to true if the action is using closures.</param>
    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Method should fail with an exception if func is null.")]
    public WeakFunc(object? target, Func<T, TResult>? func, bool keepTargetAlive = false)
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
            var type = this.FuncReference.Target.GetType();

            if (type.Name.StartsWith("<>", StringComparison.Ordinal) && type.Name.Contains("DisplayClass", StringComparison.Ordinal))
            {
                Debug.WriteLine("You are attempting to register a lambda with a closure without using keepTargetAlive. Are you sure?");
            }
        }
#endif
    }

    /// <summary>
    /// Gets the name of the method that this WeakFunc represents.
    /// </summary>
    public override string MethodName => staticFunc is null
        ? Method!.Name
        : staticFunc.Method.Name;

    /// <summary>
    /// Gets a value indicating whether the Func owner is still alive, or if it was collected
    /// by the Garbage Collector already.
    /// </summary>
    public override bool IsAlive
    {
        get
        {
            if (staticFunc is null && Reference is null)
            {
                return false;
            }

            return staticFunc is null
                ? Reference is not null && Reference.IsAlive
                : Reference is null || Reference.IsAlive;
        }
    }

    /// <summary>
    /// Executes the Func. This only happens if the Func owner
    /// is still alive.
    /// </summary>
    /// <param name="parameter">A parameter to be passed to the action.</param>
    /// <returns>The result of the Func stored as reference.</returns>
    public TResult Execute(T? parameter = default)
    {
        if (staticFunc is not null)
        {
            return staticFunc(parameter!);
        }

        var funcTarget = FuncTarget;

        if (!IsAlive)
        {
            return default!;
        }

        if (Method is not null
            && (LiveReference is not null || FuncReference is not null)
            && funcTarget is not null)
        {
            return (TResult)Method.Invoke(
                funcTarget,
                new object[] { parameter! })!;
        }

        return default!;
    }

    /// <summary>
    /// Executes the Func with a parameter of type object. This parameter
    /// will be casted to T. This method implements <see cref="IExecuteWithObject.ExecuteWithObject" />
    /// and can be useful if you store multiple WeakFunc{T} instances but don't know in advance
    /// what type T represents.
    /// </summary>
    /// <param name="parameter">The parameter that will be passed to the Func after
    /// being casted to T.</param>
    /// <returns>The result of the execution as object, to be casted to T.</returns>
    public object ExecuteWithObject(object parameter)
    {
        var parameterCasted = (T)parameter;
        return Execute(parameterCasted)!;
    }

    /// <summary>
    /// Sets all the functions that this WeakFunc contains to null,
    /// which is a signal for containing objects that this WeakFunc
    /// should be deleted.
    /// </summary>
    public new void MarkForDeletion()
    {
        staticFunc = null;
        base.MarkForDeletion();
    }
}