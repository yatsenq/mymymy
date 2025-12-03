using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using StayFit.UI.ViewModels.Base;

namespace StayFit.UI.Services.Navigation;

public class NavigationService : INavigationService
{
	private readonly IServiceProvider _serviceProvider;
	private readonly Stack<BaseViewModel> _navigationStack = new();

	public NavigationService(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	public void NavigateTo<TViewModel>() where TViewModel : class
	{
		NavigateTo<TViewModel>(null);
	}

	public void NavigateTo<TViewModel>(object? parameter) where TViewModel : class
	{
		var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
		
		if (viewModel is BaseViewModel baseViewModel)
		{
			_navigationStack.Push(baseViewModel);
			CurrentViewModel = baseViewModel;
		}
	}

	public void NavigateBack()
	{
		if (_navigationStack.Count > 1)
		{
			_navigationStack.Pop();
			CurrentViewModel = _navigationStack.Peek();
		}
	}

	public bool CanGoBack => _navigationStack.Count > 1;

	public BaseViewModel? CurrentViewModel { get; private set; }
}

