using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using Atc.Wpf.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// ReSharper disable once UnusedParameter.Local
namespace Atc.Wpf.Sample
{
    /// <summary>
    /// Interaction logic for App.
    /// </summary>
    public partial class App
    {
        private readonly IHost host;

        public App()
        {
            this.host = Host.CreateDefaultBuilder()
                .ConfigureLogging(logging =>
                {
                    logging
                        .AddDebug()
                        .SetMinimumLevel(LogLevel.Trace);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<IMainWindowViewModel, MainWindowViewModel>();
                    services.AddSingleton<MainWindow>();
                })
                .Build();
        }

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

            string exceptionMessage = ex.GetMessage(includeInnerMessage: true);
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
            string exceptionMessage = args.Exception.GetMessage(includeInnerMessage: true);
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

        private async void ApplicationStartup(object sender, StartupEventArgs args)
        {
            await this.host
                .StartAsync()
                .ConfigureAwait(false);

            var mainWindow = this.host
                .Services
                .GetService<MainWindow>()!;

            mainWindow.Show();
        }

        private async void ApplicationExit(object sender, ExitEventArgs args)
        {
            await this.host
                .StopAsync()
                .ConfigureAwait(false);

            this.host.Dispose();
        }
    }
}