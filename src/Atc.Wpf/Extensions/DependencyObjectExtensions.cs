using System.Windows.Media;

// ReSharper disable ConstantConditionalAccessQualifier
// ReSharper disable ConvertIfStatementToSwitchStatement
// ReSharper disable InvertIf
// ReSharper disable once CheckNamespace
namespace System.Windows
{
    /// <summary>
    /// Extension methods for DependencyObject.
    /// </summary>
    public static class DependencyObjectExtensions
    {
        /// <summary>
        /// Sets the value of the <paramref name="property"/> only if it hasn't been explicitly set.
        /// </summary>
        /// <typeparam name="T">The generic type.</typeparam>
        /// <param name="obj">The dependency-object.</param>
        /// <param name="property">The dependency-property.</param>
        /// <param name="value">The value.</param>
        /// <returns>Return true if the property is set, otherwise false.</returns>
        public static bool SetIfDefault<T>(this DependencyObject obj, DependencyProperty property, T value)
        {
            if (DependencyPropertyHelper.GetValueSource(obj, property).BaseValueSource is not BaseValueSource.Default)
            {
                return false;
            }

            obj?.SetValue(property, value);
            return true;
        }

        /// <summary>
        /// Finds a parent of a given item on the visual tree.
        /// </summary>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="child">A direct or indirect child of the
        /// queried item.</param>
        /// <returns>The first parent item that matches the submitted
        /// type parameter. If not matching item can be found, a null
        /// reference is being returned.</returns>
        public static T? TryFindParent<T>(this DependencyObject child)
            where T : DependencyObject
        {
            while (true)
            {
                // Get parent item
                var parentObject = child.GetParentObject();

                // We've reached the end of the tree
                if (parentObject is null)
                {
                    return null;
                }

                // Check if the parent matches the type we're looking for
                if (parentObject is T parent)
                {
                    return parent;
                }

                child = parentObject;
            }
        }

        /// <summary>
        /// This method is an alternative to WPF's
        /// <see cref="VisualTreeHelper.GetParent"/> method, which also
        /// supports content elements. Keep in mind that for content element,
        /// this method falls back to the logical tree of the element.
        /// </summary>
        /// <param name="child">The item to be processed.</param>
        /// <returns>The submitted item's parent, if available. Otherwise null.</returns>
        public static DependencyObject? GetParentObject(this DependencyObject? child)
        {
            if (child is null)
            {
                return null;
            }

            // Handle content elements separately
            if (child is ContentElement contentElement)
            {
                DependencyObject parent = ContentOperations.GetParent(contentElement);
                if (parent is not null)
                {
                    return parent;
                }

                return contentElement is FrameworkContentElement fce
                    ? fce.Parent
                    : null;
            }

            var childParent = VisualTreeHelper.GetParent(child);
            if (childParent is not null)
            {
                return childParent;
            }

            // Also try searching for parent in framework elements (such as DockPanel, etc)
            if (child is FrameworkElement frameworkElement)
            {
                var parent = frameworkElement.Parent;
                if (parent is not null)
                {
                    return parent;
                }
            }

            return null;
        }
    }
}