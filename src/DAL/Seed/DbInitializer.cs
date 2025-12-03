using StayFit.DAL.Entities;
using StayFit.DAL.Context;

namespace StayFit.DAL.Seed;

public static class DbInitializer
{
    public static void Seed(StayFitDbContext context)
    {
        if (!context.Users.Any())
        {
            var users = new List<User>
            {
                new() { Email = "anna@example.com", FirstName = "Anna", LastName = "Koval", Gender = "FEMALE", ActivityLevel = "MODERATE" },
                new() { Email = "ivan@example.com", FirstName = "Ivan", LastName = "Shevchenko", Gender = "MALE", ActivityLevel = "HIGH" }
            };
            context.Users.AddRange(users);
            context.SaveChanges();
        }

        // Products imported from CSV via Program.cs
    }
}
