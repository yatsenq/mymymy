using MediatR;
using Microsoft.EntityFrameworkCore;
using StayFit.DAL.Interfaces;

namespace StayFit.BLL.Features.Progress.GetWeightHistory;

/// <summary>
/// Обробник запиту історії ваги користувача.
/// </summary>
public class GetWeightHistoryQueryHandler : IRequestHandler<GetWeightHistoryQuery, IReadOnlyList<WeightHistoryEntryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetWeightHistoryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<WeightHistoryEntryDto>> Handle(GetWeightHistoryQuery request, CancellationToken cancellationToken)
    {
        var userId = request.UserId;

        Console.WriteLine($"[GetWeightHistory] Loading for UserId={userId}");

        var items = await _context.WeightHistories
            .AsNoTracking()
            .Where(w => w.UserId == userId)
            .OrderBy(w => w.Date)
            .Select(w => new WeightHistoryEntryDto(
                w.WeightHistoryId,
                w.Date,
                w.Weight,
                w.Bmi))
            .ToListAsync(cancellationToken);

        Console.WriteLine($"[GetWeightHistory] Loaded {items.Count} entries");

        return items;
    }
}


