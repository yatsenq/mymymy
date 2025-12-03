// <copyright file="CreateFoodDiaryEntryCommandHandler.cs" company="Muxluk/StayFit">
// Copyright (c) Muxluk/StayFit. All rights reserved.
// </copyright>

using MediatR;
using Microsoft.EntityFrameworkCore;
using StayFit.DAL.Entities;
using StayFit.DAL.Exceptions;
using StayFit.DAL.Interfaces;

namespace StayFit.BLL.Features.FoodDiary.CreateEntry;

/// <summary>
/// Обробник команди для створення нового запису в щоденнику харчування.
/// </summary>
public class CreateFoodDiaryEntryCommandHandler : IRequestHandler<CreateFoodDiaryEntryCommand, int>
{
    private readonly IApplicationDbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateFoodDiaryEntryCommandHandler"/> class.
    /// </summary>
    /// <param name="context">Контекст бази даних.</param>
    public CreateFoodDiaryEntryCommandHandler(IApplicationDbContext context)
    {
        this.context = context;
    }

    /// <inheritdoc/>
    public async Task<int> Handle(CreateFoodDiaryEntryCommand request, CancellationToken cancellationToken)
    {
        // 1. Перевірка чи існує продукт
        Console.WriteLine($"[CreateEntry] Searching for ProductId={request.productId}");
        var product = await this.context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.ProductId == request.productId, cancellationToken);

        Console.WriteLine($"[CreateEntry] Product found: {product != null}, Name: {product?.Name}");
        
        if (product is null)
        {
            throw new BusinessValidationException("Product not found.");
        }

        // 2. Перевірка чи існує MealType
        var mealTypeExists = await this.context.MealTypes
            .AnyAsync(mt => mt.MealTypeId == request.mealTypeId, cancellationToken);

        if (!mealTypeExists)
        {
            throw new BusinessValidationException("MealType not found.");
        }

        // 3. Конвертація float ваги в decimal для розрахунків
        var weightDecimal = (decimal)request.weightGrams;

        // 4. Створення сутності FoodDiary
        var foodDiaryEntry = new StayFit.DAL.Entities.FoodDiary
        {
            UserId = request.userId,
            ProductId = request.productId,
            MealTypeId = request.mealTypeId,
            WeightGrams = weightDecimal,
            Date = request.date.Date,
            Time = request.date.TimeOfDay,

            Calories = (weightDecimal / 100) * product.CaloriesPer100g,
            Protein = (weightDecimal / 100) * product.ProteinPer100g,
            Fat = (weightDecimal / 100) * product.FatPer100g,
            Carbs = (weightDecimal / 100) * product.CarbsPer100g,
        };

        // 5. Збереження в БД
        await this.context.FoodDiaries.AddAsync(foodDiaryEntry, cancellationToken);
        await this.context.SaveChangesAsync(cancellationToken);

        return foodDiaryEntry.DiaryEntryId;
    }
}
