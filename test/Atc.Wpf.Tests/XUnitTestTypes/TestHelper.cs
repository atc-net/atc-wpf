using System;
using System.ComponentModel;

namespace Atc.Wpf.Tests.XUnitTestTypes
{
    public static class TestHelper
    {
        public static bool HandlePropertyChangedEventArgs(
            PropertyChangedEventArgs eventArgs,
            bool expectedAsEmpty,
            string propertyName)
        {
            if (eventArgs == null)
            {
                throw new ArgumentNullException(nameof(eventArgs));
            }

            if (expectedAsEmpty)
            {
                if (string.IsNullOrEmpty(eventArgs.PropertyName))
                {
                    return true;
                }
            }
            else
            {
                if (propertyName == null)
                {
                    throw new ArgumentNullException(nameof(propertyName));
                }

                if (propertyName.Equals(eventArgs.PropertyName, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }
    }
}