using MediatR;
using StayFit.BLL.Features.Progress.GetWeightHistory;

namespace StayFit.BLL.Features.Progress.AddWeightEntry;

/// <summary>
/// Команда додавання/оновлення запису ваги користувача.
/// Якщо запис на цю дату вже існує, вага оновлюється.
/// </summary>
/// <param name="UserId">Ідентифікатор користувача.</param>
/// <param name="Date">Дата вимірювання.</param>
/// <param name="Weight">Вага, кг.</param>
public record AddWeightEntryCommand(
    int UserId,
    DateTime Date,
    decimal Weight) : IRequest<WeightHistoryEntryDto>;


