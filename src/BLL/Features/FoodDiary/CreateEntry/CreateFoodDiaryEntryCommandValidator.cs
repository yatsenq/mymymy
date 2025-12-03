// <copyright file="CreateFoodDiaryEntryCommandValidator.cs" company="Muxluk/StayFit">
// Copyright (c) Muxluk/StayFit. All rights reserved.
// </copyright>

using FluentValidation;

namespace StayFit.BLL.Features.FoodDiary.CreateEntry;

/// <summary>
/// Валідатор для команди створення запису в щоденнику харчування.
/// </summary>
public class CreateFoodDiaryEntryCommandValidator : AbstractValidator<CreateFoodDiaryEntryCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateFoodDiaryEntryCommandValidator"/> class.
    /// </summary>
    public CreateFoodDiaryEntryCommandValidator()
    {
        this.RuleFor(x => x.userId).GreaterThan(0);
        this.RuleFor(x => x.productId).GreaterThan(0);
        this.RuleFor(x => x.mealTypeId).GreaterThan(0);
        this.RuleFor(x => x.weightGrams).GreaterThan(0);
        this.RuleFor(x => x.date).NotEmpty();
    }
}
