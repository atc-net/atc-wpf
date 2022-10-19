namespace Atc.Wpf.Theming
{
    /// <summary>
    /// Helps boxing Boolean values.
    /// </summary>
    /// <remarks>
    /// https://xstatic2.wordpress.com/2011/10/21/tip-improving-boolean-dependency-properties-performance/
    /// </remarks>
    public static class BooleanBoxes
    {
        /// <summary>
        /// Gets a boxed representation for <see cref="bool"/> "true" value.
        /// </summary>
        public static readonly object TrueBox = true;

        /// <summary>
        /// Gets a boxed representation for <see cref="bool"/> "false" value.
        /// </summary>
        public static readonly object FalseBox = false;

        /// <summary>
        /// Returns a boxed representation for the specified Boolean value.
        /// </summary>
        /// <param name="value">The value to box.</param>
        /// <returns>A boxed <see cref="bool"/> value.</returns>
        public static object Box(bool value)
            => value
                ? TrueBox
                : FalseBox;

        /// <summary>
        /// Returns a boxed value for the specified nullable <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A boxed nullable <see cref="bool"/> value.</returns>
        public static object? Box(bool? value)
        {
            if (value.HasValue)
            {
                return value.Value
                    ? TrueBox
                    : FalseBox;
            }

            return null;
        }
    }
}