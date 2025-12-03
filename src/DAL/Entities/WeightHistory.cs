using System;

namespace StayFit.DAL.Entities;

public class WeightHistory
{
    public int WeightHistoryId { get; set; }
    public int UserId { get; set; }
    public DateTime Date { get; set; }
    public decimal Weight { get; set; }
    public decimal Bmi { get; set; }
    public string? Notes { get; set; }

    public User User { get; set; } = null!;
}
