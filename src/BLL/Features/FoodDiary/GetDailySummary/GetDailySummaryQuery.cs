// <copyright file="GetDailySummaryQuery.cs" company="Muxluk/StayFit">
// Copyright (c) Muxluk/StayFit. All rights reserved.
// </copyright>

using MediatR;

namespace StayFit.BLL.Features.FoodDiary.GetDailySummary;

/// <summary>
/// Запит на отримання щоденного зведення поживних речовин для користувача.
/// </summary>
/// <param name="userId">Ідентифікатор користувача.</param>
/// <param name="date">Дата, за яку необхідно отримати зведення.</param>
public record GetDailySummaryQuery(
    int userId,
    DateTime date): IRequest<DailySummaryDto>;
