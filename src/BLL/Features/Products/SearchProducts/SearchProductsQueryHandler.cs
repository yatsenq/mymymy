using MediatR;
using Microsoft.EntityFrameworkCore;
using StayFit.DAL.Interfaces;

namespace StayFit.BLL.Features.Products.SearchProducts;

/// <summary>
/// Обробник запиту пошуку продуктів.
/// </summary>
public class SearchProductsQueryHandler : IRequestHandler<SearchProductsQuery, IReadOnlyList<ProductDto>>
{
    private readonly IApplicationDbContext _context;

    public SearchProductsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ProductDto>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Products.AsNoTracking();

        // Якщо є текст для пошуку - фільтруємо по назві (case-insensitive)
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.Trim();
            query = query.Where(p => EF.Functions.ILike(p.Name, $"%{searchTerm}%"));
        }

        // Сортуємо по назві та обмежуємо кількість
        var products = await query
            .OrderBy(p => p.Name)
            .Take(request.Limit)
            .Select(p => new ProductDto(
                p.ProductId,
                p.Name,
                p.Category,
                p.CaloriesPer100g,
                p.ProteinPer100g,
                p.FatPer100g,
                p.CarbsPer100g))
            .ToListAsync(cancellationToken);

        return products;
    }
}

