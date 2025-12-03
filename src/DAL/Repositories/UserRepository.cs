using Microsoft.EntityFrameworkCore;
using StayFit.DAL.Context;
using StayFit.DAL.Entities;

namespace StayFit.DAL.Repositories
{
    public class UserRepository
    {
        private readonly StayFitDbContext _context;

        public UserRepository(StayFitDbContext context)
        {
            _context = context;
        }

        // CREATE
        public async Task AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        // READ (Get by Id)
        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        // READ (Get all)
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        // UPDATE
        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        // DELETE
        public async Task DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}
