namespace Atc.Wpf.Components.Tests.Dialogs;

/// <summary>
/// <see cref="InputDialogBoxHelper"/> opens its dialog without an owner when the host has no
/// <see cref="Application"/> or main window, instead of throwing.
/// </summary>
public sealed class InputDialogBoxHelperOwnerTests
{
    [StaFact]
    public void GetOwnerWindow_WithoutAnApplication_ReturnsNull()
    {
        Assert.Null(Application.Current);

        Assert.Null(InputDialogBoxHelper.GetOwnerWindow());
    }
}