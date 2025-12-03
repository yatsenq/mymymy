using Microsoft.EntityFrameworkCore;
using StayFit.DAL.Context;
using StayFit.DAL.Entities;

namespace StayFit.DAL.Repositories
{
    public class ProductRepository
    {
        private readonly StayFitDbContext _context;

        public ProductRepository(StayFitDbContext context)
        {
            _context = context;
        }

        // CREATE
        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        // READ ALL
        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products.AsNoTracking().ToListAsync();
        }

        // READ BY ID
        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        // UPDATE
        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        // DELETE
        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
    }
}
