using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using StayFit.BLL.Features.FoodDiary.CreateEntry;
using StayFit.BLL.Features.FoodDiary.GetDailySummary;
using StayFit.BLL.Features.FoodDiary.DeleteEntry;
using StayFit.BLL.Features.Products.SearchProducts;
using StayFit.UI.Services;
using StayFit.UI.ViewModels.Base;

namespace StayFit.UI.ViewModels
{
    public class DiaryViewModel : BaseViewModel
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        private DateTime _selectedDate = DateTime.Today;
        private int? _selectedProductId;
        private int? _selectedMealTypeId;
        private decimal _weightGrams = 100;

        private decimal _totalCalories;
        private decimal _totalProtein;
        private decimal _totalFat;
        private decimal _totalCarbs;
        private decimal _targetCalories = 2000;

        public DiaryViewModel(IMediator mediator, ICurrentUserService currentUserService)
        {
            Console.WriteLine("[DiaryViewModel] Constructor called");
            _mediator = mediator;
            _currentUserService = currentUserService;
            Title = "Щоденник";

            Entries = new ObservableCollection<FoodDiaryEntryDto>();
            AvailableProducts = new ObservableCollection<ProductDto>();
            MealTypes = new ObservableCollection<MealTypeDto>
            {
                new MealTypeDto { MealTypeId = 1, Name = "Сніданок" },
                new MealTypeDto { MealTypeId = 2, Name = "Обід" },
                new MealTypeDto { MealTypeId = 3, Name = "Вечеря" },
                new MealTypeDto { MealTypeId = 4, Name = "Перекус" }
            };

            AddEntryCommand = new RelayCommand(async _ => await AddEntryAsync(), _ => CanAddEntry);
            DeleteEntryCommand = new RelayCommand(async entry => await DeleteEntryAsync(entry as FoodDiaryEntryDto));

            _ = LoadProductsAsync();
            _ = LoadEntriesAsync();
        }

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (SetProperty(ref _selectedDate, value))
                {
                    _ = LoadEntriesAsync();
                }
            }
        }

        public int? SelectedProductId
        {
            get => _selectedProductId;
            set
            {
                if (SetProperty(ref _selectedProductId, value))
                {
                    AddEntryCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public int? SelectedMealTypeId
        {
            get => _selectedMealTypeId;
            set
            {
                if (SetProperty(ref _selectedMealTypeId, value))
                {
                    AddEntryCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public decimal WeightGrams
        {
            get => _weightGrams;
            set
            {
                if (SetProperty(ref _weightGrams, value))
                {
                    AddEntryCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public decimal TotalCalories
        {
            get => _totalCalories;
            private set => SetProperty(ref _totalCalories, value);
        }

        public decimal TotalProtein
        {
            get => _totalProtein;
            private set => SetProperty(ref _totalProtein, value);
        }

        public decimal TotalFat
        {
            get => _totalFat;
            private set => SetProperty(ref _totalFat, value);
        }

        public decimal TotalCarbs
        {
            get => _totalCarbs;
            private set => SetProperty(ref _totalCarbs, value);
        }

        public decimal TargetCalories
        {
            get => _targetCalories;
            private set => SetProperty(ref _targetCalories, value);
        }

        public ObservableCollection<FoodDiaryEntryDto> Entries { get; }
        public ObservableCollection<ProductDto> AvailableProducts { get; }
        public ObservableCollection<MealTypeDto> MealTypes { get; }

        public RelayCommand AddEntryCommand { get; }
        public RelayCommand DeleteEntryCommand { get; }

        private bool CanAddEntry
        {
            get
            {
                var result = SelectedProductId.HasValue && SelectedMealTypeId.HasValue && WeightGrams > 0;
                Console.WriteLine($"[CanAddEntry] ProductId={SelectedProductId}, MealTypeId={SelectedMealTypeId}, Weight={WeightGrams}, Result={result}");
                return result;
            }
        }

        private async Task LoadProductsAsync()
        {
            try
            {
                Console.WriteLine("[DiaryViewModel] Loading products from DB...");
                var query = new SearchProductsQuery(SearchTerm: null, Limit: 100);
                var products = await _mediator.Send(query);

                AvailableProducts.Clear();
                foreach (var product in products)
                {
                    AvailableProducts.Add(new ProductDto
                    {
                        ProductId = product.ProductId,
                        Name = product.Name,
                        CaloriesPer100g = product.CaloriesPer100g,
                        ProteinPer100g = product.ProteinPer100g,
                        FatPer100g = product.FatPer100g,
                        CarbsPer100g = product.CarbsPer100g
                    });
                }

                Console.WriteLine($"[DiaryViewModel] Loaded {AvailableProducts.Count} products from DB");
                
                // Якщо продуктів немає - додаємо fallback
                if (AvailableProducts.Count == 0)
                {
                    Console.WriteLine("[DiaryViewModel] No products in DB, using fallback");
                    AvailableProducts.Add(new ProductDto { ProductId = 1, Name = "Куряча грудка", CaloriesPer100g = 165, ProteinPer100g = 31, FatPer100g = 3.6m, CarbsPer100g = 0 });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DiaryViewModel] LoadProducts ERROR: {ex.Message}");
                Console.WriteLine($"[DiaryViewModel] StackTrace: {ex.StackTrace}");
                
                // Fallback до моків, якщо БД недоступна
                AvailableProducts.Clear();
                AvailableProducts.Add(new ProductDto { ProductId = 1, Name = "Куряча грудка", CaloriesPer100g = 165, ProteinPer100g = 31, FatPer100g = 3.6m, CarbsPer100g = 0 });
                AvailableProducts.Add(new ProductDto { ProductId = 2, Name = "Рис", CaloriesPer100g = 130, ProteinPer100g = 2.7m, FatPer100g = 0.3m, CarbsPer100g = 28 });
            }
        }
        public async Task RefreshDataAsync()
        {
            Console.WriteLine("[DiaryViewModel] RefreshDataAsync - перезавантаження даних з БД");
            await LoadEntriesAsync();
        }
        private async Task LoadEntriesAsync()
        {
            if (!_currentUserService.UserId.HasValue)
            {
                Console.WriteLine("[DiaryViewModel] LoadEntriesAsync: UserId is null");
                return;
            }

            try
            {
                Console.WriteLine($"[DiaryViewModel] LoadEntriesAsync START - UserId={_currentUserService.UserId.Value}, Date={SelectedDate:yyyy-MM-dd}");
                
                var query = new GetDailySummaryQuery(_currentUserService.UserId.Value, SelectedDate);
                var summary = await _mediator.Send(query);

                Console.WriteLine($"[DiaryViewModel] Отримано summary: Entries={summary.Entries?.Count ?? 0}");

                Entries.Clear();

                if (summary.Entries is { Count: > 0 })
                {
                    Console.WriteLine($"[DiaryViewModel] Додаємо {summary.Entries.Count} записів до UI");
                    
                    foreach (var entry in summary.Entries)
                    {
                        var dto = new FoodDiaryEntryDto
                        {
                            FoodDiaryId = entry.DiaryEntryId,
                            ProductName = entry.ProductName ?? "Unknown",
                            MealTypeName = GetMealTypeName(entry.MealTypeId),
                            WeightGrams = (decimal)entry.WeightGrams,
                            Calories = (decimal)entry.Calories,
                            Protein = (decimal)entry.Protein,
                            Fat = (decimal)entry.Fat,
                            Carbs = (decimal)entry.Carbs
                        };
                        
                        Entries.Add(dto);
                        Console.WriteLine($"[DiaryViewModel] Додано: {dto.ProductName}, Calories={dto.Calories}");
                    }
                }
                else
                {
                    Console.WriteLine("[DiaryViewModel] Немає записів у summary");
                }

                CalculateTotals();
                
                Console.WriteLine($"[DiaryViewModel] LoadEntriesAsync END - Entries.Count={Entries.Count}, TotalCalories={TotalCalories}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DiaryViewModel] LoadEntriesAsync ERROR: {ex.Message}");
                Console.WriteLine($"[DiaryViewModel] StackTrace: {ex.StackTrace}");
            }
        }

        private async Task AddEntryAsync()
        {
            Console.WriteLine("[DiaryViewModel] AddEntry STARTED");
            if (!_currentUserService.UserId.HasValue || !SelectedProductId.HasValue || !SelectedMealTypeId.HasValue)
                return;

            try
            {
                var product = AvailableProducts.FirstOrDefault(p => p.ProductId == SelectedProductId.Value);
                if (product == null) return;

                var multiplier = WeightGrams / 100m;
                var calories = product.CaloriesPer100g * multiplier;
                var protein = product.ProteinPer100g * multiplier;
                var fat = product.FatPer100g * multiplier;
                var carbs = product.CarbsPer100g * multiplier;

                var command = new CreateFoodDiaryEntryCommand(
                    _currentUserService.UserId.Value,
                    SelectedProductId.Value,
                    SelectedMealTypeId.Value,
                    (float)WeightGrams,
                    SelectedDate);

                var foodDiaryId = await _mediator.Send(command);

                if (foodDiaryId > 0)
                {
                    Entries.Add(new FoodDiaryEntryDto
                    {
                        FoodDiaryId = foodDiaryId,
                        ProductName = product.Name,
                        MealTypeName = GetMealTypeName(SelectedMealTypeId.Value),
                        WeightGrams = WeightGrams,
                        Calories = calories,
                        Protein = protein,
                        Fat = fat,
                        Carbs = carbs
                    });

                    CalculateTotals();

                    SelectedProductId = null;
                    SelectedMealTypeId = null;
                    WeightGrams = 100;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DiaryViewModel] AddEntry ERROR: {ex.Message}");
            }
        }

        private async Task DeleteEntryAsync(FoodDiaryEntryDto? entry)
        {
            if (entry == null) return;
            
            try
            {
                Console.WriteLine($"[DiaryViewModel] DeleteEntry STARTED for FoodDiaryId={entry.FoodDiaryId}");
                
                // Викликаємо MediatR для видалення з БД
                var command = new DeleteFoodDiaryEntryCommand(entry.FoodDiaryId);
                await _mediator.Send(command);
                
                Console.WriteLine($"[DiaryViewModel] DeleteEntry SUCCESS");
                
                // Тільки після успішного видалення з БД - видаляємо з UI
                Entries.Remove(entry);
                CalculateTotals();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DiaryViewModel] DeleteEntry ERROR: {ex.Message}");
                System.Windows.MessageBox.Show($"Помилка видалення: {ex.Message}", "Помилка", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void CalculateTotals()
        {
            TotalCalories = Entries.Sum(e => e.Calories);
            TotalProtein = Entries.Sum(e => e.Protein);
            TotalFat = Entries.Sum(e => e.Fat);
            TotalCarbs = Entries.Sum(e => e.Carbs);
        }

        private static string GetMealTypeName(int mealTypeId)
        {
            return mealTypeId switch
            {
                1 => "Сніданок",
                2 => "Обід",
                3 => "Вечеря",
                4 => "Перекус",
                _ => "Невідомо"
            };
        }
    }

    public class FoodDiaryEntryDto
    {
        public int FoodDiaryId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string MealTypeName { get; set; } = string.Empty;
        public decimal WeightGrams { get; set; }
        public decimal Calories { get; set; }
        public decimal Protein { get; set; }
        public decimal Fat { get; set; }
        public decimal Carbs { get; set; }
    }

    public class ProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal CaloriesPer100g { get; set; }
        public decimal ProteinPer100g { get; set; }
        public decimal FatPer100g { get; set; }
        public decimal CarbsPer100g { get; set; }
    }

    public class MealTypeDto
    {
        public int MealTypeId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
