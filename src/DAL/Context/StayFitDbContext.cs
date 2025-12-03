using Microsoft.EntityFrameworkCore;
using StayFit.DAL.Entities;
using StayFit.DAL.Interfaces;

namespace StayFit.DAL.Context
{
    public class StayFitDbContext : DbContext, IApplicationDbContext
    {
        public StayFitDbContext(DbContextOptions<StayFitDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<UserGoal> UserGoals { get; set; } = null!;
        public DbSet<UserSetting> UserSettings { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<FoodDiary> FoodDiaries { get; set; } = null!;
        public DbSet<MealType> MealTypes { get; set; } = null!;
        public DbSet<WeightHistory> WeightHistories { get; set; } = null!;
        public DbSet<DailySummary> DailySummaries { get; set; } = null!;
        public DbSet<UserSession> UserSessions { get; set; } = null!;
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; } = null!;
        public DbSet<ActivityLog> ActivityLogs { get; set; } = null!;

        public new async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Map to lowercase table names to match schema.sql
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<UserGoal>().ToTable("user_goals");
            modelBuilder.Entity<UserSetting>().ToTable("user_settings");
            // Product mapped below
            modelBuilder.Entity<FoodDiary>(entity =>
            {
                entity.ToTable("food_diaries");
                entity.HasKey(fd => fd.DiaryEntryId);
                entity.Property(fd => fd.DiaryEntryId).HasColumnName("diary_entry_id");
                entity.Property(fd => fd.UserId).HasColumnName("user_id");
                entity.Property(fd => fd.ProductId).HasColumnName("product_id");
                entity.Property(fd => fd.MealTypeId).HasColumnName("meal_type_id");
                entity.Property(fd => fd.Date).HasColumnName("date");
                entity.Property(fd => fd.Time).HasColumnName("time");
                entity.Property(fd => fd.WeightGrams).HasColumnName("weight_grams");
                entity.Property(fd => fd.Calories).HasColumnName("calories");
                entity.Property(fd => fd.Protein).HasColumnName("protein");
                entity.Property(fd => fd.CreatedAt).HasColumnName("created_at");
                
                entity.Ignore(fd => fd.Notes);
            });
            modelBuilder.Entity<MealType>(entity =>
            {
                entity.ToTable("meal_types");

                entity.HasKey(m => m.MealTypeId);
                entity.Property(m => m.MealTypeId).HasColumnName("meal_type_id");
                entity.Property(m => m.Name).HasColumnName("name");
                entity.Property(m => m.DisplayOrder).HasColumnName("display_order");
            });

            modelBuilder.Entity<WeightHistory>(entity =>
            {
                entity.ToTable("weight_history");

                entity.HasKey(w => w.WeightHistoryId);
                entity.Property(w => w.WeightHistoryId).HasColumnName("weight_entry_id");
                entity.Property(w => w.UserId).HasColumnName("user_id");
                entity.Property(w => w.Date).HasColumnName("date");
                entity.Property(w => w.Weight).HasColumnName("weight");
                entity.Property(w => w.Bmi).HasColumnName("bmi");
                entity.Property(w => w.Notes).HasColumnName("notes");
            });
            modelBuilder.Entity<DailySummary>().ToTable("daily_summary");
            modelBuilder.Entity<UserSession>().ToTable("user_sessions");
            modelBuilder.Entity<PasswordResetToken>().ToTable("password_reset_tokens");
            modelBuilder.Entity<ActivityLog>().ToTable("activity_log");

            // Приклад — унікальний індекс
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasOne(u => u.UserSettings)
                .WithOne(s => s.User)
                .HasForeignKey<UserSetting>(s => s.UserId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.UserGoal)
                .WithOne(g => g.User)
                .HasForeignKey<UserGoal>(g => g.UserId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.FoodDiaries)
                .WithOne(fd => fd.User)
                .HasForeignKey(fd => fd.UserId);

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("products");
                entity.Property(e => e.ProductId).HasColumnName("product_id");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Category).HasColumnName("category");
                entity.Property(e => e.CaloriesPer100g).HasColumnName("calories_per_100g");
                entity.Property(e => e.ProteinPer100g).HasColumnName("protein_per_100g");
                entity.Property(e => e.FatPer100g).HasColumnName("fat_per_100g");
                entity.Property(e => e.CarbsPer100g).HasColumnName("carbs_per_100g");
                entity.Property(e => e.IsGlobal).HasColumnName("is_global");
                entity.Property(e => e.CreatedByUserId).HasColumnName("created_by_user_id");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                
                // Ignore properties not in schema
                entity.Ignore(e => e.UpdatedAt);
                entity.Ignore(e => e.IsApproved);
            });

            modelBuilder.Entity<Product>()
            .HasIndex(p => p.Name)
            .HasDatabaseName("idx_products_name");

            modelBuilder.Entity<Product>()
                .HasMany(p => p.FoodDiaries)
                .WithOne(f => f.Product)
                .HasForeignKey(f => f.ProductId);

        }
    }
}
