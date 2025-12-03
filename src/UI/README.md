# StayFit UI - MVVM Architecture

Цей проект містить реалізацію MVVM-архітектури для додатку StayFit.

## Структура проекту

```
UI/
├── ViewModels/
│   ├── Base/
│   │   ├── BaseViewModel.cs      # Базовий клас для всіх ViewModels
│   │   └── RelayCommand.cs      # Реалізація ICommand для команд
│   ├── LoginViewModel.cs        # ViewModel для входу
│   ├── DiaryViewModel.cs        # ViewModel для щоденника харчування
│   ├── ProgressViewModel.cs     # ViewModel для прогресу
│   └── SettingsViewModel.cs     # ViewModel для налаштувань
├── Services/
│   ├── Navigation/
│   │   ├── INavigationService.cs
│   │   └── NavigationService.cs
│   └── ServiceLocator.cs
└── DependencyInjection/
    └── ServiceCollectionExtensions.cs
```

## Базові класи

### BaseViewModel

Базовий клас для всіх ViewModels, який реалізує `INotifyPropertyChanged` для двостороннього зв'язування даних.

**Властивості:**

- `IsBusy` - індикатор завантаження
- `Title` - заголовок сторінки

**Методи:**

- `OnPropertyChanged()` - викликається при зміні властивості
- `SetProperty<T>()` - встановлює значення властивості та викликає `OnPropertyChanged`

### RelayCommand

Реалізація `ICommand` для виконання команд у ViewModels.

**Використання:**

```csharp
public RelayCommand LoginCommand { get; }

LoginCommand = new RelayCommand(async _ => await LoginAsync(), _ => CanLogin);
```

### NavigationService

Сервіс для навігації між сторінками.

**Використання:**

```csharp
_navigationService.NavigateTo<DiaryViewModel>();
_navigationService.NavigateBack();
```

## ViewModels

### LoginViewModel

Управляє процесом входу користувача.

**Властивості:**

- `Email` - email користувача
- `Password` - пароль користувача
- `ErrorMessage` - повідомлення про помилку
- `IsLoading` - індикатор завантаження
- `CanLogin` - чи можна виконати вхід

**Команди:**

- `LoginCommand` - команда для входу

### DiaryViewModel

Управляє щоденником харчування.

**Властивості:**

- `SelectedDate` - обрана дата
- `SelectedProductId` - обраний продукт
- `SelectedMealTypeId` - обраний тип прийому їжі
- `WeightGrams` - вага в грамах
- `DailySummary` - підсумок за день
- `TotalCalories`, `TotalProtein`, `TotalFat`, `TotalCarbs` - підсумки макронутрієнтів

**Команди:**

- `LoadDailySummaryCommand` - завантажити підсумок за день
- `AddEntryCommand` - додати запис до щоденника

### ProgressViewModel

Управляє переглядом прогресу користувача.

**Властивості:**

- `SelectedStartDate` - початкова дата періоду
- `SelectedEndDate` - кінцева дата періоду
- `IsLoading` - індикатор завантаження

**Команди:**

- `LoadProgressCommand` - завантажити дані про прогрес

### SettingsViewModel

Управляє налаштуваннями користувача.

**Властивості:**

- `DailyCalorieGoal` - щоденна норма калорій
- `ProteinGoal` - норма білків
- `FatGoal` - норма жирів
- `CarbsGoal` - норма вуглеводів
- `IsLoading` - індикатор завантаження

**Команди:**

- `SaveSettingsCommand` - зберегти налаштування
- `LoadSettingsCommand` - завантажити налаштування

## Інтеграція з BLL через MediatR

Всі ViewModels використовують `IMediator` для відправки запитів та команд до BLL:

```csharp
var query = new LoginUserQuery(Email, Password);
var response = await _mediator.Send(query);
```

## Налаштування Dependency Injection

### Приклад конфігурації для WPF:

```csharp
// App.xaml.cs або Program.cs
var services = new ServiceCollection();

// Реєстрація BLL та DAL
services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(LoginUserQueryHandler).Assembly));
// ... інші реєстрації BLL/DAL

// Реєстрація UI
services.AddUI();

var serviceProvider = services.BuildServiceProvider();
ServiceLocator.Initialize(serviceProvider);
```

