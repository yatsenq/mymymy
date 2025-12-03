// <copyright file="Program.cs" company="Muxluk/StayFit">
// Copyright (c) Muxluk/StayFit. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StayFit.DAL.Context;
using StayFit.DAL.Seed;
using System;
using System.IO;
using StayFit.Database;

public sealed class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== StayFit Database Connection Test ===");
        Console.WriteLine();

        // --- 1. Try to locate appsettings.json automatically ---
        string baseDir = AppContext.BaseDirectory;
        string[] possiblePaths =
        {
            Path.Combine(baseDir, "..", "..", "..", "appsettings.json"),              // src/database/
            Path.Combine(baseDir, "..", "..", "..", "..", "DAL", "appsettings.json") // src/DAL/
        };

        string? configPath = null;
        foreach (var path in possiblePaths)
        {
            var fullPath = Path.GetFullPath(path);
            if (File.Exists(fullPath))
            {
                configPath = fullPath;
                break;
            }
        }

        if (configPath == null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ERROR: appsettings.json not found in database/ or DAL/ folders.");
            Console.ResetColor();
            return;
        }

        Console.WriteLine($"Using config file: {configPath}");

        // --- 2. Build configuration ---
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.GetDirectoryName(configPath)!)
            .AddJsonFile(Path.GetFileName(configPath), optional: false, reloadOnChange: true)
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ERROR: Connection string 'DefaultConnection' not found.");
            Console.ResetColor();
            return;
        }

        // --- 3. Configure services and DbContext ---
        var services = new ServiceCollection();
        services.AddDbContext<StayFitDbContext>(options => options.UseNpgsql(connectionString));
        var provider = services.BuildServiceProvider();

        // --- 4. Check database connection ---
        using var context = provider.GetRequiredService<StayFitDbContext>();
        Console.WriteLine("Checking database connection...");

        if (context.Database.CanConnect())
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Connection established successfully.");
            Console.ResetColor();

            context.Database.EnsureCreated();

            try
            {
                DbInitializer.Seed(context);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Test data seeded successfully.");

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Warning: Error during seeding: {ex.Message}");
            }

            // Import products
            try 
            {
                if (!context.Products.Any())
                {
                    string csvPath = Path.Combine(AppContext.BaseDirectory, "products.csv");
                    // Check if csv exists in source dir if not in bin
                    if (!File.Exists(csvPath))
                    {
                            csvPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "products.csv");
                    }
                    
                    if (File.Exists(csvPath))
                    {
                        StayFit.Database.ProductImporter.Import(context, csvPath);
                    }
                    else
                    {
                        Console.WriteLine("products.csv not found, skipping import.");
                    }
                }
                else
                {
                    Console.WriteLine("Products already exist, skipping import.");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error during product import: {ex.Message}");
            }

            // Verification
            var productCount = context.Products.Count();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nVerification: Total products in database: {productCount}");
            if (productCount > 0)
            {
                var firstProduct = context.Products.First();
                Console.WriteLine($"Sample Product: {firstProduct.Name} ({firstProduct.CaloriesPer100g} kcal)");
            }
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Failed to connect to PostgreSQL.");
        }

        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
