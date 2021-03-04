// ReSharper disable once CheckNamespace
namespace System.Windows
{
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
    }
}