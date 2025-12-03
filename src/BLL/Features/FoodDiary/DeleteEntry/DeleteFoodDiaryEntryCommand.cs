using MediatR;

namespace StayFit.BLL.Features.FoodDiary.DeleteEntry;

public record DeleteFoodDiaryEntryCommand(int DiaryEntryId) : IRequest;