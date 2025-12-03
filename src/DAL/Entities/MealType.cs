namespace StayFit.DAL.Entities;

public class MealType
{
    public int MealTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}
