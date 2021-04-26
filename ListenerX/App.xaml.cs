using ListenerX.ChromaExtension;
using ListenerX.Classes;
using ListenerX.Helpers;
using ListenerX.Visualization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using VirtualGrid.Interfaces;

namespace ListenerX
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public readonly IServiceProvider ServiceProvider;
        public App()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();

            var settings = Settings.LoadSettings();

            services.AddSingleton<ModuleActivator>();
            services.AddSingleton<IVirtualLedGrid>(_ => VirtualGrid.VirtualLedGrid.CreateDefaultGrid());
            services.AddSingleton<ISettings>(_ => settings);
            services.AddSingleton<RealTimePlayback>();
            services.AddSingleton<ChromaWorker>();
            services.AddSingleton<MainWindow>();

            services.AddTransient<SettingsPage>();
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}
