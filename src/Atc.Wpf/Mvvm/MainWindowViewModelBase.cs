using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Atc.Wpf.Command;

namespace Atc.Wpf.Mvvm
{
    public class MainWindowViewModelBase : ViewModelBase, IMainWindowViewModel
    {
        private WindowState windowState;

        /// <summary>
        /// Gets or sets the state of the window.
        /// </summary>
        /// <value>
        /// The state of the window.
        /// </value>
        public WindowState WindowState
        {
            get => this.windowState;
            set
            {
                this.windowState = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets the application exit command.
        /// </summary>
        /// <value>
        /// The application exit command.
        /// </value>
        public ICommand ApplicationExitCommand => new RelayCommand(ApplicationExitCommandHandler);

        /// <inheritdoc />
        public void OnLoaded(object sender, RoutedEventArgs args)
        {
            // Method intentionally left empty.
        }

        /// <inheritdoc />
        public void OnClosing(object sender, CancelEventArgs args)
        {
            Application.Current.Shutdown(-1);
        }

        /// <inheritdoc />
        public void OnKeyDown(object sender, KeyEventArgs args)
        {
            if (args is null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (args.Key == Key.F11)
            {
                this.WindowState = this.WindowState == WindowState.Maximized
                    ? WindowState.Normal
                    : WindowState.Maximized;
                args.Handled = true;
            }
        }

        /// <inheritdoc />
        public void OnKeyUp(object sender, KeyEventArgs args)
        {
            // Method intentionally left empty.
        }

        private void ApplicationExitCommandHandler()
        {
            this.OnClosing(this, new CancelEventArgs());
        }
    }
}