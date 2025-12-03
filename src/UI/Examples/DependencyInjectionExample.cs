using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using StayFit.BLL.Common.Interfaces;
using StayFit.BLL.Features.Auth.Login;
using StayFit.DAL.Context;
using StayFit.UI.DependencyInjection;
using StayFit.UI.Services;

namespace StayFit.UI.Examples;

/// <summary>
/// Приклад конфігурації Dependency Injection для інтеграції UI з BLL та DAL
/// </summary>
public static class DependencyInjectionExample
{
	/// <summary>
	/// Приклад для WPF Application
	/// </summary>
	public static IServiceProvider ConfigureServicesForWpf()
	{
		var services = new ServiceCollection();

		// 1. Конфігурація
		var configuration = new ConfigurationBuilder()
			.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
			.Build();

		var connectionString = configuration.GetConnectionString("DefaultConnection");

		// 2. Реєстрація DbContext
		services.AddDbContext<StayFitDbContext>(options =>
			options.UseNpgsql(connectionString));



		// 3. Реєстрація MediatR з BLL
		services.AddMediatR(cfg =>
		{
			cfg.RegisterServicesFromAssembly(typeof(LoginUserQueryHandler).Assembly);
		});

		// 4. Реєстрація JWT Token Generator (якщо потрібно)
		// services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

		// 5. Реєстрація UI компонентів
		services.AddUI();

		// 6. Ініціалізація ServiceLocator
		var serviceProvider = services.BuildServiceProvider();
		ServiceLocator.Initialize(serviceProvider);

		return serviceProvider;
	}

	/// <summary>
	/// Приклад для .NET MAUI Application (MauiProgram.cs)
	/// ВАЖЛИВО: Цей код потребує посилання на Microsoft.Maui.Controls
	/// Для використання розкоментуйте та додайте using Microsoft.Maui.Controls;
	/// </summary>
	/*
	public static MauiAppBuilder ConfigureServicesForMaui(this MauiAppBuilder builder)
	{
		// 1. Конфігурація
		var configuration = new ConfigurationBuilder()
			.AddJsonStream(GetEmbeddedResourceStream("appsettings.json"))
			.Build();

		var connectionString = configuration.GetConnectionString("DefaultConnection");

		// 2. Реєстрація DbContext
		builder.Services.AddDbContext<StayFitDbContext>(options =>
			options.UseNpgsql(connectionString));

		builder.Services.AddScoped<IApplicationDbContext>(provider =>
			provider.GetRequiredService<StayFitDbContext>());

		// 3. Реєстрація MediatR з BLL
		builder.Services.AddMediatR(cfg =>
		{
			cfg.RegisterServicesFromAssembly(typeof(LoginUserQueryHandler).Assembly);
		});

		// 4. Реєстрація JWT Token Generator (якщо потрібно)
		// builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

		// 5. Реєстрація UI компонентів
		builder.Services.AddUI();

		// 6. Реєстрація Pages (для MAUI)
		builder.Services.AddTransient<LoginPage>();
		builder.Services.AddTransient<DiaryPage>();
		builder.Services.AddTransient<ProgressPage>();
		builder.Services.AddTransient<SettingsPage>();

		return builder;
	}

	private static Stream GetEmbeddedResourceStream(string resourceName)
	{
		var assembly = typeof(DependencyInjectionExample).Assembly;
		return assembly.GetManifestResourceStream(resourceName) 
		       ?? throw new FileNotFoundException($"Resource {resourceName} not found");
	}
	*/
}

// Приклади сторінок (для MAUI) - розкоментуйте при використанні MAUI
/*
public class LoginPage { }
public class DiaryPage { }
public class ProgressPage { }
public class SettingsPage { }
*/

