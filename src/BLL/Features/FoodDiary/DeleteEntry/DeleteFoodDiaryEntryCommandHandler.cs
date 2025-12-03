// <copyright file="DeleteFoodDiaryEntryCommandHandler.cs" company="Muxluk/StayFit">
// Copyright (c) Muxluk/StayFit. All rights reserved.
// </copyright>

using MediatR;
using Microsoft.EntityFrameworkCore;
using StayFit.DAL.Exceptions;
using StayFit.DAL.Interfaces;

namespace StayFit.BLL.Features.FoodDiary.DeleteEntry;

/// <summary>
/// Обробник команди для видалення запису з щоденника харчування.
/// </summary>
public class DeleteFoodDiaryEntryCommandHandler : IRequestHandler<DeleteFoodDiaryEntryCommand>
{
    private readonly IApplicationDbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteFoodDiaryEntryCommandHandler"/> class.
    /// </summary>
    /// <param name="context">Контекст бази даних.</param>
    public DeleteFoodDiaryEntryCommandHandler(IApplicationDbContext context)
    {
        this.context = context;
    }

    /// <inheritdoc/>
    public async Task Handle(DeleteFoodDiaryEntryCommand request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[DeleteEntry] Deleting DiaryEntryId={request.DiaryEntryId}");

        var entry = await this.context.FoodDiaries
            .FirstOrDefaultAsync(f => f.DiaryEntryId == request.DiaryEntryId, cancellationToken);

        if (entry is null)
        {
            throw new BusinessValidationException("Food diary entry not found.");
        }

        this.context.FoodDiaries.Remove(entry);
        await this.context.SaveChangesAsync(cancellationToken);

        Console.WriteLine($"[DeleteEntry] SUCCESS");
    }
}