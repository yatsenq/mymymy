namespace StayFit.DAL.Entities;

public class UserSetting
{
    public int UserSettingId { get; set; }
    public int UserId { get; set; }
    public string Language { get; set; } = "uk";
    public string Theme { get; set; } = "LIGHT";
    public bool ReminderFoodEnabled { get; set; } = true;
    public bool WeeklyReportsEnabled { get; set; } = true;

    public User User { get; set; } = null!;
}
