using System;
using System.Threading.Tasks;
using MediatR;
using StayFit.BLL.Features.User.UpdateProfile;
using StayFit.BLL.Features.User.GetProfile;
using StayFit.UI.Services;
using StayFit.UI.ViewModels.Base;

namespace StayFit.UI.ViewModels;

public class SettingsViewModel : BaseViewModel
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;
    
    private decimal _height;
    private decimal _weight;
    private decimal _targetWeight;
    private string _activityLevel = "SEDENTARY";
    private string _gender = "MALE";
    private DateTime _dateOfBirth = DateTime.Now.AddYears(-25);
    
    private decimal _bmi;
    private decimal _bmr;
    private decimal _tdee;
    private decimal _recommendedCalories;
    private string _bmiStatus = string.Empty;
    private string _goalStatus = string.Empty;
    
    private bool _isLoading;
    private string _errorMessage = string.Empty;
    private string _successMessage = string.Empty;

    public SettingsViewModel(IMediator mediator, ICurrentUserService currentUserService)
    {
        Console.WriteLine("[SettingsViewModel] Constructor called");
        _mediator = mediator;
        _currentUserService = currentUserService;
        Title = "Налаштування";
        
        UpdateProfileCommand = new RelayCommand(async _ => await UpdateProfileAsync(), _ => CanSave);
        Console.WriteLine("[SettingsViewModel] UpdateProfileCommand created");
        
        // Завантажити дані при створенні
        _ = LoadUserProfileAsync();
    }

    // Фізичні параметри
    public decimal Height
    {
        get => _height;
        set
        {
            if (SetProperty(ref _height, value))
            {
                UpdateProfileCommand.RaiseCanExecuteChanged();
                CalculateMetricsLocally();
            }
        }
    }

    public decimal Weight
    {
        get => _weight;
        set
        {
            if (SetProperty(ref _weight, value))
            {
                UpdateProfileCommand.RaiseCanExecuteChanged();
                CalculateMetricsLocally();
            }
        }
    }

    public decimal TargetWeight
    {
        get => _targetWeight;
        set
        {
            if (SetProperty(ref _targetWeight, value))
            {
                UpdateProfileCommand.RaiseCanExecuteChanged();
                CalculateMetricsLocally();
            }
        }
    }

    public string ActivityLevel
    {
        get => _activityLevel;
        set
        {
            if (SetProperty(ref _activityLevel, value))
            {
                UpdateProfileCommand.RaiseCanExecuteChanged();
                CalculateMetricsLocally();
            }
        }
    }

    // Розраховані метрики (readonly)
    public decimal Bmi
    {
        get => _bmi;
        private set => SetProperty(ref _bmi, value);
    }

    public decimal Bmr
    {
        get => _bmr;
        private set => SetProperty(ref _bmr, value);
    }

    public decimal Tdee
    {
        get => _tdee;
        private set => SetProperty(ref _tdee, value);
    }

    public decimal RecommendedCalories
    {
        get => _recommendedCalories;
        private set => SetProperty(ref _recommendedCalories, value);
    }

    public string BmiStatus
    {
        get => _bmiStatus;
        private set => SetProperty(ref _bmiStatus, value);
    }

    public string GoalStatus
    {
        get => _goalStatus;
        private set => SetProperty(ref _goalStatus, value);
    }

    // UI стан
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public string SuccessMessage
    {
        get => _successMessage;
        set => SetProperty(ref _successMessage, value);
    }

    public bool CanSave
    {
        get
        {
            var result = Height > 0 && Weight > 0 && !IsLoading;
            Console.WriteLine($"[SettingsViewModel] CanSave = {result} (H:{Height}, W:{Weight}, Loading:{IsLoading})");
            return result;
        }
    }

    public RelayCommand UpdateProfileCommand { get; }

    private async Task LoadUserProfileAsync()
    {
        Console.WriteLine($"[SettingsViewModel] LoadUserProfileAsync - UserId: {_currentUserService.UserId}");
        
        if (!_currentUserService.UserId.HasValue)
        {
            ErrorMessage = "❌ Користувач не авторизований";
            Console.WriteLine("[SettingsViewModel] ERROR - No UserId");
            return;
        }

        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            var query = new GetUserProfileQuery { UserId = _currentUserService.UserId.Value };
            var result = await _mediator.Send(query);

            if (result.Success)
            {
                Console.WriteLine($"[SettingsViewModel] Profile loaded - H:{result.Height}, W:{result.CurrentWeight}");
                
                if (result.Height.HasValue)
                    Height = result.Height.Value;
                
                if (result.CurrentWeight.HasValue)
                    Weight = result.CurrentWeight.Value;
                
                if (result.TargetWeight.HasValue) TargetWeight = result.TargetWeight.Value;
                if (!string.IsNullOrEmpty(result.ActivityLevel))
                {
                    ActivityLevel = result.ActivityLevel;
                }
                else
                {
                    ActivityLevel = "MODERATELY_ACTIVE"; // Default
                }
                    ActivityLevel = "MODERATELY_ACTIVE"; // Default
                if (!string.IsNullOrEmpty(result.Gender))
                    _gender = result.Gender;

                _dateOfBirth = result.DateOfBirth;

                CalculateMetricsLocally();
            }
            else
            {
                ErrorMessage = "⚠️ Не вдалося завантажити профіль";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SettingsViewModel] LoadProfile EXCEPTION: {ex.Message}");
            ErrorMessage = $"❌ Помилка завантаження: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void CalculateMetricsLocally()
    {
        if (Height <= 0 || Weight <= 0)
        {
            Bmi = 0;
            Bmr = 0;
            Tdee = 0;
            RecommendedCalories = 0;
            BmiStatus = string.Empty;
            GoalStatus = string.Empty;
            return;
        }

        var heightInMeters = Height / 100;
        Bmi = Math.Round(Weight / (heightInMeters * heightInMeters), 1);

        BmiStatus = Bmi switch
        {
            < 18.5m => "Недостатня вага",
            >= 18.5m and < 25m => "Нормальна вага",
            >= 25m and < 30m => "Надлишкова вага",
            _ => "Ожиріння"
        };

        var age = DateTime.UtcNow.Year - _dateOfBirth.Year;
        
        if (_gender == "MALE")
        {
            Bmr = Math.Round((10 * Weight) + (6.25m * Height) - (5 * age) + 5, 0);
        }
        else
        {
            Bmr = Math.Round((10 * Weight) + (6.25m * Height) - (5 * age) - 161, 0);
        }

        var multiplier = ActivityLevel switch
        {
            "SEDENTARY" => 1.2m,
            "LIGHTLY_ACTIVE" => 1.375m,
            "MODERATELY_ACTIVE" => 1.55m,
            "VERY_ACTIVE" => 1.725m,
            "EXTRA_ACTIVE" => 1.9m,
            _ => 1.2m
        };
        
        Tdee = Math.Round(Bmr * multiplier, 0);

        if (TargetWeight > 0)
        {
            var weightDiff = Weight - TargetWeight;
            
            if (Math.Abs(weightDiff) < 1)
            {
                RecommendedCalories = Tdee;
                GoalStatus = "Підтримка поточної ваги";
            }
            else if (weightDiff > 0)
            {
                RecommendedCalories = Math.Round(Tdee - 500, 0);
                GoalStatus = $"Схуднути на {Math.Abs(weightDiff):F1} кг";
            }
            else
            {
                RecommendedCalories = Math.Round(Tdee + 500, 0);
                GoalStatus = $"Набрати {Math.Abs(weightDiff):F1} кг";
            }
        }
        else
        {
            RecommendedCalories = Tdee;
            GoalStatus = string.Empty;
        }
    }

    private async Task UpdateProfileAsync()
    {
        Console.WriteLine("[SettingsViewModel] ========== UpdateProfileAsync START ==========");
        Console.WriteLine($"[SettingsViewModel] CanSave: {CanSave}");
        
        if (!CanSave)
        {
            Console.WriteLine("[SettingsViewModel] CanSave is FALSE - ABORTING");
            return;
        }

        if (!_currentUserService.UserId.HasValue)
        {
            Console.WriteLine("[SettingsViewModel] No UserId - ABORTING");
            ErrorMessage = "❌ Користувач не авторизований";
            return;
        }

        Console.WriteLine($"[SettingsViewModel] UserId: {_currentUserService.UserId.Value}");
        Console.WriteLine($"[SettingsViewModel] Height: {Height}, Weight: {Weight}, TargetWeight: {TargetWeight}");

        IsLoading = true;
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;

        try
        {
            var command = new UpdateUserProfileCommand
            {
                UserId = _currentUserService.UserId.Value,
                Height = Height,
                CurrentWeight = Weight,
                TargetWeight = TargetWeight,
                ActivityLevel = ActivityLevel
            };

            Console.WriteLine("[SettingsViewModel] Sending command via MediatR...");
            var result = await _mediator.Send(command);
            Console.WriteLine($"[SettingsViewModel] Result received - Success: {result.Success}");

            if (result.Success)
            {
                SuccessMessage = "✅ Профіль успішно збережено в базу даних!";
                Console.WriteLine("[SettingsViewModel] SUCCESS!");
                
                if (result.Bmi.HasValue)
                    Bmi = result.Bmi.Value;
                if (result.Bmr.HasValue)
                    Bmr = result.Bmr.Value;
                if (result.Tdee.HasValue)
                    Tdee = result.Tdee.Value;
                
                CalculateMetricsLocally();
            }
            else
            {
                ErrorMessage = result.Message;
                Console.WriteLine($"[SettingsViewModel] FAILED: {result.Message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SettingsViewModel] EXCEPTION: {ex.Message}");
            Console.WriteLine($"[SettingsViewModel] StackTrace: {ex.StackTrace}");
            ErrorMessage = $"❌ Помилка збереження: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
            Console.WriteLine("[SettingsViewModel] ========== UpdateProfileAsync END ==========");
        }
    }
}
