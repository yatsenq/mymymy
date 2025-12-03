using Microsoft.EntityFrameworkCore;
using StayFit.DAL.Entities;

namespace StayFit.DAL.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<UserGoal> UserGoals { get; set; }
        DbSet<UserSetting> UserSettings { get; set; }
        DbSet<Product> Products { get; set; }
        DbSet<FoodDiary> FoodDiaries { get; set; }
        DbSet<MealType> MealTypes { get; set; }
        DbSet<WeightHistory> WeightHistories { get; set; }
        DbSet<DailySummary> DailySummaries { get; set; }
        DbSet<UserSession> UserSessions { get; set; }
        DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
        DbSet<ActivityLog> ActivityLogs { get; set; }
        
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}