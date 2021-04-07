using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Atc.Wpf.Mvvm
{
    [SuppressMessage("Design", "CA1030:Use events where appropriate", Justification = "OK.")]
    public interface IObservableObject : INotifyPropertyChanged
    {
        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        void RaisePropertyChanged([CallerMemberName] string? propertyName = null);

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression);

        /// <summary>
        /// Verifies the name of the property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <exception cref="ArgumentException">Property not found, propertyName</exception>
        void VerifyPropertyName(string? propertyName);
    }
}