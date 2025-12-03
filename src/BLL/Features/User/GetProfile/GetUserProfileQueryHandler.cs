// <copyright file="GetUserProfileQueryHandler.cs" company="Muxluk/StayFit">
// Copyright (c) Muxluk/StayFit. All rights reserved.
// </copyright>


using MediatR;
using Microsoft.EntityFrameworkCore;
using StayFit.DAL.Interfaces;

namespace StayFit.BLL.Features.User.GetProfile;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, GetUserProfileResult>
{
    private readonly IApplicationDbContext _context;

    public GetUserProfileQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GetUserProfileResult> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == request.UserId, cancellationToken);

            if (user == null)
            {
                return new GetUserProfileResult
                {
                    Success = false,
                    Message = "Користувача не знайдено"
                };
            }

            return new GetUserProfileResult
            {
                Success = true,
                Message = "Профіль завантажено",
                Height = user.Height,
                CurrentWeight = user.CurrentWeight,
                TargetWeight = user.TargetWeight,
                ActivityLevel = user.ActivityLevel,
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth
            };
        }
        catch (Exception ex)
        {
            return new GetUserProfileResult
            {
                Success = false,
                Message = $"Помилка: {ex.Message}"
            };
        }
    }
}