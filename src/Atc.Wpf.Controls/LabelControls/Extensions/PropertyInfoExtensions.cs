// ReSharper disable CheckNamespace
namespace System.Reflection;

public static class PropertyInfoExtensions
{
    // TODO: Atc - Extensions
    public static bool IsNullable(
        this PropertyInfo propertyInfo)
    {
        ArgumentNullException.ThrowIfNull(propertyInfo);

        return Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null;
    }
}