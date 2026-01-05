namespace Atc.Wpf.Controls.Viewers.JsonTree;

/// <summary>
/// Specifies the type of a JSON node.
/// </summary>
public enum JsonNodeType
{
    /// <summary>
    /// An object node containing properties.
    /// </summary>
    Object,

    /// <summary>
    /// An array node containing elements.
    /// </summary>
    Array,

    /// <summary>
    /// A property node with a name and value.
    /// </summary>
    Property,

    /// <summary>
    /// A string value.
    /// </summary>
    String,

    /// <summary>
    /// An integer value.
    /// </summary>
    Integer,

    /// <summary>
    /// A floating-point value.
    /// </summary>
    Float,

    /// <summary>
    /// A boolean value.
    /// </summary>
    Boolean,

    /// <summary>
    /// A null value.
    /// </summary>
    Null,

    /// <summary>
    /// A date/time value.
    /// </summary>
    Date,

    /// <summary>
    /// A GUID value.
    /// </summary>
    Guid,

    /// <summary>
    /// A URI value.
    /// </summary>
    Uri,
}