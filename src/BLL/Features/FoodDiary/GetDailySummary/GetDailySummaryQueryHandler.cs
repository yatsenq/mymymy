// <copyright file="GetDailySummaryQueryHandler.cs" company="Muxluk/StayFit">
// Copyright (c) Muxluk/StayFit. All rights reserved.
// </copyright>
using MediatR;
using Microsoft.EntityFrameworkCore;
using StayFit.DAL.Interfaces;

namespace StayFit.BLL.Features.FoodDiary.GetDailySummary;

/// <summary>
/// Обробник запиту на отримання щоденного зведення поживних речовин.
/// </summary>
public class GetDailySummaryQueryHandler : IRequestHandler<GetDailySummaryQuery, DailySummaryDto>
{
    private readonly IApplicationDbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetDailySummaryQueryHandler"/> class.
    /// </summary>
    /// <param name="context">Контекст бази даних.</param>
    public GetDailySummaryQueryHandler(IApplicationDbContext context)
    {
        this.context = context;
    }

    /// <inheritdoc/>
    public async Task<DailySummaryDto> Handle(GetDailySummaryQuery request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[GetDailySummary] Loading for UserId={request.userId}, Date={request.date:yyyy-MM-dd}");

        // Завантажити записи з щоденника
        var entries = await this.context.FoodDiaries
            .AsNoTracking()
            .Include(fd => fd.Product)
            .Include(fd => fd.MealType)
            .Where(fd => fd.UserId == request.userId && fd.Date.Date == request.date.Date)
            .Select(fd => new FoodDiaryEntryDto(
                fd.DiaryEntryId,
                fd.ProductId,
                fd.Product != null ? fd.Product.Name : "Unknown",
                fd.MealTypeId,
                (float)fd.WeightGrams,
                (float)fd.Calories,
                (float)fd.Protein,
                (float)fd.Fat,
                (float)fd.Carbs))
            .ToListAsync(cancellationToken);

        Console.WriteLine($"[GetDailySummary] Found {entries.Count} entries");

        // Розрахувати totals
        var totalCalories = entries.Sum(e => e.Calories);
        var totalProtein = entries.Sum(e => e.Protein);
        var totalFat = entries.Sum(e => e.Fat);
        var totalCarbs = entries.Sum(e => e.Carbs);

        return new DailySummaryDto(
            totalCalories,
            totalProtein,
            totalFat,
            totalCarbs,
            entries);
    }
}
