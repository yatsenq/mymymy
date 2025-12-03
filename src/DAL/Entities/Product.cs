using System;
using System.Collections.Generic;

namespace StayFit.DAL.Entities;

public class Product
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = "OTHER";
    public decimal CaloriesPer100g { get; set; }
    public decimal ProteinPer100g { get; set; }
    public decimal FatPer100g { get; set; }
    public decimal CarbsPer100g { get; set; }
    public bool IsGlobal { get; set; } = true;
    public int? CreatedByUserId { get; set; }
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public bool IsApproved { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // 🔗 Додано, щоб не було помилки з контекстом
    public ICollection<FoodDiary> FoodDiaries { get; set; } = new List<FoodDiary>();

    public User? CreatedByUser { get; set; }
}