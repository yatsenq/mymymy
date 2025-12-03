// <copyright file="LoginUserQuery.cs" company="Muxluk/StayFit">
// Copyright (c) Muxluk/StayFit. All rights reserved.
// </copyright>

using MediatR;
using StayFit.BLL.Features.Auth.Login.Dtos;

namespace StayFit.BLL.Features.Auth.Login;

/// <summary>
/// Запит для входу користувача в систему.
/// </summary>
/// <param name="email">Електронна пошта користувача.</param>
/// <param name="password">Пароль користувача.</param>
public record LoginUserQuery(
    string email,
    string password) : IRequest<AuthResponseDto>;
