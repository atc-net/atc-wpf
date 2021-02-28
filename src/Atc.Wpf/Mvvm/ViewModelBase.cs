using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using Atc.Wpf.Messaging;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Atc.Wpf.Mvvm
{
    /// <summary>
    /// A base class for a the ViewModel class, to be used in the MVVM pattern design.
    /// </summary>
    public abstract class ViewModelBase : ObservableObject, ICleanup
    {
        private bool isEnable;
        private bool isVisible;
        private bool isBusy;
        private bool isDirty;
        private bool isSelected;

        public static Guid ViewModelId => Guid.NewGuid();

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelBase"/> class.
        /// </summary>
        protected ViewModelBase()
            : this(messenger: null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelBase"/> class.
        /// </summary>
        /// <param name="messenger">The messenger.</param>
        protected ViewModelBase(IMessenger? messenger)
        {
            this.MessengerInstance = messenger ?? Messenger.Default;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is enable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is enable; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnable
        {
            get => this.isEnable;
            set
            {
                this.isEnable = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is visible.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsVisible
        {
            get => this.isVisible;
            set
            {
                if (this.isVisible == value)
                {
                    return;
                }

                this.isVisible = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is busy.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is busy; otherwise, <c>false</c>.
        /// </value>
        public bool IsBusy
        {
            get => this.isBusy;
            set
            {
                this.isBusy = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is dirty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is dirty; otherwise, <c>false</c>.
        /// </value>
        public bool IsDirty
        {
            get => this.isDirty;
            set
            {
                this.isDirty = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [is selected].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is selected]; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelected
        {
            get => this.isSelected;
            set
            {
                this.isSelected = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is in design mode.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is in design mode; otherwise, <c>false</c>.
        /// </value>
        public static bool IsInDesignMode => DesignerProperties.GetIsInDesignMode(new DependencyObject());

        /// <summary>
        /// Gets the messenger instance.
        /// </summary>
        /// <value>
        /// The messenger instance.
        /// </value>
        protected IMessenger MessengerInstance { get; init; }

        /// <inheritdoc />
        public virtual void Cleanup()
        {
            this.MessengerInstance.UnRegister(this);
        }

        /// <summary>
        /// Broadcasts the specified old value.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        public void Broadcast<T>(string propertyName, T oldValue, T newValue)
        {
            var message = new PropertyChangedMessage<T>(this, propertyName, oldValue, newValue);
            this.MessengerInstance.Send(message);
        }

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="broadcast">if set to <c>true</c> [broadcast].</param>
        /// <exception cref="ArgumentException">This method cannot be called with an empty string, propertyName</exception>
        [SuppressMessage("Design", "CA1030:Use events where appropriate", Justification = "OK.")]
        public void RaisePropertyChanged<T>(
            string propertyName,
            T oldValue = default,
            T newValue = default,
            bool broadcast = false)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("This method cannot be called with an empty string", propertyName);
            }

            this.RaisePropertyChanged(propertyName);
            if (broadcast)
            {
                this.Broadcast(propertyName, oldValue, newValue);
            }
        }
    }
}