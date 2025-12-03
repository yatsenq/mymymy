// <copyright file="DailySummaryDto.cs" company="Muxluk/StayFit">
// Copyright (c) Muxluk/StayFit. All rights reserved.
// </copyright>
namespace StayFit.BLL.Features.FoodDiary.GetDailySummary;

/// <summary>
/// DTO, що містить щоденне зведення поживних речовин.
/// </summary>
public record DailySummaryDto(
    float TotalCalories,
    float TotalProtein,
    float TotalFat,
    float TotalCarbs,
    List<FoodDiaryEntryDto>? Entries);

/// <summary>
/// DTO для одного запису в щоденнику.
/// </summary>
public record FoodDiaryEntryDto(
    int DiaryEntryId,
    int ProductId,
    string? ProductName,
    int MealTypeId,
    float WeightGrams,
    float Calories,
    float Protein,
    float Fat,
    float Carbs);
