// ReSharper disable CheckNamespace
namespace Atc.Wpf;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class AttachedPropertyAttribute<T>(
    string propertyName) :
    DependencyPropertyAttribute<T>(propertyName);