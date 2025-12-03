# MVVM Architecture - StayFit

## Огляд

Реалізовано повну MVVM-архітектуру для додатку StayFit з чітким поділом на View, ViewModel та Model (BLL/DAL).

## Структура

```
src/UI/
├── ViewModels/
│   ├── Base/
│   │   ├── BaseViewModel.cs      ✅ Базовий клас з INotifyPropertyChanged
│   │   └── RelayCommand.cs       ✅ Реалізація ICommand для команд
│   ├── LoginViewModel.cs         ✅ ViewModel для входу
│   ├── DiaryViewModel.cs         ✅ ViewModel для щоденника
│   ├── ProgressViewModel.cs      ✅ ViewModel для прогресу
│   └── SettingsViewModel.cs      ✅ ViewModel для налаштувань
├── Services/
│   ├── Navigation/
│   │   ├── INavigationService.cs ✅ Інтерфейс навігації
│   │   └── NavigationService.cs  ✅ Реалізація навігації
│   └── ServiceLocator.cs        ✅ Service Locator pattern
├── DependencyInjection/
│   └── ServiceCollectionExtensions.cs ✅ Розширення для DI
└── Examples/
    └── DependencyInjectionExample.cs  ✅ Приклади конфігурації
```

## Реалізовані компоненти

### ✅ Базові класи

1. **BaseViewModel**
   - Реалізує `INotifyPropertyChanged` для двостороннього зв'язування
   - Властивості: `IsBusy`, `Title`
   - Методи: `OnPropertyChanged()`, `SetProperty<T>()`

2. **RelayCommand**
   - Реалізує `ICommand` для виконання команд
   - Підтримує умови виконання (`CanExecute`)
   - Генерічний варіант `RelayCommand<T>`

3. **NavigationService**
   - Управління навігацією між сторінками
   - Методи: `NavigateTo<T>()`, `NavigateBack()`
   - Зберігає історію навігації

### ✅ ViewModels

1. **LoginViewModel**
   - Властивості: `Email`, `Password`, `ErrorMessage`, `IsLoading`
   - Команда: `LoginCommand`
   - Інтеграція з `LoginUserQuery` через MediatR

2. **DiaryViewModel**
   - Властивості: `SelectedDate`, `SelectedProductId`, `SelectedMealTypeId`, `WeightGrams`
   - Підсумки: `TotalCalories`, `TotalProtein`, `TotalFat`, `TotalCarbs`
   - Команди: `LoadDailySummaryCommand`, `AddEntryCommand`
   - Інтеграція з `GetDailySummaryQuery` та `CreateFoodDiaryEntryCommand`

3. **ProgressViewModel**
   - Властивості: `SelectedStartDate`, `SelectedEndDate`, `IsLoading`
   - Команда: `LoadProgressCommand`
   - Готова для інтеграції з запитами про прогрес

4. **SettingsViewModel**
   - Властивості: `DailyCalorieGoal`, `ProteinGoal`, `FatGoal`, `CarbsGoal`
   - Команди: `SaveSettingsCommand`, `LoadSettingsCommand`
   - Двостороннє зв'язування для всіх налаштувань

### ✅ Dependency Injection

1. **ServiceCollectionExtensions**
   - Метод `AddUI()` - реєстрація всіх ViewModels та сервісів
   - Метод `AddViewModels()` - реєстрація ViewModels
   - Метод `AddNavigationServices()` - реєстрація навігації

2. **ServiceLocator**
   - Патерн Service Locator для доступу до сервісів
   - Ініціалізація через `Initialize()`

## Інтеграція з BLL/DAL

### Через MediatR

Всі ViewModels використовують `IMediator` для відправки запитів/команд:

```csharp
// Приклад з LoginViewModel
var query = new LoginUserQuery(Email, Password);
var response = await _mediator.Send(query);
```

### Dependency Injection

```csharp
// Реєстрація в DI контейнері
services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(LoginUserQueryHandler).Assembly));
services.AddUI();
```

## Двостороннє зв'язування (Binding)

Всі властивості ViewModels підтримують двостороннє зв'язування через `INotifyPropertyChanged`:

```xml
<!-- WPF -->
<TextBox Text="{Binding Email, UpdateSourceTrigger=PropertyChanged}" />

<!-- MAUI -->
<Entry Text="{Binding Email}" />
```

При зміні значення в UI автоматично оновлюється властивість в ViewModel, і навпаки.

## Командна система

Всі дії користувача реалізовані через команди:

```csharp
LoginCommand = new RelayCommand(
    async _ => await LoginAsync(), 
    _ => CanLogin
);
```

Команди автоматично оновлюють свій стан через `RaiseCanExecuteChanged()`.

## Навігація

Навігація між сторінками здійснюється через `INavigationService`:

```csharp
_navigationService.NavigateTo<DiaryViewModel>();
_navigationService.NavigateBack();
```

## Вимоги до UI проекту

Для використання цієї MVVM-архітектури потрібно:

1. **Створити UI проект** (WPF або .NET MAUI)
2. **Додати посилання** на проект `UI.csproj`
3. **Налаштувати DI** (див. `Examples/DependencyInjectionExample.cs`)
4. **Створити View** (XAML сторінки) та прив'язати їх до ViewModels
5. **Інтегрувати з BLL/DAL** через MediatR

## Приклад використання

### WPF

```csharp
// App.xaml.cs
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        var serviceProvider = DependencyInjectionExample.ConfigureServicesForWpf();
        var mainWindow = new MainWindow();
        mainWindow.DataContext = serviceProvider.GetRequiredService<LoginViewModel>();
        mainWindow.Show();
    }
}
```

### .NET MAUI

```csharp
// MauiProgram.cs
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();
        
        return builder.ConfigureServicesForMaui().Build();
    }
}
```

## Наступні кроки

1. ✅ Створено базові класи
2. ✅ Створено ViewModels
3. ✅ Налаштовано DI
4. ⏳ Створити UI проект (WPF або MAUI)
5. ⏳ Створити View (XAML сторінки)
6. ⏳ Інтегрувати сервіс автентифікації
7. ⏳ Додати обробку помилок (діалоги, тости)

## Примітки

- `UserId` встановлений як константа (1) - потрібно інтегрувати сервіс автентифікації
- Помилки виводяться в консоль - потрібно додати сервіс для відображення повідомлень
- `RelayCommand` використовує власний `ICommand` - для WPF/MAUI може знадобитися адаптер

