using MediatR;

namespace StayFit.BLL.Features.Progress.GetWeightHistory;

/// <summary>
/// Запит на отримання історії ваги користувача.
/// </summary>
/// <param name="UserId">Ідентифікатор користувача.</param>
public record GetWeightHistoryQuery(int UserId) : IRequest<IReadOnlyList<WeightHistoryEntryDto>>;


