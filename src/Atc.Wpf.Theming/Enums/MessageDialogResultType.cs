// ReSharper disable CheckNamespace
namespace Atc.Wpf.Theming;

/// <summary>
/// An enum representing the result of a Message Dialog.
/// </summary>
public enum MessageDialogResultType
{
    Canceled = -1,
    Negative = 0,
    Affirmative = 1,
    FirstAuxiliary,
    SecondAuxiliary,
}