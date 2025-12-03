// <copyright file="GetUserProfileQuery.cs" company="Muxluk/StayFit">
// Copyright (c) Muxluk/StayFit. All rights reserved.
// </copyright>

using MediatR;

namespace StayFit.BLL.Features.User.GetProfile;

public class GetUserProfileQuery : IRequest<GetUserProfileResult>
{
    public int UserId { get; set; }
}

public class GetUserProfileResult
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public decimal? Height { get; set; }

    public decimal? CurrentWeight { get; set; }

    public decimal? TargetWeight { get; set; }

    public string? ActivityLevel { get; set; }

    public string? Gender { get; set; }

    public DateTime DateOfBirth { get; set; }
}
