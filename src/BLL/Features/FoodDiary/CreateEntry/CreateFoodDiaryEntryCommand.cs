// <copyright file="CreateFoodDiaryEntryCommand.cs" company="Muxluk/StayFit">
// Copyright (c) Muxluk/StayFit. All rights reserved.
// </copyright>

using MediatR;

namespace StayFit.BLL.Features.FoodDiary.CreateEntry;

/// <summary>
/// Команда для створення нового запису в щоденнику харчування.
/// </summary>
/// <param name="userId">Ідентифікатор користувача.</param>
/// <param name="productId">Ідентифікатор продукту.</param>
/// <param name="mealTypeId">Ідентифікатор типу прийому їжі.</param>
/// <param name="weightGrams">Вага продукту в грамах.</param>
/// <param name="date">Дата запису.</param>
public record CreateFoodDiaryEntryCommand(
    int userId,
    int productId,
    int mealTypeId,
    float weightGrams,
    DateTime date): IRequest<int>;
