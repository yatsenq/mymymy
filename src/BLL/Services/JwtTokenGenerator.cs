// <copyright file="JwtTokenGenerator.cs" company="Muxluk/StayFit">
// Copyright (c) Muxluk/StayFit. All rights reserved.
// </copyright>

using System;
using StayFit.BLL.Common.Interfaces;
using StayFit.DAL.Entities;

namespace StayFit.BLL.Services
{
    /// <summary>
    /// Генератор JWT-токенів.
    /// </summary>
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        /// <inheritdoc/>
        public (string token, DateTime expiresAt) GenerateToken(User user)
        {
            // Тимчасова заглушка - повертаємо простий токен
            var token = $"temp_token_{user.UserId}_{Guid.NewGuid()}";
            var expiresAt = DateTime.UtcNow.AddDays(7);

            return (token, expiresAt);
        }
    }
}
