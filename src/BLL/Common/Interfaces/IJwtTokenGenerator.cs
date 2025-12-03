// <copyright file="IJwtTokenGenerator.cs" company="Muxluk/StayFit">
// Copyright (c) Muxluk/StayFit. All rights reserved.
// </copyright>

using StayFit.DAL.Entities;

namespace StayFit.BLL.Common.Interfaces;

/// <summary>
/// Інтерфейс для генерації JWT-токенів.
/// </summary>
public interface IJwtTokenGenerator
{
    /// <summary>
    /// Генерує JWT-токен та термін його дії.
    /// </summary>
    /// <param name="user">Об'єкт користувача, для якого генерується токен.</param>
    /// <returns>Кортеж, що містить згенерований токен та дату його закінчення.</returns>
    (string token, DateTime expiresAt) GenerateToken(User user);
}
