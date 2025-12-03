using System;
using System.Text.RegularExpressions;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using StayFit.BLL.Features.Auth.Register;

namespace StayFit.WPF.Views
{
    public partial class RegisterView : Window
    {
        private readonly IMediator? _mediator;

        public RegisterView()
        {
            InitializeComponent();
            
            try
            {
                var app = Application.Current as App;
                _mediator = app?.Services?.GetService<IMediator>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка ініціалізації: {ex.Message}", 
                               "Помилка", 
                               MessageBoxButton.OK, 
                               MessageBoxImage.Warning);
            }
        }

        private async void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Очистити помилки
                TxtError.Text = "";

                // Отримати дані
                string firstName = TxtFirstName.Text.Trim();
                string lastName = TxtLastName.Text.Trim();
                string email = TxtEmail.Text.Trim();
                string password = TxtPassword.Password;
                string confirmPassword = TxtConfirmPassword.Password;

                // ========== ВАЛІДАЦІЯ ==========

                // Ім'я
                if (string.IsNullOrWhiteSpace(firstName))
                {
                    TxtError.Text = "Введіть ім'я";
                    TxtFirstName.Focus();
                    return;
                }

                if (firstName.Length < 2)
                {
                    TxtError.Text = "Ім'я має містити мінімум 2 символи";
                    TxtFirstName.Focus();
                    return;
                }

                if (!Regex.IsMatch(firstName, @"^[А-ЯҐа-яґІіЇїЄєҐґA-Za-z '\-]+$"))
                {
                    TxtError.Text = "Ім'я містить недопустимі символи";
                    TxtFirstName.Focus();
                    return;
                }

                // Прізвище
                if (string.IsNullOrWhiteSpace(lastName))
                {
                    TxtError.Text = "Введіть прізвище";
                    TxtLastName.Focus();
                    return;
                }

                if (lastName.Length < 2)
                {
                    TxtError.Text = "Прізвище має містити мінімум 2 символи";
                    TxtLastName.Focus();
                    return;
                }

                if (!Regex.IsMatch(lastName, @"^[А-ЯҐа-яґІіЇїЄєҐґA-Za-z '\-]+$"))
                {
                    TxtError.Text = "Прізвище містить недопустимі символи";
                    TxtLastName.Focus();
                    return;
                }

                // Email
                if (string.IsNullOrWhiteSpace(email))
                {
                    TxtError.Text = "Введіть email";
                    TxtEmail.Focus();
                    return;
                }

                if (!IsValidEmail(email))
                {
                    TxtError.Text = "Введіть коректний email (example@domain.com)";
                    TxtEmail.Focus();
                    return;
                }

                // Пароль
                if (string.IsNullOrWhiteSpace(password))
                {
                    TxtError.Text = "Введіть пароль";
                    TxtPassword.Focus();
                    return;
                }

                if (password.Length < 6)
                {
                    TxtError.Text = "Пароль має містити мінімум 6 символів";
                    TxtPassword.Focus();
                    return;
                }

                if (password != confirmPassword)
                {
                    TxtError.Text = "Паролі не співпадають";
                    TxtConfirmPassword.Focus();
                    return;
                }

                // Дата народження
                if (!DateBirth.SelectedDate.HasValue)
                {
                    TxtError.Text = "Оберіть дату народження";
                    DateBirth.Focus();
                    return;
                }

                DateTime birthDate = DateBirth.SelectedDate.Value;
                int age = CalculateAge(birthDate);

                if (age < 13)
                {
                    TxtError.Text = "Реєстрація доступна тільки з 13 років";
                    DateBirth.Focus();
                    return;
                }

                if (age > 120)
                {
                    TxtError.Text = "Некоректна дата народження";
                    DateBirth.Focus();
                    return;
                }

                // Стать
                string gender = RbMale.IsChecked == true ? "MALE" : "FEMALE";

                // ========== РЕЄСТРАЦІЯ ЧЕРЕЗ MEDIATR ==========

                if (_mediator == null)
                {
                    TxtError.Text = "Помилка системи. Перезапустіть додаток.";
                    return;
                }

                // Показати завантаження
                BtnRegister.IsEnabled = false;
                BtnRegister.Content = "Реєстрація...";

                try
                {
                    // Створити команду
                    var command = new RegisterUserCommand(
                        firstName,
                        lastName,
                        email,
                        password,
                        birthDate,
                        gender,
                        170.0f,
                        70.0f
                    );

                    // Виконати команду
                    var userId = await _mediator.Send(command);

                    // ✅ Успіх
                    MessageBox.Show($"✅ Реєстрація успішна!\nТепер ви можете увійти в систему.", 
                                   "Успіх", 
                                   MessageBoxButton.OK, 
                                   MessageBoxImage.Information);

                    // Відкрити вікно входу
                    var app = Application.Current as App;
                    var loginWindow = app?.Services?.GetService<LoginView>() ?? new LoginView();
                    loginWindow.Show();
                    this.Close();
                }
                catch (Exception ex)
                {
                    // ❌ Помилка
                    string errorMessage = ex.Message;
                    
                    if (errorMessage.Contains("already exists"))
                    {
                        TxtError.Text = "Користувач з таким email вже існує";
                    }
                    else if (errorMessage.Contains("email"))
                    {
                        TxtError.Text = "Некоректний email";
                    }
                    else
                    {
                        TxtError.Text = $"Помилка реєстрації: {errorMessage}";
                    }

                    BtnRegister.IsEnabled = true;
                    BtnRegister.Content = "ЗАРЕЄСТРУВАТИСЯ";
                }
            }
            catch (Exception ex)
            {
                TxtError.Text = $"Критична помилка: {ex.Message}";
                BtnRegister.IsEnabled = true;
                BtnRegister.Content = "ЗАРЕЄСТРУВАТИСЯ";
            }
        }

        private void BtnBackToLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var app = Application.Current as App;
                var loginWindow = app?.Services?.GetService<LoginView>() ?? new LoginView();
                loginWindow.Show();
                this.Close();
            }
            catch
            {
                var loginWindow = new LoginView();
                loginWindow.Show();
                this.Close();
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        private int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}