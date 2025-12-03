// <copyright file="LoginUserQueryHandler.cs" company="Muxluk/StayFit">
// Copyright (c) Muxluk/StayFit. All rights reserved.
// </copyright>

using MediatR;
using Microsoft.EntityFrameworkCore;
using StayFit.BLL.Common.Interfaces;
using StayFit.BLL.Features.Auth.Login.Dtos;
using StayFit.DAL.Entities;
using StayFit.DAL.Exceptions;
using StayFit.DAL.Interfaces;

namespace StayFit.BLL.Features.Auth.Login;

/// <summary>
/// Обробник запиту для входу користувача.
/// </summary>
public class LoginUserQueryHandler : IRequestHandler<LoginUserQuery, AuthResponseDto>
{
    private readonly IApplicationDbContext context;
    private readonly IJwtTokenGenerator jwtTokenGenerator;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginUserQueryHandler"/> class.
    /// </summary>
    /// <param name="context">Контекст бази даних.</param>
    /// <param name="jwtTokenGenerator">Генератор JWT-токенів.</param>
    public LoginUserQueryHandler(IApplicationDbContext context, IJwtTokenGenerator jwtTokenGenerator)
    {
        this.context = context;
        this.jwtTokenGenerator = jwtTokenGenerator;
    }

    /// <inheritdoc/>
    public async Task<AuthResponseDto> Handle(LoginUserQuery request, CancellationToken cancellationToken)
    {
        var user = await this.context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == request.email, cancellationToken);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.password, user.PasswordHash))
        {
            throw new AuthenticationFailedException("Invalid email or password.");
        }

        var (token, expiresAt) = this.jwtTokenGenerator.GenerateToken(user);

        return new AuthResponseDto(user.UserId, token, expiresAt);
    }
}
