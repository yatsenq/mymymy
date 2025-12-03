// <copyright file="GetDailySummaryQueryValidator.cs" company="Muxluk/StayFit">
// Copyright (c) Muxluk/StayFit. All rights reserved.
// </copyright>

using FluentValidation;

namespace StayFit.BLL.Features.FoodDiary.GetDailySummary;

/// <summary>
/// Валідатор для запиту отримання щоденного зведення.
/// </summary>
public class GetDailySummaryQueryValidator : AbstractValidator<GetDailySummaryQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetDailySummaryQueryValidator"/> class.
    /// </summary>
    public GetDailySummaryQueryValidator()
    {
        this.RuleFor(x => x.userId).GreaterThan(0);
        this.RuleFor(x => x.date).NotEmpty();
    }
}
