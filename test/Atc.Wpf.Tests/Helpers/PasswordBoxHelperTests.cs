namespace Atc.Wpf.Tests.Helpers;

public sealed class PasswordBoxHelperTests
{
    [StaFact]
    public void PasswordChanged_KeepsOneWayBoundPasswordBinding()
    {
        // Arrange
        var source = new TextBlock { Text = "initial" };
        var passwordBox = new PasswordBox();
        BindingOperations.SetBinding(
            passwordBox,
            PasswordBoxHelper.BoundPasswordProperty,
            new Binding(nameof(TextBlock.Text)) { Source = source, Mode = BindingMode.OneWay });

        Assert.Equal("initial", passwordBox.Password);

        // Act
        passwordBox.Password = "t";
        passwordBox.Password = "ty";
        source.Text = string.Empty;

        // Assert
        Assert.NotNull(BindingOperations.GetBindingExpression(passwordBox, PasswordBoxHelper.BoundPasswordProperty));
        Assert.Equal(string.Empty, passwordBox.Password);
    }

    [StaFact]
    public void PasswordChanged_PushesTypedPasswordToTwoWaySource()
    {
        // Arrange
        var source = new TextBlock { Text = "initial" };
        var passwordBox = new PasswordBox();
        BindingOperations.SetBinding(
            passwordBox,
            PasswordBoxHelper.BoundPasswordProperty,
            new Binding(nameof(TextBlock.Text)) { Source = source, Mode = BindingMode.TwoWay });

        // Act
        passwordBox.Password = "typed";

        // Assert
        Assert.Equal("typed", source.Text);
        Assert.NotNull(BindingOperations.GetBindingExpression(passwordBox, PasswordBoxHelper.BoundPasswordProperty));
    }

    [StaFact]
    public void BoundPassword_InitialValueEqualToDefault_StillPushesTypedPasswordToSource()
    {
        // Arrange
        var source = new TextBlock { Text = string.Empty };
        var passwordBox = new PasswordBox();
        BindingOperations.SetBinding(
            passwordBox,
            PasswordBoxHelper.BoundPasswordProperty,
            new Binding(nameof(TextBlock.Text)) { Source = source, Mode = BindingMode.TwoWay });

        // Act
        passwordBox.Password = "typed";

        // Assert
        Assert.Equal("typed", source.Text);
        Assert.Equal("typed", PasswordBoxHelper.GetBoundPassword(passwordBox));
    }

    [StaFact]
    public void BoundPassword_InitialValueEqualToDefault_StillUpdatesBoundPasswordOnTyping()
    {
        // Arrange
        var passwordBox = new PasswordBox();
        PasswordBoxHelper.SetBoundPassword(passwordBox, string.Empty);

        // Act
        passwordBox.Password = "typed";

        // Assert
        Assert.Equal("typed", PasswordBoxHelper.GetBoundPassword(passwordBox));
    }
}