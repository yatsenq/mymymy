using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using StayFit.UI.ViewModels;
using StayFit.UI.Services;
using MediatR;
using StayFit.DAL.Exceptions;

namespace StayFit.WPF.Views
{
    public partial class LoginView : Window
    {
        private readonly IServiceScope? _scope;
        private readonly LoginViewModel? _viewModel;

        public LoginView()
        {
            InitializeComponent();

            try
            {
                var app = Application.Current as App;
                if (app?.Services != null)
                {
                    _scope = app.Services.CreateScope();
                    _viewModel = _scope.ServiceProvider.GetService<LoginViewModel>();
                    DataContext = _viewModel;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка ініціалізації ViewModel: {ex.Message}",
                               "Помилка",
                               MessageBoxButton.OK,
                               MessageBoxImage.Warning);
            }

            this.Loaded += LoginView_Loaded;
        }

        private void LoginView_Loaded(object sender, RoutedEventArgs e)
        {
            BtnLogin.Click += BtnLogin_Click;
            BtnRegister.Click += BtnRegister_Click;
        }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_viewModel == null)
                {
                    MessageBox.Show("ViewModel не ініціалізовано.", "Помилка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _viewModel.Password = TxtPassword.Password;

                if (string.IsNullOrWhiteSpace(_viewModel.Email))
                {
                    MessageBox.Show("Введіть email", "Помилка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(_viewModel.Password))
                {
                    MessageBox.Show("Введіть пароль", "Помилка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                BtnLogin.IsEnabled = false;
                BtnLogin.Content = "Перевірка...";
                _viewModel.ErrorMessage = null;

                try
                {
                    var query = new StayFit.BLL.Features.Auth.Login.LoginUserQuery(
                        _viewModel.Email,
                        _viewModel.Password
                    );

                    var app = Application.Current as App;

                    // Використовуємо окремий scope, щоб коректно витягнути scoped-сервіси (DbContext, IMediator)
                    IServiceProvider? loginProvider = _scope?.ServiceProvider;
                    using var scope = loginProvider == null ? app?.Services?.CreateScope() : null;
                    loginProvider ??= scope?.ServiceProvider;

                    var mediator = loginProvider?.GetService<IMediator>();

                    if (mediator != null)
                    {
                        var response = await mediator.Send(query);

                        if (response != null)
                        {
                            // ЗБЕРЕГТИ USERID В СЕРВІС
                            var currentUserService = loginProvider?.GetService<ICurrentUserService>();
                            if (currentUserService != null)
                            {
                                currentUserService.UserId = response.userId;
                                currentUserService.Email = _viewModel.Email;
                            }

                            var mainWindow = app.Services.GetService<MainWindow>();
                            mainWindow?.Show();
                            this.Close();
                            return;
                        }

                        _viewModel.ErrorMessage = "Невірний email або пароль";
                    }
                }
                catch (AuthenticationFailedException)
                {
                    _viewModel.ErrorMessage = "Невірний email або пароль";
                }
                catch (Exception)
                {
                    _viewModel.ErrorMessage = "Сталася помилка під час входу. Спробуйте пізніше.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Критична помилка: {ex.Message}", "Помилка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                BtnLogin.IsEnabled = true;
                BtnLogin.Content = "Увійти";
            }
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            var app = Application.Current as App;
            var registerWindow = app?.Services?.GetService<RegisterView>();

            if (registerWindow != null)
            {
                registerWindow.Show();
                this.Close();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _scope?.Dispose();
            base.OnClosed(e);
        }
    }
}
