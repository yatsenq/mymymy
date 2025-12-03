using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace StayFit.DAL.Entities;

public class User
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = "MALE";
    public decimal? Height { get; set; }
    public decimal? CurrentWeight { get; set; }
    
    public decimal? TargetWeight { get; set; }
    
    public string ActivityLevel { get; set; } = "MODERATELY_ACTIVE";
    
  
    public decimal? Bmr { get; set; }
    
    
    public decimal? Tdee { get; set; }
    
    public decimal? Bmi { get; set; }
    
   
    public string? BmiCategory { get; set; }
  
    public string Role { get; set; } = "USER";
    
    
    public bool IsVerified { get; set; } = false;
    
    
    public bool IsActive { get; set; } = true;
    
    
    public string? ProfilePhotoUrl { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    

    public DateTime? LastLogin { get; set; }

    // Навігаційні властивості
    
    public UserGoal? UserGoal { get; set; }
    
    public UserSetting? UserSettings { get; set; }
    
    public ICollection<FoodDiary> FoodDiaries { get; set; } = new List<FoodDiary>();
    
    
    public ICollection<WeightHistory> WeightHistories { get; set; } = new List<WeightHistory>();
    
    
    public ICollection<UserSession> UserSessions { get; set; } = new List<UserSession>();

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
