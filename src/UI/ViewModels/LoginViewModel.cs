using MediatR;
using StayFit.BLL.Features.Auth.Login;
using StayFit.BLL.Features.Auth.Login.Dtos;
using StayFit.UI.Services.Navigation;
using StayFit.UI.ViewModels.Base;

namespace StayFit.UI.ViewModels;

public class LoginViewModel : BaseViewModel
{
	private readonly IMediator _mediator;
	private readonly INavigationService _navigationService;

	private string _email = string.Empty;
	private string _password = string.Empty;
	private string? _errorMessage;
	private bool _isLoading;

	public LoginViewModel(IMediator mediator, INavigationService navigationService)
	{
		_mediator = mediator;
		_navigationService = navigationService;
		Title = "Вхід";
		LoginCommand = new RelayCommand(async _ => await LoginAsync(), _ => CanLogin);
	}

	public string Email
	{
		get => _email;
		set
		{
			if (SetProperty(ref _email, value))
			{
				LoginCommand.RaiseCanExecuteChanged();
				OnPropertyChanged(nameof(CanLogin));
			}
		}
	}

	public string Password
	{
		get => _password;
		set
		{
			if (SetProperty(ref _password, value))
			{
				LoginCommand.RaiseCanExecuteChanged();
				OnPropertyChanged(nameof(CanLogin));
			}
		}
	}

	public string? ErrorMessage
	{
		get => _errorMessage;
		set => SetProperty(ref _errorMessage, value);
	}

	public bool IsLoading
	{
		get => _isLoading;
		set => SetProperty(ref _isLoading, value);
	}

	public bool CanLogin => !string.IsNullOrWhiteSpace(Email) && 
	                       !string.IsNullOrWhiteSpace(Password) && 
	                       !IsLoading;

	public RelayCommand LoginCommand { get; }

	private async Task LoginAsync()
	{
		if (!CanLogin)
			return;

		IsLoading = true;
		ErrorMessage = null;

		try
		{
			var query = new LoginUserQuery(Email, Password);
			var response = await _mediator.Send(query);

			if (response != null)
			{
				// Збереження токену та userId (можна використати SecureStorage або сервіс автентифікації)
				// Тут можна зберегти токен для подальших запитів
				
				// Навігація на головну сторінку після успішного входу
				// _navigationService.NavigateTo<DiaryViewModel>();
			}
		}
		catch (Exception ex)
		{
			ErrorMessage = $"Помилка входу: {ex.Message}";
		}
		finally
		{
			IsLoading = false;
		}
	}
}

