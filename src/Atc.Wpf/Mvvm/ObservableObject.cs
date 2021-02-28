using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Atc.Wpf.Mvvm
{
    /// <summary>
    /// A base class for objects of which the properties must be observable.
    /// </summary>
    /// <seealso cref="INotifyPropertyChanged" />
    [SuppressMessage("Design", "CA1030:Use events where appropriate", Justification = "OK.")]
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.VerifyPropertyName(propertyName);
            var handler = this.PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        public void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            var handler = this.PropertyChanged;
            if (handler is null)
            {
                return;
            }

            var propertyName = GetPropertyName(propertyExpression);
            if (!string.IsNullOrEmpty(propertyName))
            {
                this.RaisePropertyChanged(propertyName);
            }
        }

        /// <summary>
        /// Verifies the name of the property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <exception cref="ArgumentException">Property not found, propertyName</exception>
        public void VerifyPropertyName(string? propertyName)
        {
            var info = this.GetType().GetTypeInfo();
            if (string.IsNullOrEmpty(propertyName) ||
                info.GetDeclaredProperty(propertyName) is not null)
            {
                return;
            }

            // Check base types
            var found = false;
            while (info.BaseType is not null && info.BaseType != typeof(object))
            {
                info = info.BaseType.GetTypeInfo();
                if (info.GetDeclaredProperty(propertyName) is null)
                {
                    continue;
                }

                found = true;
                break;
            }

            if (!found)
            {
                throw new ArgumentException("Property not found", propertyName);
            }
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        /// <returns>The name of the property.</returns>
        protected static string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression is null)
            {
                throw new ArgumentNullException(nameof(propertyExpression));
            }

            if (!(propertyExpression.Body is MemberExpression body))
            {
                throw new ArgumentException("Invalid argument", nameof(propertyExpression));
            }

            if (!(body.Member is PropertyInfo property))
            {
                throw new ArgumentException("Argument is not a property", nameof(propertyExpression));
            }

            return property.Name;
        }

        /// <summary>
        /// Called when property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        [SuppressMessage("Major Code Smell", "S4144:Methods should not have identical implementations", Justification = "OK.")]
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.VerifyPropertyName(propertyName);
            var handler = this.PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool Set<T>(
            Expression<Func<T>> propertyExpression,
            ref T field,
            T newValue)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }

            field = newValue;
            this.RaisePropertyChanged(propertyExpression);
            return true;
        }

        protected bool Set<T>(
            string? propertyName,
            ref T field,
            T newValue)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }

            field = newValue;
            this.RaisePropertyChanged(propertyName);
            return true;
        }

        protected bool Set<T>(
            ref T field,
            T newValue,
            [CallerMemberName] string? propertyName = null)
        {
            return this.Set(propertyName, ref field, newValue);
        }
    }
}