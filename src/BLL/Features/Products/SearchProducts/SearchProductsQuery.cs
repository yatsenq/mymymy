using MediatR;

namespace StayFit.BLL.Features.Products.SearchProducts;

/// <summary>
/// Запит для пошуку продуктів за назвою.
/// </summary>
/// <param name="SearchTerm">Текст для пошуку (якщо порожній або null - повертає всі продукти).</param>
/// <param name="Limit">Максимальна кількість результатів (за замовчуванням 50).</param>
public record SearchProductsQuery(
    string? SearchTerm = null,
    int Limit = 50) : IRequest<IReadOnlyList<ProductDto>>;

