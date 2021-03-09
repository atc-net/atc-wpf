using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace System.Windows.Markup
{
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
        private readonly List<WeakReference> targetObjects = new List<WeakReference>();

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
            if (manager is null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            manager.RegisterExtension(this);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is target alive.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is target alive; otherwise, <c>false</c>.
        /// </value>
        public bool IsTargetAlive =>
            this.targetObjects.Count == 0 || this.targetObjects.Any(reference => reference.IsAlive);

        /// <summary>
        /// Gets a value indicating whether this instance is in design mode.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is in design mode; otherwise, <c>false</c>.
        /// </value>
        protected bool IsInDesignMode =>
            this.targetObjects
                .Select(reference => reference.Target as DependencyObject)
                .Any(element => element != null && DesignerProperties.GetIsInDesignMode(element));

        /// <summary>
        /// Gets the Target Property the extension is associated with.
        /// </summary>
        /// <remarks>
        /// Can either be a <see cref="DependencyProperty" /> or <see cref="PropertyInfo" />.
        /// </remarks>
        protected object? TargetProperty => this.targetProperty as DependencyProperty;

        /// <summary>
        /// Gets the type of the Target Property.
        /// </summary>
        protected Type? TargetPropertyType
        {
            get
            {
                Type? propertyType = null;
                if (this.targetProperty is DependencyProperty dependencyProperty)
                {
                    propertyType = dependencyProperty.PropertyType;
                }
                else
                {
                    var info = this.targetProperty as PropertyInfo;
                    if (info != null)
                    {
                        propertyType = info.PropertyType;
                    }
                }

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
            if (serviceProvider is null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            var targetHelper = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (targetHelper?.TargetObject is null)
            {
                return null;
            }

            this.targetProperty = targetHelper.TargetProperty;
            if (!(targetHelper.TargetObject is DependencyObject) && this.targetProperty is DependencyProperty)
            {
                return this;
            }

            this.targetObjects.Add(new WeakReference(targetHelper.TargetObject));
            return this.GetValue();
        }

        /// <summary>
        /// Update the associated target.
        /// </summary>
        public void UpdateTarget()
        {
            if (this.targetProperty is null)
            {
                return;
            }

            foreach (WeakReference reference in this.targetObjects)
            {
                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (this.targetProperty is DependencyProperty dependencyProperty)
                {
                    var target = reference.Target as DependencyObject;
                    target?.SetValue(dependencyProperty, this.GetValue());
                }
                else if (this.targetProperty is PropertyInfo info)
                {
                    object? target = reference.Target;
                    if (target != null)
                    {
                        info.SetValue(target, this.GetValue(), null);
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
}