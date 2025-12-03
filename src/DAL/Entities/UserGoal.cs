using System;

namespace StayFit.DAL.Entities;

public class UserGoal
{
    public int UserGoalId { get; set; }
    public int UserId { get; set; }
    public decimal DailyCalories { get; set; }
    public bool IsAutoCalculated { get; set; } = true;
    public string GoalType { get; set; } = "MAINTENANCE";
    public decimal ProteinGrams { get; set; }
    public decimal FatGrams { get; set; }
    public decimal CarbsGrams { get; set; }
    public decimal ProteinPercent { get; set; }
    public decimal FatPercent { get; set; }
    public decimal CarbsPercent { get; set; }
    public int MealsPerDay { get; set; }
    public decimal? WeightChangeRate { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
}
