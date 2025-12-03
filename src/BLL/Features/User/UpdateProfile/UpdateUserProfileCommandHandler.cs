// <copyright file="UpdateUserProfileCommandHandler.cs" company="Muxluk/StayFit">
// Copyright (c) Muxluk/StayFit. All rights reserved.
// </copyright>

// <copyright file="UpdateUserProfileCommandHandler.cs" company="Muxluk/StayFit">
// Copyright (c) Muxluk/StayFit. All rights reserved.
// </copyright>
using MediatR;
using Microsoft.EntityFrameworkCore;
using StayFit.DAL.Interfaces;

namespace StayFit.BLL.Features.User.UpdateProfile;

public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, UpdateUserProfileResult>
{
    private readonly IApplicationDbContext _context;

    public UpdateUserProfileCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UpdateUserProfileResult> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[UpdateProfile] START - UserId: {request.UserId}");

        try
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == request.UserId, cancellationToken);

            if (user == null)
            {
                Console.WriteLine($"[UpdateProfile] ERROR - User not found");
                return new UpdateUserProfileResult
                {
                    Success = false,
                    Message = "Користувача не знайдено"
                };
            }

            Console.WriteLine($"[UpdateProfile] User found: {user.Email}");
            Console.WriteLine($"[UpdateProfile] Old values - Height: {user.Height}, Weight: {user.CurrentWeight}, TargetWeight: {user.TargetWeight}");

            // Оновлення даних
            if (request.Height.HasValue)
            {
                Console.WriteLine($"[UpdateProfile] Updating Height: {user.Height} -> {request.Height.Value}");
                user.Height = request.Height.Value;
            }

            if (request.CurrentWeight.HasValue)
            {
                Console.WriteLine($"[UpdateProfile] Updating Weight: {user.CurrentWeight} -> {request.CurrentWeight.Value}");
                user.CurrentWeight = request.CurrentWeight.Value;
            }

            if (request.TargetWeight.HasValue)
            {
                Console.WriteLine($"[UpdateProfile] Updating TargetWeight: {user.TargetWeight} -> {request.TargetWeight.Value}");
                user.TargetWeight = request.TargetWeight.Value;
            }

            if (!string.IsNullOrEmpty(request.ActivityLevel))
            {
                Console.WriteLine($"[UpdateProfile] Updating ActivityLevel: {user.ActivityLevel} -> {request.ActivityLevel}");
                user.ActivityLevel = request.ActivityLevel;
            }

            Console.WriteLine($"[UpdateProfile] Saving changes...");
            var savedCount = await _context.SaveChangesAsync(cancellationToken);
            Console.WriteLine($"[UpdateProfile] Saved {savedCount} changes");

            // Розрахунок метрик
            var metrics = CalculateMetrics(user);
            Console.WriteLine($"[UpdateProfile] Calculated metrics - BMI: {metrics.Bmi}, BMR: {metrics.Bmr}, TDEE: {metrics.Tdee}");

            return new UpdateUserProfileResult
            {
                Success = true,
                Message = "Профіль успішно оновлено!",
                Bmi = metrics.Bmi,
                Bmr = metrics.Bmr,
                Tdee = metrics.Tdee
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[UpdateProfile] EXCEPTION: {ex.Message}");
            Console.WriteLine($"[UpdateProfile] StackTrace: {ex.StackTrace}");
            return new UpdateUserProfileResult
            {
                Success = false,
                Message = $"Помилка: {ex.Message}"
            };
        }
    }

    private static (decimal? Bmi, decimal? Bmr, decimal? Tdee) CalculateMetrics(DAL.Entities.User user)
    {
        if (!user.Height.HasValue || !user.CurrentWeight.HasValue)
            return (null, null, null);

        // BMI = вага(кг) / (зріст(м))^2
        var heightInMeters = user.Height.Value / 100;
        var bmi = user.CurrentWeight.Value / (heightInMeters * heightInMeters);

        // BMR (Mifflin-St Jeor)
        decimal bmr = 0;
        var age = DateTime.UtcNow.Year - user.DateOfBirth.Year;

        if (user.Gender == "MALE")
        {
            bmr = (10 * user.CurrentWeight.Value) + (6.25m * user.Height.Value) - (5 * age) + 5;
        }
        else // FEMALE
        {
            bmr = (10 * user.CurrentWeight.Value) + (6.25m * user.Height.Value) - (5 * age) - 161;
        }

        // TDEE = BMR * activity multiplier
        var activityMultiplier = user.ActivityLevel switch
        {
            "SEDENTARY" => 1.2m,
            "LIGHTLY_ACTIVE" => 1.375m,
            "MODERATELY_ACTIVE" => 1.55m,
            "VERY_ACTIVE" => 1.725m,
            "EXTRA_ACTIVE" => 1.9m,
            _ => 1.2m
        };

        var tdee = bmr * activityMultiplier;

        return (Math.Round(bmi, 1), Math.Round(bmr, 0), Math.Round(tdee, 0));
    }
}
