namespace StayFit.BLL.Features.Products.SearchProducts;

/// <summary>
/// DTO для продукту.
/// </summary>
public record ProductDto(
    int ProductId,
    string Name,
    string Category,
    decimal CaloriesPer100g,
    decimal ProteinPer100g,
    decimal FatPer100g,
    decimal CarbsPer100g);

