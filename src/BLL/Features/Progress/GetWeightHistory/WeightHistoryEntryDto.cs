namespace StayFit.BLL.Features.Progress.GetWeightHistory;

/// <summary>
/// DTO одного запису історії ваги.
/// </summary>
/// <param name="Id">Ідентифікатор запису.</param>
/// <param name="Date">Дата вимірювання.</param>
/// <param name="Weight">Вага, кг.</param>
/// <param name="Bmi">Індекс маси тіла.</param>
public record WeightHistoryEntryDto(
    int Id,
    DateTime Date,
    decimal Weight,
    decimal Bmi);


