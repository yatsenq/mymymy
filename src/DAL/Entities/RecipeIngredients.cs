// <copyright file="RecipeIngredient.cs" company="Muxluk/StayFit">
// Copyright (c) Muxluk/StayFit. All rights reserved.
// </copyright>

namespace StayFit.DAL.Entities;

/// <summary>
/// Сутність, що представляє інгредієнт у конкретному рецепті.
/// </summary>
public class RecipeIngredient
{
    /// <summary>
    /// Отримує або задає ідентифікатор інгредієнта рецепту.
    /// </summary>
    public int RecipeIngredientId { get; set; }

    /// <summary>
    /// Отримує або задає ідентифікатор рецепту.
    /// </summary>
    public int RecipeId { get; set; }

    /// <summary>
    /// Отримує або задає ідентифікатор продукту.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Отримує або задає вагу інгредієнта в грамах.
    /// </summary>
    public decimal WeightGrams { get; set; }

    /// <summary>
    /// Отримує або задає індекс порядку для кроків.
    /// </summary>
    public int OrderIndex { get; set; }

    /// <summary>
    /// Отримує або задає навігаційну властивість до рецепту.
    /// </summary>
    public Recipe Recipe { get; set; } = null!;

    /// <summary>
    /// Отримує або задає навігаційну властивість до продукту.
    /// </summary>
    public Product Product { get; set; } = null!;
}