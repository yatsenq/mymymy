// <copyright file="RegisterUserCommandHandler.cs" company="Muxluk/StayFit">
// Copyright (c) Muxluk/StayFit. All rights reserved.
// </copyright>

using MediatR;
using Microsoft.EntityFrameworkCore;
using StayFit.DAL.Entities;
using StayFit.DAL.Exceptions;
using StayFit.DAL.Interfaces;

namespace StayFit.BLL.Features.Auth.Register;

/// <summary>
/// Обробник команди для реєстрації нового користувача.
/// </summary>
public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, int>
{
    /// <summary>
    /// Контекст бази даних застосунку.
    /// </summary>
    private readonly IApplicationDbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterUserCommandHandler"/> class.
    /// </summary>
    /// <param name="context">Контекст бази даних.</param>
    public RegisterUserCommandHandler(IApplicationDbContext context)
    {
        this.context = context;
    }

    /// <inheritdoc/>
    public async Task<int> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // Перевірка чи email вже існує
        var emailExists = await this.context.Users
            .AnyAsync(u => u.Email == request.email, cancellationToken);

        if (emailExists)
        {
            throw new BusinessValidationException("User with this email already exists.");
        }

        // Створення сутності користувача
        var user = new StayFit.DAL.Entities.User
        {
            FirstName = request.firstName,
            LastName = request.lastName,
            Email = request.email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.password),
            DateOfBirth = DateTime.SpecifyKind(request.dateOfBirth, DateTimeKind.Utc),
            Gender = request.gender,
            Height = (decimal)request.height,
            CurrentWeight = (decimal)request.weight,
            ActivityLevel = "MODERATELY_ACTIVE",
            Role = "USER",
            CreatedAt = DateTime.UtcNow,
        };

        // Збереження в БД
        await this.context.Users.AddAsync(user, cancellationToken);
        await this.context.SaveChangesAsync(cancellationToken);

        return user.UserId;
    }
}
