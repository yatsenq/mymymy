using Microsoft.Extensions.DependencyInjection;
using StayFit.UI.Services;
using StayFit.UI.Services.Navigation;
using StayFit.UI.ViewModels;

namespace StayFit.UI.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddViewModels(this IServiceCollection services)
    {
        services.AddSingleton<LoginViewModel>();
        services.AddSingleton<DiaryViewModel>();
        services.AddSingleton<ProgressViewModel>();
        services.AddSingleton<SettingsViewModel>();

        return services;
    }

    public static IServiceCollection AddNavigationServices(this IServiceCollection services)
    {
        services.AddSingleton<INavigationService, NavigationService>();

        return services;
    }

    public static IServiceCollection AddUI(this IServiceCollection services)
    {
        services.AddSingleton<ICurrentUserService, CurrentUserService>();
        services.AddViewModels();
        services.AddNavigationServices();

        return services;
    }
}
