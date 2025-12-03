// <copyright file="RegisterUserCommand.cs" company="Muxluk/StayFit">
// Copyright (c) Muxluk/StayFit. All rights reserved.
// </copyright>

using FluentValidation.Results;
using MediatR;

namespace StayFit.BLL.Features.Auth.Register;

/// <summary>
/// Команда для реєстрації нового користувача.
/// </summary>
/// <param name="firstName">Ім'я користувача.</param>
/// <param name="lastName">Прізвище користувача.</param>
/// <param name="email">Електронна пошта користувача.</param>
/// <param name="password">Пароль користувача.</param>
/// <param name="dateOfBirth">Дата народження користувача.</param>
/// <param name="gender">Стать користувача.</param>
/// <param name="height">Зріст користувача.</param>
/// <param name="weight">Вага користувача.</param>
public record RegisterUserCommand(
    string firstName,
    string lastName,
    string email,
    string password,
    DateTime dateOfBirth,
    string gender,
    float height,
    float weight): IRequest<int>;
