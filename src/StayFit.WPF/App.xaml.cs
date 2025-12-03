using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using MediatR;
using StayFit.DAL.Context;
using StayFit.DAL.Interfaces;
using StayFit.BLL.Common.Interfaces;
using StayFit.BLL.Services;
using StayFit.UI.DependencyInjection;
using StayFit.WPF.Views;
using StayFit.BLL.Features.Auth.Login;

namespace StayFit.WPF
{
    public partial class App : Application
    {
        private IHost? _host;
        
        public IServiceProvider? Services => _host?.Services;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Глобальний хендлінг необроблених винятків, щоб бачити причину крашу
            this.DispatcherUnhandledException += (sender, args) =>
            {
                MessageBox.Show(
                    $"DispatcherUnhandledException:\n{args.Exception.Message}\n\n{args.Exception.StackTrace}",
                    "Помилка UI-потоку",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                // Не позначаємо як оброблений, щоб у разі критичних помилок процес все одно завершився
            };

            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                if (args.ExceptionObject is Exception ex)
                {
                    MessageBox.Show(
                        $"UnhandledException:\n{ex.Message}\n\n{ex.StackTrace}",
                        "Критична помилка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            };
            
            // FIX: Npgsql DateTime UTC
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            try
            {
                _host = Host.CreateDefaultBuilder()
                    .ConfigureAppConfiguration((context, config) =>
                    {
                        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    })
                    .ConfigureServices((context, services) =>
                    {
                        var connectionString = context.Configuration.GetConnectionString("DefaultConnection");
                        services.AddDbContext<StayFitDbContext>(options =>
                        options.UseNpgsql(connectionString)
                            .UseSnakeCaseNamingConvention());
                        
                        services.AddScoped<IApplicationDbContext>(provider =>
                            provider.GetRequiredService<StayFitDbContext>());
                        
                        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
                        
                        services.AddMediatR(cfg =>
                            cfg.RegisterServicesFromAssembly(typeof(LoginUserQueryHandler).Assembly));
                        
                        services.AddUI();
                        
                        services.AddTransient<MainWindow>();
                        services.AddTransient<LoginView>();
                        services.AddTransient<RegisterView>();
                        services.AddTransient<DiaryView>();
                        services.AddTransient<ProgressView>();
                        services.AddTransient<SettingsView>();
                    })
                    .Build();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка ініціалізації: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            var loginView = _host?.Services.GetRequiredService<LoginView>();
            loginView?.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _host?.Dispose();
            base.OnExit(e);
        }
    }
}
