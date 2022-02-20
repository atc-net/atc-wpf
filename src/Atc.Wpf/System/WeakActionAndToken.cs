// ReSharper disable InconsistentNaming
// ReSharper disable once CheckNamespace
namespace System;

[SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "OK.")]
[SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "OK.")]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "OK.")]
[SuppressMessage("Minor Code Smell", "S1104:Fields should not have public accessibility", Justification = "OK.")]
public struct WeakActionAndToken
{
    /// <summary>
    /// The action.
    /// </summary>
    public WeakAction? Action;

    /// <summary>
    /// The token.
    /// </summary>
    public object? Token;
}