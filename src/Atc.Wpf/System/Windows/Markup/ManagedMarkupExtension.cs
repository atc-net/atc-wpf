// ReSharper disable once CheckNamespace
namespace System.Windows.Markup;

/// <summary>
/// Defines a base class for markup extensions which are managed by a central.
/// This allows the associated markup targets to be updated via the manager.
/// </summary>
/// <remarks>
/// The ManagedMarkupExtension holds a weak reference to the target object to allow it to update
/// the target.  A weak reference is used to avoid a circular dependency which would prevent the
/// target being garbage collected.
/// </remarks>
public abstract class ManagedMarkupExtension : MarkupExtension
{
    /// <summary>
    /// List of weak reference to the target DependencyObjects to allow them to
    /// be garbage collected.
    /// </summary>
    private readonly List<WeakReference> targetObjects = new();

    /// <summary>
    /// The target property.
    /// </summary>
    private object? targetProperty;

    /// <summary>
    /// Initializes a new instance of the <see cref="ManagedMarkupExtension" /> class.
    /// </summary>
    /// <param name="manager">The manager.</param>
    protected ManagedMarkupExtension(MarkupExtensionManager manager)
    {
        ArgumentNullException.ThrowIfNull(manager);

        manager.RegisterExtension(this);
    }

    /// <summary>
    /// Gets a value indicating whether this instance is target alive.
    /// </summary>
    /// <value>
    ///   <see langword="true" /> if this instance is target alive; otherwise, <see langword="false" />.
    /// </value>
    public bool IsTargetAlive =>
        targetObjects.Count == 0 || targetObjects.Exists(reference => reference.IsAlive);

    /// <summary>
    /// Gets a value indicating whether this instance is in design mode.
    /// </summary>
    /// <value>
    ///   <see langword="true" /> if this instance is in design mode; otherwise, <see langword="false" />.
    /// </value>
    protected bool IsInDesignMode =>
        targetObjects
            .Select(reference => reference.Target as DependencyObject)
            .Any(element => element is not null && DesignerProperties.GetIsInDesignMode(element));

    /// <summary>
    /// Gets the Target Property the extension is associated with.
    /// </summary>
    /// <remarks>
    /// Can either be a <see cref="DependencyProperty" /> or <see cref="PropertyInfo" />.
    /// </remarks>
    protected object? TargetProperty => targetProperty as DependencyProperty;

    /// <summary>
    /// Gets the type of the Target Property.
    /// </summary>
    protected Type? TargetPropertyType
    {
        get
        {
            var propertyType = targetProperty switch
            {
                DependencyProperty dependencyProperty => dependencyProperty.PropertyType,
                PropertyInfo info => info.PropertyType,
                _ => null,
            };

            return propertyType;
        }
    }

    /// <summary>
    /// When implemented in a derived class, returns an object that is set as the value of the target
    /// property for this markup extension.
    /// </summary>
    /// <returns>
    /// The object value to set on the property where the extension is applied.
    /// </returns>
    /// <param name="serviceProvider">Object that can provide services for the markup extension.</param>
    public override object? ProvideValue(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        var targetHelper = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        if (targetHelper?.TargetObject is null)
        {
            return null;
        }

        targetProperty = targetHelper.TargetProperty;
        if (targetHelper.TargetObject is not DependencyObject && targetProperty is DependencyProperty)
        {
            return this;
        }

        targetObjects.Add(new WeakReference(targetHelper.TargetObject));
        return GetValue();
    }

    /// <summary>
    /// Update the associated target.
    /// </summary>
    public void UpdateTarget()
    {
        if (targetProperty is null)
        {
            return;
        }

        foreach (var reference in targetObjects)
        {
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (targetProperty is DependencyProperty dependencyProperty)
            {
                var target = reference.Target as DependencyObject;
                target?.SetValue(dependencyProperty, GetValue());
            }
            else if (targetProperty is PropertyInfo info)
            {
                var target = reference.Target;
                if (target is not null)
                {
                    info.SetValue(target, GetValue(), index: null);
                }
            }
        }
    }

    /// <summary>
    /// Return the value associated with the key from the resource manager.
    /// </summary>
    /// <returns>The value from the resources if possible otherwise the default value.</returns>
    protected abstract object GetValue();
}