// <copyright file="UpdateUserProfileCommand.cs" company="Muxluk/StayFit">
// Copyright (c) Muxluk/StayFit. All rights reserved.
// </copyright>


using MediatR;

namespace StayFit.BLL.Features.User.UpdateProfile;

public class UpdateUserProfileCommand : IRequest<UpdateUserProfileResult>
{
    public int UserId { get; set; }
    public decimal? Height { get; set; }
    public decimal? CurrentWeight { get; set; }
    public decimal? TargetWeight { get; set; }
    public string? ActivityLevel { get; set; }
}

public class UpdateUserProfileResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public decimal? Bmi { get; set; }
    public decimal? Bmr { get; set; }
    public decimal? Tdee { get; set; }
}
