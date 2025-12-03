using MediatR;
using Microsoft.EntityFrameworkCore;
using StayFit.DAL.Interfaces;

namespace StayFit.BLL.Features.Progress.DeleteWeightEntry;

/// <summary>
/// Обробник команди видалення запису ваги.
/// </summary>
public class DeleteWeightEntryCommandHandler : IRequestHandler<DeleteWeightEntryCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public DeleteWeightEntryCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteWeightEntryCommand request, CancellationToken cancellationToken)
    {
        var userId = request.UserId;
        var id = request.WeightHistoryId;

        Console.WriteLine($"[DeleteWeightEntry] UserId={userId}, Id={id}");

        var entity = await _context.WeightHistories
            .FirstOrDefaultAsync(w => w.WeightHistoryId == id && w.UserId == userId, cancellationToken);

        if (entity is null)
        {
            Console.WriteLine("[DeleteWeightEntry] Not found");
            return Unit.Value;
        }

        _context.WeightHistories.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        Console.WriteLine("[DeleteWeightEntry] Deleted");

        return Unit.Value;
    }
}


