using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using StayFit.BLL.Features.Progress.AddWeightEntry;
using StayFit.BLL.Features.Progress.DeleteWeightEntry;
using StayFit.BLL.Features.Progress.GetWeightHistory;
using StayFit.BLL.Features.User.GetProfile;
using StayFit.UI.Services;
using StayFit.UI.ViewModels.Base;

namespace StayFit.UI.ViewModels;

public class ProgressViewModel : BaseViewModel
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    private DateTime _selectedDate = DateTime.Today;
    private string _newWeight = ""; // <-- STRING, БЕЗ КРАШІВ
    private decimal _currentWeight;
    private decimal _startWeight;
    private decimal _goalWeight = 70;
    private decimal _weightChange;
    private string _weightChangeText = string.Empty;
    private bool _isLoading;
    private decimal? _profileStartWeight;
    private decimal _remainingWeight;

    public ProgressViewModel(IMediator mediator, ICurrentUserService currentUserService)
    {
        Console.WriteLine("[ProgressViewModel] Constructor START");

        _mediator = mediator;
        _currentUserService = currentUserService;
        Title = "Прогрес";
        _profileStartWeight = null;

        WeightHistory = new ObservableCollection<WeightEntryDto>();

        AddWeightCommand = new RelayCommand(async _ => await AddWeightAsync(), _ => CanAddWeight);
        DeleteWeightCommand = new RelayCommand(async entry => await DeleteWeightAsync(entry as WeightEntryDto));
        LoadProgressCommand = new RelayCommand(async _ => await LoadWeightHistoryAsync());

        Console.WriteLine("[ProgressViewModel] Constructor END");

        // Завантажити реальні дані з БД асинхронно
        _ = LoadWeightHistoryAsync();
    }

    private void UpdateBarHeights()
    {
        if (!WeightHistory.Any()) return;

        var minWeight = WeightHistory.Min(w => w.Weight);
        var maxWeight = WeightHistory.Max(w => w.Weight);
        var range = maxWeight - minWeight;

        if (range == 0) range = 1;

        foreach (var entry in WeightHistory)
        {
            var normalized = (entry.Weight - minWeight) / range;
            entry.BarHeight = 30 + (int)(normalized * 90);
        }
    }

    // =======================
    //        PROPERTIES
    // =======================

    public DateTime SelectedDate
    {
        get => _selectedDate;
        set => SetProperty(ref _selectedDate, value);
    }

    // ТУТ ПРАВКА ↓↓↓
    public string NewWeight
    {
        get => _newWeight;
        set
        {
            if (SetProperty(ref _newWeight, value))
                AddWeightCommand.RaiseCanExecuteChanged();
        }
    }

    private decimal ParsedNewWeight
    {
        get
        {
            if (decimal.TryParse(NewWeight, NumberStyles.Any, CultureInfo.InvariantCulture, out var w))
                return w;

            if (decimal.TryParse(NewWeight, NumberStyles.Any, CultureInfo.CurrentCulture, out var w2))
                return w2;

            return 0;
        }
    }

    public decimal CurrentWeight
    {
        get => _currentWeight;
        private set
        {
            if (SetProperty(ref _currentWeight, value))
            {
                // Оновити залежні значення
                OnPropertyChanged(nameof(ProgressPercentage));

                if (GoalWeight < StartWeight)
                {
                    // Ціль менша — треба схуднути
                    RemainingWeight = Math.Max(0, CurrentWeight - GoalWeight);
                }
                else
                {
                    // Ціль більша — треба набрати
                    RemainingWeight = Math.Max(0, GoalWeight - CurrentWeight);
                }
            }
        }
    }

    public decimal StartWeight
    {
        get => _startWeight;
        private set => SetProperty(ref _startWeight, value);
    }

    public decimal GoalWeight
    {
        get => _goalWeight;
        set
        {
            if (SetProperty(ref _goalWeight, value))
            {
                OnPropertyChanged(nameof(ProgressPercentage));

                if (GoalWeight < StartWeight)
                {
                    // Ціль менша — треба схуднути
                    RemainingWeight = Math.Max(0, CurrentWeight - GoalWeight);
                }
                else
                {
                    // Ціль більша — треба набрати
                    RemainingWeight = Math.Max(0, GoalWeight - CurrentWeight);
                }
            }
        }
    }

    public decimal WeightChange
    {
        get => _weightChange;
        private set => SetProperty(ref _weightChange, value);
    }

    public string WeightChangeText
    {
        get => _weightChangeText;
        private set => SetProperty(ref _weightChangeText, value);
    }

    public decimal ProgressPercentage
    {
        get
        {
            if (StartWeight == GoalWeight) return 0;

            // Якщо ціль менша за стартову – худнемо
            if (GoalWeight < StartWeight)
            {
                var totalToLose = StartWeight - GoalWeight;
                if (totalToLose == 0) return 0;
                var lost = StartWeight - CurrentWeight;
                var percentage = (lost / totalToLose) * 100;
                return Math.Clamp(percentage, 0, 100);
            }

            // Якщо ціль більша – набираємо вагу
            var totalToGain = GoalWeight - StartWeight;
            if (totalToGain == 0) return 0;
            var gained = CurrentWeight - StartWeight;
            var percentageGain = (gained / totalToGain) * 100;
            return Math.Clamp(percentageGain, 0, 100);
        }
    }

    public decimal RemainingWeight
    {
        get => _remainingWeight;
        private set => SetProperty(ref _remainingWeight, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ObservableCollection<WeightEntryDto> WeightHistory { get; }

    public RelayCommand AddWeightCommand { get; }
    public RelayCommand DeleteWeightCommand { get; }
    public RelayCommand LoadProgressCommand { get; }

    // =======================
    //        LOGIC
    // =======================

    private bool CanAddWeight =>
        ParsedNewWeight > 20 &&
        ParsedNewWeight < 300 &&
        SelectedDate.Date <= DateTime.Today;

    private async Task LoadWeightHistoryAsync()
    {
        if (_currentUserService.UserId is null)
        {
            Console.WriteLine("[ProgressViewModel] LoadWeightHistoryAsync: UserId is null");
            return;
        }

        IsLoading = true;
        try
        {
            var userId = _currentUserService.UserId.Value;

            // 1) Профіль користувача (поточна/цільова вага)
            var profile = await _mediator.Send(new GetUserProfileQuery { UserId = userId });
            if (profile.Success)
            {
                _profileStartWeight = profile.CurrentWeight;
                StartWeight = _profileStartWeight ?? StartWeight;
                CurrentWeight = profile.CurrentWeight ?? CurrentWeight;
                if (profile.TargetWeight != null)
                {
                    GoalWeight = profile.TargetWeight.Value;
                }
            }

            // 2) Історія ваги
            var result = await _mediator.Send(new GetWeightHistoryQuery(userId));

            WeightHistory.Clear();

            foreach (var item in result)
            {
                WeightHistory.Add(new WeightEntryDto
                {
                    Id = item.Id,
                    Weight = item.Weight,
                    Date = item.Date
                });
            }

            UpdateBarHeights();
            CalculateStatistics();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ProgressViewModel] LoadWeightHistoryAsync ERROR: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task AddWeightAsync()
    {
        var weight = ParsedNewWeight;
        if (weight <= 0) return;
        if (_currentUserService.UserId is null) return;

        IsLoading = true;

        try
        {
            var userId = _currentUserService.UserId.Value;
            var added = await _mediator.Send(new AddWeightEntryCommand(userId, SelectedDate, weight));

            // Оновити локальну колекцію
            var existing = WeightHistory.FirstOrDefault(w => w.Id == added.Id);
            if (existing is null)
            {
                WeightHistory.Add(new WeightEntryDto
                {
                    Id = added.Id,
                    Weight = added.Weight,
                    Date = added.Date
                });
            }
            else
            {
                existing.Weight = added.Weight;
                existing.Date = added.Date;
            }

            var sorted = WeightHistory.OrderBy(w => w.Date).ToList();
            WeightHistory.Clear();
            foreach (var entry in sorted)
                WeightHistory.Add(entry);

            UpdateBarHeights();
            CalculateStatistics();

            NewWeight = "";
            SelectedDate = DateTime.Today;

            await Task.CompletedTask;
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task DeleteWeightAsync(WeightEntryDto? entry)
    {
        if (entry == null) return;
        if (_currentUserService.UserId is null) return;

        try
        {
            var userId = _currentUserService.UserId.Value;
            await _mediator.Send(new DeleteWeightEntryCommand(userId, entry.Id));

            WeightHistory.Remove(entry);
            UpdateBarHeights();
            CalculateStatistics();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ProgressViewModel] DeleteWeightAsync ERROR: {ex.Message}");
        }

        await Task.CompletedTask;
    }

    private void CalculateStatistics()
    {
        if (!WeightHistory.Any())
        {
            var baseWeight = _profileStartWeight ?? 0;
            StartWeight = baseWeight;
            CurrentWeight = baseWeight;
            WeightChange = 0;

            if (GoalWeight < StartWeight)
                RemainingWeight = Math.Max(0, CurrentWeight - GoalWeight);
            else
                RemainingWeight = Math.Max(0, GoalWeight - CurrentWeight);

            WeightChangeText = "Немає даних";
            return;
        }

        var sorted = WeightHistory.OrderBy(w => w.Date).ToList();
        StartWeight = _profileStartWeight ?? sorted.First().Weight;

        // Поточна вага — це вимір за сьогодні, якщо він є,
        // інакше беремо вагу профілю, а якщо її нема — останній вимір
        var today = DateTime.Today;
        var todayEntry = sorted.LastOrDefault(w => w.Date.Date == today);
        if (todayEntry is not null)
        {
            CurrentWeight = todayEntry.Weight;
        }
        else if (_profileStartWeight.HasValue)
        {
            CurrentWeight = _profileStartWeight.Value;
        }
        else
        {
            CurrentWeight = sorted.Last().Weight;
        }

        if (GoalWeight < StartWeight)
            RemainingWeight = Math.Max(0, CurrentWeight - GoalWeight);
        else
            RemainingWeight = Math.Max(0, GoalWeight - CurrentWeight);
        WeightChange = CurrentWeight - StartWeight;

        if (WeightChange < 0)
            WeightChangeText = $"▼ {Math.Abs(WeightChange):F1} кг";
        else if (WeightChange > 0)
            WeightChangeText = $"▲ {WeightChange:F1} кг";
        else
            WeightChangeText = "Без змін";
    }
}

public class WeightEntryDto
{
    public int Id { get; set; }
    public decimal Weight { get; set; }
    public DateTime Date { get; set; }
    public int BarHeight { get; set; } = 50;
    public string DateFormatted => Date.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
    public string ShortDate => Date.ToString("dd.MM", CultureInfo.InvariantCulture);
}
