using System.Diagnostics;
using System.Reflection;

// ReSharper disable once UnusedMemberInSuper.Global
// ReSharper disable once CheckNamespace
namespace System
{
    /// <summary>
    /// Stores an <see cref="Action" /> without causing a hard reference
    /// to be created to the Action's owner. The owner can be garbage collected at any time.
    /// </summary>
    public class WeakAction
    {
        private Action? staticAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakAction" /> class.
        /// </summary>
        /// <param name="action">The action that will be associated to this instance.</param>
        /// <param name="keepTargetAlive">If true, the target of the Action will
        /// be kept as a hard reference, which might cause a memory leak.</param>
        public WeakAction(Action? action, bool keepTargetAlive = false)
            : this(action?.Target, action, keepTargetAlive)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakAction" /> class.
        /// </summary>
        /// <param name="target">The action's owner.</param>
        /// <param name="action">The action that will be associated to this instance.</param>
        /// <param name="keepTargetAlive">If true, the target of the Action will
        /// be kept as a hard reference, which might cause a memory leak.</param>
        public WeakAction(object? target, Action? action, bool keepTargetAlive = false)
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
        /// Initializes a new instance of the <see cref="WeakAction"/> class.
        /// Initializes an empty instance of the <see cref="WeakAction" /> class.
        /// </summary>
        protected WeakAction()
        {
        }

        /// <summary>
        /// Gets a value indicating whether the WeakAction is static or not.
        /// </summary>
        public bool IsStatic => staticAction is not null;

        /// <summary>
        /// Gets the name of the method that this WeakAction represents.
        /// </summary>
        public virtual string MethodName => staticAction is null
            ? this.Method!.Name
            : this.staticAction.Method.Name;

        /// <summary>
        /// Gets a value indicating whether the Action's owner is still alive, or if it was collected
        /// by the Garbage Collector already.
        /// </summary>
        public virtual bool IsAlive
        {
            get
            {
                if (this.staticAction is null
                    && this.Reference is null
                    && this.LiveReference is null)
                {
                    return false;
                }

                if (this.staticAction is not null)
                {
                    return this.Reference is null || this.Reference.IsAlive;
                }

                // Non static action
                if (this.LiveReference is not null)
                {
                    return true;
                }

                return this.Reference is not null && this.Reference.IsAlive;
            }
        }

        /// <summary>
        /// Gets the Action's owner. This object is stored as a <see cref="WeakReference" />.
        /// </summary>
        public object? Target => this.Reference?.Target;

        /// <summary>
        /// Gets or sets the <see cref="MethodInfo" /> corresponding to this WeakAction's
        /// method passed in the constructor.
        /// </summary>
        protected MethodInfo? Method { get; set; }

        /// <summary>
        /// Gets or sets a WeakReference to this WeakAction's action's target.
        /// This is not necessarily the same as
        /// <see cref="Reference" />, for example if the
        /// method is anonymous.
        /// </summary>
        protected WeakReference? ActionReference { get; set; }

        /// <summary>
        /// Gets or sets saves the <see cref="ActionReference"/> as a hard reference. This is
        /// used in relation with this instance's constructor and only if
        /// the constructor's keepTargetAlive parameter is true.
        /// </summary>
        protected object? LiveReference { get; set; }

        /// <summary>
        /// Gets or sets a WeakReference to the target passed when constructing
        /// the WeakAction. This is not necessarily the same as
        /// <see cref="ActionReference" />, for example if the
        /// method is anonymous.
        /// </summary>
        protected WeakReference? Reference { get; set; }

        /// <summary>
        /// Gets the target of the weak reference.
        /// </summary>
        protected object? ActionTarget => LiveReference ?? ActionReference?.Target;

        /// <summary>
        /// Executes the action. This only happens if the action's owner
        /// is still alive.
        /// </summary>
        public void Execute()
        {
            if (this.staticAction is not null)
            {
                this.staticAction();
                return;
            }

            var actionTarget = this.ActionTarget;

            if (!this.IsAlive)
            {
                return;
            }

            if (this.Method is null || (this.LiveReference is null && this.ActionReference is null) || actionTarget is null)
            {
                return;
            }

            _ = this.Method.Invoke(actionTarget, parameters: null);
        }

        /// <summary>
        /// Sets the reference that this instance stores to null.
        /// </summary>
        public void MarkForDeletion()
        {
            this.Reference = null;
            this.ActionReference = null;
            this.LiveReference = null;
            this.Method = null;
            this.staticAction = null;
        }
    }
}