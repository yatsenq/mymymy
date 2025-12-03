// <copyright file="AuthResponseDto.cs" company="Muxluk/StayFit">
// Copyright (c) Muxluk/StayFit. All rights reserved.
// </copyright>

namespace StayFit.BLL.Features.Auth.Login.Dtos;

/// <summary>
/// Відповідь, що містить інформацію про автентифікацію.
/// </summary>
/// <param name="userId">Ідентифікатор користувача.</param>
/// <param name="token">Згенерований JWT-токен.</param>
/// <param name="expiresAt">Час закінчення дії токена.</param>
public record AuthResponseDto(
    int userId,
    string token,
    DateTime expiresAt);