### Приклад конфігурації для .NET MAUI:

```csharp
// MauiProgram.cs
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // Реєстрація BLL та DAL
        builder.Services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(LoginUserQueryHandler).Assembly));
        // ... інші реєстрації BLL/DAL

        // Реєстрація UI
        builder.Services.AddUI();

        return builder.Build();
    }
}
```

## Використання в View

### Приклад для WPF:

```xml
<Window x:Class="StayFit.Views.LoginView"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Вхід">
    <Grid>
        <TextBox Text="{Binding Email, UpdateSourceTrigger=PropertyChanged}" />
        <PasswordBox x:Name="PasswordBox" />
        <Button Command="{Binding LoginCommand}" Content="Вхід" />
        <TextBlock Text="{Binding ErrorMessage}" Foreground="Red" />
    </Grid>
</Window>
```

```csharp
// LoginView.xaml.cs
public partial class LoginView : Window
{
    public LoginView()
    {
        InitializeComponent();
        DataContext = ServiceLocator.GetService<LoginViewModel>();
    }
}
```

### Приклад для .NET MAUI:

```xml
<!-- LoginPage.xaml -->
<ContentPage x:Class="StayFit.Views.LoginPage"
             xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">
    <VerticalStackLayout>
        <Entry Text="{Binding Email}" />
        <Entry Text="{Binding Password}" IsPassword="True" />
        <Button Text="Вхід" Command="{Binding LoginCommand}" />
        <Label Text="{Binding ErrorMessage}" TextColor="Red" />
    </VerticalStackLayout>
</ContentPage>
```

```csharp
// LoginPage.xaml.cs
public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
```

## Двостороннє зв'язування (Binding)

Всі властивості ViewModels підтримують двостороннє зв'язування через `INotifyPropertyChanged`:

```xml
<TextBox Text="{Binding Email, UpdateSourceTrigger=PropertyChanged}" />
```

При зміні значення в TextBox автоматично оновлюється властивість `Email` в ViewModel, і навпаки.

## Командна система

Всі дії користувача (натискання кнопок, вибір зі списку) реалізовані через команди:

```csharp
LoginCommand = new RelayCommand(async _ => await LoginAsync(), _ => CanLogin);
```

Перший параметр - метод, який виконується при виклику команди.
Другий параметр (опціональний) - умова, коли команда може бути виконана.

## Навігація

Навігація між сторінками здійснюється через `INavigationService`:

```csharp
_navigationService.NavigateTo<DiaryViewModel>();
_navigationService.NavigateBack();
```

## Примітки

1. **UserId**: В поточній реалізації `UserId` встановлений як константа (1). Потрібно інтегрувати сервіс автентифікації для отримання реального ID користувача.

2. **Збереження токену**: Після успішного входу потрібно зберегти токен (наприклад, через SecureStorage для MAUI або AppSettings для WPF).

3. **Обробка помилок**: Зараз помилки виводяться в консоль. Потрібно інтегрувати сервіс для відображення повідомлень користувачу.

4. **ICommand**: `RelayCommand` реалізує власний інтерфейс `ICommand` для сумісності. Для використання в WPF або MAUI може знадобитися адаптер для `System.Windows.Input.ICommand` або використання стандартного інтерфейсу з цих фреймворків.

5. **Адаптер для ICommand (опціонально)**: Якщо потрібно використовувати стандартний `System.Windows.Input.ICommand` з WPF/MAUI, можна створити адаптер:

```csharp
// WPF/MAUI адаптер
public class WpfRelayCommand : System.Windows.Input.ICommand
{
    private readonly RelayCommand _relayCommand;

    public WpfRelayCommand(RelayCommand relayCommand)
    {
        _relayCommand = relayCommand;
        _relayCommand.CanExecuteChanged += (s, e) => CanExecuteChanged?.Invoke(s, e);
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => _relayCommand.CanExecute(parameter);
    public void Execute(object? parameter) => _relayCommand.Execute(parameter);
}
```
