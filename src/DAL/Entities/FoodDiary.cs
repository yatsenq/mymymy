using System;

namespace StayFit.DAL.Entities;

public class FoodDiary
{
    public int DiaryEntryId { get; set; }
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public int MealTypeId { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan Time { get; set; }
    public decimal WeightGrams { get; set; }
    public decimal Calories { get; set; }
    public decimal Protein { get; set; }
    public decimal Fat { get; set; }
    public decimal Carbs { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public Product Product { get; set; } = null!;
    public MealType MealType { get; set; } = null!;
}
