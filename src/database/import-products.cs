using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using StayFit.DAL.Context;
using StayFit.DAL.Entities;

namespace StayFit.Database
{
    public class ProductImporter
    {
        public static void Import(StayFitDbContext context, string csvFilePath)
        {
            if (!File.Exists(csvFilePath))
            {
                Console.WriteLine($"Error: File not found at {csvFilePath}");
                return;
            }

            Console.WriteLine($"Reading products from {csvFilePath}...");

            var products = new List<Product>();
            var lines = File.ReadAllLines(csvFilePath);

            // Skip header
            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(',');
                if (parts.Length < 6) continue;

                try
                {
                    var product = new Product
                    {
                        Name = parts[0],
                        Category = parts[1],
                        CaloriesPer100g = decimal.Parse(parts[2], CultureInfo.InvariantCulture),
                        ProteinPer100g = decimal.Parse(parts[3], CultureInfo.InvariantCulture),
                        FatPer100g = decimal.Parse(parts[4], CultureInfo.InvariantCulture),
                        CarbsPer100g = decimal.Parse(parts[5], CultureInfo.InvariantCulture),
                        IsGlobal = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsApproved = true
                    };
                    products.Add(product);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing line {i + 1}: {ex.Message}");
                }
            }

            Console.WriteLine($"Found {products.Count} products. Importing...");

            using var transaction = context.Database.BeginTransaction();
            try
            {
                context.Products.AddRange(products);
                context.SaveChanges();
                transaction.Commit();
                Console.WriteLine($"Successfully imported {products.Count} products.");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"Error importing products: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
            }
        }
    }
}
