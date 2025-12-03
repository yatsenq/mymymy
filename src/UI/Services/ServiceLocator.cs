using Microsoft.Extensions.DependencyInjection;
using StayFit.UI.Services.Navigation;
using StayFit.UI.ViewModels;

namespace StayFit.UI.Services;

public static class ServiceLocator
{
	private static IServiceProvider? _serviceProvider;

	public static void Initialize(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	public static T GetService<T>() where T : class
	{
		if (_serviceProvider == null)
			throw new InvalidOperationException("ServiceProvider не ініціалізовано. Викличте Initialize() спочатку.");

		return _serviceProvider.GetRequiredService<T>();
	}
}

