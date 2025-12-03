using MediatR;

namespace StayFit.BLL.Features.Progress.DeleteWeightEntry;

/// <summary>
/// Команда видалення запису ваги користувача.
/// </summary>
/// <param name="UserId">Ідентифікатор користувача.</param>
/// <param name="WeightHistoryId">Ідентифікатор запису історії ваги.</param>
public record DeleteWeightEntryCommand(int UserId, int WeightHistoryId) : IRequest<Unit>;


