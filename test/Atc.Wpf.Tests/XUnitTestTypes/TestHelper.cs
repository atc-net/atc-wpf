namespace Atc.Wpf.Tests.XUnitTestTypes;

public static class TestHelper
{
    public static bool HandlePropertyChangedEventArgs(
        PropertyChangedEventArgs eventArgs,
        bool expectedAsEmpty,
        string propertyName)
    {
        ArgumentNullException.ThrowIfNull(eventArgs);

        if (expectedAsEmpty)
        {
            if (string.IsNullOrEmpty(eventArgs.PropertyName))
            {
                return true;
            }
        }
        else
        {
            ArgumentNullException.ThrowIfNull(propertyName);

            if (propertyName.Equals(eventArgs.PropertyName, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }
}