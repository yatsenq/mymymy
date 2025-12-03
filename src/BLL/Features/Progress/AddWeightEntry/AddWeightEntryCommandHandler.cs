using MediatR;
using Microsoft.EntityFrameworkCore;
using StayFit.BLL.Features.Progress.GetWeightHistory;
using StayFit.DAL.Interfaces;

namespace StayFit.BLL.Features.Progress.AddWeightEntry;

/// <summary>
/// Обробник команди додавання/оновлення запису ваги.
/// </summary>
public class AddWeightEntryCommandHandler : IRequestHandler<AddWeightEntryCommand, WeightHistoryEntryDto>
{
    private readonly IApplicationDbContext _context;

    public AddWeightEntryCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<WeightHistoryEntryDto> Handle(AddWeightEntryCommand request, CancellationToken cancellationToken)
    {
        var userId = request.UserId;
        var date = request.Date.Date;
        var weight = request.Weight;

        Console.WriteLine($"[AddWeightEntry] UserId={userId}, Date={date:yyyy-MM-dd}, Weight={weight}");

        // Отримати користувача для розрахунку BMI (якщо є зріст)
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);

        decimal bmi = 0;
        if (user?.Height != null && user.Height > 0)
        {
            var heightMeters = (decimal)user.Height / 100m;
            if (heightMeters > 0)
            {
                bmi = Math.Round(weight / (heightMeters * heightMeters), 2);
            }
        }

        var existing = await _context.WeightHistories
            .FirstOrDefaultAsync(w => w.UserId == userId && w.Date == date, cancellationToken);

        if (existing is null)
        {
            var entity = new DAL.Entities.WeightHistory
            {
                UserId = userId,
                Date = date,
                Weight = weight,
                Bmi = bmi
            };

            await _context.WeightHistories.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            Console.WriteLine($"[AddWeightEntry] Inserted WeightHistoryId={entity.WeightHistoryId}");

            return new WeightHistoryEntryDto(entity.WeightHistoryId, entity.Date, entity.Weight, entity.Bmi);
        }
        else
        {
            existing.Weight = weight;
            existing.Bmi = bmi;

            await _context.SaveChangesAsync(cancellationToken);

            Console.WriteLine($"[AddWeightEntry] Updated WeightHistoryId={existing.WeightHistoryId}");

            return new WeightHistoryEntryDto(existing.WeightHistoryId, existing.Date, existing.Weight, existing.Bmi);
        }
    }
}


