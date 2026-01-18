namespace Atc.Wpf.Navigation;

/// <summary>
/// Represents a collection of parameters passed during navigation.
/// </summary>
public sealed class NavigationParameters : Dictionary<string, object?>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationParameters"/> class.
    /// </summary>
    public NavigationParameters()
        : base(StringComparer.Ordinal)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationParameters"/> class
    /// with the specified parameters.
    /// </summary>
    /// <param name="parameters">Initial parameters.</param>
    public NavigationParameters(IDictionary<string, object?> parameters)
        : base(parameters, StringComparer.Ordinal)
    {
    }

    /// <summary>
    /// Gets a parameter value by key, returning default if not found.
    /// </summary>
    /// <typeparam name="T">The expected type of the parameter.</typeparam>
    /// <param name="key">The parameter key.</param>
    /// <returns>The parameter value or default.</returns>
    public T? GetValue<T>(string key)
    {
        if (TryGetValue(key, out var value) && value is T typedValue)
        {
            return typedValue;
        }

        return default;
    }

    /// <summary>
    /// Gets a parameter value by key, returning the specified default if not found.
    /// </summary>
    /// <typeparam name="T">The expected type of the parameter.</typeparam>
    /// <param name="key">The parameter key.</param>
    /// <param name="defaultValue">The default value to return if not found.</param>
    /// <returns>The parameter value or the specified default.</returns>
    public T GetValueOrDefault<T>(
        string key,
        T defaultValue)
    {
        if (TryGetValue(key, out var value) && value is T typedValue)
        {
            return typedValue;
        }

        return defaultValue;
    }

    /// <summary>
    /// Adds a parameter with the specified key and value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The parameter key.</param>
    /// <param name="value">The parameter value.</param>
    /// <returns>This instance for fluent chaining.</returns>
    public NavigationParameters WithParameter<T>(
        string key,
        T value)
    {
        this[key] = value;
        return this;
    }
}