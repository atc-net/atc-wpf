using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Threading;
using Atc.Wpf.Diagnostics;
using Atc.Wpf.Mvvm;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Atc.Wpf.Sample
{
    /// <summary>
    /// Interaction logic for App.
    /// </summary>
    public partial class App
    {
        private ServiceProvider? serviceProvider;

        /// <summary>
        /// Raises the Startup event.
        /// </summary>
        /// <param name="e">The <see cref="StartupEventArgs"/> instance containing the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            if (e is null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            // Hook on error before app really starts
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
            Current.DispatcherUnhandledException += this.ApplicationOnDispatcherUnhandledException;
            base.OnStartup(e);

            if (Debugger.IsAttached)
            {
                BindingErrorTraceListener.StartTrace();
            }
        }

        // ReSharper disable once UnusedParameter.Local
        [SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "OK.")]
        private static ServiceCollection CreateServiceCollectionAndRegister()
        {
            var services = new ServiceCollection();
            _ = services.AddLogging(logging =>
            {
                _ = logging.AddDebug().SetMinimumLevel(LogLevel.Trace);
            });

            // Singleton's
            _ = services.AddSingleton<MainWindow>();
            _ = services.AddSingleton<IMainWindowViewModel, MainWindowViewModel>();
            return services;
        }

        /// <summary>
        /// Currents the domain unhandled exception.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="UnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            if (!(args is { ExceptionObject: Exception ex }))
            {
                return;
            }

            string exceptionMessage = ex.GetMessage(true);
            _ = MessageBox.Show(
                    exceptionMessage,
                    "CurrentDomain Unhandled Exception",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
        }

        /// <summary>
        /// Applications the on dispatcher unhandled exception.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="DispatcherUnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private void ApplicationOnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            string exceptionMessage = args.Exception.GetMessage(true);
            if (exceptionMessage.IndexOf(
                "BindingExpression:Path=HorizontalContentAlignment; DataItem=null; target element is 'ComboBoxItem'",
                StringComparison.Ordinal) != -1)
            {
                args.Handled = true;
                return;
            }

            _ = MessageBox.Show(
                    exceptionMessage,
                    "Dispatcher Unhandled Exception",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

            args.Handled = true;
            this.Shutdown(-1);
        }

        private void OnStartupInitialize(object sender, StartupEventArgs startupEventArgs)
        {
            var services = CreateServiceCollectionAndRegister();
            this.serviceProvider = services.BuildServiceProvider();
            var mainWindow = serviceProvider.GetService<MainWindow>()!;
            mainWindow.Show();
        }
    }
}