// <copyright file="PasswordResetToken.cs" company="Muxluk/StayFit">
// Copyright (c) Muxluk/StayFit. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.DataAnnotations;

namespace StayFit.DAL.Entities;

/// <summary>
/// Сутність, що представляє токен для скидання пароля.
/// </summary>
public class PasswordResetToken
{
    /// <summary>Отримує або задає ідентифікатор токена.</summary>
    [Key]
    public int TokenId { get; set; }

    /// <summary>Отримує або задає ідентифікатор користувача.</summary>
    public int UserId { get; set; }

    /// <summary>Отримує або задає хеш токена.</summary>
    public string TokenHash { get; set; } = string.Empty;

    /// <summary>Отримує або задає час закінчення дії токена.</summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>Отримує або задає значення, яке вказує, чи був токен використаний.</summary>
    public bool IsUsed { get; set; } // FIX CA1805: Removed = false

    /// <summary>Отримує або задає кількість спроб використання токена.</summary>
    public int Attempts { get; set; } // FIX CA1805: Removed = 0

    /// <summary>Отримує або задає час створення токена.</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Отримує або задає навігаційну властивість до користувача.</summary>
    public User User { get; set; } = null!;
}