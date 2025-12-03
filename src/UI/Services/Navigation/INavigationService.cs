namespace StayFit.UI.Services.Navigation;

public interface INavigationService
{
	void NavigateTo<TViewModel>() where TViewModel : class;
	void NavigateTo<TViewModel>(object? parameter) where TViewModel : class;
	void NavigateBack();
	bool CanGoBack { get; }
}

