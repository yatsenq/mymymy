using System;

namespace StayFit.DAL.Entities;

public class DailySummary
{
    public int DailySummaryId { get; set; }
    public int UserId { get; set; }
    public DateTime Date { get; set; }
    public decimal TotalCalories { get; set; }
    public decimal TotalProtein { get; set; }
    public decimal TotalFat { get; set; }
    public decimal TotalCarbs { get; set; }
    public bool GoalAchieved { get; set; }
    public decimal GoalPercentage { get; set; }

    public User User { get; set; } = null!;
}
